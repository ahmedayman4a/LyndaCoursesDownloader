using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LyndaCoursesDownloader.GUIDownloader
{
    public partial class CurrentOperationLabel : UserControl
    {
        public CurrentOperationLabel()
        {
            InitializeComponent();
        }
        public int TotalWidth
        {
            set
            {
                this.Width = value;
            }
        }
        public new string Text
        {
            get { return lblCurrentOperation.Text; }
            set
            {
                lblCurrentOperation.Text = value;
                int margin = (this.Width - lblCurrentOperation.Width) / 2;
                lblCurrentOperation.Location = new Point(margin, lblCurrentOperation.Location.Y);
            }
        }

    }
}
