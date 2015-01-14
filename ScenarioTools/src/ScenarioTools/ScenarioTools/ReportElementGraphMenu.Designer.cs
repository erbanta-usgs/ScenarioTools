namespace ScenarioTools
{
    partial class ReportElementGraphMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportElementGraphMenu));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageDomainRange = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxValueMax = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxValueMin = new System.Windows.Forms.TextBox();
            this.radioButtonValueRangeManual = new System.Windows.Forms.RadioButton();
            this.radioButtonValueRangeAutomatic = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.radioButtonDateRangeManual = new System.Windows.Forms.RadioButton();
            this.radioButtonDateRangeAutomatic = new System.Windows.Forms.RadioButton();
            this.tabPageDataSeries = new System.Windows.Forms.TabPage();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxDataSeries = new System.Windows.Forms.ListBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageDomainRange.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageDataSeries.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageDomainRange);
            this.tabControl1.Controls.Add(this.tabPageDataSeries);
            this.tabControl1.Location = new System.Drawing.Point(16, 15);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(757, 378);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.textBoxName);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabPageGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageGeneral.Size = new System.Drawing.Size(749, 349);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(12, 23);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(325, 23);
            this.textBoxName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // tabPageDomainRange
            // 
            this.tabPageDomainRange.Controls.Add(this.groupBox2);
            this.tabPageDomainRange.Controls.Add(this.groupBox1);
            this.tabPageDomainRange.Location = new System.Drawing.Point(4, 25);
            this.tabPageDomainRange.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageDomainRange.Name = "tabPageDomainRange";
            this.tabPageDomainRange.Size = new System.Drawing.Size(749, 349);
            this.tabPageDomainRange.TabIndex = 1;
            this.tabPageDomainRange.Text = "Domain/Range";
            this.tabPageDomainRange.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxValueMax);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxValueMin);
            this.groupBox2.Controls.Add(this.radioButtonValueRangeManual);
            this.groupBox2.Controls.Add(this.radioButtonValueRangeAutomatic);
            this.groupBox2.Location = new System.Drawing.Point(5, 176);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(327, 166);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Range";
            // 
            // textBoxValueMax
            // 
            this.textBoxValueMax.Location = new System.Drawing.Point(9, 129);
            this.textBoxValueMax.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxValueMax.Name = "textBoxValueMax";
            this.textBoxValueMax.Size = new System.Drawing.Size(308, 23);
            this.textBoxValueMax.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(147, 110);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "to";
            // 
            // textBoxValueMin
            // 
            this.textBoxValueMin.Location = new System.Drawing.Point(9, 81);
            this.textBoxValueMin.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxValueMin.Name = "textBoxValueMin";
            this.textBoxValueMin.Size = new System.Drawing.Size(308, 23);
            this.textBoxValueMin.TabIndex = 6;
            // 
            // radioButtonValueRangeManual
            // 
            this.radioButtonValueRangeManual.AutoSize = true;
            this.radioButtonValueRangeManual.Location = new System.Drawing.Point(9, 52);
            this.radioButtonValueRangeManual.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonValueRangeManual.Name = "radioButtonValueRangeManual";
            this.radioButtonValueRangeManual.Size = new System.Drawing.Size(72, 21);
            this.radioButtonValueRangeManual.TabIndex = 5;
            this.radioButtonValueRangeManual.TabStop = true;
            this.radioButtonValueRangeManual.Text = "Manual";
            this.radioButtonValueRangeManual.UseVisualStyleBackColor = true;
            this.radioButtonValueRangeManual.CheckedChanged += new System.EventHandler(this.radioButtonValueRangeManual_CheckedChanged);
            // 
            // radioButtonValueRangeAutomatic
            // 
            this.radioButtonValueRangeAutomatic.AutoSize = true;
            this.radioButtonValueRangeAutomatic.Location = new System.Drawing.Point(9, 23);
            this.radioButtonValueRangeAutomatic.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonValueRangeAutomatic.Name = "radioButtonValueRangeAutomatic";
            this.radioButtonValueRangeAutomatic.Size = new System.Drawing.Size(88, 21);
            this.radioButtonValueRangeAutomatic.TabIndex = 5;
            this.radioButtonValueRangeAutomatic.TabStop = true;
            this.radioButtonValueRangeAutomatic.Text = "Automatic";
            this.radioButtonValueRangeAutomatic.UseVisualStyleBackColor = true;
            this.radioButtonValueRangeAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonValueRangeAutomatic_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dateTimePickerEnd);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Controls.Add(this.radioButtonDateRangeManual);
            this.groupBox1.Controls.Add(this.radioButtonDateRangeAutomatic);
            this.groupBox1.Location = new System.Drawing.Point(5, 5);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(327, 162);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Domain";
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Location = new System.Drawing.Point(9, 129);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(308, 23);
            this.dateTimePickerEnd.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 110);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "to";
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Location = new System.Drawing.Point(9, 81);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(308, 23);
            this.dateTimePickerStart.TabIndex = 2;
            // 
            // radioButtonDateRangeManual
            // 
            this.radioButtonDateRangeManual.AutoSize = true;
            this.radioButtonDateRangeManual.Location = new System.Drawing.Point(9, 53);
            this.radioButtonDateRangeManual.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDateRangeManual.Name = "radioButtonDateRangeManual";
            this.radioButtonDateRangeManual.Size = new System.Drawing.Size(72, 21);
            this.radioButtonDateRangeManual.TabIndex = 1;
            this.radioButtonDateRangeManual.TabStop = true;
            this.radioButtonDateRangeManual.Text = "Manual";
            this.radioButtonDateRangeManual.UseVisualStyleBackColor = true;
            this.radioButtonDateRangeManual.CheckedChanged += new System.EventHandler(this.radioButtonDateRangeManual_CheckedChanged);
            // 
            // radioButtonDateRangeAutomatic
            // 
            this.radioButtonDateRangeAutomatic.AutoSize = true;
            this.radioButtonDateRangeAutomatic.Location = new System.Drawing.Point(9, 25);
            this.radioButtonDateRangeAutomatic.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDateRangeAutomatic.Name = "radioButtonDateRangeAutomatic";
            this.radioButtonDateRangeAutomatic.Size = new System.Drawing.Size(88, 21);
            this.radioButtonDateRangeAutomatic.TabIndex = 0;
            this.radioButtonDateRangeAutomatic.TabStop = true;
            this.radioButtonDateRangeAutomatic.Text = "Automatic";
            this.radioButtonDateRangeAutomatic.UseVisualStyleBackColor = true;
            this.radioButtonDateRangeAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonDateRangeAutomatic_CheckedChanged);
            // 
            // tabPageDataSeries
            // 
            this.tabPageDataSeries.Controls.Add(this.buttonDown);
            this.tabPageDataSeries.Controls.Add(this.buttonRemove);
            this.tabPageDataSeries.Controls.Add(this.buttonUp);
            this.tabPageDataSeries.Controls.Add(this.label4);
            this.tabPageDataSeries.Controls.Add(this.listBoxDataSeries);
            this.tabPageDataSeries.Location = new System.Drawing.Point(4, 25);
            this.tabPageDataSeries.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageDataSeries.Name = "tabPageDataSeries";
            this.tabPageDataSeries.Size = new System.Drawing.Size(749, 349);
            this.tabPageDataSeries.TabIndex = 2;
            this.tabPageDataSeries.Text = "Data Series";
            this.tabPageDataSeries.UseVisualStyleBackColor = true;
            // 
            // buttonDown
            // 
            this.buttonDown.Image = global::ScenarioTools.Properties.Resources.down_arrow;
            this.buttonDown.Location = new System.Drawing.Point(683, 154);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(60, 55);
            this.buttonDown.TabIndex = 12;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemove.Image")));
            this.buttonRemove.Location = new System.Drawing.Point(683, 91);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(60, 55);
            this.buttonRemove.TabIndex = 11;
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Image = global::ScenarioTools.Properties.Resources.up_arrow;
            this.buttonUp.Location = new System.Drawing.Point(683, 28);
            this.buttonUp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(60, 55);
            this.buttonUp.TabIndex = 10;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Data Series";
            // 
            // listBoxDataSeries
            // 
            this.listBoxDataSeries.FormattingEnabled = true;
            this.listBoxDataSeries.ItemHeight = 16;
            this.listBoxDataSeries.Location = new System.Drawing.Point(8, 28);
            this.listBoxDataSeries.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxDataSeries.Name = "listBoxDataSeries";
            this.listBoxDataSeries.Size = new System.Drawing.Size(667, 308);
            this.listBoxDataSeries.TabIndex = 9;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(565, 400);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(100, 28);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(673, 400);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 28);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // ReportElementGraphMenu
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(789, 443);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ReportElementGraphMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Graph";
            this.Load += new System.EventHandler(this.ReportElementGraphMenu_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPageDomainRange.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageDataSeries.ResumeLayout(false);
            this.tabPageDataSeries.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageDomainRange;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.RadioButton radioButtonDateRangeManual;
        private System.Windows.Forms.RadioButton radioButtonDateRangeAutomatic;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxValueMax;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxValueMin;
        private System.Windows.Forms.RadioButton radioButtonValueRangeManual;
        private System.Windows.Forms.RadioButton radioButtonValueRangeAutomatic;
        private System.Windows.Forms.TabPage tabPageDataSeries;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxDataSeries;
    }
}