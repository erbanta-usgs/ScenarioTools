namespace ScenarioTools.Dialogs
{
    partial class DataSeriesMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSeriesMenu));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBoxPointColors = new System.Windows.Forms.GroupBox();
            this.radioButtonPointNone = new System.Windows.Forms.RadioButton();
            this.radioButtonPointBlue = new System.Windows.Forms.RadioButton();
            this.radioButtonPointGreen = new System.Windows.Forms.RadioButton();
            this.radioButtonPointOrange = new System.Windows.Forms.RadioButton();
            this.radioButtonPointRed = new System.Windows.Forms.RadioButton();
            this.radioButtonPointBlack = new System.Windows.Forms.RadioButton();
            this.groupBoxColorRamps = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxColorRamp = new System.Windows.Forms.PictureBox();
            this.btnColor1 = new System.Windows.Forms.Button();
            this.btnColor0 = new System.Windows.Forms.Button();
            this.groupBoxDataProvider = new System.Windows.Forms.GroupBox();
            this.comboBoxDataProviders = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxLineColors = new System.Windows.Forms.GroupBox();
            this.radioButtonLineNone = new System.Windows.Forms.RadioButton();
            this.radioButtonLineBlue = new System.Windows.Forms.RadioButton();
            this.radioButtonLineGreen = new System.Windows.Forms.RadioButton();
            this.radioButtonLineOrange = new System.Windows.Forms.RadioButton();
            this.radioButtonLineRed = new System.Windows.Forms.RadioButton();
            this.radioButtonLineBlack = new System.Windows.Forms.RadioButton();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxConvertFlowToFlux = new System.Windows.Forms.CheckBox();
            this.checkBoxVisible = new System.Windows.Forms.CheckBox();
            this.groupBoxPointColors.SuspendLayout();
            this.groupBoxColorRamps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorRamp)).BeginInit();
            this.groupBoxLineColors.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(131, 546);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 31);
            this.buttonCancel.TabIndex = 16;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(11, 546);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(112, 31);
            this.buttonOk.TabIndex = 15;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // groupBoxPointColors
            // 
            this.groupBoxPointColors.Controls.Add(this.radioButtonPointNone);
            this.groupBoxPointColors.Controls.Add(this.radioButtonPointBlue);
            this.groupBoxPointColors.Controls.Add(this.radioButtonPointGreen);
            this.groupBoxPointColors.Controls.Add(this.radioButtonPointOrange);
            this.groupBoxPointColors.Controls.Add(this.radioButtonPointRed);
            this.groupBoxPointColors.Controls.Add(this.radioButtonPointBlack);
            this.groupBoxPointColors.Location = new System.Drawing.Point(8, 247);
            this.groupBoxPointColors.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxPointColors.Name = "groupBoxPointColors";
            this.groupBoxPointColors.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxPointColors.Size = new System.Drawing.Size(380, 94);
            this.groupBoxPointColors.TabIndex = 14;
            this.groupBoxPointColors.TabStop = false;
            this.groupBoxPointColors.Text = "Point Series Color";
            // 
            // radioButtonPointNone
            // 
            this.radioButtonPointNone.Location = new System.Drawing.Point(190, 51);
            this.radioButtonPointNone.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonPointNone.Name = "radioButtonPointNone";
            this.radioButtonPointNone.Size = new System.Drawing.Size(182, 28);
            this.radioButtonPointNone.TabIndex = 13;
            this.radioButtonPointNone.TabStop = true;
            this.radioButtonPointNone.Text = "No Point Series";
            this.radioButtonPointNone.UseVisualStyleBackColor = true;
            // 
            // radioButtonPointBlue
            // 
            this.radioButtonPointBlue.AutoSize = true;
            this.radioButtonPointBlue.Image = global::ScenarioTools.Properties.Resources.blue;
            this.radioButtonPointBlue.Location = new System.Drawing.Point(190, 26);
            this.radioButtonPointBlue.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonPointBlue.Name = "radioButtonPointBlue";
            this.radioButtonPointBlue.Size = new System.Drawing.Size(40, 23);
            this.radioButtonPointBlue.TabIndex = 10;
            this.radioButtonPointBlue.TabStop = true;
            this.radioButtonPointBlue.UseVisualStyleBackColor = true;
            // 
            // radioButtonPointGreen
            // 
            this.radioButtonPointGreen.AutoSize = true;
            this.radioButtonPointGreen.Image = global::ScenarioTools.Properties.Resources.green;
            this.radioButtonPointGreen.Location = new System.Drawing.Point(100, 53);
            this.radioButtonPointGreen.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonPointGreen.Name = "radioButtonPointGreen";
            this.radioButtonPointGreen.Size = new System.Drawing.Size(40, 23);
            this.radioButtonPointGreen.TabIndex = 12;
            this.radioButtonPointGreen.TabStop = true;
            this.radioButtonPointGreen.UseVisualStyleBackColor = true;
            // 
            // radioButtonPointOrange
            // 
            this.radioButtonPointOrange.AutoSize = true;
            this.radioButtonPointOrange.Image = global::ScenarioTools.Properties.Resources.orange;
            this.radioButtonPointOrange.Location = new System.Drawing.Point(100, 26);
            this.radioButtonPointOrange.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonPointOrange.Name = "radioButtonPointOrange";
            this.radioButtonPointOrange.Size = new System.Drawing.Size(40, 23);
            this.radioButtonPointOrange.TabIndex = 9;
            this.radioButtonPointOrange.TabStop = true;
            this.radioButtonPointOrange.UseVisualStyleBackColor = true;
            // 
            // radioButtonPointRed
            // 
            this.radioButtonPointRed.AutoSize = true;
            this.radioButtonPointRed.Image = global::ScenarioTools.Properties.Resources.red;
            this.radioButtonPointRed.Location = new System.Drawing.Point(10, 53);
            this.radioButtonPointRed.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonPointRed.Name = "radioButtonPointRed";
            this.radioButtonPointRed.Size = new System.Drawing.Size(40, 23);
            this.radioButtonPointRed.TabIndex = 11;
            this.radioButtonPointRed.TabStop = true;
            this.radioButtonPointRed.UseVisualStyleBackColor = true;
            // 
            // radioButtonPointBlack
            // 
            this.radioButtonPointBlack.AutoSize = true;
            this.radioButtonPointBlack.Image = global::ScenarioTools.Properties.Resources.black;
            this.radioButtonPointBlack.Location = new System.Drawing.Point(10, 26);
            this.radioButtonPointBlack.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonPointBlack.Name = "radioButtonPointBlack";
            this.radioButtonPointBlack.Size = new System.Drawing.Size(40, 23);
            this.radioButtonPointBlack.TabIndex = 8;
            this.radioButtonPointBlack.TabStop = true;
            this.radioButtonPointBlack.UseVisualStyleBackColor = true;
            // 
            // groupBoxColorRamps
            // 
            this.groupBoxColorRamps.Controls.Add(this.label3);
            this.groupBoxColorRamps.Controls.Add(this.pictureBoxColorRamp);
            this.groupBoxColorRamps.Controls.Add(this.btnColor1);
            this.groupBoxColorRamps.Controls.Add(this.btnColor0);
            this.groupBoxColorRamps.Location = new System.Drawing.Point(8, 349);
            this.groupBoxColorRamps.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxColorRamps.Name = "groupBoxColorRamps";
            this.groupBoxColorRamps.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxColorRamps.Size = new System.Drawing.Size(380, 91);
            this.groupBoxColorRamps.TabIndex = 14;
            this.groupBoxColorRamps.TabStop = false;
            this.groupBoxColorRamps.Text = "Color Ramp";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 24);
            this.label3.TabIndex = 19;
            this.label3.Text = "Select end-member colors";
            // 
            // pictureBoxColorRamp
            // 
            this.pictureBoxColorRamp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxColorRamp.Location = new System.Drawing.Point(37, 55);
            this.pictureBoxColorRamp.Name = "pictureBoxColorRamp";
            this.pictureBoxColorRamp.Size = new System.Drawing.Size(242, 23);
            this.pictureBoxColorRamp.TabIndex = 18;
            this.pictureBoxColorRamp.TabStop = false;
            // 
            // btnColor1
            // 
            this.btnColor1.Location = new System.Drawing.Point(280, 54);
            this.btnColor1.Name = "btnColor1";
            this.btnColor1.Size = new System.Drawing.Size(25, 25);
            this.btnColor1.TabIndex = 16;
            this.btnColor1.UseVisualStyleBackColor = true;
            this.btnColor1.Click += new System.EventHandler(this.btnColor1_Click);
            // 
            // btnColor0
            // 
            this.btnColor0.Location = new System.Drawing.Point(11, 54);
            this.btnColor0.Name = "btnColor0";
            this.btnColor0.Size = new System.Drawing.Size(25, 25);
            this.btnColor0.TabIndex = 15;
            this.btnColor0.UseVisualStyleBackColor = true;
            this.btnColor0.Click += new System.EventHandler(this.btnColor0_Click);
            // 
            // groupBoxDataProvider
            // 
            this.groupBoxDataProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDataProvider.AutoSize = true;
            this.groupBoxDataProvider.Location = new System.Drawing.Point(442, 4);
            this.groupBoxDataProvider.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxDataProvider.Name = "groupBoxDataProvider";
            this.groupBoxDataProvider.Size = new System.Drawing.Size(446, 449);
            this.groupBoxDataProvider.TabIndex = 17;
            this.groupBoxDataProvider.TabStop = false;
            this.groupBoxDataProvider.Text = "Data Set";
            // 
            // comboBoxDataProviders
            // 
            this.comboBoxDataProviders.FormattingEnabled = true;
            this.comboBoxDataProviders.Location = new System.Drawing.Point(8, 100);
            this.comboBoxDataProviders.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxDataProviders.Name = "comboBoxDataProviders";
            this.comboBoxDataProviders.Size = new System.Drawing.Size(429, 30);
            this.comboBoxDataProviders.TabIndex = 1;
            this.comboBoxDataProviders.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataProviders_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 72);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 24);
            this.label2.TabIndex = 15;
            this.label2.Text = "Data Category";
            // 
            // groupBoxLineColors
            // 
            this.groupBoxLineColors.Controls.Add(this.radioButtonLineNone);
            this.groupBoxLineColors.Controls.Add(this.radioButtonLineBlue);
            this.groupBoxLineColors.Controls.Add(this.radioButtonLineGreen);
            this.groupBoxLineColors.Controls.Add(this.radioButtonLineOrange);
            this.groupBoxLineColors.Controls.Add(this.radioButtonLineRed);
            this.groupBoxLineColors.Controls.Add(this.radioButtonLineBlack);
            this.groupBoxLineColors.Location = new System.Drawing.Point(8, 145);
            this.groupBoxLineColors.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxLineColors.Name = "groupBoxLineColors";
            this.groupBoxLineColors.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxLineColors.Size = new System.Drawing.Size(380, 94);
            this.groupBoxLineColors.TabIndex = 13;
            this.groupBoxLineColors.TabStop = false;
            this.groupBoxLineColors.Text = "Line Series Color";
            // 
            // radioButtonLineNone
            // 
            this.radioButtonLineNone.Location = new System.Drawing.Point(190, 51);
            this.radioButtonLineNone.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonLineNone.Name = "radioButtonLineNone";
            this.radioButtonLineNone.Size = new System.Drawing.Size(182, 28);
            this.radioButtonLineNone.TabIndex = 7;
            this.radioButtonLineNone.TabStop = true;
            this.radioButtonLineNone.Text = "No Line Series";
            this.radioButtonLineNone.UseVisualStyleBackColor = true;
            // 
            // radioButtonLineBlue
            // 
            this.radioButtonLineBlue.AutoSize = true;
            this.radioButtonLineBlue.Image = global::ScenarioTools.Properties.Resources.blue;
            this.radioButtonLineBlue.Location = new System.Drawing.Point(190, 26);
            this.radioButtonLineBlue.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonLineBlue.Name = "radioButtonLineBlue";
            this.radioButtonLineBlue.Size = new System.Drawing.Size(40, 23);
            this.radioButtonLineBlue.TabIndex = 4;
            this.radioButtonLineBlue.TabStop = true;
            this.radioButtonLineBlue.UseVisualStyleBackColor = true;
            // 
            // radioButtonLineGreen
            // 
            this.radioButtonLineGreen.AutoSize = true;
            this.radioButtonLineGreen.Image = global::ScenarioTools.Properties.Resources.green;
            this.radioButtonLineGreen.Location = new System.Drawing.Point(100, 53);
            this.radioButtonLineGreen.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonLineGreen.Name = "radioButtonLineGreen";
            this.radioButtonLineGreen.Size = new System.Drawing.Size(40, 23);
            this.radioButtonLineGreen.TabIndex = 6;
            this.radioButtonLineGreen.TabStop = true;
            this.radioButtonLineGreen.UseVisualStyleBackColor = true;
            // 
            // radioButtonLineOrange
            // 
            this.radioButtonLineOrange.AutoSize = true;
            this.radioButtonLineOrange.Image = global::ScenarioTools.Properties.Resources.orange;
            this.radioButtonLineOrange.Location = new System.Drawing.Point(100, 26);
            this.radioButtonLineOrange.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonLineOrange.Name = "radioButtonLineOrange";
            this.radioButtonLineOrange.Size = new System.Drawing.Size(40, 23);
            this.radioButtonLineOrange.TabIndex = 3;
            this.radioButtonLineOrange.TabStop = true;
            this.radioButtonLineOrange.UseVisualStyleBackColor = true;
            // 
            // radioButtonLineRed
            // 
            this.radioButtonLineRed.AutoSize = true;
            this.radioButtonLineRed.Image = global::ScenarioTools.Properties.Resources.red;
            this.radioButtonLineRed.Location = new System.Drawing.Point(10, 53);
            this.radioButtonLineRed.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonLineRed.Name = "radioButtonLineRed";
            this.radioButtonLineRed.Size = new System.Drawing.Size(40, 23);
            this.radioButtonLineRed.TabIndex = 5;
            this.radioButtonLineRed.TabStop = true;
            this.radioButtonLineRed.UseVisualStyleBackColor = true;
            // 
            // radioButtonLineBlack
            // 
            this.radioButtonLineBlack.AutoSize = true;
            this.radioButtonLineBlack.Image = global::ScenarioTools.Properties.Resources.black;
            this.radioButtonLineBlack.Location = new System.Drawing.Point(10, 26);
            this.radioButtonLineBlack.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonLineBlack.Name = "radioButtonLineBlack";
            this.radioButtonLineBlack.Size = new System.Drawing.Size(40, 23);
            this.radioButtonLineBlack.TabIndex = 2;
            this.radioButtonLineBlack.TabStop = true;
            this.radioButtonLineBlack.UseVisualStyleBackColor = true;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(8, 36);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(429, 28);
            this.textBoxName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.checkBoxConvertFlowToFlux);
            this.panel1.Controls.Add(this.checkBoxVisible);
            this.panel1.Controls.Add(this.groupBoxPointColors);
            this.panel1.Controls.Add(this.groupBoxDataProvider);
            this.panel1.Controls.Add(this.groupBoxColorRamps);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxName);
            this.panel1.Controls.Add(this.comboBoxDataProviders);
            this.panel1.Controls.Add(this.groupBoxLineColors);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(16, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 525);
            this.panel1.TabIndex = 17;
            this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            // 
            // checkBoxConvertFlowToFlux
            // 
            this.checkBoxConvertFlowToFlux.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxConvertFlowToFlux.AutoSize = true;
            this.checkBoxConvertFlowToFlux.Location = new System.Drawing.Point(141, 493);
            this.checkBoxConvertFlowToFlux.Name = "checkBoxConvertFlowToFlux";
            this.checkBoxConvertFlowToFlux.Size = new System.Drawing.Size(518, 28);
            this.checkBoxConvertFlowToFlux.TabIndex = 19;
            this.checkBoxConvertFlowToFlux.Text = "For display, convert flow (L**3/T) values to flux (L/T) values";
            this.checkBoxConvertFlowToFlux.UseVisualStyleBackColor = true;
            // 
            // checkBoxVisible
            // 
            this.checkBoxVisible.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxVisible.AutoSize = true;
            this.checkBoxVisible.Location = new System.Drawing.Point(8, 493);
            this.checkBoxVisible.Name = "checkBoxVisible";
            this.checkBoxVisible.Size = new System.Drawing.Size(88, 28);
            this.checkBoxVisible.TabIndex = 18;
            this.checkBoxVisible.Text = "Visible";
            this.checkBoxVisible.UseVisualStyleBackColor = true;
            // 
            // DataSeriesMenu
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(936, 598);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(892, 630);
            this.Name = "DataSeriesMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Series";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataSeriesMenu_FormClosing);
            this.Load += new System.EventHandler(this.DataSeriesMenu_Load);
            this.groupBoxPointColors.ResumeLayout(false);
            this.groupBoxPointColors.PerformLayout();
            this.groupBoxColorRamps.ResumeLayout(false);
            this.groupBoxColorRamps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorRamp)).EndInit();
            this.groupBoxLineColors.ResumeLayout(false);
            this.groupBoxLineColors.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBoxLineColors;
        private System.Windows.Forms.RadioButton radioButtonLineBlue;
        private System.Windows.Forms.RadioButton radioButtonLineGreen;
        private System.Windows.Forms.RadioButton radioButtonLineOrange;
        private System.Windows.Forms.RadioButton radioButtonLineRed;
        private System.Windows.Forms.RadioButton radioButtonLineBlack;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDataProviders;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBoxDataProvider;
        private System.Windows.Forms.GroupBox groupBoxColorRamps;
        private System.Windows.Forms.GroupBox groupBoxPointColors;
        private System.Windows.Forms.RadioButton radioButtonPointNone;
        private System.Windows.Forms.RadioButton radioButtonPointBlue;
        private System.Windows.Forms.RadioButton radioButtonPointGreen;
        private System.Windows.Forms.RadioButton radioButtonPointOrange;
        private System.Windows.Forms.RadioButton radioButtonPointRed;
        private System.Windows.Forms.RadioButton radioButtonPointBlack;
        private System.Windows.Forms.RadioButton radioButtonLineNone;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btnColor1;
        private System.Windows.Forms.Button btnColor0;
        private System.Windows.Forms.PictureBox pictureBoxColorRamp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxVisible;
        private System.Windows.Forms.CheckBox checkBoxConvertFlowToFlux;
    }
}