using System;
using System.ComponentModel;
using System.Windows.Forms;

using ScenarioTools.Reporting;

namespace ScenarioTools.Dialogs
{
    public partial class ConfigureChartDialog : Form
    {
        #region Fields

        public ReportElementChart repElemChart;
        public BackgroundWorker backgroundWorker;

        #endregion Fields

        public ConfigureChartDialog()
        {
            InitializeComponent();
        }

        private void ConfigureChartDialog_Load(object sender, EventArgs e)
        {
            tbYMax.Text = repElemChart.Chart.ChartAreas[0].AxisY.Maximum.ToString();
            tbYMin.Text = repElemChart.Chart.ChartAreas[0].AxisY.Minimum.ToString();
            if (repElemChart.ValueRangeIsAutomatic)
            {
                rbYMaxAuto.Checked = true;
                rbYMaxFixed.Checked = false;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            double yMin, yMax;
            try
            {
                yMax = Convert.ToDouble(tbYMax.Text);
                repElemChart.Chart.ChartAreas[0].AxisY.Maximum = yMax;
            }
            catch
            {
                tbYMax.Text = repElemChart.Chart.ChartAreas[0].AxisY.Maximum.ToString();
            }
            try
            {
                yMin = Convert.ToDouble(tbYMin.Text);
                repElemChart.Chart.ChartAreas[0].AxisY.Minimum = yMin;
            }
            catch
            {
                tbYMin.Text = repElemChart.Chart.ChartAreas[0].AxisY.Minimum.ToString();
            }
            if (backgroundWorker != null)
            {
                backgroundWorker.ReportProgress(0);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
 