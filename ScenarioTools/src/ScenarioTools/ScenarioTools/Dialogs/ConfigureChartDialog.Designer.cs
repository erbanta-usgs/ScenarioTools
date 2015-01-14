namespace ScenarioTools.Dialogs
{
    partial class ConfigureChartDialog
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pgAxes = new System.Windows.Forms.TabPage();
            this.gbYAxis = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbYMinFixed = new System.Windows.Forms.RadioButton();
            this.rbYMinAuto = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbYMaxFixed = new System.Windows.Forms.RadioButton();
            this.rbYMaxAuto = new System.Windows.Forms.RadioButton();
            this.tbYMax = new System.Windows.Forms.TextBox();
            this.tbYMin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gbXAxis = new System.Windows.Forms.GroupBox();
            this.tbXMax = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbXMin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pgChartArea = new System.Windows.Forms.TabPage();
            this.pgData = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.pgAxes.SuspendLayout();
            this.gbYAxis.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbXAxis.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.pgAxes);
            this.tabControl1.Controls.Add(this.pgChartArea);
            this.tabControl1.Controls.Add(this.pgData);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(891, 590);
            this.tabControl1.TabIndex = 0;
            // 
            // pgAxes
            // 
            this.pgAxes.Controls.Add(this.gbYAxis);
            this.pgAxes.Controls.Add(this.gbXAxis);
            this.pgAxes.Location = new System.Drawing.Point(4, 27);
            this.pgAxes.Margin = new System.Windows.Forms.Padding(4);
            this.pgAxes.Name = "pgAxes";
            this.pgAxes.Padding = new System.Windows.Forms.Padding(4);
            this.pgAxes.Size = new System.Drawing.Size(883, 559);
            this.pgAxes.TabIndex = 0;
            this.pgAxes.Text = "Axes";
            this.pgAxes.UseVisualStyleBackColor = true;
            // 
            // gbYAxis
            // 
            this.gbYAxis.Controls.Add(this.panel3);
            this.gbYAxis.Controls.Add(this.panel1);
            this.gbYAxis.Controls.Add(this.tbYMax);
            this.gbYAxis.Controls.Add(this.tbYMin);
            this.gbYAxis.Controls.Add(this.label3);
            this.gbYAxis.Controls.Add(this.label4);
            this.gbYAxis.Location = new System.Drawing.Point(406, 4);
            this.gbYAxis.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbYAxis.Name = "gbYAxis";
            this.gbYAxis.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbYAxis.Size = new System.Drawing.Size(473, 553);
            this.gbYAxis.TabIndex = 1;
            this.gbYAxis.TabStop = false;
            this.gbYAxis.Text = "Y Axis";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbYMinFixed);
            this.panel3.Controls.Add(this.rbYMinAuto);
            this.panel3.Location = new System.Drawing.Point(115, 64);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(183, 25);
            this.panel3.TabIndex = 9;
            // 
            // rbYMinFixed
            // 
            this.rbYMinFixed.AutoSize = true;
            this.rbYMinFixed.Location = new System.Drawing.Point(93, 1);
            this.rbYMinFixed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbYMinFixed.Name = "rbYMinFixed";
            this.rbYMinFixed.Size = new System.Drawing.Size(61, 22);
            this.rbYMinFixed.TabIndex = 1;
            this.rbYMinFixed.TabStop = true;
            this.rbYMinFixed.Text = "Fixed";
            this.rbYMinFixed.UseVisualStyleBackColor = true;
            // 
            // rbYMinAuto
            // 
            this.rbYMinAuto.AutoSize = true;
            this.rbYMinAuto.Location = new System.Drawing.Point(17, 1);
            this.rbYMinAuto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbYMinAuto.Name = "rbYMinAuto";
            this.rbYMinAuto.Size = new System.Drawing.Size(56, 22);
            this.rbYMinAuto.TabIndex = 0;
            this.rbYMinAuto.TabStop = true;
            this.rbYMinAuto.Text = "Auto";
            this.rbYMinAuto.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbYMaxFixed);
            this.panel1.Controls.Add(this.rbYMaxAuto);
            this.panel1.Location = new System.Drawing.Point(115, 30);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(183, 25);
            this.panel1.TabIndex = 8;
            // 
            // rbYMaxFixed
            // 
            this.rbYMaxFixed.AutoSize = true;
            this.rbYMaxFixed.Location = new System.Drawing.Point(93, 1);
            this.rbYMaxFixed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbYMaxFixed.Name = "rbYMaxFixed";
            this.rbYMaxFixed.Size = new System.Drawing.Size(61, 22);
            this.rbYMaxFixed.TabIndex = 1;
            this.rbYMaxFixed.TabStop = true;
            this.rbYMaxFixed.Text = "Fixed";
            this.rbYMaxFixed.UseVisualStyleBackColor = true;
            // 
            // rbYMaxAuto
            // 
            this.rbYMaxAuto.AutoSize = true;
            this.rbYMaxAuto.Location = new System.Drawing.Point(17, 1);
            this.rbYMaxAuto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbYMaxAuto.Name = "rbYMaxAuto";
            this.rbYMaxAuto.Size = new System.Drawing.Size(56, 22);
            this.rbYMaxAuto.TabIndex = 0;
            this.rbYMaxAuto.TabStop = true;
            this.rbYMaxAuto.Text = "Auto";
            this.rbYMaxAuto.UseVisualStyleBackColor = true;
            // 
            // tbYMax
            // 
            this.tbYMax.Location = new System.Drawing.Point(310, 30);
            this.tbYMax.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbYMax.Name = "tbYMax";
            this.tbYMax.Size = new System.Drawing.Size(121, 24);
            this.tbYMax.TabIndex = 7;
            // 
            // tbYMin
            // 
            this.tbYMin.Location = new System.Drawing.Point(310, 64);
            this.tbYMin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbYMin.Name = "tbYMin";
            this.tbYMin.Size = new System.Drawing.Size(121, 24);
            this.tbYMin.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Maximum:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 18);
            this.label4.TabIndex = 4;
            this.label4.Text = "Minimum:";
            // 
            // gbXAxis
            // 
            this.gbXAxis.Controls.Add(this.tbXMax);
            this.gbXAxis.Controls.Add(this.label2);
            this.gbXAxis.Controls.Add(this.tbXMin);
            this.gbXAxis.Controls.Add(this.label1);
            this.gbXAxis.Enabled = false;
            this.gbXAxis.Location = new System.Drawing.Point(4, 4);
            this.gbXAxis.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbXAxis.Name = "gbXAxis";
            this.gbXAxis.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbXAxis.Size = new System.Drawing.Size(396, 553);
            this.gbXAxis.TabIndex = 0;
            this.gbXAxis.TabStop = false;
            this.gbXAxis.Text = "X Axis";
            // 
            // tbXMax
            // 
            this.tbXMax.Location = new System.Drawing.Point(167, 29);
            this.tbXMax.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbXMax.Name = "tbXMax";
            this.tbXMax.Size = new System.Drawing.Size(163, 24);
            this.tbXMax.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "Maximum:";
            // 
            // tbXMin
            // 
            this.tbXMin.Location = new System.Drawing.Point(167, 63);
            this.tbXMin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbXMin.Name = "tbXMin";
            this.tbXMin.Size = new System.Drawing.Size(163, 24);
            this.tbXMin.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minimum:";
            // 
            // pgChartArea
            // 
            this.pgChartArea.Location = new System.Drawing.Point(4, 27);
            this.pgChartArea.Margin = new System.Windows.Forms.Padding(4);
            this.pgChartArea.Name = "pgChartArea";
            this.pgChartArea.Padding = new System.Windows.Forms.Padding(4);
            this.pgChartArea.Size = new System.Drawing.Size(883, 559);
            this.pgChartArea.TabIndex = 1;
            this.pgChartArea.Text = "Chart Area";
            this.pgChartArea.UseVisualStyleBackColor = true;
            // 
            // pgData
            // 
            this.pgData.Location = new System.Drawing.Point(4, 27);
            this.pgData.Margin = new System.Windows.Forms.Padding(4);
            this.pgData.Name = "pgData";
            this.pgData.Size = new System.Drawing.Size(883, 559);
            this.pgData.TabIndex = 2;
            this.pgData.Text = "Data";
            this.pgData.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnApply);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 594);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(891, 43);
            this.panel2.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(451, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(362, 4);
            this.btnApply.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(84, 32);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // ConfigureChartDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 637);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConfigureChartDialog";
            this.Text = "Configure Chart";
            this.Load += new System.EventHandler(this.ConfigureChartDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.pgAxes.ResumeLayout(false);
            this.gbYAxis.ResumeLayout(false);
            this.gbYAxis.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbXAxis.ResumeLayout(false);
            this.gbXAxis.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pgAxes;
        private System.Windows.Forms.TabPage pgChartArea;
        private System.Windows.Forms.TabPage pgData;
        private System.Windows.Forms.GroupBox gbYAxis;
        private System.Windows.Forms.GroupBox gbXAxis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbXMin;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbXMax;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbYMax;
        private System.Windows.Forms.TextBox tbYMin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbYMaxFixed;
        private System.Windows.Forms.RadioButton rbYMaxAuto;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbYMinFixed;
        private System.Windows.Forms.RadioButton rbYMinAuto;
    }
}