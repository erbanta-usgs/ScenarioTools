namespace ModflowOutputViewer
{
    partial class ModflowOutputViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModflowOutputViewer));
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.menuMainFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileOpenDataset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileCloseDataset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainCloseCurrentFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuMainFileAddFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuMainFileNewBasemap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileAddBasemap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileRemoveBasemap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileSaveBasemap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuMainFileExport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileExportCurrentFileXml = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileExportShapefiles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileSaveBinaryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuMainFilePrintPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFilePrintPDF = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainFileSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuMainFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainMap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainMapSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuMainHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelZoomTo = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelView = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelEdit = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelAnalysis = new System.Windows.Forms.ToolStripLabel();
            this.cboGriddedValuesDisplayOption = new System.Windows.Forms.ToolStripComboBox();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.statusStripMainLocation = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripMainCellCoord = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripMainDataValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripMainAnalysisValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripMainDataFilePath = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonToggleLeftPanel = new System.Windows.Forms.Button();
            this.buttonToggleRightPanel = new System.Windows.Forms.Button();
            this.splitConMain = new System.Windows.Forms.SplitContainer();
            this.splitConLeftPanel = new System.Windows.Forms.SplitContainer();
            this.tabData = new System.Windows.Forms.TabControl();
            this.tabPageData = new System.Windows.Forms.TabPage();
            this.imageListMain = new System.Windows.Forms.ImageList(this.components);
            this.tvwData = new System.Windows.Forms.TreeView();
            this.contextMenuData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuDataEditExcludedValues = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageReference = new System.Windows.Forms.TabPage();
            this.panelReference = new System.Windows.Forms.Panel();
            this.lblReferenceFileLabel = new System.Windows.Forms.Label();
            this.lblModelLayerLinkOption = new System.Windows.Forms.Label();
            this.lblReferenceLayer = new System.Windows.Forms.Label();
            this.lblTimeStepLinkOption = new System.Windows.Forms.Label();
            this.lblReferenceFile = new System.Windows.Forms.Label();
            this.btnSelectReferenceLayer = new System.Windows.Forms.Button();
            this.lblReferenceLayerLabel = new System.Windows.Forms.Label();
            this.tabPageAnalysis = new System.Windows.Forms.TabPage();
            this.txtAnalysisDescription = new System.Windows.Forms.TextBox();
            this.lblAnalysisOption = new System.Windows.Forms.Label();
            this.cboAnalysisOption = new System.Windows.Forms.ComboBox();
            this.gboxContents = new System.Windows.Forms.GroupBox();
            this.tvwContents = new System.Windows.Forms.TreeView();
            this.splitConMap = new System.Windows.Forms.SplitContainer();
            this.panelMapHeader = new System.Windows.Forms.Panel();
            this.lblCurrentLayerDescription = new System.Windows.Forms.Label();
            this.splitConRightPanel = new System.Windows.Forms.SplitContainer();
            this.gboxMapLegend = new System.Windows.Forms.GroupBox();
            this.gboxIndexMap = new System.Windows.Forms.GroupBox();
            this.btnSelectReferenceFile = new System.Windows.Forms.Button();
            this.btnSelectCurrentFile = new System.Windows.Forms.Button();
            this.toolStripButtonSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReCenter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomToGrid = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFullExtent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonHideSidePanels = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowSidePanels = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonContourLayer = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditGriddedValues = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripButtonEditCurrentDataLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonEditReferenceDataLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonEditAnalysisLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonEditBasemap = new System.Windows.Forms.ToolStripButton();
            this.menuMainEditContourLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainEditDataValues = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainEditAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainEditBasemap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainEditModflowMetadata = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainViewHideSidePanels = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainViewShowSidePanels = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainMapPointer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainMapReCenter = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainMapZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainMapZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainMapZoomModelGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainMapZoomFullExtent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonEditMetadata = new System.Windows.Forms.ToolStripButton();
            this.legendMap = new USGS.Puma.UI.MapViewer.MapLegend();
            this.menuMain.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.splitConMain.Panel1.SuspendLayout();
            this.splitConMain.Panel2.SuspendLayout();
            this.splitConMain.SuspendLayout();
            this.splitConLeftPanel.Panel1.SuspendLayout();
            this.splitConLeftPanel.Panel2.SuspendLayout();
            this.splitConLeftPanel.SuspendLayout();
            this.tabData.SuspendLayout();
            this.tabPageData.SuspendLayout();
            this.contextMenuData.SuspendLayout();
            this.tabPageReference.SuspendLayout();
            this.panelReference.SuspendLayout();
            this.tabPageAnalysis.SuspendLayout();
            this.gboxContents.SuspendLayout();
            this.splitConMap.Panel1.SuspendLayout();
            this.splitConMap.Panel2.SuspendLayout();
            this.splitConMap.SuspendLayout();
            this.panelMapHeader.SuspendLayout();
            this.splitConRightPanel.Panel1.SuspendLayout();
            this.splitConRightPanel.Panel2.SuspendLayout();
            this.splitConRightPanel.SuspendLayout();
            this.gboxMapLegend.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainFile,
            this.menuMainEdit,
            this.menuMainView,
            this.menuMainMap,
            this.menuMainHelp});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(1002, 24);
            this.menuMain.TabIndex = 0;
            this.menuMain.Text = "menuStrip1";
            // 
            // menuMainFile
            // 
            this.menuMainFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainFileOpenDataset,
            this.menuMainFileCloseDataset,
            this.menuMainCloseCurrentFile,
            this.menuMainFileSeparator1,
            this.menuMainFileAddFile,
            this.menuMainFileSeparator2,
            this.menuMainFileNewBasemap,
            this.menuMainFileAddBasemap,
            this.menuMainFileRemoveBasemap,
            this.menuMainFileSaveBasemap,
            this.menuMainFileSeparator3,
            this.menuMainFileExport,
            this.menuMainFileSaveBinaryOutput,
            this.menuMainFileSeparator4,
            this.menuMainFilePrintPreview,
            this.menuMainFilePrint,
            this.menuMainFilePrintPDF,
            this.menuMainFileSeparator5,
            this.menuMainFileExit});
            this.menuMainFile.Name = "menuMainFile";
            this.menuMainFile.Size = new System.Drawing.Size(37, 20);
            this.menuMainFile.Text = "File";
            // 
            // menuMainFileOpenDataset
            // 
            this.menuMainFileOpenDataset.Name = "menuMainFileOpenDataset";
            this.menuMainFileOpenDataset.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileOpenDataset.Text = "Open Dataset ...";
            this.menuMainFileOpenDataset.Click += new System.EventHandler(this.menuMainFileOpenDataset_Click);
            // 
            // menuMainFileCloseDataset
            // 
            this.menuMainFileCloseDataset.Name = "menuMainFileCloseDataset";
            this.menuMainFileCloseDataset.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileCloseDataset.Text = "Close Dataset";
            this.menuMainFileCloseDataset.Click += new System.EventHandler(this.menuMainFileCloseDataset_Click);
            // 
            // menuMainCloseCurrentFile
            // 
            this.menuMainCloseCurrentFile.Name = "menuMainCloseCurrentFile";
            this.menuMainCloseCurrentFile.Size = new System.Drawing.Size(175, 22);
            this.menuMainCloseCurrentFile.Text = "Close Current File";
            this.menuMainCloseCurrentFile.Click += new System.EventHandler(this.menuMainCloseCurrentFile_Click);
            // 
            // menuMainFileSeparator1
            // 
            this.menuMainFileSeparator1.Name = "menuMainFileSeparator1";
            this.menuMainFileSeparator1.Size = new System.Drawing.Size(172, 6);
            // 
            // menuMainFileAddFile
            // 
            this.menuMainFileAddFile.Name = "menuMainFileAddFile";
            this.menuMainFileAddFile.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileAddFile.Text = "Add Files ...";
            this.menuMainFileAddFile.Click += new System.EventHandler(this.menuMainFileAddFile_Click);
            // 
            // menuMainFileSeparator2
            // 
            this.menuMainFileSeparator2.Name = "menuMainFileSeparator2";
            this.menuMainFileSeparator2.Size = new System.Drawing.Size(172, 6);
            // 
            // menuMainFileNewBasemap
            // 
            this.menuMainFileNewBasemap.Name = "menuMainFileNewBasemap";
            this.menuMainFileNewBasemap.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileNewBasemap.Text = "New Basemap ...";
            this.menuMainFileNewBasemap.Click += new System.EventHandler(this.menuMainFileNewBasemap_Click);
            // 
            // menuMainFileAddBasemap
            // 
            this.menuMainFileAddBasemap.Name = "menuMainFileAddBasemap";
            this.menuMainFileAddBasemap.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileAddBasemap.Text = "Load Basemap ...";
            this.menuMainFileAddBasemap.Click += new System.EventHandler(this.menuMainFileAddBasemap_Click);
            // 
            // menuMainFileRemoveBasemap
            // 
            this.menuMainFileRemoveBasemap.Name = "menuMainFileRemoveBasemap";
            this.menuMainFileRemoveBasemap.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileRemoveBasemap.Text = "Remove Basemap";
            this.menuMainFileRemoveBasemap.Click += new System.EventHandler(this.menuMainFileRemoveBasemap_Click);
            // 
            // menuMainFileSaveBasemap
            // 
            this.menuMainFileSaveBasemap.Name = "menuMainFileSaveBasemap";
            this.menuMainFileSaveBasemap.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileSaveBasemap.Text = "Save Basemap";
            this.menuMainFileSaveBasemap.Click += new System.EventHandler(this.menuMainFileSaveBasemap_Click);
            // 
            // menuMainFileSeparator3
            // 
            this.menuMainFileSeparator3.Name = "menuMainFileSeparator3";
            this.menuMainFileSeparator3.Size = new System.Drawing.Size(172, 6);
            // 
            // menuMainFileExport
            // 
            this.menuMainFileExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainFileExportCurrentFileXml,
            this.menuMainFileExportShapefiles});
            this.menuMainFileExport.Name = "menuMainFileExport";
            this.menuMainFileExport.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileExport.Text = "Export";
            // 
            // menuMainFileExportCurrentFileXml
            // 
            this.menuMainFileExportCurrentFileXml.Name = "menuMainFileExportCurrentFileXml";
            this.menuMainFileExportCurrentFileXml.Size = new System.Drawing.Size(176, 22);
            this.menuMainFileExportCurrentFileXml.Text = "Current File as XML";
            this.menuMainFileExportCurrentFileXml.Click += new System.EventHandler(this.menuMainFileExportCurrentFileXml_Click);
            // 
            // menuMainFileExportShapefiles
            // 
            this.menuMainFileExportShapefiles.Name = "menuMainFileExportShapefiles";
            this.menuMainFileExportShapefiles.Size = new System.Drawing.Size(176, 22);
            this.menuMainFileExportShapefiles.Text = "Shapefiles ...";
            this.menuMainFileExportShapefiles.Click += new System.EventHandler(this.menuMainFileExportShapefiles_Click);
            // 
            // menuMainFileSaveBinaryOutput
            // 
            this.menuMainFileSaveBinaryOutput.Name = "menuMainFileSaveBinaryOutput";
            this.menuMainFileSaveBinaryOutput.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileSaveBinaryOutput.Text = "Save Binary Output";
            this.menuMainFileSaveBinaryOutput.Click += new System.EventHandler(this.menuMainFileSaveBinaryOutput_Click);
            // 
            // menuMainFileSeparator4
            // 
            this.menuMainFileSeparator4.Name = "menuMainFileSeparator4";
            this.menuMainFileSeparator4.Size = new System.Drawing.Size(172, 6);
            // 
            // menuMainFilePrintPreview
            // 
            this.menuMainFilePrintPreview.Name = "menuMainFilePrintPreview";
            this.menuMainFilePrintPreview.Size = new System.Drawing.Size(175, 22);
            this.menuMainFilePrintPreview.Text = "Print Preview ...";
            this.menuMainFilePrintPreview.Click += new System.EventHandler(this.menuMainFilePrintPreview_Click);
            // 
            // menuMainFilePrint
            // 
            this.menuMainFilePrint.Name = "menuMainFilePrint";
            this.menuMainFilePrint.Size = new System.Drawing.Size(175, 22);
            this.menuMainFilePrint.Text = "Print ...";
            this.menuMainFilePrint.Click += new System.EventHandler(this.menuMainFilePrint_Click);
            // 
            // menuMainFilePrintPDF
            // 
            this.menuMainFilePrintPDF.Name = "menuMainFilePrintPDF";
            this.menuMainFilePrintPDF.Size = new System.Drawing.Size(175, 22);
            this.menuMainFilePrintPDF.Text = "Print to PDF ...";
            this.menuMainFilePrintPDF.Click += new System.EventHandler(this.menuMainFilePrintPDF_Click);
            // 
            // menuMainFileSeparator5
            // 
            this.menuMainFileSeparator5.Name = "menuMainFileSeparator5";
            this.menuMainFileSeparator5.Size = new System.Drawing.Size(172, 6);
            // 
            // menuMainFileExit
            // 
            this.menuMainFileExit.Name = "menuMainFileExit";
            this.menuMainFileExit.Size = new System.Drawing.Size(175, 22);
            this.menuMainFileExit.Text = "Exit";
            this.menuMainFileExit.Click += new System.EventHandler(this.menuMainFileExit_Click);
            // 
            // menuMainEdit
            // 
            this.menuMainEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainEditContourLayer,
            this.menuMainEditDataValues,
            this.menuMainEditAnalysis,
            this.menuMainEditBasemap,
            this.menuMainEditModflowMetadata});
            this.menuMainEdit.Name = "menuMainEdit";
            this.menuMainEdit.Size = new System.Drawing.Size(39, 20);
            this.menuMainEdit.Text = "Edit";
            // 
            // menuMainView
            // 
            this.menuMainView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainViewHideSidePanels,
            this.menuMainViewShowSidePanels});
            this.menuMainView.Name = "menuMainView";
            this.menuMainView.Size = new System.Drawing.Size(44, 20);
            this.menuMainView.Text = "View";
            // 
            // menuMainMap
            // 
            this.menuMainMap.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainMapPointer,
            this.menuMainMapReCenter,
            this.menuMainMapZoomIn,
            this.menuMainMapZoomOut,
            this.menuMainMapSeparator1,
            this.menuMainMapZoomModelGrid,
            this.menuMainMapZoomFullExtent});
            this.menuMainMap.Name = "menuMainMap";
            this.menuMainMap.Size = new System.Drawing.Size(43, 20);
            this.menuMainMap.Text = "Map";
            // 
            // menuMainMapSeparator1
            // 
            this.menuMainMapSeparator1.Name = "menuMainMapSeparator1";
            this.menuMainMapSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // menuMainHelp
            // 
            this.menuMainHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainHelpAbout});
            this.menuMainHelp.Name = "menuMainHelp";
            this.menuMainHelp.Size = new System.Drawing.Size(44, 20);
            this.menuMainHelp.Text = "Help";
            // 
            // menuMainHelpAbout
            // 
            this.menuMainHelpAbout.Name = "menuMainHelpAbout";
            this.menuMainHelpAbout.Size = new System.Drawing.Size(119, 22);
            this.menuMainHelpAbout.Text = "About ...";
            this.menuMainHelpAbout.Click += new System.EventHandler(this.menuMainHelpAbout_Click);
            // 
            // toolStripMain
            // 
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.toolStripButtonSelect,
            this.toolStripButtonReCenter,
            this.toolStripButtonZoomIn,
            this.toolStripButtonZoomOut,
            this.toolStripSeparator2,
            this.toolStripLabelZoomTo,
            this.toolStripButtonZoomToGrid,
            this.toolStripButtonFullExtent,
            this.toolStripSeparator3,
            this.toolStripLabelView,
            this.toolStripButtonHideSidePanels,
            this.toolStripButtonShowSidePanels,
            this.toolStripSeparator4,
            this.toolStripLabelEdit,
            this.toolStripButtonContourLayer,
            this.toolStripButtonEditGriddedValues,
            this.toolStripButtonEditBasemap,
            this.toolStripButtonEditMetadata,
            this.toolStripSeparator9,
            this.toolStripLabelAnalysis,
            this.cboGriddedValuesDisplayOption});
            this.toolStripMain.Location = new System.Drawing.Point(0, 24);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(1002, 25);
            this.toolStripMain.TabIndex = 1;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelZoomTo
            // 
            this.toolStripLabelZoomTo.Name = "toolStripLabelZoomTo";
            this.toolStripLabelZoomTo.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabelZoomTo.Text = "Zoom to:";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelView
            // 
            this.toolStripLabelView.Name = "toolStripLabelView";
            this.toolStripLabelView.Size = new System.Drawing.Size(35, 22);
            this.toolStripLabelView.Text = "View:";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelEdit
            // 
            this.toolStripLabelEdit.Name = "toolStripLabelEdit";
            this.toolStripLabelEdit.Size = new System.Drawing.Size(30, 22);
            this.toolStripLabelEdit.Text = "Edit:";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelAnalysis
            // 
            this.toolStripLabelAnalysis.Name = "toolStripLabelAnalysis";
            this.toolStripLabelAnalysis.Size = new System.Drawing.Size(48, 22);
            this.toolStripLabelAnalysis.Text = "Display:";
            // 
            // cboGriddedValuesDisplayOption
            // 
            this.cboGriddedValuesDisplayOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGriddedValuesDisplayOption.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.cboGriddedValuesDisplayOption.Name = "cboGriddedValuesDisplayOption";
            this.cboGriddedValuesDisplayOption.Size = new System.Drawing.Size(275, 25);
            this.cboGriddedValuesDisplayOption.SelectedIndexChanged += new System.EventHandler(this.cboGriddedValuesDisplayOption_SelectedIndexChanged);
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStripMainLocation,
            this.statusStripMainCellCoord,
            this.statusStripMainDataValue,
            this.statusStripMainAnalysisValue,
            this.statusStripMainDataFilePath});
            this.statusStripMain.Location = new System.Drawing.Point(0, 607);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(1002, 22);
            this.statusStripMain.TabIndex = 2;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // statusStripMainLocation
            // 
            this.statusStripMainLocation.AutoSize = false;
            this.statusStripMainLocation.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusStripMainLocation.Name = "statusStripMainLocation";
            this.statusStripMainLocation.Size = new System.Drawing.Size(150, 17);
            this.statusStripMainLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStripMainCellCoord
            // 
            this.statusStripMainCellCoord.AutoSize = false;
            this.statusStripMainCellCoord.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusStripMainCellCoord.Name = "statusStripMainCellCoord";
            this.statusStripMainCellCoord.Size = new System.Drawing.Size(100, 17);
            this.statusStripMainCellCoord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStripMainDataValue
            // 
            this.statusStripMainDataValue.AutoSize = false;
            this.statusStripMainDataValue.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusStripMainDataValue.Name = "statusStripMainDataValue";
            this.statusStripMainDataValue.Size = new System.Drawing.Size(185, 17);
            this.statusStripMainDataValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStripMainAnalysisValue
            // 
            this.statusStripMainAnalysisValue.AutoSize = false;
            this.statusStripMainAnalysisValue.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusStripMainAnalysisValue.Name = "statusStripMainAnalysisValue";
            this.statusStripMainAnalysisValue.Size = new System.Drawing.Size(185, 17);
            this.statusStripMainAnalysisValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStripMainDataFilePath
            // 
            this.statusStripMainDataFilePath.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusStripMainDataFilePath.Name = "statusStripMainDataFilePath";
            this.statusStripMainDataFilePath.Size = new System.Drawing.Size(367, 17);
            this.statusStripMainDataFilePath.Spring = true;
            this.statusStripMainDataFilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonToggleLeftPanel
            // 
            this.buttonToggleLeftPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonToggleLeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonToggleLeftPanel.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.buttonToggleLeftPanel.FlatAppearance.BorderSize = 0;
            this.buttonToggleLeftPanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonToggleLeftPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonToggleLeftPanel.Location = new System.Drawing.Point(0, 49);
            this.buttonToggleLeftPanel.Name = "buttonToggleLeftPanel";
            this.buttonToggleLeftPanel.Size = new System.Drawing.Size(16, 558);
            this.buttonToggleLeftPanel.TabIndex = 3;
            this.buttonToggleLeftPanel.Text = "<";
            this.buttonToggleLeftPanel.UseVisualStyleBackColor = false;
            this.buttonToggleLeftPanel.Click += new System.EventHandler(this.buttonToggleLeftPanel_Click);
            // 
            // buttonToggleRightPanel
            // 
            this.buttonToggleRightPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonToggleRightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonToggleRightPanel.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.buttonToggleRightPanel.FlatAppearance.BorderSize = 0;
            this.buttonToggleRightPanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonToggleRightPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonToggleRightPanel.Location = new System.Drawing.Point(986, 49);
            this.buttonToggleRightPanel.Name = "buttonToggleRightPanel";
            this.buttonToggleRightPanel.Size = new System.Drawing.Size(16, 558);
            this.buttonToggleRightPanel.TabIndex = 4;
            this.buttonToggleRightPanel.Text = ">";
            this.buttonToggleRightPanel.UseVisualStyleBackColor = false;
            this.buttonToggleRightPanel.Click += new System.EventHandler(this.buttonToggleRightPanel_Click);
            // 
            // splitConMain
            // 
            this.splitConMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitConMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitConMain.Location = new System.Drawing.Point(16, 49);
            this.splitConMain.Name = "splitConMain";
            // 
            // splitConMain.Panel1
            // 
            this.splitConMain.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitConMain.Panel1.Controls.Add(this.splitConLeftPanel);
            // 
            // splitConMain.Panel2
            // 
            this.splitConMain.Panel2.Controls.Add(this.splitConMap);
            this.splitConMain.Size = new System.Drawing.Size(970, 558);
            this.splitConMain.SplitterDistance = 262;
            this.splitConMain.TabIndex = 5;
            // 
            // splitConLeftPanel
            // 
            this.splitConLeftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitConLeftPanel.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitConLeftPanel.Location = new System.Drawing.Point(0, 0);
            this.splitConLeftPanel.Name = "splitConLeftPanel";
            this.splitConLeftPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitConLeftPanel.Panel1
            // 
            this.splitConLeftPanel.Panel1.Controls.Add(this.tabData);
            // 
            // splitConLeftPanel.Panel2
            // 
            this.splitConLeftPanel.Panel2.Controls.Add(this.gboxContents);
            this.splitConLeftPanel.Size = new System.Drawing.Size(262, 558);
            this.splitConLeftPanel.SplitterDistance = 208;
            this.splitConLeftPanel.TabIndex = 0;
            // 
            // tabData
            // 
            this.tabData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabData.Controls.Add(this.tabPageData);
            this.tabData.Controls.Add(this.tabPageReference);
            this.tabData.Controls.Add(this.tabPageAnalysis);
            this.tabData.Location = new System.Drawing.Point(0, 6);
            this.tabData.Name = "tabData";
            this.tabData.SelectedIndex = 0;
            this.tabData.Size = new System.Drawing.Size(262, 205);
            this.tabData.TabIndex = 0;
            this.tabData.Resize += new System.EventHandler(this.tabData_Resize);
            // 
            // tabPageData
            // 
            this.tabPageData.Controls.Add(this.btnSelectReferenceFile);
            this.tabPageData.Controls.Add(this.btnSelectCurrentFile);
            this.tabPageData.Controls.Add(this.tvwData);
            this.tabPageData.Location = new System.Drawing.Point(4, 22);
            this.tabPageData.Name = "tabPageData";
            this.tabPageData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageData.Size = new System.Drawing.Size(254, 179);
            this.tabPageData.TabIndex = 0;
            this.tabPageData.Text = "Data";
            this.tabPageData.UseVisualStyleBackColor = true;
            // 
            // imageListMain
            // 
            this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
            this.imageListMain.TransparentColor = System.Drawing.Color.Magenta;
            this.imageListMain.Images.SetKeyName(0, "find.bmp");
            this.imageListMain.Images.SetKeyName(1, "BinaryLayerFile.bmp");
            this.imageListMain.Images.SetKeyName(2, "folder_closed.bmp");
            this.imageListMain.Images.SetKeyName(3, "folder_open.bmp");
            this.imageListMain.Images.SetKeyName(4, "LayerArray.bmp");
            this.imageListMain.Images.SetKeyName(5, "BinaryLayerFileSelectedCurrent2.bmp");
            this.imageListMain.Images.SetKeyName(6, "BinaryLayerArraySelectedReference2.bmp");
            this.imageListMain.Images.SetKeyName(7, "BinaryLayerArraySelectedBoth2.bmp");
            // 
            // tvwData
            // 
            this.tvwData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwData.ContextMenuStrip = this.contextMenuData;
            this.tvwData.ImageIndex = 0;
            this.tvwData.ImageList = this.imageListMain;
            this.tvwData.Location = new System.Drawing.Point(3, 3);
            this.tvwData.Name = "tvwData";
            this.tvwData.SelectedImageIndex = 0;
            this.tvwData.Size = new System.Drawing.Size(248, 141);
            this.tvwData.TabIndex = 0;
            this.tvwData.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvwData_NodeMouseDoubleClick);
            this.tvwData.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvwData_BeforeCollapse);
            this.tvwData.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvwData_NodeMouseClick);
            // 
            // contextMenuData
            // 
            this.contextMenuData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuDataEditExcludedValues});
            this.contextMenuData.Name = "contextMenuData";
            this.contextMenuData.Size = new System.Drawing.Size(194, 26);
            this.contextMenuData.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuData_Opening);
            // 
            // contextMenuDataEditExcludedValues
            // 
            this.contextMenuDataEditExcludedValues.Name = "contextMenuDataEditExcludedValues";
            this.contextMenuDataEditExcludedValues.Size = new System.Drawing.Size(193, 22);
            this.contextMenuDataEditExcludedValues.Text = "Edit Excluded Values ...";
            this.contextMenuDataEditExcludedValues.Click += new System.EventHandler(this.contextMenuDataEditExcludedValues_Click);
            // 
            // tabPageReference
            // 
            this.tabPageReference.Controls.Add(this.panelReference);
            this.tabPageReference.Location = new System.Drawing.Point(4, 22);
            this.tabPageReference.Name = "tabPageReference";
            this.tabPageReference.Size = new System.Drawing.Size(254, 179);
            this.tabPageReference.TabIndex = 3;
            this.tabPageReference.Text = "Reference";
            this.tabPageReference.UseVisualStyleBackColor = true;
            // 
            // panelReference
            // 
            this.panelReference.BackColor = System.Drawing.Color.White;
            this.panelReference.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelReference.Controls.Add(this.lblReferenceFileLabel);
            this.panelReference.Controls.Add(this.lblModelLayerLinkOption);
            this.panelReference.Controls.Add(this.lblReferenceLayer);
            this.panelReference.Controls.Add(this.lblTimeStepLinkOption);
            this.panelReference.Controls.Add(this.lblReferenceFile);
            this.panelReference.Controls.Add(this.btnSelectReferenceLayer);
            this.panelReference.Controls.Add(this.lblReferenceLayerLabel);
            this.panelReference.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelReference.Location = new System.Drawing.Point(0, 0);
            this.panelReference.Name = "panelReference";
            this.panelReference.Size = new System.Drawing.Size(254, 179);
            this.panelReference.TabIndex = 12;
            // 
            // lblReferenceFileLabel
            // 
            this.lblReferenceFileLabel.AutoSize = true;
            this.lblReferenceFileLabel.Location = new System.Drawing.Point(3, 9);
            this.lblReferenceFileLabel.Name = "lblReferenceFileLabel";
            this.lblReferenceFileLabel.Size = new System.Drawing.Size(79, 13);
            this.lblReferenceFileLabel.TabIndex = 5;
            this.lblReferenceFileLabel.Text = "Reference File:";
            // 
            // lblModelLayerLinkOption
            // 
            this.lblModelLayerLinkOption.AutoSize = true;
            this.lblModelLayerLinkOption.Location = new System.Drawing.Point(3, 116);
            this.lblModelLayerLinkOption.Name = "lblModelLayerLinkOption";
            this.lblModelLayerLinkOption.Size = new System.Drawing.Size(116, 13);
            this.lblModelLayerLinkOption.TabIndex = 4;
            this.lblModelLayerLinkOption.Text = "Model layer linked to ...";
            // 
            // lblReferenceLayer
            // 
            this.lblReferenceLayer.AutoSize = true;
            this.lblReferenceLayer.Location = new System.Drawing.Point(18, 59);
            this.lblReferenceLayer.Name = "lblReferenceLayer";
            this.lblReferenceLayer.Size = new System.Drawing.Size(109, 13);
            this.lblReferenceLayer.TabIndex = 11;
            this.lblReferenceLayer.Text = "array layer information";
            // 
            // lblTimeStepLinkOption
            // 
            this.lblTimeStepLinkOption.AutoSize = true;
            this.lblTimeStepLinkOption.Location = new System.Drawing.Point(3, 93);
            this.lblTimeStepLinkOption.Name = "lblTimeStepLinkOption";
            this.lblTimeStepLinkOption.Size = new System.Drawing.Size(108, 13);
            this.lblTimeStepLinkOption.TabIndex = 3;
            this.lblTimeStepLinkOption.Text = "Time step linked to ...";
            // 
            // lblReferenceFile
            // 
            this.lblReferenceFile.AutoSize = true;
            this.lblReferenceFile.Location = new System.Drawing.Point(18, 22);
            this.lblReferenceFile.Name = "lblReferenceFile";
            this.lblReferenceFile.Size = new System.Drawing.Size(122, 13);
            this.lblReferenceFile.TabIndex = 10;
            this.lblReferenceFile.Text = "reference file information";
            // 
            // btnSelectReferenceLayer
            // 
            this.btnSelectReferenceLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectReferenceLayer.Location = new System.Drawing.Point(196, 54);
            this.btnSelectReferenceLayer.Name = "btnSelectReferenceLayer";
            this.btnSelectReferenceLayer.Size = new System.Drawing.Size(53, 23);
            this.btnSelectReferenceLayer.TabIndex = 2;
            this.btnSelectReferenceLayer.Text = "Select";
            this.btnSelectReferenceLayer.UseVisualStyleBackColor = true;
            this.btnSelectReferenceLayer.Click += new System.EventHandler(this.btnSelectReferenceLayer_Click);
            // 
            // lblReferenceLayerLabel
            // 
            this.lblReferenceLayerLabel.AutoSize = true;
            this.lblReferenceLayerLabel.Location = new System.Drawing.Point(3, 45);
            this.lblReferenceLayerLabel.Name = "lblReferenceLayerLabel";
            this.lblReferenceLayerLabel.Size = new System.Drawing.Size(115, 13);
            this.lblReferenceLayerLabel.TabIndex = 6;
            this.lblReferenceLayerLabel.Text = "Reference Data Layer:";
            // 
            // tabPageAnalysis
            // 
            this.tabPageAnalysis.Controls.Add(this.txtAnalysisDescription);
            this.tabPageAnalysis.Controls.Add(this.lblAnalysisOption);
            this.tabPageAnalysis.Controls.Add(this.cboAnalysisOption);
            this.tabPageAnalysis.Location = new System.Drawing.Point(4, 22);
            this.tabPageAnalysis.Name = "tabPageAnalysis";
            this.tabPageAnalysis.Size = new System.Drawing.Size(254, 179);
            this.tabPageAnalysis.TabIndex = 4;
            this.tabPageAnalysis.Text = "Analysis";
            this.tabPageAnalysis.UseVisualStyleBackColor = true;
            // 
            // txtAnalysisDescription
            // 
            this.txtAnalysisDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnalysisDescription.BackColor = System.Drawing.Color.White;
            this.txtAnalysisDescription.Location = new System.Drawing.Point(3, 48);
            this.txtAnalysisDescription.Multiline = true;
            this.txtAnalysisDescription.Name = "txtAnalysisDescription";
            this.txtAnalysisDescription.ReadOnly = true;
            this.txtAnalysisDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAnalysisDescription.Size = new System.Drawing.Size(248, 128);
            this.txtAnalysisDescription.TabIndex = 2;
            // 
            // lblAnalysisOption
            // 
            this.lblAnalysisOption.AutoSize = true;
            this.lblAnalysisOption.Location = new System.Drawing.Point(3, 5);
            this.lblAnalysisOption.Name = "lblAnalysisOption";
            this.lblAnalysisOption.Size = new System.Drawing.Size(41, 13);
            this.lblAnalysisOption.TabIndex = 1;
            this.lblAnalysisOption.Text = "Option:";
            // 
            // cboAnalysisOption
            // 
            this.cboAnalysisOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAnalysisOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnalysisOption.FormattingEnabled = true;
            this.cboAnalysisOption.Location = new System.Drawing.Point(3, 21);
            this.cboAnalysisOption.Name = "cboAnalysisOption";
            this.cboAnalysisOption.Size = new System.Drawing.Size(248, 21);
            this.cboAnalysisOption.TabIndex = 0;
            // 
            // gboxContents
            // 
            this.gboxContents.Controls.Add(this.tvwContents);
            this.gboxContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboxContents.Location = new System.Drawing.Point(0, 0);
            this.gboxContents.Name = "gboxContents";
            this.gboxContents.Size = new System.Drawing.Size(262, 346);
            this.gboxContents.TabIndex = 0;
            this.gboxContents.TabStop = false;
            this.gboxContents.Text = "Contents";
            // 
            // tvwContents
            // 
            this.tvwContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwContents.ImageIndex = 0;
            this.tvwContents.ImageList = this.imageListMain;
            this.tvwContents.Location = new System.Drawing.Point(3, 16);
            this.tvwContents.Name = "tvwContents";
            this.tvwContents.SelectedImageIndex = 0;
            this.tvwContents.Size = new System.Drawing.Size(256, 327);
            this.tvwContents.TabIndex = 0;
            this.tvwContents.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvwContents_NodeMouseDoubleClick);
            this.tvwContents.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwContents_AfterSelect);
            this.tvwContents.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvwContents_NodeMouseClick);
            // 
            // splitConMap
            // 
            this.splitConMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitConMap.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitConMap.Location = new System.Drawing.Point(0, 0);
            this.splitConMap.Name = "splitConMap";
            // 
            // splitConMap.Panel1
            // 
            this.splitConMap.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitConMap.Panel1.Controls.Add(this.panelMapHeader);
            // 
            // splitConMap.Panel2
            // 
            this.splitConMap.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitConMap.Panel2.Controls.Add(this.splitConRightPanel);
            this.splitConMap.Size = new System.Drawing.Size(704, 558);
            this.splitConMap.SplitterDistance = 472;
            this.splitConMap.TabIndex = 0;
            // 
            // panelMapHeader
            // 
            this.panelMapHeader.Controls.Add(this.lblCurrentLayerDescription);
            this.panelMapHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMapHeader.Location = new System.Drawing.Point(0, 0);
            this.panelMapHeader.Name = "panelMapHeader";
            this.panelMapHeader.Size = new System.Drawing.Size(472, 27);
            this.panelMapHeader.TabIndex = 0;
            // 
            // lblCurrentLayerDescription
            // 
            this.lblCurrentLayerDescription.AutoSize = true;
            this.lblCurrentLayerDescription.Location = new System.Drawing.Point(2, 6);
            this.lblCurrentLayerDescription.Name = "lblCurrentLayerDescription";
            this.lblCurrentLayerDescription.Size = new System.Drawing.Size(132, 13);
            this.lblCurrentLayerDescription.TabIndex = 1;
            this.lblCurrentLayerDescription.Text = "Current data layer: <none>";
            // 
            // splitConRightPanel
            // 
            this.splitConRightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitConRightPanel.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitConRightPanel.Location = new System.Drawing.Point(0, 0);
            this.splitConRightPanel.Name = "splitConRightPanel";
            this.splitConRightPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitConRightPanel.Panel1
            // 
            this.splitConRightPanel.Panel1.Controls.Add(this.gboxMapLegend);
            // 
            // splitConRightPanel.Panel2
            // 
            this.splitConRightPanel.Panel2.Controls.Add(this.gboxIndexMap);
            this.splitConRightPanel.Size = new System.Drawing.Size(228, 558);
            this.splitConRightPanel.SplitterDistance = 334;
            this.splitConRightPanel.TabIndex = 0;
            // 
            // gboxMapLegend
            // 
            this.gboxMapLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gboxMapLegend.Controls.Add(this.legendMap);
            this.gboxMapLegend.Location = new System.Drawing.Point(0, 3);
            this.gboxMapLegend.Name = "gboxMapLegend";
            this.gboxMapLegend.Size = new System.Drawing.Size(228, 331);
            this.gboxMapLegend.TabIndex = 0;
            this.gboxMapLegend.TabStop = false;
            this.gboxMapLegend.Text = "Legend";
            // 
            // gboxIndexMap
            // 
            this.gboxIndexMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboxIndexMap.Location = new System.Drawing.Point(0, 0);
            this.gboxIndexMap.Name = "gboxIndexMap";
            this.gboxIndexMap.Size = new System.Drawing.Size(228, 220);
            this.gboxIndexMap.TabIndex = 0;
            this.gboxIndexMap.TabStop = false;
            this.gboxIndexMap.Text = "Index Map";
            // 
            // btnSelectReferenceFile
            // 
            this.btnSelectReferenceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectReferenceFile.ImageIndex = 6;
            this.btnSelectReferenceFile.ImageList = this.imageListMain;
            this.btnSelectReferenceFile.Location = new System.Drawing.Point(130, 150);
            this.btnSelectReferenceFile.Name = "btnSelectReferenceFile";
            this.btnSelectReferenceFile.Size = new System.Drawing.Size(121, 23);
            this.btnSelectReferenceFile.TabIndex = 2;
            this.btnSelectReferenceFile.Text = "Set as Reference";
            this.btnSelectReferenceFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelectReferenceFile.UseVisualStyleBackColor = true;
            this.btnSelectReferenceFile.Click += new System.EventHandler(this.btnSelectReferenceFile_Click);
            // 
            // btnSelectCurrentFile
            // 
            this.btnSelectCurrentFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectCurrentFile.ImageIndex = 5;
            this.btnSelectCurrentFile.ImageList = this.imageListMain;
            this.btnSelectCurrentFile.Location = new System.Drawing.Point(2, 150);
            this.btnSelectCurrentFile.Name = "btnSelectCurrentFile";
            this.btnSelectCurrentFile.Size = new System.Drawing.Size(122, 23);
            this.btnSelectCurrentFile.TabIndex = 1;
            this.btnSelectCurrentFile.Text = "Set as Current";
            this.btnSelectCurrentFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelectCurrentFile.UseVisualStyleBackColor = true;
            this.btnSelectCurrentFile.Click += new System.EventHandler(this.btnSelectCurrentFile_Click);
            // 
            // toolStripButtonSelect
            // 
            this.toolStripButtonSelect.Checked = true;
            this.toolStripButtonSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelect.Image = global::ModflowOutputViewer.Properties.Resources.SelectArrow;
            this.toolStripButtonSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelect.Name = "toolStripButtonSelect";
            this.toolStripButtonSelect.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSelect.Text = "Select";
            this.toolStripButtonSelect.Click += new System.EventHandler(this.toolStripButtonSelect_Click);
            // 
            // toolStripButtonReCenter
            // 
            this.toolStripButtonReCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReCenter.Image = global::ModflowOutputViewer.Properties.Resources.ReCenter;
            this.toolStripButtonReCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReCenter.Name = "toolStripButtonReCenter";
            this.toolStripButtonReCenter.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonReCenter.Text = "ReCenter";
            this.toolStripButtonReCenter.Click += new System.EventHandler(this.toolStripButtonReCenter_Click);
            // 
            // toolStripButtonZoomIn
            // 
            this.toolStripButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomIn.Image = global::ModflowOutputViewer.Properties.Resources.ZoomIn;
            this.toolStripButtonZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomIn.Name = "toolStripButtonZoomIn";
            this.toolStripButtonZoomIn.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoomIn.Text = "Zoom In";
            this.toolStripButtonZoomIn.Click += new System.EventHandler(this.toolStripButtonZoomIn_Click);
            // 
            // toolStripButtonZoomOut
            // 
            this.toolStripButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomOut.Image = global::ModflowOutputViewer.Properties.Resources.ZoomOut;
            this.toolStripButtonZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomOut.Name = "toolStripButtonZoomOut";
            this.toolStripButtonZoomOut.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoomOut.Text = "Zoom Out";
            this.toolStripButtonZoomOut.Click += new System.EventHandler(this.toolStripButtonZoomOut_Click);
            // 
            // toolStripButtonZoomToGrid
            // 
            this.toolStripButtonZoomToGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomToGrid.Image = global::ModflowOutputViewer.Properties.Resources.ZoomToGrid;
            this.toolStripButtonZoomToGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomToGrid.Name = "toolStripButtonZoomToGrid";
            this.toolStripButtonZoomToGrid.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoomToGrid.Text = "Zoom to model grid";
            this.toolStripButtonZoomToGrid.Click += new System.EventHandler(this.toolStripButtonZoomToGrid_Click);
            // 
            // toolStripButtonFullExtent
            // 
            this.toolStripButtonFullExtent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFullExtent.Image = global::ModflowOutputViewer.Properties.Resources.globe;
            this.toolStripButtonFullExtent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFullExtent.Name = "toolStripButtonFullExtent";
            this.toolStripButtonFullExtent.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFullExtent.Text = "Full Extent";
            this.toolStripButtonFullExtent.ToolTipText = "Zoom to full map extent";
            this.toolStripButtonFullExtent.Click += new System.EventHandler(this.toolStripButtonFullExtent_Click);
            // 
            // toolStripButtonHideSidePanels
            // 
            this.toolStripButtonHideSidePanels.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonHideSidePanels.Image = global::ModflowOutputViewer.Properties.Resources.HideSidePanels;
            this.toolStripButtonHideSidePanels.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonHideSidePanels.Name = "toolStripButtonHideSidePanels";
            this.toolStripButtonHideSidePanels.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonHideSidePanels.Text = "Hide side panels";
            this.toolStripButtonHideSidePanels.Click += new System.EventHandler(this.toolStripButtonHideSidePanels_Click);
            // 
            // toolStripButtonShowSidePanels
            // 
            this.toolStripButtonShowSidePanels.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowSidePanels.Image = global::ModflowOutputViewer.Properties.Resources.ShowSidePanels;
            this.toolStripButtonShowSidePanels.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowSidePanels.Name = "toolStripButtonShowSidePanels";
            this.toolStripButtonShowSidePanels.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShowSidePanels.Text = "Show side panels";
            this.toolStripButtonShowSidePanels.Click += new System.EventHandler(this.toolStripButtonShowSidePanels_Click);
            // 
            // toolStripButtonContourLayer
            // 
            this.toolStripButtonContourLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonContourLayer.Image = global::ModflowOutputViewer.Properties.Resources.ContourLayer;
            this.toolStripButtonContourLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonContourLayer.Name = "toolStripButtonContourLayer";
            this.toolStripButtonContourLayer.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonContourLayer.Text = "Contour Layer";
            this.toolStripButtonContourLayer.ToolTipText = "Edit data contour layer";
            this.toolStripButtonContourLayer.Click += new System.EventHandler(this.toolStripButtonContourLayer_Click);
            // 
            // toolStripButtonEditGriddedValues
            // 
            this.toolStripButtonEditGriddedValues.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditGriddedValues.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEditCurrentDataLayer,
            this.toolStripButtonEditReferenceDataLayer,
            this.toolStripButtonEditAnalysisLayer});
            this.toolStripButtonEditGriddedValues.Image = global::ModflowOutputViewer.Properties.Resources.GriddedValuesLayer;
            this.toolStripButtonEditGriddedValues.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditGriddedValues.Name = "toolStripButtonEditGriddedValues";
            this.toolStripButtonEditGriddedValues.Size = new System.Drawing.Size(29, 22);
            this.toolStripButtonEditGriddedValues.Text = "toolStripDropDownButton1";
            // 
            // toolStripButtonEditCurrentDataLayer
            // 
            this.toolStripButtonEditCurrentDataLayer.Name = "toolStripButtonEditCurrentDataLayer";
            this.toolStripButtonEditCurrentDataLayer.Size = new System.Drawing.Size(184, 22);
            this.toolStripButtonEditCurrentDataLayer.Text = "Current Data Layer";
            this.toolStripButtonEditCurrentDataLayer.Click += new System.EventHandler(this.toolStripButtonEditCurrentDataLayer_Click);
            // 
            // toolStripButtonEditReferenceDataLayer
            // 
            this.toolStripButtonEditReferenceDataLayer.Name = "toolStripButtonEditReferenceDataLayer";
            this.toolStripButtonEditReferenceDataLayer.Size = new System.Drawing.Size(184, 22);
            this.toolStripButtonEditReferenceDataLayer.Text = "Reference Data Layer";
            this.toolStripButtonEditReferenceDataLayer.Click += new System.EventHandler(this.toolStripButtonEditReferenceDataLayer_Click);
            // 
            // toolStripButtonEditAnalysisLayer
            // 
            this.toolStripButtonEditAnalysisLayer.Name = "toolStripButtonEditAnalysisLayer";
            this.toolStripButtonEditAnalysisLayer.Size = new System.Drawing.Size(184, 22);
            this.toolStripButtonEditAnalysisLayer.Text = "Analysis Layer";
            this.toolStripButtonEditAnalysisLayer.Click += new System.EventHandler(this.toolStripButtonEditAnalysisLayer_Click);
            // 
            // toolStripButtonEditBasemap
            // 
            this.toolStripButtonEditBasemap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditBasemap.Image = global::ModflowOutputViewer.Properties.Resources.Basemap_1;
            this.toolStripButtonEditBasemap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditBasemap.Name = "toolStripButtonEditBasemap";
            this.toolStripButtonEditBasemap.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEditBasemap.Text = "Edit basemap";
            this.toolStripButtonEditBasemap.ToolTipText = "Edit basemap";
            this.toolStripButtonEditBasemap.Click += new System.EventHandler(this.toolStripButtonEditBasemap_Click);
            // 
            // menuMainEditContourLayer
            // 
            this.menuMainEditContourLayer.Image = global::ModflowOutputViewer.Properties.Resources.ContourLayer;
            this.menuMainEditContourLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainEditContourLayer.Name = "menuMainEditContourLayer";
            this.menuMainEditContourLayer.Size = new System.Drawing.Size(187, 22);
            this.menuMainEditContourLayer.Text = "Contour Layer ...";
            // 
            // menuMainEditDataValues
            // 
            this.menuMainEditDataValues.Image = global::ModflowOutputViewer.Properties.Resources.GriddedValuesLayer;
            this.menuMainEditDataValues.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainEditDataValues.Name = "menuMainEditDataValues";
            this.menuMainEditDataValues.Size = new System.Drawing.Size(187, 22);
            this.menuMainEditDataValues.Text = "Data Values Layer ...";
            // 
            // menuMainEditAnalysis
            // 
            this.menuMainEditAnalysis.Image = global::ModflowOutputViewer.Properties.Resources.AnalysisLayer;
            this.menuMainEditAnalysis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainEditAnalysis.Name = "menuMainEditAnalysis";
            this.menuMainEditAnalysis.Size = new System.Drawing.Size(187, 22);
            this.menuMainEditAnalysis.Text = "Analysis Layer ...";
            // 
            // menuMainEditBasemap
            // 
            this.menuMainEditBasemap.Image = global::ModflowOutputViewer.Properties.Resources.Basemap_1;
            this.menuMainEditBasemap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainEditBasemap.Name = "menuMainEditBasemap";
            this.menuMainEditBasemap.Size = new System.Drawing.Size(187, 22);
            this.menuMainEditBasemap.Text = "Basemap ...";
            this.menuMainEditBasemap.Click += new System.EventHandler(this.menuMainEditBasemap_Click);
            // 
            // menuMainEditModflowMetadata
            // 
            this.menuMainEditModflowMetadata.Image = global::ModflowOutputViewer.Properties.Resources.EditMetadata;
            this.menuMainEditModflowMetadata.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.menuMainEditModflowMetadata.Name = "menuMainEditModflowMetadata";
            this.menuMainEditModflowMetadata.Size = new System.Drawing.Size(187, 22);
            this.menuMainEditModflowMetadata.Text = "MODFLOW Metadata";
            this.menuMainEditModflowMetadata.Click += new System.EventHandler(this.menuMainEditModflowMetadata_Click);
            // 
            // menuMainViewHideSidePanels
            // 
            this.menuMainViewHideSidePanels.Image = global::ModflowOutputViewer.Properties.Resources.HideSidePanels;
            this.menuMainViewHideSidePanels.Name = "menuMainViewHideSidePanels";
            this.menuMainViewHideSidePanels.Size = new System.Drawing.Size(165, 22);
            this.menuMainViewHideSidePanels.Text = "Hide Side Panels";
            this.menuMainViewHideSidePanels.Click += new System.EventHandler(this.menuMainViewHideSidePanels_Click);
            // 
            // menuMainViewShowSidePanels
            // 
            this.menuMainViewShowSidePanels.Image = global::ModflowOutputViewer.Properties.Resources.ShowSidePanels;
            this.menuMainViewShowSidePanels.Name = "menuMainViewShowSidePanels";
            this.menuMainViewShowSidePanels.Size = new System.Drawing.Size(165, 22);
            this.menuMainViewShowSidePanels.Text = "Show Side Panels";
            this.menuMainViewShowSidePanels.Click += new System.EventHandler(this.menuMainViewShowSidePanels_Click);
            // 
            // menuMainMapPointer
            // 
            this.menuMainMapPointer.Image = global::ModflowOutputViewer.Properties.Resources.SelectArrow;
            this.menuMainMapPointer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainMapPointer.Name = "menuMainMapPointer";
            this.menuMainMapPointer.Size = new System.Drawing.Size(182, 22);
            this.menuMainMapPointer.Text = "Pointer Tool";
            this.menuMainMapPointer.Click += new System.EventHandler(this.menuMainMapPointer_Click);
            // 
            // menuMainMapReCenter
            // 
            this.menuMainMapReCenter.Image = global::ModflowOutputViewer.Properties.Resources.ReCenter;
            this.menuMainMapReCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainMapReCenter.Name = "menuMainMapReCenter";
            this.menuMainMapReCenter.Size = new System.Drawing.Size(182, 22);
            this.menuMainMapReCenter.Text = "Re-Center Tool";
            this.menuMainMapReCenter.Click += new System.EventHandler(this.menuMainMapReCenter_Click);
            // 
            // menuMainMapZoomIn
            // 
            this.menuMainMapZoomIn.Image = global::ModflowOutputViewer.Properties.Resources.ZoomIn;
            this.menuMainMapZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainMapZoomIn.Name = "menuMainMapZoomIn";
            this.menuMainMapZoomIn.Size = new System.Drawing.Size(182, 22);
            this.menuMainMapZoomIn.Text = "Zoom In Tool";
            this.menuMainMapZoomIn.Click += new System.EventHandler(this.menuMainMapZoomIn_Click);
            // 
            // menuMainMapZoomOut
            // 
            this.menuMainMapZoomOut.Image = global::ModflowOutputViewer.Properties.Resources.ZoomOut;
            this.menuMainMapZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainMapZoomOut.Name = "menuMainMapZoomOut";
            this.menuMainMapZoomOut.Size = new System.Drawing.Size(182, 22);
            this.menuMainMapZoomOut.Text = "Zoom Out Tool";
            this.menuMainMapZoomOut.Click += new System.EventHandler(this.menuMainMapZoomOut_Click);
            // 
            // menuMainMapZoomModelGrid
            // 
            this.menuMainMapZoomModelGrid.Image = global::ModflowOutputViewer.Properties.Resources.ZoomToGrid;
            this.menuMainMapZoomModelGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainMapZoomModelGrid.Name = "menuMainMapZoomModelGrid";
            this.menuMainMapZoomModelGrid.Size = new System.Drawing.Size(182, 22);
            this.menuMainMapZoomModelGrid.Text = "Zoom to Model Grid";
            this.menuMainMapZoomModelGrid.Click += new System.EventHandler(this.menuMainMapZoomModelGrid_Click);
            // 
            // menuMainMapZoomFullExtent
            // 
            this.menuMainMapZoomFullExtent.Image = global::ModflowOutputViewer.Properties.Resources.globe;
            this.menuMainMapZoomFullExtent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuMainMapZoomFullExtent.Name = "menuMainMapZoomFullExtent";
            this.menuMainMapZoomFullExtent.Size = new System.Drawing.Size(182, 22);
            this.menuMainMapZoomFullExtent.Text = "Zoom to Full Extent";
            this.menuMainMapZoomFullExtent.Click += new System.EventHandler(this.menuMainMapZoomFullExtent_Click);
            // 
            // toolStripButtonEditMetadata
            // 
            this.toolStripButtonEditMetadata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEditMetadata.Image = global::ModflowOutputViewer.Properties.Resources.EditMetadata;
            this.toolStripButtonEditMetadata.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditMetadata.Name = "toolStripButtonEditMetadata";
            this.toolStripButtonEditMetadata.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEditMetadata.Text = "Edit Metadata";
            this.toolStripButtonEditMetadata.ToolTipText = "Edit MODFLOW Metadata";
            this.toolStripButtonEditMetadata.Click += new System.EventHandler(this.toolStripButtonEditMetadata_Click);
            // 
            // legendMap
            // 
            this.legendMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.legendMap.BackColor = System.Drawing.Color.White;
            this.legendMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.legendMap.LegendTitle = "";
            this.legendMap.Location = new System.Drawing.Point(3, 23);
            this.legendMap.Name = "legendMap";
            this.legendMap.Size = new System.Drawing.Size(222, 306);
            this.legendMap.TabIndex = 0;
            // 
            // ModflowOutputViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 629);
            this.Controls.Add(this.splitConMain);
            this.Controls.Add(this.buttonToggleRightPanel);
            this.Controls.Add(this.buttonToggleLeftPanel);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.menuMain);
            this.MainMenuStrip = this.menuMain;
            this.Name = "ModflowOutputViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MODFLOW Head Viewer";
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.splitConMain.Panel1.ResumeLayout(false);
            this.splitConMain.Panel2.ResumeLayout(false);
            this.splitConMain.ResumeLayout(false);
            this.splitConLeftPanel.Panel1.ResumeLayout(false);
            this.splitConLeftPanel.Panel2.ResumeLayout(false);
            this.splitConLeftPanel.ResumeLayout(false);
            this.tabData.ResumeLayout(false);
            this.tabPageData.ResumeLayout(false);
            this.contextMenuData.ResumeLayout(false);
            this.tabPageReference.ResumeLayout(false);
            this.panelReference.ResumeLayout(false);
            this.panelReference.PerformLayout();
            this.tabPageAnalysis.ResumeLayout(false);
            this.tabPageAnalysis.PerformLayout();
            this.gboxContents.ResumeLayout(false);
            this.splitConMap.Panel1.ResumeLayout(false);
            this.splitConMap.Panel2.ResumeLayout(false);
            this.splitConMap.ResumeLayout(false);
            this.panelMapHeader.ResumeLayout(false);
            this.panelMapHeader.PerformLayout();
            this.splitConRightPanel.Panel1.ResumeLayout(false);
            this.splitConRightPanel.Panel2.ResumeLayout(false);
            this.splitConRightPanel.ResumeLayout(false);
            this.gboxMapLegend.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem menuMainFile;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel statusStripMainLocation;
        private System.Windows.Forms.ToolStripStatusLabel statusStripMainCellCoord;
        private System.Windows.Forms.ToolStripStatusLabel statusStripMainDataValue;
        private System.Windows.Forms.ToolStripStatusLabel statusStripMainAnalysisValue;
        private System.Windows.Forms.ToolStripStatusLabel statusStripMainDataFilePath;
        private System.Windows.Forms.Button buttonToggleLeftPanel;
        private System.Windows.Forms.Button buttonToggleRightPanel;
        private System.Windows.Forms.SplitContainer splitConMain;
        private System.Windows.Forms.SplitContainer splitConMap;
        private System.Windows.Forms.SplitContainer splitConLeftPanel;
        private System.Windows.Forms.SplitContainer splitConRightPanel;
        private System.Windows.Forms.GroupBox gboxMapLegend;
        private System.Windows.Forms.TabControl tabData;
        private System.Windows.Forms.TabPage tabPageData;
        private System.Windows.Forms.GroupBox gboxContents;
        private System.Windows.Forms.GroupBox gboxIndexMap;
        private USGS.Puma.UI.MapViewer.MapLegend legendMap;
        private System.Windows.Forms.TreeView tvwContents;
        private System.Windows.Forms.TreeView tvwData;
        private System.Windows.Forms.Panel panelMapHeader;
        private System.Windows.Forms.TabPage tabPageReference;
        private System.Windows.Forms.Button btnSelectReferenceLayer;
        private System.Windows.Forms.Label lblModelLayerLinkOption;
        private System.Windows.Forms.Label lblTimeStepLinkOption;
        private System.Windows.Forms.Label lblReferenceFileLabel;
        private System.Windows.Forms.Label lblReferenceLayerLabel;
        private System.Windows.Forms.Button btnSelectCurrentFile;
        private System.Windows.Forms.Button btnSelectReferenceFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonReCenter;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomIn;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomOut;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonFullExtent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowGridlines;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripButtonContourLayer;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomToGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripLabel toolStripLabelAnalysis;
        private System.Windows.Forms.ToolStripComboBox cboGriddedValuesDisplayOption;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripLabel toolStripLabelZoomTo;
        private System.Windows.Forms.ToolStripLabel toolStripLabelView;
        private System.Windows.Forms.ToolStripLabel toolStripLabelEdit;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileOpenDataset;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileCloseDataset;
        private System.Windows.Forms.ToolStripMenuItem menuMainCloseCurrentFile;
        private System.Windows.Forms.ToolStripSeparator menuMainFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileAddFile;
        private System.Windows.Forms.ToolStripSeparator menuMainFileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileAddBasemap;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileRemoveBasemap;
        private System.Windows.Forms.ToolStripSeparator menuMainFileSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileExport;
        private System.Windows.Forms.ToolStripSeparator menuMainFileSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuMainFilePrintPreview;
        private System.Windows.Forms.ToolStripMenuItem menuMainFilePrint;
        private System.Windows.Forms.ToolStripMenuItem menuMainFilePrintPDF;
        private System.Windows.Forms.ToolStripSeparator menuMainFileSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileExportCurrentFileXml;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileExportShapefiles;
        private System.Windows.Forms.ToolStripMenuItem menuMainEdit;
        private System.Windows.Forms.ToolStripMenuItem menuMainView;
        private System.Windows.Forms.ToolStripMenuItem menuMainMap;
        private System.Windows.Forms.ToolStripMenuItem menuMainHelp;
        private System.Windows.Forms.ToolStripMenuItem menuMainEditContourLayer;
        private System.Windows.Forms.ToolStripMenuItem menuMainEditDataValues;
        private System.Windows.Forms.ToolStripMenuItem menuMainEditAnalysis;
        private System.Windows.Forms.ToolStripMenuItem menuMainEditBasemap;
        private System.Windows.Forms.ToolStripMenuItem menuMainMapPointer;
        private System.Windows.Forms.ToolStripMenuItem menuMainMapReCenter;
        private System.Windows.Forms.ToolStripMenuItem menuMainMapZoomIn;
        private System.Windows.Forms.ToolStripMenuItem menuMainMapZoomOut;
        private System.Windows.Forms.ToolStripSeparator menuMainMapSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuMainMapZoomModelGrid;
        private System.Windows.Forms.ToolStripMenuItem menuMainMapZoomFullExtent;
        private System.Windows.Forms.ToolStripMenuItem menuMainHelpAbout;
        private System.Windows.Forms.Label lblCurrentLayerDescription;
        private System.Windows.Forms.ImageList imageListMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditBasemap;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButtonEditGriddedValues;
        private System.Windows.Forms.ToolStripMenuItem toolStripButtonEditCurrentDataLayer;
        private System.Windows.Forms.ToolStripMenuItem toolStripButtonEditReferenceDataLayer;
        private System.Windows.Forms.ToolStripMenuItem toolStripButtonEditAnalysisLayer;
        private System.Windows.Forms.TabPage tabPageAnalysis;
        private System.Windows.Forms.TextBox txtAnalysisDescription;
        private System.Windows.Forms.Label lblAnalysisOption;
        private System.Windows.Forms.ComboBox cboAnalysisOption;
        private System.Windows.Forms.Label lblReferenceLayer;
        private System.Windows.Forms.Label lblReferenceFile;
        private System.Windows.Forms.Panel panelReference;
        private System.Windows.Forms.ToolStripButton toolStripButtonHideSidePanels;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowSidePanels;
        private System.Windows.Forms.ToolStripMenuItem menuMainViewHideSidePanels;
        private System.Windows.Forms.ToolStripMenuItem menuMainViewShowSidePanels;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileSaveBinaryOutput;
        private System.Windows.Forms.ContextMenuStrip contextMenuData;
        private System.Windows.Forms.ToolStripMenuItem contextMenuDataEditExcludedValues;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileNewBasemap;
        private System.Windows.Forms.ToolStripMenuItem menuMainFileSaveBasemap;
        private System.Windows.Forms.ToolStripMenuItem menuMainEditModflowMetadata;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditMetadata;

    }
}

