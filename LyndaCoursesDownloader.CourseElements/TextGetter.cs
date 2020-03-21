using OpenQA.Selenium;
using System;
using System.Diagnostics;

namespace LyndaCoursesDownloader.CourseElements
{
    public static class TextGetter
    {
        private const int timeout = 5;
        public static string GetText(Func<string> TextGetterMethod)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            string content = TextGetterMethod();
            while (string.IsNullOrWhiteSpace(content) && stopwatch.Elapsed < TimeSpan.FromSeconds(timeout))
            {
                content = TextGetterMethod();
            }
            if (stopwatch.Elapsed >= TimeSpan.FromSeconds(timeout))
            {
                throw new WebDriverTimeoutException("Searching for text in element has timed out after " + timeout + " seconds");
            }
            return content;
        }
    }
}
