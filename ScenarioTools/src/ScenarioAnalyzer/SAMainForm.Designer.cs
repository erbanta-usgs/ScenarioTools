namespace ScenarioAnalyzer
{
    sealed partial class SAMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAMainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileMenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenuExportPDF = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileMenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.rereadRecomputeAllDisplayElementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rereadRecomputeSelectedElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewSTMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditAddNewChart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditAddNewTable = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.editMenuElementProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuMapLayerProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuDataSeriesProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditAddNewContourLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditAddNewColorFillLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditAddNewDataSeries = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openScenarioAnalyzerHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleAutoRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabelPrimary = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.genericBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Font = new System.Drawing.Font("Tahoma", 11F);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.menuItemProject,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(9, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(784, 31);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            this.menuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuNew,
            this.fileMenuOpen,
            this.toolStripSeparator1,
            this.fileMenuSave,
            this.fileMenuSaveAs,
            this.fileMenuExportPDF,
            this.toolStripSeparator2,
            this.fileMenuExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 27);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // fileMenuNew
            // 
            this.fileMenuNew.Name = "fileMenuNew";
            this.fileMenuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.fileMenuNew.Size = new System.Drawing.Size(213, 28);
            this.fileMenuNew.Text = "&New";
            this.fileMenuNew.Click += new System.EventHandler(this.fileMenuNew_Click);
            // 
            // fileMenuOpen
            // 
            this.fileMenuOpen.Name = "fileMenuOpen";
            this.fileMenuOpen.ShortcutKeyDisplayString = "";
            this.fileMenuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileMenuOpen.Size = new System.Drawing.Size(213, 28);
            this.fileMenuOpen.Text = "&Open...";
            this.fileMenuOpen.Click += new System.EventHandler(this.fileMenuOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(210, 6);
            // 
            // fileMenuSave
            // 
            this.fileMenuSave.Name = "fileMenuSave";
            this.fileMenuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileMenuSave.Size = new System.Drawing.Size(213, 28);
            this.fileMenuSave.Text = "&Save";
            this.fileMenuSave.Click += new System.EventHandler(this.fileMenuSave_Click);
            // 
            // fileMenuSaveAs
            // 
            this.fileMenuSaveAs.Name = "fileMenuSaveAs";
            this.fileMenuSaveAs.Size = new System.Drawing.Size(213, 28);
            this.fileMenuSaveAs.Text = "Save &As...";
            this.fileMenuSaveAs.Click += new System.EventHandler(this.fileMenuSaveAs_Click);
            // 
            // fileMenuExportPDF
            // 
            this.fileMenuExportPDF.Name = "fileMenuExportPDF";
            this.fileMenuExportPDF.Size = new System.Drawing.Size(213, 28);
            this.fileMenuExportPDF.Text = "&Export to PDF...";
            this.fileMenuExportPDF.Click += new System.EventHandler(this.fileMenuExportPDF_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(210, 6);
            // 
            // fileMenuExit
            // 
            this.fileMenuExit.Name = "fileMenuExit";
            this.fileMenuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.fileMenuExit.Size = new System.Drawing.Size(213, 28);
            this.fileMenuExit.Text = "E&xit";
            this.fileMenuExit.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // menuItemProject
            // 
            this.menuItemProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemProjectSettings,
            this.rereadRecomputeAllDisplayElementsToolStripMenuItem,
            this.rereadRecomputeSelectedElementToolStripMenuItem});
            this.menuItemProject.Name = "menuItemProject";
            this.menuItemProject.Size = new System.Drawing.Size(79, 27);
            this.menuItemProject.Text = "Project";
            this.menuItemProject.DropDownOpening += new System.EventHandler(this.menuItemProject_DropDownOpening);
            // 
            // menuItemProjectSettings
            // 
            this.menuItemProjectSettings.Name = "menuItemProjectSettings";
            this.menuItemProjectSettings.Size = new System.Drawing.Size(393, 28);
            this.menuItemProjectSettings.Text = "Settings...";
            this.menuItemProjectSettings.Click += new System.EventHandler(this.menuItemProjectSettings_Click);
            // 
            // rereadRecomputeAllDisplayElementsToolStripMenuItem
            // 
            this.rereadRecomputeAllDisplayElementsToolStripMenuItem.Name = "rereadRecomputeAllDisplayElementsToolStripMenuItem";
            this.rereadRecomputeAllDisplayElementsToolStripMenuItem.Size = new System.Drawing.Size(393, 28);
            this.rereadRecomputeAllDisplayElementsToolStripMenuItem.Text = "Reread/Recompute All Elements";
            this.rereadRecomputeAllDisplayElementsToolStripMenuItem.Click += new System.EventHandler(this.rereadRecomputeAllElementsToolStripMenuItem_Click);
            // 
            // rereadRecomputeSelectedElementToolStripMenuItem
            // 
            this.rereadRecomputeSelectedElementToolStripMenuItem.Name = "rereadRecomputeSelectedElementToolStripMenuItem";
            this.rereadRecomputeSelectedElementToolStripMenuItem.Size = new System.Drawing.Size(393, 28);
            this.rereadRecomputeSelectedElementToolStripMenuItem.Text = "Reread/Recompute Selected Element";
            this.rereadRecomputeSelectedElementToolStripMenuItem.Click += new System.EventHandler(this.rereadRecomputeSelectedElementToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewSTMapToolStripMenuItem,
            this.menuEditAddNewChart,
            this.menuEditAddNewTable,
            this.toolStripMenuItem1,
            this.editMenuElementProperties,
            this.editMenuMapLayerProperties,
            this.editMenuDataSeriesProperties,
            this.menuEditAddNewContourLayer,
            this.menuEditAddNewColorFillLayer,
            this.menuEditAddNewDataSeries,
            this.toolStripMenuItem2,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(54, 27);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // addNewSTMapToolStripMenuItem
            // 
            this.addNewSTMapToolStripMenuItem.Name = "addNewSTMapToolStripMenuItem";
            this.addNewSTMapToolStripMenuItem.Size = new System.Drawing.Size(284, 28);
            this.addNewSTMapToolStripMenuItem.Text = "Add New Map";
            this.addNewSTMapToolStripMenuItem.Click += new System.EventHandler(this.addNewSTMapToolStripMenuItem_Click);
            // 
            // menuEditAddNewChart
            // 
            this.menuEditAddNewChart.Name = "menuEditAddNewChart";
            this.menuEditAddNewChart.Size = new System.Drawing.Size(284, 28);
            this.menuEditAddNewChart.Text = "Add New Chart";
            this.menuEditAddNewChart.Click += new System.EventHandler(this.menuEditAddNewChart_Click);
            // 
            // menuEditAddNewTable
            // 
            this.menuEditAddNewTable.Name = "menuEditAddNewTable";
            this.menuEditAddNewTable.Size = new System.Drawing.Size(284, 28);
            this.menuEditAddNewTable.Text = "Add New Table";
            this.menuEditAddNewTable.Click += new System.EventHandler(this.menuEditAddNewTable_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(281, 6);
            // 
            // editMenuElementProperties
            // 
            this.editMenuElementProperties.Name = "editMenuElementProperties";
            this.editMenuElementProperties.Size = new System.Drawing.Size(284, 28);
            this.editMenuElementProperties.Text = "Element Properties...";
            this.editMenuElementProperties.Click += new System.EventHandler(this.editMenuElementProperties_Click);
            // 
            // editMenuMapLayerProperties
            // 
            this.editMenuMapLayerProperties.Name = "editMenuMapLayerProperties";
            this.editMenuMapLayerProperties.Size = new System.Drawing.Size(284, 28);
            this.editMenuMapLayerProperties.Text = "Map Layer Properties...";
            this.editMenuMapLayerProperties.Click += new System.EventHandler(this.editMenuMapLayerProperties_Click);
            // 
            // editMenuDataSeriesProperties
            // 
            this.editMenuDataSeriesProperties.Name = "editMenuDataSeriesProperties";
            this.editMenuDataSeriesProperties.Size = new System.Drawing.Size(284, 28);
            this.editMenuDataSeriesProperties.Text = "Data Series Properties...";
            this.editMenuDataSeriesProperties.Click += new System.EventHandler(this.editMenuDataSeriesProperties_Click);
            // 
            // menuEditAddNewContourLayer
            // 
            this.menuEditAddNewContourLayer.Name = "menuEditAddNewContourLayer";
            this.menuEditAddNewContourLayer.Size = new System.Drawing.Size(284, 28);
            this.menuEditAddNewContourLayer.Text = "Add New Contour Layer";
            this.menuEditAddNewContourLayer.Click += new System.EventHandler(this.menuEditAddNewContourLayer_Click);
            // 
            // menuEditAddNewColorFillLayer
            // 
            this.menuEditAddNewColorFillLayer.Name = "menuEditAddNewColorFillLayer";
            this.menuEditAddNewColorFillLayer.Size = new System.Drawing.Size(284, 28);
            this.menuEditAddNewColorFillLayer.Text = "Add New Color-Fill Layer";
            this.menuEditAddNewColorFillLayer.Click += new System.EventHandler(this.menuEditAddNewColorFillLayer_Click);
            // 
            // menuEditAddNewDataSeries
            // 
            this.menuEditAddNewDataSeries.Name = "menuEditAddNewDataSeries";
            this.menuEditAddNewDataSeries.Size = new System.Drawing.Size(284, 28);
            this.menuEditAddNewDataSeries.Text = "Add New Data Series";
            this.menuEditAddNewDataSeries.Click += new System.EventHandler(this.menuEditAddNewDataSeries_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(281, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(284, 28);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(284, 28);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openScenarioAnalyzerHelpToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.toggleAutoRefreshToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(60, 27);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // openScenarioAnalyzerHelpToolStripMenuItem
            // 
            this.openScenarioAnalyzerHelpToolStripMenuItem.Name = "openScenarioAnalyzerHelpToolStripMenuItem";
            this.openScenarioAnalyzerHelpToolStripMenuItem.Size = new System.Drawing.Size(272, 28);
            this.openScenarioAnalyzerHelpToolStripMenuItem.Text = "Scenario Analyzer Help";
            this.openScenarioAnalyzerHelpToolStripMenuItem.Click += new System.EventHandler(this.scenarioAnalyzerHelpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(272, 28);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toggleAutoRefreshToolStripMenuItem
            // 
            this.toggleAutoRefreshToolStripMenuItem.Name = "toggleAutoRefreshToolStripMenuItem";
            this.toggleAutoRefreshToolStripMenuItem.Size = new System.Drawing.Size(272, 28);
            this.toggleAutoRefreshToolStripMenuItem.Text = "Toggle Auto Refresh";
            this.toggleAutoRefreshToolStripMenuItem.Visible = false;
            this.toggleAutoRefreshToolStripMenuItem.Click += new System.EventHandler(this.toggleAutoRefreshToolStripMenuItem_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 31);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitContainer.Panel2.BackgroundImage")));
            this.splitContainer.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.splitContainer.Panel2.Controls.Add(this.pictureBoxPreview);
            this.splitContainer.Size = new System.Drawing.Size(784, 435);
            this.splitContainer.SplitterDistance = 248;
            this.splitContainer.SplitterWidth = 6;
            this.splitContainer.TabIndex = 1;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Margin = new System.Windows.Forms.Padding(4);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(244, 433);
            this.treeView.TabIndex = 0;
            this.treeView.Click += new System.EventHandler(this.treeView_Click);
            this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
            this.treeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseClick);
            this.treeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDoubleClick);
            this.treeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseUp);
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPreview.Location = new System.Drawing.Point(4, 4);
            this.pictureBoxPreview.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(510, 423);
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.Click += new System.EventHandler(this.pictureBoxPreview_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelPrimary,
            this.toolStripProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 464);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(784, 40);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabelPrimary
            // 
            this.statusLabelPrimary.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusLabelPrimary.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusLabelPrimary.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.statusLabelPrimary.Name = "statusLabelPrimary";
            this.statusLabelPrimary.Size = new System.Drawing.Size(180, 35);
            this.statusLabelPrimary.Text = "statusLabelPrimary";
            this.statusLabelPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusLabelPrimary.Click += new System.EventHandler(this.statusLabelPrimary_Click);
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(150, 34);
            // 
            // genericBackgroundWorker
            // 
            this.genericBackgroundWorker.WorkerReportsProgress = true;
            this.genericBackgroundWorker.WorkerSupportsCancellation = true;
            this.genericBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.imageMakerBackgroundWorker_DoWork);
            this.genericBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.imageMakerBackgroundWorker_ProgressChanged);
            this.genericBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.imageMakerBackgroundWorker_RunWorkerCompleted);
            // 
            // SAMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 506);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(270, 190);
            this.Name = "SAMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "USGS Scenario Analyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SAMainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileMenuExit;
        private System.Windows.Forms.ToolStripMenuItem fileMenuOpen;
        private System.Windows.Forms.ToolStripMenuItem fileMenuNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileMenuSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelPrimary;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripMenuItem fileMenuSaveAs;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.ToolStripMenuItem fileMenuExportPDF;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuElementProperties;
        private System.Windows.Forms.ToolStripMenuItem editMenuMapLayerProperties;
        private System.Windows.Forms.ToolStripMenuItem editMenuDataSeriesProperties;
        private System.Windows.Forms.ToolStripMenuItem menuEditAddNewChart;
        private System.Windows.Forms.ToolStripMenuItem menuEditAddNewTable;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuEditAddNewContourLayer;
        private System.Windows.Forms.ToolStripMenuItem menuEditAddNewColorFillLayer;
        private System.Windows.Forms.ToolStripMenuItem menuEditAddNewDataSeries;
        private System.ComponentModel.BackgroundWorker genericBackgroundWorker;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewSTMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemProject;
        private System.Windows.Forms.ToolStripMenuItem menuItemProjectSettings;
        private System.Windows.Forms.ToolStripMenuItem rereadRecomputeAllDisplayElementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleAutoRefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rereadRecomputeSelectedElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openScenarioAnalyzerHelpToolStripMenuItem;

    }
}

