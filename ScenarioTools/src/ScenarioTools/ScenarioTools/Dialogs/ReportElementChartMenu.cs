using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using ScenarioTools.Reporting;

namespace ScenarioTools.Dialogs
{
    public partial class ReportElementChartMenu : Form
    {
        #region Fields

        private ReportElementChart _reportElement;
        private bool _resizing = false;
        private ConfigureChartDialog _configureDialog;
        private Chart _currentChart;
        
        // Chart fields that ConfigureChartDialog may adjust, but need
        // to be saved in case user Cancels from ReportElementChartMenu


        #endregion Fields

        public ReportElementChartMenu(ReportElementChart reportElement)
        {
            InitializeComponent();
            // Store a reference to the report element.
            _reportElement = reportElement;
            string name = "CurrentChart";
            _reportElement.Chart.Name = name;
            this.splitContainer1.Panel2.Controls.Clear();

            // The Controls.Add method messes up the chart font, so save the current font
            System.Drawing.FontFamily currentFontFamily = _reportElement.Chart.Font.FontFamily;
            System.Drawing.FontStyle currentFontSyle = FontStyle.Regular;
            float currentEmSize = _reportElement.Chart.Font.Size;
            System.Drawing.Font currentFont = new Font(_reportElement.Chart.Font, currentFontSyle);
            string fontNameBefore = _reportElement.Chart.Font.SystemFontName;

            this.splitContainer1.Panel2.Controls.Add(_reportElement.Chart);

            // Reapply saved font
            _reportElement.Chart.Font = currentFont;
            string fontNameAfter = _reportElement.Chart.Font.SystemFontName;

            Control[] controls = this.splitContainer1.Panel2.Controls.Find(name,true);
            _currentChart = (Chart)controls[0];
            //_currentChart.Dock = DockStyle.Fill;
        }

        #region Properties

        public ReportElementChart ReportElement
        {
            get
            {
                return _reportElement;
            }
            set
            {
                _reportElement = value;
                string name = "CurrentChart";
                value.Chart.Name = name;
                splitContainer1.Panel2.Controls.Clear();

                // The Controls.Add method messes up the chart font, so save the current font
                System.Drawing.FontFamily currentFontFamily = _reportElement.Chart.Font.FontFamily;
                System.Drawing.FontStyle currentFontSyle = FontStyle.Regular;
                float currentEmSize = _reportElement.Chart.Font.Size;
                System.Drawing.Font currentFont = new Font(_reportElement.Chart.Font, currentFontSyle);

                splitContainer1.Panel2.Controls.Add(value.Chart);

                // Reapply saved font
                _reportElement.Chart.Font = currentFont;

                Control[] controls = splitContainer1.Panel2.Controls.Find(name,true);
                _currentChart = (Chart)controls[0];
                //_currentChart.Dock = DockStyle.Fill;
            }
        }

        #endregion Properties

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

        private void ReportElementChartMenu_Load(object sender, EventArgs e)
        {
            // Suspend layout of this form.
            this.SuspendLayout();

            // Populate the name text box.
            textBoxName.Text = _reportElement.Name;

            // Select the appropriate date range radio button and populate and enable the date range boxes appropriately.
            if (_reportElement.DateRangeIsAutomatic)
            {
                // Check the automatic date range button.
                radioButtonDateRangeAutomatic.Checked = true;

                // Populate the start and end date range boxes with the extrema of the data series.
                try
                {
                    dateTimePickerStart.Value = _reportElement.GetMinTimeOfDataSeries();
                    dateTimePickerEnd.Value = _reportElement.GetMaxTimeOfDataSeries();
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
                dateTimePickerStart.Value = _reportElement.DateRangeStart;
                dateTimePickerEnd.Value = _reportElement.DateRangeEnd;
            }

            // Select the appropriate value range button and populate and enable the value range boxes appropriately.
            if (_reportElement.ValueRangeIsAutomatic)
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
            textBoxValueMin.Text = _reportElement.ValueRangeMin + "";
            textBoxValueMax.Text = _reportElement.ValueRangeMax + "";

            // Populate the elements list box with the report elements.
            for (int i = 0; i < _reportElement.NumDataSeries; i++)
            {
                listBoxDataSeries.Items.Add(_reportElement.GetDataSeries(i));
            }

            // Instantiate the Configure Chart dialog
            _configureDialog = new ConfigureChartDialog();

            // Remove unused "Chart Properties" tab
            tabControl1.TabPages.Remove(pgChartProperties);

            // Resume layout of this form.
            this.ResumeLayout();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Save the name.
            _reportElement.Name = textBoxName.Text;

            // Save the date settings.
            _reportElement.DateRangeIsAutomatic = radioButtonDateRangeAutomatic.Checked;
            if (!_reportElement.DateRangeIsAutomatic)
            {
                _reportElement.DateRangeStart = dateTimePickerStart.Value;
                _reportElement.DateRangeEnd = dateTimePickerEnd.Value;
            }

            // Save the value range settings.
            _reportElement.ValueRangeIsAutomatic = radioButtonValueRangeAutomatic.Checked;
            if (!_reportElement.ValueRangeIsAutomatic)
            {
                float valueRangeMin;
                if (float.TryParse(textBoxValueMin.Text, out valueRangeMin))
                {
                    _reportElement.ValueRangeMin = valueRangeMin;
                }
                float valueRangeMax;
                if (float.TryParse(textBoxValueMax.Text, out valueRangeMax))
                {
                    _reportElement.ValueRangeMax = valueRangeMax;
                }
            }

            // Save the elements.
            _reportElement.ClearDataSeries();
            for (int i = 0; i < listBoxDataSeries.Items.Count; i++)
            {
                _reportElement.AddDataSeries((DataSeries)listBoxDataSeries.Items[i]);
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

        private void pgChartProperties_Layout(object sender, LayoutEventArgs e)
        {
            if (!_resizing)
            {
                try
                {
                    if (_reportElement.ValueRangeIsAutomatic)
                    {
                        _reportElement.Chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
                        _reportElement.Chart.ChartAreas[0].AxisY.Minimum = float.NaN;
                        _reportElement.Chart.ChartAreas[0].AxisY.Maximum = float.NaN;
                        _reportElement.Chart.ChartAreas[0].AxisY2.IsStartedFromZero = true;
                        _reportElement.Chart.ChartAreas[0].AxisY2.Minimum = float.NaN;
                        _reportElement.Chart.ChartAreas[0].AxisY2.Maximum = float.NaN;
                    }
                    else
                    {
                        _reportElement.Chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
                        _reportElement.Chart.ChartAreas[0].AxisY.Minimum = _reportElement.ValueRangeMin;
                        _reportElement.Chart.ChartAreas[0].AxisY.Maximum = _reportElement.ValueRangeMax;
                        _reportElement.Chart.ChartAreas[0].AxisY2.IsStartedFromZero = false;
                        _reportElement.Chart.ChartAreas[0].AxisY2.Minimum = _reportElement.ValueRangeMin;
                        _reportElement.Chart.ChartAreas[0].AxisY2.Maximum = _reportElement.ValueRangeMax;
                    }
                    if (_reportElement.DateRangeIsAutomatic)
                    {
                        _reportElement.Chart.ChartAreas[0].AxisX.Minimum = float.NaN;
                        _reportElement.Chart.ChartAreas[0].AxisX.Maximum = float.NaN;
                        _reportElement.Chart.ChartAreas[0].AxisX2.Minimum = float.NaN;
                        _reportElement.Chart.ChartAreas[0].AxisX2.Maximum = float.NaN;
                    }
                    else
                    {
                        _reportElement.Chart.ChartAreas[0].AxisX.Minimum = _reportElement.DateRangeStart.ToOADate();
                        _reportElement.Chart.ChartAreas[0].AxisX.Maximum = _reportElement.DateRangeEnd.ToOADate();
                        _reportElement.Chart.ChartAreas[0].AxisX2.Minimum = _reportElement.DateRangeStart.ToOADate();
                        _reportElement.Chart.ChartAreas[0].AxisX2.Maximum = _reportElement.DateRangeEnd.ToOADate();
                    }
                    _reportElement.Chart.ChartAreas[0].AxisX.IsStartedFromZero = false;
                    _reportElement.Chart.ChartAreas[0].AxisX2.IsStartedFromZero = false;
                    _reportElement.Chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                    _reportElement.Chart.ChartAreas[0].AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;


                    int numSeries = _reportElement.NumDataSeries;
                    if (numSeries > 0)
                    {
                        DataSeries ds;
                        for (int i = 0; i < numSeries; i++)
                        {
                            ds = _reportElement.GetDataSeries(i);
                            int width = splitContainer1.Panel2.Width;
                            int height = splitContainer1.Panel2.Height;
                            float minVal = float.NaN;
                            float maxVal = float.NaN;
                            long minTicks = 0;
                            long maxTicks = 1;
                            float xcMin = float.NaN;
                            float xcMax = float.NaN;
                            float ycMin = float.NaN;
                            float ycMax = float.NaN;
                            int nc = splitContainer1.Panel2.Controls.Count;
                            if (_reportElement.ValueRangeIsAutomatic)
                            {
                                _currentChart.ChartAreas[0].AxisY.IsStartedFromZero = true;
                                _currentChart.ChartAreas[0].AxisY.Minimum = float.NaN;
                                _currentChart.ChartAreas[0].AxisY.Maximum = float.NaN;
                                _currentChart.ChartAreas[0].AxisY2.IsStartedFromZero = true;
                                _currentChart.ChartAreas[0].AxisY2.Minimum = float.NaN;
                                _currentChart.ChartAreas[0].AxisY2.Maximum = float.NaN;
                            }
                            else
                            {
                                ycMin = Convert.ToSingle(_currentChart.ChartAreas[0].AxisY.Minimum);
                                ycMax = Convert.ToSingle(_currentChart.ChartAreas[0].AxisY.Maximum);
                                _currentChart.ChartAreas[0].AxisY.IsStartedFromZero = false;
                                _currentChart.ChartAreas[0].AxisY2.IsStartedFromZero = false;
                            }
                            ds.DrawContent(_currentChart, width, height, minVal, maxVal, minTicks, maxTicks, xcMin, xcMax, ycMin, ycMax);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Unknown error encountered in construction of chart.");
                }
            }
        }

        private void ReportElementChartMenu_ResizeBegin(object sender, EventArgs e)
        {
            _resizing = true;
        }

        private void ReportElementChartMenu_ResizeEnd(object sender, EventArgs e)
        {
            _resizing = false;
        }

        private void ShowConfigureDialog()
        {
            _configureDialog.repElemChart = _reportElement;
            _configureDialog.backgroundWorker = backgroundWorker1;
            _configureDialog.ShowDialog();
        }


        private BackgroundWorker NewBackgroundWorker()
        {
            BackgroundWorker newBackgroundWorker = new BackgroundWorker();
            newBackgroundWorker.WorkerReportsProgress = true;
            newBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
            newBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            newBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            return newBackgroundWorker;
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            // Open Configure Chart dialog
            if (backgroundWorker1 != null)
            {
                backgroundWorker1.Dispose();
            }
            backgroundWorker1 = NewBackgroundWorker();
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ShowConfigureDialog();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this._reportElement.Chart.Invalidate();
            _reportElement.Chart.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //_configureDialog.Close();
        }

        private void ReportElementChartMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            //_configureDialog.Close();
            if (backgroundWorker1 != null)
            {
                if (backgroundWorker1.WorkerSupportsCancellation)
                {
                    backgroundWorker1.CancelAsync();
                }
            }
        }

        private void ReportElementChartMenu_Deactivate(object sender, EventArgs e)
        {
            //_configureDialog.Close();
            if (backgroundWorker1 != null)
            {
                if (backgroundWorker1.WorkerSupportsCancellation)
                {
                    backgroundWorker1.CancelAsync();
                }
            }
        }

        private void pgDomainRange_Leave(object sender, EventArgs e)
        {
            if (radioButtonValueRangeManual.Checked)
            {
                _reportElement.ValueRangeIsAutomatic = false;
                _currentChart.ChartAreas[0].AxisY.IsStartedFromZero = false;
                _currentChart.ChartAreas[0].AxisY2.IsStartedFromZero = false;
                try
                {
                    _reportElement.Chart.ChartAreas[0].AxisY.Minimum = Convert.ToDouble(textBoxValueMin.Text);
                    _reportElement.Chart.ChartAreas[0].AxisY2.Minimum = Convert.ToDouble(textBoxValueMin.Text);
                    _reportElement.ValueRangeMin = Convert.ToSingle(textBoxValueMin.Text);
                }
                catch
                {
                    textBoxValueMin.Text = _reportElement.Chart.ChartAreas[0].AxisY.Minimum.ToString();                   
                }
                try
                {
                    _reportElement.Chart.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(textBoxValueMax.Text);
                    _reportElement.Chart.ChartAreas[0].AxisY2.Maximum = Convert.ToDouble(textBoxValueMax.Text);
                    _reportElement.ValueRangeMax = Convert.ToSingle(textBoxValueMax.Text);
                }
                catch
                {
                    textBoxValueMax.Text = _reportElement.Chart.ChartAreas[0].AxisY.Maximum.ToString();
                }
            }
            else
            {
                _reportElement.ValueRangeIsAutomatic = true;
                _currentChart.ChartAreas[0].AxisY.IsStartedFromZero = true;
                _currentChart.ChartAreas[0].AxisY.Minimum = float.NaN;
                _currentChart.ChartAreas[0].AxisY.Maximum = float.NaN;
                _currentChart.ChartAreas[0].AxisY2.IsStartedFromZero = true;
                _currentChart.ChartAreas[0].AxisY2.Minimum = float.NaN;
                _currentChart.ChartAreas[0].AxisY2.Maximum = float.NaN;
            }
            if (radioButtonDateRangeManual.Checked)
            {
                _reportElement.DateRangeIsAutomatic = false;
                _reportElement.DateRangeStart = dateTimePickerStart.Value;
                _reportElement.DateRangeEnd = dateTimePickerEnd.Value;
                _currentChart.ChartAreas[0].AxisX.Minimum = dateTimePickerStart.Value.ToOADate();
                _currentChart.ChartAreas[0].AxisX.Maximum = dateTimePickerEnd.Value.ToOADate();
                _currentChart.ChartAreas[0].AxisX2.Minimum = dateTimePickerStart.Value.ToOADate();
                _currentChart.ChartAreas[0].AxisX2.Maximum = dateTimePickerEnd.Value.ToOADate();
            }
            else
            {
                _reportElement.DateRangeIsAutomatic = true;
                _currentChart.ChartAreas[0].AxisX.Minimum = float.NaN;
                _currentChart.ChartAreas[0].AxisX.Maximum = float.NaN;
                _currentChart.ChartAreas[0].AxisX.IsStartedFromZero = false;
                _currentChart.ChartAreas[0].AxisX2.Minimum = float.NaN;
                _currentChart.ChartAreas[0].AxisX2.Maximum = float.NaN;
                _currentChart.ChartAreas[0].AxisX2.IsStartedFromZero = false;
            }
        }
   
    }
}
