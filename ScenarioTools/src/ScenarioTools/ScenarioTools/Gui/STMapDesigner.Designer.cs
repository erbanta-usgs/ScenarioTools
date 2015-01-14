namespace ScenarioTools.Gui
{
    partial class STMapDesigner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(STMapDesigner));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.textBoxBrightness = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ckbxBackgroundImage = new System.Windows.Forms.CheckBox();
            this.panelMapLegend = new System.Windows.Forms.Panel();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemAddLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLayout = new System.Windows.Forms.TabPage();
            this.tabPageExtent = new System.Windows.Forms.TabPage();
            this.comboBoxExtents = new System.Windows.Forms.ComboBox();
            this.btnSaveExtent = new System.Windows.Forms.Button();
            this.tbExtentName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblSouth = new System.Windows.Forms.Label();
            this.lblEast = new System.Windows.Forms.Label();
            this.lblWest = new System.Windows.Forms.Label();
            this.lblNorth = new System.Windows.Forms.Label();
            this.tbSouth = new System.Windows.Forms.TextBox();
            this.tbEast = new System.Windows.Forms.TextBox();
            this.tbWest = new System.Windows.Forms.TextBox();
            this.tbNorth = new System.Windows.Forms.TextBox();
            this.rbManual = new System.Windows.Forms.RadioButton();
            this.rbSelectFromList = new System.Windows.Forms.RadioButton();
            this.rbAutomatic = new System.Windows.Forms.RadioButton();
            this.tabPageLayers = new System.Windows.Forms.TabPage();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.listBoxLayers = new System.Windows.Forms.ListBox();
            this.textBoxXCoord = new System.Windows.Forms.TextBox();
            this.textBoxYcoord = new System.Windows.Forms.TextBox();
            this.labelX = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.btnSaveUserDrawnExtent = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageLayout.SuspendLayout();
            this.tabPageExtent.SuspendLayout();
            this.tabPageLayers.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(2, 4);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.panelMapLegend);
            this.splitContainer1.Panel1.Controls.Add(this.tbName);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1MinSize = 160;
            this.splitContainer1.Size = new System.Drawing.Size(731, 356);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnApply);
            this.groupBox1.Controls.Add(this.hScrollBar1);
            this.groupBox1.Controls.Add(this.textBoxBrightness);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ckbxBackgroundImage);
            this.groupBox1.Location = new System.Drawing.Point(0, 207);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 150);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Background Image";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(37, 112);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(127, 33);
            this.btnApply.TabIndex = 13;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar1.LargeChange = 1;
            this.hScrollBar1.Location = new System.Drawing.Point(5, 88);
            this.hScrollBar1.Maximum = 255;
            this.hScrollBar1.Minimum = -255;
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(205, 21);
            this.hScrollBar1.TabIndex = 12;
            this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
            // 
            // textBoxBrightness
            // 
            this.textBoxBrightness.Location = new System.Drawing.Point(107, 57);
            this.textBoxBrightness.Name = "textBoxBrightness";
            this.textBoxBrightness.Size = new System.Drawing.Size(56, 28);
            this.textBoxBrightness.TabIndex = 10;
            this.textBoxBrightness.Leave += new System.EventHandler(this.textBoxBrightness_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 24);
            this.label2.TabIndex = 9;
            this.label2.Text = "Brightness";
            // 
            // ckbxBackgroundImage
            // 
            this.ckbxBackgroundImage.Checked = true;
            this.ckbxBackgroundImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbxBackgroundImage.Location = new System.Drawing.Point(7, 28);
            this.ckbxBackgroundImage.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.ckbxBackgroundImage.Name = "ckbxBackgroundImage";
            this.ckbxBackgroundImage.Size = new System.Drawing.Size(130, 28);
            this.ckbxBackgroundImage.TabIndex = 8;
            this.ckbxBackgroundImage.Text = "Show image";
            this.ckbxBackgroundImage.UseVisualStyleBackColor = true;
            this.ckbxBackgroundImage.CheckedChanged += new System.EventHandler(this.ckbxBackgroundImage_CheckedChanged);
            // 
            // panelMapLegend
            // 
            this.panelMapLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMapLegend.BackColor = System.Drawing.SystemColors.Window;
            this.panelMapLegend.Location = new System.Drawing.Point(0, 40);
            this.panelMapLegend.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.panelMapLegend.Name = "panelMapLegend";
            this.panelMapLegend.Size = new System.Drawing.Size(214, 160);
            this.panelMapLegend.TabIndex = 8;
            this.panelMapLegend.Click += new System.EventHandler(this.panelMapLegend_Click);
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Location = new System.Drawing.Point(66, 9);
            this.tbName.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(146, 28);
            this.tbName.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "Name:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(105, 404);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 30);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(18, 404);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 30);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAddLayer});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(155, 28);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // menuItemAddLayer
            // 
            this.menuItemAddLayer.Name = "menuItemAddLayer";
            this.menuItemAddLayer.Size = new System.Drawing.Size(154, 24);
            this.menuItemAddLayer.Text = "Add Layer...";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageLayout);
            this.tabControl1.Controls.Add(this.tabPageExtent);
            this.tabControl1.Controls.Add(this.tabPageLayers);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(743, 399);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageLayout
            // 
            this.tabPageLayout.Controls.Add(this.splitContainer1);
            this.tabPageLayout.Location = new System.Drawing.Point(4, 31);
            this.tabPageLayout.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tabPageLayout.Name = "tabPageLayout";
            this.tabPageLayout.Padding = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tabPageLayout.Size = new System.Drawing.Size(735, 364);
            this.tabPageLayout.TabIndex = 0;
            this.tabPageLayout.Text = "Layout";
            this.tabPageLayout.UseVisualStyleBackColor = true;
            this.tabPageLayout.Enter += new System.EventHandler(this.tabPageLayout_Enter);
            this.tabPageLayout.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabPageLayout_Layout);
            // 
            // tabPageExtent
            // 
            this.tabPageExtent.Controls.Add(this.comboBoxExtents);
            this.tabPageExtent.Controls.Add(this.btnSaveExtent);
            this.tabPageExtent.Controls.Add(this.tbExtentName);
            this.tabPageExtent.Controls.Add(this.lblName);
            this.tabPageExtent.Controls.Add(this.lblSouth);
            this.tabPageExtent.Controls.Add(this.lblEast);
            this.tabPageExtent.Controls.Add(this.lblWest);
            this.tabPageExtent.Controls.Add(this.lblNorth);
            this.tabPageExtent.Controls.Add(this.tbSouth);
            this.tabPageExtent.Controls.Add(this.tbEast);
            this.tabPageExtent.Controls.Add(this.tbWest);
            this.tabPageExtent.Controls.Add(this.tbNorth);
            this.tabPageExtent.Controls.Add(this.rbManual);
            this.tabPageExtent.Controls.Add(this.rbSelectFromList);
            this.tabPageExtent.Controls.Add(this.rbAutomatic);
            this.tabPageExtent.Location = new System.Drawing.Point(4, 31);
            this.tabPageExtent.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tabPageExtent.Name = "tabPageExtent";
            this.tabPageExtent.Padding = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tabPageExtent.Size = new System.Drawing.Size(735, 364);
            this.tabPageExtent.TabIndex = 1;
            this.tabPageExtent.Text = "Extent";
            this.tabPageExtent.UseVisualStyleBackColor = true;
            this.tabPageExtent.Enter += new System.EventHandler(this.tabPageExtent_Enter);
            this.tabPageExtent.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabPageExtent_Layout);
            this.tabPageExtent.Leave += new System.EventHandler(this.tabPageExtent_Leave);
            // 
            // comboBoxExtents
            // 
            this.comboBoxExtents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExtents.FormattingEnabled = true;
            this.comboBoxExtents.Location = new System.Drawing.Point(35, 79);
            this.comboBoxExtents.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.comboBoxExtents.Name = "comboBoxExtents";
            this.comboBoxExtents.Size = new System.Drawing.Size(323, 30);
            this.comboBoxExtents.TabIndex = 2;
            this.comboBoxExtents.SelectedIndexChanged += new System.EventHandler(this.comboBoxExtents_SelectedIndexChanged);
            // 
            // btnSaveExtent
            // 
            this.btnSaveExtent.Location = new System.Drawing.Point(112, 325);
            this.btnSaveExtent.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.btnSaveExtent.Name = "btnSaveExtent";
            this.btnSaveExtent.Size = new System.Drawing.Size(158, 30);
            this.btnSaveExtent.TabIndex = 9;
            this.btnSaveExtent.Text = "Save Manual Extent";
            this.btnSaveExtent.UseVisualStyleBackColor = true;
            this.btnSaveExtent.Click += new System.EventHandler(this.btnSaveExtent_Click);
            // 
            // tbExtentName
            // 
            this.tbExtentName.Location = new System.Drawing.Point(112, 158);
            this.tbExtentName.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tbExtentName.Name = "tbExtentName";
            this.tbExtentName.Size = new System.Drawing.Size(246, 28);
            this.tbExtentName.TabIndex = 4;
            this.tbExtentName.TextChanged += new System.EventHandler(this.tbExtentName_TextChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(49, 160);
            this.lblName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(61, 24);
            this.lblName.TabIndex = 12;
            this.lblName.Text = "Name";
            // 
            // lblSouth
            // 
            this.lblSouth.AutoSize = true;
            this.lblSouth.Location = new System.Drawing.Point(72, 271);
            this.lblSouth.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSouth.Name = "lblSouth";
            this.lblSouth.Size = new System.Drawing.Size(59, 24);
            this.lblSouth.TabIndex = 11;
            this.lblSouth.Text = "South";
            // 
            // lblEast
            // 
            this.lblEast.AutoSize = true;
            this.lblEast.Location = new System.Drawing.Point(312, 241);
            this.lblEast.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEast.Name = "lblEast";
            this.lblEast.Size = new System.Drawing.Size(46, 24);
            this.lblEast.TabIndex = 10;
            this.lblEast.Text = "East";
            // 
            // lblWest
            // 
            this.lblWest.AutoSize = true;
            this.lblWest.Location = new System.Drawing.Point(22, 240);
            this.lblWest.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWest.Name = "lblWest";
            this.lblWest.Size = new System.Drawing.Size(52, 24);
            this.lblWest.TabIndex = 9;
            this.lblWest.Text = "West";
            // 
            // lblNorth
            // 
            this.lblNorth.AutoSize = true;
            this.lblNorth.Location = new System.Drawing.Point(76, 208);
            this.lblNorth.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNorth.Name = "lblNorth";
            this.lblNorth.Size = new System.Drawing.Size(56, 24);
            this.lblNorth.TabIndex = 8;
            this.lblNorth.Text = "North";
            // 
            // tbSouth
            // 
            this.tbSouth.Location = new System.Drawing.Point(134, 269);
            this.tbSouth.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tbSouth.Name = "tbSouth";
            this.tbSouth.Size = new System.Drawing.Size(112, 28);
            this.tbSouth.TabIndex = 8;
            this.tbSouth.TextChanged += new System.EventHandler(this.tbSouth_TextChanged);
            // 
            // tbEast
            // 
            this.tbEast.Location = new System.Drawing.Point(193, 238);
            this.tbEast.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tbEast.Name = "tbEast";
            this.tbEast.Size = new System.Drawing.Size(112, 28);
            this.tbEast.TabIndex = 7;
            this.tbEast.TextChanged += new System.EventHandler(this.tbEast_TextChanged);
            // 
            // tbWest
            // 
            this.tbWest.Location = new System.Drawing.Point(75, 238);
            this.tbWest.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tbWest.Name = "tbWest";
            this.tbWest.Size = new System.Drawing.Size(112, 28);
            this.tbWest.TabIndex = 6;
            this.tbWest.TextChanged += new System.EventHandler(this.tbWest_TextChanged);
            // 
            // tbNorth
            // 
            this.tbNorth.Location = new System.Drawing.Point(134, 206);
            this.tbNorth.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tbNorth.Name = "tbNorth";
            this.tbNorth.Size = new System.Drawing.Size(112, 28);
            this.tbNorth.TabIndex = 5;
            this.tbNorth.TextChanged += new System.EventHandler(this.tbNorth_TextChanged);
            // 
            // rbManual
            // 
            this.rbManual.AutoSize = true;
            this.rbManual.Location = new System.Drawing.Point(17, 128);
            this.rbManual.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.rbManual.Name = "rbManual";
            this.rbManual.Size = new System.Drawing.Size(93, 28);
            this.rbManual.TabIndex = 3;
            this.rbManual.Text = "Manual";
            this.rbManual.UseVisualStyleBackColor = true;
            this.rbManual.Click += new System.EventHandler(this.rbManual_Click);
            // 
            // rbSelectFromList
            // 
            this.rbSelectFromList.AutoSize = true;
            this.rbSelectFromList.Location = new System.Drawing.Point(17, 49);
            this.rbSelectFromList.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.rbSelectFromList.Name = "rbSelectFromList";
            this.rbSelectFromList.Size = new System.Drawing.Size(165, 28);
            this.rbSelectFromList.TabIndex = 1;
            this.rbSelectFromList.Text = "Select From List";
            this.rbSelectFromList.UseVisualStyleBackColor = true;
            this.rbSelectFromList.Click += new System.EventHandler(this.rbSelectFromList_Click);
            // 
            // rbAutomatic
            // 
            this.rbAutomatic.AutoSize = true;
            this.rbAutomatic.Checked = true;
            this.rbAutomatic.Location = new System.Drawing.Point(17, 19);
            this.rbAutomatic.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.rbAutomatic.Name = "rbAutomatic";
            this.rbAutomatic.Size = new System.Drawing.Size(114, 28);
            this.rbAutomatic.TabIndex = 0;
            this.rbAutomatic.TabStop = true;
            this.rbAutomatic.Text = "Automatic";
            this.rbAutomatic.UseVisualStyleBackColor = true;
            this.rbAutomatic.Click += new System.EventHandler(this.rbAutomatic_Click);
            // 
            // tabPageLayers
            // 
            this.tabPageLayers.Controls.Add(this.buttonDown);
            this.tabPageLayers.Controls.Add(this.buttonRemove);
            this.tabPageLayers.Controls.Add(this.buttonUp);
            this.tabPageLayers.Controls.Add(this.listBoxLayers);
            this.tabPageLayers.Location = new System.Drawing.Point(4, 31);
            this.tabPageLayers.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tabPageLayers.Name = "tabPageLayers";
            this.tabPageLayers.Padding = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tabPageLayers.Size = new System.Drawing.Size(735, 364);
            this.tabPageLayers.TabIndex = 2;
            this.tabPageLayers.Text = "Layers";
            this.tabPageLayers.UseVisualStyleBackColor = true;
            this.tabPageLayers.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabPageLayers_Layout);
            // 
            // buttonDown
            // 
            this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDown.Image = global::ScenarioTools.Properties.Resources.down_arrow;
            this.buttonDown.Location = new System.Drawing.Point(688, 119);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(42, 49);
            this.buttonDown.TabIndex = 18;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemove.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemove.Image")));
            this.buttonRemove.Location = new System.Drawing.Point(688, 62);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(42, 49);
            this.buttonRemove.TabIndex = 17;
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUp.Image = global::ScenarioTools.Properties.Resources.up_arrow;
            this.buttonUp.Location = new System.Drawing.Point(688, 7);
            this.buttonUp.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(42, 49);
            this.buttonUp.TabIndex = 16;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // listBoxLayers
            // 
            this.listBoxLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxLayers.FormattingEnabled = true;
            this.listBoxLayers.ItemHeight = 22;
            this.listBoxLayers.Location = new System.Drawing.Point(6, 7);
            this.listBoxLayers.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.listBoxLayers.Name = "listBoxLayers";
            this.listBoxLayers.Size = new System.Drawing.Size(677, 488);
            this.listBoxLayers.TabIndex = 0;
            // 
            // textBoxXCoord
            // 
            this.textBoxXCoord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxXCoord.Location = new System.Drawing.Point(399, 406);
            this.textBoxXCoord.Name = "textBoxXCoord";
            this.textBoxXCoord.Size = new System.Drawing.Size(150, 28);
            this.textBoxXCoord.TabIndex = 12;
            // 
            // textBoxYcoord
            // 
            this.textBoxYcoord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxYcoord.Location = new System.Drawing.Point(582, 406);
            this.textBoxYcoord.Name = "textBoxYcoord";
            this.textBoxYcoord.Size = new System.Drawing.Size(150, 28);
            this.textBoxYcoord.TabIndex = 13;
            // 
            // labelX
            // 
            this.labelX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(377, 408);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(24, 24);
            this.labelX.TabIndex = 14;
            this.labelX.Text = "X";
            // 
            // labelY
            // 
            this.labelY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(562, 408);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(22, 24);
            this.labelY.TabIndex = 15;
            this.labelY.Text = "Y";
            // 
            // btnSaveUserDrawnExtent
            // 
            this.btnSaveUserDrawnExtent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveUserDrawnExtent.Location = new System.Drawing.Point(224, 404);
            this.btnSaveUserDrawnExtent.Name = "btnSaveUserDrawnExtent";
            this.btnSaveUserDrawnExtent.Size = new System.Drawing.Size(122, 30);
            this.btnSaveUserDrawnExtent.TabIndex = 16;
            this.btnSaveUserDrawnExtent.Text = "Save Extent";
            this.btnSaveUserDrawnExtent.UseVisualStyleBackColor = true;
            this.btnSaveUserDrawnExtent.Click += new System.EventHandler(this.btnSaveUserDrawnExtent_Click);
            // 
            // STMapDesigner
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(752, 456);
            this.Controls.Add(this.btnSaveUserDrawnExtent);
            this.Controls.Add(this.labelY);
            this.Controls.Add(this.labelX);
            this.Controls.Add(this.textBoxYcoord);
            this.Controls.Add(this.textBoxXCoord);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(760, 488);
            this.Name = "STMapDesigner";
            this.Text = "Map Designer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.STMapDesigner_FormClosing);
            this.Load += new System.EventHandler(this.STMapDesigner_Load);
            this.ResizeBegin += new System.EventHandler(this.STMapDesigner_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.STMapDesigner_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.STMapDesigner_LocationChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageLayout.ResumeLayout(false);
            this.tabPageExtent.ResumeLayout(false);
            this.tabPageExtent.PerformLayout();
            this.tabPageLayers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddLayer;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Panel panelMapLegend;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLayout;
        private System.Windows.Forms.TabPage tabPageExtent;
        private System.Windows.Forms.TabPage tabPageLayers;
        private System.Windows.Forms.RadioButton rbManual;
        private System.Windows.Forms.RadioButton rbSelectFromList;
        private System.Windows.Forms.RadioButton rbAutomatic;
        private System.Windows.Forms.TextBox tbSouth;
        private System.Windows.Forms.TextBox tbEast;
        private System.Windows.Forms.TextBox tbWest;
        private System.Windows.Forms.TextBox tbNorth;
        private System.Windows.Forms.Label lblSouth;
        private System.Windows.Forms.Label lblEast;
        private System.Windows.Forms.Label lblWest;
        private System.Windows.Forms.Label lblNorth;
        private System.Windows.Forms.TextBox tbExtentName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnSaveExtent;
        private System.Windows.Forms.ComboBox comboBoxExtents;
        private System.Windows.Forms.ListBox listBoxLayers;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.TextBox textBoxXCoord;
        private System.Windows.Forms.TextBox textBoxYcoord;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ckbxBackgroundImage;
        private System.Windows.Forms.TextBox textBoxBrightness;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnSaveUserDrawnExtent;
    }
}