namespace ScenarioManager
{
    partial class SMMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SMMainForm));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menutItemFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProjectModelGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSimulationStartTime = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditAddNewScenario = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditRename = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditSelectedItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditCopyScenario = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditAddNewPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditAddNewRiverPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditAddNewWellPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditAddNewChdPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditAddNewRchPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditAddNewGhbPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditAddNewFeatureSet = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemView = new System.Windows.Forms.ToolStripMenuItem();
            this.descriptionOfSelectedElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemImport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemImportModflowWellPackageFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemImportModflowRiverPackageFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExportScenario = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExportScenarioAndRunSimulation = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExportAndRunAllScenarios = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExportPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuRename = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuCopyScenario = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuAddPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuAddRiverPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuAddWellPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuAddChdPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuAddRchPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuAddGhbPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuAddNewFeatureSet = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnGetUnitFromNameFile = new System.Windows.Forms.Button();
            this.textBoxCbcFlag = new System.Windows.Forms.TextBox();
            this.labelCbcFlag = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblNam = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuMouseDown = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemMouseDownAddScenario = new System.Windows.Forms.ToolStripMenuItem();
            this.imageMakerBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.openScenarioManagerHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenu.SuspendLayout();
            this.contextMenuStripNode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.contextMenuMouseDown.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Font = new System.Drawing.Font("Tahoma", 12F);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile,
            this.menuItemProject,
            this.menuItemEdit,
            this.menuItemView,
            this.menuItemImport,
            this.menuItemExport,
            this.helpToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Padding = new System.Windows.Forms.Padding(9, 2, 2, 2);
            this.MainMenu.Size = new System.Drawing.Size(685, 32);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Text = "menuStrip1";
            // 
            // menuItemFile
            // 
            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFileNew,
            this.menuItemFileOpen,
            this.menuItemFileSave,
            this.menutItemFileSaveAs,
            this.toolStripMenuItem1,
            this.menuItemFileExit});
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(53, 28);
            this.menuItemFile.Text = "File";
            this.menuItemFile.DropDownOpening += new System.EventHandler(this.menuItemFile_DropDownOpening);
            // 
            // menuItemFileNew
            // 
            this.menuItemFileNew.Name = "menuItemFileNew";
            this.menuItemFileNew.Size = new System.Drawing.Size(215, 28);
            this.menuItemFileNew.Text = "New";
            this.menuItemFileNew.Click += new System.EventHandler(this.menuItemFileNew_Click);
            // 
            // menuItemFileOpen
            // 
            this.menuItemFileOpen.Name = "menuItemFileOpen";
            this.menuItemFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuItemFileOpen.Size = new System.Drawing.Size(215, 28);
            this.menuItemFileOpen.Text = "Open...";
            this.menuItemFileOpen.Click += new System.EventHandler(this.menuItemFileOpen_Click);
            // 
            // menuItemFileSave
            // 
            this.menuItemFileSave.Name = "menuItemFileSave";
            this.menuItemFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuItemFileSave.Size = new System.Drawing.Size(215, 28);
            this.menuItemFileSave.Text = "Save";
            this.menuItemFileSave.Click += new System.EventHandler(this.menuItemFileSave_Click);
            // 
            // menutItemFileSaveAs
            // 
            this.menutItemFileSaveAs.Name = "menutItemFileSaveAs";
            this.menutItemFileSaveAs.Size = new System.Drawing.Size(215, 28);
            this.menutItemFileSaveAs.Text = "Save As...";
            this.menutItemFileSaveAs.Click += new System.EventHandler(this.menuItemFileSaveAs_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(212, 6);
            // 
            // menuItemFileExit
            // 
            this.menuItemFileExit.Name = "menuItemFileExit";
            this.menuItemFileExit.Size = new System.Drawing.Size(215, 28);
            this.menuItemFileExit.Text = "Exit";
            this.menuItemFileExit.Click += new System.EventHandler(this.menuItemFileExit_Click);
            // 
            // menuItemProject
            // 
            this.menuItemProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemProjectSettings,
            this.menuItemProjectModelGrid,
            this.menuItemSimulationStartTime});
            this.menuItemProject.Name = "menuItemProject";
            this.menuItemProject.Size = new System.Drawing.Size(84, 28);
            this.menuItemProject.Text = "Project";
            this.menuItemProject.DropDownOpening += new System.EventHandler(this.menuItemProject_DropDownOpening);
            // 
            // menuItemProjectSettings
            // 
            this.menuItemProjectSettings.Name = "menuItemProjectSettings";
            this.menuItemProjectSettings.Size = new System.Drawing.Size(292, 28);
            this.menuItemProjectSettings.Text = "Settings...";
            this.menuItemProjectSettings.Click += new System.EventHandler(this.menuItemProjectSettings_Click);
            // 
            // menuItemProjectModelGrid
            // 
            this.menuItemProjectModelGrid.Name = "menuItemProjectModelGrid";
            this.menuItemProjectModelGrid.Size = new System.Drawing.Size(292, 28);
            this.menuItemProjectModelGrid.Text = "Model Grid...";
            this.menuItemProjectModelGrid.Click += new System.EventHandler(this.menuItemProjectModelGrid_Click);
            // 
            // menuItemSimulationStartTime
            // 
            this.menuItemSimulationStartTime.Name = "menuItemSimulationStartTime";
            this.menuItemSimulationStartTime.Size = new System.Drawing.Size(292, 28);
            this.menuItemSimulationStartTime.Text = "Simulation Start Time...";
            this.menuItemSimulationStartTime.Click += new System.EventHandler(this.menuItemProjectSimulationStartTime_Click);
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemEditAddNewScenario,
            this.menuItemEditRename,
            this.menuItemEditSelectedItem,
            this.menuItemEditCut,
            this.menuItemEditCopy,
            this.menuItemEditPaste,
            this.menuItemEditDelete,
            this.menuItemEditCopyScenario,
            this.menuItemEditUndo,
            this.menuItemEditRedo,
            this.menuItemEditAddNewPackage,
            this.menuItemEditAddNewFeatureSet});
            this.menuItemEdit.Name = "menuItemEdit";
            this.menuItemEdit.Size = new System.Drawing.Size(56, 28);
            this.menuItemEdit.Text = "Edit";
            this.menuItemEdit.DropDownOpening += new System.EventHandler(this.menuItemEdit_DropDownOpening);
            // 
            // menuItemEditAddNewScenario
            // 
            this.menuItemEditAddNewScenario.Name = "menuItemEditAddNewScenario";
            this.menuItemEditAddNewScenario.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuItemEditAddNewScenario.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditAddNewScenario.Text = "Add New Scenario";
            this.menuItemEditAddNewScenario.Click += new System.EventHandler(this.menuItemEditAddScenario_Click);
            // 
            // menuItemEditRename
            // 
            this.menuItemEditRename.Name = "menuItemEditRename";
            this.menuItemEditRename.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditRename.Text = "Rename...";
            this.menuItemEditRename.Click += new System.EventHandler(this.menuItemEditRename_Click);
            // 
            // menuItemEditSelectedItem
            // 
            this.menuItemEditSelectedItem.Name = "menuItemEditSelectedItem";
            this.menuItemEditSelectedItem.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditSelectedItem.Text = "Edit Selected Item...";
            this.menuItemEditSelectedItem.Click += new System.EventHandler(this.menuItemEditSelectedItem_Click);
            // 
            // menuItemEditCut
            // 
            this.menuItemEditCut.Name = "menuItemEditCut";
            this.menuItemEditCut.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditCut.Text = "Cut";
            this.menuItemEditCut.Click += new System.EventHandler(this.menuItemEditCut_Click);
            // 
            // menuItemEditCopy
            // 
            this.menuItemEditCopy.Name = "menuItemEditCopy";
            this.menuItemEditCopy.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditCopy.Text = "Copy";
            this.menuItemEditCopy.Click += new System.EventHandler(this.menuItemEditCopy_Click);
            // 
            // menuItemEditPaste
            // 
            this.menuItemEditPaste.Name = "menuItemEditPaste";
            this.menuItemEditPaste.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditPaste.Text = "Paste";
            this.menuItemEditPaste.Click += new System.EventHandler(this.menuItemEditPaste_Click);
            // 
            // menuItemEditDelete
            // 
            this.menuItemEditDelete.Name = "menuItemEditDelete";
            this.menuItemEditDelete.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditDelete.Text = "Delete";
            this.menuItemEditDelete.Click += new System.EventHandler(this.menuItemEditDelete_Click);
            // 
            // menuItemEditCopyScenario
            // 
            this.menuItemEditCopyScenario.Name = "menuItemEditCopyScenario";
            this.menuItemEditCopyScenario.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditCopyScenario.Text = "Duplicate Scenario";
            this.menuItemEditCopyScenario.Click += new System.EventHandler(this.menuItemEditCopyScenario_Click);
            // 
            // menuItemEditUndo
            // 
            this.menuItemEditUndo.Enabled = false;
            this.menuItemEditUndo.Name = "menuItemEditUndo";
            this.menuItemEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.menuItemEditUndo.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditUndo.Text = "Undo";
            this.menuItemEditUndo.Click += new System.EventHandler(this.menuItemEditUndo_Click);
            // 
            // menuItemEditRedo
            // 
            this.menuItemEditRedo.Enabled = false;
            this.menuItemEditRedo.Name = "menuItemEditRedo";
            this.menuItemEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.menuItemEditRedo.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditRedo.Text = "Redo";
            this.menuItemEditRedo.Click += new System.EventHandler(this.menuItemEditRedo_Click);
            // 
            // menuItemEditAddNewPackage
            // 
            this.menuItemEditAddNewPackage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemEditAddNewRiverPackage,
            this.menuItemEditAddNewWellPackage,
            this.menuItemEditAddNewChdPackage,
            this.menuItemEditAddNewRchPackage,
            this.menuItemEditAddNewGhbPackage});
            this.menuItemEditAddNewPackage.Name = "menuItemEditAddNewPackage";
            this.menuItemEditAddNewPackage.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditAddNewPackage.Text = "Add New Package";
            // 
            // menuItemEditAddNewRiverPackage
            // 
            this.menuItemEditAddNewRiverPackage.Name = "menuItemEditAddNewRiverPackage";
            this.menuItemEditAddNewRiverPackage.Size = new System.Drawing.Size(243, 28);
            this.menuItemEditAddNewRiverPackage.Text = "River Package";
            this.menuItemEditAddNewRiverPackage.Click += new System.EventHandler(this.menuItemEditAddNewRiverPackage_Click);
            // 
            // menuItemEditAddNewWellPackage
            // 
            this.menuItemEditAddNewWellPackage.Name = "menuItemEditAddNewWellPackage";
            this.menuItemEditAddNewWellPackage.Size = new System.Drawing.Size(243, 28);
            this.menuItemEditAddNewWellPackage.Text = "Well Package";
            this.menuItemEditAddNewWellPackage.Click += new System.EventHandler(this.menuItemEditAddNewWellPackage_Click);
            // 
            // menuItemEditAddNewChdPackage
            // 
            this.menuItemEditAddNewChdPackage.Name = "menuItemEditAddNewChdPackage";
            this.menuItemEditAddNewChdPackage.Size = new System.Drawing.Size(243, 28);
            this.menuItemEditAddNewChdPackage.Text = "CHD Package";
            this.menuItemEditAddNewChdPackage.Click += new System.EventHandler(this.menuItemEditAddNewChdPackage_Click);
            // 
            // menuItemEditAddNewRchPackage
            // 
            this.menuItemEditAddNewRchPackage.Name = "menuItemEditAddNewRchPackage";
            this.menuItemEditAddNewRchPackage.Size = new System.Drawing.Size(243, 28);
            this.menuItemEditAddNewRchPackage.Text = "Recharge Package";
            this.menuItemEditAddNewRchPackage.Click += new System.EventHandler(this.menuItemEditAddNewRchPackage_Click);
            // 
            // menuItemEditAddNewGhbPackage
            // 
            this.menuItemEditAddNewGhbPackage.Name = "menuItemEditAddNewGhbPackage";
            this.menuItemEditAddNewGhbPackage.Size = new System.Drawing.Size(243, 28);
            this.menuItemEditAddNewGhbPackage.Text = "GHB Package";
            this.menuItemEditAddNewGhbPackage.Click += new System.EventHandler(this.menuItemEditAddNewGhbPackage_Click);
            // 
            // menuItemEditAddNewFeatureSet
            // 
            this.menuItemEditAddNewFeatureSet.Name = "menuItemEditAddNewFeatureSet";
            this.menuItemEditAddNewFeatureSet.Size = new System.Drawing.Size(310, 28);
            this.menuItemEditAddNewFeatureSet.Text = "Add New Feature Set";
            this.menuItemEditAddNewFeatureSet.Click += new System.EventHandler(this.menuItemEditAddNewFeatureSet_Click);
            // 
            // menuItemView
            // 
            this.menuItemView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.descriptionOfSelectedElementToolStripMenuItem,
            this.menuItemViewRefresh});
            this.menuItemView.Name = "menuItemView";
            this.menuItemView.Size = new System.Drawing.Size(65, 28);
            this.menuItemView.Text = "View";
            this.menuItemView.DropDownOpening += new System.EventHandler(this.menuItemView_DropDownOpening);
            // 
            // descriptionOfSelectedElementToolStripMenuItem
            // 
            this.descriptionOfSelectedElementToolStripMenuItem.Name = "descriptionOfSelectedElementToolStripMenuItem";
            this.descriptionOfSelectedElementToolStripMenuItem.Size = new System.Drawing.Size(400, 28);
            this.descriptionOfSelectedElementToolStripMenuItem.Text = "Documentation of Selected Element";
            this.descriptionOfSelectedElementToolStripMenuItem.Click += new System.EventHandler(this.documentationOfSelectedElementToolStripMenuItem_Click);
            // 
            // menuItemViewRefresh
            // 
            this.menuItemViewRefresh.Name = "menuItemViewRefresh";
            this.menuItemViewRefresh.Size = new System.Drawing.Size(400, 28);
            this.menuItemViewRefresh.Text = "Refresh";
            this.menuItemViewRefresh.Click += new System.EventHandler(this.menuItemViewRefresh_Click);
            // 
            // menuItemImport
            // 
            this.menuItemImport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemImportModflowWellPackageFile,
            this.menuItemImportModflowRiverPackageFile});
            this.menuItemImport.Enabled = false;
            this.menuItemImport.Name = "menuItemImport";
            this.menuItemImport.Size = new System.Drawing.Size(82, 28);
            this.menuItemImport.Text = "Import";
            this.menuItemImport.Visible = false;
            this.menuItemImport.DropDownOpening += new System.EventHandler(this.menuItemImport_DropDownOpening);
            // 
            // menuItemImportModflowWellPackageFile
            // 
            this.menuItemImportModflowWellPackageFile.Enabled = false;
            this.menuItemImportModflowWellPackageFile.Name = "menuItemImportModflowWellPackageFile";
            this.menuItemImportModflowWellPackageFile.Size = new System.Drawing.Size(361, 28);
            this.menuItemImportModflowWellPackageFile.Text = "MODFLOW Well Package File...";
            // 
            // menuItemImportModflowRiverPackageFile
            // 
            this.menuItemImportModflowRiverPackageFile.Enabled = false;
            this.menuItemImportModflowRiverPackageFile.Name = "menuItemImportModflowRiverPackageFile";
            this.menuItemImportModflowRiverPackageFile.Size = new System.Drawing.Size(361, 28);
            this.menuItemImportModflowRiverPackageFile.Text = "MODFLOW River Package File...";
            // 
            // menuItemExport
            // 
            this.menuItemExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemExportScenario,
            this.menuItemExportScenarioAndRunSimulation,
            this.menuItemExportAndRunAllScenarios,
            this.menuItemExportPackage});
            this.menuItemExport.Name = "menuItemExport";
            this.menuItemExport.Size = new System.Drawing.Size(79, 28);
            this.menuItemExport.Text = "Export";
            this.menuItemExport.DropDownOpening += new System.EventHandler(this.menuItemExport_DropDownOpening);
            this.menuItemExport.Click += new System.EventHandler(this.menuItemExport_Click);
            // 
            // menuItemExportScenario
            // 
            this.menuItemExportScenario.Name = "menuItemExportScenario";
            this.menuItemExportScenario.Size = new System.Drawing.Size(398, 28);
            this.menuItemExportScenario.Text = "Export Scenario";
            this.menuItemExportScenario.Click += new System.EventHandler(this.menuItemExportScenario_Click);
            // 
            // menuItemExportScenarioAndRunSimulation
            // 
            this.menuItemExportScenarioAndRunSimulation.Name = "menuItemExportScenarioAndRunSimulation";
            this.menuItemExportScenarioAndRunSimulation.Size = new System.Drawing.Size(398, 28);
            this.menuItemExportScenarioAndRunSimulation.Text = "Export Scenario and Run Simulation";
            this.menuItemExportScenarioAndRunSimulation.Click += new System.EventHandler(this.menuItemExportScenarioAndRunSimulation_Click);
            // 
            // menuItemExportAndRunAllScenarios
            // 
            this.menuItemExportAndRunAllScenarios.Name = "menuItemExportAndRunAllScenarios";
            this.menuItemExportAndRunAllScenarios.Size = new System.Drawing.Size(398, 28);
            this.menuItemExportAndRunAllScenarios.Text = "Export and Run All Scenarios";
            this.menuItemExportAndRunAllScenarios.Click += new System.EventHandler(this.menuItemExportAndRunAllScenarios_Click);
            // 
            // menuItemExportPackage
            // 
            this.menuItemExportPackage.Enabled = false;
            this.menuItemExportPackage.Name = "menuItemExportPackage";
            this.menuItemExportPackage.Size = new System.Drawing.Size(398, 28);
            this.menuItemExportPackage.Text = "Export Package";
            this.menuItemExportPackage.Click += new System.EventHandler(this.menuItemExportPackage_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openScenarioManagerHelpToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(63, 28);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(339, 28);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // contextMenuStripNode
            // 
            this.contextMenuStripNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuRename,
            this.contextMenuEdit,
            this.contextMenuCut,
            this.contextMenuCopy,
            this.contextMenuPaste,
            this.contextMenuDelete,
            this.contextMenuCopyScenario,
            this.contextMenuAddPackage,
            this.contextMenuAddNewFeatureSet});
            this.contextMenuStripNode.Name = "contextMenuStrip1";
            this.contextMenuStripNode.Size = new System.Drawing.Size(219, 220);
            this.contextMenuStripNode.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripNode_Opening);
            // 
            // contextMenuRename
            // 
            this.contextMenuRename.Name = "contextMenuRename";
            this.contextMenuRename.Size = new System.Drawing.Size(218, 24);
            this.contextMenuRename.Text = "Rename...";
            this.contextMenuRename.Click += new System.EventHandler(this.contextMenuRename_Click);
            // 
            // contextMenuEdit
            // 
            this.contextMenuEdit.Name = "contextMenuEdit";
            this.contextMenuEdit.Size = new System.Drawing.Size(218, 24);
            this.contextMenuEdit.Text = "Edit...";
            this.contextMenuEdit.Click += new System.EventHandler(this.contextMenuEdit_Click);
            // 
            // contextMenuCut
            // 
            this.contextMenuCut.Name = "contextMenuCut";
            this.contextMenuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.contextMenuCut.Size = new System.Drawing.Size(218, 24);
            this.contextMenuCut.Text = "Cut";
            this.contextMenuCut.Click += new System.EventHandler(this.contextMenuCut_Click);
            // 
            // contextMenuCopy
            // 
            this.contextMenuCopy.Name = "contextMenuCopy";
            this.contextMenuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.contextMenuCopy.Size = new System.Drawing.Size(218, 24);
            this.contextMenuCopy.Text = "Copy";
            this.contextMenuCopy.Click += new System.EventHandler(this.contextMenuCopy_Click);
            // 
            // contextMenuPaste
            // 
            this.contextMenuPaste.Enabled = false;
            this.contextMenuPaste.Name = "contextMenuPaste";
            this.contextMenuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.contextMenuPaste.Size = new System.Drawing.Size(218, 24);
            this.contextMenuPaste.Text = "Paste";
            this.contextMenuPaste.Click += new System.EventHandler(this.contextMenuPaste_Click);
            // 
            // contextMenuDelete
            // 
            this.contextMenuDelete.Name = "contextMenuDelete";
            this.contextMenuDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.contextMenuDelete.Size = new System.Drawing.Size(218, 24);
            this.contextMenuDelete.Text = "Delete";
            this.contextMenuDelete.Click += new System.EventHandler(this.contextMenuDelete_Click);
            // 
            // contextMenuCopyScenario
            // 
            this.contextMenuCopyScenario.Name = "contextMenuCopyScenario";
            this.contextMenuCopyScenario.Size = new System.Drawing.Size(218, 24);
            this.contextMenuCopyScenario.Text = "Duplicate Scenario";
            this.contextMenuCopyScenario.Click += new System.EventHandler(this.contextMenuCopyScenario_Click);
            // 
            // contextMenuAddPackage
            // 
            this.contextMenuAddPackage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuAddRiverPackage,
            this.contextMenuAddWellPackage,
            this.contextMenuAddChdPackage,
            this.contextMenuAddRchPackage,
            this.contextMenuAddGhbPackage});
            this.contextMenuAddPackage.Name = "contextMenuAddPackage";
            this.contextMenuAddPackage.Size = new System.Drawing.Size(218, 24);
            this.contextMenuAddPackage.Text = "Add New Package...";
            // 
            // contextMenuAddRiverPackage
            // 
            this.contextMenuAddRiverPackage.Name = "contextMenuAddRiverPackage";
            this.contextMenuAddRiverPackage.Size = new System.Drawing.Size(196, 22);
            this.contextMenuAddRiverPackage.Text = "River Package";
            this.contextMenuAddRiverPackage.Click += new System.EventHandler(this.contextMenuAddRiverPackage_Click);
            // 
            // contextMenuAddWellPackage
            // 
            this.contextMenuAddWellPackage.Name = "contextMenuAddWellPackage";
            this.contextMenuAddWellPackage.Size = new System.Drawing.Size(196, 22);
            this.contextMenuAddWellPackage.Text = "Well Package";
            this.contextMenuAddWellPackage.Click += new System.EventHandler(this.contextMenuAddWellPackage_Click);
            // 
            // contextMenuAddChdPackage
            // 
            this.contextMenuAddChdPackage.Name = "contextMenuAddChdPackage";
            this.contextMenuAddChdPackage.Size = new System.Drawing.Size(196, 22);
            this.contextMenuAddChdPackage.Text = "CHD Package";
            this.contextMenuAddChdPackage.Click += new System.EventHandler(this.contextMenuAddChdPackage_Click);
            // 
            // contextMenuAddRchPackage
            // 
            this.contextMenuAddRchPackage.Name = "contextMenuAddRchPackage";
            this.contextMenuAddRchPackage.Size = new System.Drawing.Size(196, 22);
            this.contextMenuAddRchPackage.Text = "Recharge Package";
            this.contextMenuAddRchPackage.Click += new System.EventHandler(this.contextMenuAddRchPackage_Click);
            // 
            // contextMenuAddGhbPackage
            // 
            this.contextMenuAddGhbPackage.Name = "contextMenuAddGhbPackage";
            this.contextMenuAddGhbPackage.Size = new System.Drawing.Size(196, 22);
            this.contextMenuAddGhbPackage.Text = "GHB Package";
            this.contextMenuAddGhbPackage.Click += new System.EventHandler(this.contextMenuAddGhbPackage_Click);
            // 
            // contextMenuAddNewFeatureSet
            // 
            this.contextMenuAddNewFeatureSet.Name = "contextMenuAddNewFeatureSet";
            this.contextMenuAddNewFeatureSet.Size = new System.Drawing.Size(218, 24);
            this.contextMenuAddNewFeatureSet.Text = "Add New Feature Set";
            this.contextMenuAddNewFeatureSet.Click += new System.EventHandler(this.contextMenuAddNewFeatureSet_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 32);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1MinSize = 85;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnGetUnitFromNameFile);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxCbcFlag);
            this.splitContainer1.Panel2.Controls.Add(this.labelCbcFlag);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.lblNam);
            this.splitContainer1.Panel2.Controls.Add(this.tbDescription);
            this.splitContainer1.Panel2.Controls.Add(this.lblName);
            this.splitContainer1.Panel2.Controls.Add(this.lblTitle);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 1, 1);
            this.splitContainer1.Panel2MinSize = 365;
            this.splitContainer1.Size = new System.Drawing.Size(685, 278);
            this.splitContainer1.SplitterDistance = 292;
            this.splitContainer1.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.HotTracking = true;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(292, 278);
            this.treeView1.TabIndex = 2;
            this.treeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_BeforeLabelEdit);
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.Click += new System.EventHandler(this.treeView1_Click);
            this.treeView1.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.treeView1_ControlAdded);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ScenarioManager.ico");
            this.imageList1.Images.SetKeyName(1, "WellPumpIcons.ico");
            this.imageList1.Images.SetKeyName(2, "RivIcon.ico");
            this.imageList1.Images.SetKeyName(3, "ChdIcon.ico");
            this.imageList1.Images.SetKeyName(4, "RchIcon.ico");
            this.imageList1.Images.SetKeyName(5, "GhbIconSet.ico");
            this.imageList1.Images.SetKeyName(6, "usgs_icon.ico");
            // 
            // btnGetUnitFromNameFile
            // 
            this.btnGetUnitFromNameFile.Location = new System.Drawing.Point(255, 65);
            this.btnGetUnitFromNameFile.Name = "btnGetUnitFromNameFile";
            this.btnGetUnitFromNameFile.Size = new System.Drawing.Size(108, 30);
            this.btnGetUnitFromNameFile.TabIndex = 11;
            this.btnGetUnitFromNameFile.Text = "Choose...";
            this.btnGetUnitFromNameFile.UseVisualStyleBackColor = true;
            this.btnGetUnitFromNameFile.Click += new System.EventHandler(this.btnGetUnitFromNameFile_Click);
            // 
            // textBoxCbcFlag
            // 
            this.textBoxCbcFlag.Location = new System.Drawing.Point(137, 65);
            this.textBoxCbcFlag.MaxLength = 10;
            this.textBoxCbcFlag.Name = "textBoxCbcFlag";
            this.textBoxCbcFlag.Size = new System.Drawing.Size(112, 30);
            this.textBoxCbcFlag.TabIndex = 10;
            this.textBoxCbcFlag.Enter += new System.EventHandler(this.textBoxCbcFlag_Enter);
            this.textBoxCbcFlag.Leave += new System.EventHandler(this.textBoxCbcFlag_Leave);
            // 
            // labelCbcFlag
            // 
            this.labelCbcFlag.AutoSize = true;
            this.labelCbcFlag.Location = new System.Drawing.Point(17, 68);
            this.labelCbcFlag.Name = "labelCbcFlag";
            this.labelCbcFlag.Size = new System.Drawing.Size(99, 25);
            this.labelCbcFlag.TabIndex = 9;
            this.labelCbcFlag.Text = "CBCFlag:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 25);
            this.label1.TabIndex = 8;
            this.label1.Text = "Description";
            // 
            // lblNam
            // 
            this.lblNam.AutoSize = true;
            this.lblNam.Location = new System.Drawing.Point(17, 35);
            this.lblNam.Name = "lblNam";
            this.lblNam.Size = new System.Drawing.Size(70, 25);
            this.lblNam.TabIndex = 7;
            this.lblNam.Text = "Name:";
            // 
            // tbDescription
            // 
            this.tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDescription.Enabled = false;
            this.tbDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDescription.Location = new System.Drawing.Point(2, 126);
            this.tbDescription.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(386, 150);
            this.tbDescription.TabIndex = 4;
            this.tbDescription.Click += new System.EventHandler(this.tbDescription_Click);
            this.tbDescription.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbDescription_MouseClick);
            this.tbDescription.TextChanged += new System.EventHandler(this.tbDescription_TextChanged);
            this.tbDescription.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDescription_MouseDown);
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.Location = new System.Drawing.Point(132, 35);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(244, 25);
            this.lblName.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(74, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(179, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Scenario Summary";
            // 
            // statusStrip
            // 
            this.statusStrip.Font = new System.Drawing.Font("Tahoma", 9F);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.StatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 312);
            this.statusStrip.MinimumSize = new System.Drawing.Size(0, 30);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.Size = new System.Drawing.Size(685, 30);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            this.statusStrip.Click += new System.EventHandler(this.statusStrip_Click);
            // 
            // progressBar
            // 
            this.progressBar.Margin = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(281, 26);
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(82, 25);
            this.StatusLabel.Text = "StatusLabel";
            // 
            // contextMenuMouseDown
            // 
            this.contextMenuMouseDown.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemMouseDownAddScenario});
            this.contextMenuMouseDown.Name = "contextMenuMouseDown";
            this.contextMenuMouseDown.Size = new System.Drawing.Size(211, 28);
            // 
            // menuItemMouseDownAddScenario
            // 
            this.menuItemMouseDownAddScenario.Name = "menuItemMouseDownAddScenario";
            this.menuItemMouseDownAddScenario.Size = new System.Drawing.Size(210, 24);
            this.menuItemMouseDownAddScenario.Text = "Add New Scenario...";
            this.menuItemMouseDownAddScenario.Click += new System.EventHandler(this.menuItemMouseDownAddScenario_Click);
            // 
            // imageMakerBackgroundWorker
            // 
            this.imageMakerBackgroundWorker.WorkerReportsProgress = true;
            this.imageMakerBackgroundWorker.WorkerSupportsCancellation = true;
            this.imageMakerBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.imageMakerBackgroundWorker_DoWork);
            this.imageMakerBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.imageMakerBackgroundWorker_ProgressChanged);
            this.imageMakerBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.imageMakerBackgroundWorker_RunWorkerCompleted);
            // 
            // openScenarioManagerHelpToolStripMenuItem
            // 
            this.openScenarioManagerHelpToolStripMenuItem.Name = "openScenarioManagerHelpToolStripMenuItem";
            this.openScenarioManagerHelpToolStripMenuItem.Size = new System.Drawing.Size(286, 28);
            this.openScenarioManagerHelpToolStripMenuItem.Text = "Scenario Manager Help";
            this.openScenarioManagerHelpToolStripMenuItem.Click += new System.EventHandler(this.scenarioManagerHelpToolStripMenuItem_Click);
            // 
            // SMMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 342);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.MainMenu);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainMenu;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(480, 240);
            this.Name = "SMMainForm";
            this.Text = "USGS Scenario Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.contextMenuStripNode.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.contextMenuMouseDown.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItemFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileNew;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuItemEdit;
        private System.Windows.Forms.ToolStripMenuItem menuItemExport;
        private System.Windows.Forms.ToolStripMenuItem menuItemView;
        private System.Windows.Forms.ToolStripMenuItem menutItemFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuItemImport;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemImportModflowWellPackageFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemImportModflowRiverPackageFile;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNode;
        private System.Windows.Forms.ToolStripMenuItem contextMenuCut;
        private System.Windows.Forms.ToolStripMenuItem contextMenuCopy;
        private System.Windows.Forms.ToolStripMenuItem contextMenuPaste;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditAddNewScenario;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripMenuItem contextMenuCopyScenario;
        private System.Windows.Forms.ToolStripMenuItem contextMenuDelete;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem contextMenuAddPackage;
        private System.Windows.Forms.ToolStripMenuItem contextMenuAddWellPackage;
        private System.Windows.Forms.ToolStripMenuItem contextMenuAddRiverPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditUndo;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditRedo;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label lblNam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem contextMenuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuItemProjectModelGrid;
        private System.Windows.Forms.ToolStripMenuItem menuItemSimulationStartTime;
        private System.Windows.Forms.ToolStripMenuItem menuItemProject;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewRefresh;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripMenuItem menuItemProjectSettings;
        private System.Windows.Forms.ContextMenuStrip contextMenuMouseDown;
        private System.Windows.Forms.ToolStripMenuItem menuItemMouseDownAddScenario;
        private System.Windows.Forms.ToolStripMenuItem contextMenuRename;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportScenario;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportScenarioAndRunSimulation;
        private System.Windows.Forms.ToolStripMenuItem contextMenuAddChdPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditAddNewPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditAddNewRiverPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditAddNewWellPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditAddNewChdPackage;
        private System.Windows.Forms.ToolStripMenuItem contextMenuAddNewFeatureSet;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditAddNewFeatureSet;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditRename;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditSelectedItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditCut;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditCopy;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditPaste;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditDelete;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditCopyScenario;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportAndRunAllScenarios;
        private System.ComponentModel.BackgroundWorker imageMakerBackgroundWorker;
        private System.Windows.Forms.ToolStripMenuItem descriptionOfSelectedElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contextMenuAddRchPackage;
        private System.Windows.Forms.ToolStripMenuItem contextMenuAddGhbPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditAddNewRchPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditAddNewGhbPackage;
        private System.Windows.Forms.Label labelCbcFlag;
        private System.Windows.Forms.Button btnGetUnitFromNameFile;
        private System.Windows.Forms.TextBox textBoxCbcFlag;
        private System.Windows.Forms.ToolStripMenuItem openScenarioManagerHelpToolStripMenuItem;
    }
}

