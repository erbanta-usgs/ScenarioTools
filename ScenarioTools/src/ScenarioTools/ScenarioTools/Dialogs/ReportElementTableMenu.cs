using System;
using System.Windows.Forms;

using ScenarioTools.Reporting;

namespace ScenarioTools.Dialogs
{
    public partial class ReportElementTableMenu : Form
    {
        ReportElementTable reportElement;

        public ReportElementTableMenu(ReportElementTable reportElement)
        {
            InitializeComponent();

            // Store a reference to the report element.
            this.reportElement = reportElement;
        }
        private void ReportElementTableMenu_Load(object sender, EventArgs e)
        {
            // Suspend layout of this form.
            this.SuspendLayout();

            // Populate the name text box.
            textBoxName.Text = reportElement.Name;
            ckbxIncludeTimeInTable.Checked = reportElement.IncludeTimeInTable;

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
            reportElement.IncludeTimeInTable = ckbxIncludeTimeInTable.Checked;

            // Save the date settings.
            reportElement.DateRangeIsAutomatic = radioButtonDateRangeAutomatic.Checked;
            if (!reportElement.DateRangeIsAutomatic)
            {
                reportElement.DateRangeStart = dateTimePickerStart.Value;
                reportElement.DateRangeEnd = dateTimePickerEnd.Value;
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
            int index = listBoxDataSeries.SelectedIndex;
            if (index > 0)
            {
                // Reassign pointers to move selected data series up one position
                DataSeries selectedDataSeries = (DataSeries)listBoxDataSeries.Items[index];
                listBoxDataSeries.Items[index] = listBoxDataSeries.Items[index - 1];
                listBoxDataSeries.Items[index - 1] = selectedDataSeries;
                // Adjust selected item to follow move
                listBoxDataSeries.SelectedIndex = index - 1;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int index = listBoxDataSeries.SelectedIndex;
            if (index > -1)
            {
                listBoxDataSeries.Items.RemoveAt(index);
            }

        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            int index = listBoxDataSeries.SelectedIndex;
            if ((index > -1) && (index < listBoxDataSeries.Items.Count - 1))
            {
                // Reassign pointers to move selected data series down one position
                DataSeries selectedDataSeries = (DataSeries)listBoxDataSeries.Items[index];
                listBoxDataSeries.Items[index] = listBoxDataSeries.Items[index + 1];
                listBoxDataSeries.Items[index + 1] = selectedDataSeries;
                // Adjust selected item to follow move
                listBoxDataSeries.SelectedIndex = index + 1;
            }
        }
    }
}
