using Bumblebee.Setup;
using LyndaCoursesDownloader.CourseContent;
using LyndaCoursesDownloader.CourseElements;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LyndaCoursesDownloader.CourseExtractor
{
    public static class Extractor
    {
        public delegate void ExtractionProgressChangedEventHandler();
        public static event ExtractionProgressChangedEventHandler ExtractionProgressChanged;
        private static Session _session;
        private static CoursePage _coursePage;
        private static List<Video> _allVideos;
        private static Course _course;
        private static object _statusLock = new object();
        private static Task _initializationTask;

        public static Task InitializeDriver(Browser selectedBrowser)
        {
            _initializationTask = new Task(() =>
            {
                //KillDrivers();
                switch (selectedBrowser)
                {
                    case Browser.Firefox:
                        _session = new Session<CustomFirefox>();
                        break;
                    case Browser.Chrome:
                        _session = new Session<CustomChrome>();
                        break;
                }
                _session.NavigateTo<AboutPage>("https://www.lynda.com/aboutus/"); //used about page for quicker loading
                _session.Driver.Manage().Cookies.DeleteAllCookies();
            });
            _initializationTask.Start();
            return _initializationTask;
        }

        public static Task Login(string token, string courseUrl)
        {
            return _initializationTask.ContinueWith((t) =>
            {
                _session.Driver.Manage().Cookies.AddCookie(new Cookie("token", token, "www.lynda.com", "/", new DateTime(2222, 1, 1)));

                _coursePage = _session.NavigateTo<CoursePage>(courseUrl);
                if (_session.Driver.PageSource.Contains("submenu-account"))
                {
                    _session.Driver.Manage().Cookies.DeleteAllCookies();
                    _session.Driver.Manage().Cookies.AddCookie(new Cookie("token", token, "www.lynda.com", "/", new DateTime(2222, 1, 1)));
                    _session.NavigateTo<CoursePage>(courseUrl);
                }
                else
                {
                    throw new InvalidTokenException();
                }
            });
        }

        public static void ExtractCourseStructure(out int numberOfVideos)
        {
            _course = ExtractCourseStructure();
            _allVideos = _course.Chapters.SelectMany(ch => ch.Videos).ToList();
            numberOfVideos = _allVideos.Count;
        }
        private static Course ExtractCourseStructure()
        {
            Course course = new Course()
            {
                Name = _coursePage.CourseName,
                Chapters = new List<Chapter>()
            };
            int i = 1;
            foreach (var chapterBlock in _coursePage.ChapterBlocks)
            {
                chapterBlock.ChapterId = i;
                course.Chapters.Add(chapterBlock);
                i++;
            }

            return course;
        }

        public static Course ExtractCourse(Quality selectedQuality)
        {
            bool _isFirstVideo = true;
            WebDriverWait wait = new WebDriverWait(_session.Driver, TimeSpan.FromSeconds(30));
            Video video = _allVideos.GetAvailableVideo(_statusLock);
            _session.NavigateTo<CoursePage>(video.VideoUrl);
            Video nextVideo = _allVideos.GetAvailableVideo(_statusLock);
            while (!(video is null))
            {
                if (nextVideo is null)
                {
                    if (_isFirstVideo)
                    {
                        _isFirstVideo = false;
                        ExtractVideo(video, wait, selectedQuality);
                    }
                    else
                    {
                        ExtractVideo(video, wait);
                    }
                    break;
                }
                else
                {
                    if (_isFirstVideo)
                    {
                        _isFirstVideo = false;
                        ExtractVideo(video, wait, selectedQuality, nextVideo);
                    }
                    else
                    {
                        ExtractVideo(video, wait, null, nextVideo);
                    }
                    video = nextVideo;
                    nextVideo = _allVideos.GetAvailableVideo(_statusLock);
                }

            }

            return _course;
        }

        private static void ExtractVideo(Video video, WebDriverWait wait, Quality? selectedQuality = null, Video nextVideo = null)
        {
            _session.Driver.SwitchTo().Window(_session.Driver.WindowHandles.First());
            if (!(nextVideo is null))
            {
                _session.ExecuteJavaScript($"window.open('{nextVideo.VideoUrl}','_blank');");
                _session.Driver.SwitchTo().Window(_session.Driver.WindowHandles.First());
            }
            Retry.Do(() =>
            {
                var videoBlock = _session.CurrentPage<CoursePage>().VideoBlock;
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("banner-play")));
                videoBlock.VideoId = video.Id;
                videoBlock.WatchVideoButton.Click();

                if (!(selectedQuality is null))
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("player-settings")));
                    videoBlock.QualitySettings.Click();
                    switch (selectedQuality)
                    {
                        case Quality.Low:
                            videoBlock.Quality360.Click();
                            break;
                        case Quality.Medium:
                            videoBlock.Quality540.Click();
                            break;
                        case Quality.High:
                            videoBlock.Quality720.Click();
                            break;
                    }
                }
                video.VideoDownloadUrl = videoBlock.VideoDownloadUrl;
                var captionPage = _session.NavigateTo<CaptionsPage>(videoBlock.CaptionElement.GetAttribute("src"));
                video.CaptionText = captionPage.CaptionText;
                _session.Driver.Close();
                ExtractionProgressChanged();
                Monitor.Enter(_statusLock);
                video.CurrentVideoStatus = CurrentStatus.Finished;
                Monitor.Exit(_statusLock);
            },
            exceptionMessage: "An error occured while extracting video with title " + video.Name,
            actionOnError: () =>
            {
                _session.NavigateTo<CoursePage>(video.VideoUrl);
            });
        }
        public static void KillDrivers()
        {
            if (!(_session is null))
            {
                _session.Driver.Quit();
                _session = null;
                _coursePage = null;
                _allVideos = null;
                _course = null;
                _initializationTask = null;
                _statusLock = new object();
            }
            Process[] geckodriverProcesses = Process.GetProcessesByName("geckodriver");
            foreach (var geckodriverProcess in geckodriverProcesses)
            {
                geckodriverProcess.KillTree();
            }
            Process[] chromedriverProcesses = Process.GetProcessesByName("chromedriver");
            foreach (var chromedriverProcess in chromedriverProcesses)
            {
                chromedriverProcess.KillTree();
            }
        }
        public static void CloseTabs()
        {
            var windows = _session.Driver.WindowHandles;
            int windowsLeft = windows.Count;
            foreach (var window in windows)
            {
                if (windowsLeft == 1)
                {
                    return;
                }
                else
                {
                    _session.Driver.SwitchTo().Window(window);
                    _session.Driver.Close();
                    windowsLeft--;
                }
            }
        }
    }
}
