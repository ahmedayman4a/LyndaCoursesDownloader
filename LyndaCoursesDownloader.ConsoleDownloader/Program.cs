using LyndaCoursesDownloader.CourseContent;
using LyndaCoursesDownloader.CourseExtractor;
using LyndaCoursesDownloader.DownloaderConfig;
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
                    Console.WriteLine(TUI.continueGlyph + "Logging in...");
                    Extractor.InitializeDriver(config.Browser);
                    try
                    {
                        Extractor.Login(config.AuthenticationToken, courseUrl).Wait();
                        Console.WriteLine(TUI.continueGlyph + "Logged in successfully");
                        Console.WriteLine(TUI.endGlyph + "Intializing Course Extractor");
                        Course course = new Course();
                        int videosCount;
                        Extractor.ExtractCourseStructure(out videosCount);
                        using (pbarExtractor = new ProgressBar(videosCount, "Extracting Course Links - This will take some time", optionPbarExtractor))
                        {
                            Extractor.ExtractionProgressChanged += Extractor_ExtractionProgressChanged;
                            course = Extractor.ExtractCourse(config.Quality);
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
                        return;
                    }
                    catch (Exception ex)
                    {
                        TUI.ShowError("An error occured while extracting the course data : " + ex.Message);
                        TUI.ShowError("Error details : " + ex.StackTrace);
                        TUI.ShowError("Trying again...");
                        RunWithConfig(config);
                        return;
                    }


                }
                catch (WebDriverException ex)
                {
                    TUI.ShowError("An error occured in the driver : " + ex.Message);
                    TUI.ShowError("Error details : " + ex.StackTrace);
                    TUI.ShowError("Trying again...");
                    RunWithConfig(config);
                    return;
                }
                catch (Exception ex)
                {
                    TUI.ShowError("An error occured : " + ex.Message);
                    TUI.ShowError("Error details : " + ex.StackTrace);
                    RunWithConfig(config);
                    return;
                }

            }
        }

        private static void RunWithoutConfig()
        {
            while (true)
            {
                try
                {
                    Browser selectedBrowser = TUI.GetBrowser();
                    Extractor.InitializeDriver(selectedBrowser);
                    string courseUrl = TUI.GetCourseUrl();

                    while (true)
                    {
                        try
                        {
                            string token = TUI.GetLoginToken();
                            var loginTask = Extractor.Login(token, courseUrl);
                            var courseRootDirectory = TUI.GetPath();
                            var selectedQuality = TUI.GetQuality();
                            Console.WriteLine(TUI.continueGlyph + "Logging in...");
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
                            Extractor.ExtractCourseStructure(out int videosCount);
                            using (pbarExtractor = new ProgressBar(videosCount, "Extracting Course Links - This will take some time", optionPbarExtractor))
                            {
                                Extractor.ExtractionProgressChanged += Extractor_ExtractionProgressChanged;
                                course = Extractor.ExtractCourse(selectedQuality);
                            }

                            CourseDownloader.DownloadCourse(course, courseRootDirectory);
                            return;
                        }
                        catch (InvalidTokenException)
                        {
                            TUI.ShowError("The token you supplied is invalid - Login Failed");
                        }
                    }
                }



                catch (WebDriverException ex)
                {
                    TUI.ShowError("An error occured in the driver : " + ex.Message);
                    TUI.ShowError("Error details : " + ex.StackTrace);
                    TUI.ShowError("Trying again...");
                    RunWithoutConfig();
                    return;
                }
                catch (Exception ex)
                {
                    TUI.ShowError("An error occured : " + ex.Message);
                    TUI.ShowError("Error details : " + ex.StackTrace);
                    RunWithoutConfig();
                    return;
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
