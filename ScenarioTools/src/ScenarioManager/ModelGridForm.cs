using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScenarioTools.Util;

namespace ScenarioManager
{
    public partial class ModelGridForm : Form
    {
        public string ShapefilePath;
        public string Directory;

        public ModelGridForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tempStr;
            tempStr = FileUtil.FindFile(FileType.Shapefile, openFileDialog1, tbFilename.Text);
            if (tempStr != "")
            {
                tbFilename.Text = tempStr;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ShapefilePath = tbFilename.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ModelGridForm_Load(object sender, EventArgs e)
        {
            tbFilename.Text = ShapefilePath;
            openFileDialog1.InitialDirectory = Directory;
        }
    }
}
