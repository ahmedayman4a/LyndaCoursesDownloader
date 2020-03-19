using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Text;

namespace LyndaCoursesDownloader.CourseExtractor
{
    public class CustomFirefox : CustomEnviroment
    {
        public override IWebDriver CreateWebDriver()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var service = FirefoxDriverService.CreateDefaultService();

            var firefoxOptions = new FirefoxOptions
            {
                PageLoadStrategy = PageLoadStrategy.Eager,
            };
            //firefoxOptions.AddArgument("-headless");
            var firefoxProfile = new FirefoxProfile();
            firefoxProfile.SetPreference("media.volume_scale", "0.0");
            firefoxOptions.Profile = firefoxProfile;
            firefoxOptions.LogLevel = FirefoxDriverLogLevel.Fatal;
            service.HideCommandPromptWindow = true;

            IWebDriver driver = null;
            try
            {
                driver = new FirefoxDriver(service, firefoxOptions);
            }
            catch (WebDriverException)
            {
                CreateWebDriver();
            }
            
            FixDriverCommandExecutionDelay(driver);


            return driver;
        }


    }
}
