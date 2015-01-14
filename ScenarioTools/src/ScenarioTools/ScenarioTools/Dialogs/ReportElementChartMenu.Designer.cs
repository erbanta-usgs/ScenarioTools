namespace ScenarioTools.Dialogs
{
    partial class ReportElementChartMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportElementChartMenu));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pgGeneral = new System.Windows.Forms.TabPage();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pgDomainRange = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxValueMax = new System.Windows.Forms.TextBox();
            this.textBoxValueMin = new System.Windows.Forms.TextBox();
            this.radioButtonValueRangeManual = new System.Windows.Forms.RadioButton();
            this.radioButtonValueRangeAutomatic = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.radioButtonDateRangeManual = new System.Windows.Forms.RadioButton();
            this.radioButtonDateRangeAutomatic = new System.Windows.Forms.RadioButton();
            this.pgDataSeries = new System.Windows.Forms.TabPage();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxDataSeries = new System.Windows.Forms.ListBox();
            this.pgChartProperties = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnConfigure = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tabControl1.SuspendLayout();
            this.pgGeneral.SuspendLayout();
            this.pgDomainRange.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pgDataSeries.SuspendLayout();
            this.pgChartProperties.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.pgGeneral);
            this.tabControl1.Controls.Add(this.pgDomainRange);
            this.tabControl1.Controls.Add(this.pgDataSeries);
            this.tabControl1.Controls.Add(this.pgChartProperties);
            this.tabControl1.Location = new System.Drawing.Point(3, 5);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(777, 454);
            this.tabControl1.TabIndex = 0;
            // 
            // pgGeneral
            // 
            this.pgGeneral.Controls.Add(this.textBoxName);
            this.pgGeneral.Controls.Add(this.label1);
            this.pgGeneral.Location = new System.Drawing.Point(4, 31);
            this.pgGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.pgGeneral.Name = "pgGeneral";
            this.pgGeneral.Padding = new System.Windows.Forms.Padding(4);
            this.pgGeneral.Size = new System.Drawing.Size(769, 419);
            this.pgGeneral.TabIndex = 0;
            this.pgGeneral.Text = "General";
            this.pgGeneral.UseVisualStyleBackColor = true;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(4, 32);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(365, 28);
            this.textBoxName.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // pgDomainRange
            // 
            this.pgDomainRange.Controls.Add(this.groupBox2);
            this.pgDomainRange.Controls.Add(this.groupBox1);
            this.pgDomainRange.Location = new System.Drawing.Point(4, 31);
            this.pgDomainRange.Margin = new System.Windows.Forms.Padding(4);
            this.pgDomainRange.Name = "pgDomainRange";
            this.pgDomainRange.Padding = new System.Windows.Forms.Padding(4);
            this.pgDomainRange.Size = new System.Drawing.Size(769, 419);
            this.pgDomainRange.TabIndex = 1;
            this.pgDomainRange.Text = "Domain/Range";
            this.pgDomainRange.UseVisualStyleBackColor = true;
            this.pgDomainRange.Leave += new System.EventHandler(this.pgDomainRange_Leave);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxValueMax);
            this.groupBox2.Controls.Add(this.textBoxValueMin);
            this.groupBox2.Controls.Add(this.radioButtonValueRangeManual);
            this.groupBox2.Controls.Add(this.radioButtonValueRangeAutomatic);
            this.groupBox2.Location = new System.Drawing.Point(9, 202);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(497, 187);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Range (Y-axis)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 24);
            this.label6.TabIndex = 9;
            this.label6.Text = "Maximum:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 24);
            this.label5.TabIndex = 8;
            this.label5.Text = "Minimum:";
            // 
            // textBoxValueMax
            // 
            this.textBoxValueMax.Location = new System.Drawing.Point(132, 145);
            this.textBoxValueMax.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxValueMax.Name = "textBoxValueMax";
            this.textBoxValueMax.Size = new System.Drawing.Size(346, 28);
            this.textBoxValueMax.TabIndex = 8;
            // 
            // textBoxValueMin
            // 
            this.textBoxValueMin.Location = new System.Drawing.Point(132, 91);
            this.textBoxValueMin.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxValueMin.Name = "textBoxValueMin";
            this.textBoxValueMin.Size = new System.Drawing.Size(346, 28);
            this.textBoxValueMin.TabIndex = 7;
            // 
            // radioButtonValueRangeManual
            // 
            this.radioButtonValueRangeManual.AutoSize = true;
            this.radioButtonValueRangeManual.Location = new System.Drawing.Point(10, 58);
            this.radioButtonValueRangeManual.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonValueRangeManual.Name = "radioButtonValueRangeManual";
            this.radioButtonValueRangeManual.Size = new System.Drawing.Size(93, 28);
            this.radioButtonValueRangeManual.TabIndex = 6;
            this.radioButtonValueRangeManual.TabStop = true;
            this.radioButtonValueRangeManual.Text = "Manual";
            this.radioButtonValueRangeManual.UseVisualStyleBackColor = true;
            this.radioButtonValueRangeManual.CheckedChanged += new System.EventHandler(this.radioButtonValueRangeManual_CheckedChanged);
            // 
            // radioButtonValueRangeAutomatic
            // 
            this.radioButtonValueRangeAutomatic.AutoSize = true;
            this.radioButtonValueRangeAutomatic.Location = new System.Drawing.Point(10, 26);
            this.radioButtonValueRangeAutomatic.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonValueRangeAutomatic.Name = "radioButtonValueRangeAutomatic";
            this.radioButtonValueRangeAutomatic.Size = new System.Drawing.Size(114, 28);
            this.radioButtonValueRangeAutomatic.TabIndex = 5;
            this.radioButtonValueRangeAutomatic.TabStop = true;
            this.radioButtonValueRangeAutomatic.Text = "Automatic";
            this.radioButtonValueRangeAutomatic.UseVisualStyleBackColor = true;
            this.radioButtonValueRangeAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonValueRangeAutomatic_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dateTimePickerEnd);
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Controls.Add(this.radioButtonDateRangeManual);
            this.groupBox1.Controls.Add(this.radioButtonDateRangeAutomatic);
            this.groupBox1.Location = new System.Drawing.Point(9, 9);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(497, 182);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Domain (X-axis)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "Maximum:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "Minimum:";
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Location = new System.Drawing.Point(132, 148);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(346, 28);
            this.dateTimePickerEnd.TabIndex = 4;
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Location = new System.Drawing.Point(132, 94);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(346, 28);
            this.dateTimePickerStart.TabIndex = 2;
            // 
            // radioButtonDateRangeManual
            // 
            this.radioButtonDateRangeManual.AutoSize = true;
            this.radioButtonDateRangeManual.Location = new System.Drawing.Point(10, 59);
            this.radioButtonDateRangeManual.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDateRangeManual.Name = "radioButtonDateRangeManual";
            this.radioButtonDateRangeManual.Size = new System.Drawing.Size(93, 28);
            this.radioButtonDateRangeManual.TabIndex = 1;
            this.radioButtonDateRangeManual.TabStop = true;
            this.radioButtonDateRangeManual.Text = "Manual";
            this.radioButtonDateRangeManual.UseVisualStyleBackColor = true;
            this.radioButtonDateRangeManual.CheckedChanged += new System.EventHandler(this.radioButtonDateRangeManual_CheckedChanged);
            // 
            // radioButtonDateRangeAutomatic
            // 
            this.radioButtonDateRangeAutomatic.AutoSize = true;
            this.radioButtonDateRangeAutomatic.Location = new System.Drawing.Point(10, 28);
            this.radioButtonDateRangeAutomatic.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDateRangeAutomatic.Name = "radioButtonDateRangeAutomatic";
            this.radioButtonDateRangeAutomatic.Size = new System.Drawing.Size(114, 28);
            this.radioButtonDateRangeAutomatic.TabIndex = 0;
            this.radioButtonDateRangeAutomatic.TabStop = true;
            this.radioButtonDateRangeAutomatic.Text = "Automatic";
            this.radioButtonDateRangeAutomatic.UseVisualStyleBackColor = true;
            this.radioButtonDateRangeAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonDateRangeAutomatic_CheckedChanged);
            // 
            // pgDataSeries
            // 
            this.pgDataSeries.Controls.Add(this.buttonDown);
            this.pgDataSeries.Controls.Add(this.buttonRemove);
            this.pgDataSeries.Controls.Add(this.buttonUp);
            this.pgDataSeries.Controls.Add(this.label4);
            this.pgDataSeries.Controls.Add(this.listBoxDataSeries);
            this.pgDataSeries.Location = new System.Drawing.Point(4, 31);
            this.pgDataSeries.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pgDataSeries.Name = "pgDataSeries";
            this.pgDataSeries.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pgDataSeries.Size = new System.Drawing.Size(769, 419);
            this.pgDataSeries.TabIndex = 2;
            this.pgDataSeries.Text = "Data Series";
            this.pgDataSeries.UseVisualStyleBackColor = true;
            // 
            // buttonDown
            // 
            this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDown.Image = global::ScenarioTools.Properties.Resources.down_arrow;
            this.buttonDown.Location = new System.Drawing.Point(691, 164);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(68, 62);
            this.buttonDown.TabIndex = 15;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemove.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemove.Image")));
            this.buttonRemove.Location = new System.Drawing.Point(691, 94);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(68, 62);
            this.buttonRemove.TabIndex = 14;
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUp.Image = global::ScenarioTools.Properties.Resources.up_arrow;
            this.buttonUp.Location = new System.Drawing.Point(691, 23);
            this.buttonUp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(68, 62);
            this.buttonUp.TabIndex = 13;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 4);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 24);
            this.label4.TabIndex = 10;
            this.label4.Text = "Data Series";
            // 
            // listBoxDataSeries
            // 
            this.listBoxDataSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxDataSeries.FormattingEnabled = true;
            this.listBoxDataSeries.ItemHeight = 22;
            this.listBoxDataSeries.Location = new System.Drawing.Point(4, 32);
            this.listBoxDataSeries.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxDataSeries.Name = "listBoxDataSeries";
            this.listBoxDataSeries.Size = new System.Drawing.Size(671, 378);
            this.listBoxDataSeries.TabIndex = 11;
            // 
            // pgChartProperties
            // 
            this.pgChartProperties.Controls.Add(this.splitContainer1);
            this.pgChartProperties.Location = new System.Drawing.Point(4, 31);
            this.pgChartProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pgChartProperties.Name = "pgChartProperties";
            this.pgChartProperties.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pgChartProperties.Size = new System.Drawing.Size(769, 419);
            this.pgChartProperties.TabIndex = 3;
            this.pgChartProperties.Text = "Chart Properties";
            this.pgChartProperties.UseVisualStyleBackColor = true;
            this.pgChartProperties.Layout += new System.Windows.Forms.LayoutEventHandler(this.pgChartProperties_Layout);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 4);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnConfigure);
            this.splitContainer1.Size = new System.Drawing.Size(763, 411);
            this.splitContainer1.SplitterDistance = 129;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnConfigure
            // 
            this.btnConfigure.Enabled = false;
            this.btnConfigure.Location = new System.Drawing.Point(18, 73);
            this.btnConfigure.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.Size = new System.Drawing.Size(102, 58);
            this.btnConfigure.TabIndex = 0;
            this.btnConfigure.Text = "Configure...";
            this.btnConfigure.UseVisualStyleBackColor = true;
            this.btnConfigure.Visible = false;
            this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(3, 467);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 30);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(94, 467);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            // 
            // ReportElementChartMenu
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(782, 505);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(542, 524);
            this.Name = "ReportElementChartMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Chart";
            this.Deactivate += new System.EventHandler(this.ReportElementChartMenu_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReportElementChartMenu_FormClosing);
            this.Load += new System.EventHandler(this.ReportElementChartMenu_Load);
            this.ResizeBegin += new System.EventHandler(this.ReportElementChartMenu_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.ReportElementChartMenu_ResizeEnd);
            this.tabControl1.ResumeLayout(false);
            this.pgGeneral.ResumeLayout(false);
            this.pgGeneral.PerformLayout();
            this.pgDomainRange.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pgDataSeries.ResumeLayout(false);
            this.pgDataSeries.PerformLayout();
            this.pgChartProperties.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pgGeneral;
        private System.Windows.Forms.TabPage pgDomainRange;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabPage pgDataSeries;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxValueMax;
        private System.Windows.Forms.TextBox textBoxValueMin;
        private System.Windows.Forms.RadioButton radioButtonValueRangeManual;
        private System.Windows.Forms.RadioButton radioButtonValueRangeAutomatic;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.RadioButton radioButtonDateRangeManual;
        private System.Windows.Forms.RadioButton radioButtonDateRangeAutomatic;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxDataSeries;
        private System.Windows.Forms.TabPage pgChartProperties;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button btnConfigure;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
    }
}