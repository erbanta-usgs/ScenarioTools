namespace ScenarioTools.Dialogs
{
    partial class DataSeriesMenuContourBox
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
            this.textBoxContourInterval = new System.Windows.Forms.TextBox();
            this.labelContourInterval = new System.Windows.Forms.Label();
            this.textBoxStartingContour = new System.Windows.Forms.TextBox();
            this.labelStartingContour = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonAutomatic = new System.Windows.Forms.RadioButton();
            this.radioButtonEqualInterval = new System.Windows.Forms.RadioButton();
            this.radioButtonListOfValues = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxContourInterval
            // 
            this.textBoxContourInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxContourInterval.Location = new System.Drawing.Point(6, 203);
            this.textBoxContourInterval.Name = "textBoxContourInterval";
            this.textBoxContourInterval.Size = new System.Drawing.Size(236, 28);
            this.textBoxContourInterval.TabIndex = 4;
            // 
            // labelContourInterval
            // 
            this.labelContourInterval.AutoSize = true;
            this.labelContourInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContourInterval.Location = new System.Drawing.Point(2, 176);
            this.labelContourInterval.Name = "labelContourInterval";
            this.labelContourInterval.Size = new System.Drawing.Size(141, 24);
            this.labelContourInterval.TabIndex = 8;
            this.labelContourInterval.Text = "Contour Interval";
            // 
            // textBoxStartingContour
            // 
            this.textBoxStartingContour.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStartingContour.Location = new System.Drawing.Point(6, 141);
            this.textBoxStartingContour.Name = "textBoxStartingContour";
            this.textBoxStartingContour.Size = new System.Drawing.Size(236, 28);
            this.textBoxStartingContour.TabIndex = 3;
            // 
            // labelStartingContour
            // 
            this.labelStartingContour.AutoSize = true;
            this.labelStartingContour.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStartingContour.Location = new System.Drawing.Point(2, 114);
            this.labelStartingContour.Name = "labelStartingContour";
            this.labelStartingContour.Size = new System.Drawing.Size(144, 24);
            this.labelStartingContour.TabIndex = 6;
            this.labelStartingContour.Text = "Starting Contour";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonAutomatic);
            this.groupBox1.Controls.Add(this.radioButtonEqualInterval);
            this.groupBox1.Controls.Add(this.radioButtonListOfValues);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 107);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Contour Interval";
            // 
            // radioButtonAutomatic
            // 
            this.radioButtonAutomatic.AutoSize = true;
            this.radioButtonAutomatic.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonAutomatic.Location = new System.Drawing.Point(8, 23);
            this.radioButtonAutomatic.Name = "radioButtonAutomatic";
            this.radioButtonAutomatic.Size = new System.Drawing.Size(114, 28);
            this.radioButtonAutomatic.TabIndex = 0;
            this.radioButtonAutomatic.TabStop = true;
            this.radioButtonAutomatic.Text = "Automatic";
            this.radioButtonAutomatic.UseVisualStyleBackColor = true;
            this.radioButtonAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonAutomatic_CheckedChanged);
            // 
            // radioButtonEqualInterval
            // 
            this.radioButtonEqualInterval.AutoSize = true;
            this.radioButtonEqualInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonEqualInterval.Location = new System.Drawing.Point(8, 48);
            this.radioButtonEqualInterval.Name = "radioButtonEqualInterval";
            this.radioButtonEqualInterval.Size = new System.Drawing.Size(144, 28);
            this.radioButtonEqualInterval.TabIndex = 1;
            this.radioButtonEqualInterval.TabStop = true;
            this.radioButtonEqualInterval.Text = "Equal Interval";
            this.radioButtonEqualInterval.UseVisualStyleBackColor = true;
            this.radioButtonEqualInterval.CheckedChanged += new System.EventHandler(this.radioButtonEqualInterval_CheckedChanged);
            // 
            // radioButtonListOfValues
            // 
            this.radioButtonListOfValues.AutoSize = true;
            this.radioButtonListOfValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonListOfValues.Location = new System.Drawing.Point(8, 73);
            this.radioButtonListOfValues.Name = "radioButtonListOfValues";
            this.radioButtonListOfValues.Size = new System.Drawing.Size(141, 28);
            this.radioButtonListOfValues.TabIndex = 2;
            this.radioButtonListOfValues.TabStop = true;
            this.radioButtonListOfValues.Text = "List of Values";
            this.radioButtonListOfValues.UseVisualStyleBackColor = true;
            this.radioButtonListOfValues.CheckedChanged += new System.EventHandler(this.radioButtonListOfValues_CheckedChanged);
            // 
            // DataSeriesMenuContourBox
            // 
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxContourInterval);
            this.Controls.Add(this.labelContourInterval);
            this.Controls.Add(this.textBoxStartingContour);
            this.Controls.Add(this.labelStartingContour);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DataSeriesMenuContourBox";
            this.Size = new System.Drawing.Size(258, 238);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxContourInterval;
        private System.Windows.Forms.Label labelContourInterval;
        private System.Windows.Forms.TextBox textBoxStartingContour;
        private System.Windows.Forms.Label labelStartingContour;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonEqualInterval;
        private System.Windows.Forms.RadioButton radioButtonListOfValues;
        private System.Windows.Forms.RadioButton radioButtonAutomatic;

    }
}
