using Bumblebee.Setup;
using LyndaCoursesDownloader.CourseContent;
using LyndaCoursesDownloader.CourseElements;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LyndaCoursesDownloader.CourseExtractor
{
    public class Extractor
    {
        public delegate void ExtractionProgressChangedEventHandler();
        public event ExtractionProgressChangedEventHandler ExtractionProgressChanged;
        private static int NumberOfSessions = 1; // ability to set number of sessions in the future
        private static Session[] Sessions = new Session[NumberOfSessions];
        private static CoursePage coursePage;
        private static List<Video> allVideos;
        private static Browser SelectedBrowser;
        private static Course course;
        private static object StatusLock = new object();

        public Task InitializeDriver(Browser selectedBrowser)
        {

            SelectedBrowser = selectedBrowser;
            return new Task(() =>
            {
                //KillDrivers();
                switch (selectedBrowser)
                {
                    case Browser.Firefox:
                        Parallel.For(0, NumberOfSessions, (i) =>
                        {
                            Sessions[i] = new Session<CustomFirefox>();
                        });
                        break;
                    case Browser.Chrome:
                        Parallel.For(0, NumberOfSessions, (i) =>
                        {
                            Sessions[i] = new Session<CustomChrome>();
                        });
                        break;
                }
                Parallel.For(0, NumberOfSessions, (i) =>
                {
                    Sessions[i].NavigateTo<AboutPage>("https://www.lynda.com/aboutus/"); //used about page for quicker loading
                });
                Sessions[0].Driver.Manage().Cookies.DeleteAllCookies();
            });
        }

        public Task Login(string token, string courseUrl, Task initializationTask)
        {
            return initializationTask.ContinueWith((t) =>
            {
                Sessions[0].Driver.Manage().Cookies.AddCookie(new Cookie("token", token, "www.lynda.com", "/", new DateTime(2222, 1, 1)));

                coursePage = Sessions[0].NavigateTo<CoursePage>(courseUrl);
                if (Sessions[0].Driver.PageSource.Contains("submenu-account"))
                {
                    Parallel.For(1, NumberOfSessions, (i) =>
                    {
                        Sessions[i].Driver.Manage().Cookies.DeleteAllCookies();
                        Sessions[i].Driver.Manage().Cookies.AddCookie(new Cookie("token", token, "www.lynda.com", "/", new DateTime(2222, 1, 1)));
                    });
                    Sessions[0].NavigateTo<CoursePage>(courseUrl);
                }
                else
                {
                    throw new InvalidTokenException();
                }
            });
        }

        public void ExtractCourseStructure(out int numberOfVideos)
        {
            course = ExtractCourseStructure();
            allVideos = course.Chapters.SelectMany(ch => ch.Videos).ToList();
            numberOfVideos = allVideos.Count;
        }
        private static Course ExtractCourseStructure()
        {
            Course course = new Course()
            {
                Name = coursePage.CourseName,
                Chapters = new List<Chapter>()
            };
            int i = 1;
            foreach (var chapterBlock in coursePage.ChapterBlocks)
            {
                chapterBlock.ChapterId = i;
                course.Chapters.Add(chapterBlock);
                i++;
            }

            return course;
        }

        public Course ExtractCourse(Quality selectedQuality)
        {
            Parallel.ForEach(Sessions, (session) =>
            {
                 bool _isFirstVideo = true;
                WebDriverWait wait = new WebDriverWait(session.Driver, TimeSpan.FromSeconds(30));
                Video video = allVideos.GetAvailableVideo(StatusLock);
                session.NavigateTo<CoursePage>(video.VideoUrl);
                Video nextVideo = allVideos.GetAvailableVideo(StatusLock);
                while (!(video is null))
                {
                    if (nextVideo is null)
                    {
                        if (_isFirstVideo)
                        {
                            _isFirstVideo = false;
                            ExtractVideo(video, session, wait, selectedQuality);
                        }
                        else
                        {
                            ExtractVideo(video, session, wait);
                        }
                        return;
                    }
                    else
                    {
                        if (_isFirstVideo)
                        {
                            _isFirstVideo = false;
                            ExtractVideo(video, session, wait, selectedQuality, nextVideo);
                        }
                        else
                        {
                            ExtractVideo(video, session, wait, null, nextVideo);
                        }
                        video = nextVideo;
                        nextVideo = allVideos.GetAvailableVideo(StatusLock);
                    }


                }
            });


            #region OldForeach


            //int j = 1;
            //foreach (var video in allVideos)
            //{
            //    session.NavigateTo<CoursePage>(video.VideoUrl);
            //    var videoBlock = session.CurrentPage<CoursePage>().VideoBlock;
            //    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("banner-play")));

            //    videoBlock.VideoId = video.Id;
            //    videoBlock.WatchVideoButton.Click();
            //    videoBlock.QualitySettings.Click();
            //    switch (selectedQuality)
            //    {
            //        case Quality.Low:
            //            videoBlock.Quality360.Click();
            //            break;
            //        case Quality.Medium:
            //            videoBlock.Quality540.Click();
            //            break;
            //        case Quality.High:
            //            videoBlock.Quality720.Click();
            //            break;
            //    }

            //    video.VideoDownloadUrl = videoBlock.VideoDownloadUrl;
            //    video.CaptionText = session.NavigateTo<CaptionsPage>(videoBlock.CaptionElement.GetAttribute("src")).CaptionText;
            //    ExtractionProgressChanged(j/allVideos.Count());
            //}
            #endregion
            return course;
        }

        private void ExtractVideo(Video video, Session session, WebDriverWait wait, Quality? selectedQuality = null, Video nextVideo = null)
        {
            session.Driver.SwitchTo().Window(session.Driver.WindowHandles.First());
            if (!(nextVideo is null))
                session.ExecuteJavaScript($"window.open('{nextVideo.VideoUrl}','_blank');");
            session.Driver.SwitchTo().Window(session.Driver.WindowHandles.First());
            var videoBlock = session.CurrentPage<CoursePage>().VideoBlock;
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
            video.CaptionText = session.NavigateTo<CaptionsPage>(videoBlock.CaptionElement.GetAttribute("src")).CaptionText;
            session.Driver.Close();
            ExtractionProgressChanged();
            Monitor.Enter(StatusLock);
            video.CurrentVideoStatus = CurrentStatus.Finished;
            Monitor.Exit(StatusLock);
        }
        public static void KillDrivers()
        {
            if (!(Sessions[0] is null))
            {
                Sessions[0].Driver.Quit();
            }
            switch (SelectedBrowser)
            {
                case Browser.Firefox:
                    Process[] geckodriverProcesses = Process.GetProcessesByName("geckodriver");
                    foreach (var geckodriverProcess in geckodriverProcesses)
                    {
                        geckodriverProcess.KillTree();
                    }
                    break;
                case Browser.Chrome:
                    Process[] chromedriverProcesses = Process.GetProcessesByName("chromedriver");
                    foreach (var chromedriverProcess in chromedriverProcesses)
                    {
                        chromedriverProcess.KillTree();
                    }
                    break;
            }

        }
    }
}
