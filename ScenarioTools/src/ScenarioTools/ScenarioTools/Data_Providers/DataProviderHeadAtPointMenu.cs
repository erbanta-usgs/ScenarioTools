using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Dialogs;

namespace ScenarioTools.Data_Providers
{
    public partial class DataProviderHeadAtPointMenu : UserControl, IDataProviderMenu
    {
        DataProviderHeadAtPoint dataProvider;

        public DataProviderHeadAtPointMenu(DataProviderHeadAtPoint dataProvider)
        {
            InitializeComponent();

            // Store a reference to the data provider.
            this.dataProvider = dataProvider;
        }
        private void DataProviderHeadAtPointMenu_Load(object sender, EventArgs e)
        {
            // Populate the name file text box.
            textBoxHeadsFile.Text = dataProvider.HeadsFile;

            // Populate the column and row text boxes.
            textBoxColumn.Text = dataProvider.Column + "";
            textBoxRow.Text = dataProvider.Row + "";
            textBoxLayer.Text = dataProvider.Layer + "";

            // Populate the data identifier combo box
            int minLayer;
            int maxLayer;
            Data_Providers.DataProviderManager.PopulateComboBoxFromHeadTypeBinaryFile(dataProvider.HeadsFile, comboBox1, dataProvider.DataDescriptor, out minLayer, out maxLayer);

            this.Dock = DockStyle.Fill;
        }
        public bool UpdateDataProvider(out string errorMessage)
        {
            errorMessage = "";
            Console.WriteLine("Updating data provider.");

            // Get the heads file.
            string headsFile = textBoxHeadsFile.Text;

            // Parse the column, row, and layer.
            int column, row, layer;
            if (int.TryParse(textBoxColumn.Text, out column) && int.TryParse(textBoxRow.Text, out row) &&
                int.TryParse(textBoxLayer.Text, out layer))
            {
                string descriptor = "";
                if (comboBox1.SelectedItem != null)
                {
                    descriptor = comboBox1.SelectedItem.ToString();
                }

                // If we've gotten to this point, check if any of the data have changed.
                if (!headsFile.Equals(dataProvider.HeadsFile) || column != dataProvider.Column ||
                    row != dataProvider.Row || layer != dataProvider.Layer || descriptor != dataProvider.DataDescriptor)
                {
                    // If any of the data have changed, update the data provider and invalidate the dataset.
                    dataProvider.HeadsFile = headsFile;
                    dataProvider.DataDescriptor = descriptor;
                    dataProvider.Column = column;
                    dataProvider.Row = row;
                    dataProvider.Layer = layer;
                    dataProvider.InvalidateDataset();
                }
                return true;
            }
            else
            {
                errorMessage = "Error parsing layer, row, or column.";
                return false;
            }
        }
        private void buttonBrowseHeadsFile_Click(object sender, EventArgs e)
        {
            // Make and show an open-file dialog. Only continue if accepted.
            OpenFileDialog dialog = DialogHelper.GetHeadsFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxHeadsFile.Text = dialog.FileName;

                // Populate the data identifier combo box
                int minLayer;
                int maxLayer;
                Data_Providers.DataProviderManager.PopulateComboBoxFromHeadTypeBinaryFile(textBoxHeadsFile.Text, comboBox1, dataProvider.DataDescriptor, out minLayer, out maxLayer);
            }
        }
    }
}
