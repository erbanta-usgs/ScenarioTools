using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.UI;
using USGS.Puma.UI.MapViewer;
using USGS.Puma.Modflow;
using USGS.Puma.Modpath;
using USGS.Puma.Modpath.IO;
using USGS.Puma.NTS;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;
using USGS.ModflowTrainingTools;

namespace ModflowOutputViewer
{
    /// <summary>
    /// 
    /// </summary>
    public enum ActiveTool
    {
        Pointer = 0,
        ReCenter = 1,
        ZoomIn = 2,
        ZoomOut = 3,
        DefinePolygon = 4,
        DefineRectangle = 5,
        DefineLineString = 6,
        DefinePoint = 7
    }

    public partial class ModflowOutputViewer : Form
    {
        #region Private fields
        // Controls that will be created at startup
        MapControl mapControl = null;
        IndexMapControl indexMapControl = null;

        #region Toolbar Items
        // Toolbar items
        //private System.Windows.Forms.ToolStripButton toolStripButtonToggleDataPanel;
        //private System.Windows.Forms.ToolStripButton toolStripButtonToggleUtilityPanel;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        //private System.Windows.Forms.ToolStripButton toolStripButtonReCenter;
        //private System.Windows.Forms.ToolStripButton toolStripButtonZoomIn;
        //private System.Windows.Forms.ToolStripButton toolStripButtonZoomOut;
        //private System.Windows.Forms.ToolStripButton toolStripButtonSelect;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        //private System.Windows.Forms.ToolStripButton toolStripButtonFullExtent;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        //private System.Windows.Forms.ToolStripButton toolStripButtonShowGridlines;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        //private System.Windows.Forms.ToolStripButton toolStripButtonEditBasemap;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        //private System.Windows.Forms.ToolStripButton toolStripButtonContourLayer;
        //private System.Windows.Forms.ToolStripButton toolStripButtonGriddedValuesLayer;
        ////private System.Windows.Forms.ToolStripButton toolStripButtonShowBasemap;
        //private System.Windows.Forms.ToolStripButton toolStripButtonEditAnalysis;
        //private System.Windows.Forms.ToolStripButton toolStripButtonZoomToGrid;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        //private System.Windows.Forms.ToolStripLabel toolStripLabelAnalysis;
        //private System.Windows.Forms.ToolStripComboBox toolStripComboBoxAnalysis;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        //private System.Windows.Forms.ToolStripLabel toolStripLabelZoomTo;
        //private System.Windows.Forms.ToolStripLabel toolStripLabelView;
        //private System.Windows.Forms.ToolStripLabel toolStripLabelEdit;
        //private System.Windows.Forms.ToolStripButton toolStripButtonToggleContentsPanel;
        #endregion

        // Miscellaneous
        private bool _DataNodeRightClick = false;
        private int _GriddedValuesDisplayMode = 0;
        private bool _LeftPanelCollapsed = false;
        private bool _RightPanelCollapsed = false;
        private ActiveTool _ActiveTool = ActiveTool.Pointer;
        private bool _ProcessingActiveToolButtonSelection = false;
        private bool _ProcessingPeriodStepLayerSelection = false;
        private bool _ProcessAnalyisTypeSelection = true;
        private bool _ContentsTreeShiftPressed = false;
        private bool _ContentsNodeRightClick = false;
        private LineStringFeedback _LineStringFeedback = null;
        private bool _ContourLayerPreferredVisible = true;
        private bool _GriddedValuesPreferredVisible = true;

        // Datasets
        //private Dictionary<string, DatasetInfo> _Datasets = null;
        private DatasetInfo _Dataset = null;
        private CellCenteredArealGrid _ModelGrid = null;
        private Dictionary<string, FeatureLayer> _DatasetsCurrentDataMapLayers = null;
        private Dictionary<string, FeatureLayer> _DatasetsReferenceDataMapLayers = null;
        private Dictionary<string, FeatureLayer> _DatasetsAnalysisMapLayers = null;
        
        // Current file
        private TreeNode _CurrentFileNode = null;
        private CurrentData _CurrentData = null;
        private LayerDataRecord<float> _CurrentLayerDataRecord = null;
        private LayerDataRecordHeaderCollection _CurrentFileHeaderRecords = null;
        private FeatureLayer _CurrentValuesMapLayer = null;
        private int _CurrentValuesRendererIndex = 0;
        private FeatureLayer _CurrentContourMapLayer = null;
        private FeatureLayer _GridlinesMapLayer = null;
        private FeatureLayer _GridOutlineMapLayer = null;
        private ToolTip _MapTip = null;
        private ContourEngineData _ContourEngineData = null;

        // Reference file
        private TreeNode _ReferenceFileNode = null;
        private FeatureLayer _ReferenceValuesMapLayer = null;
        private LayerDataRecord<float> _ReferenceLayerDataRecord = null;
        private ReferenceData _ReferenceData = null;
        private int _ReferenceValuesRendererIndex = 0;

        // Basemap items
        private Basemap _Basemap = null;
        private List<FeatureLayer> _BasemapLayers = null;
        private bool _ShowBasemap = true;

        // Project definition data
        private ModflowOutputViewerDef _ViewerDef = null;

        // Analysis
        private LayerAnalysis _Analysis = null;
        private List<LayerAnalysis> _AnalysisList = null;
        private FeatureLayer _AnalysisValuesMapLayer = null;
        private Array2d<float> _AnalysisArray = null;

        // Map tools related fields
        private Cursor _ReCenterCursor = null;
        private Cursor _ZoomInCursor = null;
        private Cursor _ZoomOutCursor = null;
        private Feature _HotFeature = null;

        // Printer settings
        private System.Drawing.Printing.PrinterSettings _PrinterSettings = null;
        private System.Drawing.Printing.PageSettings _PdfPageSettings = null;

        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public ModflowOutputViewer(string[] args)
        {
            InitializeComponent();

            _GriddedValuesDisplayMode = 0;

            // Create and initialize map and index map controls
            mapControl = CreateAndInitializeMapControl();
            mapControl.Dock = DockStyle.Fill;
            indexMapControl = new IndexMapControl();
            indexMapControl.Dock = DockStyle.Fill;
            indexMapControl.Cursor = _ReCenterCursor;
            indexMapControl.SourceMap = mapControl;
            legendMap.AutoScroll = true;
            legendMap.LayerVisibilityChanged += new EventHandler<EventArgs>(legendMap_LayerVisibilityChanged);

            splitConMap.Panel1.Controls.Clear();
            splitConMap.Panel1.Controls.Add(mapControl);
            splitConMap.Panel1.Controls.Add(panelMapHeader);
            gboxIndexMap.Controls.Add(indexMapControl);

            // Initialize other data components
            SetAnalysisSummaryTextPanel();
            btnSelectCurrentFile.Enabled = false;
            btnSelectReferenceFile.Enabled = false;

            _MapTip = new ToolTip();

            _ContourEngineData = new ContourEngineData();

            _CurrentData = new CurrentData();
            _ReferenceData = new ReferenceData();

            // Build the reference values and difference values analysis objects
            _AnalysisList = new List<LayerAnalysis>();
            LayerAnalysis analysis = new LayerAnalysis();
            analysis.AnalysisType = LayerAnalysisType.ReferenceLayerValues;
            analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.AllValues;
            analysis.ColorRampOption = DifferenceAnalysisColorRampOption.Rainbow5;
            _AnalysisList.Add(analysis);

            analysis = new LayerAnalysis();
            analysis.AnalysisType = LayerAnalysisType.DifferenceValues;
            analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.AllValues;
            analysis.ColorRampOption = DifferenceAnalysisColorRampOption.Rainbow5;
            _AnalysisList.Add(analysis);

            SetAnalysisSummaryTextPanel();

            //tvwContents.ContextMenuStrip = contextMenuContents;

            //Initialize an empty viewer definition object.
            _ViewerDef = new ModflowOutputViewerDef();
            
            //_Datasets = new Dictionary<string, DatasetInfo>();
            _DatasetsCurrentDataMapLayers = new Dictionary<string, FeatureLayer>();
            _DatasetsReferenceDataMapLayers = new Dictionary<string, FeatureLayer>();
            _DatasetsAnalysisMapLayers = new Dictionary<string, FeatureLayer>();
            _BasemapLayers = new List<FeatureLayer>();

            cboGriddedValuesDisplayOption.Items.Add("Show current data layer as shaded cells");
            cboGriddedValuesDisplayOption.Items.Add("Show reference data layer as shaded cells");
            cboGriddedValuesDisplayOption.Items.Add("Show analysis layer as shaded cells");
            cboGriddedValuesDisplayOption.SelectedIndex = 0;

            cboAnalysisOption.Items.Add("Difference values");
            cboAnalysisOption.SelectedIndex = 0;

            _ReCenterCursor = MapControl.CreateCursor(MapControlCursor.ReCenter);
            _ZoomInCursor = MapControl.CreateCursor(MapControlCursor.ZoomIn);
            _ZoomOutCursor = MapControl.CreateCursor(MapControlCursor.ZoomOut);

            //toolStripButtonSelect.Checked = true;

            _MapTip = new ToolTip();
            _MapTip.ShowAlways = true;

            tvwData.ShowNodeToolTips = true;
            tvwData.HideSelection = false;
            DatasetHelper.InitializeDatasetTreeview(tvwData);
            //btnSelectCurrentFile.Enabled = false;

            BuildCurrentFileSummary();

            // Check the command line arguments and open a dataset if a simulation
            // file was specified.
            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (!string.IsNullOrEmpty(args[0]))
                    {
                        OpenDataset(args[0]);
                        AddCompatibleFiles(_Dataset);
                    }
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public ModflowOutputViewer() : this(null) { }

        #endregion

        #region Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonToggleLeftPanel_Click(object sender, EventArgs e)
        {
            SetLeftPanel(!_LeftPanelCollapsed);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonToggleRightPanel_Click(object sender, EventArgs e)
        {
            SetRightPanel(!_RightPanelCollapsed);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabData_Resize(object sender, EventArgs e)
        {
            tabData.Refresh();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCurrentFile_Click(object sender, EventArgs e)
        {
            OpenCurrentFile(tvwData.SelectedNode);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectReferenceFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (_CurrentFileNode != null)
                {
                    // Return without doing anything if the seleced node is
                    // already the reference file node.
                    if (_ReferenceFileNode != null)
                    {
                        if (_ReferenceFileNode.Tag.Equals(tvwData.SelectedNode.Tag))
                        {
                            return;
                        }
                    }

                    _ProcessAnalyisTypeSelection = false;

                    // Open the specified reference file
                    OpenReferenceFile(tvwData.SelectedNode);

                    // Set the layer link options
                    _ReferenceData.TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                    _ReferenceData.ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;

                    // Get the new reference data layer
                    UpdateReferenceLayerDataRecord();

                    // For now, always set the analysis type to show reference values
                    // and to link reference layer to the current time step and layer
                    int analysisTypeIndex = 1;

                    UpdateReferenceValuesRenderer();

                    cboGriddedValuesDisplayOption.SelectedIndex = 0;
                    SetLayerAnalysisType(analysisTypeIndex, true);

                    _ProcessAnalyisTypeSelection = true;
                }
            }
            finally
            {
                _ProcessAnalyisTypeSelection = true;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectReferenceLayer_Click(object sender, EventArgs e)
        {
            if (_ReferenceData.FileReader != null)
            {
                ReferenceDataLinkOptionDialog dialog = new ReferenceDataLinkOptionDialog(_ReferenceData.TimeStepLinkOption, _ReferenceData.ModelLayerLinkOption, _ReferenceData.FileReader);

                if (dialog.ShowDialog() == DialogResult.OK)
                {

                    _ReferenceData.TimeStepLinkOption = dialog.TimeStepLinkOption;
                    _ReferenceData.ModelLayerLinkOption = dialog.ModelLayerLinkOption;
                    _ReferenceData.SpecifiedTimeStep = 0;
                    _ReferenceData.SpecifiedStressPeriod = 0;
                    _ReferenceData.SpecifiedModelLayer = 0;
                    if (_ReferenceData.TimeStepLinkOption == ReferenceDataTimeStepLinkOption.SpecifyTimeStep)
                    {
                        _ReferenceData.SpecifiedStressPeriod = dialog.SpecifiedStressPeriod;
                        _ReferenceData.SpecifiedTimeStep = dialog.SpecifiedTimeStep;
                    }
                    if (_ReferenceData.ModelLayerLinkOption == ReferenceDataModelLayerLinkOption.SpecifyModelLayer)
                    {
                        _ReferenceData.SpecifiedModelLayer = dialog.SpecifiedModelLayer;
                    }

                    UpdateReferenceLayerDataRecord();
                    UpdateReferenceValuesRenderer();
                    UpdateAnalysisValuesRenderer();
                    SetAnalysisSummaryTextPanel();
                    BuildMapLayers(false);
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvwData_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (tvwData.Nodes.Contains(e.Node))
            {
                e.Cancel = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvwData_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (e.Node.Tag == null)
            {
                btnSelectCurrentFile.Enabled = false;
                btnSelectReferenceFile.Enabled = false;
            }
            else
            {
                DataItemTag tag = (DataItemTag)e.Node.Tag;
                if (tag.IsLayerData)
                {
                    btnSelectCurrentFile.Enabled = true;
                    if (_CurrentFileNode != null)
                    { btnSelectReferenceFile.Enabled = true; }
                }
                else
                {
                    btnSelectCurrentFile.Enabled = false;
                    btnSelectReferenceFile.Enabled = false;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                //_DataNodeRightClick = true;
                tvwData.SelectedNode = e.Node;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvwData_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenCurrentFile(e.Node);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuData_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = tvwData.SelectedNode;
            if (node == null)
            {
                e.Cancel = true;
            }
            else
            {
                DataItemTag tag = tvwData.SelectedNode.Tag as DataItemTag;
                if (tag == null)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (tag.IsFileNode)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuDataEditExcludedValues_Click(object sender, EventArgs e)
        {
            TreeNode node = tvwData.SelectedNode;
            if (node == null)
            { return; }

            DataItemTag tag = node.Tag as DataItemTag;
            if (tag == null)
            { return; }

            if (tag.IsFileNode)
            {
                EditExcludedValuesDialog dialog = new EditExcludedValuesDialog();
                dialog.HNoFlo = tag.HNoFlo;
                dialog.HDry = tag.HDry;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tag.HNoFlo = dialog.HNoFlo;
                    tag.HDry = dialog.HDry;
                }

                // Check to see if the file that was just changed is either
                // the current file or the reference file. If so, close the
                // file and reopen it for the changes to take effect.
                if (node.Equals(_CurrentFileNode))
                {
                    CloseCurrentFile();
                    OpenCurrentFile(node);
                }
                else if (node.Equals(_ReferenceFileNode))
                {
                    CloseReferenceFile();
                    OpenReferenceFile(node);
                }
            }
        }

        #endregion

        #region Main Menu Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileOpenDataset_Click(object sender, EventArgs e)
        {
            BrowseModflowDatasets();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileCloseDataset_Click(object sender, EventArgs e)
        {
            CloseDataset();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainCloseCurrentFile_Click(object sender, EventArgs e)
        {
            if (_CurrentFileNode != null)
            { CloseCurrentFile(); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileAddFile_Click(object sender, EventArgs e)
        {
            BrowseModflowLayerFiles();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileNewBasemap_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "New Basemap File";
            dialog.Filter = "Basemaps (*.basemap)|*.basemap|All files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.DefaultExt = "pbm";
            dialog.CheckFileExists = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if(File.Exists(dialog.FileName))
                {
                    string messageText = "The specified file already exists." + Environment.NewLine + "Do you want to overwrite it?";
                    DialogResult result = MessageBox.Show(messageText, "File exists", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    { return; }
                }

                Basemap newBasemap = new Basemap();
                Basemap.Write(dialog.FileName, newBasemap);
                LoadBasemap(dialog.FileName);
                if (_Basemap != null)
                {
                    BasemapEditDialog editBasemapDialog = new BasemapEditDialog(_Basemap);
                    if (editBasemapDialog.ShowDialog() == DialogResult.OK)
                    {
                        _BasemapLayers = _Basemap.CreateBasemapLayers();
                        BuildMapLayers(false);
                        indexMapControl.UpdateMapImage();
                    }
                }

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileAddBasemap_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open Basemap File";
            dialog.Filter = "Basemaps (*.basemap)|*.basemap|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadBasemap(dialog.FileName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileRemoveBasemap_Click(object sender, EventArgs e)
        {
            _Basemap = null;
            _BasemapLayers.Clear();
            if (_ModelGrid != null)
            { BuildMapLayers(true); }
            else
            { BuildMapLayers(false); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileSaveBasemap_Click(object sender, EventArgs e)
        {
            if (_Basemap != null)
            {
                Basemap.Write(_Basemap);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileExportCurrentFileXml_Click(object sender, EventArgs e)
        {
            if (_CurrentData.FileReader != null)
            {
                string filename = _CurrentData.FileReader.Filename + ".xml";
                XmlLayerDataWriter<float>.Write(_CurrentData.FileReader, filename);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileExportShapefiles_Click(object sender, EventArgs e)
        {
            if (_Dataset == null)
            { return; }

            ExportShapefilesDialog dialog = new ExportShapefilesDialog(_Dataset.ParentFolderName);
            dialog.GridOutlineLayer = _GridOutlineMapLayer;
            dialog.GridlinesLayer = _GridlinesMapLayer;
            dialog.ContourLayer = _CurrentContourMapLayer;
            dialog.CurrentValuesLayer = _CurrentValuesMapLayer;
            dialog.AnalysisValuesLayer = _AnalysisValuesMapLayer;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Add code to write message to status strip
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileSaveBinaryOutput_Click(object sender, EventArgs e)
        {
            bool validData = false;
            if (_CurrentData != null)
            {
                if (_CurrentData.FileReader != null)
                {
                    if (_CurrentData.FileReader.OutputPrecision != OutputPrecisionType.Undefined)
                    {
                        validData = true;
                    }
                }
            }

            if (validData)
            {
                SaveNewBinaryOutputDialog dialog = new SaveNewBinaryOutputDialog();
                dialog.InitializeDialog(_CurrentData.FileReader);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Nothing to do here
                }
            }
            else
            {
                MessageBox.Show("No current data has been specified.");
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFilePrintPreview_Click(object sender, EventArgs e)
        {
            if (mapControl.LayerCount > 0)
            {
                PrintMap(true);
            }
            else
            {
                MessageBox.Show("The map is empty.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFilePrint_Click(object sender, EventArgs e)
        {
            if (mapControl.LayerCount > 0)
            {
                PrintMap(false);
            }
            else
            {
                MessageBox.Show("The map is empty.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFilePrintPDF_Click(object sender, EventArgs e)
        {
            if (mapControl.LayerCount > 0)
            {
                PrintPDF();
            }
            else
            {
                MessageBox.Show("The map is empty.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainFileExit_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainEditBasemap_Click(object sender, EventArgs e)
        {
            EditBasemap();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainEditModflowMetadata_Click(object sender, EventArgs e)
        {
            EditMetadata();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainViewHideSidePanels_Click(object sender, EventArgs e)
        {
            SetSidePanels(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainViewShowSidePanels_Click(object sender, EventArgs e)
        {
            SetSidePanels(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainHelpAbout_Click(object sender, EventArgs e)
        {
            AboutBoxModflowOutputViewer dialog = new AboutBoxModflowOutputViewer();
            dialog.ShowDialog();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainMapPointer_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.Pointer);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainMapReCenter_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ReCenter);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainMapZoomIn_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomIn);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainMapZoomOut_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomOut);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainMapZoomModelGrid_Click(object sender, EventArgs e)
        {
            ZoomToGrid();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMainMapZoomFullExtent_Click(object sender, EventArgs e)
        {
            mapControl.SizeToFullExtent();
        }

        #endregion

        #region Tool Strip Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomIn);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonReCenter_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ReCenter);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomOut);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSelect_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.Pointer);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonFullExtent_Click(object sender, EventArgs e)
        {
            mapControl.SizeToFullExtent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomToGrid_Click(object sender, EventArgs e)
        {
            ZoomToGrid();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEditCurrentDataLayer_Click(object sender, EventArgs e)
        {
            SelectCellValuesRendererDialog dialog = new SelectCellValuesRendererDialog(_CurrentValuesRendererIndex);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _CurrentValuesRendererIndex = dialog.SelectedIndex;
                UpdateCurrentValuesRendererColorRamp();
                BuildMapLegend();
                mapControl.Refresh();
                indexMapControl.UpdateMapImage();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEditReferenceDataLayer_Click(object sender, EventArgs e)
        {
            SelectCellValuesRendererDialog dialog = new SelectCellValuesRendererDialog(_ReferenceValuesRendererIndex);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _ReferenceValuesRendererIndex = dialog.SelectedIndex;
                UpdateReferenceValuesRendererColorRamp();
                BuildMapLegend();
                mapControl.Refresh();
                indexMapControl.UpdateMapImage();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEditAnalysisLayer_Click(object sender, EventArgs e)
        {
            EditAnalysisLayerDialog dialog = new EditAnalysisLayerDialog(_Analysis);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dialog.GetData(_Analysis);

                if (_CurrentLayerDataRecord != null)
                {
                    // Update the analysis values renderer
                    UpdateAnalysisValuesRenderer();

                    // Rebuild the map layers
                    BuildMapLayers(false);
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEditBasemap_Click(object sender, EventArgs e)
        {
            EditBasemap();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEditMetadata_Click(object sender, EventArgs e)
        {
            EditMetadata();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonContourLayer_Click(object sender, EventArgs e)
        {
            EditContourProperties();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboGriddedValuesDisplayOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetGriddedValuesDisplayMode(cboGriddedValuesDisplayOption.SelectedIndex, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonHideSidePanels_Click(object sender, EventArgs e)
        {
            SetLeftPanel(true);
            SetRightPanel(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonShowSidePanels_Click(object sender, EventArgs e)
        {
            SetLeftPanel(false);
            SetRightPanel(false);

        }

        #endregion

        #region Map control event handlers
        private void mapControl_MouseClick(object sender, MouseEventArgs e)
        {
            ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
            switch (_ActiveTool)
            {
                case ActiveTool.Pointer:
                    ShowMapTip(e.X, e.Y);
                    break;
                case ActiveTool.ReCenter:
                    mapControl.Center = pt;
                    break;
                case ActiveTool.ZoomIn:
                    mapControl.Zoom(2.0, pt.X, pt.Y);
                    break;
                case ActiveTool.ZoomOut:
                    mapControl.Zoom(0.5, pt.X, pt.Y);
                    break;
                default:
                    break;
            }
        }
        private void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateStatusBarLocationInfo(e.X, e.Y);

            switch (_ActiveTool)
            {
                case ActiveTool.Pointer:
                    FindContourLineHit(e.X, e.Y);
                    if (_HotFeature == null)
                    { mapControl.Cursor = System.Windows.Forms.Cursors.Default; }
                    else
                    { mapControl.Cursor = System.Windows.Forms.Cursors.Hand; }
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
        private void mapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (_ActiveTool)
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

        #region Legend Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void legendMap_LayerVisibilityChanged(object sender, EventArgs e)
        {
            // Add code to loop through legend map layers and save the current
            // visible state for the contour and gridded values layer, if present.
            for (int i = 0; i < legendMap.Items.Count; i++)
            {
                GraphicLayer mapLayer= legendMap.Items[i].MapLayer;
                if (_CurrentContourMapLayer != null)
                {
                    if (mapLayer.Equals(_CurrentContourMapLayer))
                    {
                        _ContourLayerPreferredVisible = _CurrentContourMapLayer.Visible;
                    }
                }
                if (_CurrentValuesMapLayer != null)
                {
                    if (mapLayer.Equals(_CurrentValuesMapLayer))
                    {
                        _GriddedValuesPreferredVisible = _CurrentValuesMapLayer.Visible;
                    }
                }
                if (_ReferenceValuesMapLayer != null)
                {
                    if (mapLayer.Equals(_ReferenceValuesMapLayer))
                    {
                        _GriddedValuesPreferredVisible = _ReferenceValuesMapLayer.Visible;
                    }
                }
                if (_AnalysisValuesMapLayer != null)
                {
                    if (mapLayer.Equals(_AnalysisValuesMapLayer))
                    {
                        _GriddedValuesPreferredVisible = _AnalysisValuesMapLayer.Visible;
                    }
                }
            }

            mapControl.Refresh();
        }

        #endregion

        #region Contents Treeview Event Handler
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvwContents_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _ContentsNodeRightClick = true;
                tvwContents.SelectedNode = e.Node;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvwContents_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvwContents.SelectedNode = e.Node;
            if (e.Node.Tag != null)
            {
                //LayerDataRecordHeader header = (LayerDataRecordHeader)e.Node.Tag;
                //LoadCurrentDataLayer(header);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvwContents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tvwContents.SelectedNode = e.Node;

            if (e.Node.Tag != null)
            {
                // First make sure the treeview gets redrawn before any of the time-consuming map rendering
                // takes place. This will avoid some annoying flashing of the treeview node text.
                tvwContents.Refresh();

                LayerDataRecordHeader header = (LayerDataRecordHeader)e.Node.Tag;
                LoadCurrentDataLayer(header);

            }

        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        private void EditBasemap()
        {
            if (_Basemap != null)
            {
                BasemapEditDialog dialog = new BasemapEditDialog(_Basemap);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _BasemapLayers = _Basemap.CreateBasemapLayers();
                    BuildMapLayers(false);
                    indexMapControl.UpdateMapImage();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        private void LoadBasemap(string filename)
        {
            string basemapDirectory = "";
            string localBasemapName = "";
            Basemap bm = null;
            _Basemap = null;
            try
            {
                basemapDirectory = Path.GetDirectoryName(filename);
                localBasemapName = Path.GetFileName(filename);
                bm = Basemap.Read(basemapDirectory, localBasemapName);
                _Basemap = bm;
                _BasemapLayers = _Basemap.CreateBasemapLayers();
            }
            catch (Exception)
            {
                _Basemap = null;
                _BasemapLayers = null;
                System.Windows.Forms.MessageBox.Show("The basemap could not be loaded.");
            }

            BuildMapLayers(false);
            BuildMapLegend();

        }
        /// <summary>
        /// 
        /// </summary>
        private void ClearMapLegend()
        {
            legendMap.Clear(true);
        }
        /// <summary>
        /// 
        /// </summary>
        private void BuildMapLegend()
        {
            ClearMapLegend();
            Collection<GraphicLayer> legendItems = new Collection<GraphicLayer>();

            if (_CurrentFileNode != null)
            {
                legendMap.LegendTitle = _CurrentFileNode.Text;
            }

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

            // Add contour layer if present
            if (_CurrentContourMapLayer != null)
            {
                legendItems.Add(_CurrentContourMapLayer);
            }

            // Add gridded values layers
            if (_GriddedValuesDisplayMode == 0)
            {
                if (_CurrentLayerDataRecord != null)
                {
                    legendItems.Add(_CurrentValuesMapLayer);
                }
            }
            else if (_GriddedValuesDisplayMode == 1)
            {
                if (_ReferenceLayerDataRecord != null)
                {
                    legendItems.Add(_ReferenceValuesMapLayer);
                }
            }
            else if (_GriddedValuesDisplayMode == 2)
            {
                if (_ReferenceLayerDataRecord != null)
                {
                    legendItems.Add(_AnalysisValuesMapLayer);
                }
            }

            // Add basemap layers
            if (_Basemap != null)
            {
                if (_BasemapLayers != null)
                {
                    if (legendMap.LegendTitle == "")
                    {
                        legendMap.LegendTitle = "Basemap:" + _Basemap.Filename;
                    }
                    for (int i = 0; i < _BasemapLayers.Count; i++)
                    {
                        legendItems.Add(_BasemapLayers[i]);
                    }
                }
            }

            legendMap.AddItems(legendItems);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="filename"></param>
        private void ExportFeatureLayerAsShapefile(FeatureLayer layer, string filename)
        {
            if (layer != null)
            {
                if (layer.FeatureCount > 0)
                {
                    FeatureCollection fc = layer.GetFeatures();
                    ExportFeaturesAsShapefile(fc, filename);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="features"></param>
        /// <param name="filename"></param>
        private void ExportFeaturesAsShapefile(FeatureCollection features, string filename)
        {
            if (features != null)
            {
                if (features.Count > 0)
                {
                    USGS.Puma.NTS.IO.ShapefileDataWriter writer = new USGS.Puma.NTS.IO.ShapefileDataWriter(filename);
                    writer.Write(features);
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string FindNameFileFromHeadFile(string filename)
        {
            string basename = System.IO.Path.GetFileNameWithoutExtension(filename);
            basename = basename + ".nam";
            string parentDirectory = System.IO.Path.GetDirectoryName(filename);
            string s = System.IO.Path.Combine(parentDirectory, basename);
            if (System.IO.File.Exists(s))
            { return s; }
            else
            { return ""; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        //private BinaryLayerDataReader<float> OpenHeadFileReader(string dataFile)
        //{

        //    BinaryLayerDataReader<float> reader = new BinaryLayerDataReader<float>(dataFile);

        //    // Now check to see if a valid binary Modflow layer-data file was opened.
        //    bool isValid = false;
        //    if (reader != null)
        //    {
        //        isValid = reader.Valid;
        //    }

        //    return reader;

        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        private void InitializeHeadFileNavControls(LayerDataRecordHeaderCollection headers)
        {
            string key = null;

            tvwContents.BeginUpdate();
            tvwContents.Nodes.Clear();
            tvwContents.EndUpdate();

            if (headers == null)
            { return; }

            // Turn off treeview drawing while building node structure
            tvwContents.BeginUpdate();

            TreeNode contentsRootNode = tvwContents.Nodes.Add("Current file data layers");
            contentsRootNode.ImageIndex = 2;
            contentsRootNode.SelectedImageIndex = 2;
            TreeNode node = null;
            string keyLayer = "";

            int count = 0;
            foreach (LayerDataRecordHeader header in headers)
            {
                count += 1;
                key = header.StressPeriod.ToString();
                if (contentsRootNode.Nodes.ContainsKey(key))
                { node = contentsRootNode.Nodes[key]; }
                else
                {
                    node = contentsRootNode.Nodes.Add(key, "Period " + header.StressPeriod.ToString());
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }

                key = key + "," + header.TimeStep.ToString();
                if (node.Nodes.ContainsKey(key))
                { node = node.Nodes[key]; }
                else
                {
                    node = node.Nodes.Add(key, "Step " + header.TimeStep.ToString());
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }

                keyLayer = header.Layer.ToString();
                key = key + "," + keyLayer;
                if (node.Nodes.ContainsKey(key))
                { node = node.Nodes[key]; }
                else
                {
                    node = node.Nodes.Add(key, "Layer " + header.Layer.ToString());
                    node.ImageIndex = 4;
                    node.SelectedImageIndex = 4;
                }
                node.Tag = header;
                if (count == 1)
                {
                    tvwContents.SelectedNode = node;
                }

            }

            if (contentsRootNode.Nodes.Count == 1)
            {
                contentsRootNode.Expand();
                contentsRootNode.Nodes[0].Expand();
                if (contentsRootNode.Nodes[0].Nodes.Count == 1)
                { contentsRootNode.Nodes[0].Nodes[0].Expand(); }
            }

            // Re-establish treeview drawing mode.
            tvwContents.EndUpdate();


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private string BuildRecordKey(LayerDataRecordHeader header)
        {
            if 
                (header == null)
            { return ""; }
            else
            { return BuildRecordKey(header.StressPeriod, header.TimeStep, header.Layer, header.Text); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stressPeriodIndex"></param>
        /// <param name="timeStepIndex"></param>
        /// <param name="layerIndex"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private string BuildRecordKey(int stressPeriodIndex, int timeStepIndex, int layerIndex, string dataType)
        {
            try
            {
                string key = stressPeriodIndex.ToString() + "," + timeStepIndex.ToString() + "," + layerIndex.ToString() + "," + dataType.Trim().ToUpper();
                return key;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private LayerDataRecord<float> GetLayerDataRecord(string key, BinaryLayerReader reader)
        {
            LayerDataRecord<float> rec = null;
            //Do not attempt to process if key is blank.
            if (string.IsNullOrEmpty(key.Trim()))
                return null;

            // Make sure that a valid MODFLOW binary layer reader exists. If not, return.
            if (reader == null) return null;
            if (!reader.Valid) return null;

            // Use the record key it to find the layer record index.
            int recIndex = reader.FindRecordIndex(key);

            // If the record index is valid, read the layer record.
            if (recIndex > -1)
            {
                rec = reader.GetRecordAsSingle(recIndex);
            }

            return rec;
        }
        /// <summary>
        /// 
        /// </summary>
        private void BrowseModflowLayerFiles()
        {
            // Setup an open file dialog. Set the file filter to show Modflow name files that
            // have an extension ".nam".
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Add a MODFLOW Binary Layer Output File";
            dialog.Filter = "Head and Drawdown files (*.hed;*.ddn)|*.hed;*.ddn|All files (*.*)|*.*";
            dialog.Multiselect = true;

            // Show the dialog and process the results if the OK button was pressed.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddFiles(dialog.FileNames);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenames"></param>
        private void AddFiles(string[] filenames)
        {
            bool isValid = false;
            float hNoFlo;
            float hDry;
            Collection<string> dataTypes = null;

            foreach (string filename in filenames)
            {
                BinaryLayerReader reader = null;
                try
                {
                    reader = new BinaryLayerReader(filename);
                    if (reader != null)
                    {
                        isValid = reader.Valid;
                        if (isValid)
                        {
                            if (_ModelGrid != null)
                            {
                                if (_ModelGrid.RowCount == reader.RowCount && _ModelGrid.ColumnCount == reader.ColumnCount)
                                {
                                    dataTypes = reader.GetDataTypes();
                                }
                                else
                                {
                                    isValid = false;
                                    MessageBox.Show("The layer output file dimensions do not match the current open dataset."); 
                                }
                            }
                            else
                            {
                                _ModelGrid = new CellCenteredArealGrid(reader.RowCount, reader.ColumnCount, 1, 0, 0, 0);
                                dataTypes = reader.GetDataTypes();
                            }
                        }
                        else
                        {
                            MessageBox.Show("The file is not a valid Modflow binary layer file.");
                        }


                        if (isValid)
                        {
                            if (_Dataset != null)
                            {
                                hNoFlo = _Dataset.HNoFlo;
                                hDry = _Dataset.HDry;
                            }
                            else
                            {
                                hNoFlo = (float)1.0E+30;
                                hDry = (float)1.0E+20;
                            }
                            TreeNode node = DatasetHelper.AddFile(tvwData, filename, dataTypes, hNoFlo, hDry);
                        }

                        reader.Close();
                        reader = null;

                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filenames"></param>
        /// <returns></returns>
        public string[] FindCompatibleHeadFiles(string[] filenames, int rowCount, int columnCount)
        {
            List<string> headFiles = new List<string>();
            bool isValid = false;

            foreach (string filename in filenames)
            {
                BinaryLayerReader reader = null;
                try
                {
                    reader = new BinaryLayerReader(filename);
                    if (reader != null)
                    {
                        isValid = reader.Valid;
                        if (isValid)
                        {
                            if (rowCount != reader.RowCount || columnCount != reader.ColumnCount)
                            {
                                isValid = false;
                            }
                        }


                        if (isValid)
                        {
                            headFiles.Add(filename);
                        }

                        reader.Close();
                        reader = null;

                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                }
            }

            return headFiles.ToArray<string>();

        }
        /// <summary>
        /// 
        /// </summary>
        private void CloseDataset()
        {
            if (_Dataset == null)
            { return; }

            // If necessary, close any open data layers
            if (_CurrentFileNode != null)
            { CloseCurrentFile(); }

            // Now remove the dataset
            //_Datasets.Clear();
            _DatasetsAnalysisMapLayers.Clear();
            _DatasetsCurrentDataMapLayers.Clear();
            _DatasetsReferenceDataMapLayers.Clear();
            _GridlinesMapLayer = null;
            _GridOutlineMapLayer = null;
            _Dataset = null;
            _ModelGrid = null;

            // Turn off treeview drawing while updating the nodes
            tvwData.BeginUpdate();
            // Delete and reset the dataset node
            tvwData.Nodes["data"].Nodes.RemoveByKey("dataset");
            TreeNode node = tvwData.Nodes["data"].Nodes.Insert(0, "dataset", "Dataset: <none>");
            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;

            // Delete the ancillary file nodes
            tvwData.Nodes["data"].Nodes["files"].Nodes.Clear();

            // Turn treeview drawing back on
            tvwData.EndUpdate();

            // Disable the SelectCurrentFile and SelectReferenceFile buttons
            btnSelectCurrentFile.Enabled = false;
            btnSelectReferenceFile.Enabled = false;

            // Update the map and other controls
            SetAnalysisSummaryTextPanel();
            BuildMapLayers(true);
            mapControl.Refresh();
            indexMapControl.UpdateMapImage();

            tvwData.Focus();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void OpenCurrentFile(TreeNode node)
        {
            OpenCurrentFile(node, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="refDataNode"></param>
        private void OpenCurrentFile(TreeNode node, TreeNode refDataNode)
        {
            try
            {
                if (node == null)
                    return;
                if (node.Tag == null)
                    return;
                DataItemTag tag = (DataItemTag)node.Tag;
                if (tag.IsLayerData)
                {
                    // Check to see if the selected node is already the
                    // current file node. If so, return without doing anything.
                    if (_CurrentFileNode != null)
                    {
                        if (_CurrentFileNode.Tag.Equals(node.Tag))
                        {
                            return;
                        }
                    }
                    // Close the current file
                    CloseCurrentFile();

                    if (System.IO.File.Exists(tag.Pathname))
                    {
                        if (_CurrentData.OpenFile(tag.Pathname, tag.HNoFlo, tag.HDry))
                        {
                            if (_CurrentData.FileReader.Valid)
                            {
                                _CurrentFileHeaderRecords = _CurrentData.FileReader.ReadAllRecordHeaders();
                                InitializeHeadFileNavControls(_CurrentFileHeaderRecords);

                                // Set current file info
                                _CurrentFileNode = node;
                                _CurrentData.HNoFlo = tag.HNoFlo;
                                _CurrentData.HDry = tag.HDry;

                                // Set reference file info
                                if (refDataNode == null)
                                { 
                                    OpenReferenceFile(_CurrentFileNode);
                                    _CurrentFileNode.ImageIndex = 7;
                                    _CurrentFileNode.SelectedImageIndex = 7;
                                }
                                else
                                { 
                                    OpenReferenceFile(refDataNode);
                                    _CurrentFileNode.ImageIndex = 5;
                                    _CurrentFileNode.SelectedImageIndex = 5;
                                    refDataNode.ImageIndex = 6;
                                    refDataNode.SelectedImageIndex = 6;
                                }

                                // Reset layer link options to point to the layer below at the current
                                // time step.
                                _ProcessAnalyisTypeSelection = false;
                                _ReferenceData.TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                                _ReferenceData.ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;
                                cboGriddedValuesDisplayOption.SelectedIndex = 0;
                                SetLayerAnalysisType(1, false);

                                _ProcessAnalyisTypeSelection = true;

                                BuildCurrentFileSummary();

                                // Get the Current data values map layer. Create it if it does not already exist.
                                if (_DatasetsCurrentDataMapLayers.ContainsKey(tag.DatasetKey))
                                { _CurrentValuesMapLayer = _DatasetsCurrentDataMapLayers[tag.DatasetKey]; }
                                else
                                {
                                    _CurrentValuesMapLayer = CreateValuesMapLayer();
                                    _CurrentValuesMapLayer.LayerName = "Current Data Values";
                                    _DatasetsCurrentDataMapLayers.Add(tag.DatasetKey, _CurrentValuesMapLayer);
                                }

                                // Get the Reference data values map layer. Create it if it does not already exist.
                                if (_DatasetsReferenceDataMapLayers.ContainsKey(tag.DatasetKey))
                                { _ReferenceValuesMapLayer = _DatasetsReferenceDataMapLayers[tag.DatasetKey]; }
                                else
                                {
                                    _ReferenceValuesMapLayer = CreateValuesMapLayer();
                                    _ReferenceValuesMapLayer.LayerName = "Reference Data Values";
                                    _DatasetsReferenceDataMapLayers.Add(tag.DatasetKey, _ReferenceValuesMapLayer);
                                }

                                // Get the Analysis map layer. Create it if it does not already exist.
                                if (_DatasetsAnalysisMapLayers.ContainsKey(tag.DatasetKey))
                                { _AnalysisValuesMapLayer = _DatasetsAnalysisMapLayers[tag.DatasetKey]; }
                                else
                                {
                                    _AnalysisValuesMapLayer = CreateValuesMapLayer();
                                    _AnalysisValuesMapLayer.LayerName = "Analysis Values";
                                    _DatasetsAnalysisMapLayers.Add(tag.DatasetKey, _AnalysisValuesMapLayer);
                                }

                                if (_CurrentFileHeaderRecords.Count > 0)
                                {
                                    LayerDataRecordHeader firstRecord = _CurrentFileHeaderRecords[0];
                                    LoadCurrentDataLayer(firstRecord);
                                }
                                else
                                {
                                    lblCurrentLayerDescription.Text = "";
                                    BuildMapLayers(true);
                                }

                            }
                            else
                            { _CurrentData.CloseFile(); }

                        }
                        else
                        {
                            _CurrentData.CloseFile();

                        }
                    }
                    else
                    {
                        // Modflow output file does not exist.
                    }
                }

            }
            finally
            {
                _ProcessAnalyisTypeSelection = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void OpenReferenceFile(TreeNode node)
        {
            if (node == null)
                return;
            if (node.Tag == null)
                return;
            if (_CurrentFileNode == null)
            { return; }

            DataItemTag tag = (DataItemTag)node.Tag;
            if (tag.IsLayerData)
            {
                CloseReferenceFile();

                if (System.IO.File.Exists(tag.Pathname))
                {
                    if (!_ReferenceData.OpenFile(tag.Pathname, tag.HNoFlo, tag.HDry))
                    { MessageBox.Show("Reference file could not be opened."); }
                    else
                    { 
                        _ReferenceFileNode = node;
                        if (_ReferenceFileNode.Equals(_CurrentFileNode))
                        {
                            _CurrentFileNode.ImageIndex = 7;
                            _CurrentFileNode.SelectedImageIndex = 7;
                            _ReferenceFileNode.ImageIndex = 7;
                            _ReferenceFileNode.SelectedImageIndex = 7;
                        }
                        else
                        {
                            _CurrentFileNode.ImageIndex = 5;
                            _CurrentFileNode.SelectedImageIndex = 5;
                            _ReferenceFileNode.ImageIndex = 6;
                            _ReferenceFileNode.SelectedImageIndex = 6;
                        }
                    }
                }
                else
                {
                    // reference file not found
                }

                _ReferenceData.TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                _ReferenceData.ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;

                SetAnalysisSummaryTextPanel();

            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void CloseCurrentFile()
        { CloseCurrentFile(true); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="refresh"></param>
        private void CloseCurrentFile(bool refresh)
        {
            if (_CurrentFileNode != null)
            {
                _CurrentFileNode.ImageIndex = 1;
                _CurrentFileNode.SelectedImageIndex = 1;
            }
            _CurrentFileNode = null;
            _CurrentLayerDataRecord = null;
            _CurrentContourMapLayer = null;
            _CurrentValuesMapLayer = null;

            _CurrentData.CloseFile();
            _ReferenceData.CloseFile();

            lblCurrentLayerDescription.Text = "Current data layer: <none>";
            btnSelectReferenceLayer.Enabled = false;

            tvwContents.BeginUpdate();
            tvwContents.Nodes.Clear();
            tvwContents.EndUpdate();

            if (_ReferenceFileNode != null)
            {
                _ReferenceFileNode.ImageIndex = 1;
                _ReferenceFileNode.SelectedImageIndex = 1;
            }
            _ReferenceFileNode = null;
            _ReferenceLayerDataRecord = null;
            _ReferenceValuesMapLayer = null;
            _AnalysisValuesMapLayer = null;
            _AnalysisArray = null;

            BuildMapLayers(true);
            //UpdateCurrentValuesLayerLegend();
            //UpdateAnalysisValuesLayerLegend();
            SetAnalysisSummaryTextPanel();
            if (refresh)
            {
                mapControl.Refresh();
                indexMapControl.UpdateMapImage();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        private void CloseReferenceFile()
        {
            if (_ReferenceFileNode != null)
            {
                _ReferenceFileNode.ImageIndex = 1;
                _ReferenceFileNode.SelectedImageIndex = 1;
            }
            if (_CurrentFileNode != null)
            {
                _CurrentFileNode.ImageIndex = 5;
                _CurrentFileNode.SelectedImageIndex = 5;
            }
            _ReferenceFileNode = null;
            _ReferenceData.CloseFile();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        private void LoadCurrentDataLayer(LayerDataRecordHeader header)
        {
            try
            {
                if (_CurrentFileNode != null && header != null)
                {
                    // Get the new current layer data record
                    DataItemTag tag = _CurrentFileNode.Tag as DataItemTag;
                    string key = BuildRecordKey(header);
                    _CurrentLayerDataRecord = null;
                    _ReferenceLayerDataRecord = null;
                    _CurrentLayerDataRecord = GetLayerDataRecord(key, _CurrentData.FileReader);

                    // Get the new reference data layer
                    UpdateReferenceLayerDataRecord();

                    // Generate the new data map layers and renderers
                    GenerateAndBuildContourLayer(_CurrentLayerDataRecord.DataArray, _ModelGrid);
                    UpdateCurrentValuesRenderer();
                    UpdateReferenceValuesRenderer();
                    UpdateAnalysisValuesRenderer();


                    // Update the current layer description text
                    if (_CurrentLayerDataRecord != null)
                    {
                        lblCurrentLayerDescription.Text = _CurrentFileNode.Text + " :  Period " + _CurrentLayerDataRecord.StressPeriod.ToString() + ", Step " + _CurrentLayerDataRecord.TimeStep.ToString() + ", Model layer " + _CurrentLayerDataRecord.Layer.ToString(); 
                    }
                    else
                    {
                        lblCurrentLayerDescription.Text = "Data layer:  <none>"; 
                    }

                    // Rebuild the map layers
                    BuildMapLayers(false);
                }
                else
                {
                    lblCurrentLayerDescription.Text = "Data layer:  <none>";
                }

            }
            finally
            {
                // add code
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateReferenceLayerDataRecord()
        {
            _ReferenceLayerDataRecord = null;
            _AnalysisArray = null;

            if (_CurrentLayerDataRecord != null)
            {
                btnSelectReferenceLayer.Enabled = true;
                _ReferenceLayerDataRecord = _ReferenceData.GetLayerDataRecord(_CurrentLayerDataRecord);
                if (_ReferenceLayerDataRecord != null)
                {
                    _AnalysisArray = GetAnalysisArray();
                }
            }
            else
            { btnSelectReferenceLayer.Enabled = false; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathname"></param>
        private void OpenDataset(string pathname)
        {
            // Find the dataset name file based on the file pathname
            System.IO.FileInfo fInfo = new FileInfo(pathname);

            string nameFile = "";
            if (fInfo.Extension.ToLower() == ".nam")
            { nameFile = pathname; }
            else
            {
                MessageBox.Show("The selected file is not a MODFLOW name file.");
                return;
                //nameFile = FindNameFileFromHeadFile(pathname); 
            }

            if (!System.IO.File.Exists(nameFile))
            {
                MessageBox.Show("The MODFLOW dataset name file does not exist.");
                return;
            }
            else
            {
                // Check to see if the dataset is already loaded
                string key = nameFile.ToLower();

                if (_Dataset != null)
                { CloseDataset(); }

                DatasetInfo dataset = new DatasetInfo(nameFile);
                if (dataset.Valid)
                {
                    //_Datasets.Add(key, dataset);
                    TreeNode node = DatasetHelper.AddDataset(tvwData, dataset);
                    _Dataset = dataset;
                    _ModelGrid = (CellCenteredArealGrid)_Dataset.Grid;
                    _GridlinesMapLayer = CreateModelGridlinesLayer(_ModelGrid, Color.DarkGray);
                    _GridOutlineMapLayer = CreateModelGridOutlineLayer(_ModelGrid, Color.Black);
                    
                    if (!string.IsNullOrEmpty(_Dataset.Metadata.BasemapFile.Trim()))
                    {
                        string basemapPath = _Dataset.Metadata.BasemapFile;
                        if (!Path.IsPathRooted(basemapPath))
                        {
                            basemapPath = Path.Combine(_Dataset.Metadata.SourcefileDirectory, basemapPath);
                        }
                        LoadBasemap(basemapPath);
                    }
                    else
                    {
                        BuildMapLayers(true);
                    }

                    TreeNode headNode = DatasetHelper.GetHeadNode(tvwData);
                    TreeNode drawdownNode = DatasetHelper.GetDrawdownNode(tvwData);

                    if (headNode != null)
                    {
                        if (drawdownNode == null)
                        { OpenCurrentFile(headNode, null); }
                        else
                        { OpenCurrentFile(headNode, drawdownNode); }
                    }
                    else
                    {
                        if (drawdownNode != null)
                        {
                            OpenCurrentFile(drawdownNode, null);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("The MODFLOW dataset could not be read.");
                    return;
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        private void BrowseModflowDatasets()
        {
            // Setup an open file dialog. Set the file filter to show Modflow name files that
            // have an extension ".nam".
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open a MODFLOW Dataset";
            dialog.Filter = "Modflow datasets (*.nam)|*.nam|Datasets and files (*.nam;*.hed;*.ddn)|*.nam;*.hed;*.ddn|All files (*.*)|*.*";
            dialog.Multiselect = true;

            // Show the dialog and process the results if the OK button was pressed.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string datasetNameFile;
                List<string> filenames = new List<string>();
                datasetNameFile = "";
                foreach (string filename in dialog.FileNames)
                {
                    if (Path.GetExtension(filename).ToLower() == ".nam")
                    {
                        datasetNameFile = filename;
                    }
                    else
                    {
                        filenames.Add(filename);
                    }
                }

                if (!string.IsNullOrEmpty(datasetNameFile))
                {
                    OpenDataset(datasetNameFile);
                }

                if (filenames.Count > 0)
                {
                    // If the user selected multiple files from the dialog, just
                    // add those files.
                    AddFiles(filenames.ToArray<string>());
                }
                else
                {
                    // If the user only selected a dataset name file and that 
                    // file was successfully opened, find all of the head and
                    // drawdown files in the name file directory that have the
                    // same row and column dimensions. Add those files to the
                    // data panel file list.

                    AddCompatibleFiles(_Dataset);

                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset"></param>
        private void AddCompatibleFiles(DatasetInfo dataset)
        {
            if (dataset != null)
            {
                if (dataset.Grid != null)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dataset.ParentFolderName);
                    FileInfo[] files = dirInfo.GetFiles();
                    List<string> headFileList = new List<string>();
                    if (files.Length > 0)
                    {
                        foreach (FileInfo file in files)
                        {
                            string pathname = file.FullName;
                            string lcPathname = pathname.ToLower();
                            bool skip = false;
                            if (dataset.BinaryHeadFile.ToLower() == lcPathname)
                            { skip = true; }
                            else if (dataset.BinaryDrawdownFile.ToLower() == lcPathname)
                            { skip = true; }
                            if (!skip)
                            {
                                headFileList.Add(pathname);
                            }
                        }
                        string[] headFiles = FindCompatibleHeadFiles(headFileList.ToArray<string>(), dataset.Grid.RowCount, dataset.Grid.ColumnCount);
                        AddFiles(headFiles);
                    }
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Array2d<float> GetAnalysisArray()
        {
            if (_ReferenceLayerDataRecord != null)
            {
                Array2d<float> buffer = null;
                List<float> excludeValuesCurrent = new List<float>();
                excludeValuesCurrent.Add(_CurrentData.HNoFlo);
                if (_CurrentData.HDry != _CurrentData.HNoFlo)
                { excludeValuesCurrent.Add(_CurrentData.HDry); }
                List<float> excludeValuesReference = new List<float>();
                excludeValuesReference.Add(_ReferenceData.HNoFlo);
                if (_ReferenceData.HDry != _ReferenceData.HNoFlo)
                { excludeValuesReference.Add(_ReferenceData.HDry); }
                buffer = _Analysis.CreateAnalysisArray(_CurrentLayerDataRecord.DataArray, _ReferenceLayerDataRecord.DataArray, excludeValuesCurrent, excludeValuesReference, _CurrentData.HNoFlo);
                return buffer;
            }
            else
            { return null; }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ColorRamp GetColorRamp(int index)
        {
            ColorRamp ramp = null;
            switch (index)
            {
                case 0:
                    ramp = ColorRamp.Rainbow7;
                    break;
                case 1:
                    ramp = ColorRamp.Rainbow5;
                    break;
                case 2:
                    ramp = ColorRamp.ThreeColors(Color.Green, Color.Orange, Color.Red);
                    break;
                case 3:
                    ramp = ColorRamp.ThreeColors(Color.Blue, Color.White, Color.Red);
                    break;
                default:
                    break;
            }
            return ramp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="excludedValues"></param>
        /// <returns></returns>
        private double[] CalculateMinimumAndMaximum(Array2d<float> values, float[] excludedValues)
        {
            IArrayUtility<float> util = new ArrayUtility();
            float minVal;
            float maxVal;

            if (excludedValues == null)
            { util.FindMinimumAndMaximum(values, out minVal, out maxVal); }
            else
            { util.FindMinimumAndMaximum(values, out minVal, out maxVal, excludedValues); }
            double minValue = (double)minVal;
            double maxValue = (double)maxVal;

            return new double[2] { minValue, maxValue };

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="excludedValues"></param>
        /// <returns></returns>
        private double[] CalculateMinimumAndMaximum(double[] values, double[] excludedValues)
        {
            IArrayUtility<double> util = new ArrayUtility();
            double minValue;
            double maxValue;

            if (excludedValues == null)
            { util.FindMinimumAndMaximum(values, out minValue, out maxValue); }
            else
            { util.FindMinimumAndMaximum(values, out minValue, out maxValue, excludedValues); }

            return new double[2] { minValue, maxValue };

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void FindContourLineHit(int x, int y)
        {
            _HotFeature = null;
            ICoordinate c = mapControl.ToMapPoint(x, y);
            IPoint pt = new USGS.Puma.NTS.Geometries.Point(c);
            double tol = (mapControl.ViewportExtent.Width / Convert.ToDouble(mapControl.ViewportSize.Width)) * 2.0;
            if (_CurrentContourMapLayer != null)
            {
                if (_CurrentContourMapLayer.Visible)
                {
                    for (int i = 0; i < _CurrentContourMapLayer.FeatureCount; i++)
                    {
                        Feature f = _CurrentContourMapLayer.GetFeature(i);
                        if (f.Geometry.IsWithinDistance(pt as IGeometry, tol))
                        {
                            _HotFeature = f;
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ShowMapTip(int x, int y)
        {
            if (_HotFeature != null)
            {
                string slevel = null;
                float value = Convert.ToSingle(_HotFeature.Attributes["Value"]);
                if (value >= 1.0f && value < 10000.0f)
                {
                    slevel = "Value = " + value.ToString("#.##");
                }
                else if (value <= -1.0f && value > -10000.0f)
                {
                    slevel = "Value = " + value.ToString("#.##");
                }
                else
                {
                    slevel = "Value = " + value.ToString("#.###E+00");
                }

                _MapTip.Show(slevel, mapControl, x + 12, y - 15, 2000);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ZoomToGrid()
        {
            if (_GridOutlineMapLayer != null)
            {
                IEnvelope rect = _GridOutlineMapLayer.Extent;
                mapControl.SetExtent(rect.MinX, rect.MaxX, rect.MinY, rect.MaxY);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void UpdateStatusBarLocationInfo(int x, int y)
        {
            ICoordinate coord = mapControl.ToMapPoint(x, y);
            string mouseLocation = "";
            string cellCoord = "";
            string dataCellValue = "";
            string analysisCellValue = "";
            string dataFilePath = "";

            if (mapControl.LayerCount > 0)
            { mouseLocation = coord.X.ToString("#.00") + ", " + coord.Y.ToString("#.00"); }

            if (_CurrentFileNode != null)
            {
                DataItemTag tag = _CurrentFileNode.Tag as DataItemTag;
                GridCell cell = _ModelGrid.FindRowColumn(coord);

                // Process the cell and contour information if the location is within the grid.
                if (cell != null)
                {
                    // Update status bar with current grid cell and contour data
                    cellCoord = "R" + cell.Row.ToString() + " C" + cell.Column.ToString();
                    dataCellValue = "Current data layer:  " + _CurrentLayerDataRecord.DataArray[cell.Row, cell.Column].ToString("#.######E+00");
                    if (cboGriddedValuesDisplayOption.SelectedIndex == 1)
                    {
                        analysisCellValue = "Reference layer:  " + _ReferenceLayerDataRecord.DataArray[cell.Row, cell.Column].ToString("#.######E+00");
                    }
                    else if (cboGriddedValuesDisplayOption.SelectedIndex == 2)
                    {
                        if (_AnalysisArray != null)
                        { analysisCellValue = "Analysis layer:  " + _AnalysisArray[cell.Row, cell.Column].ToString("#.######E+00"); }

                    }
                }
                dataFilePath = "";
            }

            statusStripMainLocation.Text = mouseLocation;
            statusStripMainCellCoord.Text = cellCoord;
            statusStripMainDataValue.Text = dataCellValue;
            statusStripMainAnalysisValue.Text = analysisCellValue;
            statusStripMainDataFilePath.Text = dataFilePath;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelGrid"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private FeatureLayer CreateModelGridOutlineLayer(CellCenteredArealGrid modelGrid, Color color)
        {
            Array2d<float> bottomElevation = _Dataset.DisData.GetBottom(_Dataset.DisData.LayerCount).GetDataArrayCopy(true);
            Array2d<float> topElevation = _Dataset.DisData.Top.GetDataArrayCopy(true);
            IMultiLineString outline = modelGrid.GetOutline(new GridCell(1, 1), new GridCell(modelGrid.RowCount, modelGrid.ColumnCount), topElevation, bottomElevation);
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
        /// <summary>
        /// 
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
            layer.LayerName = "Model gridlines";
            layer.Visible = false;

            return layer;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelGrid"></param>
        /// <returns></returns>
        private FeatureLayer CreateModelGridCellPolygonsLayer(CellCenteredArealGrid modelGrid)
        {
            if (modelGrid == null)
            { return null; }

            Dictionary<string, Array2d<float>> dict = new Dictionary<string, Array2d<float>>();
            dict.Add("Value", new Array2d<float>(modelGrid.RowCount, modelGrid.ColumnCount));
            Feature[] features = USGS.Puma.Utilities.GeometryFactory.CreateGridCellPolygons((ICellCenteredArealGrid)modelGrid, dict);
            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Polygon, features);
            return layer;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private FeatureLayer CreateValuesMapLayer()
        {
            FeatureLayer layer = null;
            if (_ModelGrid == null)
            { throw new Exception("No model grid is defined."); }

            layer = CreateModelGridCellPolygonsLayer(_ModelGrid);
            IFeatureRenderer renderer = CreateColorRampRenderer(SymbolType.FillSymbol, null, GetColorRamp(0), 255, 0, 0);
            (renderer as ColorRampRenderer).RenderField = "Values";
            layer.Renderer = renderer;

            return layer;

        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateCurrentValuesRendererColorRamp()
        {
            if (_CurrentValuesMapLayer == null)
                return;

            ColorRampRenderer renderer = (ColorRampRenderer)(_CurrentValuesMapLayer.Renderer);
            renderer.ColorRamp = GetColorRamp(_CurrentValuesRendererIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateReferenceValuesRendererColorRamp()
        {
            if (_ReferenceValuesMapLayer == null)
                return;

            ColorRampRenderer renderer = (ColorRampRenderer)(_ReferenceValuesMapLayer.Renderer);
            renderer.ColorRamp = GetColorRamp(_ReferenceValuesRendererIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateCurrentValuesRenderer()
        {
            if (_CurrentValuesMapLayer == null)
                return;

            // Update render array
            ColorRampRenderer renderer = (ColorRampRenderer)(_CurrentValuesMapLayer.Renderer);
            if (_CurrentLayerDataRecord == null)
            { renderer.RenderArray = null; }
            else
            { renderer.SetRenderArray(_CurrentLayerDataRecord.DataArray); }

            // Update minimum and maximum
            double[] minMaxValues = new double[2] { 0, 0 };
            double[] excludedValues = new double[2];
            excludedValues[0] = (double)_CurrentData.HNoFlo;
            excludedValues[1] = (double)_CurrentData.HDry;

            if (renderer.RenderArray != null)
            {
                minMaxValues = CalculateMinimumAndMaximum(renderer.RenderArray, excludedValues);
            }
            renderer.MinimumValue = minMaxValues[0];
            renderer.MaximumValue = minMaxValues[1];
            renderer.ExcludedValues.Add(excludedValues[0]);
            renderer.ExcludedValues.Add(excludedValues[1]);

            // Update color ramp
            renderer.ColorRamp = GetColorRamp(_CurrentValuesRendererIndex);

        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateReferenceValuesRenderer()
        {
            if (_ReferenceValuesMapLayer == null)
            { return; }
            if (_ReferenceLayerDataRecord == null)
            { return; }

            // Update render array
            ColorRampRenderer renderer = (ColorRampRenderer)(_ReferenceValuesMapLayer.Renderer);
            if (_CurrentLayerDataRecord == null)
            { renderer.RenderArray = null; }
            else
            { renderer.SetRenderArray(_ReferenceLayerDataRecord.DataArray); }

            // Update minimum and maximum
            double[] minMaxValues = new double[2] { 0, 0 };
            double[] excludedValues = new double[2];
            excludedValues[0] = (double)_ReferenceData.HNoFlo;
            excludedValues[1] = (double)_ReferenceData.HDry;

            if (renderer.RenderArray != null)
            {
                minMaxValues = CalculateMinimumAndMaximum(renderer.RenderArray, excludedValues);
            }
            renderer.MinimumValue = minMaxValues[0];
            renderer.MaximumValue = minMaxValues[1];
            renderer.ExcludedValues.Add(excludedValues[0]);
            renderer.ExcludedValues.Add(excludedValues[1]);

            // Update color ramp
            renderer.ColorRamp = GetColorRamp(_ReferenceValuesRendererIndex);

        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateAnalysisValuesRenderer()
        {
            if (_ReferenceLayerDataRecord != null)
            {
                if (_AnalysisValuesMapLayer != null)
                {
                    List<float> excludedValues = _ReferenceData.GetExcludedValues();
                    if (_Analysis.AnalysisType == LayerAnalysisType.DifferenceValues)
                    {
                        if (!excludedValues.Contains(_CurrentData.HNoFlo))
                        {
                            excludedValues.Add(_CurrentData.HNoFlo);
                        }
                        if (!excludedValues.Contains(_CurrentData.HDry))
                        {
                            excludedValues.Add(_CurrentData.HDry);
                        }
                    }
                    _Analysis.UpdateRenderer((ColorRampRenderer)(_AnalysisValuesMapLayer.Renderer), _AnalysisArray, excludedValues);
                }
            }
            SetAnalysisSummaryTextPanel();
            
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateAnalysisValuesRendererColorRamp()
        {
            if (_AnalysisValuesMapLayer != null)
            {
                List<float> excludedValues = _ReferenceData.GetExcludedValues();
                _Analysis.UpdateRenderer((ColorRampRenderer)(_AnalysisValuesMapLayer.Renderer), excludedValues);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolType"></param>
        /// <param name="renderField"></param>
        /// <param name="colorRamp"></param>
        /// <param name="alpha"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="useCenteredMaximum"></param>
        /// <returns></returns>
        private IFeatureRenderer CreateColorRampRenderer(SymbolType symbolType, string renderField, ColorRamp colorRamp, int alpha, double minValue, double maxValue, bool useCenteredMaximum)
        {
            ColorRamp ramp = null;
            if (colorRamp == null)
            { ramp = ColorRamp.Rainbow7; }
            else
            { ramp = colorRamp; }
            ISolidFillSymbol symbol = new SolidFillSymbol();
            symbol.Color = Color.FromArgb(alpha, Color.Black);
            symbol.EnableOutline = false;
            symbol.OneColorForFillAndOutline = true;
            ColorRampRenderer renderer = new ColorRampRenderer(symbolType, colorRamp);
            renderer.RenderField = renderField;
            renderer.BaseSymbol = symbol;
            if (useCenteredMaximum)
            {
                double centeredMax = ComputeCenteredMaximum(minValue, maxValue, 0);
                renderer.MinimumValue = -centeredMax;
                renderer.MaximumValue = centeredMax;
            }
            else
            {
                renderer.MinimumValue = minValue;
                renderer.MaximumValue = maxValue;
            }
            return renderer as IColorRampRenderer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolType"></param>
        /// <param name="renderArray"></param>
        /// <param name="colorRamp"></param>
        /// <param name="alpha"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        private IFeatureRenderer CreateColorRampRenderer(SymbolType symbolType, Array2d<float> renderArray, ColorRamp colorRamp, int alpha, double minValue, double maxValue)
        {
            ColorRamp ramp = null;
            if (colorRamp == null)
            { ramp = ColorRamp.Rainbow7; }
            else
            { ramp = colorRamp; }
            ISolidFillSymbol symbol = new SolidFillSymbol();
            symbol.Color = Color.FromArgb(alpha, Color.Black);
            symbol.EnableOutline = false;
            symbol.OneColorForFillAndOutline = true;
            ColorRampRenderer renderer = new ColorRampRenderer(symbolType, colorRamp);
            renderer.RenderField = "";
            renderer.BaseSymbol = symbol;
            renderer.MinimumValue = minValue;
            renderer.MaximumValue = maxValue;
            renderer.UseRenderArray = true;
            renderer.RenderArray = null;
            if (renderArray != null)
            { renderer.SetRenderArray(renderArray.GetValues()); }
            return renderer as IColorRampRenderer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="centerOnValue"></param>
        /// <returns></returns>
        private double ComputeCenteredMaximum(double minValue, double maxValue, double centerOnValue)
        {
            double minVal = minValue;
            double maxVal = maxValue;
            if (minValue > maxValue)
            {
                minVal = maxValue;
                maxVal = minValue;
            }
            if (minVal <= 0 && maxVal <= 0)
            { return Math.Abs(minVal); }
            else if (minVal >= 0 && maxVal >= 0)
            { return maxVal; }
            else
            {
                double a = Math.Abs(maxVal - centerOnValue);
                double b = Math.Abs(minVal - centerOnValue);
                if (a >= b)
                { return a; }
                else
                { return b; }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="buildMapLayers"></param>
        private void SetLayerAnalysisType(int index, bool buildMapLayers)
        {
            if (index == 0 || index == 1)
            {
                _Analysis = _AnalysisList[index];
                
                if (_CurrentLayerDataRecord != null && _ReferenceLayerDataRecord != null)
                {
                    _AnalysisArray = GetAnalysisArray();
                    UpdateAnalysisValuesRenderer();
                    if (buildMapLayers)
                    { BuildMapLayers(false); }
                }
                txtAnalysisDescription.Text = _Analysis.Description;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullExtent"></param>
        private void BuildMapLayers(bool fullExtent)
        {
            ColorRampRenderer renderer = null;

            bool forceFullExtent = fullExtent;
            if (mapControl.LayerCount == 0)
                forceFullExtent = true;

            mapControl.ClearLayers();

            // Add gridded value layers for all 3 modes. Only one of the 3 should
            // be loaded on the map legend an accessible any any given time.
            // Call SetGriddedValuesDisplayMode now to make sure all layer visibility
            // and accessibilities are set properly.
            SetGriddedValuesDisplayMode(_GriddedValuesDisplayMode, false);

            // Current data layer gridded values
            if (_CurrentValuesMapLayer != null)
            {
                if (_CurrentLayerDataRecord != null)
                {
                    renderer = (ColorRampRenderer)(_CurrentValuesMapLayer.Renderer);
                    if (renderer.RenderArray != null)
                    { mapControl.AddLayer(_CurrentValuesMapLayer); }
                }
            }
            // Reference data layer gridded values
            if (_ReferenceValuesMapLayer != null)
            {
                if (_ReferenceLayerDataRecord != null)
                {
                    renderer = (ColorRampRenderer)(_ReferenceValuesMapLayer.Renderer);
                    if (renderer.RenderArray != null)
                    { mapControl.AddLayer(_ReferenceValuesMapLayer); }
                }
            }
            //Analysis layer gridded values
            if (_AnalysisValuesMapLayer != null)
            {
                if (_ReferenceLayerDataRecord != null)
                {
                    renderer = (ColorRampRenderer)(_AnalysisValuesMapLayer.Renderer);
                    if (renderer.RenderArray != null)
                    { mapControl.AddLayer(_AnalysisValuesMapLayer); }
                }
            }

            //Basemap layers (add in reverse order)
            if (_BasemapLayers!=null)
            {
                if (_BasemapLayers.Count > 0)
                {
                    for (int i = _BasemapLayers.Count - 1; i > -1; i--)
                    {
                        mapControl.AddLayer(_BasemapLayers[i]);
                    }
                }
            }

            //Interior grid lines
            if (_GridlinesMapLayer != null)
            { mapControl.AddLayer(_GridlinesMapLayer); }

            //Grid outline
            if (_GridOutlineMapLayer != null)
            { mapControl.AddLayer(_GridOutlineMapLayer); }

            //Data contour layer
            if (_CurrentContourMapLayer != null)
            {
                mapControl.AddLayer(_CurrentContourMapLayer);
            }

            if (mapControl.LayerCount > 0)
            {
                if (forceFullExtent)
                {
                    if (_ModelGrid == null)
                    { mapControl.SizeToFullExtent(); }
                    else
                    { ZoomToGrid(); }
                }
            }
            BuildMapLegend();
            mapControl.Refresh();
            indexMapControl.UpdateMapImage();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="modelGrid"></param>
        /// <returns></returns>
        private ContourLineList GenerateContours(Array2d<float> buffer, CellCenteredArealGrid modelGrid)
        {
            if (buffer == null)
                throw new ArgumentNullException();
            if ((buffer.RowCount != modelGrid.RowCount) || (buffer.ColumnCount != modelGrid.ColumnCount))
                throw new ArgumentException("Array does not match model grid dimensions.");

            ContourEngine ce = new ContourEngine(modelGrid);
            ce.UseDefaultNoDataRange = false;
            ce.ExcludedValues.Add(_CurrentData.HNoFlo);
            if (_CurrentData.HDry != _CurrentData.HNoFlo)
            { ce.ExcludedValues.Add(_CurrentData.HDry); }
            ce.LayerArray = buffer;
            float refContour = _ContourEngineData.ReferenceContour;

            float conInterval;

            switch (_ContourEngineData.ContourIntervalOption)
            {
                case ContourIntervalOption.AutomaticConstantInterval:
                    conInterval = ce.ComputeContourInterval();
                    ce.ContourLevels = ce.GenerateConstantIntervals(conInterval, refContour);
                    break;
                case ContourIntervalOption.SpecifiedConstantInterval:
                    conInterval = _ContourEngineData.ConstantContourInterval;
                    ce.ContourLevels = ce.GenerateConstantIntervals(conInterval, refContour);
                    break;
                case ContourIntervalOption.SpecifiedContourLevels:
                    ce.ContourLevels = _ContourEngineData.ContourLevels;
                    break;
                default:
                    break;
            }

            ContourLineList conLineList = ce.CreateContours();
            return conLineList;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contourList"></param>
        /// <returns></returns>
        private FeatureLayer BuildContourLayer(ContourLineList contourList)
        {
            if (contourList == null)
                throw new ArgumentNullException("The specified contour list does not exist.");

            FeatureLayer contourLayer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(contourLayer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = Color.Black;
            symbol.Width = 2.0f;
            for (int i = 0; i < contourList.Count; i++)
            {
                IAttributesTable attributes = new AttributesTable();
                attributes.AddAttribute("Value", contourList[i].ContourLevel);
                contourLayer.AddFeature(contourList[i].Contour as IGeometry, attributes);
            }

            contourLayer.LayerName = "Current Data Contours";
            return contourLayer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="modelGrid"></param>
        private void GenerateAndBuildContourLayer(Array2d<float> buffer, CellCenteredArealGrid modelGrid)
        {
            ContourLineList contourList = GenerateContours(buffer, modelGrid);
            FeatureLayer contourLayer = BuildContourLayer(contourList);
            _CurrentContourMapLayer = contourLayer;
            _CurrentContourMapLayer.Visible = _ContourLayerPreferredVisible;
        }
        /// <summary>
        /// 
        /// </summary>
        private void EditContourProperties()
        {
            EditContouringOptionsDialog dialog = new EditContouringOptionsDialog(_ContourEngineData);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (_CurrentLayerDataRecord != null)
                {
                    ContourEngineData data = dialog.GetData();
                    _ContourEngineData.ContourIntervalOption = data.ContourIntervalOption;
                    _ContourEngineData.ConstantContourInterval = data.ConstantContourInterval;

                    DataItemTag tag = _CurrentFileNode.Tag as DataItemTag;
                    GenerateAndBuildContourLayer(_CurrentLayerDataRecord.DataArray, _ModelGrid);
                    BuildMapLayers(false);
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        private void SelectActiveTool(ActiveTool tool)
        {
            // Process the selection
            _ProcessingActiveToolButtonSelection = true;

            toolStripButtonSelect.Checked = false;
            menuMainMapPointer.Checked = false;
            toolStripButtonReCenter.Checked = false;
            menuMainMapReCenter.Checked = false;
            toolStripButtonZoomIn.Checked = false;
            menuMainMapZoomIn.Checked = false;
            toolStripButtonZoomOut.Checked = false;
            menuMainMapZoomOut.Checked = false;

            switch (tool)
            {
                case ActiveTool.Pointer:
                    _ActiveTool = tool;
                    mapControl.Cursor = System.Windows.Forms.Cursors.Default;
                    toolStripButtonSelect.Checked = true;
                    menuMainMapPointer.Checked = true;
                    break;
                case ActiveTool.ReCenter:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ReCenterCursor;
                    toolStripButtonReCenter.Checked = true;
                    menuMainMapReCenter.Checked = true;
                    break;
                case ActiveTool.ZoomIn:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ZoomInCursor;
                    toolStripButtonZoomIn.Checked = true;
                    menuMainMapZoomIn.Checked = true;
                    break;
                case ActiveTool.ZoomOut:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ZoomOutCursor;
                    toolStripButtonZoomOut.Checked = true;
                    menuMainMapZoomOut.Checked = true;
                    break;
                default:
                    throw new ArgumentException();
            }

            _ProcessingActiveToolButtonSelection = false;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private MapControl CreateAndInitializeMapControl()
        {
            MapControl c = new MapControl();
            c.BackColor = System.Drawing.Color.White;
            c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            c.MapBackgroundColor = System.Drawing.Color.White;
            c.TabIndex = 0;

            // Connect the MouseDoubleClick event handler
            c.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mapControl_MouseDoubleClick);
            // Connect the MouseClick event handler
            c.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapControl_MouseClick);
            // Connect the MouseMove event handler
            c.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapControl_MouseMove);

            return c;
        }
        /// <summary>
        /// 
        /// </summary>
        private void BuildCurrentFileSummary()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Data file:  ");
            if (_CurrentFileNode == null)
            { sb.Append("<none selected>"); }
            else
            {
                DataItemTag tag = (DataItemTag)_CurrentFileNode.Tag;
                sb.Append(tag.Label);
                sb.Append("     Path = ").Append(tag.Pathname);
            }

            //txtDataset.Text = sb.ToString();

        }
        /// <summary>
        /// 
        /// </summary>
        private void SetAnalysisSummaryTextPanel()
        {
            lblReferenceFile.Text = "None selected";
            lblReferenceLayer.Text = "None selected";
            lblTimeStepLinkOption.Text = "";
            lblModelLayerLinkOption.Text = "";

            if (_CurrentFileNode == null)
            { return; }

            StringBuilder sb = new StringBuilder();
            if (_AnalysisValuesMapLayer != null)
            {
                ColorRampRenderer renderer = (ColorRampRenderer)(_AnalysisValuesMapLayer.Renderer);
                if (renderer.RenderArray == null)
                {
                    lblReferenceLayer.Text = "Reference layer cannot be displayed.";
                }
            }
            if (_ReferenceFileNode != null)
            {
                lblReferenceFile.Text = _ReferenceFileNode.Text;

                switch (_ReferenceData.TimeStepLinkOption)
                {
                    case ReferenceDataTimeStepLinkOption.CurrentTimeStep:
                        lblTimeStepLinkOption.Text = "Time linked to current time step";
                        break;
                    case ReferenceDataTimeStepLinkOption.PreviousTimeStep:
                        lblTimeStepLinkOption.Text = "Time linked to previous time step";
                        break;
                    case ReferenceDataTimeStepLinkOption.NextTimeStep:
                        lblTimeStepLinkOption.Text = "Time linked to next time step";
                        break;
                    case ReferenceDataTimeStepLinkOption.SpecifyTimeStep:
                        lblTimeStepLinkOption.Text = "Specified stress period and time step";
                        break;
                    default:
                        break;
                }

                switch (_ReferenceData.ModelLayerLinkOption)
                {
                    case ReferenceDataModelLayerLinkOption.CurrentModelLayer:
                        lblModelLayerLinkOption.Text = "Layer linked to current model layer";
                        break;
                    case ReferenceDataModelLayerLinkOption.ModelLayerBelow:
                        lblModelLayerLinkOption.Text = "Layer linked to model layer below";
                        break;
                    case ReferenceDataModelLayerLinkOption.ModelLayerAbove:
                        lblModelLayerLinkOption.Text = "Layer linked to model layer above";
                        break;
                    case ReferenceDataModelLayerLinkOption.SpecifyModelLayer:
                        lblModelLayerLinkOption.Text = "Specified model layer";
                        break;
                    default:
                        break;
                }

                if (_ReferenceLayerDataRecord != null)
                {
                    sb.Append("Period ").Append(_ReferenceLayerDataRecord.StressPeriod.ToString()).Append(",  ");
                    sb.Append("Step ").Append(_ReferenceLayerDataRecord.TimeStep.ToString()).Append(",  ");
                    sb.Append("Model layer ").Append(_ReferenceLayerDataRecord.Layer.ToString());
                    lblReferenceLayer.Text = sb.ToString();
                }
                else
                {
                    lblReferenceLayer.Text = "Specified layer does not exist";
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="refreshMap"></param>
        private void SetGriddedValuesDisplayMode(int mode, bool refreshMap)
        {
            bool currentVisible = false;
            bool referenceVisible = false;
            bool analysisVisible = false;
            if (mode == 0)
            {
                currentVisible = _GriddedValuesPreferredVisible;
            }
            else if (mode == 1)
            {
                referenceVisible = _GriddedValuesPreferredVisible;
            }
            else if (mode == 2)
            {
                analysisVisible = _GriddedValuesPreferredVisible;
            }
            else
            {
                throw new ArgumentException("Invalid GriddedValuesDisplayMode.");
            }

            _GriddedValuesDisplayMode = mode;

            if (_CurrentValuesMapLayer != null)
            { _CurrentValuesMapLayer.Visible = currentVisible; }
            if (_ReferenceValuesMapLayer != null)
            { _ReferenceValuesMapLayer.Visible = referenceVisible; }
            if (_AnalysisValuesMapLayer != null)
            { _AnalysisValuesMapLayer.Visible = analysisVisible; }

            if (refreshMap)
            {
                BuildMapLegend();
                mapControl.Refresh();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftPanel"></param>
        /// <param name="rightPanel"></param>
        private void SetLeftPanel(bool collapsed)
        {
            _LeftPanelCollapsed = collapsed;
            splitConMain.Panel1Collapsed = _LeftPanelCollapsed;
            if (_LeftPanelCollapsed)
            {
                buttonToggleLeftPanel.Text = ">";
            }
            else
            {
                buttonToggleLeftPanel.Text = "<";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftPanel"></param>
        /// <param name="rightPanel"></param>
        private void SetRightPanel(bool collapsed)
        {
            _RightPanelCollapsed = collapsed;
            splitConMap.Panel2Collapsed = _RightPanelCollapsed;
            if (_RightPanelCollapsed)
            {
                buttonToggleRightPanel.Text = "<";
            }
            else
            {
                buttonToggleRightPanel.Text = ">";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collapsed"></param>
        private void SetSidePanels(bool collapsed)
        {
            SetLeftPanel(collapsed);
            SetRightPanel(collapsed);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="showPrintPreview"></param>
        private void PrintMap(bool showPrintPreview)
        {
            // select the printer
            System.Windows.Forms.PrintDialog printDialog = new PrintDialog();
            // set the print dialog to the current application printer and printer settings
            // if one exists.
            if (_PrinterSettings != null)
            { printDialog.PrinterSettings = _PrinterSettings; }

            // display print dialog. If the dialog returns OK then save the printer settings,
            // construct the print document, and display the print preview dialog.
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                _PrinterSettings = printDialog.PrinterSettings;
                MapIO mapIO = new MapIO();
                mapIO.Map = mapControl;
                mapIO.Print(_PrinterSettings, this, showPrintPreview);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void PrintPDF()
        {
            string directoryName = null;
            PrintPdfDialog printPdfDialog = new PrintPdfDialog();
            if (_Dataset != null)
            {
                directoryName = _Dataset.ParentFolderName;
            }
            else if (_CurrentData.FileReader != null)
            {
                directoryName = System.IO.Path.GetDirectoryName(_CurrentData.FileReader.Filename);
                printPdfDialog.Filename = System.IO.Path.GetDirectoryName(_CurrentData.FileReader.Filename) + @"\MapOutput.pdf";
            }
            else
            { directoryName = @"C:"; }

            printPdfDialog.Filename = directoryName + @"\MapOutput.pdf";

            if (printPdfDialog.ShowDialog() == DialogResult.OK)
            {
                // Initialize page settings
                System.Windows.Forms.PageSetupDialog pageSetupDialog = new PageSetupDialog();
                if (_PdfPageSettings == null)
                { _PdfPageSettings = new System.Drawing.Printing.PageSettings(); }

                // Set pageSetupDialog page settings
                pageSetupDialog.PageSettings = _PdfPageSettings;

                if (pageSetupDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string heading = "";
                        if (_CurrentFileNode != null)
                        {
                            DataItemTag tag = _CurrentFileNode.Tag as DataItemTag;
                            heading = tag.Pathname;
                        }
                        _PdfPageSettings = pageSetupDialog.PageSettings;
                        MapIO mapIO = new MapIO();
                        mapIO.Map = mapControl;
                        mapIO.ExportPDF(printPdfDialog.Filename, _PdfPageSettings, heading, printPdfDialog.Title, printPdfDialog.Description, false, 10);
                    }
                    catch (Exception ex)
                    {
                        if (ex is ArgumentException)
                        {
                            ArgumentException argEx = ex as ArgumentException;
                            MessageBox.Show(argEx.Message);
                        }
                        else
                        { MessageBox.Show(ex.Message); }

                        return;
                    }

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void EditMetadata()
        {
            if (_Dataset != null)
            {
                if (_Dataset.Metadata != null)
                {
                    ModflowMetadaEditDialog dialog = new ModflowMetadaEditDialog();
                    dialog.Metadata = _Dataset.Metadata;
                    if (_Basemap != null)
                    {
                        dialog.CurrentBasemapFile = Path.Combine(_Basemap.BasemapDirectory, _Basemap.Filename);
                    }
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        // Save the metadata
                        ModflowMetadata.Write(_Dataset.Metadata.Filename, _Dataset.Metadata);

                        // Remove the basemap if one is loaded
                        if (_Basemap != null)
                        {
                            _Basemap = null;
                            _BasemapLayers.Clear();
                        }

                        // Re-load the current dataset
                        string filename = _Dataset.DatasetBaseName + ".nam";
                        filename = Path.Combine(_Dataset.ParentFolderName, filename);
                        OpenDataset(filename);

                    }
                }
            }
        }

        
        #endregion















    }
}
