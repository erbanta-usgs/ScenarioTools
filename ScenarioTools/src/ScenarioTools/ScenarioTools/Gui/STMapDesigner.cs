using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using GeoAPI.Geometries;
using ScenarioTools.Geometry;
using ScenarioTools.Reporting;

namespace ScenarioTools.Gui
{
    public partial class STMapDesigner : Form
    {
        #region Fields
        private string _selectedExtentName;
        private string _originalDesiredExtentName;
        private int _originalBackgroundImageBrightness;
        private bool _convertFlowToFlux;
        private bool _generatingImage;
        private bool _pressedOk;
        private bool _loading;
        private bool _mapNeedsRefresh;
        private bool _displayingImage;
        private bool _processingMouseUp;
        private bool _processingMouseDown;
        private bool _layoutTabNeedsRefresh;
        private STMap _stMap;
        private STMap _stMapOriginal;
        private Extent _userDrawnExtent;
        private Extent _selectedExtent;
        private Extent _defaultExtent;
        private List<Extent> _extents;
        private List<Extent> _localExtents;
        private List<Extent> _originalExtents;
        private ICoordinate _point0;
        private ICoordinate _point1;
        private Point _mouseDownPoint;
        private Bitmap _originalBitmap;
        private Rectangle _selectedRectangle;
        #endregion Fields

        #region Constructors
        private STMapDesigner()
        {
            InitializeComponent();
            _selectedExtentName = "";
            _localExtents = new List<Extent>();
            _originalExtents = new List<Extent>();
            _generatingImage = false;
            //_userDrawnExtent = null;
            _defaultExtent = new Extent();
            _userDrawnExtent = new Extent();
        }
        public STMapDesigner(STMap stMap, List<Extent> extents)
            : this()
        {
            _stMapOriginal = stMap;
            // Make _stMap as a deep clone of stMap
            // STMapDesigner will change only _stMap unless/until OK button is clicked
            _stMap = new STMap(stMap);
            _originalDesiredExtentName = _stMap.DesiredExtentName;
            _originalBackgroundImageBrightness = _stMap.BackgroundImageBrightness;
            if (!Extent.ExtentIsNull(_stMap.Extent))
            {
                _selectedExtentName = _stMap.Extent.Name;
                _selectedExtent = _stMap.Extent;
            }
            _extents = extents;
            for (int i = 0; i < extents.Count; i++)
            {
                if (!extents[i].IsNull)
                {
                    Extent newExtent = new Extent(extents[i]);
                    _localExtents.Add(newExtent);
                    _originalExtents.Add(newExtent);
                }
            }
            populateExtentsComboBox();
            splitContainer1.Panel2.Controls.Add(_stMap.MapControl);
            //
            panelMapLegend.Controls.Add(_stMap.STMapLegend);
            // Do not do the following...do not allow STMapDesigner to alter 
            // data series, unless there is a real need to do so.
            //
            //for (int i = 0; i < stMap.DataSeriesCount; i++)
            //{
            //    DataSeries ds = new DataSeries(stMap.DataSeriesList[i]);
            //    _dataSeriesList.Add(ds);
            //}

            if (!_stMap.EventHandlersAdded)
            {
                // Connect STMapLegend events to event handlers. 
                _stMap.STMapLegend.LayerVisibilityChanged +=
                    new EventHandler<EventArgs>(mapLegend_LayerVisibilityChanged);

                // Connect MapControl events to event handlers
                _stMap.MapControl.MouseDown += new System.Windows.Forms.MouseEventHandler(MapControl_MouseDown);
                _stMap.MapControl.MouseMove += new System.Windows.Forms.MouseEventHandler(MapControl_MouseMove);
                _stMap.MapControl.MouseUp += new System.Windows.Forms.MouseEventHandler(MapControl_MouseUp);
                _stMap.EventHandlersAdded = true;
            }

            // Initialize points of interest
            _mouseDownPoint = new Point();
            initializePoints();
        }
        #endregion Constructors

        #region Properties
        public STMap STMap
        {
            get
            {
                return _stMap;
            }
            set
            {
                _stMap = value;
            }
        }
        public Extent Extent
        {
            get
            {
                return _selectedExtent;
            }
            private set
            {
                _selectedExtent = value;
                if (value != null)
                {
                    _stMap.Extent = _selectedExtent;
                    _selectedExtentName = _selectedExtent.Name;
                }
                else
                {
                    _stMap.Extent = null;
                    _selectedExtentName = "";
                }
            }
        }
        #endregion Properties

        #region Methods
        private void initializePoints()
        {
            _point0 = null;
            _point1 = null;
            _mouseDownPoint.X = -1;
            _mouseDownPoint.Y = -1;
        }
        private void refreshMap()
        {
            refreshMap(false);
        }
        private void refreshMap(bool forceBackgroundRebuild)
        {
            if (!_generatingImage && !_loading)
            {
                if (_mapNeedsRefresh)
                {
                    _generatingImage = true;
                    enableControls(false);
                    _stMap.MapControl.ClearLayers();
                    _stMap.STMapLegend.Clear(true);
                    _stMap.NeatLineColor = Color.Aqua;
                    _stMap.BuildMapLayers(false, getMapLayerList(), getExtentFromExtentTab(), ckbxBackgroundImage.Checked, forceBackgroundRebuild);
                    if (GlobalStaticVariables.BackgroundImageLayer != null)
                    {
                        if (GlobalStaticVariables.BackgroundImageLayer.Extent is Extent)
                        {
                            ensureListIncludesExtent((Extent)GlobalStaticVariables.BackgroundImageLayer.Extent);
                            populateExtentsComboBox();
                        }
                    }
                    if (!_processingMouseDown && !_processingMouseUp)
                    {
                        if (_originalBitmap == null)
                        {
                            assignOriginalBitmap(true);
                        }
                    }
                    enableControls(true);
                    _generatingImage = false;
                    _mapNeedsRefresh = false;
                }
            }
        }
        private void assignOriginalBitmap(bool renderImage)
        {
            if (renderImage)
            {
                if (_stMap.MapControl != null)
                {
                    if (_selectedExtent != null)
                    {
                        _stMap.MapControl.MapExtent = _selectedExtent;
                    }
                    else
                    {
                        _stMap.MapControl.SizeToFullExtent();
                        //_stMap.MapControl.MapExtent = generateAutomaticExtent();
                    }
                    _originalBitmap = (Bitmap)_stMap.MapControl.RenderAsImage();
                }
            }
            else
            {
                if (_stMap.MapControl.Image != null)
                {
                    if (_originalBitmap != null)
                    {
                        _originalBitmap.Dispose();
                        _originalBitmap = null;
                    }
                    _originalBitmap = new Bitmap(_stMap.MapControl.Image);
                }
            }
        }
        private void clearManualTextBoxes()
        {
            tbExtentName.Text = "";
            tbNorth.Text = "";
            tbWest.Text = "";
            tbSouth.Text = "";
            tbEast.Text = "";
        }
        private void populateManualTextBoxes()
        {
            // Populate Manual extent name and coordinates
            if (_selectedExtentName == "")
            {
                if (_stMap.Extent != null)
                {
                    populateManualTextBoxes(_stMap.Extent);
                }
            }
            else
            {
                Extent extent = getExtentFromList();
                if (extent != null)
                {
                    populateManualTextBoxes(extent);
                }
            }
            btnSaveExtent.Enabled = false;
        }
        private void populateManualTextBoxes(Extent extent)
        {
            if (extent != null)
            {
                tbExtentName.Text = extent.Name;
                tbNorth.Text = extent.North.ToString();
                tbWest.Text = extent.West.ToString();
                tbSouth.Text = extent.South.ToString();
                tbEast.Text = extent.East.ToString();
            }
        }
        private void setupExtentControls()
        {
            // Enable or disable controls
            comboBoxExtents.Enabled = rbSelectFromList.Checked;
            tbExtentName.Enabled = rbManual.Checked;
            tbWest.Enabled = rbManual.Checked;
            tbSouth.Enabled = rbManual.Checked;
            tbEast.Enabled = rbManual.Checked;
            tbNorth.Enabled = rbManual.Checked;

            btnSaveExtent.Enabled = false;

            lblName.Enabled = rbManual.Checked;
            lblNorth.Enabled = rbManual.Checked;
            lblWest.Enabled = rbManual.Checked;
            lblSouth.Enabled = rbManual.Checked;
            lblEast.Enabled = rbManual.Checked;
        }
        private void selectExtent(Extent extent)
        {
            bool found = false;
            for (int i = 0; i < comboBoxExtents.Items.Count; i++)
            {
                if (extent.Name == ((Extent)comboBoxExtents.Items[i]).Name)
                {
                    found = true;
                    comboBoxExtents.SelectedIndex = i;
                }
            }
            if (!found)
            {
                comboBoxExtents.Items.Add(extent);
                comboBoxExtents.SelectedIndex = comboBoxExtents.Items.Count - 1;
            }
        }
        private void populateExtentsComboBox()
        {
            int selectedIndex = -1;
            comboBoxExtents.Items.Clear();
            if (_localExtents != null)
            {
                for (int i = 0; i < _localExtents.Count; i++)
                {
                    comboBoxExtents.Items.Add(_localExtents[i]);
                    if (_selectedExtentName != "" && selectedIndex < 0)
                    {
                        if (_localExtents[i].Name == _selectedExtentName)
                        {
                            selectedIndex = i;
                        }
                    }
                }
                if (selectedIndex >= 0)
                {
                    comboBoxExtents.SelectedIndex = selectedIndex;
                }
                else
                {
                    if (comboBoxExtents.Items.Count > 0)
                    {
                        comboBoxExtents.SelectedIndex = 0;
                    }
                }
            }
        }
        private void refreshMapLegend()
        {
            _stMap.STMapLegend.FitWidth(panelMapLegend.Width);
        }
        private bool saveManualExtent()
        {
            bool replace = false;
            // Check for errors
            if (tbExtentName.Text == "")
            {
                string msg = "Error: Extent name is blank";
                MessageBox.Show(msg);
                return false;
            }
            for (int i = 0; i < comboBoxExtents.Items.Count; i++)
            {
                if (tbExtentName.Text == comboBoxExtents.Items[i].ToString())
                {
                    string text = "Replace existing extent?";
                    string caption = "Confirmation of change to extent";
                    MessageBoxButtons btns = MessageBoxButtons.OKCancel;
                    DialogResult dr = MessageBox.Show(text, caption, btns);
                    if (dr == DialogResult.OK)
                    {
                        if (tbExtentName.Text == WorkspaceUtil.MODEL_GRID_EXTENT_NAME)
                        {
                            string errMsg = "The extent named \"" + WorkspaceUtil.MODEL_GRID_EXTENT_NAME + 
                                            "\" may not be revised.  Provide a different name.";
                            MessageBox.Show(errMsg);
                            return false;
                        }
                        else if (tbExtentName.Text == WorkspaceUtil.BACKGROUND_IMAGE_EXTENT_NAME)
                        {
                            string errMsg = "The extent named \"" + WorkspaceUtil.BACKGROUND_IMAGE_EXTENT_NAME +
                                            "\" may not be revised.  Provide a different name.";
                            MessageBox.Show(errMsg);
                            return false;
                        }
                        else
                        {
                            replace = true;
                        }
                    }
                    else
                    {
                        // No need to diplay message when user clicks Cancel
                        return false;
                    }
                    break;
                }
            }

            // Save new extent (or revise current extent) based on contents of text boxes
            try
            {
                if (replace)
                {
                    Extent extent = getExtentByName(tbExtentName.Text);
                    bool OK = assignManualCoordinates(extent);
                    if (!OK)
                    {
                        string msg = "Error assigning extent coordinates";
                        MessageBox.Show(msg);
                        return false;
                    }
                    btnSaveExtent.Enabled = false;
                    _mapNeedsRefresh = true;
                    return true;
                }
                else
                {
                    Extent newExtent = getManualExtent();
                    if (!Extent.ExtentIsNull(newExtent))
                    {
                        _localExtents.Add(newExtent);
                        _selectedExtentName = newExtent.Name;
                        populateExtentsComboBox();
                        rbSelectFromList.Checked = true;
                        setupExtentControls();
                        btnSaveExtent.Enabled = false;
                        _mapNeedsRefresh = true;
                        return true;
                    }
                    else
                    {
                        string msg = "Error encountered in defining extent";
                        MessageBox.Show(msg);
                        return false;
                    }
                }
            }
            catch
            {
                string msg = "Error encountered in defining extent";
                MessageBox.Show(msg);
                return false;
            }
        }
        private void selectExtentFromList()
        {
            if (comboBoxExtents.Items.Count > 0)
            {
                int selIndex = comboBoxExtents.SelectedIndex;
                if (selIndex < 0)
                {
                    selIndex = 0;
                    comboBoxExtents.SelectedIndex = 0;
                }
            }
        }
        private void ensureListIncludesExtent(Extent extent)
        {
            if (extent != null)
            {
                // First, search list to find out if _localExtents already contains specified extent
                string extentName = extent.Name;
                bool extentFound = false;
                for (int i = 0; i < _localExtents.Count; i++)
                {
                    if (_localExtents[i].Name == extentName)
                    {
                        extentFound = true;
                    }
                }

                // If _localExtents does not contain specified extent, add it
                if (!extentFound)
                {
                    _localExtents.Add(extent);
                }
            }
        }
        private Extent getExtentByName(string extentName)
        {
            for (int i = 0; i < _localExtents.Count; i++)
            {
                if (_localExtents[i].Name == extentName)
                {
                    return _localExtents[i];
                }                
            }
            return null;
        }
        private Extent getExtentFromExtentTab()
        {
            if (rbAutomatic.Checked)
            {
                return generateAutomaticExtent();
            }
            if (rbSelectFromList.Checked)
            {
                return getExtentFromList();
            }
            if (rbManual.Checked)
            {
                return getManualExtent();
            }
            return null;
        }
        private Extent getExtentFromList()
        {
            selectExtentFromList();
            for (int i = 0; i < _localExtents.Count; i++)
            {
                if (_localExtents[i].Name == _selectedExtentName)
                {
                    return _localExtents[i];
                }
            }
            return null;
        }
        private Extent getManualExtent()
        {
            try
            {
                double west = Convert.ToDouble(tbWest.Text);
                double south = Convert.ToDouble(tbSouth.Text);
                double east = Convert.ToDouble(tbEast.Text);
                double north = Convert.ToDouble(tbNorth.Text);
                Extent newExtent = new Extent(west, south, east, north);
                newExtent.Name = tbExtentName.Text;
                return newExtent;
            }
            catch
            {
                return null;
            }
        }
        private Extent generateAutomaticExtent()
        {
            return Extent.GetEnclosingExtent(_extents);
        }
        private List<STMapLayer> getMapLayerList()
        {
            List<STMapLayer> list = new List<STMapLayer>();
            for (int i = 0; i < listBoxLayers.Items.Count; i++)
            {
                list.Add((STMapLayer)listBoxLayers.Items[i]);
            }
            return list;
        }
        private void prepareToGenerateBackgroundImage()
        {
            enableControls(false);
        }
        private void enableControls(bool enable)
        {
            // Layout tab
            btnOK.Enabled = enable;
            btnCancel.Enabled = enable;
            tbName.Enabled = enable;
            ckbxBackgroundImage.Enabled = enable;
            panelMapLegend.Enabled = enable;

            // Extent tab
            rbAutomatic.Enabled = enable;
            rbManual.Enabled = enable;
            rbSelectFromList.Enabled = enable;
            if (enable)
            {
                setupExtentControls();
            }
            else
            {
                tbExtentName.Enabled = false;
                tbNorth.Enabled = false;
                tbWest.Enabled = false;
                tbEast.Enabled = false;
                tbSouth.Enabled = false;
                comboBoxExtents.Enabled = false;
                btnSaveExtent.Enabled = false;
            }

            // Layers tab
            buttonDown.Enabled = enable;
            buttonRemove.Enabled = enable;
            buttonUp.Enabled = enable;

            WaitCursor(!enable);
        }
        private bool assignManualCoordinates(Extent extent)
        {
            try
            {
                extent.West = Convert.ToDouble(tbWest.Text);
                extent.South = Convert.ToDouble(tbSouth.Text);
                extent.East = Convert.ToDouble(tbEast.Text);
                extent.North = Convert.ToDouble(tbNorth.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion Methods

        #region Form event handlers
        private void STMapDesigner_Load(object sender, EventArgs e)
        {
            _loading = true;
            _pressedOk = false;
            _point0 = null;
            _point1 = null;
            tbName.Text = _stMap.Name;
            Height = _stMap.MapDesignerHeight;
            Width = _stMap.MapDesignerWidth;
            hScrollBar1.Value = _stMap.BackgroundImageBrightness;
            textBoxBrightness.Text = _stMap.BackgroundImageBrightness.ToString();

            ckbxBackgroundImage.Checked = _stMap.ShowBackgroundImage;

            // Set up Extent tab
            clearManualTextBoxes();
            populateExtentsComboBox();
            rbAutomatic.Checked = false;
            rbSelectFromList.Checked = false;
            rbManual.Checked = false;
            if (Extent.ExtentIsNull(_stMap.Extent))
            {
                rbAutomatic.Checked = true;
                _stMap.Extent = generateAutomaticExtent();
            }
            else
            {
                if (comboBoxExtents.Items.Count > 0)
                {
                    rbSelectFromList.Checked = true;
                }
                else
                {
                    rbManual.Checked = true;
                }
            }
            populateManualTextBoxes();
            setupExtentControls();

            // Set up Layers tab

            // Populate the layers list box with the map layers.
            for (int i = 0; i <_stMap.DataSeriesCount(); i++)
            {
                listBoxLayers.Items.Add(_stMap.GetSTMapLayer(i));
            }

            Extent = getExtentFromExtentTab();
            populateExtentsComboBox();
            _loading = false;
            _mapNeedsRefresh = true;
            _displayingImage = false;
            _processingMouseUp = false;
            _processingMouseDown = false;
            _layoutTabNeedsRefresh = true;
            btnSaveUserDrawnExtent.Enabled = false;
            WaitCursor(false);
        }
        private void STMapDesigner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _stMap.MapDesignerHeight = Height;
            _stMap.MapDesignerWidth = Width;
            if (!_pressedOk)
            {
                PrepareToCancel();
            }
        }
        private void STMapDesigner_ResizeEnd(object sender, EventArgs e)
        {
            _stMap.MapControl.GenerateImage = true;
            refreshMap();
            if (_originalBitmap != null)
            {
                assignOriginalBitmap(false);
            } 
        }
        #endregion Form event handlers

        #region Control event handlers
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStrip1.Items["menuItemAddLayer"].Enabled = false;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _pressedOk = true;
            _stMapOriginal.Name = tbName.Text;
            _stMapOriginal.ShowBackgroundImage = ckbxBackgroundImage.Checked;
            bool extentIsAutomatic = false;

            // Save the extent
            if (rbAutomatic.Checked)
            {
                _stMapOriginal.Extent = null;
                extentIsAutomatic = true;
            }
            else if (rbSelectFromList.Checked)
            {
                _stMapOriginal.Extent = getExtentFromList();
            }
            else if (rbManual.Checked)
            {
                if (btnSaveExtent.Enabled)
                {
                    if (saveManualExtent())
                    {
                        _stMapOriginal.Extent = getExtentFromList();
                    }
                }
            }
            _extents.Clear();            
            for (int i = 0; i < _localExtents.Count; i++)
            {
                _extents.Add(_localExtents[i]);
            }
            if (_extents.Count == 0)
            {
                _stMapOriginal.DesiredExtentName = "";
            }
            else if (_extents.Count==1)
            {
                _stMapOriginal.DesiredExtentName = _extents[0].Name;
            }

            // Save the map layers.
            _stMapOriginal.ClearMapLayerList();
            _stMapOriginal.ClearDataSeries();
            for (int i = 0; i < listBoxLayers.Items.Count; i++)
            {
                _stMapOriginal.AddLayer((STMapLayer)listBoxLayers.Items[i]);
                _stMapOriginal.DataSeriesList[i].Visible = ((STMapLayer)listBoxLayers.Items[i]).Visible;
                _stMapOriginal.DataSeriesList[i].ConvertFlowToFlux = ((STMapLayer)listBoxLayers.Items[i]).ConvertFlowToFlux;
            }

            if (extentIsAutomatic)
            {
                _stMapOriginal.DesiredExtentName = "";
                _stMapOriginal.Extent = null;
            }
            else
            {
                _stMapOriginal.DesiredExtentName = _stMap.DesiredExtentName;
                _stMapOriginal.Extent = new Extent(_stMap.Extent);                
            }
            _stMapOriginal.BackgroundImageBrightness = _stMap.BackgroundImageBrightness;

            // Reconstruct the MapControl layer list
            _stMapOriginal.MapControl.ClearLayers();
            for (int i = 0; i < _stMap.MapControl.LayerCount; i++)
            {
                _stMapOriginal.MapControl.AddLayer(_stMap.MapControl.GetLayer(i));
            }
            if (_stMap.MapControl.Image != null)
            {
                _stMapOriginal.MapControl.Image = new Bitmap(_stMap.MapControl.Image);
            }

            if (_stMap.MapControl.MapExtent == null || extentIsAutomatic)
            {
                _stMapOriginal.MapControl.SizeToFullExtent();
            }
            else
            {
                _stMapOriginal.MapControl.MapExtent = new Extent(_stMap.MapControl.MapExtent);
            }
        }
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            refreshMapLegend();
        }
        private void tbExtentName_TextChanged(object sender, EventArgs e)
        {
            btnSaveExtent.Enabled = true;
        }
        private void tbNorth_TextChanged(object sender, EventArgs e)
        {
            btnSaveExtent.Enabled = true;
        }
        private void tbWest_TextChanged(object sender, EventArgs e)
        {
            btnSaveExtent.Enabled = true;
        }
        private void tbEast_TextChanged(object sender, EventArgs e)
        {
            btnSaveExtent.Enabled = true;
        }
        private void tbSouth_TextChanged(object sender, EventArgs e)
        {
            btnSaveExtent.Enabled = true;
        }
        private void btnSaveExtent_Click(object sender, EventArgs e)
        {
            saveManualExtent();
        }
        private void rbSelectFromList_Click(object sender, EventArgs e)
        {
            selectExtentFromList();
            setupExtentControls();
        }
        private void rbManual_Click(object sender, EventArgs e)
        {
            setupExtentControls();
            populateManualTextBoxes();
        }
        private void rbAutomatic_Click(object sender, EventArgs e)
        {
            _selectedExtentName = "";
            setupExtentControls();
            _mapNeedsRefresh = true;
        }
        private void comboBoxExtents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxExtents.SelectedItem != null)
            {
                string extentName = comboBoxExtents.SelectedItem.ToString();
                for (int i = 0; i < _localExtents.Count; i++)
                {
                    if (_localExtents[i].Name == extentName)
                    {
                        _selectedExtentName = _localExtents[i].Name;
                        _stMap.DesiredExtentName = _selectedExtentName;
                        _mapNeedsRefresh = true;
                        break;
                    }
                }
            }
            populateManualTextBoxes();
        }
        private void tabPageExtent_Leave(object sender, EventArgs e)
        {
            Extent = getExtentFromExtentTab();
            if (!Extent.ExtentIsNull(Extent))
            {
                _selectedExtentName = Extent.Name;
                _selectedExtent = Extent;
            }
            else
            {
                _selectedExtentName = "";
                _selectedExtent = null;
            }
        }
        private void tabPageLayout_Enter(object sender, EventArgs e)
        {
            if (_layoutTabNeedsRefresh)
            {
                Extent = getExtentFromExtentTab();
                _stMap.MapControl.GenerateImage = true; // Causes RefreshMap to recreate Image
                _originalBitmap = null; // Causes RefreshMap to copy Image to _originalBitmap
                refreshMap();
                btnSaveUserDrawnExtent.Enabled = false;
                //assignOriginalBitmap(true);
            }
        }
        private void tabPageExtent_Enter(object sender, EventArgs e)
        {
            populateExtentsComboBox();
        }
        #endregion Control event handlers

        #region mapLegend Event Handlers
        /// <summary>
        /// Handles the LayerVisibilityChanged event of the mapLegend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapLegend_LayerVisibilityChanged(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region MapControl Event Handlers
        void MapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!_processingMouseUp && !_processingMouseDown)
            {
                _processingMouseDown = true;
                //if (_originalBitmap != null)
                //{
                //    _originalBitmap.Dispose();
                //}
                //_originalBitmap = new Bitmap(_stMap.MapControl.Image);
                initializePoints();
                _point0 = _stMap.ToMapPoint(e.X, e.Y);
                _mouseDownPoint.X = e.X;
                _mouseDownPoint.Y = e.Y;
                _point1 = null;
                restoreOriginalBitmap();
                //_userDrawnExtent = null;
                //_processingMouseDown = false;
            }
        }
        void MapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_stMap != null)
            {
                // Display coordinates
                ICoordinate pt = _stMap.ToMapPoint(e.X, e.Y);
                textBoxXCoord.Text = String.Format("{0:0.0}", pt.X);
                textBoxYcoord.Text = String.Format("{0:0.0}", pt.Y);
                if (_point0 != null)
                {
                    // Display image with rectangle
                    Point currentPoint = new Point(e.X, e.Y);
                    displayImageWithRectangle(currentPoint);
                    _stMap.MapControl.GenerateImage = false;
                }
            }
        }
        void MapControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!_processingMouseUp)
            {
                _processingMouseUp = true;
                _point1 = _stMap.ToMapPoint(e.X, e.Y);
                if (_point0 != null && _point1 != null)
                {
                    if (_point0.Equals(_point1))
                    {
                        btnSaveUserDrawnExtent.Enabled = false;
                        string msg = "Coordinates: (" + String.Format("{0:0.0}", _point0.X) + ", " + String.Format("{0:0.0}", _point0.Y) +
                            ")";
                        MessageBox.Show(msg);
                        _stMap.MapControl.GenerateImage = true;
                    }
                    else
                    {
                        // Display image with rectangle
                        Point currentPoint = new Point(e.X, e.Y);
                        displayImageWithRectangle(currentPoint);
                        _selectedRectangle = GeometryHelper.MakeRectangle(_mouseDownPoint, currentPoint);

                        // Create an extent, and enable buttun to allow user to name and store the new extent
                        Extent tempExtent = new Extent(_point0, _point1);
                        _userDrawnExtent.CopyFrom(tempExtent);
                        btnSaveUserDrawnExtent.Enabled = true;
                        _stMap.MapControl.GenerateImage = false;
                    }
                }
                initializePoints();
                _processingMouseUp = false;
                _processingMouseDown = false;
                enableControls(true);
                this.Invalidate(true);
                this.Refresh();
            }
        }
        #endregion MapControl Event Handlers

        private void displayImageWithRectangle(Point currentPoint)
        {
            if (!_displayingImage)
            {
                _displayingImage = true;
                Rectangle currentRectangle = GeometryHelper.MakeRectangle(_mouseDownPoint, currentPoint);
                Bitmap bitmapWithRectangle = drawRectangleOnOriginalBitmap(currentRectangle);
                displayImage(bitmapWithRectangle);
                _displayingImage = false;
            }
        }
        private Bitmap drawRectangleOnOriginalBitmap(Rectangle rectangle)
        {
            // Make a copy of the original image (without rectangle)
            Bitmap newBitmap = new Bitmap(_originalBitmap); 

            // Draw the rectangle on the copy
            System.Drawing.Graphics g = null;
            try
            {
                g = System.Drawing.Graphics.FromImage(newBitmap);
                g.PageUnit = System.Drawing.GraphicsUnit.Pixel;
                Pen pen = new Pen(Color.Aquamarine, 2.0f);
                g.DrawRectangle(pen, rectangle);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (g != null)
                { g.Dispose(); }
            }
            return newBitmap;
        }
        private void restoreOriginalBitmap()
        {
            if (_originalBitmap != null)
            {
                _stMap.MapControl.Image = new Bitmap(_originalBitmap);
                _stMap.MapControl.Refresh();
            }
        }
        private void displayImage(Bitmap newImage)
        {
            if (newImage != null)
            {
                _stMap.MapControl.GenerateImage = false;
                _stMap.MapControl.Image = new Bitmap(newImage);
                _stMap.MapControl.Invalidate(true);
                _stMap.MapControl.Refresh();
                _stMap.MapControl.GenerateImage = true;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            // If there is an item selected in the list box, remove it.
            if (listBoxLayers.SelectedIndex >= 0)
            {
                listBoxLayers.Items.RemoveAt(listBoxLayers.SelectedIndex);
                _mapNeedsRefresh = true;
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            // If there is a selected element in the list box (not at index zero), move it up one index.
            if (listBoxLayers.SelectedIndex > 0)
            {
                int index = listBoxLayers.SelectedIndex;
                object item = listBoxLayers.Items[index];
                listBoxLayers.Items.RemoveAt(index);
                listBoxLayers.Items.Insert(index - 1, item);

                // Select the item that was previously selected.
                listBoxLayers.SelectedIndex = index - 1;
                _mapNeedsRefresh = true;
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            // If there is a selected element in the list box (not at the bottom of the list), move it up one index.
            if (listBoxLayers.SelectedIndex < listBoxLayers.Items.Count - 1)
            {
                int index = listBoxLayers.SelectedIndex;
                object item = listBoxLayers.Items[index];
                listBoxLayers.Items.RemoveAt(index);
                listBoxLayers.Items.Insert(index + 1, item);

                // Select the item that was previously selected.
                listBoxLayers.SelectedIndex = index + 1;
                _mapNeedsRefresh = true;
            }
        }

        private void ckbxBackgroundImage_Click(object sender, EventArgs e)
        {
            
        }

        private void PrepareToCancel()
        {
            // Restore original desired extent name and background image brightness
            _stMap.DesiredExtentName = _originalDesiredExtentName;
            _stMap.BackgroundImageBrightness = _originalBackgroundImageBrightness;

            // Restore visibility settings of graphics layers on form load
            STMapLayer tempSTMapLayer;
            for (int i = 0; i < _stMap.STMapLayerCount; i++)
            {
                tempSTMapLayer = _stMap.GetSTMapLayer(i);
                tempSTMapLayer.Visible = tempSTMapLayer.DataSeries.Visible;
                tempSTMapLayer.ConvertFlowToFlux = tempSTMapLayer.DataSeries.ConvertFlowToFlux;
            }

            // Restore original list of extents
            _extents.Clear();
            for (int i = 0; i < _originalExtents.Count; i++)
            {
                _extents.Add(_originalExtents[i]);
            }
        }

        private void tabPageLayout_Layout(object sender, LayoutEventArgs e)
        {
            labelX.Visible = true;
            labelY.Visible = true;
            textBoxXCoord.Visible = true;
            textBoxYcoord.Visible = true;
            textBoxBrightness.Text = _stMap.BackgroundImageBrightness.ToString();
            btnSaveUserDrawnExtent.Visible = true;
            _stMap.MapControl.GenerateImage = true;
        }

        private void tabPageLayers_Layout(object sender, LayoutEventArgs e)
        {
            labelX.Visible = false;
            labelY.Visible = false;
            textBoxXCoord.Visible = false;
            textBoxYcoord.Visible = false;
            btnSaveUserDrawnExtent.Visible = false;
        }

        private void tabPageExtent_Layout(object sender, LayoutEventArgs e)
        {
            labelX.Visible = false;
            labelY.Visible = false;
            textBoxXCoord.Visible = false;
            textBoxYcoord.Visible = false;
            btnSaveUserDrawnExtent.Visible = false;
        }
        private void WaitCursor(bool showWaitCursor)
        {
            if (showWaitCursor)
            {
                this.Cursor = Cursors.WaitCursor;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ckbxBackgroundImage_CheckedChanged(object sender, EventArgs e)
        {
            _mapNeedsRefresh = true;
            refreshMap();
        }

        private void textBoxBrightness_Leave(object sender, EventArgs e)
        {
            string errmsg = "Brightness must be an integer between -255 and 255.";
            try
            {
                int brightness = Convert.ToInt32(textBoxBrightness.Text);
                if (brightness >= -255 && brightness <= 255)
                {
                    hScrollBar1.Value = brightness;
                }
                else
                {
                    MessageBox.Show(errmsg);
                    textBoxBrightness.Focus();
                }
            }
            catch
            {
                MessageBox.Show(errmsg);
                textBoxBrightness.Focus();
            }
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            int brightness = hScrollBar1.Value;
            textBoxBrightness.Text = brightness.ToString();
            _stMap.BackgroundImageBrightness = brightness;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            _stMap.BackgroundImageBrightness = hScrollBar1.Value;
            if (ckbxBackgroundImage.Checked)
            {
                _mapNeedsRefresh = true;
                refreshMap(false);
            }
        }

        private void btnSaveUserDrawnExtent_Click(object sender, EventArgs e)
        {
            if (_userDrawnExtent != null)
            {
                if (!_userDrawnExtent.Equals(_defaultExtent))
                {
                    ExtentDialog extentDialog = new ExtentDialog(_userDrawnExtent, _localExtents);
                    DialogResult dr = extentDialog.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        // Display map using new extent
                        Extent = new Extent(_userDrawnExtent);
                        _extents.Add(Extent);
                        _selectedExtent = Extent;
                        _selectedExtentName = Extent.Name;
                        populateExtentsComboBox();
                        _mapNeedsRefresh = true;
                        _stMap.MapControl.GenerateImage = true; // Causes RefreshMap to recreate Image
                        _originalBitmap = null; // Causes RefreshMap to copy Image to _originalBitmap 
                        refreshMap();
                    }
                    else
                    {
                        restoreOriginalBitmap();
                    }
                    _userDrawnExtent.CopyFrom(_defaultExtent);
                    _layoutTabNeedsRefresh = false; 
                    btnSaveUserDrawnExtent.Enabled = false;
                    _layoutTabNeedsRefresh = true;
                    _stMap.MapControl.GenerateImage = true;
                }
            }
        }

        public void CopyExtentFrom(Extent newExtent)
        {
            Extent.CopyFrom(_userDrawnExtent);
            _stMap.Extent.CopyFrom(_userDrawnExtent);
            _stMap.MapControl.SetExtent(_userDrawnExtent.MinX, _userDrawnExtent.MaxX,
                _userDrawnExtent.MinY, _userDrawnExtent.MaxY);
        }

        private void STMapDesigner_ResizeBegin(object sender, EventArgs e)
        {
            
        }

        private void STMapDesigner_LocationChanged(object sender, EventArgs e)
        {

        }

        private void panelMapLegend_Click(object sender, EventArgs e)
        {

        }
    }
}
