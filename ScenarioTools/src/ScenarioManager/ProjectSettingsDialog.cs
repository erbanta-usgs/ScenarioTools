using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Dialogs;
using ScenarioTools.SMDataClasses;
using ScenarioTools.Util;

namespace ScenarioManager
{
    public partial class ProjectSettingsDialog : Form
    {
        #region Fields

        private SMProjectSettings _smProjectSettings;
        private OpenFileDialog _openFileDialog;

        #endregion Fields

        public ProjectSettingsDialog(SMProjectSettings smProjectSettings)
        {
            InitializeComponent();
            _smProjectSettings = (SMProjectSettings)smProjectSettings.Clone();
            this.tbModflowExe.Text = _smProjectSettings.ModflowExecutable;
            int numCores = SystemUtil.NumberProcessorCores();
            this.udMaxThreads.Maximum = numCores;
            if (_smProjectSettings.MaxSimultaneousRuns <= numCores)
            {
                this.udMaxThreads.Value = _smProjectSettings.MaxSimultaneousRuns;
            }
            else
            {
                this.udMaxThreads.Value = numCores;
            }
        }

        #region Properties

        public SMProjectSettings SmProjectSettings
        {
            get
            {
                return _smProjectSettings;
            }
            set
            {
                _smProjectSettings = value;
            }
        }

        #endregion Properties

        #region Event handlers
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            _openFileDialog = DialogHelper.GetNameFileDialog();
            _openFileDialog.FileName = cbNameFiles.SelectedText;
            DialogResult dr = _openFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string fName = _openFileDialog.FileName;
                SmProjectSettings.MasterNameFile = fName;
                int indx = cbNameFiles.FindStringExact(fName);
                if (indx > -1)
                {
                    cbNameFiles.SelectedIndex = indx;
                }
                else
                {
                    cbNameFiles.Items.Add(fName);
                    cbNameFiles.SelectedItem = fName;
                }
            }
        }
        private void btnBrowseModflowExe_Click(object sender, EventArgs e)
        {
            _openFileDialog = DialogHelper.GetExecutableFileDialog();
            _openFileDialog.FileName = tbModflowExe.Text;
            DialogResult dr = _openFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                tbModflowExe.Text = _openFileDialog.FileName;
                SmProjectSettings.ModflowExecutable = tbModflowExe.Text;
            }
        }

        private void cbNameFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indx = cbNameFiles.SelectedIndex;
            if (indx > -1)
            {
                SmProjectSettings.MasterNameFile = ((string)cbNameFiles.Items[indx]);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string fName = cbNameFiles.Text;
            int indx = cbNameFiles.FindStringExact(fName);
            if (indx > -1)
            {
                cbNameFiles.SelectedIndex = indx;
            }
            else
            {
                cbNameFiles.Items.Add(fName);
                cbNameFiles.SelectedItem = fName;
                SmProjectSettings.MasterNameFile = fName;
            }
            SmProjectSettings.NameFiles.Clear();
            foreach (string str in cbNameFiles.Items)
            {
                SmProjectSettings.NameFiles.Add(str);
            }
            SmProjectSettings.ModflowExecutable = tbModflowExe.Text;
            SmProjectSettings.MaxSimultaneousRuns = Convert.ToInt32(udMaxThreads.Value);
            SmProjectSettings.BackgroundImageFile = tbBackgroundImageFile.Text;
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (cbNameFiles.Items.Count > 0)
            {
                int indx = cbNameFiles.SelectedIndex;
                cbNameFiles.Items.RemoveAt(indx);
                if (cbNameFiles.Items.Count > 0)
                {
                    if (indx > 0)
                    {
                        cbNameFiles.SelectedIndex = indx - 1;
                    }
                    else
                    {
                        cbNameFiles.SelectedIndex = 0;
                    }
                }
                else
                {
                    cbNameFiles.Text = "";
                    SmProjectSettings.MasterNameFile = "";
                }
            }
        }

        private void ProjectSettingsDialog_Load(object sender, EventArgs e)
        {
            int indx;
            string fName;
            for (int i = 0; i < SmProjectSettings.NameFiles.Count; i++)
            {
                fName = SmProjectSettings.NameFiles[i];
                indx = cbNameFiles.FindStringExact(fName);
                if (indx < 0)
                {
                    cbNameFiles.Items.Add(fName);
                }
                if (SmProjectSettings.MasterNameFile == fName)
                {
                    cbNameFiles.SelectedIndex = cbNameFiles.Items.Count - 1; ;
                }
            }
            int maxSimRuns = Convert.ToInt32(SmProjectSettings.MaxSimultaneousRuns);
            if (maxSimRuns <= udMaxThreads.Maximum && maxSimRuns >= udMaxThreads.Minimum)
            {
                udMaxThreads.Value = Convert.ToInt32(SmProjectSettings.MaxSimultaneousRuns);
            }
            tbModflowExe.Text = SmProjectSettings.ModflowExecutable;
            tbBackgroundImageFile.Text = SmProjectSettings.BackgroundImageFile;
            btnBrowseModflowExe.Focus();
        }

        #endregion Event handlers

        private void btnBrowseImageFile_Click(object sender, EventArgs e)
        {
            _openFileDialog = DialogHelper.GetImageFileOpenDialog();
            _openFileDialog.FileName = tbBackgroundImageFile.Text;
            DialogResult dr = _openFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                tbBackgroundImageFile.Text = _openFileDialog.FileName;
                SmProjectSettings.BackgroundImageFile = tbBackgroundImageFile.Text;
            }
        }
    }
}
