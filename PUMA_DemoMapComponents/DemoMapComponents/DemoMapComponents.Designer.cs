namespace DemoMapComponents
{
    partial class DemoMapComponents
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
            this.panelMapControl = new System.Windows.Forms.Panel();
            this.panelIndexMap = new System.Windows.Forms.Panel();
            this.panelMapLegend = new System.Windows.Forms.Panel();
            this.lblMapViewer = new System.Windows.Forms.Label();
            this.lblMapLegend = new System.Windows.Forms.Label();
            this.lblIndexMap = new System.Windows.Forms.Label();
            this.lblHeadFile = new System.Windows.Forms.Label();
            this.txtHeadFile = new System.Windows.Forms.TextBox();
            this.btnBrowseHeadFiles = new System.Windows.Forms.Button();
            this.btnEditContourData = new System.Windows.Forms.Button();
            this.btnZoomFullExtent = new System.Windows.Forms.Button();
            this.btnZoomToGrid = new System.Windows.Forms.Button();
            this.btnExportContours = new System.Windows.Forms.Button();
            this.btnImportShapefile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panelMapControl
            // 
            this.panelMapControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMapControl.BackColor = System.Drawing.Color.White;
            this.panelMapControl.Location = new System.Drawing.Point(12, 94);
            this.panelMapControl.Name = "panelMapControl";
            this.panelMapControl.Size = new System.Drawing.Size(673, 547);
            this.panelMapControl.TabIndex = 0;
            // 
            // panelIndexMap
            // 
            this.panelIndexMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelIndexMap.BackColor = System.Drawing.Color.White;
            this.panelIndexMap.Location = new System.Drawing.Point(691, 453);
            this.panelIndexMap.Name = "panelIndexMap";
            this.panelIndexMap.Size = new System.Drawing.Size(252, 188);
            this.panelIndexMap.TabIndex = 1;
            // 
            // panelMapLegend
            // 
            this.panelMapLegend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMapLegend.BackColor = System.Drawing.Color.White;
            this.panelMapLegend.Location = new System.Drawing.Point(691, 94);
            this.panelMapLegend.Name = "panelMapLegend";
            this.panelMapLegend.Size = new System.Drawing.Size(252, 332);
            this.panelMapLegend.TabIndex = 2;
            // 
            // lblMapViewer
            // 
            this.lblMapViewer.AutoSize = true;
            this.lblMapViewer.Location = new System.Drawing.Point(12, 78);
            this.lblMapViewer.Name = "lblMapViewer";
            this.lblMapViewer.Size = new System.Drawing.Size(28, 13);
            this.lblMapViewer.TabIndex = 3;
            this.lblMapViewer.Text = "Map";
            // 
            // lblMapLegend
            // 
            this.lblMapLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMapLegend.AutoSize = true;
            this.lblMapLegend.Location = new System.Drawing.Point(688, 78);
            this.lblMapLegend.Name = "lblMapLegend";
            this.lblMapLegend.Size = new System.Drawing.Size(67, 13);
            this.lblMapLegend.TabIndex = 4;
            this.lblMapLegend.Text = "Map Legend";
            // 
            // lblIndexMap
            // 
            this.lblIndexMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIndexMap.AutoSize = true;
            this.lblIndexMap.Location = new System.Drawing.Point(688, 437);
            this.lblIndexMap.Name = "lblIndexMap";
            this.lblIndexMap.Size = new System.Drawing.Size(57, 13);
            this.lblIndexMap.TabIndex = 5;
            this.lblIndexMap.Text = "Index Map";
            // 
            // lblHeadFile
            // 
            this.lblHeadFile.AutoSize = true;
            this.lblHeadFile.Location = new System.Drawing.Point(12, 9);
            this.lblHeadFile.Name = "lblHeadFile";
            this.lblHeadFile.Size = new System.Drawing.Size(114, 13);
            this.lblHeadFile.TabIndex = 6;
            this.lblHeadFile.Text = "MODFLOW Head File:";
            // 
            // txtHeadFile
            // 
            this.txtHeadFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeadFile.Location = new System.Drawing.Point(132, 6);
            this.txtHeadFile.Name = "txtHeadFile";
            this.txtHeadFile.ReadOnly = true;
            this.txtHeadFile.Size = new System.Drawing.Size(734, 20);
            this.txtHeadFile.TabIndex = 7;
            // 
            // btnBrowseHeadFiles
            // 
            this.btnBrowseHeadFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseHeadFiles.Location = new System.Drawing.Point(868, 4);
            this.btnBrowseHeadFiles.Name = "btnBrowseHeadFiles";
            this.btnBrowseHeadFiles.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseHeadFiles.TabIndex = 8;
            this.btnBrowseHeadFiles.Text = "Browse";
            this.btnBrowseHeadFiles.UseVisualStyleBackColor = true;
            this.btnBrowseHeadFiles.Click += new System.EventHandler(this.btnBrowseHeadFiles_Click);
            // 
            // btnEditContourData
            // 
            this.btnEditContourData.Location = new System.Drawing.Point(447, 40);
            this.btnEditContourData.Name = "btnEditContourData";
            this.btnEditContourData.Size = new System.Drawing.Size(114, 23);
            this.btnEditContourData.TabIndex = 9;
            this.btnEditContourData.Text = "Edit Contour Data";
            this.btnEditContourData.UseVisualStyleBackColor = true;
            this.btnEditContourData.Click += new System.EventHandler(this.btnEditContourData_Click);
            // 
            // btnZoomFullExtent
            // 
            this.btnZoomFullExtent.Location = new System.Drawing.Point(15, 40);
            this.btnZoomFullExtent.Name = "btnZoomFullExtent";
            this.btnZoomFullExtent.Size = new System.Drawing.Size(102, 23);
            this.btnZoomFullExtent.TabIndex = 11;
            this.btnZoomFullExtent.Text = "Full Extent";
            this.btnZoomFullExtent.UseVisualStyleBackColor = true;
            this.btnZoomFullExtent.Click += new System.EventHandler(this.btnZoomFullExtent_Click);
            // 
            // btnZoomToGrid
            // 
            this.btnZoomToGrid.Location = new System.Drawing.Point(123, 40);
            this.btnZoomToGrid.Name = "btnZoomToGrid";
            this.btnZoomToGrid.Size = new System.Drawing.Size(102, 23);
            this.btnZoomToGrid.TabIndex = 12;
            this.btnZoomToGrid.Text = "Zoom to Grid";
            this.btnZoomToGrid.UseVisualStyleBackColor = true;
            this.btnZoomToGrid.Click += new System.EventHandler(this.btnZoomToGrid_Click);
            // 
            // btnExportContours
            // 
            this.btnExportContours.Location = new System.Drawing.Point(231, 40);
            this.btnExportContours.Name = "btnExportContours";
            this.btnExportContours.Size = new System.Drawing.Size(102, 23);
            this.btnExportContours.TabIndex = 13;
            this.btnExportContours.Text = "Export Contours";
            this.btnExportContours.UseVisualStyleBackColor = true;
            this.btnExportContours.Click += new System.EventHandler(this.btnExportContours_Click);
            // 
            // btnImportShapefile
            // 
            this.btnImportShapefile.Location = new System.Drawing.Point(339, 40);
            this.btnImportShapefile.Name = "btnImportShapefile";
            this.btnImportShapefile.Size = new System.Drawing.Size(102, 23);
            this.btnImportShapefile.TabIndex = 14;
            this.btnImportShapefile.Text = "Import Shapefile";
            this.btnImportShapefile.UseVisualStyleBackColor = true;
            this.btnImportShapefile.Click += new System.EventHandler(this.btnImportShapefile_Click);
            // 
            // DemoMapComponents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 663);
            this.Controls.Add(this.btnImportShapefile);
            this.Controls.Add(this.btnExportContours);
            this.Controls.Add(this.btnZoomToGrid);
            this.Controls.Add(this.btnZoomFullExtent);
            this.Controls.Add(this.btnEditContourData);
            this.Controls.Add(this.btnBrowseHeadFiles);
            this.Controls.Add(this.txtHeadFile);
            this.Controls.Add(this.lblHeadFile);
            this.Controls.Add(this.lblIndexMap);
            this.Controls.Add(this.lblMapLegend);
            this.Controls.Add(this.lblMapViewer);
            this.Controls.Add(this.panelMapLegend);
            this.Controls.Add(this.panelIndexMap);
            this.Controls.Add(this.panelMapControl);
            this.Name = "DemoMapComponents";
            this.Text = "Demo Map Components";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMapControl;
        private System.Windows.Forms.Panel panelIndexMap;
        private System.Windows.Forms.Panel panelMapLegend;
        private System.Windows.Forms.Label lblMapViewer;
        private System.Windows.Forms.Label lblMapLegend;
        private System.Windows.Forms.Label lblIndexMap;
        private System.Windows.Forms.Label lblHeadFile;
        private System.Windows.Forms.TextBox txtHeadFile;
        private System.Windows.Forms.Button btnBrowseHeadFiles;
        private System.Windows.Forms.Button btnEditContourData;
        private System.Windows.Forms.Button btnZoomFullExtent;
        private System.Windows.Forms.Button btnZoomToGrid;
        private System.Windows.Forms.Button btnExportContours;
        private System.Windows.Forms.Button btnImportShapefile;
    }
}

