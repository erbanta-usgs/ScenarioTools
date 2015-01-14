using System;
using System.Drawing;
using System.Windows.Forms;

using ScenarioTools.DatasetCalculator;
using ScenarioTools.Data_Providers;
using ScenarioTools.Reporting;

namespace ScenarioTools.Dialogs
{
    public partial class DataSeriesMenu : Form
    {
        private const int MAX_FORM_AUTO_WIDTH = 800;
        private const int MAX_FORM_AUTO_HEIGHT = 480;

        #region Fields
        private int _tabControlRightBuffer;
        private int _tabControlBottomBuffer;
        private int _panelDataProviderRightBuffer;
        private int _panelDataProviderBottomBuffer;
        private int _buttonsBottomBuffer;
        private int _cancelButtonRightBuffer;
        private int _okButtonRightBuffer;
        private bool _okToClose;
        private DataSeries _dataSeries;
        private IDataProviderMenu _dataProviderMenu;
        private DataSeriesMenuContourBox _contourBox;
        private Color _rampColor0;
        private Color _rampColor1;
        #endregion Fields

        private void configureForContourMap()
        {
            // Increase the form height
            this.Height = 630;

            // Hide the color-ramp and point-color boxes.
            groupBoxColorRamps.Visible = false;
            groupBoxPointColors.Visible = false;

            // Make sure the line colors box is visible.
            groupBoxLineColors.Visible = true;
            // But make the "No Line Series" button invisible
            radioButtonLineNone.Visible = false;

            // Also, add a box for configuring the contours.
            _contourBox = new DataSeriesMenuContourBox();
            this.panel1.Controls.Add(_contourBox);
            _contourBox.Top = groupBoxLineColors.Bottom;
            _contourBox.Left = groupBoxLineColors.Left;
            _contourBox.Width = groupBoxLineColors.Width;

            // Set the values of the contour box according to the contour settings.
            _contourBox.StartContour = _dataSeries.StartContour;
            _contourBox.ContourInterval = _dataSeries.ContourInterval;
            _contourBox.ContourValues = _dataSeries.ContourValues;
            _contourBox.ContourSpecificationType = _dataSeries.ContourSpecificationType;

            // Update the radio buttons.
            _contourBox.UpdateRadioButtons();
        }

        public DataSeriesMenu(DataSeries dataSeries)
        {
            InitializeComponent();

            // Get the form layout properties.
            _tabControlRightBuffer = this.Width - panel1.Right;
            _tabControlBottomBuffer = this.Height - panel1.Bottom;
            _panelDataProviderRightBuffer = panel1.Width - groupBoxDataProvider.Right;
            _panelDataProviderBottomBuffer = panel1.Height - groupBoxDataProvider.Bottom;
            _buttonsBottomBuffer = this.Height - buttonOk.Bottom;
            _cancelButtonRightBuffer = this.Width - buttonCancel.Right;
            _okButtonRightBuffer = this.Width - buttonOk.Right;

            // Store a reference to the data series.
            this._dataSeries = dataSeries;

            // Configure the menu according to the data series type.
            this.SuspendLayout();

            // Set defaults
            checkBoxConvertFlowToFlux.Visible = false;

            // This case is for charts.
            if (dataSeries.DataSeriesType == DataSeriesTypeEnum.ChartSeries)
            {
                // Hide the color ramps box.
                groupBoxColorRamps.Visible = false;
                this.Text = "Data Series for a Chart";
            }
            // This case shows the color ramp option for color fill maps.
            else if (dataSeries.DataSeriesType == DataSeriesTypeEnum.ColorFillMapSeries)
            {
                // Hide the line and point color boxes and move the color box into its position.
                groupBoxLineColors.Visible = false;
                groupBoxPointColors.Visible = false;
                groupBoxColorRamps.Location = groupBoxLineColors.Location;
                this.Text = "Data Series for a Color-Fill Map Layer";
                checkBoxConvertFlowToFlux.Visible = true;
            }
            else if (dataSeries.DataSeriesType == DataSeriesTypeEnum.ContourMapSeries)
            {
                this.configureForContourMap();
                this.Text = "Data Series for a Contour Map Layer";
                checkBoxConvertFlowToFlux.Visible = true;
            }
            // This case hides both options for tables.
            else
            {
                groupBoxColorRamps.Visible = false;
                groupBoxLineColors.Visible = false;
                groupBoxPointColors.Visible = false;
                this.Text = "Data Series for a Table";
            }

            // Assign colors
            _rampColor0 = dataSeries.RampColor0;
            _rampColor1 = dataSeries.RampColor1;

            this.ResumeLayout();
        }

        void comboBoxDataProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBoxDataProvider.Text = "Data Set";
            // Get the data provider from the selected item.
            if (comboBoxDataProviders.SelectedItem != null)
            {
                IDataProvider dataProvider = (IDataProvider)comboBoxDataProviders.SelectedItem;

                // Clear the data provider menu panel.
                groupBoxDataProvider.Controls.Clear();

                // Get the appropriate menu for the data provider and add it to the data provider menu panel.
                _dataProviderMenu = DataProviderManager.GetMenu(dataProvider);
                if (_dataProviderMenu is UserControl)
                {
                    groupBoxDataProvider.Width = ((UserControl)_dataProviderMenu).Width + 5;
                    int width = groupBoxDataProvider.Width;
                    this.Width = groupBoxDataProvider.Left + width + 50;
                    groupBoxDataProvider.Width = width;
                    ((UserControl)_dataProviderMenu).Top = 24;
                    ((UserControl)_dataProviderMenu).Left = 3;
                }
                groupBoxDataProvider.Controls.Add((UserControl)_dataProviderMenu);

                // If the form is too small to accommodate the data provider menu, resize it.
                UserControl dataProviderMenuControl = (UserControl)_dataProviderMenu;
                int necessaryWidth = dataProviderMenuControl.Width + groupBoxDataProvider.Left + panel1.Left + _tabControlRightBuffer + 
                    _panelDataProviderRightBuffer;
                Size newMinimumSize = new Size(necessaryWidth, MinimumSize.Height);
                this.MinimumSize = newMinimumSize;
                this.Width = necessaryWidth;

                // If the data provider menu is Data Set Calculator, assign group box text accordingly
                if (_dataProviderMenu is DataProviderCalculatedSeriesMenu || _dataProviderMenu is DataProviderCalculatedMapMenu)
                {
                    groupBoxDataProvider.Text = "Data Set Calculator";
                }
                Refresh();
            }
        }

        private void DataSeriesMenu_Load(object sender, EventArgs e)
        {
            // Suspend form layout while manipulating the form controls.
            this.SuspendLayout();

            _okToClose = true;

            // Get the current data provider of the data series.
            IDataProvider dataProvider = _dataSeries.DataProvider;

            // Get the available data providers.
            IDataProvider[] dataProviders = DataProviderManager.GetDataProviders(_dataSeries.DataConsumerType, this._dataSeries.ParentElement);

            // If the current data provider is not null, place it into the available data providers.
            if (dataProvider != null)
            {
                for (int i = 0; i < dataProviders.Length; i++)
                {
                    if (dataProvider.GetType().Equals(dataProviders[i].GetType()))
                    {
                        dataProviders[i] = dataProvider;
                    }
                }
            }

            // Load the data providers in the combo box and select the appropriate index.
            comboBoxDataProviders.Items.Clear();
            for (int i = 0; i < dataProviders.Length; i++)
            {
                comboBoxDataProviders.Items.Add(dataProviders[i]);
            }
            if (comboBoxDataProviders.Items.Count > 0)
            {
                if (dataProvider != null)
                {
                    comboBoxDataProviders.SelectedItem = dataProvider;
                }
                else
                {
                    comboBoxDataProviders.SelectedIndex = 0;
                }
            }

            // Populate the name text box.
            textBoxName.Text = _dataSeries.Name;

            #region Select line color
            // Select the appropriate line color.
            Color lineColor = _dataSeries.LineSeriesColor;
            if (ColorUtil.ColorEquals(lineColor, Color.Black))
            {
                radioButtonLineBlack.Checked = true;
            }
            else if (ColorUtil.ColorEquals(lineColor, Color.Orange))
            {
                radioButtonLineOrange.Checked = true;
            }
            else if (ColorUtil.ColorEquals(lineColor, Color.Blue))
            {
                radioButtonLineBlue.Checked = true;
            }
            else if (ColorUtil.ColorEquals(lineColor, Color.Red))
            {
                radioButtonLineRed.Checked = true;
            }
            else if (ColorUtil.ColorEquals(lineColor, Color.Green))
            {
                radioButtonLineGreen.Checked = true;
            }
            else
            {
                radioButtonLineNone.Checked = true;
            }
            #endregion Select line color

            #region Select point color
            // Select the appropriate point color.
            Color pointColor = _dataSeries.PointSeriesColor;
            if (ColorUtil.ColorEquals(pointColor, Color.Black))
            {
                radioButtonPointBlack.Checked = true;
            }
            else if (ColorUtil.ColorEquals(pointColor, Color.Orange))
            {
                radioButtonPointOrange.Checked = true;
            }
            else if (ColorUtil.ColorEquals(pointColor, Color.Blue))
            {
                radioButtonPointBlue.Checked = true;
            }
            else if (ColorUtil.ColorEquals(pointColor, Color.Red))
            {
                radioButtonPointRed.Checked = true;
            }
            else if (ColorUtil.ColorEquals(pointColor, Color.Green))
            {
                radioButtonPointGreen.Checked = true;
            }
            else
            {
                radioButtonPointNone.Checked = true;
            }
            #endregion Select point color

            // Assign ramp colors
            _rampColor0 = _dataSeries.RampColor0;
            _rampColor1 = _dataSeries.RampColor1;

            // Assign Visible and ConvertFlowToFlux checkboxes
            checkBoxVisible.Checked = _dataSeries.Visible;
            checkBoxConvertFlowToFlux.Checked = _dataSeries.ConvertFlowToFlux;

            refreshColors();

            // Resume the form layout.
            this.ResumeLayout();
        }
        private void buttonOk_Click(object sender, EventArgs e)
        {
            bool OK = true;
            string errMessage = "";
            // Store the data provider in the data series.
            if (comboBoxDataProviders.SelectedItem != null)
            {
                _dataSeries.DataProvider = (IDataProvider)comboBoxDataProviders.SelectedItem;
                if (_dataSeries.DataProvider is DataProviderCalculatedMap)
                {
                    // Data series is for a Map
                    DataProviderCalculatedMap dataProvCalcMap = (DataProviderCalculatedMap)_dataSeries.DataProvider;
                    DataProviderCalculatedMapMenu dataProvCalcMapMenu = (DataProviderCalculatedMapMenu)_dataProviderMenu;
                    string expression = dataProvCalcMapMenu.Expression;
                    OK = dataProvCalcMap.ValidateExpression(expression, out errMessage);
                }
                if (_dataSeries.DataProvider is DatasetCalculator.DataProviderCalculatedSeries)
                {
                    // Data series is for a Chart or a Table
                    DatasetCalculator.DataProviderCalculatedSeries dataProvCalcSeries = (DatasetCalculator.DataProviderCalculatedSeries)_dataSeries.DataProvider;
                    DatasetCalculator.DataProviderCalculatedSeriesMenu dataProvCalcSeriesMenu = (DatasetCalculator.DataProviderCalculatedSeriesMenu)_dataProviderMenu;
                    string expression = dataProvCalcSeriesMenu.Expression;
                    OK = dataProvCalcSeries.ValidateExpression(expression, out errMessage);
                }
            }

            if (OK)
            {
                // Store the name in the data series.
                _dataSeries.Name = textBoxName.Text;

                // Store the line color in the data series.
                _dataSeries.Visible = false;
                if (radioButtonLineBlack.Checked)
                {
                    _dataSeries.LineSeriesColor = Color.Black;
                    _dataSeries.Visible = true;
                }
                else if (radioButtonLineOrange.Checked)
                {
                    _dataSeries.LineSeriesColor = Color.Orange;
                    _dataSeries.Visible = true;
                }
                else if (radioButtonLineBlue.Checked)
                {
                    _dataSeries.LineSeriesColor = Color.Blue;
                    _dataSeries.Visible = true;
                }
                else if (radioButtonLineRed.Checked)
                {
                    _dataSeries.LineSeriesColor = Color.Red;
                    _dataSeries.Visible = true;
                }
                else if (radioButtonLineGreen.Checked)
                {
                    _dataSeries.LineSeriesColor = Color.Green;
                    _dataSeries.Visible = true;
                }
                else
                {
                    _dataSeries.LineSeriesColor = Color.Transparent;
                }

                // Store the point color in the data series.
                if (radioButtonPointBlack.Checked)
                {
                    _dataSeries.PointSeriesColor = Color.Black;
                    _dataSeries.Visible = true;
                }
                else if (radioButtonPointOrange.Checked)
                {
                    _dataSeries.PointSeriesColor = Color.Orange;
                    _dataSeries.Visible = true;
                }
                else if (radioButtonPointBlue.Checked)
                {
                    _dataSeries.PointSeriesColor = Color.Blue;
                    _dataSeries.Visible = true;
                }
                else if (radioButtonPointRed.Checked)
                {
                    _dataSeries.PointSeriesColor = Color.Red;
                    _dataSeries.Visible = true;
                }
                else if (radioButtonPointGreen.Checked)
                {
                    _dataSeries.PointSeriesColor = Color.Green;
                    _dataSeries.Visible = true;
                }
                else
                {
                    _dataSeries.PointSeriesColor = Color.Transparent;
                }

                // CheckBoxVisible overrides previous logic related to Visible property
                _dataSeries.Visible = checkBoxVisible.Checked;

                // Store ConvertFlowToFlux
                _dataSeries.ConvertFlowToFlux = checkBoxConvertFlowToFlux.Checked;

                // Store the ramp colors
                _dataSeries.RampColor0 = _rampColor0;
                _dataSeries.RampColor1 = _rampColor1;

                // If this is a contour layer, store the contour information.
                if (_dataSeries.DataSeriesType == DataSeriesTypeEnum.ContourMapSeries)
                {
                    // Store the data from the dialog in the data series object.
                    _dataSeries.StartContour = _contourBox.StartContour;
                    _dataSeries.ContourInterval = _contourBox.ContourInterval;
                    _dataSeries.ContourValues = _contourBox.ContourValues;
                    _dataSeries.ContourSpecificationType = _contourBox.ContourSpecificationType;
                    //Console.WriteLine("The contour specification type is " + dataSeries.ContourSpecificationType);
                }

                // Update the data provider from the data provider menu.
                if (_dataProviderMenu != null)
                {
                    OK = _dataProviderMenu.UpdateDataProvider(out errMessage);
                }
                _okToClose = true;
            }
            
            if (!OK)
            {
                MessageBox.Show(errMessage);
                _okToClose = false;
            }
        }

        private void DataSeriesMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !_okToClose;
            _okToClose = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _okToClose = true;
        }

        private void general_SizeChanged(object sender, EventArgs e)
        {
            int newPanelWidth = panel1.Width - _panelDataProviderRightBuffer - groupBoxDataProvider.Left;
            groupBoxDataProvider.Width = newPanelWidth;
        }

        private void refreshColors()
        {
            // Make and assign button images
            ColorRampImageMaker imageMakerColor0 = new ColorRampImageMaker(_rampColor0, _rampColor0);
            btnColor0.Image = imageMakerColor0.GetImage(btnColor0.Width, btnColor0.Height, 
                ColorRampImageMaker.RampDirection.LeftToRight, false);
            ColorRampImageMaker imageMakerColor1 = new ColorRampImageMaker(_rampColor1, _rampColor1);
            btnColor1.Image = imageMakerColor1.GetImage(btnColor1.Width, btnColor1.Height, 
                ColorRampImageMaker.RampDirection.LeftToRight, false);

            // Draw color ramp
            ColorRampImageMaker imageMakerRamp = new ColorRampImageMaker(_rampColor0,_rampColor1);
            int rampWidth = pictureBoxColorRamp.ClientSize.Width;
            int rampHeight = pictureBoxColorRamp.ClientSize.Height;
            pictureBoxColorRamp.Image = imageMakerRamp.GetImage(rampWidth, rampHeight,
                ColorRampImageMaker.RampDirection.LeftToRight, false);
        }

        private void btnColor0_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = _rampColor0;
            DialogResult dr = colorDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _rampColor0 = colorDialog1.Color;
                refreshColors();
            }
        }

        private void btnColor1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = _rampColor1;
            DialogResult dr = colorDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _rampColor1 = colorDialog1.Color;
                refreshColors();
            }
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            int newPanelWidth = panel1.Width - _panelDataProviderRightBuffer - groupBoxDataProvider.Left;
            groupBoxDataProvider.Width = newPanelWidth;
        }
    }
}
