using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Dialogs;
using ScenarioTools.ModflowReaders;

namespace ScenarioTools.Data_Providers
{
    public partial class DataProviderCbbMapMenu : UserControl, IDataProviderMenu
    {
        DataProviderCbbMap dataProvider;

        public DataProviderCbbMapMenu(DataProviderCbbMap dataProvider)
        {
            InitializeComponent();

            // Store a reference to the data provider.
            this.dataProvider = dataProvider;
        }
        private void DataProviderCbbMapMenu_Load(object sender, EventArgs e)
        {
            // Populate the text boxes.
            textBoxCbbFile.Text = dataProvider.CbbFile;
            textBoxStressPeriod.Text = dataProvider.StressPeriod + "";
            textBoxTimestep.Text = dataProvider.Timestep + "";
            this.Dock = DockStyle.Fill;

            // Populate the combo box.
            populateIdentifierComboBox();

            // Assign values to NumericUpDowns
            numericUpDownStartLayer.Value = dataProvider.StartLayer;
            numericUpDownEndLayer.Value = dataProvider.EndLayer;
        }
        private void populateIdentifierComboBox()
        {
            // Populate the combo box.
            bool isCompact;
            int precision = PrecisionHelpers.BudgetFilePrecision(textBoxCbbFile.Text, out isCompact);
            int numLayers;
            string[] identifiers = CbbReader.GetCbbHeaders(textBoxCbbFile.Text, precision, out numLayers);
            comboBoxDataset.Items.Clear();
            for (int i = 0; i < identifiers.Length; i++)
            {
                comboBoxDataset.Items.Add(identifiers[i]);
            }

            // Select the appropriate item in the combo box.
            if (dataProvider.DataIdentifier != null)
            {
                for (int i = 0; i < comboBoxDataset.Items.Count; i++)
                {
                    if (comboBoxDataset.Items[i].ToString().Trim().ToLower().Equals(dataProvider.DataIdentifier.ToLower().Trim()))
                    {
                        comboBoxDataset.SelectedIndex = i;
                    }
                }
            }

            // If no item is selected, select the first item in the box.
            if (comboBoxDataset.SelectedIndex < 0)
            {
                if (comboBoxDataset.Items.Count > 0)
                {
                    comboBoxDataset.SelectedIndex = 0;
                }
            }

            assignMaxLayerValues(numLayers);
        }

        private void assignMaxLayerValues(int numLayers)
        {
            // Assign max values for NumericUpDowns
            if (numLayers > 0)
            {
                numericUpDownStartLayer.Maximum = numLayers;
                numericUpDownEndLayer.Maximum = numLayers;
            }
        }

        #region IDataProviderMenu Members

        public bool UpdateDataProvider(out string errorMessage)
        {
            errorMessage = "";
            bool ok = true;
            // Invalidate the current dataset.
            dataProvider.InvalidateDataset();

            // Store the file path.
            dataProvider.CbbFile = textBoxCbbFile.Text;

            // Store the stress period.
            int stressPeriod;
            if (int.TryParse(textBoxStressPeriod.Text, out stressPeriod))
            {
                dataProvider.StressPeriod = stressPeriod;
            }
            else
            {
                ok = false;
            }

            // Store the timestep.
            int timestep;
            if (int.TryParse(textBoxTimestep.Text, out timestep))
            {
                dataProvider.Timestep = timestep;
            }
            else
            {
                ok = false;
            }

            // Store the dataset identifier.
            if (comboBoxDataset.SelectedItem != null)
            {
                dataProvider.DataIdentifier = comboBoxDataset.SelectedItem.ToString();
            }

            // Store StartLayer and EndLayer
            dataProvider.StartLayer = Convert.ToInt32(numericUpDownStartLayer.Value);
            dataProvider.EndLayer = Convert.ToInt32(numericUpDownEndLayer.Value);

            if (!ok)
            {
                errorMessage = "Error parsing stress period or time step.";
                return false;
            }
            return true;
        }

        #endregion

        private void textBoxCbbFile_TextChanged(object sender, EventArgs e)
        {
            // Populate the identifier box.
            populateIdentifierComboBox();
        }

        private void buttonBrowseHeadsFile_Click(object sender, EventArgs e)
        {
            // Make an open file dialog and set the filter.
            OpenFileDialog dialog = DialogHelper.GetCbbFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxCbbFile.Text = dialog.FileName;
            }
        }

        private void comboBoxDataset_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the value with the data provider. This is used later to select the proper value when the combo box contents are reloaded, so we
            // can't wait until the menu is closed to update it.
            if (comboBoxDataset.SelectedItem != null)
            {
                dataProvider.DataIdentifier = comboBoxDataset.SelectedItem.ToString();
            }
        }

        private void buttonLayerDialog_Click(object sender, EventArgs e)
        {
            // Get the list of available layers from the specified file.
            string[][] availableLayers = CbbReader.GetCbbAvailableRecords(textBoxCbbFile.Text);

            // Make and show a layer dialog with the layers.
            DatasetDialog layerDialog = new DatasetDialog(availableLayers);
            if (layerDialog.ShowDialog() == DialogResult.OK)
            {
                // If the user accepted the dialog, put the data from the dialog into the appropriate boxes.
                object[] selectedDataset = layerDialog.SelectedDataset;
                if (selectedDataset != null)
                {
                    comboBoxDataset.SelectedItem = selectedDataset[0];
                    textBoxStressPeriod.Text = selectedDataset[1] + "";
                    textBoxTimestep.Text = selectedDataset[2] + "";
                }
            }
        }

        private void numericUpDownStartLayer_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownStartLayer.Value > numericUpDownEndLayer.Value)
            {
                numericUpDownEndLayer.Value = numericUpDownStartLayer.Value;
            }
        }

        private void numericUpDownEndLayer_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownEndLayer.Value < numericUpDownStartLayer.Value)
            {
                numericUpDownStartLayer.Value = numericUpDownEndLayer.Value;
            }
        }
    }
}
