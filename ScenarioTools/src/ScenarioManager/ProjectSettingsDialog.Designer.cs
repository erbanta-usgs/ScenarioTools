namespace ScenarioManager
{
    partial class ProjectSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectSettingsDialog));
            this.cbNameFiles = new System.Windows.Forms.ComboBox();
            this.lblNameFile = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBrowseNamefile = new System.Windows.Forms.Button();
            this.btnRemoveFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbModflowExe = new System.Windows.Forms.TextBox();
            this.btnBrowseModflowExe = new System.Windows.Forms.Button();
            this.udMaxThreads = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tbBackgroundImageFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowseImageFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxThreads)).BeginInit();
            this.SuspendLayout();
            // 
            // cbNameFiles
            // 
            this.cbNameFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNameFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNameFiles.FormattingEnabled = true;
            this.cbNameFiles.Location = new System.Drawing.Point(4, 124);
            this.cbNameFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbNameFiles.MaxDropDownItems = 20;
            this.cbNameFiles.Name = "cbNameFiles";
            this.cbNameFiles.Size = new System.Drawing.Size(735, 30);
            this.cbNameFiles.TabIndex = 3;
            this.cbNameFiles.SelectedIndexChanged += new System.EventHandler(this.cbNameFiles_SelectedIndexChanged);
            // 
            // lblNameFile
            // 
            this.lblNameFile.AutoSize = true;
            this.lblNameFile.Location = new System.Drawing.Point(4, 96);
            this.lblNameFile.Name = "lblNameFile";
            this.lblNameFile.Size = new System.Drawing.Size(267, 24);
            this.lblNameFile.TabIndex = 5;
            this.lblNameFile.Text = "Master MODFLOW Name File:";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(8, 323);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 30);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(99, 323);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 30);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnBrowseNamefile
            // 
            this.btnBrowseNamefile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseNamefile.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseNamefile.Image")));
            this.btnBrowseNamefile.Location = new System.Drawing.Point(740, 124);
            this.btnBrowseNamefile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBrowseNamefile.Name = "btnBrowseNamefile";
            this.btnBrowseNamefile.Size = new System.Drawing.Size(30, 30);
            this.btnBrowseNamefile.TabIndex = 4;
            this.btnBrowseNamefile.UseVisualStyleBackColor = true;
            this.btnBrowseNamefile.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnRemoveFile
            // 
            this.btnRemoveFile.Location = new System.Drawing.Point(300, 90);
            this.btnRemoveFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRemoveFile.Name = "btnRemoveFile";
            this.btnRemoveFile.Size = new System.Drawing.Size(233, 30);
            this.btnRemoveFile.TabIndex = 2;
            this.btnRemoveFile.Text = "Remove File From List";
            this.btnRemoveFile.UseVisualStyleBackColor = true;
            this.btnRemoveFile.Click += new System.EventHandler(this.btnRemoveFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(346, 24);
            this.label1.TabIndex = 10;
            this.label1.Text = "Location of MODFLOW Executable File:";
            // 
            // tbModflowExe
            // 
            this.tbModflowExe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbModflowExe.Location = new System.Drawing.Point(4, 40);
            this.tbModflowExe.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbModflowExe.Name = "tbModflowExe";
            this.tbModflowExe.Size = new System.Drawing.Size(735, 28);
            this.tbModflowExe.TabIndex = 0;
            // 
            // btnBrowseModflowExe
            // 
            this.btnBrowseModflowExe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseModflowExe.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseModflowExe.Image")));
            this.btnBrowseModflowExe.Location = new System.Drawing.Point(740, 39);
            this.btnBrowseModflowExe.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBrowseModflowExe.Name = "btnBrowseModflowExe";
            this.btnBrowseModflowExe.Size = new System.Drawing.Size(30, 30);
            this.btnBrowseModflowExe.TabIndex = 1;
            this.btnBrowseModflowExe.UseVisualStyleBackColor = true;
            this.btnBrowseModflowExe.Click += new System.EventHandler(this.btnBrowseModflowExe_Click);
            // 
            // udMaxThreads
            // 
            this.udMaxThreads.Location = new System.Drawing.Point(446, 180);
            this.udMaxThreads.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.udMaxThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMaxThreads.Name = "udMaxThreads";
            this.udMaxThreads.Size = new System.Drawing.Size(55, 28);
            this.udMaxThreads.TabIndex = 5;
            this.udMaxThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 183);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(440, 24);
            this.label2.TabIndex = 14;
            this.label2.Text = "Maximum Number of Simultaneous Scenario Runs:";
            // 
            // tbBackgroundImageFile
            // 
            this.tbBackgroundImageFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBackgroundImageFile.Location = new System.Drawing.Point(4, 266);
            this.tbBackgroundImageFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbBackgroundImageFile.Name = "tbBackgroundImageFile";
            this.tbBackgroundImageFile.Size = new System.Drawing.Size(735, 28);
            this.tbBackgroundImageFile.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 238);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(343, 24);
            this.label3.TabIndex = 16;
            this.label3.Text = "Georeferenced Background Image File:";
            // 
            // btnBrowseImageFile
            // 
            this.btnBrowseImageFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseImageFile.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseImageFile.Image")));
            this.btnBrowseImageFile.Location = new System.Drawing.Point(740, 264);
            this.btnBrowseImageFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBrowseImageFile.Name = "btnBrowseImageFile";
            this.btnBrowseImageFile.Size = new System.Drawing.Size(30, 30);
            this.btnBrowseImageFile.TabIndex = 7;
            this.btnBrowseImageFile.UseVisualStyleBackColor = true;
            this.btnBrowseImageFile.Click += new System.EventHandler(this.btnBrowseImageFile_Click);
            // 
            // ProjectSettingsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(782, 370);
            this.Controls.Add(this.btnBrowseImageFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbBackgroundImageFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.udMaxThreads);
            this.Controls.Add(this.btnBrowseModflowExe);
            this.Controls.Add(this.tbModflowExe);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRemoveFile);
            this.Controls.Add(this.btnBrowseNamefile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblNameFile);
            this.Controls.Add(this.cbNameFiles);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(2248, 445);
            this.MinimumSize = new System.Drawing.Size(553, 404);
            this.Name = "ProjectSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scenario Manager Project Settings";
            this.Load += new System.EventHandler(this.ProjectSettingsDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.udMaxThreads)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbNameFiles;
        private System.Windows.Forms.Label lblNameFile;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBrowseNamefile;
        private System.Windows.Forms.Button btnRemoveFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbModflowExe;
        private System.Windows.Forms.Button btnBrowseModflowExe;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown udMaxThreads;
        private System.Windows.Forms.TextBox tbBackgroundImageFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowseImageFile;

    }
}