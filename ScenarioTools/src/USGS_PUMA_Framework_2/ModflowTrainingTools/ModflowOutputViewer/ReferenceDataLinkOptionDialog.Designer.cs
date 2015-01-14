namespace ModflowOutputViewer
{
    partial class ReferenceDataLinkOptionDialog
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
            this.lblReferenceFile = new System.Windows.Forms.Label();
            this.cboLinkOption = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSpecifiedInfo = new System.Windows.Forms.Label();
            this.btnSelectSpecifiedInfo = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtReferenceFile = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblReferenceFile
            // 
            this.lblReferenceFile.AutoSize = true;
            this.lblReferenceFile.Location = new System.Drawing.Point(12, 9);
            this.lblReferenceFile.Name = "lblReferenceFile";
            this.lblReferenceFile.Size = new System.Drawing.Size(76, 13);
            this.lblReferenceFile.TabIndex = 0;
            this.lblReferenceFile.Text = "Reference file:";
            // 
            // cboLinkOption
            // 
            this.cboLinkOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLinkOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLinkOption.FormattingEnabled = true;
            this.cboLinkOption.Location = new System.Drawing.Point(16, 80);
            this.cboLinkOption.Name = "cboLinkOption";
            this.cboLinkOption.Size = new System.Drawing.Size(319, 21);
            this.cboLinkOption.TabIndex = 1;
            this.cboLinkOption.SelectedIndexChanged += new System.EventHandler(this.cboLinkOption_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Link option:";
            // 
            // lblSpecifiedInfo
            // 
            this.lblSpecifiedInfo.AutoSize = true;
            this.lblSpecifiedInfo.Location = new System.Drawing.Point(81, 112);
            this.lblSpecifiedInfo.Name = "lblSpecifiedInfo";
            this.lblSpecifiedInfo.Size = new System.Drawing.Size(136, 13);
            this.lblSpecifiedInfo.TabIndex = 3;
            this.lblSpecifiedInfo.Text = "Period xx, Step yy, Layer zz";
            // 
            // btnSelectSpecifiedInfo
            // 
            this.btnSelectSpecifiedInfo.Location = new System.Drawing.Point(16, 107);
            this.btnSelectSpecifiedInfo.Name = "btnSelectSpecifiedInfo";
            this.btnSelectSpecifiedInfo.Size = new System.Drawing.Size(59, 23);
            this.btnSelectSpecifiedInfo.TabIndex = 4;
            this.btnSelectSpecifiedInfo.Text = "Select";
            this.btnSelectSpecifiedInfo.UseVisualStyleBackColor = true;
            this.btnSelectSpecifiedInfo.Click += new System.EventHandler(this.btnSpecifiedInfo_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(179, 141);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(260, 141);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtReferenceFile
            // 
            this.txtReferenceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReferenceFile.Location = new System.Drawing.Point(16, 25);
            this.txtReferenceFile.Name = "txtReferenceFile";
            this.txtReferenceFile.ReadOnly = true;
            this.txtReferenceFile.Size = new System.Drawing.Size(310, 20);
            this.txtReferenceFile.TabIndex = 7;
            // 
            // ReferenceDataLinkOptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 174);
            this.Controls.Add(this.txtReferenceFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectSpecifiedInfo);
            this.Controls.Add(this.lblSpecifiedInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboLinkOption);
            this.Controls.Add(this.lblReferenceFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReferenceDataLinkOptionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reference Layer Link Option";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblReferenceFile;
        private System.Windows.Forms.ComboBox cboLinkOption;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSpecifiedInfo;
        private System.Windows.Forms.Button btnSelectSpecifiedInfo;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtReferenceFile;
    }
}