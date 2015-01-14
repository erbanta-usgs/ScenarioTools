namespace ScenarioTools.Data_Providers
{
    partial class DataProviderCbbGroupMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataProviderCbbGroupMenu));
            this.textBoxLayerEnd = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLayerStart = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.treeViewGroups = new System.Windows.Forms.TreeView();
            this.buttonAddGroup = new System.Windows.Forms.Button();
            this.labelColumn = new System.Windows.Forms.Label();
            this.buttonBrowseCbbFile = new System.Windows.Forms.Button();
            this.textBoxCbbFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxDataset = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textBoxLayerEnd
            // 
            this.textBoxLayerEnd.Location = new System.Drawing.Point(266, 160);
            this.textBoxLayerEnd.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLayerEnd.Name = "textBoxLayerEnd";
            this.textBoxLayerEnd.Size = new System.Drawing.Size(176, 24);
            this.textBoxLayerEnd.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(261, 132);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 18);
            this.label5.TabIndex = 93;
            this.label5.Text = "Layer End";
            // 
            // textBoxLayerStart
            // 
            this.textBoxLayerStart.Location = new System.Drawing.Point(8, 160);
            this.textBoxLayerStart.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLayerStart.Name = "textBoxLayerStart";
            this.textBoxLayerStart.Size = new System.Drawing.Size(176, 24);
            this.textBoxLayerStart.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 132);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 18);
            this.label4.TabIndex = 91;
            this.label4.Text = "Layer Start";
            // 
            // treeViewGroups
            // 
            this.treeViewGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewGroups.CheckBoxes = true;
            this.treeViewGroups.Location = new System.Drawing.Point(9, 224);
            this.treeViewGroups.Margin = new System.Windows.Forms.Padding(4);
            this.treeViewGroups.Name = "treeViewGroups";
            this.treeViewGroups.ShowNodeToolTips = true;
            this.treeViewGroups.Size = new System.Drawing.Size(478, 137);
            this.treeViewGroups.TabIndex = 5;
            // 
            // buttonAddGroup
            // 
            this.buttonAddGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddGroup.Location = new System.Drawing.Point(9, 369);
            this.buttonAddGroup.Margin = new System.Windows.Forms.Padding(4);
            this.buttonAddGroup.Name = "buttonAddGroup";
            this.buttonAddGroup.Size = new System.Drawing.Size(243, 32);
            this.buttonAddGroup.TabIndex = 6;
            this.buttonAddGroup.Text = "Add New Cell Group File";
            this.buttonAddGroup.UseVisualStyleBackColor = true;
            this.buttonAddGroup.Click += new System.EventHandler(this.buttonAddGroup_Click);
            // 
            // labelColumn
            // 
            this.labelColumn.AutoSize = true;
            this.labelColumn.Location = new System.Drawing.Point(4, 196);
            this.labelColumn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelColumn.Name = "labelColumn";
            this.labelColumn.Size = new System.Drawing.Size(367, 18);
            this.labelColumn.TabIndex = 88;
            this.labelColumn.Text = "Cell Groups (leave empty to use all cells in layer range)";
            // 
            // buttonBrowseCbbFile
            // 
            this.buttonBrowseCbbFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseCbbFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonBrowseCbbFile.Image")));
            this.buttonBrowseCbbFile.Location = new System.Drawing.Point(459, 31);
            this.buttonBrowseCbbFile.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBrowseCbbFile.Name = "buttonBrowseCbbFile";
            this.buttonBrowseCbbFile.Size = new System.Drawing.Size(30, 30);
            this.buttonBrowseCbbFile.TabIndex = 1;
            this.buttonBrowseCbbFile.UseVisualStyleBackColor = true;
            this.buttonBrowseCbbFile.Click += new System.EventHandler(this.buttonBrowseCbbFile_Click);
            // 
            // textBoxCbbFile
            // 
            this.textBoxCbbFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCbbFile.Location = new System.Drawing.Point(8, 32);
            this.textBoxCbbFile.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCbbFile.Name = "textBoxCbbFile";
            this.textBoxCbbFile.Size = new System.Drawing.Size(451, 24);
            this.textBoxCbbFile.TabIndex = 0;
            this.textBoxCbbFile.TextChanged += new System.EventHandler(this.textBoxCbbFile_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 18);
            this.label1.TabIndex = 85;
            this.label1.Text = "Cell-By-Cell Budget File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 18);
            this.label2.TabIndex = 95;
            this.label2.Text = "Data Identifier";
            // 
            // comboBoxDataset
            // 
            this.comboBoxDataset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDataset.FormattingEnabled = true;
            this.comboBoxDataset.Location = new System.Drawing.Point(9, 96);
            this.comboBoxDataset.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxDataset.Name = "comboBoxDataset";
            this.comboBoxDataset.Size = new System.Drawing.Size(480, 26);
            this.comboBoxDataset.TabIndex = 2;
            // 
            // DataProviderCbbGroupMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxDataset);
            this.Controls.Add(this.textBoxLayerEnd);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxLayerStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.treeViewGroups);
            this.Controls.Add(this.buttonAddGroup);
            this.Controls.Add(this.labelColumn);
            this.Controls.Add(this.buttonBrowseCbbFile);
            this.Controls.Add(this.textBoxCbbFile);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DataProviderCbbGroupMenu";
            this.Size = new System.Drawing.Size(505, 411);
            this.Load += new System.EventHandler(this.DataProviderCbbGroupMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLayerEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLayerStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TreeView treeViewGroups;
        private System.Windows.Forms.Button buttonAddGroup;
        private System.Windows.Forms.Label labelColumn;
        private System.Windows.Forms.Button buttonBrowseCbbFile;
        private System.Windows.Forms.TextBox textBoxCbbFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxDataset;
    }
}
