using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LyndaCoursesDownloader.CourseExtractor
{
    public class CustomChrome : CustomEnviroment
    {
        public override IWebDriver CreateWebDriver()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string driverFileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "chromedriver.exe" : "chromedriver";
            var service = ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory(), driverFileName);
            service.SuppressInitialDiagnosticInformation = true;
            var chromeOptions = new ChromeOptions();
            chromeOptions.SetLoggingPreference(LogType.Client, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Profiler, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Server, LogLevel.Off);
            chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager;
            chromeOptions.AddArguments("--start-maximized");
            //chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--log-level=OFF");
            chromeOptions.AddArgument("--mute-audio");
            chromeOptions.AddArguments("--blink-settings=imagesEnabled=false");
            service.HideCommandPromptWindow = true;
            IWebDriver driver;
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
