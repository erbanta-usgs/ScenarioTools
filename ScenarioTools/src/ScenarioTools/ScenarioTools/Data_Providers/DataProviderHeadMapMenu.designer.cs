namespace ScenarioTools.Data_Providers
{
    partial class DataProviderHeadMapMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataProviderHeadMapMenu));
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBrowseHeadsFile = new System.Windows.Forms.Button();
            this.textBoxHeadsFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxStressPeriod = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTimestep = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownLayer = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLayer)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 24);
            this.label2.TabIndex = 55;
            this.label2.Text = "Layer";
            // 
            // buttonBrowseHeadsFile
            // 
            this.buttonBrowseHeadsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseHeadsFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonBrowseHeadsFile.Image")));
            this.buttonBrowseHeadsFile.Location = new System.Drawing.Point(389, 31);
            this.buttonBrowseHeadsFile.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBrowseHeadsFile.Name = "buttonBrowseHeadsFile";
            this.buttonBrowseHeadsFile.Size = new System.Drawing.Size(30, 30);
            this.buttonBrowseHeadsFile.TabIndex = 50;
            this.buttonBrowseHeadsFile.UseVisualStyleBackColor = true;
            this.buttonBrowseHeadsFile.Click += new System.EventHandler(this.buttonBrowseHeadsFile_Click);
            // 
            // textBoxHeadsFile
            // 
            this.textBoxHeadsFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHeadsFile.Location = new System.Drawing.Point(8, 32);
            this.textBoxHeadsFile.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxHeadsFile.Name = "textBoxHeadsFile";
            this.textBoxHeadsFile.Size = new System.Drawing.Size(380, 28);
            this.textBoxHeadsFile.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(418, 24);
            this.label1.TabIndex = 48;
            this.label1.Text = "Binary File of Head, Drawdown, or Concentration";
            // 
            // textBoxStressPeriod
            // 
            this.textBoxStressPeriod.Location = new System.Drawing.Point(8, 160);
            this.textBoxStressPeriod.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxStressPeriod.Name = "textBoxStressPeriod";
            this.textBoxStressPeriod.Size = new System.Drawing.Size(176, 28);
            this.textBoxStressPeriod.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 133);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 24);
            this.label4.TabIndex = 57;
            this.label4.Text = "Stress Period";
            // 
            // textBoxTimestep
            // 
            this.textBoxTimestep.Location = new System.Drawing.Point(200, 160);
            this.textBoxTimestep.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxTimestep.Name = "textBoxTimestep";
            this.textBoxTimestep.Size = new System.Drawing.Size(176, 28);
            this.textBoxTimestep.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(197, 133);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 24);
            this.label5.TabIndex = 59;
            this.label5.Text = "Time Step";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(200, 94);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(176, 30);
            this.comboBox1.TabIndex = 60;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 24);
            this.label3.TabIndex = 61;
            this.label3.Text = "Data Identifier";
            // 
            // numericUpDownLayer
            // 
            this.numericUpDownLayer.Location = new System.Drawing.Point(8, 95);
            this.numericUpDownLayer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLayer.Name = "numericUpDownLayer";
            this.numericUpDownLayer.Size = new System.Drawing.Size(120, 28);
            this.numericUpDownLayer.TabIndex = 62;
            this.numericUpDownLayer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // DataProviderHeadMapMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.numericUpDownLayer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBoxTimestep);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxStressPeriod);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonBrowseHeadsFile);
            this.Controls.Add(this.textBoxHeadsFile);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DataProviderHeadMapMenu";
            this.Size = new System.Drawing.Size(429, 201);
            this.Load += new System.EventHandler(this.DataProviderHeadMapMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBrowseHeadsFile;
        private System.Windows.Forms.TextBox textBoxHeadsFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStressPeriod;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTimestep;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownLayer;

    }
}