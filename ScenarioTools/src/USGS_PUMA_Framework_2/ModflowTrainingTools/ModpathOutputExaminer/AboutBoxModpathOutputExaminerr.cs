using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ModpathOutputExaminer
{
    public partial class AboutBoxModpathOutputExaminer : Form
    {
        public AboutBoxModpathOutputExaminer()
        {
            InitializeComponent();
            lblVersion.Text = "Version " + Application.ProductVersion;
            string exeFile = Application.ExecutablePath;
            DateTime date = System.IO.File.GetLastWriteTime(exeFile);
            lblDate.Text = date.ToLongDateString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void AboutBoxModflowOutputViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
