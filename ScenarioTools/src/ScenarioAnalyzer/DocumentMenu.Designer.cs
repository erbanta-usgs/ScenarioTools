namespace ScenarioAnalyzer
{
    partial class DocumentMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentMenu));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxLengthUnit = new System.Windows.Forms.ComboBox();
            this.buttonEditSimulationStartTime = new System.Windows.Forms.Button();
            this.comboBoxTimeUnit = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxSimulationStartTime = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.labelCinact = new System.Windows.Forms.Label();
            this.textBoxCinact = new System.Windows.Forms.TextBox();
            this.textBoxHdry = new System.Windows.Forms.TextBox();
            this.labelHdry = new System.Windows.Forms.Label();
            this.labelHnoflo = new System.Windows.Forms.Label();
            this.textBoxHnoflo = new System.Windows.Forms.TextBox();
            this.buttonBrowseGrid = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxGridShapefileName = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxImageFilename = new System.Windows.Forms.TextBox();
            this.textBoxAuthor = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageElements = new System.Windows.Forms.TabPage();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxElements = new System.Windows.Forms.ListBox();
            this.tabPageExtents = new System.Windows.Forms.TabPage();
            this.labelInUse = new System.Windows.Forms.Label();
            this.listViewExtents = new System.Windows.Forms.ListView();
            this.btnRemoveExtents = new System.Windows.Forms.Button();
            this.tabPageInactiveAreas = new System.Windows.Forms.TabPage();
            this.buttonBrowseForNameFile = new System.Windows.Forms.Button();
            this.textBoxNameFile = new System.Windows.Forms.TextBox();
            this.labelModflowNameFile = new System.Windows.Forms.Label();
            this.numericUpDownBlankingLayer = new System.Windows.Forms.NumericUpDown();
            this.radioButtonBlankIfAllLayersInactive = new System.Windows.Forms.RadioButton();
            this.radioButtonBlankIfAnyLayerInactive = new System.Windows.Forms.RadioButton();
            this.radioButtonBlankSpecifiedIboundLayer = new System.Windows.Forms.RadioButton();
            this.radioButtonAsIs = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageElements.SuspendLayout();
            this.tabPageExtents.SuspendLayout();
            this.tabPageInactiveAreas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBlankingLayer)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(136, 510);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 31);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(15, 510);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(112, 31);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageGeneral);
            this.tabControl.Controls.Add(this.tabPageElements);
            this.tabControl.Controls.Add(this.tabPageExtents);
            this.tabControl.Controls.Add(this.tabPageInactiveAreas);
            this.tabControl.Location = new System.Drawing.Point(16, 15);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(768, 488);
            this.tabControl.TabIndex = 3;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.label8);
            this.tabPageGeneral.Controls.Add(this.comboBoxLengthUnit);
            this.tabPageGeneral.Controls.Add(this.buttonEditSimulationStartTime);
            this.tabPageGeneral.Controls.Add(this.comboBoxTimeUnit);
            this.tabPageGeneral.Controls.Add(this.label7);
            this.tabPageGeneral.Controls.Add(this.textBoxSimulationStartTime);
            this.tabPageGeneral.Controls.Add(this.label6);
            this.tabPageGeneral.Controls.Add(this.labelCinact);
            this.tabPageGeneral.Controls.Add(this.textBoxCinact);
            this.tabPageGeneral.Controls.Add(this.textBoxHdry);
            this.tabPageGeneral.Controls.Add(this.labelHdry);
            this.tabPageGeneral.Controls.Add(this.labelHnoflo);
            this.tabPageGeneral.Controls.Add(this.textBoxHnoflo);
            this.tabPageGeneral.Controls.Add(this.buttonBrowseGrid);
            this.tabPageGeneral.Controls.Add(this.label5);
            this.tabPageGeneral.Controls.Add(this.textBoxGridShapefileName);
            this.tabPageGeneral.Controls.Add(this.buttonBrowse);
            this.tabPageGeneral.Controls.Add(this.label4);
            this.tabPageGeneral.Controls.Add(this.textBoxImageFilename);
            this.tabPageGeneral.Controls.Add(this.textBoxAuthor);
            this.tabPageGeneral.Controls.Add(this.label3);
            this.tabPageGeneral.Controls.Add(this.textBoxName);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 31);
            this.tabPageGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageGeneral.Size = new System.Drawing.Size(760, 453);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(544, 141);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(209, 24);
            this.label8.TabIndex = 21;
            this.label8.Text = "MODFLOW Length Unit";
            // 
            // comboBoxLengthUnit
            // 
            this.comboBoxLengthUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLengthUnit.FormattingEnabled = true;
            this.comboBoxLengthUnit.Items.AddRange(new object[] {
            "Feet",
            "Meters",
            "Centimeters"});
            this.comboBoxLengthUnit.Location = new System.Drawing.Point(548, 168);
            this.comboBoxLengthUnit.Name = "comboBoxLengthUnit";
            this.comboBoxLengthUnit.Size = new System.Drawing.Size(205, 30);
            this.comboBoxLengthUnit.TabIndex = 20;
            this.comboBoxLengthUnit.SelectedIndexChanged += new System.EventHandler(this.comboBoxLengthUnit_SelectedIndexChanged);
            // 
            // buttonEditSimulationStartTime
            // 
            this.buttonEditSimulationStartTime.Location = new System.Drawing.Point(256, 167);
            this.buttonEditSimulationStartTime.Name = "buttonEditSimulationStartTime";
            this.buttonEditSimulationStartTime.Size = new System.Drawing.Size(59, 34);
            this.buttonEditSimulationStartTime.TabIndex = 19;
            this.buttonEditSimulationStartTime.Text = "Edit";
            this.buttonEditSimulationStartTime.UseVisualStyleBackColor = true;
            this.buttonEditSimulationStartTime.Click += new System.EventHandler(this.buttonSimulationStartTime_Click);
            // 
            // comboBoxTimeUnit
            // 
            this.comboBoxTimeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTimeUnit.FormattingEnabled = true;
            this.comboBoxTimeUnit.Items.AddRange(new object[] {
            "Seconds",
            "Minutes",
            "Hours",
            "Days",
            "Years"});
            this.comboBoxTimeUnit.Location = new System.Drawing.Point(337, 168);
            this.comboBoxTimeUnit.Name = "comboBoxTimeUnit";
            this.comboBoxTimeUnit.Size = new System.Drawing.Size(190, 30);
            this.comboBoxTimeUnit.TabIndex = 18;
            this.comboBoxTimeUnit.SelectedIndexChanged += new System.EventHandler(this.comboBoxTimeUnit_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(333, 141);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(194, 24);
            this.label7.TabIndex = 17;
            this.label7.Text = "MODFLOW Time Unit";
            // 
            // textBoxSimulationStartTime
            // 
            this.textBoxSimulationStartTime.Location = new System.Drawing.Point(7, 168);
            this.textBoxSimulationStartTime.Name = "textBoxSimulationStartTime";
            this.textBoxSimulationStartTime.ReadOnly = true;
            this.textBoxSimulationStartTime.Size = new System.Drawing.Size(248, 28);
            this.textBoxSimulationStartTime.TabIndex = 16;
            this.textBoxSimulationStartTime.TextChanged += new System.EventHandler(this.textBoxSimulationStartTime_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(186, 24);
            this.label6.TabIndex = 15;
            this.label6.Text = "Simulation Start Time";
            // 
            // labelCinact
            // 
            this.labelCinact.AutoSize = true;
            this.labelCinact.Location = new System.Drawing.Point(196, 420);
            this.labelCinact.Name = "labelCinact";
            this.labelCinact.Size = new System.Drawing.Size(367, 24);
            this.labelCinact.TabIndex = 14;
            this.labelCinact.Text = "CINACT (must match BTN file of MT3DMS)";
            // 
            // textBoxCinact
            // 
            this.textBoxCinact.Location = new System.Drawing.Point(8, 417);
            this.textBoxCinact.Name = "textBoxCinact";
            this.textBoxCinact.Size = new System.Drawing.Size(182, 28);
            this.textBoxCinact.TabIndex = 13;
            this.textBoxCinact.TextChanged += new System.EventHandler(this.textBoxCinact_TextChanged);
            // 
            // textBoxHdry
            // 
            this.textBoxHdry.Location = new System.Drawing.Point(7, 380);
            this.textBoxHdry.Name = "textBoxHdry";
            this.textBoxHdry.Size = new System.Drawing.Size(183, 28);
            this.textBoxHdry.TabIndex = 12;
            this.textBoxHdry.TextChanged += new System.EventHandler(this.textBoxHdry_TextChanged);
            // 
            // labelHdry
            // 
            this.labelHdry.AutoSize = true;
            this.labelHdry.Location = new System.Drawing.Point(196, 384);
            this.labelHdry.Name = "labelHdry";
            this.labelHdry.Size = new System.Drawing.Size(463, 24);
            this.labelHdry.TabIndex = 11;
            this.labelHdry.Text = "HDRY (must match LPF, BCF, HUF, or UPW Package)";
            // 
            // labelHnoflo
            // 
            this.labelHnoflo.AutoSize = true;
            this.labelHnoflo.Location = new System.Drawing.Point(196, 347);
            this.labelHnoflo.Name = "labelHnoflo";
            this.labelHnoflo.Size = new System.Drawing.Size(331, 24);
            this.labelHnoflo.TabIndex = 10;
            this.labelHnoflo.Text = "HNOFLO (must match Basic Package)";
            // 
            // textBoxHnoflo
            // 
            this.textBoxHnoflo.Location = new System.Drawing.Point(7, 343);
            this.textBoxHnoflo.Name = "textBoxHnoflo";
            this.textBoxHnoflo.Size = new System.Drawing.Size(183, 28);
            this.textBoxHnoflo.TabIndex = 9;
            this.textBoxHnoflo.TextChanged += new System.EventHandler(this.textBoxHnoflo_TextChanged);
            // 
            // buttonBrowseGrid
            // 
            this.buttonBrowseGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseGrid.Image = ((System.Drawing.Image)(resources.GetObject("buttonBrowseGrid.Image")));
            this.buttonBrowseGrid.Location = new System.Drawing.Point(725, 232);
            this.buttonBrowseGrid.Name = "buttonBrowseGrid";
            this.buttonBrowseGrid.Size = new System.Drawing.Size(30, 30);
            this.buttonBrowseGrid.TabIndex = 5;
            this.buttonBrowseGrid.UseVisualStyleBackColor = true;
            this.buttonBrowseGrid.Click += new System.EventHandler(this.buttonBrowseGrid_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 206);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 24);
            this.label5.TabIndex = 8;
            this.label5.Text = "Model-Grid Shapefile";
            // 
            // textBoxGridShapefileName
            // 
            this.textBoxGridShapefileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxGridShapefileName.Location = new System.Drawing.Point(7, 233);
            this.textBoxGridShapefileName.Name = "textBoxGridShapefileName";
            this.textBoxGridShapefileName.Size = new System.Drawing.Size(718, 28);
            this.textBoxGridShapefileName.TabIndex = 4;
            this.textBoxGridShapefileName.TextChanged += new System.EventHandler(this.textBoxGridShapefileName_TextChanged);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Image = ((System.Drawing.Image)(resources.GetObject("buttonBrowse.Image")));
            this.buttonBrowse.Location = new System.Drawing.Point(725, 293);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(30, 30);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 268);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(338, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = "Georeferenced Background Image File";
            // 
            // textBoxImageFilename
            // 
            this.textBoxImageFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxImageFilename.Location = new System.Drawing.Point(7, 294);
            this.textBoxImageFilename.Name = "textBoxImageFilename";
            this.textBoxImageFilename.Size = new System.Drawing.Size(718, 28);
            this.textBoxImageFilename.TabIndex = 2;
            this.textBoxImageFilename.TextChanged += new System.EventHandler(this.textBoxImageFilename_TextChanged);
            // 
            // textBoxAuthor
            // 
            this.textBoxAuthor.Location = new System.Drawing.Point(8, 100);
            this.textBoxAuthor.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxAuthor.Name = "textBoxAuthor";
            this.textBoxAuthor.Size = new System.Drawing.Size(366, 28);
            this.textBoxAuthor.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 72);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 24);
            this.label3.TabIndex = 2;
            this.label3.Text = "Author";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(8, 34);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(366, 28);
            this.textBoxName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // tabPageElements
            // 
            this.tabPageElements.Controls.Add(this.buttonDown);
            this.tabPageElements.Controls.Add(this.buttonRemove);
            this.tabPageElements.Controls.Add(this.buttonUp);
            this.tabPageElements.Controls.Add(this.label2);
            this.tabPageElements.Controls.Add(this.listBoxElements);
            this.tabPageElements.Location = new System.Drawing.Point(4, 31);
            this.tabPageElements.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageElements.Name = "tabPageElements";
            this.tabPageElements.Size = new System.Drawing.Size(760, 453);
            this.tabPageElements.TabIndex = 1;
            this.tabPageElements.Text = "Elements";
            this.tabPageElements.UseVisualStyleBackColor = true;
            // 
            // buttonDown
            // 
            this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDown.Image = global::ScenarioAnalyzer.Properties.Resources.down_arrow;
            this.buttonDown.Location = new System.Drawing.Point(681, 238);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(68, 62);
            this.buttonDown.TabIndex = 7;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemove.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemove.Image")));
            this.buttonRemove.Location = new System.Drawing.Point(681, 167);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(68, 62);
            this.buttonRemove.TabIndex = 6;
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemoveElement_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUp.Image = global::ScenarioAnalyzer.Properties.Resources.up_arrow;
            this.buttonUp.Location = new System.Drawing.Point(683, 97);
            this.buttonUp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(68, 62);
            this.buttonUp.TabIndex = 5;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Report Elements";
            // 
            // listBoxElements
            // 
            this.listBoxElements.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxElements.FormattingEnabled = true;
            this.listBoxElements.ItemHeight = 22;
            this.listBoxElements.Location = new System.Drawing.Point(9, 34);
            this.listBoxElements.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxElements.Name = "listBoxElements";
            this.listBoxElements.Size = new System.Drawing.Size(663, 400);
            this.listBoxElements.TabIndex = 4;
            // 
            // tabPageExtents
            // 
            this.tabPageExtents.Controls.Add(this.labelInUse);
            this.tabPageExtents.Controls.Add(this.listViewExtents);
            this.tabPageExtents.Controls.Add(this.btnRemoveExtents);
            this.tabPageExtents.Location = new System.Drawing.Point(4, 31);
            this.tabPageExtents.Name = "tabPageExtents";
            this.tabPageExtents.Size = new System.Drawing.Size(760, 453);
            this.tabPageExtents.TabIndex = 2;
            this.tabPageExtents.Text = "Map Extents";
            this.tabPageExtents.UseVisualStyleBackColor = true;
            // 
            // labelInUse
            // 
            this.labelInUse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInUse.AutoSize = true;
            this.labelInUse.Location = new System.Drawing.Point(5, 420);
            this.labelInUse.Name = "labelInUse";
            this.labelInUse.Size = new System.Drawing.Size(98, 24);
            this.labelInUse.TabIndex = 9;
            this.labelInUse.Text = "labelInUse";
            // 
            // listViewExtents
            // 
            this.listViewExtents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewExtents.Location = new System.Drawing.Point(9, 8);
            this.listViewExtents.Name = "listViewExtents";
            this.listViewExtents.Size = new System.Drawing.Size(649, 400);
            this.listViewExtents.TabIndex = 8;
            this.listViewExtents.UseCompatibleStateImageBehavior = false;
            this.listViewExtents.View = System.Windows.Forms.View.List;
            this.listViewExtents.SelectedIndexChanged += new System.EventHandler(this.listViewExtents_SelectedIndexChanged);
            // 
            // btnRemoveExtents
            // 
            this.btnRemoveExtents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveExtents.Enabled = false;
            this.btnRemoveExtents.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveExtents.Image")));
            this.btnRemoveExtents.Location = new System.Drawing.Point(676, 133);
            this.btnRemoveExtents.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemoveExtents.Name = "btnRemoveExtents";
            this.btnRemoveExtents.Size = new System.Drawing.Size(68, 62);
            this.btnRemoveExtents.TabIndex = 7;
            this.btnRemoveExtents.UseVisualStyleBackColor = true;
            this.btnRemoveExtents.Click += new System.EventHandler(this.btnRemoveExtents_Click);
            // 
            // tabPageInactiveAreas
            // 
            this.tabPageInactiveAreas.Controls.Add(this.buttonBrowseForNameFile);
            this.tabPageInactiveAreas.Controls.Add(this.textBoxNameFile);
            this.tabPageInactiveAreas.Controls.Add(this.labelModflowNameFile);
            this.tabPageInactiveAreas.Controls.Add(this.numericUpDownBlankingLayer);
            this.tabPageInactiveAreas.Controls.Add(this.radioButtonBlankIfAllLayersInactive);
            this.tabPageInactiveAreas.Controls.Add(this.radioButtonBlankIfAnyLayerInactive);
            this.tabPageInactiveAreas.Controls.Add(this.radioButtonBlankSpecifiedIboundLayer);
            this.tabPageInactiveAreas.Controls.Add(this.radioButtonAsIs);
            this.tabPageInactiveAreas.Controls.Add(this.label9);
            this.tabPageInactiveAreas.Location = new System.Drawing.Point(4, 31);
            this.tabPageInactiveAreas.Name = "tabPageInactiveAreas";
            this.tabPageInactiveAreas.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInactiveAreas.Size = new System.Drawing.Size(760, 453);
            this.tabPageInactiveAreas.TabIndex = 3;
            this.tabPageInactiveAreas.Text = "Inactive Areas";
            this.tabPageInactiveAreas.UseVisualStyleBackColor = true;
            // 
            // buttonBrowseForNameFile
            // 
            this.buttonBrowseForNameFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseForNameFile.Image = global::ScenarioAnalyzer.Properties.Resources.open_document24_h;
            this.buttonBrowseForNameFile.Location = new System.Drawing.Point(718, 279);
            this.buttonBrowseForNameFile.Name = "buttonBrowseForNameFile";
            this.buttonBrowseForNameFile.Size = new System.Drawing.Size(30, 30);
            this.buttonBrowseForNameFile.TabIndex = 13;
            this.buttonBrowseForNameFile.UseVisualStyleBackColor = true;
            this.buttonBrowseForNameFile.Click += new System.EventHandler(this.buttonBrowseForNameFile_Click);
            // 
            // textBoxNameFile
            // 
            this.textBoxNameFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNameFile.Location = new System.Drawing.Point(43, 280);
            this.textBoxNameFile.Name = "textBoxNameFile";
            this.textBoxNameFile.Size = new System.Drawing.Size(675, 28);
            this.textBoxNameFile.TabIndex = 12;
            this.textBoxNameFile.TextChanged += new System.EventHandler(this.textBoxNameFile_TextChanged);
            // 
            // labelModflowNameFile
            // 
            this.labelModflowNameFile.AutoSize = true;
            this.labelModflowNameFile.Location = new System.Drawing.Point(39, 251);
            this.labelModflowNameFile.Name = "labelModflowNameFile";
            this.labelModflowNameFile.Size = new System.Drawing.Size(193, 24);
            this.labelModflowNameFile.TabIndex = 11;
            this.labelModflowNameFile.Text = "MODFLOW Name file";
            // 
            // numericUpDownBlankingLayer
            // 
            this.numericUpDownBlankingLayer.Location = new System.Drawing.Point(359, 105);
            this.numericUpDownBlankingLayer.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownBlankingLayer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownBlankingLayer.Name = "numericUpDownBlankingLayer";
            this.numericUpDownBlankingLayer.Size = new System.Drawing.Size(74, 28);
            this.numericUpDownBlankingLayer.TabIndex = 10;
            this.numericUpDownBlankingLayer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownBlankingLayer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownBlankingLayer.ValueChanged += new System.EventHandler(this.numericUpDownBlankingLayer_ValueChanged);
            // 
            // radioButtonBlankIfAllLayersInactive
            // 
            this.radioButtonBlankIfAllLayersInactive.AutoSize = true;
            this.radioButtonBlankIfAllLayersInactive.Location = new System.Drawing.Point(43, 197);
            this.radioButtonBlankIfAllLayersInactive.Name = "radioButtonBlankIfAllLayersInactive";
            this.radioButtonBlankIfAllLayersInactive.Size = new System.Drawing.Size(376, 28);
            this.radioButtonBlankIfAllLayersInactive.TabIndex = 6;
            this.radioButtonBlankIfAllLayersInactive.TabStop = true;
            this.radioButtonBlankIfAllLayersInactive.Text = "Blank where IBOUND inactive in all layers";
            this.radioButtonBlankIfAllLayersInactive.UseVisualStyleBackColor = true;
            this.radioButtonBlankIfAllLayersInactive.CheckedChanged += new System.EventHandler(this.radioButtonBlankIfAllLayersInactive_CheckedChanged);
            // 
            // radioButtonBlankIfAnyLayerInactive
            // 
            this.radioButtonBlankIfAnyLayerInactive.AutoSize = true;
            this.radioButtonBlankIfAnyLayerInactive.Location = new System.Drawing.Point(43, 151);
            this.radioButtonBlankIfAnyLayerInactive.Name = "radioButtonBlankIfAnyLayerInactive";
            this.radioButtonBlankIfAnyLayerInactive.Size = new System.Drawing.Size(379, 28);
            this.radioButtonBlankIfAnyLayerInactive.TabIndex = 5;
            this.radioButtonBlankIfAnyLayerInactive.TabStop = true;
            this.radioButtonBlankIfAnyLayerInactive.Text = "Blank where IBOUND inactive in any layer";
            this.radioButtonBlankIfAnyLayerInactive.UseVisualStyleBackColor = true;
            this.radioButtonBlankIfAnyLayerInactive.CheckedChanged += new System.EventHandler(this.radioButtonBlankIfAnyLayerInactive_CheckedChanged);
            // 
            // radioButtonBlankSpecifiedIboundLayer
            // 
            this.radioButtonBlankSpecifiedIboundLayer.AutoSize = true;
            this.radioButtonBlankSpecifiedIboundLayer.Location = new System.Drawing.Point(43, 105);
            this.radioButtonBlankSpecifiedIboundLayer.Name = "radioButtonBlankSpecifiedIboundLayer";
            this.radioButtonBlankSpecifiedIboundLayer.Size = new System.Drawing.Size(308, 28);
            this.radioButtonBlankSpecifiedIboundLayer.TabIndex = 2;
            this.radioButtonBlankSpecifiedIboundLayer.Text = "Blank by specified IBOUND layer:";
            this.radioButtonBlankSpecifiedIboundLayer.UseVisualStyleBackColor = true;
            this.radioButtonBlankSpecifiedIboundLayer.CheckedChanged += new System.EventHandler(this.radioButtonBlankSpecifiedIboundLayer_CheckedChanged);
            // 
            // radioButtonAsIs
            // 
            this.radioButtonAsIs.AutoSize = true;
            this.radioButtonAsIs.Checked = true;
            this.radioButtonAsIs.Location = new System.Drawing.Point(43, 57);
            this.radioButtonAsIs.Name = "radioButtonAsIs";
            this.radioButtonAsIs.Size = new System.Drawing.Size(390, 28);
            this.radioButtonAsIs.TabIndex = 1;
            this.radioButtonAsIs.TabStop = true;
            this.radioButtonAsIs.Text = "Leave as is (inactive areas are not blanked)";
            this.radioButtonAsIs.UseVisualStyleBackColor = true;
            this.radioButtonAsIs.CheckedChanged += new System.EventHandler(this.radioButtonAsIs_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(563, 24);
            this.label9.TabIndex = 0;
            this.label9.Text = "Specify how areas of inactive cells are to be represented on maps:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // DocumentMenu
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(811, 548);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(3000, 1000);
            this.MinimumSize = new System.Drawing.Size(829, 593);
            this.Name = "DocumentMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scenario Analyzer Project Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DocumentMenu_FormClosing);
            this.Load += new System.EventHandler(this.DocumentMenu_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPageElements.ResumeLayout(false);
            this.tabPageElements.PerformLayout();
            this.tabPageExtents.ResumeLayout(false);
            this.tabPageExtents.PerformLayout();
            this.tabPageInactiveAreas.ResumeLayout(false);
            this.tabPageInactiveAreas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBlankingLayer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageElements;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.ListBox listBoxElements;
        private System.Windows.Forms.TextBox textBoxAuthor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxImageFilename;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonBrowseGrid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxGridShapefileName;
        private System.Windows.Forms.Label labelHdry;
        private System.Windows.Forms.Label labelHnoflo;
        private System.Windows.Forms.TextBox textBoxHnoflo;
        private System.Windows.Forms.TextBox textBoxHdry;
        private System.Windows.Forms.TabPage tabPageExtents;
        private System.Windows.Forms.Button btnRemoveExtents;
        private System.Windows.Forms.Label labelCinact;
        private System.Windows.Forms.TextBox textBoxCinact;
        private System.Windows.Forms.TextBox textBoxSimulationStartTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonEditSimulationStartTime;
        private System.Windows.Forms.ComboBox comboBoxTimeUnit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxLengthUnit;
        private System.Windows.Forms.TabPage tabPageInactiveAreas;
        private System.Windows.Forms.RadioButton radioButtonAsIs;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton radioButtonBlankIfAllLayersInactive;
        private System.Windows.Forms.RadioButton radioButtonBlankIfAnyLayerInactive;
        private System.Windows.Forms.RadioButton radioButtonBlankSpecifiedIboundLayer;
        private System.Windows.Forms.NumericUpDown numericUpDownBlankingLayer;
        private System.Windows.Forms.Button buttonBrowseForNameFile;
        private System.Windows.Forms.TextBox textBoxNameFile;
        private System.Windows.Forms.Label labelModflowNameFile;
        private System.Windows.Forms.ListView listViewExtents;
        private System.Windows.Forms.Label labelInUse;
    }
}