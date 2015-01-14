using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.Dialogs
{
    public partial class EditNameDialog : Form
    {

        public EditNameDialog()
        {
            InitializeComponent();            
        }

        private void EditNameDialog_Activated(object sender, EventArgs e)
        {
            tbOld.Text = Text;
            tbNew.Text = Text;
            tbNew.Select();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Text = tbNew.Text;
        }
    }
}
