namespace ScenarioManager
{
    partial class GroupBoxLayerAssignment
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckbxUseAsMaster = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbxLayerAttribute = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbxNpkgop = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckbxUseAsMaster);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbxLayerAttribute);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbxNpkgop);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(357, 274);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Package Options";
            // 
            // ckbxUseAsMaster
            // 
            this.ckbxUseAsMaster.Location = new System.Drawing.Point(12, 29);
            this.ckbxUseAsMaster.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ckbxUseAsMaster.Name = "ckbxUseAsMaster";
            this.ckbxUseAsMaster.Size = new System.Drawing.Size(316, 58);
            this.ckbxUseAsMaster.TabIndex = 0;
            this.ckbxUseAsMaster.Text = "Use this feature set for assigning package options";
            this.ckbxUseAsMaster.UseVisualStyleBackColor = true;
            this.ckbxUseAsMaster.CheckedChanged += new System.EventHandler(this.ckbxUseAsMaster_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(286, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Attribute containing layer number";
            // 
            // cmbxLayerAttribute
            // 
            this.cmbxLayerAttribute.FormattingEnabled = true;
            this.cmbxLayerAttribute.Location = new System.Drawing.Point(11, 190);
            this.cmbxLayerAttribute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbxLayerAttribute.Name = "cmbxLayerAttribute";
            this.cmbxLayerAttribute.Size = new System.Drawing.Size(281, 30);
            this.cmbxLayerAttribute.TabIndex = 4;
            this.cmbxLayerAttribute.SelectedIndexChanged += new System.EventHandler(this.cmbxLayerAttribute_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 95);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(247, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "NRCHOP - Recharge option";
            // 
            // cmbxNpkgop
            // 
            this.cmbxNpkgop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbxNpkgop.FormattingEnabled = true;
            this.cmbxNpkgop.Items.AddRange(new object[] {
            "1: Top layer",
            "2: Assign by polygons",
            "3: Highest active cell"});
            this.cmbxNpkgop.Location = new System.Drawing.Point(11, 122);
            this.cmbxNpkgop.Margin = new System.Windows.Forms.Padding(4);
            this.cmbxNpkgop.MaxDropDownItems = 3;
            this.cmbxNpkgop.Name = "cmbxNpkgop";
            this.cmbxNpkgop.Size = new System.Drawing.Size(281, 30);
            this.cmbxNpkgop.TabIndex = 3;
            this.cmbxNpkgop.SelectedIndexChanged += new System.EventHandler(this.cbxNpkgop_SelectedIndexChanged);
            // 
            // GroupBoxLayerAssignment
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GroupBoxLayerAssignment";
            this.Size = new System.Drawing.Size(357, 274);
            this.Load += new System.EventHandler(this.GroupBoxLayerAssignment_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbxNpkgop;
        private System.Windows.Forms.ComboBox cmbxLayerAttribute;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ckbxUseAsMaster;
    }
}
