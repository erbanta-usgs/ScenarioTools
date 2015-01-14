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
    public partial class DataProviderCbbAtPointMenu : UserControl, IDataProviderMenu
    {
        DataProviderCbbAtPoint dataProvider;

        public DataProviderCbbAtPointMenu(DataProviderCbbAtPoint dataProvider)
        {
            InitializeComponent();

            // Store a reference to the data provider.
            this.dataProvider = dataProvider;
        }
        private void DataProviderCbbAtPointMenu_Load(object sender, EventArgs e)
        {
            // Populate the text boxes.
            textBoxCbbFile.Text = dataProvider.CbbFile;
            textBoxColumn.Text = dataProvider.Column + "";
            textBoxRow.Text = dataProvider.Row + "";
            textBoxLayer.Text = dataProvider.Layer + "";

            // Populate the combo box.
            int numLayers;
            CbbHelper.PopulateIdentifierComboBox(textBoxCbbFile.Text, comboBoxDataset, dataProvider.DataIdentifier, out numLayers);
            Dock = DockStyle.Fill;
        }

        private void buttonBrowseCBBFile_Click(object sender, EventArgs e)
        {
            // Make an open file dialog and set the filter.
            OpenFileDialog dialog = DialogHelper.GetCbbFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxCbbFile.Text = dialog.FileName;
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

            // Store the dataset identifier.
            if (comboBoxDataset.SelectedItem != null)
            {
                dataProvider.DataIdentifier = comboBoxDataset.SelectedItem.ToString();
            }

            // Store the column.
            int column;
            if (int.TryParse(textBoxColumn.Text, out column))
            {
                dataProvider.Column = column;
            }
            else
            {
                ok = false;
            }

            // Store the row.
            int row;
            if (int.TryParse(textBoxRow.Text, out row))
            {
                dataProvider.Row = row;
            }
            else
            {
                ok = false;
            }

            // Store the layer.
            int layer;
            if (int.TryParse(textBoxLayer.Text, out layer))
            {
                dataProvider.Layer = layer;
            }
            else
            {
                ok = false;
            }

            if (!ok)
            {
                errorMessage = "Error parsing layer, row, or column.";
                return false;
            }
            return true;
        }

        #endregion

        private void textBoxCbbFile_TextChanged(object sender, EventArgs e)
        {
            // Populate the identifier box.
            int numLayers;
            CbbHelper.PopulateIdentifierComboBox(textBoxCbbFile.Text, comboBoxDataset, dataProvider.DataIdentifier, out numLayers);
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

    }
}
