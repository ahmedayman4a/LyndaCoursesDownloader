using Bumblebee.Setup;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Reflection;

namespace LyndaCoursesDownloader.CourseExtractor
{
    public abstract class CustomEnviroment : IDriverEnvironment
    {
        public abstract IWebDriver CreateWebDriver();

        protected void FixDriverCommandExecutionDelay(IWebDriver driver)
        {

            try
            {
                PropertyInfo commandExecutorProperty = typeof(RemoteWebDriver).GetProperty("CommandExecutor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
                ICommandExecutor commandExecutor = (ICommandExecutor)commandExecutorProperty.GetValue(driver);

                FieldInfo remoteServerUriField = commandExecutor.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);

                if (remoteServerUriField == null)
                {
                    FieldInfo internalExecutorField = commandExecutor.GetType().GetField("internalExecutor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                    commandExecutor = (ICommandExecutor)internalExecutorField.GetValue(commandExecutor);
                    remoteServerUriField = commandExecutor.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);
                }

                if (remoteServerUriField != null)
                {
                    string remoteServerUri = remoteServerUriField.GetValue(commandExecutor).ToString();

                    string localhostUriPrefix = "http://localhost";

                    if (remoteServerUri.StartsWith(localhostUriPrefix))
                    {
                        remoteServerUri = remoteServerUri.Replace(localhostUriPrefix, "http://127.0.0.1");

                        remoteServerUriField.SetValue(commandExecutor, new Uri(remoteServerUri));
                    }
                }
            }
            catch (TargetException ex)
            {
                if (ex.Message.Contains("Non-static method requires a target"))
                {
                    FixDriverCommandExecutionDelay(driver);
                }
                else
                {
                    throw;
                }

            }



        }
    }
}
