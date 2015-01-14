using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Dialogs;

namespace ScenarioTools.Data_Providers
{
    public partial class DataProviderHeadMapMenu : UserControl, IDataProviderMenu
    {
        DataProviderHeadMap dataProvider;

        public DataProviderHeadMapMenu(DataProviderHeadMap dataProvider)
        {
            InitializeComponent();

            // Store a reference to the data provider.
            this.dataProvider = dataProvider;
        }

        private void DataProviderHeadMapMenu_Load(object sender, EventArgs e)
        {
            // Populate the text boxes.
            textBoxHeadsFile.Text = dataProvider.HeadsFile;
            textBoxStressPeriod.Text = dataProvider.StressPeriod + "";
            textBoxTimestep.Text = dataProvider.Timestep + "";

            // Populate the combo box
            int minLayer;
            int maxLayer;
            Data_Providers.DataProviderManager.PopulateComboBoxFromHeadTypeBinaryFile(dataProvider.HeadsFile, comboBox1, dataProvider.DataDescriptor, out minLayer, out maxLayer);

            // Populate the Layer NumericUpDown
            numericUpDownLayer.Minimum = minLayer;
            numericUpDownLayer.Maximum = maxLayer;
            numericUpDownLayer.Value = dataProvider.Layer;

            this.Dock = DockStyle.Fill;
        }

        #region IDataProviderMenu Members

        public bool UpdateDataProvider(out string errorMessage)
        {
            errorMessage = "";
            bool ok = true;
            // Invalidate the current dataset.
            dataProvider.InvalidateDataset();

            // Store the file paths.
            dataProvider.HeadsFile = textBoxHeadsFile.Text;

            // Store the data identifier
            if (comboBox1.SelectedItem != null)
            {
                dataProvider.DataDescriptor = comboBox1.SelectedItem.ToString();
            }

            // Store the layer.
            dataProvider.Layer = (int)numericUpDownLayer.Value;

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

            if (!ok)
            {
                errorMessage = "Error parsing stress period or time step";
            }
            return ok;
        }

        #endregion

        private void buttonBrowseHeadsFile_Click(object sender, EventArgs e)
        {
            // Make an open file dialog and set the filter.
            OpenFileDialog dialog = DialogHelper.GetHeadsFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxHeadsFile.Text = dialog.FileName;

                // Populate the combo box
                int minLayer;
                int maxLayer;
                Data_Providers.DataProviderManager.PopulateComboBoxFromHeadTypeBinaryFile(textBoxHeadsFile.Text, comboBox1, this.dataProvider.DataDescriptor, out minLayer, out maxLayer);

                // Populate the Layer NumericUpDown
                numericUpDownLayer.Minimum = minLayer;
                numericUpDownLayer.Maximum = maxLayer;
            }
        }
    }
}
