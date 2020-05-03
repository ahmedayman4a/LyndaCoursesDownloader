using LyndaCoursesDownloader.CourseContent;
using LyndaCoursesDownloader.CourseExtractor;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LyndaCoursesDownloader.GUIDownloader
{
    public partial class DownloaderForm : Form
    {
        private Course _course;
        private DirectoryInfo _courseRootDirectory;
        private int _videosCount;
        private int _currentVideoIndex = 1;
        private bool closePending = false;

        public CourseStatus DownloaderStatus { get; set; } = CourseStatus.Running;

        public DownloaderForm(Course course, DirectoryInfo courseRootDirectory,Font font)
        {
            _course = course;
            _courseRootDirectory = courseRootDirectory;
            InitializeComponent();
            Text = "Downloading " + _course.Name + " course";
            foreach (var control in flowLayoutPanel.Controls)
            {
                switch (control)
                {
                    case Label lbl:
                        lbl.Font = font;
                        break;
                }
            }
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
                    if (closePending) break;
                    foreach (var video in chapter.Videos)
                    {
                        if (closePending) break;
                        Retry.Do(() =>
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
                                if (closePending) return;
                                downloadClient.DownloadFileTaskAsync(new Uri(video.VideoDownloadUrl), Path.Combine(chapterDirectory.FullName, videoName)).Wait();
                            }
                        },
                        exceptionMessage: "Failed to download video with title " + video.Name,
                        actionOnError: () =>
                        {
                            UpdateUI(() => progressBarVideo.Value = 0);
                        },
                        actionOnFatal: () =>
                        {
                            DownloaderStatus = CourseStatus.Failed;
                            UpdateUI(() => Close());
                        });
                    }
                }
                UpdateUI(() => Close());
            }
            catch (Exception ex)
            {
                DownloaderStatus = CourseStatus.Failed;
                UpdateUI(() => Close());
                throw ex;
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
            if (!closePending)
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


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                DownloaderStatus = CourseStatus.Failed;
                UpdateUI(() =>
                {
                    lblDownloadingVideo.Text = "Closing...";
                    Text = "Closing";
                    lblPercentage.Visible = false;
                    lblTotal.Visible = false;
                    lblVideo.Visible = false;
                    progressBarTotal.Visible = false;
                    progressBarVideo.Visible = false;
                });
                closePending = true;
                backgroundWorker.CancelAsync();
                e.Cancel = true;
                Enabled = false;
                return;
            }
            base.OnFormClosing(e);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DownloaderStatus != CourseStatus.Failed)
            {
                DownloaderStatus = CourseStatus.Finished;
            }
            if (closePending) Close();
            closePending = false;
        }
    }
}
