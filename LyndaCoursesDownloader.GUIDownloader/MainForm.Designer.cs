namespace LyndaCoursesDownloader.GUIDownloader
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel = new System.Windows.Forms.Panel();
            this.lblCurrentOperation = new System.Windows.Forms.Label();
            this.UC_IsLoggedin = new LyndaCoursesDownloader.GUIDownloader.IsLoggedinUserControl();
            this.UC_CourseDownloaderStatus = new LyndaCoursesDownloader.GUIDownloader.CourseStatusUserControl();
            this.UC_CourseExtractorStatus = new LyndaCoursesDownloader.GUIDownloader.CourseStatusUserControl();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtCourseDirectory = new System.Windows.Forms.TextBox();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.txtCourseUrl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmboxQuality = new System.Windows.Forms.ComboBox();
            this.cmboxBrowser = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.lblCurrentOperation);
            this.panel.Controls.Add(this.UC_IsLoggedin);
            this.panel.Controls.Add(this.UC_CourseDownloaderStatus);
            this.panel.Controls.Add(this.UC_CourseExtractorStatus);
            this.panel.Controls.Add(this.progressBar);
            this.panel.Controls.Add(this.btnDownload);
            this.panel.Controls.Add(this.btnBrowse);
            this.panel.Controls.Add(this.txtCourseDirectory);
            this.panel.Controls.Add(this.txtToken);
            this.panel.Controls.Add(this.txtCourseUrl);
            this.panel.Controls.Add(this.label3);
            this.panel.Controls.Add(this.cmboxQuality);
            this.panel.Controls.Add(this.cmboxBrowser);
            this.panel.Controls.Add(this.label10);
            this.panel.Controls.Add(this.label8);
            this.panel.Controls.Add(this.label6);
            this.panel.Controls.Add(this.label5);
            this.panel.Controls.Add(this.label4);
            this.panel.Controls.Add(this.label2);
            this.panel.Controls.Add(this.label1);
            this.panel.ForeColor = System.Drawing.Color.White;
            this.panel.Location = new System.Drawing.Point(12, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(666, 375);
            this.panel.TabIndex = 0;
            // 
            // lblCurrentOperation
            // 
            this.lblCurrentOperation.Font = new System.Drawing.Font("Barlow Condensed", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentOperation.ForeColor = System.Drawing.Color.Gold;
            this.lblCurrentOperation.Location = new System.Drawing.Point(20, 287);
            this.lblCurrentOperation.Name = "lblCurrentOperation";
            this.lblCurrentOperation.Size = new System.Drawing.Size(630, 33);
            this.lblCurrentOperation.TabIndex = 8;
            this.lblCurrentOperation.Text = "Getting input from user";
            this.lblCurrentOperation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UC_IsLoggedin
            // 
            this.UC_IsLoggedin.BackColor = System.Drawing.Color.Black;
            this.UC_IsLoggedin.IsLoggedin = false;
            this.UC_IsLoggedin.Location = new System.Drawing.Point(103, 249);
            this.UC_IsLoggedin.Name = "UC_IsLoggedin";
            this.UC_IsLoggedin.Size = new System.Drawing.Size(37, 27);
            this.UC_IsLoggedin.TabIndex = 6;
            // 
            // UC_CourseDownloaderStatus
            // 
            this.UC_CourseDownloaderStatus.BackColor = System.Drawing.Color.Black;
            this.UC_CourseDownloaderStatus.Location = new System.Drawing.Point(545, 249);
            this.UC_CourseDownloaderStatus.Name = "UC_CourseDownloaderStatus";
            this.UC_CourseDownloaderStatus.Size = new System.Drawing.Size(105, 29);
            this.UC_CourseDownloaderStatus.Status = LyndaCoursesDownloader.GUIDownloader.CourseStatus.NotRunning;
            this.UC_CourseDownloaderStatus.TabIndex = 5;
            // 
            // UC_CourseExtractorStatus
            // 
            this.UC_CourseExtractorStatus.BackColor = System.Drawing.Color.Black;
            this.UC_CourseExtractorStatus.Location = new System.Drawing.Point(284, 249);
            this.UC_CourseExtractorStatus.Name = "UC_CourseExtractorStatus";
            this.UC_CourseExtractorStatus.Size = new System.Drawing.Size(105, 29);
            this.UC_CourseExtractorStatus.Status = LyndaCoursesDownloader.GUIDownloader.CourseStatus.NotRunning;
            this.UC_CourseExtractorStatus.TabIndex = 5;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(20, 326);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(631, 32);
            this.progressBar.TabIndex = 4;
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.Color.DarkGreen;
            this.btnDownload.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnDownload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnDownload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DimGray;
            this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownload.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownload.ForeColor = System.Drawing.Color.White;
            this.btnDownload.Location = new System.Drawing.Point(19, 191);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(631, 40);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "Start Downloading";
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.DarkGray;
            this.btnBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnBrowse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DimGray;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Barlow Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(615, 144);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(35, 29);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = ". . .";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtCourseDirectory
            // 
            this.txtCourseDirectory.BackColor = System.Drawing.Color.DarkGray;
            this.txtCourseDirectory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCourseDirectory.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCourseDirectory.ForeColor = System.Drawing.Color.White;
            this.txtCourseDirectory.Location = new System.Drawing.Point(150, 144);
            this.txtCourseDirectory.Name = "txtCourseDirectory";
            this.txtCourseDirectory.Size = new System.Drawing.Size(459, 29);
            this.txtCourseDirectory.TabIndex = 2;
            // 
            // txtToken
            // 
            this.txtToken.BackColor = System.Drawing.Color.DarkGray;
            this.txtToken.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtToken.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtToken.ForeColor = System.Drawing.Color.White;
            this.txtToken.Location = new System.Drawing.Point(150, 94);
            this.txtToken.Name = "txtToken";
            this.txtToken.Size = new System.Drawing.Size(500, 29);
            this.txtToken.TabIndex = 2;
            // 
            // txtCourseUrl
            // 
            this.txtCourseUrl.BackColor = System.Drawing.Color.DarkGray;
            this.txtCourseUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCourseUrl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCourseUrl.ForeColor = System.Drawing.Color.White;
            this.txtCourseUrl.Location = new System.Drawing.Point(150, 48);
            this.txtCourseUrl.Name = "txtCourseUrl";
            this.txtCourseUrl.Size = new System.Drawing.Size(500, 29);
            this.txtCourseUrl.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 27);
            this.label3.TabIndex = 0;
            this.label3.Text = "Token Cookie : ";
            // 
            // cmboxQuality
            // 
            this.cmboxQuality.BackColor = System.Drawing.Color.Black;
            this.cmboxQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboxQuality.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmboxQuality.Font = new System.Drawing.Font("Barlow Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmboxQuality.ForeColor = System.Drawing.Color.White;
            this.cmboxQuality.FormattingEnabled = true;
            this.cmboxQuality.Items.AddRange(new object[] {
            "720 (High)",
            "540 (Medium)",
            "360 (Low)"});
            this.cmboxQuality.Location = new System.Drawing.Point(459, 7);
            this.cmboxQuality.Name = "cmboxQuality";
            this.cmboxQuality.Size = new System.Drawing.Size(191, 28);
            this.cmboxQuality.TabIndex = 1;
            // 
            // cmboxBrowser
            // 
            this.cmboxBrowser.BackColor = System.Drawing.Color.Black;
            this.cmboxBrowser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboxBrowser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmboxBrowser.Font = new System.Drawing.Font("Barlow Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmboxBrowser.ForeColor = System.Drawing.Color.White;
            this.cmboxBrowser.FormattingEnabled = true;
            this.cmboxBrowser.Items.AddRange(new object[] {
            "Firefox (Fast)",
            "Chrome (Slow)"});
            this.cmboxBrowser.Location = new System.Drawing.Point(103, 7);
            this.cmboxBrowser.Name = "cmboxBrowser";
            this.cmboxBrowser.Size = new System.Drawing.Size(191, 28);
            this.cmboxBrowser.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(395, 249);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(160, 27);
            this.label10.TabIndex = 0;
            this.label10.Text = "Course Downloader: ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(149, 249);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(145, 27);
            this.label8.TabIndex = 0;
            this.label8.Text = "Course Extractor : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 249);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 27);
            this.label6.TabIndex = 0;
            this.label6.Text = "Logged in : ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 27);
            this.label5.TabIndex = 0;
            this.label5.Text = "Course Location : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(336, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 27);
            this.label4.TabIndex = 0;
            this.label4.Text = "Video Quality : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 27);
            this.label2.TabIndex = 0;
            this.label2.Text = "Course Url : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Barlow Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Browser : ";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(693, 396);
            this.Controls.Add(this.panel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LyndaCoursesDownloader - By ahmedayman4a";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DownloaderForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ComboBox cmboxBrowser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtCourseDirectory;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmboxQuality;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private CourseStatusUserControl UC_CourseExtractorStatus;
        private CourseStatusUserControl UC_CourseDownloaderStatus;
        private IsLoggedinUserControl UC_IsLoggedin;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.TextBox txtCourseUrl;
        private System.Windows.Forms.Label lblCurrentOperation;
    }
}

