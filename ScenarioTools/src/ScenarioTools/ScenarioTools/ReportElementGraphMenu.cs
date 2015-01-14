using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace ScenarioTools
{
    public partial class ReportElementGraphMenu : Form
    {
        ReportElementGraph reportElement;

        public ReportElementGraphMenu(ReportElementGraph reportElement)
        {
            InitializeComponent();

            // Store a reference to the report element.
            this.reportElement = reportElement;
        }
        private void ReportElementGraphMenu_Load(object sender, EventArgs e)
        {
            // Suspend layout of this form.
            this.SuspendLayout();

            // Populate the name text box.
            textBoxName.Text = reportElement.Name;

            // Select the appropriate date range radio button and populate and enable the date range boxes appropriately.
            if (reportElement.DateRangeIsAutomatic)
            {
                // Check the automatic date range button.
                radioButtonDateRangeAutomatic.Checked = true;

                // Populate the start and end date range boxes with the extrema of the data series.
                try
                {
                    dateTimePickerStart.Value = reportElement.GetMinTimeOfDataSeries();
                    dateTimePickerEnd.Value = reportElement.GetMaxTimeOfDataSeries();
                }
                catch { }

                // Disable the start and end date range boxes.
                dateTimePickerStart.Enabled = false;
                dateTimePickerEnd.Enabled = false;
            }
            else
            {
                // Check the manual date range button.
                radioButtonDateRangeManual.Checked = true;

                // Populate the start and end date range boxes with the values specified by the graph element.
                dateTimePickerStart.Value = reportElement.DateRangeStart;
                dateTimePickerEnd.Value = reportElement.DateRangeEnd;
            }

            // Select the appropriate value range button and populate and enable the value range boxes appropriately.
            if (reportElement.ValueRangeIsAutomatic)
            {
                // Check the automatic value range button.
                radioButtonValueRangeAutomatic.Checked = true;

                // Disable the min and max value boxes.
                textBoxValueMin.Enabled = false;
                textBoxValueMax.Enabled = false;
            }
            else
            {
                // Check the manual value range button.
                radioButtonValueRangeManual.Checked = true;
            }
            // Populate the min and max value boxes.
            textBoxValueMin.Text = reportElement.ValueRangeMin + "";
            textBoxValueMax.Text = reportElement.ValueRangeMax + "";

            // Populate the elements list box with the report elements.
            for (int i = 0; i < reportElement.NumDataSeries; i++)
            {
                listBoxDataSeries.Items.Add(reportElement.GetDataSeries(i));
            }

            // Resume layout of this form.
            this.ResumeLayout();
        }
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Save the name.
            reportElement.Name = textBoxName.Text;

            // Save the date settings.
            reportElement.DateRangeIsAutomatic = radioButtonDateRangeAutomatic.Checked;
            if (!reportElement.DateRangeIsAutomatic)
            {
                reportElement.DateRangeStart = dateTimePickerStart.Value;
                reportElement.DateRangeEnd = dateTimePickerEnd.Value;
            }

            // Save the value range settings.
            reportElement.ValueRangeIsAutomatic = radioButtonValueRangeAutomatic.Checked;
            if (!reportElement.ValueRangeIsAutomatic)
            {
                float valueRangeMin;
                if (float.TryParse(textBoxValueMin.Text, out valueRangeMin))
                {
                    reportElement.ValueRangeMin = valueRangeMin;
                }
                float valueRangeMax;
                if (float.TryParse(textBoxValueMax.Text, out valueRangeMax))
                {
                    reportElement.ValueRangeMax = valueRangeMax;
                }
            }

            // Save the elements.
            reportElement.ClearDataSeries();
            for (int i = 0; i < listBoxDataSeries.Items.Count; i++)
            {
                reportElement.AddDataSeries((DataSeries)listBoxDataSeries.Items[i]);
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            // If there is a selected element in the list box (not at index zero), move it up one index.
            if (listBoxDataSeries.SelectedIndex > 0)
            {
                int index = listBoxDataSeries.SelectedIndex;
                object item = listBoxDataSeries.Items[index];
                listBoxDataSeries.Items.RemoveAt(index);
                listBoxDataSeries.Items.Insert(index - 1, item);

                // Select the item that was previously selected.
                listBoxDataSeries.SelectedIndex = index - 1;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            // If there is an item selected in the list box, remove it.
            if (listBoxDataSeries.SelectedIndex >= 0)
            {
                listBoxDataSeries.Items.RemoveAt(listBoxDataSeries.SelectedIndex);
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            // If there is a selected element in the list box (not at the bottom of the list), move it up one index.
            if (listBoxDataSeries.SelectedIndex < listBoxDataSeries.Items.Count - 1)
            {
                int index = listBoxDataSeries.SelectedIndex;
                object item = listBoxDataSeries.Items[index];
                listBoxDataSeries.Items.RemoveAt(index);
                listBoxDataSeries.Items.Insert(index + 1, item);

                // Select the item that was previously selected.
                listBoxDataSeries.SelectedIndex = index + 1;
            }
        }

        private void radioButtonDateRangeAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            // Disable the date pickers.
            dateTimePickerStart.Enabled = false;
            dateTimePickerEnd.Enabled = false;
        }

        private void radioButtonDateRangeManual_CheckedChanged(object sender, EventArgs e)
        {
            // Enable the date pickers.
            dateTimePickerStart.Enabled = true;
            dateTimePickerEnd.Enabled = true;
        }

        private void radioButtonValueRangeAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            // Disable the value range text boxes.
            textBoxValueMin.Enabled = false;
            textBoxValueMax.Enabled = false;
        }

        private void radioButtonValueRangeManual_CheckedChanged(object sender, EventArgs e)
        {
            // Enable the value range text boxes.
            textBoxValueMin.Enabled = true;
            textBoxValueMax.Enabled = true;
        }

    }
}
