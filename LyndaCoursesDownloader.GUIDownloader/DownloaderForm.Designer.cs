namespace LyndaCoursesDownloader.GUIDownloader
{
    partial class DownloaderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloaderForm));
            this.progressBarTotal = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDownloadingVideo = new System.Windows.Forms.Label();
            this.progressBarVideo = new System.Windows.Forms.ProgressBar();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblVideo = new System.Windows.Forms.Label();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBarTotal
            // 
            this.progressBarTotal.Location = new System.Drawing.Point(3, 120);
            this.progressBarTotal.Name = "progressBarTotal";
            this.progressBarTotal.Size = new System.Drawing.Size(631, 32);
            this.progressBarTotal.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(0, 90);
            this.label5.Margin = new System.Windows.Forms.Padding(0, 25, 0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 27);
            this.label5.TabIndex = 5;
            this.label5.Text = "Total Progress :";
            // 
            // lblDownloadingVideo
            // 
            this.lblDownloadingVideo.AutoSize = true;
            this.lblDownloadingVideo.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDownloadingVideo.Location = new System.Drawing.Point(0, 0);
            this.lblDownloadingVideo.Margin = new System.Windows.Forms.Padding(0);
            this.lblDownloadingVideo.Name = "lblDownloadingVideo";
            this.lblDownloadingVideo.Size = new System.Drawing.Size(157, 27);
            this.lblDownloadingVideo.TabIndex = 5;
            this.lblDownloadingVideo.Text = "Downloading Video :";
            // 
            // progressBarVideo
            // 
            this.progressBarVideo.Location = new System.Drawing.Point(3, 30);
            this.progressBarVideo.Name = "progressBarVideo";
            this.progressBarVideo.Size = new System.Drawing.Size(582, 32);
            this.progressBarVideo.TabIndex = 6;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblDownloadingVideo);
            this.flowLayoutPanel1.Controls.Add(this.lblVideo);
            this.flowLayoutPanel1.Controls.Add(this.progressBarVideo);
            this.flowLayoutPanel1.Controls.Add(this.lblPercentage);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.lblTotal);
            this.flowLayoutPanel1.Controls.Add(this.progressBarTotal);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 12);
            this.flowLayoutPanel1.MaximumSize = new System.Drawing.Size(640, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(640, 155);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // lblVideo
            // 
            this.lblVideo.AutoSize = true;
            this.lblVideo.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVideo.Location = new System.Drawing.Point(157, 0);
            this.lblVideo.Margin = new System.Windows.Forms.Padding(0);
            this.lblVideo.Name = "lblVideo";
            this.lblVideo.Size = new System.Drawing.Size(279, 27);
            this.lblVideo.TabIndex = 7;
            this.lblVideo.Text = "[Video Name] in Chapter [Chapter Id]";
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPercentage.Location = new System.Drawing.Point(588, 34);
            this.lblPercentage.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(37, 27);
            this.lblPercentage.TabIndex = 5;
            this.lblPercentage.Text = "0%";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(123, 90);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(0, 25, 0, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(278, 27);
            this.lblTotal.TabIndex = 8;
            this.lblTotal.Text = "[Current Video]/[Total Videos Count] ";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // DownloaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(650, 171);
            this.Controls.Add(this.flowLayoutPanel1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DownloaderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Downloading [Course Name]";
            this.Load += new System.EventHandler(this.DownloaderForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarTotal;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDownloadingVideo;
        private System.Windows.Forms.ProgressBar progressBarVideo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.Label lblVideo;
        private System.Windows.Forms.Label lblTotal;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}