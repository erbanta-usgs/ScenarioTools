namespace ScenarioTools.Dialogs
{
    partial class DateTimeDialog
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
            this.udYear = new System.Windows.Forms.NumericUpDown();
            this.udDay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.udHour = new System.Windows.Forms.NumericUpDown();
            this.udMinute = new System.Windows.Forms.NumericUpDown();
            this.udSecond = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxMonth = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.udYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSecond)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // udYear
            // 
            this.udYear.Location = new System.Drawing.Point(16, 40);
            this.udYear.Margin = new System.Windows.Forms.Padding(4);
            this.udYear.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.udYear.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udYear.Name = "udYear";
            this.udYear.Size = new System.Drawing.Size(66, 28);
            this.udYear.TabIndex = 0;
            this.udYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udYear.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udYear.ValueChanged += new System.EventHandler(this.udYear_ValueChanged);
            // 
            // udDay
            // 
            this.udDay.Location = new System.Drawing.Point(205, 40);
            this.udDay.Margin = new System.Windows.Forms.Padding(4);
            this.udDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.udDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udDay.Name = "udDay";
            this.udDay.Size = new System.Drawing.Size(48, 28);
            this.udDay.TabIndex = 2;
            this.udDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Year";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(113, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "Month";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(205, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "Day";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(47, 165);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 30);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(138, 165);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // udHour
            // 
            this.udHour.Location = new System.Drawing.Point(20, 114);
            this.udHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.udHour.Name = "udHour";
            this.udHour.Size = new System.Drawing.Size(48, 28);
            this.udHour.TabIndex = 3;
            this.udHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // udMinute
            // 
            this.udMinute.Location = new System.Drawing.Point(108, 114);
            this.udMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.udMinute.Name = "udMinute";
            this.udMinute.Size = new System.Drawing.Size(48, 28);
            this.udMinute.TabIndex = 4;
            this.udMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // udSecond
            // 
            this.udSecond.Location = new System.Drawing.Point(197, 114);
            this.udSecond.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.udSecond.Name = "udSecond";
            this.udSecond.Size = new System.Drawing.Size(48, 28);
            this.udSecond.TabIndex = 5;
            this.udSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 24);
            this.label4.TabIndex = 12;
            this.label4.Text = "Hour";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(100, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 24);
            this.label5.TabIndex = 13;
            this.label5.Text = "Minute";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(183, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 24);
            this.label6.TabIndex = 14;
            this.label6.Text = "Second";
            // 
            // comboBoxMonth
            // 
            this.comboBoxMonth.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMonth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxMonth.FormattingEnabled = true;
            this.comboBoxMonth.Items.AddRange(new object[] {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"});
            this.comboBoxMonth.Location = new System.Drawing.Point(0, 0);
            this.comboBoxMonth.MaxDropDownItems = 12;
            this.comboBoxMonth.Name = "comboBoxMonth";
            this.comboBoxMonth.Size = new System.Drawing.Size(109, 30);
            this.comboBoxMonth.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.comboBoxMonth);
            this.panel1.Location = new System.Drawing.Point(89, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(109, 30);
            this.panel1.TabIndex = 18;
            // 
            // DateTimeDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(270, 205);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.udSecond);
            this.Controls.Add(this.udMinute);
            this.Controls.Add(this.udHour);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udDay);
            this.Controls.Add(this.udYear);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DateTimeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Date Time Dialog";
            this.Activated += new System.EventHandler(this.DateTimeDialog_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.udYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSecond)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown udYear;
        private System.Windows.Forms.NumericUpDown udDay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown udHour;
        private System.Windows.Forms.NumericUpDown udMinute;
        private System.Windows.Forms.NumericUpDown udSecond;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxMonth;
        private System.Windows.Forms.Panel panel1;

    }
}