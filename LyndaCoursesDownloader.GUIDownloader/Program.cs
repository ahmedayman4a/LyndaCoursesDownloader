using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LyndaCoursesDownloader.CourseElements;

namespace LyndaCoursesDownloader.GUIDownloader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "logs.txt" };
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;

            Application.Run(new MainForm());
        }
    }
}
