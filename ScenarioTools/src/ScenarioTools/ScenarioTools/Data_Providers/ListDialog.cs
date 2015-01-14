using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.Data_Providers
{
    public partial class ListDialog : Form
    {
        string[] options;
        string selectedOption;

        public ListDialog(string[] options)
        {
            InitializeComponent();

            // Store a reference to the options.
            this.options = options;

            // Set the selected option variable to null.
            this.selectedOption = null;
        }
        private void ListDialog_Load(object sender, EventArgs e)
        {
            // Populate the dialog with the user-provided options.
            if (options != null)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    listBox.Items.Add(options[i]);
                }
            }
        }
        public string SelectedOption
        {
            get
            {
                return selectedOption;
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Store the selected item.
            if (listBox.SelectedItem != null)
            {
                selectedOption = (string)listBox.SelectedItem;
            }
        }
    }
}
