namespace ScenarioTools.Data_Providers
{
    partial class DataProviderHeadGroupMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataProviderHeadGroupMenu));
            this.buttonBrowseHeadsFile = new System.Windows.Forms.Button();
            this.textBoxHeadsFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelColumn = new System.Windows.Forms.Label();
            this.buttonAddGroup = new System.Windows.Forms.Button();
            this.treeViewGroups = new System.Windows.Forms.TreeView();
            this.textBoxLayerEnd = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLayerStart = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonBrowseHeadsFile
            // 
            this.buttonBrowseHeadsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseHeadsFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonBrowseHeadsFile.Image")));
            this.buttonBrowseHeadsFile.Location = new System.Drawing.Point(438, 31);
            this.buttonBrowseHeadsFile.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBrowseHeadsFile.Name = "buttonBrowseHeadsFile";
            this.buttonBrowseHeadsFile.Size = new System.Drawing.Size(30, 30);
            this.buttonBrowseHeadsFile.TabIndex = 1;
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
            this.textBoxHeadsFile.Size = new System.Drawing.Size(430, 24);
            this.textBoxHeadsFile.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(330, 18);
            this.label1.TabIndex = 45;
            this.label1.Text = "Binary File of Head, Drawdown, or Concentration";
            // 
            // labelColumn
            // 
            this.labelColumn.AutoSize = true;
            this.labelColumn.Location = new System.Drawing.Point(4, 132);
            this.labelColumn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelColumn.Name = "labelColumn";
            this.labelColumn.Size = new System.Drawing.Size(367, 18);
            this.labelColumn.TabIndex = 48;
            this.labelColumn.Text = "Cell Groups (leave empty to use all cells in layer range)";
            // 
            // buttonAddGroup
            // 
            this.buttonAddGroup.Location = new System.Drawing.Point(9, 342);
            this.buttonAddGroup.Margin = new System.Windows.Forms.Padding(4);
            this.buttonAddGroup.Name = "buttonAddGroup";
            this.buttonAddGroup.Size = new System.Drawing.Size(231, 32);
            this.buttonAddGroup.TabIndex = 5;
            this.buttonAddGroup.Text = "Add New Cell Group File";
            this.buttonAddGroup.UseVisualStyleBackColor = true;
            this.buttonAddGroup.Click += new System.EventHandler(this.buttonAddGroup_Click);
            // 
            // treeViewGroups
            // 
            this.treeViewGroups.CheckBoxes = true;
            this.treeViewGroups.Location = new System.Drawing.Point(8, 160);
            this.treeViewGroups.Margin = new System.Windows.Forms.Padding(4);
            this.treeViewGroups.Name = "treeViewGroups";
            this.treeViewGroups.Size = new System.Drawing.Size(460, 166);
            this.treeViewGroups.TabIndex = 4;
            // 
            // textBoxLayerEnd
            // 
            this.textBoxLayerEnd.Location = new System.Drawing.Point(96, 96);
            this.textBoxLayerEnd.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLayerEnd.Name = "textBoxLayerEnd";
            this.textBoxLayerEnd.Size = new System.Drawing.Size(69, 24);
            this.textBoxLayerEnd.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(91, 68);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 18);
            this.label5.TabIndex = 83;
            this.label5.Text = "Layer End";
            // 
            // textBoxLayerStart
            // 
            this.textBoxLayerStart.Location = new System.Drawing.Point(9, 96);
            this.textBoxLayerStart.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLayerStart.Name = "textBoxLayerStart";
            this.textBoxLayerStart.Size = new System.Drawing.Size(74, 24);
            this.textBoxLayerStart.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 68);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 18);
            this.label4.TabIndex = 81;
            this.label4.Text = "Layer Start";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(186, 96);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(199, 26);
            this.comboBox1.TabIndex = 84;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(183, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 18);
            this.label2.TabIndex = 85;
            this.label2.Text = "Data Identifier";
            // 
            // DataProviderHeadGroupMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBoxLayerEnd);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxLayerStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.treeViewGroups);
            this.Controls.Add(this.buttonAddGroup);
            this.Controls.Add(this.labelColumn);
            this.Controls.Add(this.buttonBrowseHeadsFile);
            this.Controls.Add(this.textBoxHeadsFile);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DataProviderHeadGroupMenu";
            this.Size = new System.Drawing.Size(476, 385);
            this.Load += new System.EventHandler(this.DataProviderHeadGroupMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowseHeadsFile;
        private System.Windows.Forms.TextBox textBoxHeadsFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelColumn;
        private System.Windows.Forms.Button buttonAddGroup;
        private System.Windows.Forms.TreeView treeViewGroups;
        private System.Windows.Forms.TextBox textBoxLayerEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLayerStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
    }
}
