namespace ScenarioTools.Data_Providers
{
    partial class DataProviderObservedSeriesMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataProviderObservedSeriesMenu));
            this.buttonBrowseFile = new System.Windows.Forms.Button();
            this.textBoxFile = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxSeries = new System.Windows.Forms.ComboBox();
            this.cbxIncludeTimeInTable = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonBrowseFile
            // 
            this.buttonBrowseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonBrowseFile.Image")));
            this.buttonBrowseFile.Location = new System.Drawing.Point(344, 31);
            this.buttonBrowseFile.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBrowseFile.Name = "buttonBrowseFile";
            this.buttonBrowseFile.Size = new System.Drawing.Size(30, 30);
            this.buttonBrowseFile.TabIndex = 1;
            this.buttonBrowseFile.UseVisualStyleBackColor = true;
            this.buttonBrowseFile.Click += new System.EventHandler(this.buttonBrowseFile_Click);
            // 
            // textBoxFile
            // 
            this.textBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFile.Location = new System.Drawing.Point(8, 32);
            this.textBoxFile.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Size = new System.Drawing.Size(335, 28);
            this.textBoxFile.TabIndex = 0;
            this.textBoxFile.TextChanged += new System.EventHandler(this.textBoxFile_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 4);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(192, 24);
            this.label11.TabIndex = 64;
            this.label11.Text = "Borehole Sample File";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 68);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 24);
            this.label1.TabIndex = 67;
            this.label1.Text = "Series";
            // 
            // comboBoxSeries
            // 
            this.comboBoxSeries.FormattingEnabled = true;
            this.comboBoxSeries.Location = new System.Drawing.Point(8, 96);
            this.comboBoxSeries.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxSeries.Name = "comboBoxSeries";
            this.comboBoxSeries.Size = new System.Drawing.Size(366, 30);
            this.comboBoxSeries.TabIndex = 2;
            // 
            // cbxIncludeTimeInTable
            // 
            this.cbxIncludeTimeInTable.AutoSize = true;
            this.cbxIncludeTimeInTable.Location = new System.Drawing.Point(10, 132);
            this.cbxIncludeTimeInTable.Margin = new System.Windows.Forms.Padding(4);
            this.cbxIncludeTimeInTable.Name = "cbxIncludeTimeInTable";
            this.cbxIncludeTimeInTable.Size = new System.Drawing.Size(215, 28);
            this.cbxIncludeTimeInTable.TabIndex = 3;
            this.cbxIncludeTimeInTable.Text = "Include Time in Table";
            this.cbxIncludeTimeInTable.UseVisualStyleBackColor = true;
            this.cbxIncludeTimeInTable.Visible = false;
            // 
            // DataProviderObservedSeriesMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbxIncludeTimeInTable);
            this.Controls.Add(this.comboBoxSeries);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonBrowseFile);
            this.Controls.Add(this.textBoxFile);
            this.Controls.Add(this.label11);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DataProviderObservedSeriesMenu";
            this.Size = new System.Drawing.Size(384, 166);
            this.Load += new System.EventHandler(this.DataProviderObservedSeriesMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowseFile;
        private System.Windows.Forms.TextBox textBoxFile;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxSeries;
        private System.Windows.Forms.CheckBox cbxIncludeTimeInTable;
    }
}
