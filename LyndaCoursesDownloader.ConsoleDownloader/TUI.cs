using LyndaCoursesDownloader.CourseContent;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LyndaCoursesDownloader.ConsoleDownloader
{
    public static class TUI
    {
        public const string answerGlyph = "╠════════";
        public const string startGlyph = "╔══";
        public const string continueGlyph = "╠══";
        public const string endGlyph = "╚══";

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(continueGlyph + "[!!] " + message);
            Console.ResetColor();
        }

        public static bool UseConfig()
        {
            while (true)
            {
                Console.WriteLine(continueGlyph + "Would you like to use this configuration?");
                Console.WriteLine(continueGlyph + "1.Yes 2.No");
                Console.Write(answerGlyph);
                string answer = Console.ReadLine();
                switch (answer.Clean())
                {
                    case "yes":
                    case "y":
                    case "1.yes":
                    case "1":
                        return true;
                    case "no":
                    case "n":
                    case "2.n":
                    case "2":
                        return false;
                    default:
                        ShowError("The answer you entered isn't recognized");
                        ShowError("Please try again");
                        break;
                }
            }
        }
        public static DirectoryInfo GetPath()
        {
            while (true)
            {
                Console.WriteLine(continueGlyph + "Where do you want to download your course to?(ex:D:\\MyCourses)");
                Console.Write(answerGlyph);
                string pathToCourse = Console.ReadLine().Clean(false);
                if (!Directory.Exists(pathToCourse))
                {
                    ShowError("Provided directory doesn't exist");
                }
                else
                {
                    return new DirectoryInfo(pathToCourse);
                }
            }
        }

        public static Browser GetBrowser()
        {
            while (true)
            {
                Console.WriteLine(continueGlyph + "Which browser do you want the downloader to run on?");
                Console.WriteLine(continueGlyph + "Available Browsers : 1.Firefox (Recommended)  2.Chrome");
                Console.Write(answerGlyph);
                string browser = Console.ReadLine();
                switch (browser.Clean())
                {

                    case "1":
                    case "firefox":
                        return Browser.Firefox;
                    case "2":
                    case "chrome":
                        return Browser.Chrome;
                    default:
                        ShowError("The browser you entered isn't recognized");
                        ShowError("Please try again");
                        break;
                }
            }
        }

        public static string GetLoginToken()
        {

            string loginToken = "";
            while (string.IsNullOrEmpty(loginToken))
            {
                Console.WriteLine(continueGlyph + "What is the lynda security token?");
                Console.Write(answerGlyph);
                loginToken = Console.ReadLine().Clean(false);
            }
            return loginToken;
        }

        public static string GetCourseUrl()
        {
            while (true)
            {
                Console.WriteLine(continueGlyph + "What is the url of the course?");
                Console.Write(answerGlyph);
                string courseUrl = Console.ReadLine().Clean().Replace("?autoplay=true", "");
                if (Regex.IsMatch(courseUrl, @"^https?:\/\/(www\.)?lynda.com\/"))
                {
                    return courseUrl;
                }
                else
                {
                    ShowError("The supplied url isn't a valid lynda course url");
                }
            }
        }

        public static Quality GetQuality()
        {
            while (true)
            {
                Console.WriteLine(continueGlyph + "Which quality would you like the course to be downloaded in?");
                Console.WriteLine(continueGlyph + "Available Qualities : 1.360p 2.540p 3.720p");
                Console.Write(answerGlyph);
                string quality = Console.ReadLine();
                switch (quality.Clean())
                {

                    case "1":
                    case "360":
                    case "360p":
                        return Quality.Low;
                    case "2":
                    case "540":
                    case "540p":
                        return Quality.Medium;
                    case "3":
                    case "720":
                    case "720p":
                        return Quality.High;
                    default:
                        ShowError("The quality you entered isn't recognized");
                        ShowError("Please try again");
                        break;
                }
            }
        }

        private static string Clean(this string answer, bool toLower = true)
        {
            if (toLower)
            {
                return answer.ToLower().Replace(answerGlyph, "").Trim();
            }
            else
            {
                return answer.Replace(answerGlyph, "").Trim();
            }
        }

    }
}
