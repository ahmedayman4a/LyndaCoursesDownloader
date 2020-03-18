using LyndaCoursesDownloader.CourseContent;
using LyndaCoursesDownloader.CourseExtractor;
using Newtonsoft.Json;
using OpenQA.Selenium;
using ShellProgressBar;
using System;
using System.IO;

namespace LyndaCoursesDownloader.ConsoleDownloader
{
    class Program
    {
        private static ProgressBar pbarExtractor;
        private static readonly ProgressBarOptions optionPbarExtractor = new ProgressBarOptions
        {
            ScrollChildrenIntoView = true,
            ForegroundColor = ConsoleColor.Blue,
            ForegroundColorDone = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.Gray,
            ProgressBarOnBottom = true
        };
        static void Main(string[] args)
        {
            Intro();
            Console.Title = "Lynda Courses Downloader";
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            var config = new Config();
            if (File.Exists("./Config.json"))
            {
                Console.WriteLine(TUI.startGlyph + "Found a Config file");
                try
                {

                    config = Config.FromJson(File.ReadAllText("./Config.json"));
                    Console.WriteLine(TUI.continueGlyph + "Data in config file : ");
                    Console.WriteLine(TUI.continueGlyph + "Browser : " + config.Browser);
                    Console.WriteLine(TUI.continueGlyph + "Quality to download in : " + config.Quality);
                    Console.WriteLine(TUI.continueGlyph + "Course Directory/Path : " + config.CourseDirectory);
                    Console.WriteLine(TUI.continueGlyph + "Authentication Token : " + config.AuthenticationToken);
                    if (TUI.UseConfig())
                    {
                        RunWithConfig(config);
                    }
                    else
                    {
                        Console.WriteLine(TUI.continueGlyph + "The data you enter will be saved in a new Config file");
                        RunWithoutConfig();
                    }

                }
                catch (JsonSerializationException)
                {
                    TUI.ShowError("Config file is corrupt");
                    Console.WriteLine(TUI.continueGlyph + "The data you enter will be saved in a new Config file");
                    RunWithoutConfig();
                }

            }
            else
            {
                Console.WriteLine(TUI.startGlyph + "Config File not found");
                Console.WriteLine(TUI.continueGlyph + "The data you enter will be saved in a new Config file");
                RunWithoutConfig();

            }




            Console.WriteLine();
            Console.ReadLine();
        }

        private static void Intro()
        {
            Console.Write(@"
╔╗───────────╔╗───╔═══╗─────────────────╔═══╗──────────╔╗────────╔╗
║║───────────║║───║╔═╗║─────────────────╚╗╔╗║──────────║║────────║║
║║──╔╗─╔╦═╗╔═╝╠══╗║║─╚╬══╦╗╔╦═╦══╦══╦══╗─║║║╠══╦╗╔╗╔╦═╗║║╔══╦══╦═╝╠══╦═╗
║║─╔╣║─║║╔╗╣╔╗║╔╗║║║─╔╣╔╗║║║║╔╣══╣║═╣══╣─║║║║╔╗║╚╝╚╝║╔╗╣║║╔╗║╔╗║╔╗║║═╣╔╝
║╚═╝║╚═╝║║║║╚╝║╔╗║║╚═╝║╚╝║╚╝║║╠══║║═╬══║╔╝╚╝║╚╝╠╗╔╗╔╣║║║╚╣╚╝║╔╗║╚╝║║═╣║
╚═══╩═╗╔╩╝╚╩══╩╝╚╝╚═══╩══╩══╩╝╚══╩══╩══╝╚═══╩══╝╚╝╚╝╚╝╚╩═╩══╩╝╚╩══╩══╩╝
────╔═╝║
────╚══╝

█▀▄▀█ ▄▀█ █▀▄ █▀▀   █▄▄ █▄█   ▀   ▄▀█ █░█ █▀▄▀█ █▀▀ █▀▄ ▄▀█ █▄█ █▀▄▀█ ▄▀█ █▄░█ █░█ ▄▀█
█░▀░█ █▀█ █▄▀ ██▄   █▄█ ░█░   ▄   █▀█ █▀█ █░▀░█ ██▄ █▄▀ █▀█ ░█░ █░▀░█ █▀█ █░▀█ ▀▀█ █▀█

");
        }

        private static void RunWithConfig(Config config)
        {
            while (true)
            {
                try
                {

                    string courseUrl = TUI.GetCourseUrl();
                    var extractor = new Extractor();
                    var initializationTask = extractor.InitializeDriver(config.Browser);
                    initializationTask.Start();

                    try
                    {
                        extractor.Login(config.AuthenticationToken, courseUrl, initializationTask).Wait();
                        Console.WriteLine(TUI.continueGlyph + "Logged in successfully");
                        Console.WriteLine(TUI.endGlyph + "Intializing Course Extractor");
                        Course course = new Course();
                        int videosCount;
                        extractor.ExtractCourseStructure(out videosCount);
                        using (pbarExtractor = new ProgressBar(videosCount, "Extracting Course Links - This will take some time", optionPbarExtractor))
                        {
                            extractor.ExtractionProgressChanged += Extractor_ExtractionProgressChanged;
                            course = extractor.ExtractCourse(config.Quality);
                        }

                        CourseDownloader.DownloadCourse(course, config.CourseDirectory);
                        return;

                    }
                    catch (InvalidTokenException)
                    {
                        TUI.ShowError("The token you supplied is invalid - Login Failed");
                        Console.WriteLine("Creating new config file");
                        RunWithoutConfig();
                        return;
                    }
                    catch (WebDriverException ex)
                    {
                        TUI.ShowError("An error occured in the driver : " + ex.Message);
                        TUI.ShowError("Error details : " + ex.StackTrace);
                        TUI.ShowError("Trying again...");
                        RunWithConfig(config);
                    }
                    catch (Exception ex)
                    {
                        TUI.ShowError("An error occured while extracting the course data : " + ex.Message);
                        TUI.ShowError("Error details : " + ex.StackTrace);
                        TUI.ShowError("Trying again...");
                        RunWithConfig(config);
                    }


                }
                catch (WebDriverException ex)
                {
                    TUI.ShowError("An error occured in the driver : " + ex.Message);
                    TUI.ShowError("Error details : " + ex.StackTrace);
                    TUI.ShowError("Trying again...");
                    RunWithConfig(config);
                }
                catch (Exception ex)
                {
                    TUI.ShowError("An error occured : " + ex.Message);
                    TUI.ShowError("Error details : " + ex.StackTrace);
                    RunWithConfig(config);
                }

            }
        }

        private static void RunWithoutConfig()
        {
            while (true)
            {
                try
                {
                    var extractor = new Extractor();
                    Browser selectedBrowser = TUI.GetBrowser();
                    var initializationTask = extractor.InitializeDriver(selectedBrowser);
                    initializationTask.Start();
                    string courseUrl = TUI.GetCourseUrl();


                    try
                    {
                        string token = TUI.GetLoginToken();
                        var loginTask = extractor.Login(token, courseUrl, initializationTask);
                        var courseRootDirectory = TUI.GetPath();
                        var selectedQuality = TUI.GetQuality();
                        loginTask.Wait();
                        Console.WriteLine(TUI.continueGlyph + "Logged in successfully");
                        Config config = new Config
                        {
                            AuthenticationToken = token,
                            Browser = selectedBrowser,
                            Quality = selectedQuality,
                            CourseDirectory = courseRootDirectory
                        };
                        File.WriteAllText("./Config.json", config.ToJson());
                        Console.WriteLine(TUI.continueGlyph + "Saved entries to config file");
                        Console.WriteLine(TUI.endGlyph + "Intializing Course Extractor...");
                        Course course = new Course();
                        int videosCount;
                        extractor.ExtractCourseStructure(out videosCount);
                        using (pbarExtractor = new ProgressBar(videosCount, "Extracting Course Links - This will take some time", optionPbarExtractor))
                        {
                            extractor.ExtractionProgressChanged += Extractor_ExtractionProgressChanged;
                            course = extractor.ExtractCourse(selectedQuality);
                        }

                        CourseDownloader.DownloadCourse(course, courseRootDirectory);
                        return;
                    }
                    catch (InvalidTokenException)
                    {
                        TUI.ShowError("The token you supplied is invalid - Login Failed");
                    }
                    catch (WebDriverException ex)
                    {
                        TUI.ShowError("An error occured in the driver : " + ex.Message + "at");
                        TUI.ShowError("Error details : " + ex.StackTrace);
                        TUI.ShowError("Trying again...");
                        RunWithoutConfig();
                    }
                    catch (Exception ex)
                    {
                        TUI.ShowError("An error occured while extracting the course data : " + ex.Message);
                        TUI.ShowError("Error details : " + ex.StackTrace);
                        TUI.ShowError("Trying again...");
                    }
                }

                catch (WebDriverException ex)
                {
                    TUI.ShowError("An error occured in the driver : " + ex.Message);
                    TUI.ShowError("Error details : " + ex.StackTrace);
                    TUI.ShowError("Trying again...");
                    RunWithoutConfig();
                }
                catch (Exception ex)
                {
                    TUI.ShowError("An error occured : " + ex.Message);
                    TUI.ShowError("Error details : " + ex.StackTrace);
                    RunWithoutConfig();
                }
            }
        }

        private static void Extractor_ExtractionProgressChanged()
        {
            pbarExtractor.Tick();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Extractor.KillDrivers();
        }
    }
}
