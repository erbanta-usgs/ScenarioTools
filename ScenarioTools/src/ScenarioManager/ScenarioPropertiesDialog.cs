using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ScenarioTools;
using ScenarioTools.Scene;

namespace ScenarioManager
{
    public partial class ScenarioPropertiesDialog : Form
    {
        private Scenario _scenario;

        public ScenarioPropertiesDialog()
        {
            InitializeComponent();
        }

        #region Properties
        public Scenario Scenario
        {
            get
            {
                return _scenario;
            }
            set
            {
                _scenario = value;
            }
        }
        #endregion

        private void ScenarioPropertiesDialog_Load(object sender, EventArgs e)
        {
            if (_scenario != null)
            {
                tbScenarioName.Text = _scenario.Name;
                tbNameFile.Text = _scenario.NameFile();
                tbFolder.Text = _scenario.Folder();
                tbDescription.Text = _scenario.Description;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            _scenario.Name = tbScenarioName.Text;
            //_scenario.NameFile = tbNameFile.Text;
            //_scenario.Folder = tbFolder.Text;
            _scenario.Description = tbDescription.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
