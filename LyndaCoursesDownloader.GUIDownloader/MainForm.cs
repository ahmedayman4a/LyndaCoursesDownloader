using LyndaCoursesDownloader.DownloaderConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using LyndaCoursesDownloader.CourseExtractor;
using LyndaCoursesDownloader.CourseContent;
using NLog;
using System.Threading;

namespace LyndaCoursesDownloader.GUIDownloader
{
    public partial class MainForm : Form
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly object ExtractionProgressLock = new object();
        private int _videosCount;
        private int _currentVideoIndex = 0;
        public MainForm()
        {
            InitializeComponent();
            UC_CurrentOperationLabel.TotalWidth = panelWithoutConfig.Width;
            UC_CurrentOperationLabel.Text = "Waiting for input from user";
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
                    _logger.Info("Acquired data from config");
                }
                catch (JsonSerializationException ex)
                {
                    MessageBox.Show("Config file is corrupt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger.Error(ex, "Config file is corrupt");
                }
            }
            else
            {
                _logger.Info("No Config is found");
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

            if (!Regex.IsMatch(txtCourseUrl.Text, @"^https?:\/\/(www\.)?lynda.com\/"))
            {
                MessageBox.Show("The course url you supplied is not a valid lynda.com course url", "Invalid Course url", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            backgroundWorker.RunWorkerAsync();
        }

        private Course ExtractCourse()
        {
            Course course = new Course();
            UpdateUI(() => UC_CurrentOperationLabel.Text = "Logging in");
            var extractor = new Extractor();
            Browser browser = GetFromUI<Browser>(() => cmboxBrowser.SelectedIndex);
            _logger.Info("Logging in");
            var initializationTask = extractor.InitializeDriver(browser);
            try
            {
                initializationTask.Start();
                extractor.Login(txtToken.Text, txtCourseUrl.Text, initializationTask).Wait();
                _logger.Info("Logged in with course url : {0} and token of {1} characters", GetFromUI<string>(() => txtCourseUrl.Text), GetFromUI<int>(() => txtToken.Text.Length));
                UpdateUI(() =>
                {
                    UC_CurrentOperationLabel.Text = "Logged in successfully";
                    UC_IsLoggedin.IsLoggedin = true;
                });
            }
            catch (InvalidTokenException ex)
            {
                MessageBox.Show("The token or the course url you provided is invalid.\nPlease make sure you entered the right token and course url", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUI(() => EnableControls(true));
                _logger.Error(ex, "Failed to log in with course url : {0} and token of {1} characters", GetFromUI<string>(() => txtCourseUrl.Text), GetFromUI<int>(() => txtToken.Text.Length));
                backgroundWorker.CancelAsync();
            }
            UpdateUI(() =>
            {
                UC_CurrentOperationLabel.Text = "Starting Course Extractor";
                UC_CourseExtractorStatus.Status = CourseStatus.Starting;
            });
            extractor.ExtractCourseStructure(out _videosCount);
            extractor.ExtractionProgressChanged += Extractor_ExtractionProgressChanged;
            Quality quality = GetFromUI<Quality>(() => cmboxQuality.SelectedIndex);
            UpdateUI(() =>
            {
                UC_CurrentOperationLabel.Text = "Extracting Course...";
                UC_CourseExtractorStatus.Status = CourseStatus.Running;
            });
            try
            {
                course = extractor.ExtractCourse(quality);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while extracting the course.\nError Message : " + ex.Message + "\nTrying again...", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.Error(ex, "An error occured while extracting the course.Trying again...");
                UpdateUI(() => EnableControls(true));
                Extractor.KillDrivers();
                backgroundWorker.CancelAsync();
            }

            UpdateUI(() =>
            {
                UC_CurrentOperationLabel.Text = "Course Extarcted Successfully";
                UC_CourseExtractorStatus.Status = CourseStatus.Finished;
            });
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
            btnDownload.Enabled = false;
            btnBrowse.Enabled = false;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateUI(() => EnableControls(false));
            SaveConfig();
            var course = ExtractCourse();
            var downloaderForm = new DownloaderForm(course, new DirectoryInfo(txtCourseDirectory.Text));
            UpdateUI(() => UC_CourseDownloaderStatus.Status = CourseStatus.Running);
            downloaderForm.ShowDialog();
            UpdateUI(() =>
            {
                UC_CourseDownloaderStatus.Status = CourseStatus.Finished;
                EnableControls(true);
                MessageBox.Show("Course Downloaded Successfully :)","Hooray",MessageBoxButtons.OK,MessageBoxIcon.Information);
            });
        }


        private void SaveConfig()
        {
            UpdateUI(() => UC_CurrentOperationLabel.Text = "Saving config file");
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
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem with the course directory you entered.\nPlease enter another one", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUI(() => EnableControls(true));
                _logger.Error(ex, "Problem with supplied directory : ", GetFromUI<string>(() => txtCourseDirectory.Text));
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
                _logger.Error(ex, "An error occured while trying to save config");
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
            UpdateUI(() => progressBar.Value = ++_currentVideoIndex * 100 / _videosCount);
            Monitor.Exit(ExtractionProgressLock);
        }
    }
}
