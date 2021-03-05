using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LyndaCoursesDownloader.CourseContent;
using Microsoft.CSharp;
using Newtonsoft.Json;

namespace LyndaCoursesDownloader.CourseExtractor
{
    public class Extractor
    {
        //62dc0343-88fe-4f00-aa70-5771a84a13bd,7bb812d70efe55023b423d0d445262d4,AaBET0MilOE8myV9Gu+d97z8j0ptSlYJHJ1KAT4kfK+2MLNamLs+qv43qT/r+NjX38eELUIdKPOx4A49iMC5nVtmA1ib0zX3F2F4IkVd3vd9PTX6h9QFjd3aTeFvr863Zreo1M7L5JqO+meXxE05lA==
        public delegate void LinksExtractionEventHandler();
        public event LinksExtractionEventHandler LinksExtractionFinished;
        private readonly string _token;
        private readonly Quality _quality;
        private readonly Curl _curl;
        private readonly bool _downloadExerciseFiles;
        private string _courseUrl;
        private int _courseId;

        public Extractor(string courseUrl, Quality quality, string token, bool downloadExerciseFiles)
        {
            _courseUrl = courseUrl;
            _quality = quality;
            _token = token;
            _downloadExerciseFiles = downloadExerciseFiles;
            _curl = new Curl(_token);
        }

        public static string ExtractToken(Browser browser)
        {
            var cookieExtractor = new CookiesExtractor("www.lynda.com");
            List<Cookie> cookies;
            switch (browser)
            {
                case Browser.Chrome:
                    cookies = cookieExtractor.ReadChromeCookies();
                    break;
                case Browser.Firefox:
                    cookies = cookieExtractor.ReadFirefoxCookies();
                    break;
                case Browser.Edge:
                    cookies = cookieExtractor.ReadEdgeCookies();
                    break;
                default:
                    throw new ArgumentException("browser");
            }
            if (cookies.Where(c => c.Name == "token").Count() != 0)
            {
                return cookies.Where(c => c.Name == "token").First().Value;
            }
            return null;
        }

        public async Task<Course> GetCourse()
        {
            var course = Course.FromJson(await _curl.CurlRequest("https://www.lynda.com/ajax/player?courseId=" + _courseId + "&type=course"));
            if (_downloadExerciseFiles)
            {
                dynamic tagsHtml = JsonConvert.DeserializeObject(await _curl.CurlRequest("https://www.lynda.com/ajax/course/" + course.Id + "/0/getupselltabs")); //Returns the html Tag for exercise files and offline download tabs
                string exerciseFilesTag = tagsHtml["exercisetab"]; //Get the exercisetab Html tag as a string to extract the exercise file api link
                Regex pattern = new Regex(@"<a[^>]+?\/ajax\/(?<downloadUrl>(?:[^\/]+\/){2,6}\d+)[^>]*>");
                if (pattern.IsMatch(exerciseFilesTag))
                {
                    string exerciseFilesApiUrl = "https://www.lynda.com/ajax/" + pattern.Match(exerciseFilesTag).Groups["downloadUrl"].Value; //extract the exercise file api link
                    course.ExerciseFilesDownloadUrl = await _curl.GetCurlRedirectUrl(exerciseFilesApiUrl); //Get the actual download link by getting the redirect location
                }
            }
            await FillDownloadLinks(course);
            return course;
        }
        public async Task FillDownloadLinks(Course course)
        {
            List<string> linkArgumentsLines = new List<string>(), subtitleArgumentsLines = new List<string>();
            foreach (var chapter in course.Chapters)
            {
                foreach (var video in chapter.Videos)
                {
                    string linkArgument = String.Format("-X GET \"{0}\" -b token=\"{1}\" -: ", video.ApiUrl, _token);
                    string subtitleArgument = String.Format("-X GET \"{0}\" -b token=\"{1}\" -: ", video.SubtitlesUrl, _token);
                    if (linkArgumentsLines.Count() == 0)
                    {
                        linkArgumentsLines.Add(linkArgument);
                        subtitleArgumentsLines.Add(subtitleArgument);
                        continue;
                    }

                    string lastArgumentsLine = linkArgumentsLines.Last();
                    if (lastArgumentsLine.Length > 20000)
                    {
                        linkArgumentsLines.Add(linkArgument);
                    }
                    else
                    {
                        lastArgumentsLine += linkArgument;
                        linkArgumentsLines[linkArgumentsLines.Count() - 1] = lastArgumentsLine;
                    }
                    string lastSubtitlesLine = subtitleArgumentsLines.Last();
                    if (lastSubtitlesLine.Length > 20000)
                    {
                        subtitleArgumentsLines.Add(subtitleArgument);
                    }
                    else
                    {
                        lastSubtitlesLine += subtitleArgument;
                        subtitleArgumentsLines[subtitleArgumentsLines.Count() - 1] = lastSubtitlesLine;
                    }
                }
            }
            string linksResult = "";
            foreach (var linkArgumentsLine in linkArgumentsLines)
            {
                linksResult += await _curl.CurlCustomRequest(linkArgumentsLine.Remove(linkArgumentsLine.Length - 4));
            }

            if (linksResult.Contains("ERROR 429: Too Many"))
            {
                throw new TooManyRequestsException();
            }
            LinksExtractionFinished();
            string subtitlesResult = "";
            foreach (var subtitleArgumentsLine in subtitleArgumentsLines)
            {
                subtitlesResult += await _curl.CurlCustomRequest(subtitleArgumentsLine.Remove(subtitleArgumentsLine.Length - 4)) + Environment.NewLine;
            }

            var linksJson = Regex.Split(linksResult.Trim(), "(?<=[}]])").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            var allSubtitles = Regex.Split(subtitlesResult.Trim(), @"^1" + Environment.NewLine + @"|{ Status=""Not|nscript not found\"" }1" + Environment.NewLine, RegexOptions.Multiline).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            int i = 0;
            foreach (var chapter in course.Chapters)
            {
                foreach (var video in chapter.Videos)
                {
                    dynamic links = JsonConvert.DeserializeObject(linksJson[i]);
                    switch (_quality)
                    {
                        case Quality.High:
                            video.DownloadUrl = links[0].urls["720"];
                            break;
                        case Quality.Medium:
                            video.DownloadUrl = links[0].urls["540"];
                            break;
                        case Quality.Low:
                            video.DownloadUrl = links[0].urls["360"];
                            break;
                    }
                    if (allSubtitles[i].Contains("Found\", Message=\"Tra"))
                    {
                        video.Subtitles = null;
                    }
                    else
                    {
                        video.Subtitles = "1" + Environment.NewLine + allSubtitles[i].Trim();
                    }
                    i++;
                }
            }

        }

        public async Task<bool> HasValidToken()
        {
            string aboutusPage = await _curl.CurlRequest("https://www.lynda.com/aboutus/");
            if (aboutusPage.Contains("id=\"submenu-account\""))
            {
                return true;
            }
            return false;
        }
        public bool HasValidUrl()
        {
            if (!_courseUrl.Contains("https://") || !_courseUrl.Contains("http://"))
            {
                _courseUrl = "https://" + _courseUrl;
            }
            Regex patternCourseUrl = new Regex(@"https?:\/\/(?:www\.)?lynda\.com\/(?:[^\/]+\/){2,3}(?<courseId>\d+)(-2\.html|\/\d+)");

            if (patternCourseUrl.IsMatch(_courseUrl))
            {
                _courseId = int.Parse(patternCourseUrl.Match(_courseUrl).Groups["courseId"].Value);
                return true;
            }
            return false;
        }

    }
}
