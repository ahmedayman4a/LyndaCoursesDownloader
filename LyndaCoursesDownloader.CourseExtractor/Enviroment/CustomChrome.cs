using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;

namespace LyndaCoursesDownloader.CourseExtractor
{
    public class CustomChrome : CustomEnviroment
    {
        public override IWebDriver CreateWebDriver()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var service = ChromeDriverService.CreateDefaultService("./");
            var chromeOptions = new ChromeOptions();
            chromeOptions.SetLoggingPreference(LogType.Client, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Profiler, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Server, LogLevel.Off);
            chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager;
            chromeOptions.AddArgument("-headless");
            chromeOptions.AddArgument("--log-level=OFF");
            chromeOptions.AddArgument("--mute-audio");
            service.HideCommandPromptWindow = true;
            IWebDriver driver = null;
            try
            {
                driver = new ChromeDriver(service, chromeOptions);
            }
            catch (WebDriverException)
            {
                return CreateWebDriver();
            }
            
            FixDriverCommandExecutionDelay(driver);


            return driver;
        }


    }
}
