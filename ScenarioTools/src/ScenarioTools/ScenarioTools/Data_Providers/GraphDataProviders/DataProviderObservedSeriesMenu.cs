using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Dialogs;
using ScenarioTools.FileReaders;

namespace ScenarioTools.Data_Providers
{
    public partial class DataProviderObservedSeriesMenu : UserControl, IDataProviderMenu
    {
        DataProviderObservedSeries dataProvider;

        public DataProviderObservedSeriesMenu(DataProviderObservedSeries dataProvider)
        {
            InitializeComponent();

            // Store a reference to the data provider.
            this.dataProvider = dataProvider;
        }

        private void DataProviderObservedSeriesMenu_Load(object sender, EventArgs e)
        {
            // Populate the text box with the file path from the data provider.
            textBoxFile.Text = dataProvider.FilePath;

            // Select the series in the combo box.
            comboBoxSeries.SelectedItem = dataProvider.SeriesName;

            Dock = DockStyle.Fill;
        }

        #region IDataProviderMenu Members

        public bool UpdateDataProvider(out string errorMessage)
        {
            errorMessage = "";
            // Update the file path.
            dataProvider.FilePath = textBoxFile.Text;

            // Update the series.
            if (comboBoxSeries.SelectedItem != null)
            {
                dataProvider.SeriesName = comboBoxSeries.SelectedItem + "";
            }

            // Invalidate the dataset.
            dataProvider.InvalidateDataset();
            return true;
        }

        #endregion

        private void buttonBrowseFile_Click(object sender, EventArgs e)
        {
            // Show the dialog. Set the path if accepted.
            OpenFileDialog dialog = DialogHelper.GetSmpFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFile.Text = dialog.FileName;
            }
        }

        private void textBoxFile_TextChanged(object sender, EventArgs e)
        {
            // Get the currently selected index.
            int selectedIndex = comboBoxSeries.SelectedIndex;

            // Update the selections in the combo box.
            string[] availableSeries = BoreholeSampleFileReader.GetAvailableSeries(textBoxFile.Text);
            Array.Sort(availableSeries);
            comboBoxSeries.Items.Clear();
            for (int i = 0; i < availableSeries.Length; i++)
            {
                comboBoxSeries.Items.Add(availableSeries[i]);
            }

            // Select the proper element in the box.
            if (comboBoxSeries.Items.Count > 0)
            {
                // Constrain the selected index.
                if (selectedIndex < 0)
                {
                    selectedIndex = 0;
                }
                else if (selectedIndex >= comboBoxSeries.Items.Count)
                {
                    selectedIndex = comboBoxSeries.Items.Count - 1;
                }

                // Update the selected index.
                comboBoxSeries.SelectedIndex = selectedIndex;
            }
        }
    }
}
