using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace ModflowOutputViewer
{
    public partial class EditExcludedValuesDialog : Form
    {
        public EditExcludedValuesDialog()
        {
            InitializeComponent();
            textboxHNOFLO.Text = "1.0E+30";
            textboxHDRY.Text = "1.0E+20";
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public float HNoFlo
        {
            get 
            {
                return float.Parse(textboxHNOFLO.Text);
            }
            set 
            {
                textboxHNOFLO.Text = value.ToString();
            }
        }

        public float HDry
        {
            get
            {
                return float.Parse(textboxHDRY.Text);
            }
            set
            {
                textboxHDRY.Text = value.ToString();
            }
        }

        private void textboxHNOFLO_Validating(object sender, CancelEventArgs e)
        {
            float h;
            bool tryParse = float.TryParse(textboxHNOFLO.Text, out h);
            if (!tryParse)
            {
                e.Cancel = true;
                MessageBox.Show("Enter a numeric value for HNOFLO.");
            }
        }

        private void textboxHDRY_Validating(object sender, CancelEventArgs e)
        {
            float h;
            bool tryParse = float.TryParse(textboxHDRY.Text, out h);
            if (!tryParse)
            {
                e.Cancel = true;
                MessageBox.Show("Enter a numeric value for HDRY.");
            }
        }


    }
}
