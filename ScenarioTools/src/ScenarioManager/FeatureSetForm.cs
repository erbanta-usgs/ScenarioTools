using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GeoAPI.Geometries;
using ScenarioTools.Reporting;
using ScenarioTools.Scene;
using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;
using USGS.Puma.UI.MapViewer;

using ScenarioTools;
using ScenarioTools.DataClasses;
using ScenarioTools.Spatial;
using ScenarioTools.Util;

namespace ScenarioManager
{
    #region Enumerations
    public enum VarTypes
    {
        Any = -1,
        Integer = 0,
        String = 1,
        IntegerOrString = 2,
        Numeric = 3,
        NumericOrString = 4
    }
    public enum ActiveTool
    {
        Pointer = 0,
        ReCenter = 1,
        ZoomIn = 2,
        ZoomOut = 3
        //DefinePolygon = 4,
        //DefineRectangle = 5,
        //DefineLineString = 6,
        //DefinePoint = 7
    }
    #endregion Enumerations

    public partial class FeatureSetForm : Form
    {
        #region Fields

        private FeatureSet _featureSet;
        private string _previousFeatureSetShapefilePath;
        private DateTime _previousFeatureSetShapefileDateTime;
        private FeatureCollection _featureList = null;
        /// <summary>
        /// A CellCenteredArealGrid object that defines a model grid corresponding to
        /// the dimensions of the current head file.
        /// </summary>
        private USGS.Puma.FiniteDifference.CellCenteredArealGrid _ModelGrid = null;
        /// <summary>
        /// A FeatureLayer containing the interior model grid lines.
        /// </summary>
        private USGS.Puma.UI.MapViewer.FeatureLayer _GridlinesMapLayer = null;
        /// <summary>
        /// A FeatureLayer containing the grid outline.
        /// </summary>
        private USGS.Puma.UI.MapViewer.FeatureLayer _GridOutlineMapLayer = null;
        /// <summary>
        /// A FeatureLayer containing the contours for the selected head data layer.
        /// </summary>
        private USGS.Puma.UI.MapViewer.FeatureLayer _CurrentContourMapLayer = null;
        /// <summary>
        /// A FeatureLayer containing features from a user-selected shapefile
        /// </summary>
        private USGS.Puma.UI.MapViewer.FeatureLayer _ImportedShapefileMapLayer = null;
        private bool _changed;
        private bool _okToClose;
        private bool _populatingControls;
        private bool _shapefileChanged;
        private string _modelGridShapefilePath;
        private string _projectDirectory;

        /// <summary>
        /// A control that displays the contents of the map as a series of map layers.
        /// </summary>
        private USGS.Puma.UI.MapViewer.MapControl _mapControl = null;

        /// <summary>
        /// A control that displays an index map based on the mapControl as its source map
        /// </summary>
        private USGS.Puma.UI.MapViewer.IndexMapControl _indexMap = null;

        /// <summary>
        /// A control that displays a legend for the mapControl.
        /// Map layers are added manually to the legend control.
        /// </summary>
        private STMapLegend _stMapLegend = null;

        /// <summary>
        /// A control for determining the layer to which recharge applies
        /// </summary>
        private GroupBoxLayerAssignment _groupBoxRchLayerAssignment;

        private int _numSupportedPackages = 5;
        //private List<TabPage> _tabPages;

        // Map tools and items
        private Cursor _reCenterCursor = null;
        private Cursor _zoomInCursor = null;
        private Cursor _zoomOutCursor = null;
        private bool _processingActiveToolButtonSelection = false;
        private ActiveTool _activeTool = ActiveTool.Pointer;

        #endregion Fields

        public FeatureSetForm(PackageType packageType)
        {
            InitializeComponent();
            
            #region Create and initialize map component controls
            // The mapControl control is defined programmatically in the following
            // blocks of code rather than through the visual form designer. 
            // This is a little more work up front,
            // but it is safer to do it this way if the classes for those 
            // controls are undergoing development and changing frequently.

            _mapControl = new MapControl();
            _mapControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            _mapControl.TabIndex = 0;
            _mapControl.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(_mapControl);
            // Connect mapControl events to event handlers
            _mapControl.MouseDoubleClick += new MouseEventHandler(mapControl_MouseDoubleClick);
            _mapControl.MouseClick += new MouseEventHandler(mapControl_MouseClick);
            _mapControl.MouseMove += new MouseEventHandler(mapControl_MouseMove);

            // mapLegend: create, initialize, and add control to panelMapLegend container
            _stMapLegend = new STMapLegend();
            _stMapLegend.BorderStyle = BorderStyle.FixedSingle;
            _stMapLegend.TabIndex = 0;
            _stMapLegend.Dock = DockStyle.Fill;
            _stMapLegend.AutoSize = true;
            panelExplanation.Controls.Add(_stMapLegend);

            // Connect mapLegend events to event handlers
            _stMapLegend.LayerVisibilityChanged += new EventHandler<EventArgs>(mapLegend_LayerVisibilityChanged);

            // indexMap: create, initialize, and add control to panelIndexMap container
            _indexMap = new IndexMapControl();
            _indexMap.SourceMap = _mapControl; // this associates the mapControl with the indexMap control
            _indexMap.SuppressMapImageUpdate = false;
            _indexMap.BorderStyle = BorderStyle.FixedSingle;
            _indexMap.TabIndex = 0;
            _indexMap.Dock = DockStyle.Fill;
            panelIndexMap.Controls.Add(_indexMap);

            #endregion

            // Initialize other fields
            _previousFeatureSetShapefilePath = "";
            _previousFeatureSetShapefileDateTime = DateTime.MinValue;
            if (packageType == PackageType.RchType)
            {
                _groupBoxRchLayerAssignment = new GroupBoxLayerAssignment();
                pgRch.Controls.Add(_groupBoxRchLayerAssignment);
            }

            // Save package-specific tab pages and remove them; they will be added as needed
            //_tabPages = new List<TabPage>();
            //for (int i = 1; i <= _numSupportedPackages; i++)
            //{
            //    _tabPages.Add(tabControl1.TabPages[i]);
            //}
            for (int i = _numSupportedPackages; i >= 1; i--)
            {
                string s1 = (string)tabControl1.TabPages[i].Tag;
                string s2 = packageType.ToString();
                if ((string)tabControl1.TabPages[i].Tag != packageType.ToString())
                {
                    tabControl1.TabPages.Remove(tabControl1.TabPages[i]);
                }
            }

            // Create MapControl cursors
            _reCenterCursor = MapControl.CreateCursor(MapControlCursor.ReCenter);
            _zoomInCursor = MapControl.CreateCursor(MapControlCursor.ZoomIn);
            _zoomOutCursor = MapControl.CreateCursor(MapControlCursor.ZoomOut);
            _activeTool = ActiveTool.Pointer;
        }

        #region Properties
        public int Nlay { get; set; }
        public bool Changed
        {
            get
            {
                return _changed;
            }
            private set
            {
                _changed = value;
            }
        }
        public bool MustBeMaster
        {
            get
            {
                if (_featureSet.Type == PackageType.RchType)
                {
                    return _groupBoxRchLayerAssignment.MustBeMaster;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (_featureSet.Type == PackageType.RchType)
                {
                    _groupBoxRchLayerAssignment.MustBeMaster = value;
                }
            }
        }
        public string ModelGridShapefilePath
        {
            get
            {
                return _modelGridShapefilePath;
            }
            set
            {
                _modelGridShapefilePath = value;
            }
        }
        public string ProjectDirectory
        {
            private get
            {
                return _projectDirectory;
            }
            set
            {
                _projectDirectory = value;
            }
        }
        public FeatureSet FeatureSet
        { 
            get
            {
                return _featureSet;
            }
            set
            {
                _featureSet = value;
            }
        }
        public GroupBoxLayerAssignment GroupBoxRchLayerAssignment
        {
            get
            {
                return _groupBoxRchLayerAssignment;
            }
        }
        public USGS.Puma.FiniteDifference.CellCenteredArealGrid ModelGrid
        {
            get
            {
                return _ModelGrid;
            }
            set
            {
                _ModelGrid = value;
                _GridlinesMapLayer = null;
                _GridOutlineMapLayer = null;

                // Define _GridlinesMapLayer and _GridOutlineMapLayer
                _GridlinesMapLayer = CreateModelGridlinesLayer(_ModelGrid, Color.DarkGray);
                _GridOutlineMapLayer = CreateModelGridOutlineLayer(_ModelGrid, Color.Black);
            }
        }
        public ImageLayer ImageLayerBackground { get; set; }
        #endregion Properties

        #region Form Event Handlers

        private void btnFindShapefile_Click(object sender, EventArgs e)
        {
            FindFeatureSetFile(PackageType.NoType, FileType.Shapefile);
        }

        private void btnFindSmpFile_Click(object sender, EventArgs e)
        {
            FindFeatureSetFile(PackageType.NoType, FileType.Smpfile);
        }

        private void tbShapefile_TextChanged(object sender, EventArgs e)
        {
            _featureSet.ShapefileAbsolutePath = tbShapefile.Text;
            _shapefileChanged = true;
            ProcessFeatureSetShapefile();
            Changed = true;
        }

        private void tbSmp_TextChanged(object sender, EventArgs e)
        {
            _featureSet.TimeSeriesAbsolutePath = tbSmp.Text;
            Changed = true;
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            _featureSet.Name = tbName.Text;
            Changed = true;
        }

        private void FeatureSetForm_Load(object sender, EventArgs e)
        {
            _okToClose = true;
            tbName.Text = _featureSet.Name;
            tbShapefile.Text = _featureSet.ShapefileAbsolutePath;
            tbSmp.Text = _featureSet.TimeSeriesAbsolutePath;
            bool piecewise = _featureSet.InterpretationMethod == TimeSeriesInterpretationMethod.Piecewise;
            rbPiecewise.Checked = piecewise;
            rbStepwise.Checked = !piecewise;

            switch (_featureSet.Type)
            {
                case PackageType.WellType:
                    ConfigureForWel();
                    break;
                case PackageType.RiverType:
                    ConfigureForRiv();
                    break;
                case PackageType.ChdType:
                    ConfigureForChd();
                    break;
                case PackageType.RchType:
                    ConfigureForRch();
                    break;
                case PackageType.GhbType:
                    ConfigureForGhb();
                    break;

            }
            _shapefileChanged = false;
            _populatingControls = true;
            cmbxKeyField.Items.Clear();
            cmbxKeyField.Text = "";
            cmbxLabelFeatures.SelectedIndex = (int)_featureSet.LabelFeatures;
            _mapControl.ClearLayers();
            _stMapLegend.Clear(true);
            ProcessFeatureSetShapefile();
            if (_featureSet.Type == PackageType.RchType)
            {
                _groupBoxRchLayerAssignment.Npkgop = _featureSet.PackageOption;
                _groupBoxRchLayerAssignment.LayerAttribute = _featureSet.LayerAttribute;
            }
            _populatingControls = false;
            _changed = false;
        }

        private void ConfigureForGhb()
        {
            this.Text = "GHB Package Feature Set";
            this.Icon = Properties.Resources.GhbIconSet;
            grpboxSmpInterpretationMethod.Enabled = true;
            textBoxGhbSecondarySmp.Text = _featureSet.TimeSeriesSecondaryAbsolutePath;
            rbStepwise.Enabled = true;
            rbPiecewise.Enabled = true;
            cmbxLabelFeatures.Enabled = true;

            #region Layer assignment
            cmbxGhbLayerAttribute.Items.Clear();
            cmbxGhbLayerAttribute.Text = "";
            udGhbLayerPicker.Value = _featureSet.DefaultLayer;
            if (Nlay > 0)
            {
                udGhbLayerPicker.Maximum = Nlay;
            }
            switch (_featureSet.LayMethod)
            {
                case LayerMethod.Uniform:
                    // radio buttons
                    rbGhbSameLayer.Checked = true;
                    rbGhbLayerAttribute.Checked = false;
                    // combo boxes
                    cmbxGhbLayerAttribute.Enabled = false;
                    // numeric up-down
                    udGhbLayerPicker.Enabled = true;
                    break;
                case LayerMethod.ByAttribute:
                    // radio buttons
                    rbGhbSameLayer.Checked = false;
                    rbGhbLayerAttribute.Checked = true;
                    // combo boxes
                    cmbxGhbLayerAttribute.Enabled = true;
                    // numeric up-down
                    udGhbLayerPicker.Enabled = false;
                    break;
                case LayerMethod.ByCellTops:
                    // radio buttons
                    rbGhbSameLayer.Checked = false;
                    rbGhbLayerAttribute.Checked = false;
                    // combo boxes
                    cmbxGhbLayerAttribute.Enabled = false;
                    // numeric up-down
                    udGhbLayerPicker.Enabled = false;
                    break;
                case LayerMethod.UppermostActiveCell:
                    break;
            }
            #endregion Layer Assignment

            #region Leakance
            cmbxGhbLeakanceAttribute.Text = "";
            cmbxGhbLeakanceAttribute.Items.Clear();
            GeoValue geoValue = _featureSet.GetGeoValueByDescriptor("leakance");
            if (geoValue != null)
            {
                textBoxGhbLeakanceUniformValue.Text = geoValue.UniformValue.ToString();
                rbGhbLeakanceUniform.Checked = geoValue.GeoValueType == GeoValueType.Uniform;
                rbGhbLeakanceFromAttribute.Checked = geoValue.GeoValueType == GeoValueType.Attribute;
            }
            else
            {
                textBoxGhbLeakanceUniformValue.Text = "";
                rbGhbLeakanceUniform.Checked = true;
                rbGhbLeakanceFromAttribute.Checked = false;
            }
            textBoxGhbLeakanceUniformValue.Enabled = rbGhbLeakanceUniform.Checked;
            cmbxGhbLeakanceAttribute.Enabled = rbGhbLeakanceFromAttribute.Checked;
            #endregion Leakance
        }

        private void ConfigureForRch()
        {
            this.Text = "Recharge Package Feature Set";
            this.Icon = Properties.Resources.RchIcon;
            grpboxSmpInterpretationMethod.Enabled = true;
            cmbxLabelFeatures.Enabled = false;

            // radio buttons
            rbStepwise.Enabled = true;
            rbPiecewise.Enabled = true;
        }

        private void ConfigureForChd()
        {
            this.Text = "CHD Package Feature Set";
            this.Icon = Properties.Resources.ChdIcon;

            // General tab
            grpboxSmpInterpretationMethod.Enabled = false;
            cmbxLabelFeatures.Enabled = true;
            // radio buttons
            rbStepwise.Enabled = false;
            rbPiecewise.Enabled = false;
            
            // CHD tab
            // combo boxes
            cmbxChdLayerAttribute.Items.Clear();
            cmbxChdLayerAttribute.Text = "";
            // numeric up-down
            udChdLayerPicker.Value = _featureSet.DefaultLayer;
            if (Nlay > 0)
            {
                udChdLayerPicker.Maximum = Nlay;
            }
            switch (_featureSet.LayMethod)
            {
                case LayerMethod.Uniform:
                    // radio buttons
                    rbChdSameLayer.Checked = true;
                    rbChdLayerAttribute.Checked = false;
                    // combo boxes
                    cmbxChdLayerAttribute.Enabled = false;
                    // numeric up-down
                    udChdLayerPicker.Enabled = true;
                    break;
                case LayerMethod.ByAttribute:
                    // radio buttons
                    rbChdSameLayer.Checked = false;
                    rbChdLayerAttribute.Checked = true;
                    // combo boxes
                    cmbxChdLayerAttribute.Enabled = true;
                    // numeric up-down
                    udChdLayerPicker.Enabled = false;
                    break;
                case LayerMethod.ByCellTops:
                    // radio buttons
                    rbChdSameLayer.Checked = false;
                    rbChdLayerAttribute.Checked = false;
                    // combo boxes
                    cmbxChdLayerAttribute.Enabled = false;
                    // numeric up-down
                    udChdLayerPicker.Enabled = false;
                    break;
                case LayerMethod.UppermostActiveCell:
                    break;
            }
        }

        private void ConfigureForRiv()
        {
            this.Text = "River Package Feature Set";
            this.Icon = Properties.Resources.RivIcon;
            grpboxSmpInterpretationMethod.Enabled = true;
            textBoxRivSecondarySmp.Text = _featureSet.TimeSeriesSecondaryAbsolutePath;
            rbStepwise.Enabled = true;
            rbPiecewise.Enabled = true;
            cmbxLabelFeatures.Enabled = true;
            GeoValue geoValue;

            #region Layer assignment
            cmbxRivLayerAttribute.Items.Clear();
            cmbxRivLayerAttribute.Text = "";
            // numeric up-down
            udRivLayerPicker.Value = _featureSet.DefaultLayer;
            if (Nlay > 0)
            {
                udRivLayerPicker.Maximum = Nlay;
            }
            switch (_featureSet.LayMethod)
            {
                case LayerMethod.Uniform:
                    // radio buttons
                    rbRivSameLayer.Checked = true;
                    rbRivLayerAttribute.Checked = false;
                    // combo boxes
                    cmbxRivLayerAttribute.Enabled = false;
                    // numeric up-down
                    udRivLayerPicker.Enabled = true;
                    break;
                case LayerMethod.ByAttribute:
                    // radio buttons
                    rbRivSameLayer.Checked = false;
                    rbRivLayerAttribute.Checked = true;
                    // combo boxes
                    cmbxRivLayerAttribute.Enabled = true;
                    // numeric up-down
                    udRivLayerPicker.Enabled = false;
                    break;
                case LayerMethod.ByCellTops:
                    // radio buttons
                    rbRivSameLayer.Checked = false;
                    rbRivLayerAttribute.Checked = false;
                    // combo boxes
                    cmbxRivLayerAttribute.Enabled = false;
                    // numeric up-down
                    udRivLayerPicker.Enabled = false;
                    break;
                case LayerMethod.UppermostActiveCell:
                    break;
            }
            #endregion Layer assignment

            #region Hydraulic conductivity
            cmbxRivHydCondAttribute.Text = "";
            cmbxRivHydCondAttribute.Items.Clear();
            geoValue = _featureSet.GetGeoValueByDescriptor("hydraulic conductivity");
            if (geoValue != null)
            {
                textBoxRivHydCondUniformValue.Text = geoValue.UniformValue.ToString();
                rbRivHydCondUniform.Checked = geoValue.GeoValueType == GeoValueType.Uniform;
                rbRivHydCondFromAttribute.Checked = geoValue.GeoValueType == GeoValueType.Attribute;
            }
            else
            {
                textBoxRivHydCondUniformValue.Text = "";
                rbRivHydCondUniform.Checked = true;
                rbRivHydCondFromAttribute.Checked = false;
            }
            textBoxRivHydCondUniformValue.Enabled = rbRivHydCondUniform.Checked;
            cmbxRivHydCondAttribute.Enabled = rbRivHydCondFromAttribute.Checked;
            #endregion Hydraulic conductivity

            #region Width
            cmbxRivWidthAttribute.Text = "";
            cmbxRivWidthAttribute.Items.Clear();
            geoValue = _featureSet.GetGeoValueByDescriptor("width");
            if (geoValue != null)
            {
                textBoxRivWidthUniformValue.Text = geoValue.UniformValue.ToString();
                rbRivWidthUniform.Checked = geoValue.GeoValueType == GeoValueType.Uniform;
                rbRivWidthFromAttribute.Checked = geoValue.GeoValueType == GeoValueType.Attribute;
            }
            else
            {
                textBoxRivWidthUniformValue.Text = "";
                rbRivWidthUniform.Checked = true;
                rbRivWidthFromAttribute.Checked = false;
            }
            textBoxRivWidthUniformValue.Enabled = rbRivWidthUniform.Checked;
            cmbxRivWidthAttribute.Enabled = rbRivWidthFromAttribute.Checked;
            #endregion Width

            #region Thickness
            cmbxRivThicknessAttribute.Text = "";
            cmbxRivThicknessAttribute.Items.Clear();
            geoValue = _featureSet.GetGeoValueByDescriptor("riverbed thickness");
            if (geoValue != null)
            {
                textBoxRivThicknessUniformValue.Text = geoValue.UniformValue.ToString();
                rbRivThicknessUniform.Checked = geoValue.GeoValueType == GeoValueType.Uniform;
                rbRivThicknessFromAttribute.Checked = geoValue.GeoValueType == GeoValueType.Attribute;
            }
            else
            {
                textBoxRivThicknessUniformValue.Text = "";
                rbRivThicknessUniform.Checked = true;
                rbRivThicknessFromAttribute.Checked = false;
            }
            textBoxRivThicknessUniformValue.Enabled = rbRivThicknessUniform.Checked;
            cmbxRivThicknessAttribute.Enabled = rbRivThicknessFromAttribute.Checked;
            #endregion Thickness

            #region Riverbed bottom elevation
            cmbxRivBottomElevAttribute.Text = "";
            cmbxRivBottomElevAttribute.Items.Clear();
            #endregion Riverbed bottom elevation
        }

        private void ConfigureForWel()
        {
            this.Text = "Well Package Feature Set";
            this.Icon = Properties.Resources.WellPump;
            grpboxSmpInterpretationMethod.Enabled = true;
            cmbxLabelFeatures.Enabled = true;
            // radio buttons
            rbStepwise.Enabled = true;
            rbPiecewise.Enabled = true;
            // combo boxes
            cmbxWellsLayerAttribute.Items.Clear();
            cmbxWellsLayerAttribute.Text = "";
            cmbxWellsTopElevationAttribute.Items.Clear();
            cmbxWellsTopElevationAttribute.Text = "";
            cmbxWellsBottomElevationAttribute.Items.Clear();
            cmbxWellsBottomElevationAttribute.Text = "";
            // numeric up-down
            udWellsLayerPicker.Value = _featureSet.DefaultLayer;
            if (Nlay > 0)
            {
                udWellsLayerPicker.Maximum = Nlay;
            }
            switch (_featureSet.LayMethod)
            {
                case LayerMethod.Uniform:
                    // radio buttons
                    rbWellsSameLayer.Checked = true;
                    rbWellsLayerAttribute.Checked = false;
                    rbWellsByTops.Checked = false;
                    // combo boxes
                    cmbxWellsLayerAttribute.Enabled = false;
                    cmbxWellsTopElevationAttribute.Enabled = false;
                    cmbxWellsBottomElevationAttribute.Enabled = false;
                    // numeric up-down
                    udWellsLayerPicker.Enabled = true;
                    break;
                case LayerMethod.ByAttribute:
                    // radio buttons
                    rbWellsSameLayer.Checked = false;
                    rbWellsLayerAttribute.Checked = true;
                    rbWellsByTops.Checked = false;
                    // combo boxes
                    cmbxWellsLayerAttribute.Enabled = true;
                    cmbxWellsTopElevationAttribute.Enabled = false;
                    cmbxWellsBottomElevationAttribute.Enabled = false;
                    // numeric up-down
                    udWellsLayerPicker.Enabled = false;
                    break;
                case LayerMethod.ByCellTops:
                    // radio buttons
                    rbWellsSameLayer.Checked = false;
                    rbWellsLayerAttribute.Checked = false;
                    rbWellsByTops.Checked = true;
                    // combo boxes
                    cmbxWellsLayerAttribute.Enabled = false;
                    cmbxWellsTopElevationAttribute.Enabled = true;
                    cmbxWellsBottomElevationAttribute.Enabled = true;
                    // numeric up-down
                    udWellsLayerPicker.Enabled = false;
                    break;
                case LayerMethod.UppermostActiveCell:
                    break;
            }
        }

        private void cbxKeyField_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                if (_featureSet.KeyField != cmbxKeyField.Text)
                {
                    _featureSet.KeyField = cmbxKeyField.Text;
                    Changed = true;
                }
            }
        }

        private void cmbxWellsFeatureLayerAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                if (_featureSet.LayerAttribute != cmbxWellsLayerAttribute.Text)
                {
                    _featureSet.LayerAttribute = cmbxWellsLayerAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void udWellsLayerPicker_ValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                int newValue = Convert.ToInt32(udWellsLayerPicker.Value);
                if (_featureSet.DefaultLayer != newValue)
                {
                    _featureSet.DefaultLayer = newValue;
                    Changed = true;
                }
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            int newWidth;
            newWidth = _stMapLegend.Width;
            foreach (MapLegendItem item in _stMapLegend.Items)
            {
                if (item.Parent.Width < newWidth)
                {
                    item.Parent.Width = newWidth;
                }
                item.Width = newWidth;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _okToClose = true;
            bool isLayerPackage = false;
            if (_featureSet.Parent is RchPackage) // Ned TODO: or parent is EvtPackage, eventually
            {
                isLayerPackage = true;
                if (_groupBoxRchLayerAssignment.Changed)
                {
                    Changed = true;
                }
            }
            if (_shapefileChanged)
            {
                _featureSet.NeedRowAndCol = true;
            }

            // Assign time-series interpretation method
            if (rbPiecewise.Checked)
            {
                _featureSet.InterpretationMethod = TimeSeriesInterpretationMethod.Piecewise;
            }
            else if (rbStepwise.Checked)
            {
                _featureSet.InterpretationMethod = TimeSeriesInterpretationMethod.Stepwise;
            }
            
            // If Layer Assignment is by Attribute or by Cell tops and bottoms,
            // and corresponding attribute(s) are not specified, it's an error.
            switch (_featureSet.Type)
            {
                case PackageType.WellType:
                    #region
                    if (rbWellsLayerAttribute.Checked)
                    {
                        if (cmbxWellsLayerAttribute.SelectedIndex < 0)
                        {
                            string errMessage = "Error: Layer Attribute not specified";
                            _okToClose = false;
                            MessageBox.Show(errMessage);
                        }
                    }
                    if (rbWellsByTops.Checked)
                    {
                        if (cmbxWellsTopElevationAttribute.SelectedIndex < 0 || cmbxWellsBottomElevationAttribute.SelectedIndex < 0)
                        {
                            string errMessage = "Error: Cell Top or Bottom Attribute not specified";
                            _okToClose = false;
                            MessageBox.Show(errMessage);
                        }
                    }
                    break;
                    #endregion
                case PackageType.ChdType:
                    #region
                    if (rbChdLayerAttribute.Checked)
                    {
                        if (cmbxChdLayerAttribute.SelectedIndex < 0)
                        {
                            string errMessage = "Error: Layer Attribute not specified";
                            _okToClose = false;
                            MessageBox.Show(errMessage);
                        }
                    }
                    break;
                    #endregion
                case PackageType.GhbType:
                    #region
                    if (rbGhbLayerAttribute.Checked)
                    {
                        if (cmbxGhbLayerAttribute.SelectedIndex < 0)
                        {
                            string errMessage = "Error: Layer Attribute not specified";
                            _okToClose = false;
                            MessageBox.Show(errMessage);
                        }
                    }
                    break;
                    #endregion
                case PackageType.RiverType:
                    #region
                    if (rbRivLayerAttribute.Checked)
                    {
                        if (cmbxRivLayerAttribute.SelectedIndex < 0)
                        {
                            string errMessage = "Error: Layer Attribute not specified";
                            _okToClose = false;
                            MessageBox.Show(errMessage);
                        }
                    }
                    break;
                    #endregion
            }

            // Assign package option and package-layer attribute, if appropriate
            if (isLayerPackage)
            {
                _featureSet.PackageOption = _groupBoxRchLayerAssignment.Npkgop;
                _featureSet.LayerAttribute = _groupBoxRchLayerAssignment.LayerAttribute;
            }

            if (_okToClose)
            {
                // If either shapefile or SMP file is non-blank, check all entries
                if (tbShapefile.Text != "" || tbSmp.Text != "")
                {
                    string errMessage = "Error(s): ";
                    if (!File.Exists(tbShapefile.Text))
                    {
                        errMessage = errMessage + "Shapefile does not exist. ";
                        _okToClose = false;
                    }
                    if (!File.Exists(tbSmp.Text))
                    {
                        errMessage = errMessage + "SMP file does not exist. ";
                        _okToClose = false;
                    }
                    if (cmbxKeyField.Text == "")
                    {
                        errMessage = errMessage + "Linking attribute not defined.";
                        _okToClose = false;
                    }

                    // Set GeoValueType if applicable, and check validity of values specified as uniform
                    GeoValue geoValue;
                    string valueText;
                    switch (_featureSet.Type)
                    {
                        case PackageType.GhbType:
                            #region
                            geoValue = _featureSet.GetGeoValueByDescriptor("leakance");
                            if (rbGhbLeakanceUniform.Checked)
                            {
                                try
                                {
                                    double val = Convert.ToDouble(textBoxGhbLeakanceUniformValue.Text);
                                    if (geoValue != null)
                                    {
                                        geoValue.UniformValue = val;
                                        geoValue.GeoValueType = GeoValueType.Uniform;
                                    }
                                }
                                catch
                                {
                                    errMessage = "Cannot convert \"" + textBoxGhbLeakanceUniformValue.Text + "\" to a number.";
                                    _okToClose = false;
                                }
                            }
                            else
                            {
                                if (geoValue != null)
                                {
                                    geoValue.GeoValueType = GeoValueType.Attribute;
                                }
                            }
                            break;
                            #endregion
                        case PackageType.RiverType:
                            #region
                            // Hydraulic conductivity
                            geoValue = _featureSet.GetGeoValueByDescriptor("hydraulic conductivity");
                            if (rbRivHydCondUniform.Checked)
                            {
                                valueText = textBoxRivHydCondUniformValue.Text;
                                try
                                {
                                    double val = Convert.ToDouble(valueText);
                                    if (geoValue != null)
                                    {
                                        geoValue.UniformValue = val;
                                        geoValue.GeoValueType = GeoValueType.Uniform;
                                    }
                                }
                                catch
                                {
                                    errMessage = "Cannot convert \"" + valueText + "\" to a number.";
                                    _okToClose = false;
                                }
                            }
                            else
                            {
                                if (geoValue != null)
                                {
                                    geoValue.GeoValueType = GeoValueType.Attribute;
                                }
                            }

                            // Width
                            geoValue = _featureSet.GetGeoValueByDescriptor("width");
                            if (rbRivWidthUniform.Checked)
                            {
                                valueText = textBoxRivWidthUniformValue.Text;
                                try
                                {
                                    double val = Convert.ToDouble(valueText);
                                    if (geoValue != null)
                                    {
                                        geoValue.UniformValue = val;
                                        geoValue.GeoValueType = GeoValueType.Uniform;
                                    }
                                }
                                catch
                                {
                                    errMessage = "Cannot convert \"" + valueText + "\" to a number.";
                                    _okToClose = false;
                                }
                            }
                            else
                            {
                                if (geoValue != null)
                                {
                                    geoValue.GeoValueType = GeoValueType.Attribute;
                                }
                            }

                            // Thickness
                            geoValue = _featureSet.GetGeoValueByDescriptor("riverbed thickness");
                            if (rbRivThicknessUniform.Checked)
                            {
                                valueText = textBoxRivThicknessUniformValue.Text;
                                try
                                {
                                    double val = Convert.ToDouble(valueText);
                                    if (geoValue != null)
                                    {
                                        geoValue.UniformValue = val;
                                        geoValue.GeoValueType = GeoValueType.Uniform;
                                    }
                                }
                                catch
                                {
                                    errMessage = "Cannot convert \"" + valueText + "\" to a number.";
                                    _okToClose = false;
                                }
                            }
                            else
                            {
                                if (geoValue != null)
                                {
                                    geoValue.GeoValueType = GeoValueType.Attribute;
                                }
                            }

                            // Riverbed bottom elevation
                            geoValue = _featureSet.GetGeoValueByDescriptor("riverbed bottom elevation");
                            if (geoValue != null)
                            {
                                geoValue.GeoValueType = GeoValueType.Attribute;
                            }
                            break;
                            #endregion
                    }

                    if (!_okToClose)
                    {
                        MessageBox.Show(errMessage);
                    }
                }
            }
        }

        #endregion Form Event Handlers

        #region mapControl Event Handlers
        /// <summary>
        /// Handles the MouseMove event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            updateStatusBarLocationInfo(e.X, e.Y);

            switch (_activeTool)
            {
                case ActiveTool.Pointer:
                    //FindContourLineHit(e.X, e.Y);
                    //if (_HotFeature == null)
                    //{ mapControl.Cursor = System.Windows.Forms.Cursors.Default; }
                    //else
                    //{ mapControl.Cursor = System.Windows.Forms.Cursors.Hand; }
                    break;
                case ActiveTool.ReCenter:
                    break;
                case ActiveTool.ZoomIn:
                    break;
                case ActiveTool.ZoomOut:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Handles the MouseClick event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (_mapControl.LayerCount > 0)
            {
                ICoordinate pt = _mapControl.ToMapPoint(e.X, e.Y);
                switch (_activeTool)
                {
                    case ActiveTool.Pointer:
                        //ShowMapTip(e.X, e.Y);
                        break;
                    case ActiveTool.ReCenter:
                        _mapControl.Center = pt;
                        break;
                    case ActiveTool.ZoomIn:
                        _mapControl.Zoom(2.0, pt.X, pt.Y);
                        break;
                    case ActiveTool.ZoomOut:
                        _mapControl.Zoom(0.5, pt.X, pt.Y);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Handles the MouseDoubleClick event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (_activeTool)
            {
                case ActiveTool.Pointer:
                    break;
                case ActiveTool.ReCenter:
                    break;
                case ActiveTool.ZoomIn:
                    break;
                case ActiveTool.ZoomOut:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region mapLegend Event Handlers
        /// <summary>
        /// Handles the LayerVisibilityChanged event of the mapLegend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapLegend_LayerVisibilityChanged(object sender, EventArgs e)
        {
            _mapControl.Refresh();
        }

        #endregion

        #region Methods

        private void FindFeatureSetFile(PackageType packageType, FileType fileType)
        {
            string fileName = "";
            string folder = "";
            string path = "";
            openFileDialog1.RestoreDirectory = true;
            switch (fileType)
            {
                case FileType.Shapefile:
                    path = _featureSet.ShapefileAbsolutePath;
                    openFileDialog1.Filter = "Shapefiles (*.shp)|*.shp";
                    openFileDialog1.FileName = tbShapefile.Text;
                    ImportShapefile();
                    path = openFileDialog1.FileName;
                    break;
                case FileType.Smpfile:
                    path = _featureSet.TimeSeriesAbsolutePath;
                    openFileDialog1.Filter = "Time series file (*.smp)|*.smp|All Files|*.*";
                    openFileDialog1.FileName = tbSmp.Text;
                    break;
                case FileType.SecondarySmpfile:
                    path = _featureSet.TimeSeriesSecondaryAbsolutePath;
                    openFileDialog1.Filter = "Time series file (*.smp)|*.smp|All Files|*.*";
                    openFileDialog1.FileName = textBoxGhbSecondarySmp.Text;
                    break;
            }

            if (fileType == FileType.Smpfile || fileType == FileType.SecondarySmpfile)
            {
                if (path == "")
                {
                    openFileDialog1.FileName = "";
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        path = openFileDialog1.FileName;
                    }
                }
                else
                {
                    fileName = Path.GetFileName(path);
                    if (Path.IsPathRooted(path))
                    {
                        folder = Path.GetDirectoryName(path);
                        openFileDialog1.InitialDirectory = folder;
                        openFileDialog1.FileName = fileName;
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            path = openFileDialog1.FileName;
                        }
                    }
                    else
                    {
                        folder = Path.GetDirectoryName(path);
                        if (Directory.Exists(folder))
                        {
                            openFileDialog1.InitialDirectory = folder;
                        }
                        openFileDialog1.FileName = Path.GetFileName(path);
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            path = openFileDialog1.FileName;
                        }

                    }
                }
            }
            switch (fileType)
            {
                case FileType.Shapefile:
                    tbShapefile.Text = path;
                    break;
                case FileType.Smpfile:
                    tbSmp.Text = path;
                    break;
                case FileType.SecondarySmpfile:
                    switch (packageType)
                    {
                        case PackageType.GhbType:
                            textBoxGhbSecondarySmp.Text = path;
                            break;
                        case PackageType.RiverType:
                            textBoxRivSecondarySmp.Text = path;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Reads an ESRI shapefile and imports the features as a FeatureCollection
        /// The shapefile name is the full pathname, including the .shp extension.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private void ImportShapefile()
        {
            //FeatureCollection featureList = null;

            openFileDialog1.Filter = "ESRI Shapefiles (*.shp)|*.shp";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Read ESRI shapefile and import features to a FeatureCollection
                    string path = openFileDialog1.FileName;
                    DateTime _lastWriteTime = File.GetLastWriteTime(path);
                    if (_previousFeatureSetShapefilePath != path || !DateTime.Equals(_lastWriteTime, _previousFeatureSetShapefileDateTime))
                    {
                        if (_featureList != null)
                        {
                            _featureList.Clear();
                            _featureList = null;
                        }
                        _featureList = ReadAndDisplayShapefile(path); // USGS.Puma.IO.EsriShapefileIO.Import(path);
                        _previousFeatureSetShapefilePath = path;
                        _previousFeatureSetShapefileDateTime = _lastWriteTime;
                    }
                    //_ImportedShapefileMapLayer = CreateMapLayerFromFeatureCollection(_featureList, System.Drawing.Color.Blue, 1.0f, 1);
                    //string layerName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                    //_ImportedShapefileMapLayer.LayerName = layerName;
                    //BuildMapLayers(true);
                }
                catch
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private FeatureCollection ReadAndDisplayShapefile(string path)
        {
            // Read ESRI shapefile and import features to a FeatureCollection
            FeatureCollection featureList = USGS.Puma.IO.EsriShapefileIO.Import(path);
            float size = 1.0f;
            if (featureList.Count>0)
            {
                IGeometry geom = featureList[0].Geometry;
                if (geom is USGS.Puma.NTS.Geometries.Point)
                {
                    size = 2.0f;
                }
                else if (geom is USGS.Puma.NTS.Geometries.MultiLineString)
                {
                    size = 2.0f;
                }
            }
            
            _ImportedShapefileMapLayer = CreateMapLayerFromFeatureCollection(featureList, System.Drawing.Color.Blue, size, 1);
            string layerName = System.IO.Path.GetFileNameWithoutExtension(path);
            _ImportedShapefileMapLayer.LayerName = layerName;
            AssignSpatialReference(path);
            BuildMapLayers(true);
            return featureList;
        }
        private void AssignSpatialReference(string path)
        {
            // Define FeatureSet spatial reference
            string prjFile = Path.ChangeExtension(path, "prj");
            if (File.Exists(prjFile))
            {
                using (StreamReader sr = File.OpenText(prjFile))
                {
                    string wktString = sr.ReadLine();
                    _featureSet.SpatialReference = new ScenarioTools.Spatial.SpatialReference(wktString, _featureSet);
                    GlobalSpatialStaticVariables.SpatialReference = _featureSet.SpatialReference;
                }
            }
            else
            {
                _featureSet.SpatialReference = new ScenarioTools.Spatial.SpatialReference("", _featureSet);
                GlobalSpatialStaticVariables.SpatialReference = _featureSet.SpatialReference;
            }
        }

        /// <summary>
        /// Creates the map layer from a collection of features.
        /// Sets up the layer to use a single-symbol renderer with the specified color, size, and symbol style.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="color">The color.</param>
        /// <param name="size">The size.</param>
        /// <param name="symbolStyle">The symbol style.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private FeatureLayer CreateMapLayerFromFeatureCollection(FeatureCollection featureList, System.Drawing.Color color, float size, int symbolStyle)
        {

            FeatureLayer layer = null;

            if (featureList != null)
            {
                if (featureList.Count > 0)
                {
                    USGS.Puma.NTS.Features.Feature f = featureList[0];
                    if (f.Geometry is IMultiLineString || f.Geometry is ILineString)
                    {
                        layer = new FeatureLayer(LayerGeometryType.Line);
                        ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
                        symbol.Color = color;
                        symbol.Width = size;
                    }
                    else if (f.Geometry is IPolygon)
                    {
                        layer = new FeatureLayer(LayerGeometryType.Polygon);
                        ISolidFillSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ISolidFillSymbol;
                        symbol.Color = color;
                    }
                    else if (f.Geometry is IPoint)
                    {
                        layer = new FeatureLayer(LayerGeometryType.Point);
                        SimplePointSymbol symbol = (((layer.Renderer as SingleSymbolRenderer).Symbol) as SimplePointSymbol);
                        symbol.Color = color;
                        symbol.Size = size;
                        if (symbolStyle == 1)
                        {
                            symbol.SymbolType = PointSymbolTypes.Circle;
                        }
                        else if (symbolStyle == 2)
                        {
                            symbol.SymbolType = PointSymbolTypes.Square;
                        }
                        else
                        {
                            symbol.SymbolType = PointSymbolTypes.Circle;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Cannot create layer for the specified feature type.");
                    }

                    for (int i = 0; i < featureList.Count; i++)
                    {
                        layer.AddFeature(featureList[i]);
                    }

                    layer.Visible = true;
                }
            }
            return layer;
        }

        /// <summary>
        /// Builds the map.
        /// Previous content is cleared. The current copies of the grid and contour map layers are added to the map,
        /// and the map is refreshed.
        /// </summary>
        /// <param name="fullExtent"></param>
        private void BuildMapLayers(bool fullExtent)
        {
            bool forceFullExtent = fullExtent;
            if (_mapControl.LayerCount == 0)
                forceFullExtent = true;

            _mapControl.ClearLayers();

            // Background image
            if (cbxShowBackground.Checked)
            {
                if (ImageLayerBackground.GetGeoImage() != null)
                {
                    _mapControl.AddLayer(ImageLayerBackground);
                }
            }

            // Interior grid lines
            if (_GridlinesMapLayer != null)
            {_mapControl.AddLayer(_GridlinesMapLayer); }

            // Grid outline
            if (_GridOutlineMapLayer != null)
            {_mapControl.AddLayer(_GridOutlineMapLayer); }

            // Head contours
            if (_CurrentContourMapLayer != null)
            {
                _mapControl.AddLayer(_CurrentContourMapLayer);
            }

            // Imported shapefile
            if (_ImportedShapefileMapLayer != null)
            {
                _mapControl.AddLayer(_ImportedShapefileMapLayer);
            }

            // Prepare and display the map
            if (_mapControl.LayerCount > 0)
            {
                if (forceFullExtent)
                {
                    _mapControl.SizeToFullExtent();
                }
            }
            BuildMapLegend();
            _mapControl.Refresh();
            _indexMap.UpdateMapImage();
        }

        /// <summary>
        /// Clears the map legend.
        /// </summary>
        /// <remarks></remarks>
        private void ClearMapLegend()
        {
            _stMapLegend.Clear();
            _stMapLegend.LegendTitle = "";
        }
        /// <summary>
        /// Builds the map legend
        /// </summary>
        private void BuildMapLegend()
        {
            ClearMapLegend();
            Collection<GraphicLayer> legendItems = new Collection<GraphicLayer>();

            // Add model grid outline
            if (_GridOutlineMapLayer != null)
            {
                legendItems.Add(_GridOutlineMapLayer);
            }

            // Add model grid interior lines
            if (_GridlinesMapLayer != null)
            {
                legendItems.Add(_GridlinesMapLayer);
            }

            //// Add head contour layer
            //if (_CurrentContourMapLayer != null)
            //{
            //    legendItems.Add(_CurrentContourMapLayer);
            //}

            // Add imported shapefile layer
            if (_ImportedShapefileMapLayer != null)
            {
                legendItems.Add(_ImportedShapefileMapLayer);
            }

            //// Add background image
            //if (ImageLayerBackground != null)
            //{
            //    legendItems.Add(ImageLayerBackground);
            //}

            _stMapLegend.LegendTitle = "Map layers";
            _stMapLegend.AddAndLayoutItems(legendItems);
            foreach (MapLegendItem item in _stMapLegend.Items)
            {
                item.Parent.Width = _stMapLegend.Width;
            }
        }

        /// <summary>
        /// Builds the ModelGridlines layer
        /// </summary>
        /// <param name="modelGrid"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private FeatureLayer CreateModelGridlinesLayer(CellCenteredArealGrid modelGrid, Color color)
        {
            IMultiLineString outline = modelGrid.GetOutline();
            IMultiLineString[] gridlines = modelGrid.GetGridLines();

            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;

            IAttributesTable attributes = null;
            int value = 1;
            for (int i = 0; i < gridlines.Length; i++)
            {
                attributes = new AttributesTable();
                attributes.AddAttribute("Value", value);
                layer.AddFeature(gridlines[i] as IGeometry, attributes);
            }

            attributes = new AttributesTable();
            value = 0;
            attributes.AddAttribute("Value", value);
            layer.AddFeature(outline as IGeometry, attributes);
            layer.Visible = false;

            layer.LayerName = "Model gridlines";
            return layer;
        }

        /// <summary>
        /// Builds ModelGridOutline layer
        /// </summary>
        /// <param name="modelGrid"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private FeatureLayer CreateModelGridOutlineLayer(CellCenteredArealGrid modelGrid, Color color)
        {
            IMultiLineString outline = modelGrid.GetOutline();
            if (outline == null)
                throw new Exception("The model grid outline was not created.");

            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;

            IAttributesTable attributes = new AttributesTable();
            int value = 0;
            attributes.AddAttribute("Value", value);
            layer.AddFeature(outline as IGeometry, attributes);

            layer.LayerName = "Model grid outline";
            return layer;

        }

        private void PopulateComboBoxList(ComboBox CmbBox, string defaultAttribute, VarTypes vType)
        {
            _populatingControls = true;
            CmbBox.Text = "";
            CmbBox.Items.Clear();
            if (_featureList == null)
            {
                if (!string.IsNullOrEmpty(defaultAttribute))
                {
                    CmbBox.Items.Add(defaultAttribute);
                }
            }
            else
            {
                int defaultIndex = -1;
                //int k = -1;
                bool OK;
                Type type;
                int numAttributes = ((USGS.Puma.NTS.Features.Feature)_featureList.ElementAt(0)).Attributes.Count;
                string[] names = new string[numAttributes];
                AttributesTable attTable = (AttributesTable)((USGS.Puma.NTS.Features.Feature)_featureList.ElementAt(0)).Attributes;
                names = attTable.GetNames();
                for (int i = 0; i < numAttributes; i++)
                {
                    OK = false;
                    type = attTable.GetType(names[i]);
                    switch (vType)
                    {
                        case VarTypes.Any:
                            OK = true;
                            break;
                        case VarTypes.IntegerOrString:
                            if (type == typeof(string))
                            {
                                OK = true;
                            }
                            else
                            {
                                goto case VarTypes.Integer;
                            }
                            break;
                        case VarTypes.NumericOrString:
                            if (type == typeof(string))
                            {
                                OK = true;
                            }
                            else
                            {
                                goto case VarTypes.Numeric;
                            }
                            break;
                        case VarTypes.Numeric:
                            if ((type == typeof(double)) || (type ==  typeof(float)))
                            {
                                OK = true;
                            }
                            else
                            {
                                goto case VarTypes.Integer;
                            }
                            break;
                        case VarTypes.Integer:
                            if ((type == typeof(int) || (type == typeof(Int16)) || 
                                 type == typeof(Int32) || (type ==  typeof(Int64)))) 
                            { 
                                OK = true; 
                            }
                            break;
                        case VarTypes.String:
                            if ((type == typeof(string)))
                            { 
                                OK = true; 
                            }
                            break;
                    }
                    if (OK)
                    {
                        CmbBox.Items.Add(names[i]);
                    }
                }
                if (defaultAttribute != "")
                {
                    for (int i = 0; i < CmbBox.Items.Count; i++)
                    {
                        if (CmbBox.Items[i].ToString() == defaultAttribute)
                        {
                            defaultIndex = i;
                        }
                    }
                }
                if (defaultIndex != -1)
                {
                    CmbBox.SelectedIndex = defaultIndex;
                }
            }
            _populatingControls = false;
        }

        private void ProcessFeatureSetShapefile()
        {
            string path = tbShapefile.Text;
            if (File.Exists(path))
            {
                DateTime _lastWriteTime = File.GetLastWriteTime(path);
                if (_previousFeatureSetShapefilePath != path || !DateTime.Equals(_lastWriteTime, _previousFeatureSetShapefileDateTime))
                {
                    if (_featureList != null)
                    {
                        _featureList.Clear();
                        _featureList = null;
                    }
                    _featureList = ReadAndDisplayShapefile(path);
                    _previousFeatureSetShapefilePath = path;
                    _previousFeatureSetShapefileDateTime = _lastWriteTime;
                }
                else
                {
                    BuildMapLayers(true);
                }
                PopulateComboBoxList(cmbxKeyField, _featureSet.KeyField, VarTypes.String);
                GeoValue geoValue;
                switch (_featureSet.Type)
                {
                    case PackageType.WellType:
                        PopulateComboBoxList(cmbxWellsLayerAttribute, _featureSet.LayerAttribute, VarTypes.Numeric);
                        PopulateComboBoxList(cmbxWellsTopElevationAttribute, _featureSet.TopElevationAttribute, VarTypes.Numeric);
                        PopulateComboBoxList(cmbxWellsBottomElevationAttribute, _featureSet.BottomElevationAttribute, VarTypes.Numeric);
                        break;
                    case PackageType.RiverType:
                        PopulateComboBoxList(cmbxRivLayerAttribute, _featureSet.LayerAttribute, VarTypes.Numeric);
                        geoValue = _featureSet.GetGeoValueByDescriptor("hydraulic conductivity");
                        PopulateComboBoxList(cmbxRivHydCondAttribute, geoValue.Attribute, VarTypes.Numeric);
                        geoValue = _featureSet.GetGeoValueByDescriptor("riverbed bottom elevation");
                        PopulateComboBoxList(cmbxRivBottomElevAttribute, geoValue.Attribute, VarTypes.Numeric);
                        geoValue = _featureSet.GetGeoValueByDescriptor("width");
                        PopulateComboBoxList(cmbxRivWidthAttribute, geoValue.Attribute, VarTypes.Numeric);
                        geoValue = _featureSet.GetGeoValueByDescriptor("riverbed thickness");
                        PopulateComboBoxList(cmbxRivThicknessAttribute, geoValue.Attribute, VarTypes.Numeric);
                        break;
                    case PackageType.ChdType:
                        PopulateComboBoxList(cmbxChdLayerAttribute, _featureSet.LayerAttribute, VarTypes.Numeric);
                        break;
                    case PackageType.RchType:
                        PopulateComboBoxList(_groupBoxRchLayerAssignment.CmbxLayerAttribute, _featureSet.LayerAttribute, VarTypes.Numeric);
                        break;
                    case PackageType.GhbType:
                        PopulateComboBoxList(cmbxGhbLayerAttribute, _featureSet.LayerAttribute, VarTypes.Numeric);
                        geoValue = _featureSet.GetGeoValueByDescriptor("leakance");
                        PopulateComboBoxList(cmbxGhbLeakanceAttribute, geoValue.Attribute, VarTypes.Numeric);
                        break;
                }
            }
        }

        #endregion Methods

        private void FeatureSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_okToClose)
            {
                e.Cancel = true;
                _okToClose = true;
            }
        }

        private void SetupWellLayerAssignment()
        {
            switch (_featureSet.LayMethod)
            {
                case LayerMethod.Uniform:
                {
                    udWellsLayerPicker.Enabled = true;
                    cmbxWellsLayerAttribute.Enabled = false;
                    cmbxWellsTopElevationAttribute.Enabled = false;
                    cmbxWellsBottomElevationAttribute.Enabled = false;
                    break;
                }
                case LayerMethod.ByAttribute:
                {
                    udWellsLayerPicker.Enabled = false;
                    cmbxWellsLayerAttribute.Enabled = true;
                    cmbxWellsTopElevationAttribute.Enabled = false;
                    cmbxWellsBottomElevationAttribute.Enabled = false;
                    break;
                }
                case LayerMethod.ByCellTops:
                {
                    udWellsLayerPicker.Enabled = false;
                    cmbxWellsLayerAttribute.Enabled = false;
                    cmbxWellsTopElevationAttribute.Enabled = true;
                    cmbxWellsBottomElevationAttribute.Enabled = true;
                    break;
                }
                case LayerMethod.UppermostActiveCell:
                // Applies only to RCH, EVT, and ETS packages
                {
                    // I think GroupBoxLayerAssignment takes care of setup
                    // through public method SetupRchLayerAssignment.
                    break;
                }
            }
        }
        private void SetupChdLayerAssignment()
        {
            switch (_featureSet.LayMethod)
            {
                case LayerMethod.Uniform:
                    {
                        udChdLayerPicker.Enabled = true;
                        cmbxChdLayerAttribute.Enabled = false;
                        break;
                    }
                case LayerMethod.ByAttribute:
                    {
                        udChdLayerPicker.Enabled = false;
                        cmbxChdLayerAttribute.Enabled = true;
                        break;
                    }
                case LayerMethod.ByCellTops:
                    {
                        udChdLayerPicker.Enabled = false;
                        cmbxChdLayerAttribute.Enabled = false;
                        break;
                    }
                case LayerMethod.UppermostActiveCell:
                    // Applies only to RCH, EVT, and ETS packages
                    {
                        // I think GroupBoxLayerAssignment takes care of setup
                        // through public method SetupRchLayerAssignment.
                        break;
                    }
            }
        }

        private void SetupGhbLayerAssignment()
        {
            switch (_featureSet.LayMethod)
            {
                case LayerMethod.Uniform:
                    {
                        udGhbLayerPicker.Enabled = true;
                        cmbxGhbLayerAttribute.Enabled = false;
                        break;
                    }
                case LayerMethod.ByAttribute:
                    {
                        udGhbLayerPicker.Enabled = false;
                        cmbxGhbLayerAttribute.Enabled = true;
                        break;
                    }
                case LayerMethod.ByCellTops:
                    {
                        udGhbLayerPicker.Enabled = false;
                        cmbxGhbLayerAttribute.Enabled = false;
                        break;
                    }
                case LayerMethod.UppermostActiveCell:
                    // Applies only to RCH, EVT, and ETS packages
                    {
                        // I think GroupBoxLayerAssignment takes care of setup
                        // through public method SetupRchLayerAssignment.
                        break;
                    }
            }
        }
        private void SetupRivLayerAssignment()
        {
            switch (_featureSet.LayMethod)
            {
                case LayerMethod.Uniform:
                    {
                        udRivLayerPicker.Enabled = true;
                        cmbxRivLayerAttribute.Enabled = false;
                        break;
                    }
                case LayerMethod.ByAttribute:
                    {
                        udRivLayerPicker.Enabled = false;
                        cmbxRivLayerAttribute.Enabled = true;
                        break;
                    }
                case LayerMethod.ByCellTops:
                    {
                        udRivLayerPicker.Enabled = false;
                        cmbxRivLayerAttribute.Enabled = false;
                        break;
                    }
                case LayerMethod.UppermostActiveCell:
                    // Applies only to RCH, EVT, and ETS packages
                    {
                        break;
                    }
            }
        }
        private void SetupRchLayerAssignment()
        {
            int nrchop = _featureSet.PackageOption;
            _groupBoxRchLayerAssignment.SetupPkgLayerAssignment(nrchop);
        }

        private void cbxWellsTopElevationAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                if (_featureSet.TopElevationAttribute != cmbxWellsTopElevationAttribute.Text)
                {
                    _featureSet.TopElevationAttribute = cmbxWellsTopElevationAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void cbxWellsBottomElevationAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                if (_featureSet.BottomElevationAttribute != cmbxWellsBottomElevationAttribute.Text)
                {
                    _featureSet.BottomElevationAttribute = cmbxWellsBottomElevationAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void cbxShowBackground_Click(object sender, EventArgs e)
        {
            BuildMapLayers(false);
        }

        private void cbxLabelFeatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            _featureSet.LabelFeatures = (FeatureLabel)cmbxLabelFeatures.SelectedIndex;
            Changed = true;
        }

        private void rbPiecewise_CheckedChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void rbStepwise_CheckedChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void textBoxGhbSecondarySmp_TextChanged(object sender, EventArgs e)
        {
            _featureSet.TimeSeriesSecondaryAbsolutePath = textBoxGhbSecondarySmp.Text;
            Changed = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void textBoxUniformValue_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void btnGhbSecondarySmp_Click(object sender, EventArgs e)
        {
            FindFeatureSetFile(PackageType.GhbType, FileType.SecondarySmpfile);
        }

        private void rbWellsSameLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWellsSameLayer.Checked)
            {
                _featureSet.LayMethod = LayerMethod.Uniform;
            }
            SetupWellLayerAssignment();
            Changed = true;
        }

        private void rbWellsLayerAttribute_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWellsLayerAttribute.Checked)
            {
                _featureSet.LayMethod = LayerMethod.ByAttribute;
            }
            SetupWellLayerAssignment();
            Changed = true;
        }

        private void rbWellsByTops_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWellsByTops.Checked)
            {
                _featureSet.LayMethod = LayerMethod.ByCellTops;
            }
            SetupWellLayerAssignment();
            Changed = true;
        }

        private void rbChdSameLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (rbChdSameLayer.Checked)
            {
                _featureSet.LayMethod = LayerMethod.Uniform;
            }
            SetupChdLayerAssignment();
            Changed = true;
        }

        private void rbChdLayerAttribute_CheckedChanged(object sender, EventArgs e)
        {
            if (rbChdLayerAttribute.Checked)
            {
                _featureSet.LayMethod = LayerMethod.ByAttribute;
            }
            SetupChdLayerAssignment();
            Changed = true;
        }

        private void udChdLayerPicker_ValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                int newValue = Convert.ToInt32(udChdLayerPicker.Value);
                if (_featureSet.DefaultLayer != newValue)
                {
                    _featureSet.DefaultLayer = newValue;
                    Changed = true;
                }
            }
        }

        private void cmbxChdLayerAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                if (_featureSet.LayerAttribute != cmbxChdLayerAttribute.Text)
                {
                    _featureSet.LayerAttribute = cmbxChdLayerAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void rbGhbLeakanceUniform_CheckedChanged(object sender, EventArgs e)
        {
            textBoxGhbLeakanceUniformValue.Enabled = rbGhbLeakanceUniform.Checked;
            Changed = true;
        }

        private void rbGhbLeakanceFromAttribute_CheckedChanged(object sender, EventArgs e)
        {
            cmbxGhbLeakanceAttribute.Enabled = rbGhbLeakanceFromAttribute.Checked;
            Changed = true;
        }

        private void textBoxGhbLeakanceUniformValue_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void cmbxGhbLeakanceAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                GeoValue geoValue = _featureSet.GetGeoValueByDescriptor("leakance");
                if (geoValue.Attribute != cmbxGhbLeakanceAttribute.Text)
                {
                    geoValue.Attribute = cmbxGhbLeakanceAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void rbGhbSameLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGhbSameLayer.Checked)
            {
                _featureSet.LayMethod = LayerMethod.Uniform;
            }
            SetupGhbLayerAssignment();
            Changed = true;
        }

        private void rbGhbLayerAttribute_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGhbLayerAttribute.Checked)
            {
                _featureSet.LayMethod = LayerMethod.ByAttribute;
            }
            SetupGhbLayerAssignment();
            Changed = true;
        }

        private void rbGhbByTops_CheckedChanged(object sender, EventArgs e)
        {
            SetupGhbLayerAssignment();
            Changed = true;
        }

        private void udGhbLayerPicker_ValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                int newValue = Convert.ToInt32(udGhbLayerPicker.Value);
                if (_featureSet.DefaultLayer != newValue)
                {
                    _featureSet.DefaultLayer = newValue;
                    Changed = true;
                }
            }
        }

        private void cmbxGhbLayerAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                if (_featureSet.LayerAttribute != cmbxGhbLayerAttribute.Text)
                {
                    _featureSet.LayerAttribute = cmbxGhbLayerAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void btnRivSecondarySmp_Click(object sender, EventArgs e)
        {
            FindFeatureSetFile(PackageType.RiverType, FileType.SecondarySmpfile);
        }

        private void textBoxRivSecondarySmp_TextChanged(object sender, EventArgs e)
        {
            _featureSet.TimeSeriesSecondaryAbsolutePath = textBoxRivSecondarySmp.Text;
            Changed = true;
        }

        private void rbRivHydCondUniform_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRivHydCondUniformValue.Enabled = rbRivHydCondUniform.Checked;
            Changed = true;
        }

        private void textBoxRivHydCondUniformValue_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void textBoxRivWidthUniformValue_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void textBoxRivThicknessUniformValue_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void rbRivWidthUniform_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRivWidthUniformValue.Enabled = rbRivWidthUniform.Checked;
            Changed = true;
        }

        private void rbRivThicknessUniform_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRivThicknessUniformValue.Enabled = rbRivThicknessUniform.Checked;
            Changed = true;
        }

        private void rbRivHydCondFromAttribute_CheckedChanged(object sender, EventArgs e)
        {
            cmbxRivHydCondAttribute.Enabled = rbRivHydCondFromAttribute.Checked;
            Changed = true;
        }

        private void rbRivWidthFromAttribute_CheckedChanged(object sender, EventArgs e)
        {
            cmbxRivWidthAttribute.Enabled = rbRivWidthFromAttribute.Checked;
            Changed = true;
        }

        private void rbRivThicknessFromAttribute_CheckedChanged(object sender, EventArgs e)
        {
            cmbxRivThicknessAttribute.Enabled = rbRivThicknessFromAttribute.Checked;
            Changed = true;
        }

        private void cmbxRivHydCondAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                GeoValue geoValue = _featureSet.GetGeoValueByDescriptor("hydraulic conductivity");
                if (geoValue.Attribute != cmbxRivHydCondAttribute.Text)
                {
                    geoValue.Attribute = cmbxRivHydCondAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void cmbxRivWidthAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                GeoValue geoValue = _featureSet.GetGeoValueByDescriptor("width");
                if (geoValue.Attribute != cmbxRivWidthAttribute.Text)
                {
                    geoValue.Attribute = cmbxRivWidthAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void cmbxRivThicknessAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                GeoValue geoValue = _featureSet.GetGeoValueByDescriptor("thickness");
                if (geoValue.Attribute != cmbxRivThicknessAttribute.Text)
                {
                    geoValue.Attribute = cmbxRivThicknessAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void rbRivSameLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRivSameLayer.Checked)
            {
                _featureSet.LayMethod = LayerMethod.Uniform;
            }
            SetupRivLayerAssignment();
            Changed = true;
        }

        private void udRivLayerPicker_ValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                int newValue = Convert.ToInt32(udRivLayerPicker.Value);
                if (_featureSet.DefaultLayer != newValue)
                {
                    _featureSet.DefaultLayer = newValue;
                    Changed = true;
                }
            }
        }

        private void rbRivLayerAttribute_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRivLayerAttribute.Checked)
            {
                _featureSet.LayMethod = LayerMethod.ByAttribute;
            }
            SetupRivLayerAssignment();
            Changed = true;
        }

        private void cmbxRivLayerAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                if (_featureSet.LayerAttribute != cmbxRivLayerAttribute.Text)
                {
                    _featureSet.LayerAttribute = cmbxRivLayerAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void rbRivByTops_CheckedChanged(object sender, EventArgs e)
        {
            SetupRivLayerAssignment();
            Changed = true;
        }

        private void cmbxRivBottomElevAttribute_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_populatingControls)
            {
                GeoValue geoValue = _featureSet.GetGeoValueByDescriptor("riverbed bottom elevation");
                if (geoValue.Attribute != cmbxRivBottomElevAttribute.Text)
                {
                    geoValue.Attribute = cmbxRivBottomElevAttribute.Text;
                    Changed = true;
                }
            }
        }

        private void selectActiveTool(ActiveTool tool)
        {
            // Process the selection
            _processingActiveToolButtonSelection = true;

            toolStripButtonSelect.Checked = false;
            //menuMainMapToolPointer.Checked = false;
            toolStripButtonReCenter.Checked = false;
            //menuMainMapToolReCenter.Checked = false;
            toolStripButtonZoomIn.Checked = false;
            //menuMainMapToolZoomIn.Checked = false;
            toolStripButtonZoomOut.Checked = false;
            //menuMainMapToolZoomOut.Checked = false;

            switch (tool)
            {
                case ActiveTool.Pointer:
                    _activeTool = tool;
                    _mapControl.Cursor = System.Windows.Forms.Cursors.Default;
                    toolStripButtonSelect.Checked = true;
                    //menuMainMapToolPointer.Checked = true;
                    break;
                case ActiveTool.ReCenter:
                    _activeTool = tool;
                    _mapControl.Cursor = _reCenterCursor;
                    toolStripButtonReCenter.Checked = true;
                    //menuMainMapToolReCenter.Checked = true;
                    break;
                case ActiveTool.ZoomIn:
                    _activeTool = tool;
                    _mapControl.Cursor = _zoomInCursor;
                    toolStripButtonZoomIn.Checked = true;
                    //menuMainMapToolZoomIn.Checked = true;
                    break;
                case ActiveTool.ZoomOut:
                    _activeTool = tool;
                    _mapControl.Cursor = _zoomOutCursor;
                    toolStripButtonZoomOut.Checked = true;
                    //menuMainMapToolZoomOut.Checked = true;
                    break;
                default:
                    throw new ArgumentException();
            }
            _processingActiveToolButtonSelection = false;
        }

        private void toolStripButtonSelect_Click(object sender, EventArgs e)
        {
            if (!_processingActiveToolButtonSelection)
            {
                selectActiveTool(ActiveTool.Pointer);
            }

        }

        private void toolStripButtonReCenter_Click(object sender, EventArgs e)
        {
            if (!_processingActiveToolButtonSelection)
            {
                selectActiveTool(ActiveTool.ReCenter);
            }
        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            if (!_processingActiveToolButtonSelection)
            {
                selectActiveTool(ActiveTool.ZoomIn);
            }
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            if (!_processingActiveToolButtonSelection)
            {
                selectActiveTool(ActiveTool.ZoomOut);
            }
        }
        private void zoomToGrid()
        {
            if (_GridOutlineMapLayer != null)
            {
                IEnvelope rect = _GridOutlineMapLayer.Extent;
                _mapControl.SetExtent(rect.MinX, rect.MaxX, rect.MinY, rect.MaxY);
            }
        }

        private void toolStripButtonZoomToGrid_Click(object sender, EventArgs e)
        {
            zoomToGrid();
        }
        private void toolStripButtonFullExtent_Click(object sender, EventArgs e)
        {
            _mapControl.SizeToFullExtent();
        }
        private void updateStatusBarLocationInfo(int x, int y)
        {
            ICoordinate coord = _mapControl.ToMapPoint(x, y);
            string mouseLocation = "";
            string cellCoord = "";

            if (_mapControl.LayerCount > 0)
            {
                mouseLocation = "X: " + coord.X.ToString("#.00") + "  Y: " + coord.Y.ToString("#.00");
            }

            if (_ModelGrid != null)
            {
                GridCell cell = _ModelGrid.FindRowColumn(coord);
                // Process the cell and contour information if the location is within the grid.
                if (cell != null)
                {
                    // Update status bar with current grid cell and contour data
                    cellCoord = "Row " + cell.Row.ToString() + "  Col " + cell.Column.ToString();
                }
            }
            toolStripStatusLabelXyCoords.Text = mouseLocation;
            //toolStripStatusMapXyLocation.Text = mouseLocation;
            toolStripStatusLabelRowCol.Text = cellCoord;
            //toolStripStatusMapCellLocation.Text = cellCoord;

        }

        private void pgDisplay_Layout(object sender, LayoutEventArgs e)
        {
            toolStripStatusLabelRowCol.Visible = true;
            toolStripStatusLabelXyCoords.Visible = true;
        }

        private void pgGhb_Layout(object sender, LayoutEventArgs e)
        {
            toolStripStatusLabelRowCol.Visible = false;
            toolStripStatusLabelXyCoords.Visible = false;
        }

        private void pgGeneral_Layout(object sender, LayoutEventArgs e)
        {
            toolStripStatusLabelRowCol.Visible = false;
            toolStripStatusLabelXyCoords.Visible = false;
        }

        private void pgWells_Layout(object sender, LayoutEventArgs e)
        {
            toolStripStatusLabelRowCol.Visible = false;
            toolStripStatusLabelXyCoords.Visible = false;
        }

        private void pgRiv_Layout(object sender, LayoutEventArgs e)
        {
            toolStripStatusLabelRowCol.Visible = false;
            toolStripStatusLabelXyCoords.Visible = false;
        }

        private void pgChd_Layout(object sender, LayoutEventArgs e)
        {
            toolStripStatusLabelRowCol.Visible = false;
            toolStripStatusLabelXyCoords.Visible = false;
        }

        private void pgRch_Layout(object sender, LayoutEventArgs e)
        {
            toolStripStatusLabelRowCol.Visible = false;
            toolStripStatusLabelXyCoords.Visible = false;
        }
    }
}
