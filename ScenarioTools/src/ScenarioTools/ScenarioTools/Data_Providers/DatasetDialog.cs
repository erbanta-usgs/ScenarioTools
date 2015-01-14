using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.Data_Providers
{
    public partial class DatasetDialog : Form
    {
        // The options are 
        object[][] options;

        public DatasetDialog(object[][] options)
        {
            InitializeComponent();

            // Store a reference to the options. This is a two-dimensional array -- the first dimension is the options. The second dimension is the
            // elements of each option -- dataset, stress period, timestep, and layer.
            this.options = options;
        }
        public object[] SelectedDataset
        {
            get
            {
                if (listBox.SelectedItem != null)
                {
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (getListElement(options[i]).Equals(listBox.SelectedItem))
                        {
                            return options[i];
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        private void LayerDialog_Load(object sender, EventArgs e)
        {
            // Find the unique datasets and add them to the dataset box. Also add an "All" selection.
            comboBoxDataset.Items.Add("-- All Data Identifiers --");
            for (int i = 0; i < options.Length; i++)
            {
                if (!comboBoxDataset.Items.Contains(options[i][0]))
                {
                    comboBoxDataset.Items.Add(options[i][0]);
                }
            }

            // Find the unique stress periods and add them to the stress period box. Also add an "All" selection.
            comboBoxStressPeriod.Items.Add("-- All Stress Periods --");
            for (int i = 0; i < options.Length; i++)
            {
                if (!comboBoxStressPeriod.Items.Contains(options[i][1]))
                {
                    comboBoxStressPeriod.Items.Add(options[i][1]);
                }
            }

            // Find the unique timesteps and add them to the timestep box. Also add an "All" selection.
            comboBoxTimestep.Items.Add("-- All Timesteps --");
            for (int i = 0; i < options.Length; i++)
            {
                if (!comboBoxTimestep.Items.Contains(options[i][2]))
                {
                    comboBoxTimestep.Items.Add(options[i][2]);
                }
            }

            // Select the first element ("all") in each box.
            comboBoxDataset.SelectedIndex = 0;
            comboBoxStressPeriod.SelectedIndex = 0;
            comboBoxTimestep.SelectedIndex = 0;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected indices of each combo box.
            int selectedDataset = comboBoxDataset.SelectedIndex;
            int selectedStressPeriod = comboBoxStressPeriod.SelectedIndex;
            int selectedTimestep = comboBoxTimestep.SelectedIndex;

            // Clear the list box.
            listBox.Items.Clear();

            // Only add the matching items back to the list box.
            for (int i = 0; i < options.Length; i++)
            {
                bool addItem = true;
                if (selectedDataset > 0)
                {
                    if (!comboBoxDataset.SelectedItem.Equals(options[i][0]))
                    {
                        addItem = false;
                    }
                }
                if (selectedStressPeriod > 0)
                {
                    if (!comboBoxStressPeriod.SelectedItem.Equals(options[i][1]))
                    {
                        addItem = false;
                    }
                }
                if (selectedTimestep > 0)
                {
                    if (!comboBoxTimestep.SelectedItem.Equals(options[i][2]))
                    {
                        addItem = false;
                    }
                }

                if (addItem)
                {
                    listBox.Items.Add(getListElement(options[i]));
                }
            }
        }
        private string getListElement(object[] option)
        {
            return option[0] + "; stress period=" + option[1] + "; timestep=" + option[2];
        }
    }
}
