using LyndaCoursesDownloader.CourseContent;
using LyndaCoursesDownloader.CourseExtractor;
using LyndaCoursesDownloader.DownloaderConfig;
using Newtonsoft.Json;
using OpenQA.Selenium;
using Serilog;
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
            AppDomain.CurrentDomain.UnhandledException += AllUnhandledExceptions;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("./logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
                .CreateLogger();
            Intro();
            Console.Title = "Lynda Courses Downloader";
            RunApp();

            Console.WriteLine();
            Console.ReadLine();
        }

        private static void RunApp()
        {
            if (File.Exists("./Config.json"))
            {
                Console.WriteLine(TUI.startGlyph + "Found a Config file");
                try
                {
                    Config config = Config.FromJson(File.ReadAllText("./Config.json"));
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
            string courseUrl = TUI.GetCourseUrl();
            Log.Information("Logging in...");
            Console.WriteLine(TUI.continueGlyph + "Logging in...");
            try
            {
                Extractor.InitializeDriver(config.Browser);
                Extractor.Login(config.AuthenticationToken, courseUrl).Wait();
            }
            catch (InvalidTokenException ex)
            {
                Log.Error(ex, "Failed to log in with course url : {0} and token of {1} characters", courseUrl, config.AuthenticationToken.Length);
                TUI.ShowError("The token or the course url you provided is invalid. Please make sure you entered the right token and course url");
                RunWithoutConfig();
                return;
            }
            catch (Exception e) when (e.InnerException is InvalidTokenException ex)
            {
                Log.Error(ex, "Failed to log in with course url : {0} and token of {1} characters", courseUrl, config.AuthenticationToken.Length);
                TUI.ShowError("The token or the course url you provided is invalid. Please make sure you entered the right token and course url");
                RunWithoutConfig();
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unknown Exception");
                TUI.ShowError("Unknown Error occured - " + ex.Message);
                RunWithoutConfig();
                return;
            }
            Log.Information("Logged in successfully");
            Console.WriteLine(TUI.continueGlyph + "Logged in successfully");
            Log.Information("Intializing Course Extractor");
            Console.WriteLine(TUI.endGlyph + "Intializing Course Extractor");
            Course course = ExtractCourse(config);
            if (course is null)
            {
                RunWithConfig(config);
                return;
            }
            Log.Information("Course Extracted. Downloading...");
            Console.WriteLine();
            CourseDownloader.DownloadCourse(course, config.CourseDirectory);
        }

        private static void RunWithoutConfig()
        {
            Browser selectedBrowser = TUI.GetBrowser();
            Extractor.InitializeDriver(selectedBrowser);
            string courseUrl = TUI.GetCourseUrl();
            string token = TUI.GetLoginToken();
            var loginTask = Extractor.Login(token, courseUrl);
            var courseRootDirectory = TUI.GetPath();
            var selectedQuality = TUI.GetQuality();
            Log.Information("Logging in...");
            Console.WriteLine(TUI.continueGlyph + "Logging in...");
            try
            {
                loginTask.Wait();
            }
            catch (InvalidTokenException ex)
            {
                Log.Error(ex, "Failed to log in with course url : {0} and token of {1} characters", courseUrl, token.Length);
                TUI.ShowError("The token or the course url you provided is invalid. Please make sure you entered the right token and course url");
                RunWithoutConfig();
                return;
            }
            catch (Exception e) when (e.InnerException is InvalidTokenException ex)
            {
                Log.Error(ex, "Failed to log in with course url : {0} and token of {1} characters", courseUrl, token.Length);
                TUI.ShowError("The token or the course url you provided is invalid. Please make sure you entered the right token and course url");
                RunWithoutConfig();
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unknown Exception");
                TUI.ShowError("Unknown Error occured - " + ex.Message);
                RunWithoutConfig();
                return;
            }

            Log.Information("Logged in successfully");
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
            Log.Information("Saved entries to congig file. Intializing Course Extractor");
            Console.WriteLine(TUI.endGlyph + "Intializing Course Extractor");
            Course course = ExtractCourse(config);
            if (course is null)
            {
                RunWithoutConfig();
                return;
            }
            Log.Information("Course Extracted. Downloading...");
            Console.WriteLine();
            CourseDownloader.DownloadCourse(course, courseRootDirectory);
        }

        private static Course ExtractCourse(Config config)
        {
            Course course = new Course();
            Extractor.ExtractCourseStructure(out int videosCount);
            using (pbarExtractor = new ProgressBar(videosCount, "Extracting Course Links - This will take some time", optionPbarExtractor))
            {
                Retry.Do(
                    function: () =>
                    {
                        Log.Information("Extracting...");
                        Extractor.ExtractionProgressChanged += Extractor_ExtractionProgressChanged;
                        course = Extractor.ExtractCourse(config.Quality);
                    },
                    exceptionMessage: "An error occured while extracting the course",
                    actionOnError: () =>
                    {
                        Extractor.CloseTabs();
                        var progress = pbarExtractor.AsProgress<float>();
                        progress?.Report(0);
                        Extractor.ExtractionProgressChanged -= Extractor_ExtractionProgressChanged;
                    },
                    actionOnFatal: () =>
                    {
                        TUI.ShowError("Failed to extract course. You can find more info in the logs");
                        Log.Error("Unknown error occured. Running app again");
                        TUI.ShowError("Unknown error occured");
                        TUI.ShowError("Restarting...");
                        Extractor.KillDrivers();
                        Extractor.ExtractionProgressChanged -= Extractor_ExtractionProgressChanged;
                        course = null;
                    }
                );
            }

            return course;
        }

        private static void Extractor_ExtractionProgressChanged()
        {
            pbarExtractor.Tick();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Extractor.KillDrivers();
        }
        private static void AllUnhandledExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Log.Error(ex, "Unknown error occured. Running app again");
            Console.WriteLine();
            TUI.ShowError("Unknown error occured - " + ex.Message);
            TUI.ShowError("Restarting...");
            Extractor.ExtractionProgressChanged -= Extractor_ExtractionProgressChanged;
            RunApp();
        }
    }
}
