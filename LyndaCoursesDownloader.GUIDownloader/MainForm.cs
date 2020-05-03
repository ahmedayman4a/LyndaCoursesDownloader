using LyndaCoursesDownloader.DownloaderConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LyndaCoursesDownloader.CourseExtractor;
using LyndaCoursesDownloader.CourseContent;
using Serilog;
using System.Threading;
using System.Diagnostics;
using Squirrel;
using System.Drawing.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace LyndaCoursesDownloader.GUIDownloader
{
    public partial class MainForm : Form
    {
        private readonly object ExtractionProgressLock = new object();
        private int _videosCount = 0;
        private Font _font;
        private int _currentVideoIndex = 0;
        public MainForm()
        {
            InitializeComponent();
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Text = "LyndaCoursesDownloader - v" + version;
            lblCurrentOperation.Text = "Waiting for input from user";
            UC_CourseExtractorStatus.Status = CourseStatus.NotRunning;
            UC_CourseDownloaderStatus.Status = CourseStatus.NotRunning;
            UC_IsLoggedin.IsLoggedin = false;
            cmboxBrowser.SelectedIndex = 0;
            cmboxQuality.SelectedIndex = 0;
            Config config;

            if (File.Exists("./Config.json"))
            {
                try
                {
                    config = Config.FromJson(File.ReadAllText("./Config.json"));
                    txtCourseDirectory.Text = config.CourseDirectory.FullName;
                    txtToken.Text = config.AuthenticationToken;
                    cmboxBrowser.SelectedIndex = (int)config.Browser;
                    cmboxQuality.SelectedIndex = (int)config.Quality;
                    Log.Information("Acquired data from config");
                }
                catch (JsonSerializationException ex)
                {
                    MessageBox.Show("Config file is corrupt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error(ex, "Config file is corrupt");
                }
            }
            else
            {
                Log.Information("No Config is found");
            }

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            txtCourseDirectory.Text = folderBrowserDialog.SelectedPath;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            int numberOfErrors = 0;
            StringBuilder sb = new StringBuilder();

            if (String.IsNullOrWhiteSpace(txtCourseUrl.Text))
            {
                sb.Append("• ");
                sb.AppendLine("Course Url");
                numberOfErrors++;
            }
            if (String.IsNullOrWhiteSpace(txtToken.Text))
            {
                sb.Append("• ");
                sb.AppendLine("Token Cookie");
                numberOfErrors++;
            }
            if (String.IsNullOrWhiteSpace(txtCourseDirectory.Text))
            {
                sb.Append("• ");
                sb.AppendLine("Course Location");
                numberOfErrors++;
            }

            if (numberOfErrors == 1)
            {
                sb.Insert(0, "Please don't leave the following field empty:\n");
                MessageBox.Show(sb.ToString(), "Fields can't be empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (numberOfErrors > 1)
            {
                sb.Insert(0, "Please don't leave the following fields empty:\n");
                MessageBox.Show(sb.ToString(), "Fields can't be empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(txtCourseUrl.Text, @"^https?:\/\/(www\.)?lynda\.com\/"))
            {
                MessageBox.Show("The course url you supplied is not a valid lynda.com course url", "Invalid Course url", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            UC_IsLoggedin.IsLoggedin = false;
            UC_CourseExtractorStatus.Status = CourseStatus.NotRunning;
            UC_CourseDownloaderStatus.Status = CourseStatus.NotRunning;
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateUI(() => EnableControls(false));
            SaveConfig();
            var course = ExtractCourse();
            if (course is null)
            {
                return;
            }
            var downloaderForm = new DownloaderForm(course, new DirectoryInfo(txtCourseDirectory.Text), _font);
            UpdateUI(() => UC_CourseDownloaderStatus.Status = CourseStatus.Running);
            try
            {
                downloaderForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An fatal error occured while downloading the course.\nCheck the logs for more info", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex, "An fatal error occured while downloading the course");
                Extractor.KillDrivers();
                UpdateUI(() =>
                {
                    UC_CourseDownloaderStatus.Status = CourseStatus.Failed;
                    lblCurrentOperation.Text = "Course Download Failed";
                });
            }

            UpdateUI(() =>
            {
                if (downloaderForm.DownloaderStatus == CourseStatus.Finished)
                {
                    UC_CourseDownloaderStatus.Status = CourseStatus.Finished;
                    MessageBox.Show("Course Downloaded Successfully :)", "Hooray", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblCurrentOperation.Text = "Course Downloaded Successfully";
                }
                else
                {
                    UC_CourseDownloaderStatus.Status = CourseStatus.Failed;
                    lblCurrentOperation.Text = "Course Download Failed";
                }
            });
        }

        private Course ExtractCourse()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Course course = new Course();
            UpdateUI(() => lblCurrentOperation.Text = "Logging in");
            Browser browser = GetFromUI<Browser>(() => cmboxBrowser.SelectedIndex);
            Log.Information("Logging in");
            Extractor.InitializeDriver(browser);
            try
            {
                Extractor.Login(txtToken.Text, txtCourseUrl.Text).Wait();
                Log.Information("Logged in with course url : {0} and token of {1} characters", GetFromUI<string>(() => txtCourseUrl.Text), GetFromUI<int>(() => txtToken.Text.Length));
                UpdateUI(() =>
                {
                    lblCurrentOperation.Text = "Logged in successfully";
                    UC_IsLoggedin.IsLoggedin = true;
                });
            }
            catch (InvalidTokenException ex)
            {
                MessageBox.Show("The token or the course url you provided is invalid.\nPlease make sure you entered the right token and course url", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUI(() =>
                {
                    EnableControls(true);
                    lblCurrentOperation.Text = "Login Failed";
                });
                Log.Error(ex, "Failed to log in with course url : {0} and token of {1} characters", GetFromUI<string>(() => txtCourseUrl.Text), GetFromUI<int>(() => txtToken.Text.Length));
                return null;
            }
            catch (Exception e) when (e.InnerException is InvalidTokenException ex)
            {
                MessageBox.Show("The token or the course url you provided is invalid.\nPlease make sure you entered the right token and course url", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUI(() =>
                {
                    EnableControls(true);
                    lblCurrentOperation.Text = "Login Failed";
                });
                Log.Error(ex, "Failed to log in with course url : {0} and token of {1} characters", GetFromUI<string>(() => txtCourseUrl.Text), GetFromUI<int>(() => txtToken.Text.Length));
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unknown Exception");
                throw ex;
            }

            UpdateUI(() =>
            {
                lblCurrentOperation.Text = "Starting Course Extractor";
                UC_CourseExtractorStatus.Status = CourseStatus.Starting;
            });
            Extractor.ExtractCourseStructure(out _videosCount);
            Extractor.ExtractionProgressChanged += Extractor_ExtractionProgressChanged;
            Quality quality = GetFromUI<Quality>(() => cmboxQuality.SelectedIndex);
            UpdateUI(() =>
            {
                lblCurrentOperation.Text = $"Extracting Course...[0/{_videosCount}]";
                UC_CourseExtractorStatus.Status = CourseStatus.Running;
            });
            bool isExtracted = true;
            Retry.Do(
                function: () =>
                {
                    course = Extractor.ExtractCourse(quality);
                },
                exceptionMessage: "An error occured while extracting the course",
                actionOnError: () =>
                {

                    MessageBox.Show($"An error occured while extracting the course.\nTrying again", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    UpdateUI(() =>
                    {
                        progressBar.Value = 0;
                        _currentVideoIndex = 0;
                        Extractor.ExtractionProgressChanged -= Extractor_ExtractionProgressChanged;
                    });
                    Extractor.CloseTabs();
                },
                actionOnFatal: () =>
                {
                    Extractor.KillDrivers();
                    UpdateUI(() =>
                    {
                        EnableControls(true);
                        UC_CourseExtractorStatus.Status = CourseStatus.Failed;
                        lblCurrentOperation.Text = "Course Extraction Failed";
                    });
                    isExtracted = false;
                });

            if (!isExtracted)
            {
                return null;
            }
            UpdateUI(() =>
            {
                lblCurrentOperation.Text = "Course Extracted Successfully";
                UC_CourseExtractorStatus.Status = CourseStatus.Finished;
            });
            stopwatch.Stop();
            MessageBox.Show("Elapsed time : " + stopwatch.ElapsedMilliseconds + "ms");
            return course;
        }
        private void UpdateUI(Action updateAction)
        {
            Invoke(updateAction);
        }
        private T GetFromUI<T>(Func<object> getterAction)
        {
            return (T)Invoke(getterAction);
        }
        private void EnableControls(bool isEnabled)
        {
            txtCourseDirectory.Enabled = isEnabled;
            txtCourseUrl.Enabled = isEnabled;
            txtToken.Enabled = isEnabled;
            cmboxBrowser.Enabled = isEnabled;
            cmboxQuality.Enabled = isEnabled;
            btnDownload.Enabled = isEnabled;
            btnBrowse.Enabled = isEnabled;
        }

        private void SaveConfig()
        {
            UpdateUI(() => lblCurrentOperation.Text = "Saving config file");
            Config config = new Config
            {
                AuthenticationToken = txtToken.Text,
                Browser = GetFromUI<Browser>(() => cmboxBrowser.SelectedIndex),
                Quality = GetFromUI<Quality>(() => cmboxQuality.SelectedIndex)
            };
            try
            {
                config.CourseDirectory = new DirectoryInfo(txtCourseDirectory.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("There was a problem with the course directory you entered.\nPlease enter another one", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUI(() => EnableControls(true));
                backgroundWorker.CancelAsync();
            }
            try
            {
                File.WriteAllText("./Config.json", config.ToJson());
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while trying to save config", "Failed to solve", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUI(() => EnableControls(true));
                Log.Error(ex, "An error occured while trying to save config");
                backgroundWorker.CancelAsync();
            }

        }

        private void DownloaderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Extractor.KillDrivers();
        }
        private void Extractor_ExtractionProgressChanged()
        {
            Monitor.Enter(ExtractionProgressLock);
            UpdateUI(() =>
            {
                progressBar.Value = ++_currentVideoIndex * 100 / _videosCount;
                lblCurrentOperation.Text = $"Extracting Course...[{_currentVideoIndex}/{_videosCount}]";
            });
            Monitor.Exit(ExtractionProgressLock);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 0;
            _currentVideoIndex = 0;
            Extractor.ExtractionProgressChanged -= Extractor_ExtractionProgressChanged;
            EnableControls(true);
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            bool restartApp = false;
            Task.Run(async () =>
            {
                try
                {
                    using (var githubUpdateManager = UpdateManager.GitHubUpdateManager("https://github.com/ahmedayman4a/LyndaCoursesDownloader.UpdateManager.Prerelease"))
                    using(var updateManager = await githubUpdateManager)
                    {
                        Log.Information("Checking for updates...");
                        var updateInfo = await updateManager.CheckForUpdate();
                        if (updateInfo.ReleasesToApply.Any())
                        {
                            var versionCount = updateInfo.ReleasesToApply.Count;
                            Log.Information($"{versionCount} update(s) found.");

                            var versionWord = versionCount > 1 ? "versions" : "version";
                            var message = new StringBuilder().AppendLine($"App is {versionCount} {versionWord} behind.")
                                .AppendLine("If you choose to update, the app will automatically restart after the update.")
                                .AppendLine($"Would you like to update?")
                                .ToString();
                            UpdaterForm updaterForm = new UpdaterForm(message, updateManager, _font);
                            UpdateUI(() => updaterForm.ShowDialog());
                            restartApp = updaterForm.isUpdated;
                        }
                        else
                        {
                            Log.Information("No updates detected.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"There was an issue during the update process! {ex.Message}");
                }

            }).ContinueWith(t =>
            {
                if (restartApp)
                {
                    UpdateManager.RestartApp();
                }
            });

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("./fonts/Barlow.ttf");
            fontCollection.AddFontFile("./fonts/SegoeUI.ttf");
            var fontBarlow16 = new Font(fontCollection.Families[0], 16);
            _font = fontBarlow16;
            var fontBarlow20 = new Font(fontCollection.Families[0], 20);
            var fontBarlow12 = new Font(fontCollection.Families[0], 12);
            var fontSegoeUI12 = new Font(fontCollection.Families[1], 12);
            foreach (var control in panel.Controls)
            {
                switch (control)
                {
                    case Label lbl:
                        lbl.Font = fontBarlow16;
                        break;
                    case Button btn:
                        btn.Font = fontBarlow16;
                        break;
                    case TextBox txt:
                        txt.Font = fontSegoeUI12;
                        break;
                    case ComboBox cmbox:
                        cmbox.Font = fontBarlow12;
                        break;
                    case UserControl uc:
                        (uc.Controls[0] as Label).Font = fontBarlow16;
                        break;
                }
            }
            btnBrowse.Font = fontBarlow12;
            lblCurrentOperation.Font = fontBarlow20;
        }
    }
}
