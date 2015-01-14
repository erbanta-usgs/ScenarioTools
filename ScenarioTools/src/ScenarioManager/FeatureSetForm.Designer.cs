namespace ScenarioManager
{
    partial class FeatureSetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeatureSetForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pgGeneral = new System.Windows.Forms.TabPage();
            this.grpboxSmpInterpretationMethod = new System.Windows.Forms.GroupBox();
            this.rbStepwise = new System.Windows.Forms.RadioButton();
            this.rbPiecewise = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbxLabelFeatures = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbxKeyField = new System.Windows.Forms.ComboBox();
            this.btnFindSmpFile = new System.Windows.Forms.Button();
            this.btnFindShapefile = new System.Windows.Forms.Button();
            this.tbSmp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbShapefile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pgWells = new System.Windows.Forms.TabPage();
            this.groupBoxWellsLayerAssignment = new System.Windows.Forms.GroupBox();
            this.labelWells8 = new System.Windows.Forms.Label();
            this.labelWells7 = new System.Windows.Forms.Label();
            this.cmbxWellsBottomElevationAttribute = new System.Windows.Forms.ComboBox();
            this.cmbxWellsTopElevationAttribute = new System.Windows.Forms.ComboBox();
            this.cmbxWellsLayerAttribute = new System.Windows.Forms.ComboBox();
            this.rbWellsByTops = new System.Windows.Forms.RadioButton();
            this.udWellsLayerPicker = new System.Windows.Forms.NumericUpDown();
            this.rbWellsLayerAttribute = new System.Windows.Forms.RadioButton();
            this.rbWellsSameLayer = new System.Windows.Forms.RadioButton();
            this.pgRiv = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cmbxRivBottomElevAttribute = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxRivThicknessUniformValue = new System.Windows.Forms.TextBox();
            this.rbRivThicknessFromAttribute = new System.Windows.Forms.RadioButton();
            this.rbRivThicknessUniform = new System.Windows.Forms.RadioButton();
            this.cmbxRivThicknessAttribute = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxRivWidthUniformValue = new System.Windows.Forms.TextBox();
            this.rbRivWidthFromAttribute = new System.Windows.Forms.RadioButton();
            this.rbRivWidthUniform = new System.Windows.Forms.RadioButton();
            this.cmbxRivWidthAttribute = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxRivHydCondUniformValue = new System.Windows.Forms.TextBox();
            this.rbRivHydCondFromAttribute = new System.Windows.Forms.RadioButton();
            this.rbRivHydCondUniform = new System.Windows.Forms.RadioButton();
            this.cmbxRivHydCondAttribute = new System.Windows.Forms.ComboBox();
            this.btnRivSecondarySmp = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxRivSecondarySmp = new System.Windows.Forms.TextBox();
            this.groupBoxRivLayerAssignment = new System.Windows.Forms.GroupBox();
            this.cmbxRivLayerAttribute = new System.Windows.Forms.ComboBox();
            this.udRivLayerPicker = new System.Windows.Forms.NumericUpDown();
            this.rbRivLayerAttribute = new System.Windows.Forms.RadioButton();
            this.rbRivSameLayer = new System.Windows.Forms.RadioButton();
            this.pgChd = new System.Windows.Forms.TabPage();
            this.groupBoxChdLayerAssignment = new System.Windows.Forms.GroupBox();
            this.cmbxChdLayerAttribute = new System.Windows.Forms.ComboBox();
            this.udChdLayerPicker = new System.Windows.Forms.NumericUpDown();
            this.rbChdLayerAttribute = new System.Windows.Forms.RadioButton();
            this.rbChdSameLayer = new System.Windows.Forms.RadioButton();
            this.pgRch = new System.Windows.Forms.TabPage();
            this.pgGhb = new System.Windows.Forms.TabPage();
            this.groupBoxGhbLayerAssignment = new System.Windows.Forms.GroupBox();
            this.cmbxGhbLayerAttribute = new System.Windows.Forms.ComboBox();
            this.udGhbLayerPicker = new System.Windows.Forms.NumericUpDown();
            this.rbGhbLayerAttribute = new System.Windows.Forms.RadioButton();
            this.rbGhbSameLayer = new System.Windows.Forms.RadioButton();
            this.btnGhbSecondarySmp = new System.Windows.Forms.Button();
            this.labelSecondarySmp = new System.Windows.Forms.Label();
            this.textBoxGhbSecondarySmp = new System.Windows.Forms.TextBox();
            this.groupBoxGhbLeakance = new System.Windows.Forms.GroupBox();
            this.textBoxGhbLeakanceUniformValue = new System.Windows.Forms.TextBox();
            this.rbGhbLeakanceFromAttribute = new System.Windows.Forms.RadioButton();
            this.rbGhbLeakanceUniform = new System.Windows.Forms.RadioButton();
            this.cmbxGhbLeakanceAttribute = new System.Windows.Forms.ComboBox();
            this.pgDisplay = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.toolStripDisplay = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelPan = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonReCenter = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomToGrid = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFullExtent = new System.Windows.Forms.ToolStripButton();
            this.cbxShowBackground = new System.Windows.Forms.CheckBox();
            this.panelExplanation = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panelIndexMap = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelXyCoords = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelRowCol = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.pgGeneral.SuspendLayout();
            this.grpboxSmpInterpretationMethod.SuspendLayout();
            this.pgWells.SuspendLayout();
            this.groupBoxWellsLayerAssignment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWellsLayerPicker)).BeginInit();
            this.pgRiv.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxRivLayerAssignment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udRivLayerPicker)).BeginInit();
            this.pgChd.SuspendLayout();
            this.groupBoxChdLayerAssignment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udChdLayerPicker)).BeginInit();
            this.pgGhb.SuspendLayout();
            this.groupBoxGhbLayerAssignment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udGhbLayerPicker)).BeginInit();
            this.groupBoxGhbLeakance.SuspendLayout();
            this.pgDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStripDisplay.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.pgGeneral);
            this.tabControl1.Controls.Add(this.pgWells);
            this.tabControl1.Controls.Add(this.pgRiv);
            this.tabControl1.Controls.Add(this.pgChd);
            this.tabControl1.Controls.Add(this.pgRch);
            this.tabControl1.Controls.Add(this.pgGhb);
            this.tabControl1.Controls.Add(this.pgDisplay);
            this.tabControl1.Location = new System.Drawing.Point(3, 2);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(798, 522);
            this.tabControl1.TabIndex = 0;
            // 
            // pgGeneral
            // 
            this.pgGeneral.Controls.Add(this.grpboxSmpInterpretationMethod);
            this.pgGeneral.Controls.Add(this.label9);
            this.pgGeneral.Controls.Add(this.cmbxLabelFeatures);
            this.pgGeneral.Controls.Add(this.label6);
            this.pgGeneral.Controls.Add(this.cmbxKeyField);
            this.pgGeneral.Controls.Add(this.btnFindSmpFile);
            this.pgGeneral.Controls.Add(this.btnFindShapefile);
            this.pgGeneral.Controls.Add(this.tbSmp);
            this.pgGeneral.Controls.Add(this.label3);
            this.pgGeneral.Controls.Add(this.tbShapefile);
            this.pgGeneral.Controls.Add(this.label2);
            this.pgGeneral.Controls.Add(this.tbName);
            this.pgGeneral.Controls.Add(this.label1);
            this.pgGeneral.Location = new System.Drawing.Point(4, 31);
            this.pgGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.pgGeneral.Name = "pgGeneral";
            this.pgGeneral.Padding = new System.Windows.Forms.Padding(4);
            this.pgGeneral.Size = new System.Drawing.Size(790, 487);
            this.pgGeneral.TabIndex = 0;
            this.pgGeneral.Text = "General";
            this.pgGeneral.UseVisualStyleBackColor = true;
            this.pgGeneral.Layout += new System.Windows.Forms.LayoutEventHandler(this.pgGeneral_Layout);
            // 
            // grpboxSmpInterpretationMethod
            // 
            this.grpboxSmpInterpretationMethod.Controls.Add(this.rbStepwise);
            this.grpboxSmpInterpretationMethod.Controls.Add(this.rbPiecewise);
            this.grpboxSmpInterpretationMethod.Location = new System.Drawing.Point(15, 261);
            this.grpboxSmpInterpretationMethod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpboxSmpInterpretationMethod.Name = "grpboxSmpInterpretationMethod";
            this.grpboxSmpInterpretationMethod.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpboxSmpInterpretationMethod.Size = new System.Drawing.Size(341, 103);
            this.grpboxSmpInterpretationMethod.TabIndex = 15;
            this.grpboxSmpInterpretationMethod.TabStop = false;
            this.grpboxSmpInterpretationMethod.Text = "Time-Series Interpretation Method";
            // 
            // rbStepwise
            // 
            this.rbStepwise.Location = new System.Drawing.Point(25, 59);
            this.rbStepwise.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbStepwise.Name = "rbStepwise";
            this.rbStepwise.Size = new System.Drawing.Size(107, 28);
            this.rbStepwise.TabIndex = 1;
            this.rbStepwise.Text = "Stepwise";
            this.rbStepwise.UseVisualStyleBackColor = true;
            this.rbStepwise.CheckedChanged += new System.EventHandler(this.rbStepwise_CheckedChanged);
            // 
            // rbPiecewise
            // 
            this.rbPiecewise.Location = new System.Drawing.Point(25, 26);
            this.rbPiecewise.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbPiecewise.Name = "rbPiecewise";
            this.rbPiecewise.Size = new System.Drawing.Size(225, 28);
            this.rbPiecewise.TabIndex = 0;
            this.rbPiecewise.Text = "Piecewise linear";
            this.rbPiecewise.UseVisualStyleBackColor = true;
            this.rbPiecewise.CheckedChanged += new System.EventHandler(this.rbPiecewise_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(238, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(325, 24);
            this.label9.TabIndex = 14;
            this.label9.Text = "Label features in MODFLOW input file";
            // 
            // cmbxLabelFeatures
            // 
            this.cmbxLabelFeatures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbxLabelFeatures.FormattingEnabled = true;
            this.cmbxLabelFeatures.Items.AddRange(new object[] {
            "No labels",
            "Feature set name only",
            "Feature name only",
            "Feature set name and feature name"});
            this.cmbxLabelFeatures.Location = new System.Drawing.Point(242, 31);
            this.cmbxLabelFeatures.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxLabelFeatures.Name = "cmbxLabelFeatures";
            this.cmbxLabelFeatures.Size = new System.Drawing.Size(342, 30);
            this.cmbxLabelFeatures.TabIndex = 6;
            this.cmbxLabelFeatures.SelectedIndexChanged += new System.EventHandler(this.cbxLabelFeatures_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 193);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(361, 24);
            this.label6.TabIndex = 9;
            this.label6.Text = "Attribute linking shapefile to time-series file";
            // 
            // cmbxKeyField
            // 
            this.cmbxKeyField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbxKeyField.FormattingEnabled = true;
            this.cmbxKeyField.Location = new System.Drawing.Point(14, 220);
            this.cmbxKeyField.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxKeyField.Name = "cmbxKeyField";
            this.cmbxKeyField.Size = new System.Drawing.Size(342, 30);
            this.cmbxKeyField.Sorted = true;
            this.cmbxKeyField.TabIndex = 5;
            this.cmbxKeyField.SelectedValueChanged += new System.EventHandler(this.cbxKeyField_SelectedValueChanged);
            // 
            // btnFindSmpFile
            // 
            this.btnFindSmpFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindSmpFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindSmpFile.Image = ((System.Drawing.Image)(resources.GetObject("btnFindSmpFile.Image")));
            this.btnFindSmpFile.Location = new System.Drawing.Point(745, 151);
            this.btnFindSmpFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnFindSmpFile.Name = "btnFindSmpFile";
            this.btnFindSmpFile.Size = new System.Drawing.Size(30, 30);
            this.btnFindSmpFile.TabIndex = 4;
            this.btnFindSmpFile.UseVisualStyleBackColor = true;
            this.btnFindSmpFile.Click += new System.EventHandler(this.btnFindSmpFile_Click);
            // 
            // btnFindShapefile
            // 
            this.btnFindShapefile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindShapefile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindShapefile.Image = ((System.Drawing.Image)(resources.GetObject("btnFindShapefile.Image")));
            this.btnFindShapefile.Location = new System.Drawing.Point(745, 90);
            this.btnFindShapefile.Margin = new System.Windows.Forms.Padding(4);
            this.btnFindShapefile.Name = "btnFindShapefile";
            this.btnFindShapefile.Size = new System.Drawing.Size(30, 30);
            this.btnFindShapefile.TabIndex = 2;
            this.btnFindShapefile.UseVisualStyleBackColor = true;
            this.btnFindShapefile.Click += new System.EventHandler(this.btnFindShapefile_Click);
            // 
            // tbSmp
            // 
            this.tbSmp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSmp.Location = new System.Drawing.Point(14, 152);
            this.tbSmp.Margin = new System.Windows.Forms.Padding(4);
            this.tbSmp.Name = "tbSmp";
            this.tbSmp.Size = new System.Drawing.Size(730, 28);
            this.tbSmp.TabIndex = 3;
            this.tbSmp.TextChanged += new System.EventHandler(this.tbSmp_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 127);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(194, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Time-series (SMP) file";
            // 
            // tbShapefile
            // 
            this.tbShapefile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbShapefile.Location = new System.Drawing.Point(14, 91);
            this.tbShapefile.Margin = new System.Windows.Forms.Padding(4);
            this.tbShapefile.Name = "tbShapefile";
            this.tbShapefile.Size = new System.Drawing.Size(730, 28);
            this.tbShapefile.TabIndex = 1;
            this.tbShapefile.TextChanged += new System.EventHandler(this.tbShapefile_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 65);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Shapefile";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(14, 31);
            this.tbName.Margin = new System.Windows.Forms.Padding(4);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(191, 28);
            this.tbName.TabIndex = 0;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // pgWells
            // 
            this.pgWells.Controls.Add(this.groupBoxWellsLayerAssignment);
            this.pgWells.Location = new System.Drawing.Point(4, 31);
            this.pgWells.Name = "pgWells";
            this.pgWells.Size = new System.Drawing.Size(790, 487);
            this.pgWells.TabIndex = 3;
            this.pgWells.Tag = "WellType";
            this.pgWells.Text = "Wells";
            this.pgWells.UseVisualStyleBackColor = true;
            this.pgWells.Layout += new System.Windows.Forms.LayoutEventHandler(this.pgWells_Layout);
            // 
            // groupBoxWellsLayerAssignment
            // 
            this.groupBoxWellsLayerAssignment.Controls.Add(this.labelWells8);
            this.groupBoxWellsLayerAssignment.Controls.Add(this.labelWells7);
            this.groupBoxWellsLayerAssignment.Controls.Add(this.cmbxWellsBottomElevationAttribute);
            this.groupBoxWellsLayerAssignment.Controls.Add(this.cmbxWellsTopElevationAttribute);
            this.groupBoxWellsLayerAssignment.Controls.Add(this.cmbxWellsLayerAttribute);
            this.groupBoxWellsLayerAssignment.Controls.Add(this.rbWellsByTops);
            this.groupBoxWellsLayerAssignment.Controls.Add(this.udWellsLayerPicker);
            this.groupBoxWellsLayerAssignment.Controls.Add(this.rbWellsLayerAttribute);
            this.groupBoxWellsLayerAssignment.Controls.Add(this.rbWellsSameLayer);
            this.groupBoxWellsLayerAssignment.Location = new System.Drawing.Point(6, 7);
            this.groupBoxWellsLayerAssignment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxWellsLayerAssignment.Name = "groupBoxWellsLayerAssignment";
            this.groupBoxWellsLayerAssignment.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxWellsLayerAssignment.Size = new System.Drawing.Size(400, 311);
            this.groupBoxWellsLayerAssignment.TabIndex = 12;
            this.groupBoxWellsLayerAssignment.TabStop = false;
            this.groupBoxWellsLayerAssignment.Text = "Layer Assignment For Wells";
            // 
            // labelWells8
            // 
            this.labelWells8.AutoSize = true;
            this.labelWells8.Location = new System.Drawing.Point(38, 237);
            this.labelWells8.Name = "labelWells8";
            this.labelWells8.Size = new System.Drawing.Size(223, 24);
            this.labelWells8.TabIndex = 8;
            this.labelWells8.Text = "Bottom elevation attribute:";
            // 
            // labelWells7
            // 
            this.labelWells7.AutoSize = true;
            this.labelWells7.Location = new System.Drawing.Point(39, 171);
            this.labelWells7.Name = "labelWells7";
            this.labelWells7.Size = new System.Drawing.Size(199, 24);
            this.labelWells7.TabIndex = 7;
            this.labelWells7.Text = "Top elevation attribute:";
            // 
            // cmbxWellsBottomElevationAttribute
            // 
            this.cmbxWellsBottomElevationAttribute.FormattingEnabled = true;
            this.cmbxWellsBottomElevationAttribute.Location = new System.Drawing.Point(42, 264);
            this.cmbxWellsBottomElevationAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxWellsBottomElevationAttribute.Name = "cmbxWellsBottomElevationAttribute";
            this.cmbxWellsBottomElevationAttribute.Size = new System.Drawing.Size(337, 30);
            this.cmbxWellsBottomElevationAttribute.TabIndex = 6;
            this.cmbxWellsBottomElevationAttribute.TabStop = false;
            this.cmbxWellsBottomElevationAttribute.SelectedValueChanged += new System.EventHandler(this.cbxWellsBottomElevationAttribute_SelectedValueChanged);
            // 
            // cmbxWellsTopElevationAttribute
            // 
            this.cmbxWellsTopElevationAttribute.FormattingEnabled = true;
            this.cmbxWellsTopElevationAttribute.Location = new System.Drawing.Point(42, 198);
            this.cmbxWellsTopElevationAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxWellsTopElevationAttribute.Name = "cmbxWellsTopElevationAttribute";
            this.cmbxWellsTopElevationAttribute.Size = new System.Drawing.Size(337, 30);
            this.cmbxWellsTopElevationAttribute.TabIndex = 5;
            this.cmbxWellsTopElevationAttribute.TabStop = false;
            this.cmbxWellsTopElevationAttribute.SelectedValueChanged += new System.EventHandler(this.cbxWellsTopElevationAttribute_SelectedValueChanged);
            // 
            // cmbxWellsLayerAttribute
            // 
            this.cmbxWellsLayerAttribute.Enabled = false;
            this.cmbxWellsLayerAttribute.FormattingEnabled = true;
            this.cmbxWellsLayerAttribute.Location = new System.Drawing.Point(42, 99);
            this.cmbxWellsLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxWellsLayerAttribute.Name = "cmbxWellsLayerAttribute";
            this.cmbxWellsLayerAttribute.Size = new System.Drawing.Size(337, 30);
            this.cmbxWellsLayerAttribute.Sorted = true;
            this.cmbxWellsLayerAttribute.TabIndex = 4;
            this.cmbxWellsLayerAttribute.TabStop = false;
            this.cmbxWellsLayerAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxWellsFeatureLayerAttribute_SelectedValueChanged);
            // 
            // rbWellsByTops
            // 
            this.rbWellsByTops.Location = new System.Drawing.Point(19, 140);
            this.rbWellsByTops.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbWellsByTops.Name = "rbWellsByTops";
            this.rbWellsByTops.Size = new System.Drawing.Size(192, 27);
            this.rbWellsByTops.TabIndex = 3;
            this.rbWellsByTops.Text = "Use cell tops and bottoms";
            this.rbWellsByTops.UseVisualStyleBackColor = true;
            this.rbWellsByTops.CheckedChanged += new System.EventHandler(this.rbWellsByTops_CheckedChanged);
            // 
            // udWellsLayerPicker
            // 
            this.udWellsLayerPicker.Location = new System.Drawing.Point(209, 29);
            this.udWellsLayerPicker.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.udWellsLayerPicker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udWellsLayerPicker.Name = "udWellsLayerPicker";
            this.udWellsLayerPicker.Size = new System.Drawing.Size(58, 28);
            this.udWellsLayerPicker.TabIndex = 2;
            this.udWellsLayerPicker.TabStop = false;
            this.udWellsLayerPicker.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udWellsLayerPicker.ValueChanged += new System.EventHandler(this.udWellsLayerPicker_ValueChanged);
            // 
            // rbWellsLayerAttribute
            // 
            this.rbWellsLayerAttribute.Location = new System.Drawing.Point(19, 61);
            this.rbWellsLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbWellsLayerAttribute.Name = "rbWellsLayerAttribute";
            this.rbWellsLayerAttribute.Size = new System.Drawing.Size(296, 37);
            this.rbWellsLayerAttribute.TabIndex = 1;
            this.rbWellsLayerAttribute.Text = "Assign layer by attribute:";
            this.rbWellsLayerAttribute.UseVisualStyleBackColor = true;
            this.rbWellsLayerAttribute.CheckedChanged += new System.EventHandler(this.rbWellsLayerAttribute_CheckedChanged);
            // 
            // rbWellsSameLayer
            // 
            this.rbWellsSameLayer.Location = new System.Drawing.Point(19, 27);
            this.rbWellsSameLayer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbWellsSameLayer.Name = "rbWellsSameLayer";
            this.rbWellsSameLayer.Size = new System.Drawing.Size(184, 30);
            this.rbWellsSameLayer.TabIndex = 0;
            this.rbWellsSameLayer.Text = "Assign all to layer:";
            this.rbWellsSameLayer.UseVisualStyleBackColor = true;
            this.rbWellsSameLayer.CheckedChanged += new System.EventHandler(this.rbWellsSameLayer_CheckedChanged);
            // 
            // pgRiv
            // 
            this.pgRiv.Controls.Add(this.groupBox4);
            this.pgRiv.Controls.Add(this.groupBox3);
            this.pgRiv.Controls.Add(this.groupBox2);
            this.pgRiv.Controls.Add(this.groupBox1);
            this.pgRiv.Controls.Add(this.btnRivSecondarySmp);
            this.pgRiv.Controls.Add(this.label14);
            this.pgRiv.Controls.Add(this.textBoxRivSecondarySmp);
            this.pgRiv.Controls.Add(this.groupBoxRivLayerAssignment);
            this.pgRiv.Location = new System.Drawing.Point(4, 31);
            this.pgRiv.Name = "pgRiv";
            this.pgRiv.Size = new System.Drawing.Size(790, 487);
            this.pgRiv.TabIndex = 7;
            this.pgRiv.Tag = "RiverType";
            this.pgRiv.Text = "Rivers";
            this.pgRiv.UseVisualStyleBackColor = true;
            this.pgRiv.Layout += new System.Windows.Forms.LayoutEventHandler(this.pgRiv_Layout);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.cmbxRivBottomElevAttribute);
            this.groupBox4.Location = new System.Drawing.Point(378, 67);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(323, 102);
            this.groupBox4.TabIndex = 34;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Riverbed Bottom Elevation";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(15, 29);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(156, 31);
            this.label15.TabIndex = 19;
            this.label15.Text = "Attribute:";
            // 
            // cmbxRivBottomElevAttribute
            // 
            this.cmbxRivBottomElevAttribute.FormattingEnabled = true;
            this.cmbxRivBottomElevAttribute.Location = new System.Drawing.Point(19, 62);
            this.cmbxRivBottomElevAttribute.Name = "cmbxRivBottomElevAttribute";
            this.cmbxRivBottomElevAttribute.Size = new System.Drawing.Size(293, 30);
            this.cmbxRivBottomElevAttribute.TabIndex = 18;
            this.cmbxRivBottomElevAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxRivBottomElevAttribute_SelectedValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxRivThicknessUniformValue);
            this.groupBox3.Controls.Add(this.rbRivThicknessFromAttribute);
            this.groupBox3.Controls.Add(this.rbRivThicknessUniform);
            this.groupBox3.Controls.Add(this.cmbxRivThicknessAttribute);
            this.groupBox3.Location = new System.Drawing.Point(18, 344);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(343, 132);
            this.groupBox3.TabIndex = 33;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Riverbed Thickness";
            // 
            // textBoxRivThicknessUniformValue
            // 
            this.textBoxRivThicknessUniformValue.Location = new System.Drawing.Point(183, 27);
            this.textBoxRivThicknessUniformValue.Name = "textBoxRivThicknessUniformValue";
            this.textBoxRivThicknessUniformValue.Size = new System.Drawing.Size(152, 28);
            this.textBoxRivThicknessUniformValue.TabIndex = 22;
            this.textBoxRivThicknessUniformValue.TextChanged += new System.EventHandler(this.textBoxRivThicknessUniformValue_TextChanged);
            // 
            // rbRivThicknessFromAttribute
            // 
            this.rbRivThicknessFromAttribute.Location = new System.Drawing.Point(26, 60);
            this.rbRivThicknessFromAttribute.Name = "rbRivThicknessFromAttribute";
            this.rbRivThicknessFromAttribute.Size = new System.Drawing.Size(232, 31);
            this.rbRivThicknessFromAttribute.TabIndex = 21;
            this.rbRivThicknessFromAttribute.TabStop = true;
            this.rbRivThicknessFromAttribute.Text = "Value from attribute:";
            this.rbRivThicknessFromAttribute.UseVisualStyleBackColor = true;
            this.rbRivThicknessFromAttribute.CheckedChanged += new System.EventHandler(this.rbRivThicknessFromAttribute_CheckedChanged);
            // 
            // rbRivThicknessUniform
            // 
            this.rbRivThicknessUniform.Location = new System.Drawing.Point(26, 27);
            this.rbRivThicknessUniform.Name = "rbRivThicknessUniform";
            this.rbRivThicknessUniform.Size = new System.Drawing.Size(157, 28);
            this.rbRivThicknessUniform.TabIndex = 20;
            this.rbRivThicknessUniform.TabStop = true;
            this.rbRivThicknessUniform.Text = "Uniform value:";
            this.rbRivThicknessUniform.UseVisualStyleBackColor = true;
            this.rbRivThicknessUniform.CheckedChanged += new System.EventHandler(this.rbRivThicknessUniform_CheckedChanged);
            // 
            // cmbxRivThicknessAttribute
            // 
            this.cmbxRivThicknessAttribute.FormattingEnabled = true;
            this.cmbxRivThicknessAttribute.Location = new System.Drawing.Point(58, 95);
            this.cmbxRivThicknessAttribute.Name = "cmbxRivThicknessAttribute";
            this.cmbxRivThicknessAttribute.Size = new System.Drawing.Size(277, 30);
            this.cmbxRivThicknessAttribute.TabIndex = 18;
            this.cmbxRivThicknessAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxRivThicknessAttribute_SelectedValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxRivWidthUniformValue);
            this.groupBox2.Controls.Add(this.rbRivWidthFromAttribute);
            this.groupBox2.Controls.Add(this.rbRivWidthUniform);
            this.groupBox2.Controls.Add(this.cmbxRivWidthAttribute);
            this.groupBox2.Location = new System.Drawing.Point(18, 201);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(343, 128);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "River Width";
            // 
            // textBoxRivWidthUniformValue
            // 
            this.textBoxRivWidthUniformValue.Location = new System.Drawing.Point(183, 27);
            this.textBoxRivWidthUniformValue.Name = "textBoxRivWidthUniformValue";
            this.textBoxRivWidthUniformValue.Size = new System.Drawing.Size(152, 28);
            this.textBoxRivWidthUniformValue.TabIndex = 22;
            this.textBoxRivWidthUniformValue.TextChanged += new System.EventHandler(this.textBoxRivWidthUniformValue_TextChanged);
            // 
            // rbRivWidthFromAttribute
            // 
            this.rbRivWidthFromAttribute.Location = new System.Drawing.Point(26, 56);
            this.rbRivWidthFromAttribute.Name = "rbRivWidthFromAttribute";
            this.rbRivWidthFromAttribute.Size = new System.Drawing.Size(232, 31);
            this.rbRivWidthFromAttribute.TabIndex = 21;
            this.rbRivWidthFromAttribute.TabStop = true;
            this.rbRivWidthFromAttribute.Text = "Value from attribute:";
            this.rbRivWidthFromAttribute.UseVisualStyleBackColor = true;
            this.rbRivWidthFromAttribute.CheckedChanged += new System.EventHandler(this.rbRivWidthFromAttribute_CheckedChanged);
            // 
            // rbRivWidthUniform
            // 
            this.rbRivWidthUniform.Location = new System.Drawing.Point(26, 27);
            this.rbRivWidthUniform.Name = "rbRivWidthUniform";
            this.rbRivWidthUniform.Size = new System.Drawing.Size(157, 28);
            this.rbRivWidthUniform.TabIndex = 20;
            this.rbRivWidthUniform.TabStop = true;
            this.rbRivWidthUniform.Text = "Uniform value:";
            this.rbRivWidthUniform.UseVisualStyleBackColor = true;
            this.rbRivWidthUniform.CheckedChanged += new System.EventHandler(this.rbRivWidthUniform_CheckedChanged);
            // 
            // cmbxRivWidthAttribute
            // 
            this.cmbxRivWidthAttribute.FormattingEnabled = true;
            this.cmbxRivWidthAttribute.Location = new System.Drawing.Point(58, 91);
            this.cmbxRivWidthAttribute.Name = "cmbxRivWidthAttribute";
            this.cmbxRivWidthAttribute.Size = new System.Drawing.Size(277, 30);
            this.cmbxRivWidthAttribute.TabIndex = 18;
            this.cmbxRivWidthAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxRivWidthAttribute_SelectedValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxRivHydCondUniformValue);
            this.groupBox1.Controls.Add(this.rbRivHydCondFromAttribute);
            this.groupBox1.Controls.Add(this.rbRivHydCondUniform);
            this.groupBox1.Controls.Add(this.cmbxRivHydCondAttribute);
            this.groupBox1.Location = new System.Drawing.Point(18, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(343, 128);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hydraulic Conductivity";
            // 
            // textBoxRivHydCondUniformValue
            // 
            this.textBoxRivHydCondUniformValue.Location = new System.Drawing.Point(183, 27);
            this.textBoxRivHydCondUniformValue.Name = "textBoxRivHydCondUniformValue";
            this.textBoxRivHydCondUniformValue.Size = new System.Drawing.Size(152, 28);
            this.textBoxRivHydCondUniformValue.TabIndex = 22;
            this.textBoxRivHydCondUniformValue.TextChanged += new System.EventHandler(this.textBoxRivHydCondUniformValue_TextChanged);
            // 
            // rbRivHydCondFromAttribute
            // 
            this.rbRivHydCondFromAttribute.Location = new System.Drawing.Point(26, 58);
            this.rbRivHydCondFromAttribute.Name = "rbRivHydCondFromAttribute";
            this.rbRivHydCondFromAttribute.Size = new System.Drawing.Size(265, 31);
            this.rbRivHydCondFromAttribute.TabIndex = 21;
            this.rbRivHydCondFromAttribute.TabStop = true;
            this.rbRivHydCondFromAttribute.Text = "Value from attribute:";
            this.rbRivHydCondFromAttribute.UseVisualStyleBackColor = true;
            this.rbRivHydCondFromAttribute.CheckedChanged += new System.EventHandler(this.rbRivHydCondFromAttribute_CheckedChanged);
            // 
            // rbRivHydCondUniform
            // 
            this.rbRivHydCondUniform.Location = new System.Drawing.Point(26, 27);
            this.rbRivHydCondUniform.Name = "rbRivHydCondUniform";
            this.rbRivHydCondUniform.Size = new System.Drawing.Size(151, 28);
            this.rbRivHydCondUniform.TabIndex = 20;
            this.rbRivHydCondUniform.TabStop = true;
            this.rbRivHydCondUniform.Text = "Uniform value:";
            this.rbRivHydCondUniform.UseVisualStyleBackColor = true;
            this.rbRivHydCondUniform.CheckedChanged += new System.EventHandler(this.rbRivHydCondUniform_CheckedChanged);
            // 
            // cmbxRivHydCondAttribute
            // 
            this.cmbxRivHydCondAttribute.FormattingEnabled = true;
            this.cmbxRivHydCondAttribute.Location = new System.Drawing.Point(58, 91);
            this.cmbxRivHydCondAttribute.Name = "cmbxRivHydCondAttribute";
            this.cmbxRivHydCondAttribute.Size = new System.Drawing.Size(277, 30);
            this.cmbxRivHydCondAttribute.TabIndex = 18;
            this.cmbxRivHydCondAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxRivHydCondAttribute_SelectedValueChanged);
            // 
            // btnRivSecondarySmp
            // 
            this.btnRivSecondarySmp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRivSecondarySmp.Image = ((System.Drawing.Image)(resources.GetObject("btnRivSecondarySmp.Image")));
            this.btnRivSecondarySmp.Location = new System.Drawing.Point(701, 34);
            this.btnRivSecondarySmp.Name = "btnRivSecondarySmp";
            this.btnRivSecondarySmp.Size = new System.Drawing.Size(30, 30);
            this.btnRivSecondarySmp.TabIndex = 30;
            this.btnRivSecondarySmp.UseVisualStyleBackColor = true;
            this.btnRivSecondarySmp.Click += new System.EventHandler(this.btnRivSecondarySmp_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 8);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(557, 24);
            this.label14.TabIndex = 29;
            this.label14.Text = "Secondary time-series (SMP) file (to control active status; optional)";
            // 
            // textBoxRivSecondarySmp
            // 
            this.textBoxRivSecondarySmp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRivSecondarySmp.Location = new System.Drawing.Point(17, 35);
            this.textBoxRivSecondarySmp.Name = "textBoxRivSecondarySmp";
            this.textBoxRivSecondarySmp.Size = new System.Drawing.Size(684, 28);
            this.textBoxRivSecondarySmp.TabIndex = 28;
            this.textBoxRivSecondarySmp.TextChanged += new System.EventHandler(this.textBoxRivSecondarySmp_TextChanged);
            // 
            // groupBoxRivLayerAssignment
            // 
            this.groupBoxRivLayerAssignment.Controls.Add(this.cmbxRivLayerAttribute);
            this.groupBoxRivLayerAssignment.Controls.Add(this.udRivLayerPicker);
            this.groupBoxRivLayerAssignment.Controls.Add(this.rbRivLayerAttribute);
            this.groupBoxRivLayerAssignment.Controls.Add(this.rbRivSameLayer);
            this.groupBoxRivLayerAssignment.Location = new System.Drawing.Point(378, 176);
            this.groupBoxRivLayerAssignment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxRivLayerAssignment.Name = "groupBoxRivLayerAssignment";
            this.groupBoxRivLayerAssignment.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxRivLayerAssignment.Size = new System.Drawing.Size(323, 147);
            this.groupBoxRivLayerAssignment.TabIndex = 27;
            this.groupBoxRivLayerAssignment.TabStop = false;
            this.groupBoxRivLayerAssignment.Text = "Layer Assignment For River Cells";
            // 
            // cmbxRivLayerAttribute
            // 
            this.cmbxRivLayerAttribute.Enabled = false;
            this.cmbxRivLayerAttribute.FormattingEnabled = true;
            this.cmbxRivLayerAttribute.Location = new System.Drawing.Point(42, 106);
            this.cmbxRivLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxRivLayerAttribute.Name = "cmbxRivLayerAttribute";
            this.cmbxRivLayerAttribute.Size = new System.Drawing.Size(270, 30);
            this.cmbxRivLayerAttribute.Sorted = true;
            this.cmbxRivLayerAttribute.TabIndex = 4;
            this.cmbxRivLayerAttribute.TabStop = false;
            this.cmbxRivLayerAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxRivLayerAttribute_SelectedValueChanged);
            // 
            // udRivLayerPicker
            // 
            this.udRivLayerPicker.Location = new System.Drawing.Point(209, 29);
            this.udRivLayerPicker.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.udRivLayerPicker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udRivLayerPicker.Name = "udRivLayerPicker";
            this.udRivLayerPicker.Size = new System.Drawing.Size(58, 28);
            this.udRivLayerPicker.TabIndex = 2;
            this.udRivLayerPicker.TabStop = false;
            this.udRivLayerPicker.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udRivLayerPicker.ValueChanged += new System.EventHandler(this.udRivLayerPicker_ValueChanged);
            // 
            // rbRivLayerAttribute
            // 
            this.rbRivLayerAttribute.Location = new System.Drawing.Point(19, 66);
            this.rbRivLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbRivLayerAttribute.Name = "rbRivLayerAttribute";
            this.rbRivLayerAttribute.Size = new System.Drawing.Size(273, 37);
            this.rbRivLayerAttribute.TabIndex = 1;
            this.rbRivLayerAttribute.Text = "Assign layer by attribute:";
            this.rbRivLayerAttribute.UseVisualStyleBackColor = true;
            this.rbRivLayerAttribute.CheckedChanged += new System.EventHandler(this.rbRivLayerAttribute_CheckedChanged);
            // 
            // rbRivSameLayer
            // 
            this.rbRivSameLayer.Location = new System.Drawing.Point(19, 27);
            this.rbRivSameLayer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbRivSameLayer.Name = "rbRivSameLayer";
            this.rbRivSameLayer.Size = new System.Drawing.Size(192, 31);
            this.rbRivSameLayer.TabIndex = 0;
            this.rbRivSameLayer.Text = "Assign all to layer:";
            this.rbRivSameLayer.UseVisualStyleBackColor = true;
            this.rbRivSameLayer.CheckedChanged += new System.EventHandler(this.rbRivSameLayer_CheckedChanged);
            // 
            // pgChd
            // 
            this.pgChd.Controls.Add(this.groupBoxChdLayerAssignment);
            this.pgChd.Location = new System.Drawing.Point(4, 31);
            this.pgChd.Name = "pgChd";
            this.pgChd.Padding = new System.Windows.Forms.Padding(3);
            this.pgChd.Size = new System.Drawing.Size(790, 487);
            this.pgChd.TabIndex = 5;
            this.pgChd.Tag = "ChdType";
            this.pgChd.Text = "CHD";
            this.pgChd.UseVisualStyleBackColor = true;
            this.pgChd.Layout += new System.Windows.Forms.LayoutEventHandler(this.pgChd_Layout);
            // 
            // groupBoxChdLayerAssignment
            // 
            this.groupBoxChdLayerAssignment.Controls.Add(this.cmbxChdLayerAttribute);
            this.groupBoxChdLayerAssignment.Controls.Add(this.udChdLayerPicker);
            this.groupBoxChdLayerAssignment.Controls.Add(this.rbChdLayerAttribute);
            this.groupBoxChdLayerAssignment.Controls.Add(this.rbChdSameLayer);
            this.groupBoxChdLayerAssignment.Location = new System.Drawing.Point(6, 7);
            this.groupBoxChdLayerAssignment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxChdLayerAssignment.Name = "groupBoxChdLayerAssignment";
            this.groupBoxChdLayerAssignment.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxChdLayerAssignment.Size = new System.Drawing.Size(400, 149);
            this.groupBoxChdLayerAssignment.TabIndex = 13;
            this.groupBoxChdLayerAssignment.TabStop = false;
            this.groupBoxChdLayerAssignment.Text = "Layer Assignment For CHD Cells";
            // 
            // cmbxChdLayerAttribute
            // 
            this.cmbxChdLayerAttribute.Enabled = false;
            this.cmbxChdLayerAttribute.FormattingEnabled = true;
            this.cmbxChdLayerAttribute.Location = new System.Drawing.Point(42, 101);
            this.cmbxChdLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxChdLayerAttribute.Name = "cmbxChdLayerAttribute";
            this.cmbxChdLayerAttribute.Size = new System.Drawing.Size(337, 30);
            this.cmbxChdLayerAttribute.Sorted = true;
            this.cmbxChdLayerAttribute.TabIndex = 4;
            this.cmbxChdLayerAttribute.TabStop = false;
            this.cmbxChdLayerAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxChdLayerAttribute_SelectedValueChanged);
            // 
            // udChdLayerPicker
            // 
            this.udChdLayerPicker.Location = new System.Drawing.Point(209, 29);
            this.udChdLayerPicker.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.udChdLayerPicker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udChdLayerPicker.Name = "udChdLayerPicker";
            this.udChdLayerPicker.Size = new System.Drawing.Size(58, 28);
            this.udChdLayerPicker.TabIndex = 2;
            this.udChdLayerPicker.TabStop = false;
            this.udChdLayerPicker.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udChdLayerPicker.ValueChanged += new System.EventHandler(this.udChdLayerPicker_ValueChanged);
            // 
            // rbChdLayerAttribute
            // 
            this.rbChdLayerAttribute.Location = new System.Drawing.Point(19, 64);
            this.rbChdLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbChdLayerAttribute.Name = "rbChdLayerAttribute";
            this.rbChdLayerAttribute.Size = new System.Drawing.Size(306, 32);
            this.rbChdLayerAttribute.TabIndex = 1;
            this.rbChdLayerAttribute.Text = "Assign layer by attribute:";
            this.rbChdLayerAttribute.UseVisualStyleBackColor = true;
            this.rbChdLayerAttribute.CheckedChanged += new System.EventHandler(this.rbChdLayerAttribute_CheckedChanged);
            // 
            // rbChdSameLayer
            // 
            this.rbChdSameLayer.Location = new System.Drawing.Point(19, 27);
            this.rbChdSameLayer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbChdSameLayer.Name = "rbChdSameLayer";
            this.rbChdSameLayer.Size = new System.Drawing.Size(184, 30);
            this.rbChdSameLayer.TabIndex = 0;
            this.rbChdSameLayer.Text = "Assign all to layer:";
            this.rbChdSameLayer.UseVisualStyleBackColor = true;
            this.rbChdSameLayer.CheckedChanged += new System.EventHandler(this.rbChdSameLayer_CheckedChanged);
            // 
            // pgRch
            // 
            this.pgRch.Location = new System.Drawing.Point(4, 31);
            this.pgRch.Name = "pgRch";
            this.pgRch.Size = new System.Drawing.Size(790, 487);
            this.pgRch.TabIndex = 6;
            this.pgRch.Tag = "RchType";
            this.pgRch.Text = "Recharge";
            this.pgRch.UseVisualStyleBackColor = true;
            this.pgRch.Layout += new System.Windows.Forms.LayoutEventHandler(this.pgRch_Layout);
            // 
            // pgGhb
            // 
            this.pgGhb.Controls.Add(this.groupBoxGhbLayerAssignment);
            this.pgGhb.Controls.Add(this.btnGhbSecondarySmp);
            this.pgGhb.Controls.Add(this.labelSecondarySmp);
            this.pgGhb.Controls.Add(this.textBoxGhbSecondarySmp);
            this.pgGhb.Controls.Add(this.groupBoxGhbLeakance);
            this.pgGhb.Location = new System.Drawing.Point(4, 31);
            this.pgGhb.Name = "pgGhb";
            this.pgGhb.Padding = new System.Windows.Forms.Padding(3);
            this.pgGhb.Size = new System.Drawing.Size(790, 487);
            this.pgGhb.TabIndex = 4;
            this.pgGhb.Tag = "GhbType";
            this.pgGhb.Text = "GHB";
            this.pgGhb.UseVisualStyleBackColor = true;
            this.pgGhb.Layout += new System.Windows.Forms.LayoutEventHandler(this.pgGhb_Layout);
            // 
            // groupBoxGhbLayerAssignment
            // 
            this.groupBoxGhbLayerAssignment.Controls.Add(this.cmbxGhbLayerAttribute);
            this.groupBoxGhbLayerAssignment.Controls.Add(this.udGhbLayerPicker);
            this.groupBoxGhbLayerAssignment.Controls.Add(this.rbGhbLayerAttribute);
            this.groupBoxGhbLayerAssignment.Controls.Add(this.rbGhbSameLayer);
            this.groupBoxGhbLayerAssignment.Location = new System.Drawing.Point(378, 67);
            this.groupBoxGhbLayerAssignment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxGhbLayerAssignment.Name = "groupBoxGhbLayerAssignment";
            this.groupBoxGhbLayerAssignment.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxGhbLayerAssignment.Size = new System.Drawing.Size(323, 150);
            this.groupBoxGhbLayerAssignment.TabIndex = 26;
            this.groupBoxGhbLayerAssignment.TabStop = false;
            this.groupBoxGhbLayerAssignment.Text = "Layer Assignment For GHB Cells";
            // 
            // cmbxGhbLayerAttribute
            // 
            this.cmbxGhbLayerAttribute.Enabled = false;
            this.cmbxGhbLayerAttribute.FormattingEnabled = true;
            this.cmbxGhbLayerAttribute.Location = new System.Drawing.Point(42, 104);
            this.cmbxGhbLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxGhbLayerAttribute.Name = "cmbxGhbLayerAttribute";
            this.cmbxGhbLayerAttribute.Size = new System.Drawing.Size(273, 30);
            this.cmbxGhbLayerAttribute.Sorted = true;
            this.cmbxGhbLayerAttribute.TabIndex = 4;
            this.cmbxGhbLayerAttribute.TabStop = false;
            this.cmbxGhbLayerAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxGhbLayerAttribute_SelectedValueChanged);
            // 
            // udGhbLayerPicker
            // 
            this.udGhbLayerPicker.Location = new System.Drawing.Point(209, 29);
            this.udGhbLayerPicker.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.udGhbLayerPicker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udGhbLayerPicker.Name = "udGhbLayerPicker";
            this.udGhbLayerPicker.Size = new System.Drawing.Size(58, 28);
            this.udGhbLayerPicker.TabIndex = 2;
            this.udGhbLayerPicker.TabStop = false;
            this.udGhbLayerPicker.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udGhbLayerPicker.ValueChanged += new System.EventHandler(this.udGhbLayerPicker_ValueChanged);
            // 
            // rbGhbLayerAttribute
            // 
            this.rbGhbLayerAttribute.Location = new System.Drawing.Point(19, 67);
            this.rbGhbLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbGhbLayerAttribute.Name = "rbGhbLayerAttribute";
            this.rbGhbLayerAttribute.Size = new System.Drawing.Size(319, 35);
            this.rbGhbLayerAttribute.TabIndex = 1;
            this.rbGhbLayerAttribute.Text = "Assign layer by attribute:";
            this.rbGhbLayerAttribute.UseVisualStyleBackColor = true;
            this.rbGhbLayerAttribute.CheckedChanged += new System.EventHandler(this.rbGhbLayerAttribute_CheckedChanged);
            // 
            // rbGhbSameLayer
            // 
            this.rbGhbSameLayer.Location = new System.Drawing.Point(19, 27);
            this.rbGhbSameLayer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbGhbSameLayer.Name = "rbGhbSameLayer";
            this.rbGhbSameLayer.Size = new System.Drawing.Size(184, 32);
            this.rbGhbSameLayer.TabIndex = 0;
            this.rbGhbSameLayer.Text = "Assign all to layer:";
            this.rbGhbSameLayer.UseVisualStyleBackColor = true;
            this.rbGhbSameLayer.CheckedChanged += new System.EventHandler(this.rbGhbSameLayer_CheckedChanged);
            // 
            // btnGhbSecondarySmp
            // 
            this.btnGhbSecondarySmp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGhbSecondarySmp.Image = ((System.Drawing.Image)(resources.GetObject("btnGhbSecondarySmp.Image")));
            this.btnGhbSecondarySmp.Location = new System.Drawing.Point(701, 34);
            this.btnGhbSecondarySmp.Name = "btnGhbSecondarySmp";
            this.btnGhbSecondarySmp.Size = new System.Drawing.Size(30, 30);
            this.btnGhbSecondarySmp.TabIndex = 25;
            this.btnGhbSecondarySmp.UseVisualStyleBackColor = true;
            this.btnGhbSecondarySmp.Click += new System.EventHandler(this.btnGhbSecondarySmp_Click);
            // 
            // labelSecondarySmp
            // 
            this.labelSecondarySmp.AutoSize = true;
            this.labelSecondarySmp.Location = new System.Drawing.Point(14, 8);
            this.labelSecondarySmp.Name = "labelSecondarySmp";
            this.labelSecondarySmp.Size = new System.Drawing.Size(557, 24);
            this.labelSecondarySmp.TabIndex = 24;
            this.labelSecondarySmp.Text = "Secondary time-series (SMP) file (to control active status; optional)";
            // 
            // textBoxGhbSecondarySmp
            // 
            this.textBoxGhbSecondarySmp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxGhbSecondarySmp.Location = new System.Drawing.Point(17, 35);
            this.textBoxGhbSecondarySmp.Name = "textBoxGhbSecondarySmp";
            this.textBoxGhbSecondarySmp.Size = new System.Drawing.Size(684, 28);
            this.textBoxGhbSecondarySmp.TabIndex = 23;
            this.textBoxGhbSecondarySmp.TextChanged += new System.EventHandler(this.textBoxGhbSecondarySmp_TextChanged);
            // 
            // groupBoxGhbLeakance
            // 
            this.groupBoxGhbLeakance.Controls.Add(this.textBoxGhbLeakanceUniformValue);
            this.groupBoxGhbLeakance.Controls.Add(this.rbGhbLeakanceFromAttribute);
            this.groupBoxGhbLeakance.Controls.Add(this.rbGhbLeakanceUniform);
            this.groupBoxGhbLeakance.Controls.Add(this.cmbxGhbLeakanceAttribute);
            this.groupBoxGhbLeakance.Location = new System.Drawing.Point(18, 67);
            this.groupBoxGhbLeakance.Name = "groupBoxGhbLeakance";
            this.groupBoxGhbLeakance.Size = new System.Drawing.Size(343, 150);
            this.groupBoxGhbLeakance.TabIndex = 22;
            this.groupBoxGhbLeakance.TabStop = false;
            this.groupBoxGhbLeakance.Text = "Leakance";
            // 
            // textBoxGhbLeakanceUniformValue
            // 
            this.textBoxGhbLeakanceUniformValue.Location = new System.Drawing.Point(183, 27);
            this.textBoxGhbLeakanceUniformValue.Name = "textBoxGhbLeakanceUniformValue";
            this.textBoxGhbLeakanceUniformValue.Size = new System.Drawing.Size(152, 28);
            this.textBoxGhbLeakanceUniformValue.TabIndex = 22;
            this.textBoxGhbLeakanceUniformValue.TextChanged += new System.EventHandler(this.textBoxGhbLeakanceUniformValue_TextChanged);
            // 
            // rbGhbLeakanceFromAttribute
            // 
            this.rbGhbLeakanceFromAttribute.Location = new System.Drawing.Point(26, 68);
            this.rbGhbLeakanceFromAttribute.Name = "rbGhbLeakanceFromAttribute";
            this.rbGhbLeakanceFromAttribute.Size = new System.Drawing.Size(285, 34);
            this.rbGhbLeakanceFromAttribute.TabIndex = 21;
            this.rbGhbLeakanceFromAttribute.TabStop = true;
            this.rbGhbLeakanceFromAttribute.Text = "Value from attribute:";
            this.rbGhbLeakanceFromAttribute.UseVisualStyleBackColor = true;
            this.rbGhbLeakanceFromAttribute.CheckedChanged += new System.EventHandler(this.rbGhbLeakanceFromAttribute_CheckedChanged);
            // 
            // rbGhbLeakanceUniform
            // 
            this.rbGhbLeakanceUniform.Location = new System.Drawing.Point(26, 27);
            this.rbGhbLeakanceUniform.Name = "rbGhbLeakanceUniform";
            this.rbGhbLeakanceUniform.Size = new System.Drawing.Size(151, 28);
            this.rbGhbLeakanceUniform.TabIndex = 20;
            this.rbGhbLeakanceUniform.TabStop = true;
            this.rbGhbLeakanceUniform.Text = "Uniform value:";
            this.rbGhbLeakanceUniform.UseVisualStyleBackColor = true;
            this.rbGhbLeakanceUniform.CheckedChanged += new System.EventHandler(this.rbGhbLeakanceUniform_CheckedChanged);
            // 
            // cmbxGhbLeakanceAttribute
            // 
            this.cmbxGhbLeakanceAttribute.FormattingEnabled = true;
            this.cmbxGhbLeakanceAttribute.Location = new System.Drawing.Point(58, 104);
            this.cmbxGhbLeakanceAttribute.Name = "cmbxGhbLeakanceAttribute";
            this.cmbxGhbLeakanceAttribute.Size = new System.Drawing.Size(277, 30);
            this.cmbxGhbLeakanceAttribute.TabIndex = 18;
            this.cmbxGhbLeakanceAttribute.Text = "cmbxGhbLeakanceAttribute";
            this.cmbxGhbLeakanceAttribute.SelectedValueChanged += new System.EventHandler(this.cmbxGhbLeakanceAttribute_SelectedValueChanged);
            // 
            // pgDisplay
            // 
            this.pgDisplay.Controls.Add(this.splitContainer1);
            this.pgDisplay.Location = new System.Drawing.Point(4, 31);
            this.pgDisplay.Margin = new System.Windows.Forms.Padding(0);
            this.pgDisplay.Name = "pgDisplay";
            this.pgDisplay.Size = new System.Drawing.Size(790, 487);
            this.pgDisplay.TabIndex = 2;
            this.pgDisplay.Text = "Display";
            this.pgDisplay.UseVisualStyleBackColor = true;
            this.pgDisplay.Layout += new System.Windows.Forms.LayoutEventHandler(this.pgDisplay_Layout);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(790, 487);
            this.splitContainer1.SplitterDistance = 512;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.toolStripDisplay);
            this.splitContainer2.Panel1.Controls.Add(this.cbxShowBackground);
            this.splitContainer2.Panel1.Controls.Add(this.panelExplanation);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            this.splitContainer2.Panel1MinSize = 40;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelIndexMap);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Panel2MinSize = 40;
            this.splitContainer2.Size = new System.Drawing.Size(275, 487);
            this.splitContainer2.SplitterDistance = 297;
            this.splitContainer2.TabIndex = 0;
            // 
            // toolStripDisplay
            // 
            this.toolStripDisplay.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSelect,
            this.toolStripLabelPan,
            this.toolStripButtonReCenter,
            this.toolStripLabel1,
            this.toolStripButtonZoomIn,
            this.toolStripButtonZoomOut,
            this.toolStripButtonZoomToGrid,
            this.toolStripButtonFullExtent});
            this.toolStripDisplay.Location = new System.Drawing.Point(0, 0);
            this.toolStripDisplay.Name = "toolStripDisplay";
            this.toolStripDisplay.Size = new System.Drawing.Size(273, 25);
            this.toolStripDisplay.TabIndex = 3;
            this.toolStripDisplay.Text = "toolStrip1";
            // 
            // toolStripButtonSelect
            // 
            this.toolStripButtonSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSelect.Image")));
            this.toolStripButtonSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelect.Name = "toolStripButtonSelect";
            this.toolStripButtonSelect.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSelect.Text = "toolStripButton1";
            this.toolStripButtonSelect.ToolTipText = "Pointer";
            this.toolStripButtonSelect.Click += new System.EventHandler(this.toolStripButtonSelect_Click);
            // 
            // toolStripLabelPan
            // 
            this.toolStripLabelPan.Name = "toolStripLabelPan";
            this.toolStripLabelPan.Size = new System.Drawing.Size(36, 22);
            this.toolStripLabelPan.Text = "Pan:";
            // 
            // toolStripButtonReCenter
            // 
            this.toolStripButtonReCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReCenter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReCenter.Image")));
            this.toolStripButtonReCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReCenter.Name = "toolStripButtonReCenter";
            this.toolStripButtonReCenter.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonReCenter.Text = "toolStripButton1";
            this.toolStripButtonReCenter.ToolTipText = "Re-center map";
            this.toolStripButtonReCenter.Click += new System.EventHandler(this.toolStripButtonReCenter_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(52, 22);
            this.toolStripLabel1.Text = "Zoom:";
            // 
            // toolStripButtonZoomIn
            // 
            this.toolStripButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomIn.Image")));
            this.toolStripButtonZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomIn.Name = "toolStripButtonZoomIn";
            this.toolStripButtonZoomIn.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoomIn.Text = "toolStripButton1";
            this.toolStripButtonZoomIn.ToolTipText = "Zoom in";
            this.toolStripButtonZoomIn.Click += new System.EventHandler(this.toolStripButtonZoomIn_Click);
            // 
            // toolStripButtonZoomOut
            // 
            this.toolStripButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomOut.Image")));
            this.toolStripButtonZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomOut.Name = "toolStripButtonZoomOut";
            this.toolStripButtonZoomOut.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoomOut.Text = "toolStripButton1";
            this.toolStripButtonZoomOut.ToolTipText = "Zoom out";
            this.toolStripButtonZoomOut.Click += new System.EventHandler(this.toolStripButtonZoomOut_Click);
            // 
            // toolStripButtonZoomToGrid
            // 
            this.toolStripButtonZoomToGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomToGrid.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomToGrid.Image")));
            this.toolStripButtonZoomToGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomToGrid.Name = "toolStripButtonZoomToGrid";
            this.toolStripButtonZoomToGrid.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoomToGrid.Text = "toolStripButton1";
            this.toolStripButtonZoomToGrid.ToolTipText = "Zoom to grid";
            this.toolStripButtonZoomToGrid.Click += new System.EventHandler(this.toolStripButtonZoomToGrid_Click);
            // 
            // toolStripButtonFullExtent
            // 
            this.toolStripButtonFullExtent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFullExtent.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonFullExtent.Image")));
            this.toolStripButtonFullExtent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFullExtent.Name = "toolStripButtonFullExtent";
            this.toolStripButtonFullExtent.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFullExtent.Text = "toolStripButton1";
            this.toolStripButtonFullExtent.ToolTipText = "Zoom to full extent";
            this.toolStripButtonFullExtent.Click += new System.EventHandler(this.toolStripButtonFullExtent_Click);
            // 
            // cbxShowBackground
            // 
            this.cbxShowBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbxShowBackground.AutoSize = true;
            this.cbxShowBackground.Checked = true;
            this.cbxShowBackground.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxShowBackground.Location = new System.Drawing.Point(4, 267);
            this.cbxShowBackground.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbxShowBackground.Name = "cbxShowBackground";
            this.cbxShowBackground.Size = new System.Drawing.Size(191, 28);
            this.cbxShowBackground.TabIndex = 2;
            this.cbxShowBackground.Text = "Background Image";
            this.cbxShowBackground.UseVisualStyleBackColor = true;
            this.cbxShowBackground.Click += new System.EventHandler(this.cbxShowBackground_Click);
            // 
            // panelExplanation
            // 
            this.panelExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelExplanation.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelExplanation.Location = new System.Drawing.Point(-1, 58);
            this.panelExplanation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelExplanation.Name = "panelExplanation";
            this.panelExplanation.Size = new System.Drawing.Size(279, 209);
            this.panelExplanation.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 24);
            this.label4.TabIndex = 0;
            this.label4.Text = "Explanation";
            // 
            // panelIndexMap
            // 
            this.panelIndexMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelIndexMap.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelIndexMap.Location = new System.Drawing.Point(-1, 26);
            this.panelIndexMap.Margin = new System.Windows.Forms.Padding(0);
            this.panelIndexMap.Name = "panelIndexMap";
            this.panelIndexMap.Size = new System.Drawing.Size(279, 164);
            this.panelIndexMap.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 24);
            this.label5.TabIndex = 0;
            this.label5.Text = "Index Map";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "search4files.ico");
            this.imageList1.Images.SetKeyName(1, "WellPump.ico");
            this.imageList1.Images.SetKeyName(2, "RivIcon.ico");
            this.imageList1.Images.SetKeyName(3, "ChdIcon.ico");
            this.imageList1.Images.SetKeyName(4, "RchIcon.ico");
            this.imageList1.Images.SetKeyName(5, "search.ico");
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(3, 527);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 31);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(120, 527);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 31);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelXyCoords,
            this.toolStripStatusLabelRowCol});
            this.statusStrip1.Location = new System.Drawing.Point(239, 527);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(562, 31);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelXyCoords
            // 
            this.toolStripStatusLabelXyCoords.AutoSize = false;
            this.toolStripStatusLabelXyCoords.Name = "toolStripStatusLabelXyCoords";
            this.toolStripStatusLabelXyCoords.Size = new System.Drawing.Size(250, 26);
            // 
            // toolStripStatusLabelRowCol
            // 
            this.toolStripStatusLabelRowCol.AutoSize = false;
            this.toolStripStatusLabelRowCol.Name = "toolStripStatusLabelRowCol";
            this.toolStripStatusLabelRowCol.Size = new System.Drawing.Size(200, 26);
            // 
            // FeatureSetForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(806, 573);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(8998, 4495);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FeatureSetForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Any Package Feature Set";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FeatureSetForm_FormClosing);
            this.Load += new System.EventHandler(this.FeatureSetForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.pgGeneral.ResumeLayout(false);
            this.pgGeneral.PerformLayout();
            this.grpboxSmpInterpretationMethod.ResumeLayout(false);
            this.pgWells.ResumeLayout(false);
            this.groupBoxWellsLayerAssignment.ResumeLayout(false);
            this.groupBoxWellsLayerAssignment.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWellsLayerPicker)).EndInit();
            this.pgRiv.ResumeLayout(false);
            this.pgRiv.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxRivLayerAssignment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udRivLayerPicker)).EndInit();
            this.pgChd.ResumeLayout(false);
            this.groupBoxChdLayerAssignment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udChdLayerPicker)).EndInit();
            this.pgGhb.ResumeLayout(false);
            this.pgGhb.PerformLayout();
            this.groupBoxGhbLayerAssignment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udGhbLayerPicker)).EndInit();
            this.groupBoxGhbLeakance.ResumeLayout(false);
            this.groupBoxGhbLeakance.PerformLayout();
            this.pgDisplay.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStripDisplay.ResumeLayout(false);
            this.toolStripDisplay.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pgGeneral;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbShapefile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFindSmpFile;
        private System.Windows.Forms.Button btnFindShapefile;
        private System.Windows.Forms.TextBox tbSmp;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabPage pgDisplay;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panelExplanation;
        private System.Windows.Forms.Panel panelIndexMap;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbxKeyField;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cbxShowBackground;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbxLabelFeatures;
        private System.Windows.Forms.GroupBox grpboxSmpInterpretationMethod;
        private System.Windows.Forms.RadioButton rbStepwise;
        private System.Windows.Forms.RadioButton rbPiecewise;
        private System.Windows.Forms.TabPage pgWells;
        private System.Windows.Forms.TabPage pgGhb;
        private System.Windows.Forms.Button btnGhbSecondarySmp;
        private System.Windows.Forms.Label labelSecondarySmp;
        private System.Windows.Forms.TextBox textBoxGhbSecondarySmp;
        private System.Windows.Forms.GroupBox groupBoxGhbLeakance;
        private System.Windows.Forms.TextBox textBoxGhbLeakanceUniformValue;
        private System.Windows.Forms.RadioButton rbGhbLeakanceFromAttribute;
        private System.Windows.Forms.RadioButton rbGhbLeakanceUniform;
        private System.Windows.Forms.ComboBox cmbxGhbLeakanceAttribute;
        private System.Windows.Forms.GroupBox groupBoxGhbLayerAssignment;
        private System.Windows.Forms.ComboBox cmbxGhbLayerAttribute;
        private System.Windows.Forms.NumericUpDown udGhbLayerPicker;
        private System.Windows.Forms.RadioButton rbGhbLayerAttribute;
        private System.Windows.Forms.RadioButton rbGhbSameLayer;
        private System.Windows.Forms.GroupBox groupBoxWellsLayerAssignment;
        private System.Windows.Forms.Label labelWells8;
        private System.Windows.Forms.Label labelWells7;
        private System.Windows.Forms.ComboBox cmbxWellsBottomElevationAttribute;
        private System.Windows.Forms.ComboBox cmbxWellsTopElevationAttribute;
        private System.Windows.Forms.ComboBox cmbxWellsLayerAttribute;
        private System.Windows.Forms.RadioButton rbWellsByTops;
        private System.Windows.Forms.NumericUpDown udWellsLayerPicker;
        private System.Windows.Forms.RadioButton rbWellsLayerAttribute;
        private System.Windows.Forms.RadioButton rbWellsSameLayer;
        private System.Windows.Forms.TabPage pgChd;
        private System.Windows.Forms.TabPage pgRch;
        private System.Windows.Forms.TabPage pgRiv;
        private System.Windows.Forms.GroupBox groupBoxChdLayerAssignment;
        private System.Windows.Forms.ComboBox cmbxChdLayerAttribute;
        private System.Windows.Forms.NumericUpDown udChdLayerPicker;
        private System.Windows.Forms.RadioButton rbChdLayerAttribute;
        private System.Windows.Forms.RadioButton rbChdSameLayer;
        private System.Windows.Forms.GroupBox groupBoxRivLayerAssignment;
        private System.Windows.Forms.ComboBox cmbxRivLayerAttribute;
        private System.Windows.Forms.NumericUpDown udRivLayerPicker;
        private System.Windows.Forms.RadioButton rbRivLayerAttribute;
        private System.Windows.Forms.RadioButton rbRivSameLayer;
        private System.Windows.Forms.Button btnRivSecondarySmp;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxRivSecondarySmp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxRivHydCondUniformValue;
        private System.Windows.Forms.RadioButton rbRivHydCondFromAttribute;
        private System.Windows.Forms.RadioButton rbRivHydCondUniform;
        private System.Windows.Forms.ComboBox cmbxRivHydCondAttribute;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxRivThicknessUniformValue;
        private System.Windows.Forms.RadioButton rbRivThicknessFromAttribute;
        private System.Windows.Forms.RadioButton rbRivThicknessUniform;
        private System.Windows.Forms.ComboBox cmbxRivThicknessAttribute;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxRivWidthUniformValue;
        private System.Windows.Forms.RadioButton rbRivWidthFromAttribute;
        private System.Windows.Forms.RadioButton rbRivWidthUniform;
        private System.Windows.Forms.ComboBox cmbxRivWidthAttribute;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cmbxRivBottomElevAttribute;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ToolStrip toolStripDisplay;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelect;
        private System.Windows.Forms.ToolStripButton toolStripButtonReCenter;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomIn;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomOut;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomToGrid;
        private System.Windows.Forms.ToolStripButton toolStripButtonFullExtent;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelXyCoords;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelRowCol;
        private System.Windows.Forms.ToolStripLabel toolStripLabelPan;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    }
}