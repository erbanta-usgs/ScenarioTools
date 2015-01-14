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
    public partial class StringListViewer : Form
    {
        public List<string> StringList;
        public StringListViewer()
        {
            InitializeComponent();
        }

        private void StringListViewer_Load(object sender, EventArgs e)
        {
            if (StringList != null)
            {
                textBox1.Lines = StringList.ToArray();
            }
            else
            {
                textBox1.Clear();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
        }
    }
}
