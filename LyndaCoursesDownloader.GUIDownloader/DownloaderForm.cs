using LyndaCoursesDownloader.CourseContent;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LyndaCoursesDownloader.GUIDownloader
{
    public partial class DownloaderForm : Form
    {
        private Course _course;
        private DirectoryInfo _courseRootDirectory;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private int _videosCount;
        private int _currentVideoIndex = 1;
        public DownloaderForm(Course course, DirectoryInfo courseRootDirectory)
        {
            _course = course;
            _courseRootDirectory = courseRootDirectory;
            InitializeComponent();
            Text = "Downloading " + _course.Name + " course";
        }

        private void DownloaderForm_Load(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DownloadCourse();
        }

        private void DownloadCourse()
        {
            try
            {
                _videosCount = _course.Chapters.SelectMany(ch => ch.Videos).Count();
                var courseDirectory = _courseRootDirectory.CreateSubdirectory(ToSafeFileName(_course.Name));
                foreach (var chapter in _course.Chapters)
                {
                    var chapterDirectory = courseDirectory.CreateSubdirectory($"[{chapter.Id}] {ToSafeFileName(chapter.Name)}");
                    foreach (var video in chapter.Videos)
                    {
                        
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        using (var downloadClient = new WebClient())
                        {
                            downloadClient.DownloadProgressChanged += DownloadClient_DownloadProgressChanged;
                            downloadClient.DownloadFileCompleted += DownloadClient_DownloadFileCompleted;
                            UpdateUI(() =>
                            {
                                lblVideo.Text = video.Name + " - [Chapter " + chapter.Id + "]";
                                lblTotal.Text = _currentVideoIndex++ + "/" + _videosCount;
                            });
                            
                            string videoName = $"[{ video.Id}] { ToSafeFileName(video.Name)}.mp4";
                            if (!(video.CaptionText is null))
                            {
                                string captionName = $"[{ video.Id}] { ToSafeFileName(video.Name)}.srt";
                                File.WriteAllText($"{Path.Combine(chapterDirectory.FullName, ToSafeFileName(captionName))}", video.CaptionText);
                            }
                            downloadClient.DownloadFileTaskAsync(new Uri(video.VideoDownloadUrl), Path.Combine(chapterDirectory.FullName, videoName)).Wait();
                        }
                    }
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while downloading the course.\nError Message : " + ex.Message, "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.Error(ex, "An error occured while downloading the course.Trying again...");
                UpdateUI(() => Close());
            }
        }

        private void DownloadClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (_currentVideoIndex <= _videosCount)
            {
                UpdateUI(() => progressBarTotal.Value = _currentVideoIndex * 100 / _videosCount);
            }
        }
        private void UpdateUI(Action updateAction)
        {
            Invoke(updateAction);
        }

        private void DownloadClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdateUI(() =>
            {
                progressBarVideo.Value = e.ProgressPercentage;
                lblPercentage.Text = e.ProgressPercentage + "%";
            });
        }

        private static string ToSafeFileName(string fileName) => string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));


    }
}
