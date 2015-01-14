namespace ScenarioManager
{
    partial class ScenarioIdDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScenarioIdDialog));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbScenarioID = new System.Windows.Forms.TextBox();
            this.tbExplanation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(129, 180);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(79, 31);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(217, 180);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(79, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tbScenarioID
            // 
            this.tbScenarioID.Location = new System.Drawing.Point(146, 142);
            this.tbScenarioID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbScenarioID.Name = "tbScenarioID";
            this.tbScenarioID.Size = new System.Drawing.Size(198, 24);
            this.tbScenarioID.TabIndex = 0;
            // 
            // tbExplanation
            // 
            this.tbExplanation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbExplanation.Location = new System.Drawing.Point(27, 17);
            this.tbExplanation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbExplanation.Multiline = true;
            this.tbExplanation.Name = "tbExplanation";
            this.tbExplanation.ReadOnly = true;
            this.tbExplanation.Size = new System.Drawing.Size(374, 117);
            this.tbExplanation.TabIndex = 3;
            this.tbExplanation.TabStop = false;
            this.tbExplanation.Text = "The Scenario ID is used in naming a MODFLOW name file, a batch file for running t" +
    "he scenario simulation, and a folder for input and output files that are generat" +
    "ed when the simulation is run.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Scenario ID:";
            // 
            // ScenarioIdDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(426, 226);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbExplanation);
            this.Controls.Add(this.tbScenarioID);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ScenarioIdDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scenario ID";
            this.Load += new System.EventHandler(this.ScenarioIdDialog_Load);
            this.Shown += new System.EventHandler(this.ScenarioIdDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbScenarioID;
        private System.Windows.Forms.TextBox tbExplanation;
        private System.Windows.Forms.Label label1;
    }
}