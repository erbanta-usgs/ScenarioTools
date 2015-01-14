using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScenarioManager
{
    public partial class ScenarioIdDialog : Form
    {
        public ScenarioIdDialog()
        {
            InitializeComponent();
        }

        public string ScenarioID { get; set; }

        private void ScenarioIdDialog_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ScenarioID = tbScenarioID.Text;
        }

        private void Initialize()
        {
            tbScenarioID.Text = ScenarioID;
            tbScenarioID.SelectAll();
            tbScenarioID.Focus();
        }

        private void ScenarioIdDialog_Shown(object sender, EventArgs e)
        {
            Initialize();
        }

    }
}
