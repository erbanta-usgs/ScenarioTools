using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using GeoAPI.Geometries;

using OSGeo.GDAL;

using ScenarioTools;
using ScenarioTools.DataClasses;
using ScenarioTools.Dialogs;
using ScenarioTools.Geometry;
using ScenarioTools.Gui;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Reporting;
using ScenarioTools.Scene;
using ScenarioTools.SMDataClasses;
using ScenarioTools.Spatial;
using ScenarioTools.UndoItems;
using ScenarioTools.Util;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioManager
{
    public partial class SMMainForm : Form
    {
        int _numCores = 2;  // Define based on computer hardware (= # processor cores)

        #region Fields
        private TreeNode _copiedNodeClone;
        private ITaggable _taggableCutItemClone = null;
        private int _copiedNodeCloneLevel;
        private int _copiedLevel0Index, _copiedLevel1Index, _copiedLevel2Index;
        private int _maxUndoCount = 0;
        private int _pasteLevel = -1;
        private int _runsDone;
        private int _numScenarios;
        private int _progressInc;
        private int _cbcFlagTemp;
        private bool _movingNode = false;
        private bool _draggingNode = false;
        private bool _freeFormat = false;
        private bool _okToClose = true;
        private bool _refreshingSummary = false;
        private bool _exportingPackage = false;
        private bool _exportingScenario = false;
        private bool _runningAllScenarios = false;
        private bool _processingUndoItem = false;
        private string _oldNodeText = "";
        private string _projectFileName = "";
        private string _projectFileDirectory = "";
        private string _backgroundCompletionMessage = "";
        private StringHolder _gridShapefileAbsolutePath;
        private StringHolder _discretizationFileAbsolutePath;
        private StringHolder _namefileAbsolutePath;
        private readonly TagStore _tagStore = new TagStore();
        private readonly List<ITaggable> _scenarioElements = new List<ITaggable>();
        private USGS.Puma.FiniteDifference.CellCenteredArealGrid _modelGrid = null;
        private DiscretizationFile _discretizationFile = null;
        //private DateTime _simulationStartTime;
        private DateTime _simulationEndTime;
        private SMProjectSettings _smProjectSettings;
        private NamefileInfo _namefileInfo;
        private Cursor _savedCursor;
        private List<WorkerWrapper> _workerWrappers;
        private ImageLayer _imageLayerBackground;
        //
        // Forms
        private DateTimeDialog _dateTimeDialog;
        private FeatureSetForm _featureSetForm;
        private ScenarioPropertiesDialog scenarioPropertiesDialog;
        private ModelGridForm _modelGridForm;
        private ProjectSettingsDialog _projectSettingsDialog;
        private ScenarioIdDialog _scenarioIdDialog;
        private EditNameDialog _editNameDialog;
        // PSB (Partition Stress Boundaries capability) allows a MODFLOW name file to
        // contain multiple entries for stress-boundary packages.  PSB is documented in:
        // Banta, E.R., 2011, MODFLOW-CDSS, a version of MODFLOW-2005 with modifications 
        // for Colorado Decision Support Systems: U.S. Geological Survey Open-File 
        // Report 2011-1213, 19 p., available at http://pubs.usgs.gov/of/2011/1213/.
        // _supportPsb controls whether Scenario Manager allows multiple packages of
        // a given type to be generated.
        private const bool _supportPsb = false;
        private readonly UndoChain _undoChain = new UndoChain();
        #endregion Fields

        #region Properties
        private string DiscretizationFileAbsolutePath
        {
            get
            {
                return _discretizationFileAbsolutePath.String;
            }
            set
            {
                _discretizationFileAbsolutePath.String = value;
            }
        }
        private string GridShapefileAbsolutePath
        {
            get
            {
                return _gridShapefileAbsolutePath.String;
            }
            set
            {
                _gridShapefileAbsolutePath.String = value;
            }
        }
        private DateTime SimulationStartTime
        {
            get
            {
                return GlobalStaticVariables.GlobalTemporalReference.SimulationStartTime;
            }
            set
            {
                GlobalStaticVariables.GlobalTemporalReference.SimulationStartTime = value;
                assignSimulationEndTime();
            }
        }
        public SMProjectSettings SmProjectSettings
        {
            get
            {
                return _smProjectSettings;
            }
            private set
            {
                _smProjectSettings = value;
                if (_smProjectSettings.MasterNameFile != null)
                {
                    string masterDirectory = getNameFileDirectory();
                    string namefilePath = FileUtil.Relative2Absolute(_smProjectSettings.MasterNameFile, masterDirectory);
                    if (File.Exists(namefilePath))
                    {
                        readNameFile();
                    }
                }
            }
        }
        #endregion Properties

        public SMMainForm()
        {
            InitializeComponent();
            GlobalStaticVariables.Initialize();
            StaticObjects.Initialize();
            initializeFields();

            // Setup the undo chain events so we can select the Control 
            // containing the detail that was modified by an undo/redo operation,
            // or update the contacts list if we need to.
            // I think these two lines never do anything.
            _undoChain.Undone += new UndoActionEventHandler(UndoChain_Acted);
            _undoChain.Redone += new UndoActionEventHandler(UndoChain_Acted);

            treeView1.ImageIndex = 0;
            initializeUI();
        }

        private void initializeFields()
        {
            _gridShapefileAbsolutePath = new StringHolder();
            _discretizationFileAbsolutePath = new StringHolder();
            _namefileAbsolutePath = new StringHolder();
            _namefileInfo = null;
            SimulationStartTime = defaultSimulationTime();
            _simulationEndTime = defaultSimulationTime();
            _smProjectSettings = new SMProjectSettings();
            getUserSettings();
            _dateTimeDialog = new DateTimeDialog();
            _scenarioIdDialog = new ScenarioIdDialog();
            _editNameDialog = new EditNameDialog();
        }

        #region Form and other event handlers

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_exportingPackage || _exportingScenario)
            {
                CloseReason cr = e.CloseReason;
                if (cr == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                if (!_okToClose)
                {
                    exit();
                    CloseReason cr = e.CloseReason;
                    if (cr == CloseReason.UserClosing)
                    {
                        e.Cancel = true;
                    }
                }
            }
            saveUserSettings();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _modelGridForm = new ModelGridForm();
            scenarioPropertiesDialog = new ScenarioPropertiesDialog();
            _imageLayerBackground = new ImageLayer();
            _workerWrappers = new List<WorkerWrapper>();
            _numCores = SystemUtil.NumberProcessorCores();
            for (int i = 0; i < _numCores; i++)
            {
                _workerWrappers.Add(new WorkerWrapper());
            }
            enablePaste(false);
            initializeUI();
        }

        private int getNewWorkerWrappersIndex()
        {
            // If possible, make a new BackgroundWorker 
            // and return its index in _workerWrappers list.
            //
            // First, look for a null BackgroundWorker
            for (int i = 0; i < SmProjectSettings.MaxSimultaneousRuns; i++)
            {
                // The DoEvents call is required to enable 
                // BackgroundWorker(s) to define IsBusy as false.
                Application.DoEvents();
                // When a project file created on one computer is opened
                // on another computer with fewer processor cores, 
                // MaxSimultaneousRuns may be an out-of-range index
                // for the _workerWrappers list.
                if (i < _workerWrappers.Count)
                {
                    if (_workerWrappers[i].Available())
                    {
                        _workerWrappers[i].InstantiateWorker();
                        setUpBackgroundWorker(_workerWrappers[i].Worker);
                        return i;
                    }
                }
            }

            // No BackgroundWorker is available.
            return -1;
        }

        private void statusStrip_Click(object sender, EventArgs e)
        {
            if (!_exportingPackage && !_exportingScenario)
            {
                StatusLabel.Text = "";
            }
        }

        private void tbDescription_TextChanged(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if ((treeView1.SelectedNode.Tag != null) && (!_refreshingSummary)
                    && (!_movingNode) && (!_draggingNode))
                {
                    IDescribable element = GetScenarioElementByTag((int)treeView1.SelectedNode.Tag);
                    if (element != null)
                    {
                        ITaggable oldElement = null;
                        if (!_processingUndoItem)
                        {
                            if (element is ITaggable)
                            {
                                oldElement = (ITaggable)((ITaggable)element).Clone();
                            }
                        }
                        element.Description = tbDescription.Text;
                        if (!_processingUndoItem)
                        {
                            if (element is ITaggable)
                            {
                                ITaggable newElement = (ITaggable)((ITaggable)element).Clone();
                                addScenarioElementChangeUndoItem(oldElement, newElement);
                            }
                        }
                    }
                }
            }
        }

        #endregion Form and other event handlers

        #region Main menu event handlers

        private void contextMenuEdit_Click(object sender, EventArgs e)
        {
            editSelectedItem();
        }

        private void editSelectedItem()
        {
            int level = treeView1.SelectedNode.Level;
            switch (level)
            {
                case 0:
                    // Selected node is a Scenario
                    break;
                case 1:
                    // Selected node is a Package
                    break;
                case 2:
                    // Selected node is a FeatureSet
                    int tag = (int)treeView1.SelectedNode.Tag;
                    int parentTag = (int)treeView1.SelectedNode.Parent.Tag;
                    string text = treeView1.SelectedNode.Text;
                    int index = treeView1.SelectedNode.Index;
                    text = editFeatureSetAndReturnName(tag, parentTag, text, index);
                    treeView1.SelectedNode.Text = text;
                    break;
                default:
                    break;
            }
            enablePaste(false);
            refreshUI();
        }

        private void menuItemEditAddScenario_Click(object sender, EventArgs e)
        {
            addScenario();
        }

        private void menuItemEditRedo_Click(object sender, EventArgs e)
        {
            _processingUndoItem = true;
            _undoChain.Redo();
            menuItemEditUndo.Enabled = true;
            if (_undoChain.Count == _maxUndoCount)
            {
                menuItemEditRedo.Enabled = false;
            }
            else
            {
                menuItemEditRedo.Enabled = true;
            }
            refreshUI();
            _processingUndoItem = false;
        }

        private void menuItemEditUndo_Click(object sender, EventArgs e)
        {
            _processingUndoItem = true;
            _undoChain.Undo();
            menuItemEditRedo.Enabled = true;
            if (_undoChain.Count == 0)
            {
                menuItemEditUndo.Enabled = false;
            }
            else
            {
                menuItemEditUndo.Enabled = true;
            }
            refreshUI();
            _processingUndoItem = false;
        }

        private void menuItemExport_Click(object sender, EventArgs e)
        {
        }

        private void menuItemExportPackage_Click(object sender, EventArgs e)
        {
            if (defineModelGrid(false))
            {
                UseWaitCursor = true;
                enableControls(false);
                progressBar.Value = 0;
                progressBar.Visible = true;
                Refresh();
                int tag = (int)treeView1.SelectedNode.Tag;
                Package package = (Package)GetScenarioElementByTag(tag);
                package.NameFileDirectory = getNameFileDirectory();
                int bgwIndex = getNewWorkerWrappersIndex();
                if (bgwIndex >= 0)
                {
                    StatusLabel.Text = "Exporting Package...";
                    _workerWrappers[bgwIndex].RunWorkerAsync(package);
                }
            }
        }

        private void menuItemExportScenario_Click(object sender, EventArgs e)
        {
            try
            {
                int tag = (int)treeView1.SelectedNode.Tag;
                if (defineModelGrid(false))
                {
                    UseWaitCursor = true;
                    enableControls(false);
                    progressBar.Visible = true;
                    progressBar.Value = 0;
                    Refresh();
                    Scenario scenario = (Scenario)GetScenarioElementByTag(tag);
                    scenario.NameFileInfo = _namefileInfo;
                    scenario.Message = "";
                    scenario.NameFileDirectory = getNameFileDirectory();
                    int bgwIndex = getNewWorkerWrappersIndex();
                    if (bgwIndex >= 0)
                    {
                        StatusLabel.Text = "Exporting Scenario...";
                        _workerWrappers[bgwIndex].RunWorkerAsync(scenario);
                    }
                }
            }
            catch
            {
            }
        }

        private void menuItemExportScenarioAndRunSimulation_Click(object sender, EventArgs e)
        {
            if (defineModelGrid(false))
            {
                if (okToExport())
                {
                    if (treeView1.SelectedNode != null)
                    {
                        if (treeView1.SelectedNode.Tag != null)
                        {
                            int tag = (int)treeView1.SelectedNode.Tag;
                            Scenario scenario = (Scenario)GetScenarioElementByTag(tag);
                            exportAndRunScenario(scenario);
                        }
                    }
                }
            }
        }

        private bool exportAndRunScenario(Scenario scenario)
        {            
            UseWaitCursor = true;
            enableControls(false);
            progressBar.Visible = true;
            scenario.NameFileInfo = _namefileInfo;
            if (!_runningAllScenarios)
            {
                progressBar.Value = 0;
            }
            Application.DoEvents();
            scenario.Message = "Run";
            scenario.NameFileDirectory = getNameFileDirectory();
            int bgwIndex = getNewWorkerWrappersIndex();
            if (bgwIndex >= 0)
            {
                StatusLabel.Text = "Exporting Scenario...";
                _workerWrappers[bgwIndex].RunWorkerAsync(scenario);
                return true;
            }
            return false;
        }

        private void menuItemFileExit_Click(object sender, EventArgs e)
        {
            exit();
        }

        private void menuItemFileNew_Click(object sender, EventArgs e)
        {
            if (_undoChain.Dirty)
            {
                string msg = "Current project has unsaved changes.  Save changes?";
                DialogResult dr = MessageBox.Show(msg, "Scenario Manager", MessageBoxButtons.YesNoCancel);
                switch (dr)
                {
                    case DialogResult.Yes:
                        saveProject();
                        break;
                    case DialogResult.No:
                        _undoChain.Clear();
                        break;
                    case DialogResult.Cancel:
                        break;
                }
            }
            if (!_undoChain.Dirty)
            {
                initializeFields();
                initializeUI();
            }
        }

        private void menuItemFileOpen_Click(object sender, EventArgs e)
        {
            openProjectFile();
        }

        private void menuItemFileSave_Click(object sender, EventArgs e)
        {
            saveProject();
        }

        private void menuItemFileSaveAs_Click(object sender, EventArgs e)
        {
            saveProjectAs();
        }

        private void menuItemProjectModelGrid_Click(object sender, EventArgs e)
        {
            defineModelGrid(true);
        }

        private void menuItemProjectSettings_Click(object sender, EventArgs e)
        {
            editProjectSettings();
        }

        private void menuItemProjectSimulationStartTime_Click(object sender, EventArgs e)
        {
            _dateTimeDialog.Text = "Simulation Start Date and Time";
            _dateTimeDialog.DateTime = SimulationStartTime;
            DateTime origSimulationStartTime = new DateTime(SimulationStartTime.Ticks);
            TemporalReference origTemporalReference = new TemporalReference(origSimulationStartTime);
            DialogResult dr = _dateTimeDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                DateTime newSimulationStartTime = _dateTimeDialog.DateTime;
                if (!newSimulationStartTime.Equals(origSimulationStartTime))
                {
                    TemporalReference newTemporalReference = new TemporalReference(newSimulationStartTime);
                    addGlobalTemporalReferenceChangeUndoItem(origTemporalReference, newTemporalReference);
                }
                SimulationStartTime = _dateTimeDialog.DateTime;
                assignSimulationEndTime();
                StatusLabel.Text = "Simulation start time assigned.";
            }
            refreshUI(false);
        }

        private void menuItemViewRefresh_Click(object sender, EventArgs e)
        {
            refreshUI();
        }

        #endregion Main menu event handlers

        #region Context menu event handlers

        private void contextMenuAddChdPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.ChdType))
            {
                addPackageTreeItem(PackageType.ChdType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void contextMenuAddRiverPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.RiverType))
            {
                addPackageTreeItem(PackageType.RiverType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void contextMenuAddWellPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.WellType))
            {
                addPackageTreeItem(PackageType.WellType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void addNewChdSetItem_Click(object sender, EventArgs e)
        {
            addFeatureSetTreeItem(PackageType.ChdType);
        }

        private void addNewWellSetItem_Click(object sender, EventArgs e)
        {
            addFeatureSetTreeItem(PackageType.WellType);
        }

        private void addNewRiverSetItem_Click(object sender, EventArgs e)
        {
            addFeatureSetTreeItem(PackageType.RiverType);
        }

        private void contextMenuRename_Click(object sender, EventArgs e)
        {
            renameTreeItem();
        }

        private void renameTreeItem()
        {
            string errMsg = "";
            bool OK = true;
            TreeNode node = treeView1.SelectedNode;
            _editNameDialog.Text = node.Text;
            DialogResult dr = _editNameDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                if (node.Level == 0)
                {
                    if (!isValidScenarioID(_editNameDialog.Text, ref errMsg))
                    {
                        if (_editNameDialog.Text != node.Text)
                        {
                            MessageBox.Show(errMsg, "Error In Assigning Scenario ID");
                        }
                        OK = false;
                    }
                }
                if (OK)
                {
                    addTreeNodeRenameUndoItem(_editNameDialog.Text);
                    node.Text = _editNameDialog.Text;
                    switch (node.Level)
                    {
                        case 0:
                            Scenario scenario = (Scenario)GetScenarioElementByTag((int)node.Tag);
                            scenario.Name = _editNameDialog.Text;
                            break;
                        case 1:
                            Package package = (Package)GetScenarioElementByTag((int)node.Tag);
                            package.Name = _editNameDialog.Text;
                            break;
                        case 2:
                            FeatureSet featureSet = (FeatureSet)GetScenarioElementByTag((int)node.Tag);
                            featureSet.Name = _editNameDialog.Text;
                            break;
                        default:
                            break;
                    }
                }
            }
            enablePaste(false);
            refreshUI();
        }

        private void menuItemMouseDownAddScenario_Click(object sender, EventArgs e)
        {
            addScenario();
        }

        #endregion Context menu event handlers

        #region TreeView event handlers

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            // KeyDown is for a <CTRL>-key combination
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        e.SuppressKeyPress = true;
                        copyTreeItem();
                        break;
                    case Keys.V:
                        e.SuppressKeyPress = true;
                        pasteTreeItem(true);
                        break;
                    case Keys.X:
                        e.SuppressKeyPress = true;
                        cutTreeItem();
                        break;
                }
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    deleteTreeItem();
                }
            }
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            treeView1.SelectedNode = (TreeNode)e.Item;
            copyNode(treeView1.SelectedNode);
            _movingNode = true;
            _draggingNode = true;
            copyTreeItem();
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            StatusLabel.Text = "";
            // Move selected node
            if (_draggingNode)
            {
                moveNodeToNodeAt(e.X, e.Y);
            }
            else
            {
                if (!contextMenuStripNode.Visible)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        Point p = e.Location;
                        contextMenuMouseDown.Show(treeView1, p);
                    }
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            StatusLabel.Text = "";
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
                if (_movingNode)
                {
                    // Enable Paste
                    contextMenuPaste.Enabled = true;
                }

                // Pop up menu with Cut, Copy, Paste and other options
                Point screenLocation = new Point();
                screenLocation.X = Left + e.X + 5;
                screenLocation.Y = Top + e.Y + 50;
                contextMenuStripNode.Show(screenLocation);
            }
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            _oldNodeText = e.Node.Text;
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string errMsg = "";
            TreeNode node = e.Node;
            string newNodeText = e.Label;
            if (newNodeText != _oldNodeText)
            {
                if (isValidScenarioID(newNodeText, ref errMsg))
                {
                    addTreeNodeRenameUndoItem(newNodeText);
                    GetScenarioElementByTag((int)node.Tag).Name = newNodeText;
                }
                else
                {
                    MessageBox.Show(errMsg, "Error In Assigning Scenario ID");
                    GetScenarioElementByTag((int)node.Tag).Name = _oldNodeText;
                    node.Text = _oldNodeText;
                    node.Name = _oldNodeText;
                }
            }
            refreshUI();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tbDescription.Enabled = true;
            refreshSummary();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            StatusLabel.Text = "";
            string text;
            switch (e.Node.Level)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    int tag = (int)e.Node.Tag;
                    int parentTag = (int)e.Node.Parent.Tag;
                    text = e.Node.Text;
                    int index = e.Node.Index;
                    text = editFeatureSetAndReturnName(tag, parentTag, text, index);
                    e.Node.Text = text;
                    refreshUI();
                    break;
                default:
                    break;
            }
        }

        #endregion TreeView event handlers

        #region Undo/Redo methods

        private void addScenarioCopyUndoItem(int newScenarioIndex, int newLinkIndex)
        {
            _undoChain.AddUndoItem(new ScenarioCopyUndoItem(treeView1, _scenarioElements, 
                                   _tagStore.Items, newScenarioIndex, newLinkIndex));
            refreshUndoMenu();
        }

        private void addDescriptionChangeUndoItem(string oldDescription, string newDescription)
        {
            _undoChain.AddUndoItem(new DescriptionChangeUndoItem(treeView1, tbDescription, 
                                   oldDescription, newDescription));
            refreshUndoMenu();
        }

        private void addCbcFlagChangeUndoItem(Package package, int oldCbcFlag, int newCbcFlag)
        {
            _undoChain.AddUndoItem(new CbcFlagChangeUndoItem(package, oldCbcFlag, newCbcFlag));
            refreshUndoMenu();
        }

        private void addScenarioElementChangeUndoItem(ITaggable oldScenarioElement, ITaggable newScenarioElement)
        {
            _undoChain.AddUndoItem(new ScenarioElementChangeUndoItem(oldScenarioElement, newScenarioElement, 
                                                                     treeView1, _scenarioElements));
            refreshUndoMenu();
        }

        private void addScenarioManagerTreeChangeUndoItem(TreeView treeViewOld, TreeView treeViewNew, 
            List<ITaggable> scenarioElementsOld, List<ITaggable> scenarioElementsNew)
        {
            _undoChain.AddUndoItem(new ScenarioManagerTreeChangeUndoItem(treeView1, treeViewOld, treeViewNew, 
                _scenarioElements, scenarioElementsOld, scenarioElementsNew));
            refreshUndoMenu();
        }

        private void addModelGridChangeUndoItem(string oldModelGridShapefileAbsolutePath, 
                                                string newModelGridShapefileAbsolutePath)
        {
            _undoChain.AddUndoItem(new ModelGridChangeUndoItem(oldModelGridShapefileAbsolutePath, 
                                                               newModelGridShapefileAbsolutePath, 
                                                               _gridShapefileAbsolutePath));
            refreshUndoMenu();
        }

        private void addDiscretizationFileChangeUndoItem(string oldDiscretizationFileAbsolutePath, 
                                                         string newDiscretizationFileAbsolutePath)
        {
            _undoChain.AddUndoItem(new DiscretizationFileChangeUndoItem(oldDiscretizationFileAbsolutePath, 
                                                                        newDiscretizationFileAbsolutePath, 
                                                                        _discretizationFileAbsolutePath));
            refreshUndoMenu();
        }

        private void addNamefileChangeUndoItem(string oldNamefileAbsolutePath, 
                                               string newNameFileAbsolutePath)
        {
            _undoChain.AddUndoItem(new NamefileChangeUndoItem(oldNamefileAbsolutePath, 
                                                              newNameFileAbsolutePath,
                                                              _namefileAbsolutePath));
            refreshUndoMenu();
        }

        private void addProjectSettingsChangeUndoItem(SMProjectSettings oldProjectSettings, 
                                                      SMProjectSettings newProjectSettings, 
                                                      SMProjectSettings projectSettings)
        {
            _undoChain.AddUndoItem(new ProjectSettingsChangeUndoItem(oldProjectSettings, newProjectSettings, 
                                                                     projectSettings));
            refreshUndoMenu();
        }

        private void addGlobalTemporalReferenceChangeUndoItem(TemporalReference oldTemporalReference, TemporalReference newTemporalReference)
        {
            TemporalReference localOldTempRef = new TemporalReference(oldTemporalReference);
            TemporalReference localNewTempRef = new TemporalReference(newTemporalReference);
            _undoChain.AddUndoItem(new GlobalTemporalReferenceChangeUndoItem(localOldTempRef, localNewTempRef));
            refreshUndoMenu();
        }

        private void addTreeNodeAddUndoItem()
        {
            int treeLevel = treeView1.SelectedNode.Level;
            int index0, index1, index2;
            index0 = index1 = index2 = -1;
            switch (treeLevel)
            {
                case 0:
                    index0 = treeView1.SelectedNode.Index;
                    break;
                case 1:
                    index0 = treeView1.SelectedNode.Parent.Index;
                    index1 = treeView1.SelectedNode.Index;
                    break;
                case 2:
                    index0 = treeView1.SelectedNode.Parent.Parent.Index;
                    index1 = treeView1.SelectedNode.Parent.Index;
                    index2 = treeView1.SelectedNode.Index;
                    break;
            }
            ITaggable taggableItem = GetScenarioElementByTag((int)treeView1.SelectedNode.Tag);
            _undoChain.AddUndoItem(new TreeNodeAddUndoItem(treeView1, (TreeNode)treeView1.SelectedNode,
                                   treeLevel, index0, index1, index2, _scenarioElements, taggableItem));
            refreshUndoMenu();
        }

        private void addTreeNodeDeleteUndoItem()
        {
            int treeLevel = treeView1.SelectedNode.Level;
            int index0, index1, index2;
            index0 = index1 = index2 = -1;
            switch (treeLevel)
            {
                case 0:
                    index0 = treeView1.SelectedNode.Index;
                    break;
                case 1:
                    index0 = treeView1.SelectedNode.Parent.Index;
                    index1 = treeView1.SelectedNode.Index;
                    break;
                case 2:
                    index0 = treeView1.SelectedNode.Parent.Parent.Index;
                    index1 = treeView1.SelectedNode.Parent.Index;
                    index2 = treeView1.SelectedNode.Index;
                    break;
            }
            int tag = (int)treeView1.SelectedNode.Tag;
            ITaggable taggableItem = GetScenarioElementByTag(tag);
            _undoChain.AddUndoItem(new TreeNodeDeleteUndoItem(treeView1, (TreeNode)treeView1.SelectedNode,
                                   treeLevel, index0, index1, index2, _scenarioElements, taggableItem));
            refreshUndoMenu();
        }

        private void addTreeNodeMoveUndoItem()
        {
            int toTreeLevel = treeView1.SelectedNode.Parent.Level;
            int toIndex0, toIndex1, toIndex2;
            toIndex0 = toIndex1 = toIndex2 = -1;
            switch (toTreeLevel)
            {
                case 0:
                    toIndex0 = treeView1.SelectedNode.Parent.Index;
                    toIndex1 = treeView1.SelectedNode.Index;
                    break;
                case 1:
                    toIndex0 = treeView1.SelectedNode.Parent.Parent.Index;
                    toIndex1 = treeView1.SelectedNode.Parent.Index;
                    toIndex2 = treeView1.SelectedNode.Index;
                    break;
                case 2:
                    // should never happen
                    break;
            }
            _undoChain.AddUndoItem(new TreeNodeMoveUndoItem(treeView1, _copiedNodeClone, toTreeLevel, 
                                   _copiedLevel0Index, _copiedLevel1Index, _copiedLevel2Index, toIndex0, toIndex1, toIndex2));
            refreshUndoMenu();
        }

        private void addTreeNodeRenameUndoItem(string newNodeText)
        {
            _undoChain.AddUndoItem(new TreeNodeRenameUndoItem(treeView1, treeView1.SelectedNode, newNodeText));
            refreshUndoMenu();
        }

        #region Unused code from SoftwareProductions Undo utilities

        //After an undo or redo is performed, we want to move the focus to the 
        //control whose value just changed, so the user can see what the change was. 
        //The value in the control will automatically be updated by the data binding.
        private void UndoChain_Acted(object sender, UndoActionEventArgs e)
        {
        }

        #endregion

        #endregion Undo/Redo methods

        #region TreeView node methods

        private void addFeatureSetTreeItem(PackageType packageType)
        {
            _movingNode = false;
            int packageIndex = -1;
            int packageTag = -1;
            if (treeView1.SelectedNode.Level == 1)
            {
                packageTag = (int)treeView1.SelectedNode.Tag;
                packageIndex = GetScenarioElementIndexByTag(packageTag);
                Package package = (Package)GetScenarioElementByTag(packageTag);
                TreeNode newFeatureSetNode = new TreeNode();
                newFeatureSetNode.ImageIndex = newFeatureSetNode.SelectedImageIndex = (int) packageType;
                newFeatureSetNode.Text = package.GetDefaultFeatureSetNodeText();
                _scenarioElements.Add(new FeatureSet(packageType, package));
                int featureSetIndex = _scenarioElements.Count - 1;
                treeView1.SelectedNode.Nodes.Add(newFeatureSetNode);
                treeView1.SelectedNode = treeView1.SelectedNode.LastNode;
                TagLink newLink = new TagLink(treeView1.SelectedNode, _scenarioElements[featureSetIndex], _tagStore);
                _tagStore.Connect(newLink);
                _scenarioElements[packageIndex].Items.Add(_scenarioElements[featureSetIndex]);
                addTreeNodeAddUndoItem();
                treeView1.SelectedNode.Expand();
                refreshSummary();
            }
            else
            {
                string msg = "Cannot add new package set at this tree level";
                MessageBox.Show(msg);
            }
            enablePaste(false);
            refreshUI();
        }

        private void addPackageTreeItem(PackageType packageType, ITaggable parent)
        {
            _movingNode = false;
            if (treeView1.SelectedNode.Level == 0)
            {
                TreeNode newPackageNode = new TreeNode();
                newPackageNode.ImageIndex = newPackageNode.SelectedImageIndex = (int) packageType;
                switch (packageType)
                {
                    case (PackageType.WellType):
                        newPackageNode.Text = "New Well Package";
                        _scenarioElements.Add(new WellPackage(parent));
                        break;
                    case (PackageType.RiverType):
                        newPackageNode.Text = "New River Package";
                        _scenarioElements.Add(new RiverPackage(parent));
                        break;
                    case (PackageType.ChdType):
                        newPackageNode.Text = "New CHD Package";
                        _scenarioElements.Add(new ChdPackage(parent));
                        break;
                    case (PackageType.RchType):
                        newPackageNode.Text = "New Recharge Package";
                        _scenarioElements.Add(new RchPackage(parent));
                        break;
                    case (PackageType.GhbType):
                        newPackageNode.Text = "New GHB Package";
                        _scenarioElements.Add(new GhbPackage(parent));
                        break;
                }
                int scenIndex = _scenarioElements.Count - 1;
                treeView1.SelectedNode.Nodes.Add(newPackageNode);
                Scenario scenario = (Scenario) GetScenarioElementByTag((int)treeView1.SelectedNode.Tag);
                treeView1.SelectedNode = treeView1.SelectedNode.LastNode;
                TagLink newLink = new TagLink(treeView1.SelectedNode,
                               _scenarioElements[scenIndex], _tagStore);
                _scenarioElements[scenIndex].Name = treeView1.SelectedNode.Text;
                _tagStore.Connect(newLink);
                scenario.Items.Add(_scenarioElements[scenIndex]);
                treeView1.SelectedNode.Expand();
                addTreeNodeAddUndoItem();
                refreshSummary();
            }
            else
            {
                string msg = "Cannot add new package at this tree level";
                MessageBox.Show(msg);
            }
            enablePaste(false);
            refreshUI();
        }

        private void addScenario()
        {
            string tempScenarioID = nextScenarioID();
            string errMsg = "";
            // Add new scenario to tree view.
            // Determine default ScenarioID.
            _scenarioIdDialog.ScenarioID = tempScenarioID;
            // Open ScenarioIdDialog to let user override default ScenarioID
            DialogResult dr;
            dr = _scenarioIdDialog.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                tempScenarioID = _scenarioIdDialog.ScenarioID;
                if (isValidScenarioID(tempScenarioID, ref errMsg))
                {
                    treeView1.Nodes.Add(tempScenarioID);
                    treeView1.SelectedNode = treeView1.Nodes[treeView1.Nodes.Count - 1];
                    treeView1.SelectedNode.ImageIndex = 0;
                    treeView1.SelectedNode.SelectedImageIndex = 0;
                    Scenario scenario = new Scenario();
                    scenario.Name = tempScenarioID;
                    if (File.Exists(SmProjectSettings.MasterNameFile))
                    {
                        scenario.NameFileDirectory = Path.GetDirectoryName(SmProjectSettings.MasterNameFile);
                    }
                    _scenarioElements.Add(scenario);
                    int scenIndex = _scenarioElements.Count - 1;
                    TagLink newLink = new TagLink(treeView1.SelectedNode,
                                   _scenarioElements[scenIndex], _tagStore);
                    _tagStore.Connect(newLink);
                    addTreeNodeAddUndoItem();
                }
                else
                {
                    MessageBox.Show(errMsg,"Error In Assigning Scenario ID");
                }
            }
            enablePaste(false);
            refreshUI();
        }

        private void copyNode(TreeNode node)
        {
            int level;
            _copiedLevel0Index = _copiedLevel1Index = _copiedLevel2Index = -1;
            if (_copiedNodeClone == null)
            {
                _copiedNodeClone = new TreeNode();
            }
            _copiedNodeClone = (TreeNode)node.Clone();
            level = node.Level;
            _copiedNodeCloneLevel = level;
            if (level == 0)
            {
                _copiedLevel0Index = node.Index;
            }
            else if (level == 1)
            {
                _copiedLevel1Index = node.Index;
                _copiedLevel0Index = node.Parent.Index;
            }
            else if (level == 2)
            {
                _copiedLevel2Index = node.Index;
                _copiedLevel1Index = node.Parent.Index;
                _copiedLevel0Index = node.Parent.Parent.Index;
            }
        }

        private List<ITaggable> copyScenarioElements()
        {
            List<ITaggable> copy = new List<ITaggable>();
            for (int i = 0; i < _scenarioElements.Count; i++)
            {
                copy.Add((ITaggable)_scenarioElements[i].Clone());
            }
            return copy;
        }

        private TreeView copyTreeView()
        {
            TreeView treeViewCopy = new TreeView();
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                treeViewCopy.Nodes.Add((TreeNode)treeView1.Nodes[i].Clone());
            }
            return treeViewCopy;
        }

        private void copyScenarioTreeItem()
        {
            int tag = (int)treeView1.SelectedNode.Tag;
            _scenarioIdDialog.ScenarioID = nextScenarioID();
            DialogResult dr = _scenarioIdDialog.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                string newScenarioID = _scenarioIdDialog.ScenarioID;
                string errMsg = "";
                if (isValidScenarioID(newScenarioID, ref errMsg))
                {
                    _movingNode = false;
                    treeView1.Nodes.Add((TreeNode)treeView1.SelectedNode.Clone());
                    int newNodeIndex = treeView1.Nodes.Count - 1;
                    if (treeView1.SelectedNode.Tag != null)
                    {
                        tag = (int)treeView1.SelectedNode.Tag;
                        Scenario newScenario = (Scenario)((Scenario)GetScenarioElementByTag(tag)).Clone();
                        newScenario.Name = newScenarioID;
                        _scenarioElements.Add(newScenario);
                        int newScenarioIndex = _scenarioElements.Count - 1;
                        TagLink newLink = new TagLink(treeView1.Nodes[newNodeIndex],
                                                      _scenarioElements[newScenarioIndex],
                                                      _tagStore);
                        _tagStore.Connect(newLink);
                        int newLinkIndex = _tagStore.Count - 1;
                        newLink.TreeNode = treeView1.Nodes[treeView1.Nodes.Count - 1];
                        newLink.ScenarioElement = newScenario;
                        newScenario.ConnectList(_scenarioElements);
                        treeView1.SelectedNode = treeView1.Nodes[treeView1.Nodes.Count - 1];
                        treeView1.SelectedNode.Text = newScenarioID;
                        treeView1.SelectedNode.ExpandAll();
                        addScenarioCopyUndoItem(newScenarioIndex, newLinkIndex); // HERE?
                        refreshSummary();
                    }
                    treeView1.Nodes[newNodeIndex].Expand();
                    refreshUI();
                }
                else
                {
                    MessageBox.Show(errMsg, "Error In Assigning Scenario ID");
                }
            }
            enablePaste(false);
        }

        private void copyTreeItem()
        {
            _copiedNodeClone = (TreeNode)treeView1.SelectedNode.Clone();
            _copiedNodeCloneLevel = treeView1.SelectedNode.Level;
            _pasteLevel = _copiedNodeCloneLevel - 1;
            _movingNode = false;
            enablePaste(true);
            refreshUI();
        }

        private int countTreeNodesOfType(PackageType packageType)
        {
            int count = 0;
            foreach (TreeNode node in treeView1.SelectedNode.Nodes)
            {
                if (node.ImageIndex == (int) packageType) count++;
            }
            return count;
        }

        private void cutTreeItem()
        {
            TreeView treeViewOld = copyTreeView();
            List<ITaggable> scenarioElementsOld = copyScenarioElements();
            _movingNode = true;
            _pasteLevel = treeView1.SelectedNode.Level - 1;
            int tag = (int)treeView1.SelectedNode.Tag;
            _taggableCutItemClone = (ITaggable)GetScenarioElementByTag(tag).Clone();
            copyNode(treeView1.SelectedNode);
            int cutNodeTag = (int)treeView1.SelectedNode.Tag;
            removeElementByTag(cutNodeTag);
            treeView1.SelectedNode.Remove();
            addScenarioManagerTreeChangeUndoItem(treeViewOld, treeView1, scenarioElementsOld, _scenarioElements);
            enablePaste(true);
            refreshUI();
        }

        private void deleteTreeItem()
        {
            TreeView treeViewOld = copyTreeView();
            List<ITaggable> scenarioElementsOld = copyScenarioElements();
            _movingNode = false;
            _copiedNodeClone = null;
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag != null)
                {
                    int removeTag = (int) treeView1.SelectedNode.Tag;
                    ITaggable removeItem = null;
                    foreach (ITaggable e in _scenarioElements)
                    {
                        if (e.Tag == removeTag)
                        {
                            removeItem = e;
                            break;
                        }
                    }
                    if (removeItem != null)
                    {
                        if (treeView1.SelectedNode.Level > 0)
                        {
                            int parentTag = (int)treeView1.SelectedNode.Parent.Tag;
                            ITaggable parent = GetScenarioElementByTag(parentTag);
                            for (int i = parent.Items.Count-1; i >= 0; i--)
                            {
                                if (parent.Items[i].Tag == removeTag)
                                {
                                    parent.Items.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        _scenarioElements.Remove(removeItem);
                    }
                    _tagStore.Items.Remove(_tagStore.GetLinkByTag(removeTag));
                }
                treeView1.SelectedNode.Remove();
                addScenarioManagerTreeChangeUndoItem(treeViewOld, treeView1, scenarioElementsOld, _scenarioElements);
            }
            enablePaste(false);
            refreshUI();
        }

        private TreeNode makeTreeNodeAndLink(SerializableTreeNode node, TagStore linkOwner)
        {
            // Generate a TreeNode from a SerializableTreeNode,
            // generating child TreeNode elements recursively as needed.
            TreeNode treeNode = new TreeNode(node.Text);
            treeNode.ImageIndex = node.ImageIndex;
            treeNode.SelectedImageIndex = node.ImageIndex;
            treeNode.Tag = node.Tag;
            ITaggable taggableElement = GetScenarioElementByTag(node.Tag);
            if (taggableElement != null)
            {
                TagLink tagLink = new TagLink(node.Tag, treeNode, taggableElement, linkOwner);
                taggableElement.Link = tagLink;
                linkOwner.Items.Add(tagLink);
            }
            if (node.Items != null)
            {
                foreach (SerializableTreeNode n in node.Items)
                {
                    TreeNode tn = makeTreeNodeAndLink(n, linkOwner);
                    treeNode.Nodes.Add(tn);
                }
            }
            refreshUI();
            return treeNode;
        }

        private void moveNodeToNodeAt(int X, int Y)
        {
            _draggingNode = false;
            {
                TreeView treeViewOld = copyTreeView();
                List<ITaggable> scenarioElementsOld = copyScenarioElements();
                treeView1.SelectedNode = treeView1.GetNodeAt(X, Y);
                if (pasteTreeItem(false))
                {
                    int copiedNodeTag = (int)_copiedNodeClone.Tag;
                    int oldTag = (int)treeView1.SelectedNode.Tag;
                    selectCopiedNode();
                    removeElementByTag(copiedNodeTag);
                    treeView1.SelectedNode.Remove();
                    addScenarioManagerTreeChangeUndoItem(treeViewOld, treeView1, scenarioElementsOld, _scenarioElements);
                }
            }
            refreshUI();
        }

        private void removeElementByTag(int tag)
        {
            try
            {
                ITaggable taggableItem = GetScenarioElementByTag(tag);
                ITaggable taggableParent = taggableItem.Parent;
                taggableParent.Items.Remove(taggableItem);
                int copiedNodeIndex = this.GetScenarioElementIndexByTag(tag);
                this._scenarioElements.RemoveAt(copiedNodeIndex);
            }
            catch
            {
            }
        }

        private bool okToAddNodeOfType(PackageType packageType)
        {
            bool OK = true;
            if (!_supportPsb && (countTreeNodesOfType(packageType) > 0))
            {
                OK = false;
            }
            return OK;
        }

        private bool pasteTreeItem(bool createUndoItem)
        {
            bool returnValue = true;
            string msg = "";
            if (treeView1.SelectedNode == null)
            {
                returnValue = false;
            }
            else
            {
                if (treeView1.SelectedNode.Level == (_copiedNodeCloneLevel - 1))
                {
                    int newElementIndex;
                    PackageType packageType = (PackageType)_copiedNodeClone.ImageIndex;
                    ITaggable taggableItem = null;
                    ITaggable taggableParent = null;
                    bool OK = true;
                    switch (_copiedNodeCloneLevel)
                    {
                        case 1:  // Item to be pasted represents a package
                            OK = okToAddNodeOfType(packageType);
                            if (!OK)
                            {
                                msg = "Cannot paste package '" + _copiedNodeClone.Text + 
                                    "' because a package of the same type is already present in target scenario";
                            }
                            break;
                        case 2:  // Item to be pasted represents a feature set
                            OK = (treeView1.SelectedNode.ImageIndex == _copiedNodeClone.ImageIndex);
                            if (!OK)
                            {
                                msg = "Cannot paste feature set '" + _copiedNodeClone.Text +
                                    "' because feature set is not of the same type as target package";
                            }
                            break;
                    }
                    if (OK)
                    {
                        if (_movingNode)
                        {
                            taggableItem = (GetScenarioElementByTag((int)_copiedNodeClone.Tag));
                            if (taggableItem == null)
                            {
                                taggableItem = (ITaggable)_taggableCutItemClone.Clone();
                            }
                        }
                        else
                        {
                            taggableItem = (ITaggable)(GetScenarioElementByTag((int)_copiedNodeClone.Tag)).Clone();
                            if (taggableItem == null)
                            {
                                taggableItem = (ITaggable)_taggableCutItemClone.Clone();
                            }
                        }
                        TreeView treeViewOld = copyTreeView();
                        List<ITaggable> scenarioElementsOld = copyScenarioElements();
                        _scenarioElements.Add(taggableItem);
                        newElementIndex = _scenarioElements.Count - 1;
                        taggableParent = (ITaggable)GetScenarioElementByTag((int)treeView1.SelectedNode.Tag);
                        treeView1.SelectedNode.Nodes.Add((TreeNode)_copiedNodeClone.Clone());
                        int newNodeIndex = treeView1.SelectedNode.Nodes.Count - 1;
                        treeView1.SelectedNode = treeView1.SelectedNode.LastNode;
                        // Make a TagLink
                        TagLink newLink = new TagLink(treeView1.SelectedNode, taggableItem, _tagStore);
                        _tagStore.Connect(newLink);
                        taggableParent.Items.Add(taggableItem);
                        switch (treeView1.SelectedNode.Level)
                        {
                            case 1:
                                ((Package)taggableItem).Parent = taggableParent;
                                break;
                            case 2:
                                ((FeatureSet)taggableItem).Parent = taggableParent;
                                break;
                        }
                        taggableItem.ConnectList(_scenarioElements);
                        treeView1.SelectedNode.Expand();
                        if (createUndoItem)
                        {
                            addScenarioManagerTreeChangeUndoItem(treeViewOld, treeView1, scenarioElementsOld, _scenarioElements);
                        }
                        returnValue = true;
                    }
                    else
                    {
                        MessageBox.Show(msg);
                        returnValue = false;
                    }
                }
                else
                {
                    msg = "Cannot paste '" + _copiedNodeClone.Text + "' at this tree level";
                    MessageBox.Show(msg);
                    returnValue = false;
                }
            }
            if (returnValue)
            {
                _movingNode = false;
            }
            enablePaste(false);
            refreshUI();
            return returnValue;
        }

        private void enablePaste(bool enable)
        {
            contextMenuPaste.Enabled = enable;
            menuItemEditPaste.Enabled = enable;
            if (!enable)
            {
                _pasteLevel = -1;
            }
        }

        private void selectCopiedNode()
        {
            switch (_copiedNodeCloneLevel)
            {
                case 0:
                    treeView1.SelectedNode = treeView1.Nodes[_copiedLevel0Index];
                    break;
                case 1:
                    treeView1.SelectedNode = treeView1.Nodes[_copiedLevel0Index].Nodes[_copiedLevel1Index];
                    break;
                case 2:
                    treeView1.SelectedNode = treeView1.Nodes[_copiedLevel0Index].Nodes[_copiedLevel1Index].Nodes[_copiedLevel2Index];
                    break;
            }
        }

        #endregion TreeView node methods

        #region Scenario element methods

        public ITaggable GetScenarioElementByTag(int tag)
        {
            ITaggable taggableItem = null;
            foreach (ITaggable e in _scenarioElements)
            {
                if (e.Tag == tag)
                {
                    taggableItem = e;
                    break;
                }
            }
            return taggableItem;
        }

        public int GetScenarioElementIndexByTag(int tag)
        {
            for (int i=0; i< _scenarioElements.Count; i++)
            {
                if (_scenarioElements[i].Tag == tag)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetFeatureSetIndexByTag(Package parent, int tag)
        {
            for (int i = 0; i < parent.Items.Count; i++)
            {
                if (parent.Items[i].Tag == tag)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetPackageIndexByTag(Scenario parent, int tag)
        {
            for (int i = 0; i < parent.Items.Count; i++)
            {
                if (parent.Items[i].Tag == tag)
                {
                    return i;
                }
            }
            return -1;
        }

        public TagLink GetLinkByTag(TagStore tagStore, int tag)
        {
            TagLink nullTag = null;
            foreach (TagLink link in tagStore.Items)
            {
                if (link.Tag == tag)
                {
                    return link;
                }
            }
            return nullTag;
        }

        private static Scenario makeScenario(SerializableScenarioElement element, TagStore linkOwner, string projectFileDirectory)
        {
            // Generate a Scenario from a SerializableScenarioElement,
            // generating child Packages as needed.
            Scenario scenario = new Scenario();
            scenario.Description = element.Description;
            scenario.Name = element.Name;
            scenario.Tag = element.Tag;
            scenario.Link.Owner = linkOwner;
            if (element.Items != null)
            {
                foreach (SerializableScenarioElement item in element.Items)
                {
                    Package newPackage = makePackage(item, linkOwner, (ITaggable)scenario, projectFileDirectory);
                    scenario.Items.Add((ITaggable)newPackage);
                }
            }
            return scenario;
        }

        private static Package makePackage(SerializableScenarioElement element, TagStore linkOwner, ITaggable parent, string projectFileDirectory)
        {
            // Generate a Package from a SerializableScenarioElement,
            // generating child FeatureSets as needed.
            Package package = null;
            switch (element.Type)
            {
                case PackageType.RiverType:
                    package = new RiverPackage(parent);
                    break;
                case PackageType.WellType:
                    package = new WellPackage(parent);
                    break;
                case PackageType.ChdType:
                    package = new ChdPackage(parent);
                    break;
                case PackageType.RchType:
                    package = new RchPackage(parent);
                    break;
                case PackageType.GhbType:
                    package = new GhbPackage(parent);
                    break;
                default:
                    // should never happen, but will not compile otherwise
                    return package;
            }
            package.Description = element.Description;
            package.Name = element.Name;
            package.CbcFlag = element.CbcFlag;
            package.Tag = element.Tag;
            package.Parent = parent;
            package.Link.Owner = linkOwner;
            if (element.Items != null)
            {
                foreach (SerializableScenarioElement item in element.Items)
                {
                    FeatureSet newFeatureSet = makeFeatureSet(item, linkOwner, package, projectFileDirectory);
                    package.Items.Add((ITaggable)newFeatureSet);
                }
            }
            return package;
        }

        private static FeatureSet makeFeatureSet(SerializableScenarioElement element, TagStore linkOwner, ITaggable parent, string projectFileDirectory)
        {
            // Generate a FeatureSet from a SerializableScenarioElement.
            FeatureSet featureSet = new FeatureSet(element.Type, parent);
            featureSet.Description = element.Description;
            featureSet.Name = element.Name;
            featureSet.ShapefileAbsolutePath = FileUtil.Relative2Absolute(element.ShapefileRelativePath, projectFileDirectory);
            featureSet.Tag = element.Tag;
            featureSet.Parent = parent;
            featureSet.Link.Owner = linkOwner;
            featureSet.TimeSeriesAbsolutePath = FileUtil.Relative2Absolute(element.TimeSeriesRelativePath, projectFileDirectory);
            featureSet.TimeSeriesSecondaryAbsolutePath = FileUtil.Relative2Absolute(element.TimeSeriesSecondaryRelativePath, projectFileDirectory);
            featureSet.KeyField = element.KeyField;
            featureSet.LabelFeatures = element.LabelFeatures;
            featureSet.LayerAttribute = element.LayerAttribute;
            if (element.GeoValueList.Count > 0)
            {
                featureSet.GeoValueList.Clear();
                foreach (GeoValue geoValue in element.GeoValueList)
                {
                    featureSet.GeoValueList.Add(new GeoValue(geoValue));
                }
            }
            featureSet.PackageOption = element.PackageOption;
            featureSet.CbcFlag = element.CbcFlag;
            featureSet.TopElevationAttribute = element.TopElevationAttribute;
            featureSet.BottomElevationAttribute = element.BottomElevationAttribute;
            featureSet.LayMethod = element.LayMethod;
            featureSet.InterpretationMethod = element.InterpretationMethod;
            featureSet.DefaultLayer = element.DefaultLayer;
            
            return featureSet;
        }

        private string editFeatureSetAndReturnName(int tag, int parentTag, string text, 
                                                   int index)
        {
            Package package = (Package)GetScenarioElementByTag(parentTag);
            FeatureSet fs;
            fs = (FeatureSet)GetScenarioElementByTag(tag);
            fs.Name = text;
            editFeatureSet(tag, package);
            text = GetScenarioElementByTag(tag).Name;
            enablePaste(false);
            refreshUI();
            return text;
        }

        private void editFeatureSet(int tag, ITaggable parent)
        {
            int fsIndex = GetFeatureSetIndexByTag((Package)parent, tag);
            int numFeatureSets = ((Package)parent).Items.Count;
            int seIndex = GetScenarioElementIndexByTag(tag);
            _featureSetForm = new FeatureSetForm(((FeatureSet)_scenarioElements[seIndex]).Type);
            _featureSetForm.FeatureSet = (FeatureSet)_scenarioElements[seIndex].Clone();
            _featureSetForm.MustBeMaster = true;
            int indxMasterFeatureSet = 0;
            if (parent is RchPackage) // Ned TODO: or parent is EvtPackage, eventually
            {
                _featureSetForm.MustBeMaster = (numFeatureSets < 2);
                indxMasterFeatureSet = ((RchPackage)parent).IndexMasterFeatureSet;
                if (numFeatureSets == 1)
                {
                    indxMasterFeatureSet = 0;
                    ((RchPackage)parent).IndexMasterFeatureSet = 0;
                }
                _featureSetForm.GroupBoxRchLayerAssignment.UseAsMasterFeatureSet = (indxMasterFeatureSet == fsIndex);
            }
            _featureSetForm.ProjectDirectory = _projectFileDirectory;
            if (_imageLayerBackground.GetGeoImage() == null)
            {
                if (File.Exists(SmProjectSettings.BackgroundImageFile))
                {
                    // Make BackgroundWorker available to GdalHelper, 
                    // which will generate the background image
                    if (imageMakerBackgroundWorker != null)
                    {
                        imageMakerBackgroundWorker.Dispose();
                    }
                    imageMakerBackgroundWorker = new BackgroundWorker();
                    setUpImageMakerBackgroundWorker(imageMakerBackgroundWorker);
                    progressBar.Value = 0;
                    progressBar.Visible = true;
                    StatusLabel.Text = "Generating image...";
                    UseWaitCursor = true;
                    enableControls(false);
                    Application.DoEvents();

                    // Generate background image by reading georeferenced image file.
                    _imageLayerBackground.Replace(SmProjectSettings.BackgroundImageFile);

                    UseWaitCursor = false;
                    enableControls(true);
                    Application.DoEvents();
                }
            }
            StatusLabel.Text = "Opening feature set for editing...";
            Application.DoEvents();
            _featureSetForm.ImageLayerBackground = _imageLayerBackground;
            if (GridShapefileAbsolutePath != "")
            {
                if (File.Exists(GridShapefileAbsolutePath))
                {
                    _featureSetForm.ModelGridShapefilePath = GridShapefileAbsolutePath;
                }
                else
                {
                    _featureSetForm.ModelGridShapefilePath = "";
                }
            }
            if (_modelGrid == null && File.Exists(GridShapefileAbsolutePath))
            {
                _modelGrid = CreateCellCenteredArealGrid.CreateGrid(GridShapefileAbsolutePath, StaticObjects.ModelGridInfo);
            }
            if (_modelGrid != null)
            {
                _featureSetForm.ModelGrid = _modelGrid;
            }
            if (_discretizationFile != null)
            {
                _featureSetForm.Nlay = _discretizationFile.getNlay();
            }
            StatusLabel.Text = "";
            DialogResult dr = _featureSetForm.ShowDialog();
            if (dr == DialogResult.OK && _featureSetForm.Changed)
            {
                FeatureSet fsTemp = (FeatureSet)_featureSetForm.FeatureSet.Clone();
                if (fsTemp.Parent is RchPackage)
                {
                    ((RchPackage)fsTemp.Parent).AreaFeaturesNeedRepopulating = true;
                    if (numFeatureSets > 1)
                    {
                        if (_featureSetForm.GroupBoxRchLayerAssignment.UseAsMasterFeatureSet)
                        {
                            ((RchPackage)fsTemp.Parent).IndexMasterFeatureSet = fsIndex;
                        }
                        else
                        {
                            ((RchPackage)fsTemp.Parent).IndexMasterFeatureSet = 0;
                        }
                    }
                    else
                    {
                        ((RchPackage)fsTemp.Parent).IndexMasterFeatureSet = 0;
                    }
                }
                addScenarioElementChangeUndoItem((FeatureSet)_scenarioElements[seIndex], fsTemp);
                _scenarioElements[seIndex].AssignFrom(_featureSetForm.FeatureSet);
                if (treeView1.SelectedNode.Text != fsTemp.Name)
                {
                    addTreeNodeRenameUndoItem(fsTemp.Name);
                }
                treeView1.SelectedNode.Text = fsTemp.Name;
                _scenarioElements[seIndex].AssignFrom(fsTemp);
            }
            _featureSetForm.Dispose();
            refreshUI();
            return; 
        }

        #endregion Scenario element methods

        #region Project-related methods

        /// <summary>
        /// Convert paths not in scenario elements to absolute paths
        /// </summary>
        /// <param name="projectDirectory"></param>
        private void assignAbsolutePaths(string projectDirectory)
        {
            if (_projectFileDirectory != projectDirectory)
            {
                if (Directory.Exists(_projectFileDirectory) && Directory.Exists(projectDirectory))
                {
                    if (SmProjectSettings.MasterNameFile != "")
                    {
                        SmProjectSettings.MasterNameFile =
                            FileUtil.Relative2Absolute(SmProjectSettings.MasterNameFile,
                                                       _projectFileDirectory);
                    }
                    for (int i = 0; i < SmProjectSettings.NameFiles.Count; i++)
                    {
                        if (SmProjectSettings.NameFiles[i] != "")
                        {
                            SmProjectSettings.NameFiles[i] =
                                FileUtil.Relative2Absolute(SmProjectSettings.NameFiles[i],
                                                           _projectFileDirectory);
                        }
                    }
                }
            }
        }

        private bool assignDiscretizationFile()
        {
            try
            {
                string disFilePath = FileUtil.Relative2Absolute(_discretizationFileAbsolutePath.String, 
                                                                _namefileInfo.GetDirectory());
                _discretizationFile = DiscretizationFile.fromFile(disFilePath, _freeFormat,_namefileInfo);
                if (_discretizationFile != null)
                {
                    assignSimulationEndTime();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Assign NeedRowAndCol for all FeatureSets in _scenarioElements
        /// </summary>
        /// <param name="need"></param>
        private void assignNeedRowAndCol(bool need)
        {
            foreach (ITaggable item in _scenarioElements)
            {
                if (item.ElemType == ElementType.FeatureSet)
                {
                    ((FeatureSet)item).NeedRowAndCol = need;
                }
            }
        }

        private void assignRelativePaths()
        {
            SmProjectSettings.MasterNameFile =
                FileUtil.Absolute2Relative(SmProjectSettings.MasterNameFile, _projectFileDirectory);
            for (int i = 0; i < SmProjectSettings.NameFiles.Count; i++)
            {
                SmProjectSettings.NameFiles[i] =
                    FileUtil.Absolute2Relative(SmProjectSettings.NameFiles[i],
                                               _projectFileDirectory);
            }
        }

        private bool assignSimulationEndTime()
        {
            bool OK = false;
            if (_discretizationFile != null)
            {
                _simulationEndTime = SimulationStartTime + _discretizationFile.GetSimulationTimeSpan();
                OK = true;
            }
            return OK;
        }

        private void clearUndoChain()
        {
            _undoChain.Clear();
            _maxUndoCount = 0;
        }

        private void convertTaggablePaths(string oldDirectoryPath, string newDirectoryPath)
        {
            foreach (ITaggable item in _scenarioElements)
            {
                item.ConvertRelativePaths(oldDirectoryPath, newDirectoryPath);
            }
            refreshUI();
        }

        private DateTime defaultSimulationTime()
        {
            return new DateTime(1900, 1, 1);
        }

        private bool defineModelGrid(bool showDialog)
        {
            bool OK = false;
            string oldPath, oldAbsPath, tempAbsPath;
            DialogResult dialogResult;
            oldPath = GridShapefileAbsolutePath;
            oldAbsPath = GridShapefileAbsolutePath;
            tempAbsPath = GridShapefileAbsolutePath;
            if (Directory.Exists(_projectFileDirectory))
            {
                if (GridShapefileAbsolutePath == "")
                {
                    _modelGridForm.Directory = _projectFileDirectory;
                    _modelGridForm.ShapefilePath = "";
                }
            }
            else
            {
                string currentDir = Directory.GetCurrentDirectory();
                _modelGridForm.Directory = currentDir;
            }

            // If needed, show modelGridForm dialog
            if (!File.Exists(tempAbsPath) || showDialog)
            {
                _modelGridForm.ShapefilePath = "";
                _modelGridForm.Directory = "";
                if (File.Exists(tempAbsPath))
                {
                    _modelGridForm.ShapefilePath = tempAbsPath;
                }
                else if (Directory.Exists(_projectFileDirectory))
                {
                    _modelGridForm.Directory = _projectFileDirectory;
                }
                dialogResult = _modelGridForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    _modelGrid = null;
                    tempAbsPath = _modelGridForm.ShapefilePath;
                    GridShapefileAbsolutePath = tempAbsPath;
                    addModelGridChangeUndoItem(oldPath, GridShapefileAbsolutePath);
                    // Assign NeedRowAndCol = true for all FeatureSets
                    if (File.Exists(tempAbsPath))
                    {
                        OK = importModelGrid(tempAbsPath);
                        assignNeedRowAndCol(true);
                    }
                }
            }
            else
            {
                // If the model grid has already been defined and the shapefile path 
                // has not changed, no need to import shapefile again.
                if ((tempAbsPath == oldAbsPath) && (_modelGrid != null))
                {
                    OK = true;
                }
                else
                {
                    // If the grid shapefile path has changed or the model grid has 
                    // not been defined, import the shapefile to define the grid.
                    if (File.Exists(tempAbsPath))
                    {
                        OK = importModelGrid(tempAbsPath);
                        assignNeedRowAndCol(true);
                    }
                    else
                    {
                        OK = false;
                    }
                }
            }
            refreshUI(false);
            return OK;
        }

        private bool importModelGrid(string modelGridAbsolutePath)
        {
            bool OK = true;
            if (_modelGrid != null)
            {
                _modelGrid = null;
            }
            _savedCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            int progress = 20;
            progressBar.Value = progress;
            progressBar.Visible = true;
            StatusLabel.Text = "Importing model grid...";
            Application.DoEvents();
            Refresh();
            _modelGrid = CreateCellCenteredArealGrid.CreateGrid(modelGridAbsolutePath, StaticObjects.ModelGridInfo);
            progress = 80;
            progressBar.Value = progress;
            Application.DoEvents();
            Refresh();
            if (_modelGrid != null)
            {
                // Add extent for model grid
                IPolygon polygon = _modelGrid.GetPolygon();
                IEnvelope envelope = polygon.EnvelopeInternal;
                Extent gridExtent = new Extent(envelope);
                gridExtent.Name = WorkspaceUtil.MODEL_GRID_EXTENT_NAME;
                WorkspaceUtil.SetDefaultExtent(gridExtent);

                OK = true;
                StatusLabel.Text = "Model grid successfully imported.";
            }
            else
            {
                OK = false;
                StatusLabel.Text = "Error encountered in importing model grid.";
            }
            progressBar.Visible = false;
            progressBar.Value = 0;
            Application.DoEvents();
            return OK;
        }

        private void editProjectSettings()
        {
            _projectSettingsDialog = new ProjectSettingsDialog(_smProjectSettings);
            _projectSettingsDialog.SmProjectSettings.ConvertToAbsolute(_projectFileDirectory);
            DialogResult dr = _projectSettingsDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                addProjectSettingsChangeUndoItem(SmProjectSettings,
                                                 _projectSettingsDialog.SmProjectSettings,
                                                 SmProjectSettings);
                SmProjectSettings = (SMProjectSettings)_projectSettingsDialog.SmProjectSettings.Clone();
                readNameFile();
                StatusLabel.Text = "Project settings assigned.";
            }
            enablePaste(false);
            refreshUI(false);
        }

        private bool exportPackage(Package package, BackgroundWorker worker)
        {
            bool OK = true;
            string fileType = Helpers.PackageTypeToString(package.Type);
            package.NameFileEntry = _namefileInfo.GetEntry(fileType);
            int unitNumber = package.NameFileEntry.Unit;
            string message;
            string originalDir = Directory.GetCurrentDirectory();
            if (OK)
            {
                if (!getDiscretizationData())
                {
                    message = "Discretization file needs to be imported";
                    MessageBox.Show(message);
                    OK = false;
                }
            }
            if (OK)
            {
                OK = assignSimulationEndTime();
                if (!OK)
                {
                    message = "An error was encountered in assigning simulation end time";
                    MessageBox.Show(message);
                }
            }
            if (OK)
            {
                OK = ((Scenario)package.Parent).PrepareForExport(_discretizationFile, 
                                                                 SimulationStartTime);
                if (!OK)
                {
                    message = "An error was encountered in preparing for export";
                    MessageBox.Show(message);
                }
            }
            //
            if (OK)
            {
                try
                {
                    string scenarioID = package.GetScenarioID();
                    string masterDirectory = getNameFileDirectory();
                    string scenarioDirectory = masterDirectory + Path.DirectorySeparatorChar + scenarioID;
                    if (!Directory.Exists(scenarioDirectory))
                    {
                        Directory.CreateDirectory(scenarioDirectory);
                    }
                    Directory.SetCurrentDirectory(scenarioDirectory);
                    OK = package.Export(_modelGrid, StatusLabel, worker, _freeFormat);
                    if (!OK)
                    {
                        message = "Export failed for package: " + package.Name;
                        MessageBox.Show(message);
                    }
                }
                finally
                {
                    Directory.SetCurrentDirectory(originalDir);
                    StatusLabel.Text = "";
                }
            }
            enablePaste(false);
            return OK;
        }

        private bool exportScenario(Scenario scenario, BackgroundWorker worker)
        {
            bool OK = true;
            int bgwProgress = 0;
            try
            {
                worker.ReportProgress(bgwProgress);
            }
            catch
            {
            }
            if (OK)
            {
                OK = scenario.PrepareForExport(_discretizationFile, SimulationStartTime);
                if (!OK)
                {
                    scenario.Message = "Error encountered in preparing scenario for export";
                }
            }
            if (OK)
            {
                try
                {
                    Application.DoEvents();
                    string exportMessage = "Scenario Manager file: *** not saved ***";

                    if (_projectFileName != "")
                    {
                        string projFile = FileUtil.Relative2Absolute(_projectFileName, _projectFileDirectory);
                        exportMessage = "Scenario Manager file: " + projFile;
                    }
                    OK = scenario.Export(_modelGrid, _projectFileDirectory, SmProjectSettings,
                                         _namefileInfo, StatusLabel, worker, _freeFormat, 
                                         exportMessage);
                    if (!OK)
                    {
                        scenario.Message = "Error encountered in exporting scenario";
                    }
                }
                finally
                {
                }
            }
            if (OK)
            {
                string batchCommandQuotedIfNeeded = scenario.GetBatchCommand(true);
                if (File.Exists(SmProjectSettings.ModflowExecutable))
                {
                    string modflowExe = StringUtil.DoubleQuoteIfNeeded(SmProjectSettings.ModflowExecutable);
                    string modflowArg = StringUtil.DoubleQuoteIfNeeded(scenario.NameFile());
                    string modelCommand = modflowExe + " " + modflowArg;
                    string scenarioFolder = scenario.GetFolder(_smProjectSettings, _projectFileDirectory);
                    string[] contents = new string[5];
                    contents[0] = "@echo off";
                    contents[1] = "REM Batch file " + batchCommandQuotedIfNeeded + " written by Scenario Manager";
                    contents[2] = modelCommand;
                    // Ned TODO: Make batch file smart enough not to pause when NoPause argument is provided.
                    contents[3] = "pause";
                    contents[4] = "";
                    try
                    {
                        Directory.SetCurrentDirectory(_namefileInfo.GetDirectory());
                        string batchCommandNoQuotes = scenario.GetBatchCommand(false);
                        FileUtil.WriteFile(batchCommandNoQuotes, contents);
                    }
                    finally
                    {
                        Directory.SetCurrentDirectory(_projectFileDirectory);
                    }
                }
                else
                {
                    string errStr = "Warning: Batch file " + batchCommandQuotedIfNeeded + " not written because MODFLOW " +
                                    "executable has not been assigned or is invalid (use menu item: Project | Settings)";
                    MessageBox.Show(errStr);
                }
            }
            StatusLabel.Text = "";
            enablePaste(false);
            return OK;
        }

        private bool exportScenarioAndRunSimulation(Scenario scenario, BackgroundWorker worker)
        {
            bool OK = true;
            string msg = "";
            if (File.Exists(SmProjectSettings.ModflowExecutable))
            {
                if (exportScenario(scenario, worker))
                {
                    OK = true;
                }
                else
                {
                    msg = scenario.Message;
                    MessageBox.Show(msg, "Scenario Export Failed");
                    OK = false;
                }
            }
            else
            {
                msg = "Executable file for MODFLOW needs to be defined in Project Settings";
                MessageBox.Show(msg, "Scenario Not Exported");
                OK = false;
            }
            if (OK)
            {
                string scenarioFolder = scenario.GetFolder(_smProjectSettings, _projectFileDirectory);
                string batchCommandQuotedIfNeeded = scenario.GetBatchCommand(true);
                string namefilePath = FileUtil.Relative2Absolute(_smProjectSettings.MasterNameFile, this._projectFileDirectory);
                string namefileDirectory = Path.GetDirectoryName(namefilePath);
                try
                {
                    Directory.SetCurrentDirectory(namefileDirectory);
                    StatusLabel.Text = "Running simulation...";
                    SystemUtil.ExecuteCommandSyncAndWait(batchCommandQuotedIfNeeded);
                }
                finally
                {
                    Directory.SetCurrentDirectory(_projectFileDirectory);
                    StatusLabel.Text = "";
                }
            }
            return OK;
        }

        /// <summary>
        /// Tries to ensure that MODFLOW discretization data are available
        /// </summary>
        /// <returns></returns>
        private bool getDiscretizationData()
        {
            bool OK = true;
            if (_discretizationFile == null)
            {
                if (_discretizationFileAbsolutePath.String == "")
                {
                    string message = "Please assign MODFLOW name file in Project Settings";
                    MessageBox.Show(message);
                }
                else
                {
                    OK = assignDiscretizationFile();
                }
            }
            return OK;
        }

        private bool getFreeFromBasFile(NamefileInfo namefileInfo)
        {
            bool free = false;
            if (namefileInfo != null)
            {
                NameFileEntry nfeBas = namefileInfo.GetEntry("BAS6");
                if (nfeBas != null)
                {
                    string basFileName = nfeBas.Filename;
                    string masterDirectory = getNameFileDirectory();
                    basFileName = FileUtil.Relative2Absolute(basFileName, masterDirectory);
                    using (StreamReader sr = new StreamReader(basFileName))
                    {
                        string line;
                        line = ModflowHelpers.GetDataLine(sr);
                        string[] options = line.Split();
                        for (int i = 0; i < options.Length; i++)
                        {
                            if (String.Equals(options[i], "free", StringComparison.CurrentCultureIgnoreCase))
                            {
                                free = true;
                                break;
                            }
                        }
                        sr.Dispose();
                    }
                }
            }
            return free;
        }

        /// <summary>
        /// Return name of directory where all MODFLOW Name Files reside
        /// </summary>
        /// <returns></returns>
        private string getNameFileDirectory()
        {
            string masterDirectory = "";
            string masterNamefile = _smProjectSettings.MasterNameFile;
            string masterNamefilePath = FileUtil.Relative2Absolute(masterNamefile, _projectFileDirectory);
            try
            {
                masterDirectory = Path.GetDirectoryName(masterNamefilePath);
            }
            catch
            {
            }
            return masterDirectory;
        }

        private bool importDiscretizationFile()
        {
            bool OK = false;
            string tempPath = "", oldPath = "";
            DialogResult dialogResult;
            OpenFileDialog ofdDis = DialogHelper.GetDiscretizationFileDialog();
            if (Directory.Exists(_projectFileDirectory))
            {
                if (DiscretizationFileAbsolutePath != "")
                {
                    oldPath = DiscretizationFileAbsolutePath;
                    string oldDir = Path.GetDirectoryName(oldPath);
                    ofdDis.InitialDirectory = oldDir;
                    ofdDis.FileName = oldPath;
                }
                else
                {
                    ofdDis.InitialDirectory = _projectFileDirectory;
                }
            }
            dialogResult = ofdDis.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                tempPath = ofdDis.FileName;
                DiscretizationFileAbsolutePath = tempPath;
                addDiscretizationFileChangeUndoItem(oldPath, DiscretizationFileAbsolutePath);
                OK = assignDiscretizationFile();
                refreshUI();
            }
            return OK;
        }

        private bool isValidScenarioID(string scenarioID, ref string errMsg)
        {
            TreeNode node;
            bool OK = true;
            errMsg = "";
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                node = treeView1.Nodes[i];
                if (node.Level == 0)
                {
                    if (string.Equals(scenarioID, node.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        errMsg = "Scenario ID '" + scenarioID + "' already exists";
                        OK = false;
                        break;
                    }
                    if (!FileUtil.IsValidFilename(scenarioID))
                    {
                        errMsg = "String contains invalid character(s)";
                        OK = false;
                        break;
                    }
                }
            }
            return OK;
        }

        private string nextScenarioID()
        {
            string searchString = "Scenario_";
            string id = "";
            string numStr;
            string nodeName;
            int idNum;
            int idNumMax = -1;
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                nodeName = treeView1.Nodes[i].Text;
                if (nodeName.StartsWith(searchString))
                {
                    numStr = nodeName.Remove(0,9);
                    try
                    {
                        idNum = Convert.ToInt32(numStr);
                    }
                    catch
                    {
                        idNum = -1;
                    }
                    if (idNum > idNumMax)
                    {
                        idNumMax = idNum;
                    }
                }
            }
            idNumMax++;
            id = searchString + idNumMax.ToString();
            return id;
        }

        private void openProjectFile()
        {
            openFileDialog1.DefaultExt = "smgx";
            openFileDialog1.Filter = "Scenario Manager files (*.smgx)|*.smgx";
            if (Directory.Exists(_projectFileDirectory))
            {
                openFileDialog1.InitialDirectory = _projectFileDirectory;
            }
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                if (File.Exists(fileName))
                {
                    _savedCursor = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    int progress = 10;
                    progressBar.Value = progress;
                    progressBar.Visible = true;
                    StatusLabel.Text = "Opening Project File...";
                    Refresh();
                    _projectFileDirectory = Path.GetDirectoryName(fileName);
                    _projectFileName = Path.GetFileName(fileName);
                    //
                    // Read file and deserialize project
                    SMProject smProject;
                    XmlSerializer xs = new XmlSerializer(typeof(SMProject));
                    progress = 20;
                    progressBar.Value = progress;
                    using (Stream s = File.OpenRead(fileName))
                        smProject = (SMProject)xs.Deserialize(s);
                    if (smProject.SmProjectSettings.MasterNameFile != "")
                    {
                        string relativePath = smProject.SmProjectSettings.MasterNameFile;
                        smProject.SmProjectSettings.MasterNameFile = 
                            FileUtil.Relative2Absolute(relativePath, _projectFileDirectory);
                    }
                    for (int i = 0; i < smProject.SmProjectSettings.NameFiles.Count; i++)
                    {
                        string relativePath = smProject.SmProjectSettings.NameFiles[i];
                        smProject.SmProjectSettings.NameFiles[i] = 
                            FileUtil.Relative2Absolute(relativePath, _projectFileDirectory);
                    }
                    int break0 = 30;
                    int break1 = 70;
                    int break2 = 90;
                    int inc0;
                    progress = break0;
                    progressBar.Value = progress;
                    //
                    SmProjectSettings = (SMProjectSettings)smProject.SmProjectSettings.Clone();
                    GridShapefileAbsolutePath = 
                        FileUtil.Relative2Absolute(smProject.GridShapefileRelativePath,_projectFileDirectory);
                    readNameFile();
                    SimulationStartTime = smProject.SimulationStartTime;
                    //
                    // Populate _scenarioElements
                    _scenarioElements.Clear();
                    if (smProject.ScenarioElements.Count > 0)
                    {
                        int numElements = smProject.ScenarioElements.Count;
                        inc0 = (break1 - break0) / numElements;
                        foreach (SerializableScenarioElement scenarioElement in smProject.ScenarioElements)
                        {
                            // Make Scenario deserializes entire scenario, instantiating not only the 
                            // Scenario, but all included child Packages and FeatureSets
                            Scenario scenario = makeScenario(scenarioElement, _tagStore, _projectFileDirectory);
                            // Add scenario and all contained Packages and FeatureSets to _scenarioElements
                            _scenarioElements.Add((ITaggable)scenario);
                            int numPkgs = scenario.Items.Count;
                            if (numPkgs > 0)
                            {
                                int inc1 = inc0 / numPkgs;
                                foreach (Package package in scenario.Items)
                                {
                                    _scenarioElements.Add((ITaggable)package);
                                    if (package.Items.Count > 0)
                                    {
                                        int numFS = package.Items.Count;
                                        int inc2 = inc1 / numFS;
                                        foreach (FeatureSet featureSet in package.Items)
                                        {
                                            progress = progress + inc2;
                                            progressBar.Value = progress;
                                            if (featureSet.DefaultLayer < 1)
                                            {
                                                featureSet.DefaultLayer = 1;
                                            }
                                            _scenarioElements.Add((ITaggable)featureSet);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //
                    // Populate treeView1 and _tagStore
                    treeView1.Nodes.Clear();
                    _tagStore.Items.Clear();
                    treeView1.ImageIndex = smProject.TreeView.ImageIndex;
                    if (smProject.TreeView.Items.Count > 0)
                    {
                        int numItems = smProject.TreeView.Items.Count;
                        inc0 = (break2 - break1) / numItems;
                        foreach (SerializableTreeNode node in smProject.TreeView.Items)
                        {
                            TreeNode treeNode = makeTreeNodeAndLink(node, _tagStore);
                            treeView1.Nodes.Add(treeNode);
                            progress = progress + inc0;
                            progressBar.Value = progress;
                        }
                    }
                    progress = 100;
                    progressBar.Value = progress;
                    refreshUI();
                    progressBar.Visible = false;
                    progressBar.Value = 0;
                    StatusLabel.Text = "";
                    Cursor.Current = _savedCursor;
                }
            }
            enablePaste(false);
        }

        private bool readNameFile()
        {
            bool OK = true;
            string msg = "";
            if (_projectFileDirectory == "" && _smProjectSettings.MasterNameFile != "")
            {
                string pathRoot = Path.GetPathRoot(_smProjectSettings.MasterNameFile);
                if (pathRoot != "")
                {
                    _projectFileDirectory = Path.GetDirectoryName(_smProjectSettings.MasterNameFile);
                }
            }
            string namefileAbsolutePath = FileUtil.Relative2Absolute(_smProjectSettings.MasterNameFile,
                                                             _projectFileDirectory);
            if (!File.Exists(namefileAbsolutePath))
            {
                msg = "Warning: MODFLOW name file not specified or not valid";
                MessageBox.Show(msg);
                OK = false;
            }
            if (OK)
            {
                if (_projectFileDirectory == "" && _smProjectSettings.MasterNameFile != "")
                {
                    string pathRoot = Path.GetPathRoot(_smProjectSettings.MasterNameFile);
                    if (pathRoot != "")
                    {
                        _projectFileDirectory = Path.GetDirectoryName(_smProjectSettings.MasterNameFile);
                    }
                }
                namefileAbsolutePath = FileUtil.Relative2Absolute(_smProjectSettings.MasterNameFile,
                                                          _projectFileDirectory);
                if (File.Exists(namefileAbsolutePath))
                {
                    _namefileInfo = new NamefileInfo(namefileAbsolutePath);
                    if (_namefileInfo != null)
                    {
                        _freeFormat = getFreeFromBasFile(_namefileInfo);
                        DiscretizationFileAbsolutePath = FileUtil.GetRelativePath(_namefileInfo.GetEntry("DIS").Filename,
                                                                                  _projectFileDirectory);
                        if (_namefileInfo.ContainsUnknownAccess())
                        {
                            msg = "Warning: Master Name File contains one or more " +
                            "entries for which access is unknown.  Ensure all DATA " +
                            "and DATA(BINARY) files have 'OLD' or 'REPLACE' specified.";
                            MessageBox.Show(msg);
                            OK = false;
                        }
                        else
                        {
                            OK = true;
                        }
                    }
                    assignDiscretizationFile();
                }
                else
                {
                    msg = "Warning: MODFLOW name file '" + namefileAbsolutePath + "' does not exist.";
                    MessageBox.Show(msg);
                    OK = false;
                }
            }
            return OK;
        }

        private void saveProject()
        {
            if (Directory.Exists(_projectFileDirectory) && (_projectFileName != ""))
            {
                Directory.SetCurrentDirectory(_projectFileDirectory);
                List<SerializableScenarioElement> serElements = new List<SerializableScenarioElement>();
                foreach (ITaggable item in _scenarioElements)
                {
                    if (item.ElemType == ElementType.Scenario)
                    {
                        serElements.Add(new SerializableScenarioElement(item, _projectFileDirectory));
                    }
                }
                //
                SerializableTreeNode serTreeView = new SerializableTreeNode();
                serTreeView.ImageIndex = 0;
                foreach (TreeNode node in treeView1.Nodes)
                {
                    serTreeView.Items.Add(new SerializableTreeNode(node));
                }
                //
                string gridShapefileRelativePath = FileUtil.Absolute2Relative(GridShapefileAbsolutePath, 
                                                                              _projectFileDirectory);
                SMProject smProject = new SMProject(serElements, serTreeView,
                                                    gridShapefileRelativePath,
                                                    SimulationStartTime, _smProjectSettings);
                smProject.SmProjectSettings.ConvertToRelative(_projectFileDirectory);
                XmlSerializer xs = new XmlSerializer(typeof(SMProject));
                using (Stream s = File.Create(_projectFileName))
                    xs.Serialize(s, smProject);
                saveUserSettings();
                clearUndoChain();
                StatusLabel.Text = "Project file saved.";
            }
            else
            {
                saveProjectAs();
            }
            enablePaste(false);
            refreshUI(false);
        }

        private void saveProjectAs()
        {
            saveFileDialog1.FileName = _projectFileName;
            saveFileDialog1.DefaultExt = "smgx";
            saveFileDialog1.Filter = "Scenario Manager XML files (*.smgx)|*.smgx";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                // define file and directory names
                string fileName = saveFileDialog1.FileName;
                _projectFileName = Path.GetFileName(fileName);
                string newProjectDirectory = Path.GetDirectoryName(fileName);
                if (Directory.Exists(newProjectDirectory))
                {
                    convertTaggablePaths(_projectFileDirectory, newProjectDirectory);
                    // Convert paths not in scenario elements to absolute paths
                    assignAbsolutePaths(newProjectDirectory);
                    _projectFileDirectory = newProjectDirectory;
                    saveProject();
                }
            }
            refreshUI(false);
        }
        private void getUserSettings()
        {
            Properties.Settings.Default.Reload();
            // If this is the first time getting settings since new installation of app, 
            // copy settings from previous installation
            if (Properties.Settings.Default.UpgradeNeeded)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeNeeded = false;
            }
            _smProjectSettings.ModflowExecutable = Properties.Settings.Default.ModflowPath;
            _smProjectSettings.MaxSimultaneousRuns = Properties.Settings.Default.MaxSimultaneousRuns;
        }
        private void saveUserSettings()
        {
            Properties.Settings.Default.ModflowPath = _smProjectSettings.ModflowExecutable;
            Properties.Settings.Default.MaxSimultaneousRuns = _smProjectSettings.MaxSimultaneousRuns;
            Properties.Settings.Default.Save();
        }

        #endregion Project-related methods

        #region User-interface methods

        private void enableControls(bool enable)
        {
            this.MainMenu.Enabled = enable;
            this.treeView1.Enabled = enable;
            this.tbDescription.Enabled = enable;
        }

        private void exit()
        {
            if (_undoChain.Dirty)
            {
                string msg = "Project has unsaved changes.  Save now?";
                DialogResult dr = MessageBox.Show(msg, "Scenario Manager", 
                                                  MessageBoxButtons.YesNoCancel, 
                                                  MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.No:
                        _okToClose = true;
                        Close();
                        break;
                    case DialogResult.Yes:
                        saveProject();
                        exit();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _okToClose = true;
                Close();
            }
        }
        private void initializeUI()
        {
            _projectFileName = "";
            _gridShapefileAbsolutePath.String = "";
            _discretizationFileAbsolutePath.String = "";
            _namefileAbsolutePath.String = "";
            SimulationStartTime = defaultSimulationTime();
            _simulationEndTime = defaultSimulationTime();
            _scenarioElements.Clear();
            treeView1.Nodes.Clear();
            _tagStore.Items.Clear();
            tbDescription.Text = "";
            lblName.Text = "";
            lblTitle.Text = "Summary";
            StatusLabel.Text = "";
            progressBar.Visible = false;
            progressBar.Value = 0;
            clearUndoChain();
            refreshUI();
        }

        private void refreshUI()
        {
            refreshUI(true);
        }

        private void refreshUI(bool clearStatus)
        {
            if (_projectFileName != "" & _projectFileDirectory != "")
            {
                string path = FileUtil.Relative2Absolute(_projectFileName, _projectFileDirectory);
                this.Text = "USGS Scenario Manager: [" + path + "]";
            }
            else
            {
                this.Text = "USGS Scenario Manager";
            }
            menuItemSimulationStartTime.Enabled = true;
            menuItemProjectModelGrid.Enabled = true;
            menuItemProjectSettings.Enabled = true;
            if (clearStatus)
            {
                StatusLabel.Text = "";
            }
            refreshUndoRedoMenus();
            refreshSummary();
            Refresh();
        }

        private void refreshSaveMenuItem()
        {
            if (_undoChain.Dirty)
            {
                ((ToolStripMenuItem)MainMenu.Items[menuItemFile.Name]).DropDownItems[menuItemFileSave.Name].Enabled = true;
                _okToClose = false;
            }
            else
            {
                ((ToolStripMenuItem)MainMenu.Items[menuItemFile.Name]).DropDownItems[menuItemFileSave.Name].Enabled = false;
                _okToClose = true;
            }
        }

        private void refreshSummary()
        {
            // Refresh the right panel of the main form
            _refreshingSummary = true;
            tbDescription.Text = "";
            int level = -1;
            labelCbcFlag.Visible = false;
            textBoxCbcFlag.Visible = false;
            btnGetUnitFromNameFile.Visible = false;
            if (treeView1.SelectedNode == null)
            {
                tbDescription.Enabled = false;
            }
            else
            {
                tbDescription.Enabled = true;
                lblName.Text = treeView1.SelectedNode.Text;
                switch (treeView1.SelectedNode.Level)
                {
                    case 0:
                        // Element is a Scenario
                        lblTitle.Text = "Scenario Summary";
                        lblNam.Text = "Scenario ID:";
                        break;
                    case 1:
                        // Element is a Package
                        lblNam.Text = "Name:";
                        if (treeView1.SelectedNode.Tag != null)
                        {
                            labelCbcFlag.Visible = true;
                            textBoxCbcFlag.Visible = true;
                            btnGetUnitFromNameFile.Visible = true;
                            Package pkg = (Package)GetScenarioElementByTag((int)treeView1.SelectedNode.Tag);
                            if (pkg != null)
                            {
                                switch (pkg.Type)
                                {
                                    case PackageType.RiverType:
                                        lblTitle.Text = "River Package Summary";
                                        labelCbcFlag.Text = "IRIVCB:";
                                        break;
                                    case PackageType.WellType:
                                        lblTitle.Text = "Well Package Summary";
                                        labelCbcFlag.Text = "IWELCB:";
                                        break;
                                    case PackageType.ChdType:
                                        lblTitle.Text = "CHD Package Summary";
                                        labelCbcFlag.Visible = false;
                                        textBoxCbcFlag.Visible = false;
                                        btnGetUnitFromNameFile.Visible = false;
                                        break;
                                    case PackageType.RchType:
                                        lblTitle.Text = "Recharge Package Summary";
                                        labelCbcFlag.Text = "IRCHCB:";
                                        break;
                                    case PackageType.GhbType:
                                        lblTitle.Text = "GHB Package Summary";
                                        labelCbcFlag.Text = "IGHBCB:";
                                        break;
                                    default:
                                        lblTitle.Text = "Summary for package of unknown type";
                                        labelCbcFlag.Text = "CBCFlag:";
                                        break;
                                }
                                textBoxCbcFlag.Text = pkg.CbcFlag.ToString();
                            }
                            else
                            {
                                lblTitle.Text = "Package Summary";
                            }
                        }
                        else
                        {
                            lblTitle.Text = "Package Summary";
                        }
                        break;
                    case 2:
                        // Element is a FeatureSet
                        lblNam.Text = "Name:";
                        if (treeView1.SelectedNode.Tag != null)
                        {
                            FeatureSet fs = (FeatureSet)GetScenarioElementByTag((int)treeView1.SelectedNode.Tag);
                            if (fs != null)
                            {
                                switch (fs.Type)
                                {
                                    case PackageType.RiverType:
                                        lblTitle.Text = "River Feature Set Summary";
                                        break;
                                    case PackageType.WellType:
                                        lblTitle.Text = "Well Feature Set Summary";
                                        break;
                                    case PackageType.ChdType:
                                        lblTitle.Text = "CHD Feature Set Summary";
                                        break;
                                    case PackageType.RchType:
                                        lblTitle.Text = "Recharge Feature Set Summary";
                                        break;
                                    case PackageType.GhbType:
                                        lblTitle.Text = "GHB Feature Set Summary";
                                        break;
                                    default:
                                        lblTitle.Text = "Summary for feature set of unknown type";
                                        break;
                                }
                            }
                        }
                        else
                        {
                            lblTitle.Text = "Feature Set Summary";
                        }
                        break;
                }
                if (treeView1.SelectedNode.Tag != null)
                {
                    int tag2 = (int)treeView1.SelectedNode.Tag;
                    ITaggable e = GetScenarioElementByTag(tag2);
                    if (e != null)
                    {
                        if (e.Tag > 0)
                        {
                            int tag = (int)GetScenarioElementByTag(tag2).Tag;
                        }
                        tbDescription.Text = e.Description;
                        ElementType eType = e.ElemType;
                    }
                }
            }

            if (level == 1)
            {
                btnGetUnitFromNameFile.Enabled = (_namefileInfo != null);
            }
            _refreshingSummary = false;
        }

        private void refreshRedoMenu()
        {
            if (_undoChain.Count > _maxUndoCount)
            {
                _maxUndoCount = _undoChain.Count;
            }
            string itemName0 = menuItemEdit.Name;
            string itemName1 = menuItemEditRedo.Name;
            if (_undoChain.Count == _maxUndoCount)
            {
                ((ToolStripMenuItem)MainMenu.Items[itemName0]).DropDownItems[itemName1].Enabled = false;
            }
            else
            {
                ((ToolStripMenuItem)MainMenu.Items[itemName0]).DropDownItems[itemName1].Enabled = true;
            }
            refreshSaveMenuItem();
        }

        private void refreshUndoMenu()
        {
            if (_undoChain.Count > _maxUndoCount)
            {
                _maxUndoCount = _undoChain.Count;
            }
            string itemName0 = menuItemEdit.Name;
            string itemName1 = menuItemEditUndo.Name;
            if (_undoChain.Count == 0)
            {
                ((ToolStripMenuItem)MainMenu.Items[itemName0]).DropDownItems[itemName1].Enabled = false;
            }
            else
            {
                ((ToolStripMenuItem)MainMenu.Items[itemName0]).DropDownItems[itemName1].Enabled = true;
            }
            refreshSaveMenuItem();
        }

        private void refreshUndoRedoMenus()
        {
            refreshUndoMenu();
            refreshRedoMenu();
        }

        #endregion User-interface methods

        #region BackgroundWorker Methods

        private BackgroundWorker newBackgroundWorker()
        {
            BackgroundWorker newBackgroundWorker = new BackgroundWorker();
            newBackgroundWorker.WorkerReportsProgress = true;
            newBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
            newBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            newBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            return newBackgroundWorker;
        }

        private void setUpBackgroundWorker(BackgroundWorker worker)
        {
            if (worker != null)
            {
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
                worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            if (e.Argument is Package)
            {
                Package package = (Package)e.Argument;
                _exportingPackage = true;
                if (exportPackage(package, worker))
                {
                    _backgroundCompletionMessage = "Package export completed";
                }
                else
                {
                    _backgroundCompletionMessage = "Package export failed";
                }
            }
            else if (e.Argument is Scenario)
            {
                Scenario scenario = (Scenario)e.Argument;
                _exportingScenario = true;
                if (scenario.Message == "Run")
                {
                    if (exportScenarioAndRunSimulation(scenario, worker))
                    {
                        _backgroundCompletionMessage = "Scenario exported and simulation completed";
                    }
                    else
                    {
                        _backgroundCompletionMessage = "Scenario export and simulation failed. " + scenario.Message;
                        MessageBox.Show(scenario.Message);
                    }
                }
                else
                {
                    if (exportScenario(scenario, worker))
                    {
                        _backgroundCompletionMessage = "Scenario export completed";
                    }
                    else
                    {
                        _backgroundCompletionMessage = "Scenario export failed. " + scenario.Message;
                        MessageBox.Show(scenario.Message);
                    }
                }
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!_runningAllScenarios)
            {
                if (e.ProgressPercentage > progressBar.Value)
                {
                    progressBar.Value = e.ProgressPercentage;
                }
            }
            Application.DoEvents();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusLabel.Text = _backgroundCompletionMessage;
            if (_runningAllScenarios)
            {
                _runsDone++;
                StatusLabel.Text = StatusLabel.Text = " Completed " + _runsDone.ToString() 
                                   + " of " + _numScenarios.ToString() + " model runs";
            }
            if (_runningAllScenarios)
            {
                if (progressBar.Value + _progressInc < progressBar.Maximum)
                {
                    progressBar.Value = progressBar.Value + _progressInc;
                }
                else
                {
                    progressBar.Value = progressBar.Maximum;
                }
                if (_runsDone == _numScenarios)
                {
                    _runningAllScenarios = false;
                    if (_exportingPackage)
                    {
                        _exportingPackage = false;
                    }
                    if (_exportingScenario)
                    {
                        _exportingScenario = false;
                    }
                    UseWaitCursor = false;
                    progressBar.Visible = false;
                    progressBar.Value = 0;
                    enableControls(true);
                }
            }
            else
            {
                if (_exportingPackage)
                {
                    _exportingPackage = false;
                }
                if (_exportingScenario)
                {
                    _exportingScenario = false;
                }
                UseWaitCursor = false;
                progressBar.Visible = false;
                progressBar.Value = 0;
                enableControls(true);
            }
        }

        #endregion BackgroundWorker Methods

        private void menuItemProject_DropDownOpening(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
        }

        private void menuItemFile_DropDownOpening(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
        }

        private void menuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            menuItemEditRename.Enabled = false;
            menuItemEditAddNewPackage.Enabled = false;
            menuItemEditAddNewRiverPackage.Enabled = false;
            menuItemEditAddNewWellPackage.Enabled = false;
            menuItemEditAddNewChdPackage.Enabled = false;
            menuItemEditAddNewFeatureSet.Enabled = false;

            menuItemEditCopy.Enabled = false;
            menuItemEditCopyScenario.Enabled = false;
            menuItemEditCut.Enabled = false;
            menuItemEditDelete.Enabled = false;
            menuItemEditSelectedItem.Enabled = false;
            if (treeView1.SelectedNode != null)
            {
                menuItemEditRename.Enabled = true;
                int level = (int)treeView1.SelectedNode.Level;
                switch (level)
                {
                    case 0:
                        menuItemEditAddNewPackage.Enabled = true;
                        menuItemEditAddNewRiverPackage.Enabled = okToAddNodeOfType(PackageType.RiverType);
                        menuItemEditAddNewWellPackage.Enabled = okToAddNodeOfType(PackageType.WellType);
                        menuItemEditAddNewChdPackage.Enabled = okToAddNodeOfType(PackageType.ChdType);
                        menuItemEditAddNewRchPackage.Enabled = okToAddNodeOfType(PackageType.RchType);
                        menuItemEditAddNewGhbPackage.Enabled = okToAddNodeOfType(PackageType.GhbType);
                        menuItemEditCopyScenario.Enabled = true;
                        menuItemEditDelete.Enabled = true;
                        menuItemEditPaste.Enabled = (_pasteLevel == 0);
                        break;
                    case 1:
                        contextMenuAddNewFeatureSet.Enabled = true;
                        menuItemEditAddNewFeatureSet.Enabled = true;
                        menuItemEditCopy.Enabled = true;
                        menuItemEditCut.Enabled = true;
                        menuItemEditDelete.Enabled = true;
                        menuItemEditPaste.Enabled = (_pasteLevel == 1);
                        break;
                    case 2:
                        menuItemEditCopy.Enabled = true;
                        menuItemEditCut.Enabled = true;
                        menuItemEditDelete.Enabled = true;
                        menuItemEditSelectedItem.Enabled = true;
                        menuItemEditPaste.Enabled = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void menuItemView_DropDownOpening(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            descriptionOfSelectedElementToolStripMenuItem.Enabled = (treeView1.SelectedNode != null);
        }

        private void menuItemImport_DropDownOpening(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
        }

        private void menuItemExport_DropDownOpening(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            // Enable or disable menu items on the Export menu
            bool enableScenarioExportMenu = false;
            bool enablePackageExportMenu = false;
            ITaggable scnElem;
            // Enable select menu items
            if (treeView1.SelectedNode != null)
            {
                scnElem = (GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
                if (scnElem != null)
                {
                    switch (scnElem.ElemType)
                    {
                        case ElementType.Scenario:
                            enableScenarioExportMenu = true;
                            break;
                        case ElementType.Package:
                            enablePackageExportMenu = true;
                            break;
                        case ElementType.FeatureSet:
                            break;
                        default:
                            break;
                    }
                }
            }
            menuItemExportPackage.Enabled = enablePackageExportMenu;
            menuItemExportScenario.Enabled = enableScenarioExportMenu;
            menuItemExportScenarioAndRunSimulation.Enabled = enableScenarioExportMenu;
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
        }

        private void tbDescription_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
        }

        private void tbDescription_MouseClick(object sender, MouseEventArgs e)
        {
            StatusLabel.Text = "";
        }

        private void tbDescription_MouseDown(object sender, MouseEventArgs e)
        {
            StatusLabel.Text = "";
        }

        private void menuItemEditAddNewRiverPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.RiverType))
            {
                addPackageTreeItem(PackageType.RiverType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void menuItemEditAddNewWellPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.WellType))
            {
                addPackageTreeItem(PackageType.WellType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void menuItemEditAddNewChdPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.ChdType))
            {
                addPackageTreeItem(PackageType.ChdType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void menuItemEditAddNewRchPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.RchType))
            {
                addPackageTreeItem(PackageType.RchType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void menuItemEditAddNewGhbPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.GhbType))
            {
                addPackageTreeItem(PackageType.GhbType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void contextMenuCopyScenario_Click(object sender, EventArgs e)
        {
            copyScenarioTreeItem();
        }

        private void contextMenuCut_Click(object sender, EventArgs e)
        {
            cutTreeItem();
        }

        private void contextMenuCopy_Click(object sender, EventArgs e)
        {
            copyTreeItem();
        }

        private void contextMenuPaste_Click(object sender, EventArgs e)
        {
            pasteTreeItem(true);
        }

        private void contextMenuDelete_Click(object sender, EventArgs e)
        {
            deleteTreeItem();
        }

        private void contextMenuStripNode_Opening(object sender, CancelEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                int level = treeView1.SelectedNode.Level;
                int imageIndex = treeView1.SelectedNode.ImageIndex;
                StatusLabel.Text = "";
                contextMenuRename.Enabled = true;

                contextMenuCut.Enabled = false;
                contextMenuCopy.Enabled = false;
                contextMenuCopyScenario.Enabled = false;
                contextMenuAddPackage.Enabled = false;
                contextMenuEdit.Enabled = false;
                contextMenuAddNewFeatureSet.Enabled = false;
                if (level == 0)
                {
                    contextMenuCopyScenario.Enabled = true;
                    contextMenuAddPackage.Enabled = true;
                    contextMenuAddRiverPackage.Enabled = okToAddNodeOfType(PackageType.RiverType);
                    contextMenuAddWellPackage.Enabled = okToAddNodeOfType(PackageType.WellType);
                    contextMenuAddChdPackage.Enabled = okToAddNodeOfType(PackageType.ChdType);
                    contextMenuAddRchPackage.Enabled = okToAddNodeOfType(PackageType.RchType);
                    contextMenuAddGhbPackage.Enabled = okToAddNodeOfType(PackageType.GhbType);
                    contextMenuPaste.Enabled = (_pasteLevel == 0);
                }
                else if (level == 1)
                {
                    contextMenuCut.Enabled = true;
                    contextMenuCopy.Enabled = true;
                    contextMenuAddNewFeatureSet.Enabled = true;
                    contextMenuPaste.Enabled = (_pasteLevel == 1);
                }
                else if (level == 2)
                {
                    contextMenuCut.Enabled = true;
                    contextMenuCopy.Enabled = true;
                    contextMenuEdit.Enabled = true;
                    contextMenuPaste.Enabled = false;
                }
            }
        }

        private void menuItemEditRename_Click(object sender, EventArgs e)
        {
            renameTreeItem();
        }

        private void menuItemEditSelectedItem_Click(object sender, EventArgs e)
        {
            editSelectedItem();
        }

        private void menuItemEditCut_Click(object sender, EventArgs e)
        {
            cutTreeItem();
        }

        private void menuItemEditCopy_Click(object sender, EventArgs e)
        {
            copyTreeItem();
        }

        private void menuItemEditPaste_Click(object sender, EventArgs e)
        {
            pasteTreeItem(true);
        }

        private void menuItemEditDelete_Click(object sender, EventArgs e)
        {
            deleteTreeItem();
        }

        private void menuItemEditCopyScenario_Click(object sender, EventArgs e)
        {
            copyScenarioTreeItem();
        }

        private void menuItemExportAndRunAllScenarios_Click(object sender, EventArgs e)
        {
            exportAndRunAllScenarios();
        }

        private void exportAndRunAllScenarios()
        {
            if (defineModelGrid(false))
            {
                if (okToExport())
                {
                    _runningAllScenarios = true;
                    _runsDone = 0;
                    progressBar.Value = 0;
                    progressBar.Visible = true;
                    Application.DoEvents();
                    // Count the number of scenarios in _scenarioElements
                    _numScenarios = 0;
                    for (int i = 0; i < _scenarioElements.Count; i++)
                    {
                        if (_scenarioElements[i] is Scenario)
                        {
                            _numScenarios++;
                        }
                    }
                    // Increment progress bar once for each scenario started, and 
                    // once for each scenario completed
                    _progressInc = progressBar.Maximum / (_numScenarios * 2);

                    // Allocate and initialize arrays for keeping track of scenarios
                    bool[] runsStarted = new bool[_numScenarios];
                    int[] scenarioIndex = new int[_numScenarios];
                    int k;
                    for (k = 0; k < _numScenarios; k++)
                    {
                        runsStarted[k] = false;
                        scenarioIndex[k] = -1;
                    }

                    // Populate scenarioIndex[] with pointers to elements in _scenarioElements that contain scenarios
                    k = 0;
                    for (int i = 0; i < _scenarioElements.Count; i++)
                    {
                        if (_scenarioElements[i] is Scenario)
                        {
                            scenarioIndex[k] = i;
                            k++;
                        }
                    }

                    // Export and run scenarios
                    while (anyFalse(runsStarted))
                    {
                        for (k = 0; k < _numScenarios; k++)
                        {
                            if (!runsStarted[k])
                            {
                                if (exportAndRunScenario((Scenario)_scenarioElements[scenarioIndex[k]]))
                                {
                                    runsStarted[k] = true;
                                    if (progressBar.Value + _progressInc <= progressBar.Maximum)
                                    {
                                        progressBar.Value = progressBar.Value + _progressInc;
                                    }
                                    else
                                    {
                                        progressBar.Value = progressBar.Maximum;
                                    }
                                }
                                Application.DoEvents();
                            }
                        }
                    }
                }
            }
        }

        private bool anyFalse(bool[] boolArray)
        {
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (!boolArray[i])
                {
                    return true;
                }
            }
            return false;
        }

        private int countTrue(bool[] boolArray)
        {
            int k = 0;
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (boolArray[i])
                {
                    k++;
                }
            }
            return k;
        }

        private bool okToExport()
        {
            bool OK = true;
            string message = "";
            if (_namefileInfo != null)
            {
                _namefileInfo.Items.Clear();
            }
            if (Directory.Exists(_projectFileDirectory))
            {
                if (SmProjectSettings.MasterNameFile != null)
                {
                    string masterNamefilePath =
                        FileUtil.Relative2Absolute(SmProjectSettings.MasterNameFile,
                                                   _projectFileDirectory);
                    _namefileInfo = new NamefileInfo(masterNamefilePath);
                    if (_namefileInfo.ContainsUnknownAccess())
                    {
                        message = "Error: Master Name File contains one or " +
                            "more entries for which access is unknown.  Ensure all DATA " +
                            "and DATA(BINARY) files have 'OLD' or 'REPLACE' specified.";
                        MessageBox.Show(message);
                        return false;
                    }
                }
                else
                {
                    message = "Master Name File needs to be defined";
                    MessageBox.Show(message);
                    return false;
                }
            }
            else
            {
                message = "Scenario Manager project needs to be saved first";
                MessageBox.Show(message);
                return false;
            }
            if (OK)
            {
                OK = getDiscretizationData();
                if (!OK)
                {
                    message = "Discretization file needs to be imported";
                    MessageBox.Show(message);
                }
            }
            return OK;
        }

        private void setUpImageMakerBackgroundWorker(BackgroundWorker backgroundWorker)
        {
            if (backgroundWorker != null)
            {
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += new DoWorkEventHandler(imageMakerBackgroundWorker_DoWork);
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(imageMakerBackgroundWorker_ProgressChanged);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(imageMakerBackgroundWorker_RunWorkerCompleted);
                GlobalStaticVariables.GlobalBackgroundWorker = backgroundWorker;
                GlobalStaticVariables.MyDelegate a = new GlobalStaticVariables.MyDelegate(Application.DoEvents);
                GlobalStaticVariables.DoEvents = a;
            }
        }

        private void imageMakerBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            Dataset ds = (Dataset)e.Argument;
            e.Result = GdalHelper.GetBitmapBuffered(ds, 0, worker);
        }

        private void imageMakerBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < progressBar.Minimum)
            {
                progressBar.Value = progressBar.Minimum;
                StatusLabel.Text = "Generating image (progress < min)...";
            }
            else if (e.ProgressPercentage > progressBar.Maximum)
            {
                progressBar.Value = progressBar.Maximum;
                StatusLabel.Text = "Generating image (progress > max)...";
            }
            else
            {
                progressBar.Value = e.ProgressPercentage;
            }
        }

        private void imageMakerBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visible = false;
            progressBar.Value = 0;
            enableControls(true);
            UseWaitCursor = false;
            this.StatusLabel.Text = "";
            if (e.Result is Bitmap)
            {
                GlobalStaticVariables.GlobalBitmap = (Bitmap)e.Result;
            }
        }

        private void documentationOfSelectedElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                StringListViewer viewer = new StringListViewer();
                int tag = (int)treeView1.SelectedNode.Tag;
                ITaggable taggable = GetScenarioElementByTag(tag);
                List<string> listString;
                if (taggable != null)
                {
                    if (taggable is Scenario)
                    {
                        // Selected node is a Scenario
                        Scenario scenario = (Scenario)taggable;
                        listString = scenario.Describe();
                        viewer.Text = "Documentation of Scenario \"" + scenario.GetScenarioID() + "\"";
                    }
                    else if (taggable is Package)
                    {
                        // Selected node is a Package
                        Package package = (Package)taggable;
                        listString = package.Describe();
                        viewer.Text = "Documentation of Package \"" + package.Name + "\"";
                    }
                    else if (taggable is FeatureSet)
                    {
                        // Selected node is a FeatureSet
                        FeatureSet featureSet = (FeatureSet)taggable;
                        listString = featureSet.Describe();
                        viewer.Text = "Documentation of Feature Set \"" + featureSet.Name + "\"";
                    }
                    else
                    {
                        listString = new List<string>();
                        listString.Add("Scene class unknown");
                    }
                    listString.Add("");
                    viewer.StringList = listString;
                    viewer.Show();
                }
            }
        }

        private void treeView1_ControlAdded(object sender, ControlEventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            AboutBox aboutBox = new AboutBox(assembly);
            aboutBox.ShowDialog();
        }

        private void contextMenuAddRchPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.RchType))
            {
                addPackageTreeItem(PackageType.RchType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void contextMenuAddGhbPackage_Click(object sender, EventArgs e)
        {
            if (okToAddNodeOfType(PackageType.GhbType))
            {
                addPackageTreeItem(PackageType.GhbType, GetScenarioElementByTag((int)treeView1.SelectedNode.Tag));
            }
        }

        private void contextMenuAddNewFeatureSet_Click(object sender, EventArgs e)
        {
            addFeatureSetToSelectedPackageNode();
        }

        private void addFeatureSetToSelectedPackageNode()
        {
            try
            {
                PackageType packageType = (PackageType)treeView1.SelectedNode.ImageIndex;
                addFeatureSetTreeItem(packageType);
            }
            catch
            {
            }
        }

        private void menuItemEditAddNewFeatureSet_Click(object sender, EventArgs e)
        {
            addFeatureSetToSelectedPackageNode();
        }

        private void textBoxCbcFlag_Leave(object sender, EventArgs e)
        {
            try
            {
                int cbcflag = Convert.ToInt32(textBoxCbcFlag.Text);
                if (cbcflag != _cbcFlagTemp)
                {
                    // Create an Undo item for the change of CbcFlag
                    try
                    {
                        int tag = (int)treeView1.SelectedNode.Tag;
                        ITaggable scenarioElement = GetScenarioElementByTag(tag);
                        if (scenarioElement is Package)
                        {
                            Package package = (Package)scenarioElement;
                            package.CbcFlag = cbcflag;
                            addCbcFlagChangeUndoItem(package, _cbcFlagTemp, cbcflag);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
                string line = "Error: " + textBoxCbcFlag.Text + " cannot be converted to an integer!";
                MessageBox.Show(line);
                textBoxCbcFlag.Focus();
            }
        }

        private void btnGetUnitFromNameFile_Click(object sender, EventArgs e)
        {
            // Open a dialog that gets populated with entries from the name file
            // (_nameFileInfo) for which file type is DATA(BINARY) and action is REPLACE.
            // Allow user to select a file/unit number.  When clicked, that unit number
            // goes into textBoxCbcFlag.Text
            int oldUnit;
            try
            {
                oldUnit = Convert.ToInt32(textBoxCbcFlag.Text);
            }
            catch
            {
                oldUnit = 0;
            }
            NameFileUnitPicker nameFileUnitPicker = new NameFileUnitPicker(_namefileInfo, oldUnit);
            DialogResult dr = nameFileUnitPicker.ShowDialog();
            if (dr == DialogResult.OK)
            {
                int newUnit = nameFileUnitPicker.Unit;
                textBoxCbcFlag.Text = newUnit.ToString();
                int tag = (int)treeView1.SelectedNode.Tag;
                ITaggable scenarioElement = GetScenarioElementByTag(tag);
                if (scenarioElement is Package)
                {
                    Package package = (Package)scenarioElement;
                    package.CbcFlag = newUnit;
                    if (newUnit != oldUnit)
                    {
                        addCbcFlagChangeUndoItem(package, oldUnit, newUnit);
                    }
                }
            }
        }

        private void textBoxCbcFlag_Enter(object sender, EventArgs e)
        {
            try
            {
                _cbcFlagTemp = Convert.ToInt32(textBoxCbcFlag.Text);
            }
            catch
            {
                _cbcFlagTemp = 0;
            }
        }

        private void scenarioManagerHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string exePath = Application.ExecutablePath;
            string exeDir = Path.GetDirectoryName(exePath);
            string helpFilename = "ScenarioToolsHelp.chm";
            string helpPath = exeDir + Path.DirectorySeparatorChar + helpFilename;
            string helpUrl = "file://" + helpPath;
            HelpNavigator helpNavKeyIndx = HelpNavigator.KeywordIndex;
            Help.ShowHelp(this, helpUrl, helpNavKeyIndx, "Scenario Manager");
        }

    }
}
