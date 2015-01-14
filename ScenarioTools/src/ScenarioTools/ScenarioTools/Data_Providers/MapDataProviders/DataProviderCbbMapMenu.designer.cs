namespace ScenarioTools.Data_Providers
{
    partial class DataProviderCbbMapMenu
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataProviderCbbMapMenu));
            this.textBoxTimestep = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxStressPeriod = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBrowseHeadsFile = new System.Windows.Forms.Button();
            this.textBoxCbbFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDataset = new System.Windows.Forms.ComboBox();
            this.buttonLayerDialog = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownStartLayer = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownEndLayer = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartLayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndLayer)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxTimestep
            // 
            this.textBoxTimestep.Location = new System.Drawing.Point(200, 160);
            this.textBoxTimestep.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxTimestep.Name = "textBoxTimestep";
            this.textBoxTimestep.Size = new System.Drawing.Size(176, 28);
            this.textBoxTimestep.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(196, 132);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 24);
            this.label5.TabIndex = 68;
            this.label5.Text = "Time Step";
            // 
            // textBoxStressPeriod
            // 
            this.textBoxStressPeriod.Location = new System.Drawing.Point(8, 160);
            this.textBoxStressPeriod.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxStressPeriod.Name = "textBoxStressPeriod";
            this.textBoxStressPeriod.Size = new System.Drawing.Size(176, 28);
            this.textBoxStressPeriod.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 132);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 24);
            this.label4.TabIndex = 66;
            this.label4.Text = "Stress Period";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 24);
            this.label2.TabIndex = 65;
            this.label2.Text = "Data Identifier";
            // 
            // buttonBrowseHeadsFile
            // 
            this.buttonBrowseHeadsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseHeadsFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonBrowseHeadsFile.Image")));
            this.buttonBrowseHeadsFile.Location = new System.Drawing.Point(346, 31);
            this.buttonBrowseHeadsFile.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBrowseHeadsFile.Name = "buttonBrowseHeadsFile";
            this.buttonBrowseHeadsFile.Size = new System.Drawing.Size(30, 30);
            this.buttonBrowseHeadsFile.TabIndex = 1;
            this.buttonBrowseHeadsFile.UseVisualStyleBackColor = true;
            this.buttonBrowseHeadsFile.Click += new System.EventHandler(this.buttonBrowseHeadsFile_Click);
            // 
            // textBoxCbbFile
            // 
            this.textBoxCbbFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCbbFile.Location = new System.Drawing.Point(8, 32);
            this.textBoxCbbFile.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCbbFile.Name = "textBoxCbbFile";
            this.textBoxCbbFile.Size = new System.Drawing.Size(338, 28);
            this.textBoxCbbFile.TabIndex = 0;
            this.textBoxCbbFile.TextChanged += new System.EventHandler(this.textBoxCbbFile_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 24);
            this.label1.TabIndex = 62;
            this.label1.Text = "Cell-By-Cell Budget File";
            // 
            // comboBoxDataset
            // 
            this.comboBoxDataset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDataset.FormattingEnabled = true;
            this.comboBoxDataset.Location = new System.Drawing.Point(8, 96);
            this.comboBoxDataset.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxDataset.Name = "comboBoxDataset";
            this.comboBoxDataset.Size = new System.Drawing.Size(368, 30);
            this.comboBoxDataset.TabIndex = 2;
            this.comboBoxDataset.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataset_SelectedIndexChanged);
            // 
            // buttonLayerDialog
            // 
            this.buttonLayerDialog.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLayerDialog.Location = new System.Drawing.Point(8, 201);
            this.buttonLayerDialog.Margin = new System.Windows.Forms.Padding(4);
            this.buttonLayerDialog.Name = "buttonLayerDialog";
            this.buttonLayerDialog.Size = new System.Drawing.Size(368, 46);
            this.buttonLayerDialog.TabIndex = 5;
            this.buttonLayerDialog.Text = "View Available Data Sets";
            this.buttonLayerDialog.UseVisualStyleBackColor = true;
            this.buttonLayerDialog.Click += new System.EventHandler(this.buttonLayerDialog_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(196, 264);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 24);
            this.label6.TabIndex = 80;
            this.label6.Text = "End Layer";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 264);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 24);
            this.label3.TabIndex = 79;
            this.label3.Text = "Start Layer";
            // 
            // numericUpDownStartLayer
            // 
            this.numericUpDownStartLayer.Location = new System.Drawing.Point(8, 291);
            this.numericUpDownStartLayer.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownStartLayer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownStartLayer.Name = "numericUpDownStartLayer";
            this.numericUpDownStartLayer.Size = new System.Drawing.Size(93, 28);
            this.numericUpDownStartLayer.TabIndex = 81;
            this.numericUpDownStartLayer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownStartLayer.ValueChanged += new System.EventHandler(this.numericUpDownStartLayer_ValueChanged);
            // 
            // numericUpDownEndLayer
            // 
            this.numericUpDownEndLayer.Location = new System.Drawing.Point(200, 291);
            this.numericUpDownEndLayer.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownEndLayer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownEndLayer.Name = "numericUpDownEndLayer";
            this.numericUpDownEndLayer.Size = new System.Drawing.Size(93, 28);
            this.numericUpDownEndLayer.TabIndex = 82;
            this.numericUpDownEndLayer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownEndLayer.ValueChanged += new System.EventHandler(this.numericUpDownEndLayer_ValueChanged);
            // 
            // DataProviderCbbMapMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.numericUpDownEndLayer);
            this.Controls.Add(this.numericUpDownStartLayer);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonLayerDialog);
            this.Controls.Add(this.textBoxTimestep);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxStressPeriod);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonBrowseHeadsFile);
            this.Controls.Add(this.textBoxCbbFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxDataset);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DataProviderCbbMapMenu";
            this.Size = new System.Drawing.Size(390, 393);
            this.Load += new System.EventHandler(this.DataProviderCbbMapMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartLayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndLayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxTimestep;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxStressPeriod;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBrowseHeadsFile;
        private System.Windows.Forms.TextBox textBoxCbbFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDataset;
        private System.Windows.Forms.Button buttonLayerDialog;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownStartLayer;
        private System.Windows.Forms.NumericUpDown numericUpDownEndLayer;
    }
}
