using System;
using System.Windows.Forms;
using LyndaCoursesDownloader.DownloaderConfig;
using Serilog;
using Squirrel;

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
            AppDomain.CurrentDomain.UnhandledException += AllUnhandledExceptions;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("./logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
                .CreateLogger();
            using (var mgr = new UpdateManager("https://github.com/ahmedayman4a/LyndaCoursesDownloader.UpdateManager"))
            {
                SquirrelAwareApp.HandleEvents(
                  onInitialInstall: v => mgr.CreateShortcutForThisExe(),
                  onAppUpdate: v => mgr.CreateShortcutForThisExe(),
                  onAppUninstall: v => mgr.RemoveShortcutForThisExe());
            }
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Config.Restore();
            Application.Run(new MainForm());
        }

        private static void AllUnhandledExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Log.Error(ex, "Unknown error occured");

            Environment.Exit(System.Runtime.InteropServices.Marshal.GetHRForException(ex));
        }
    }
}
