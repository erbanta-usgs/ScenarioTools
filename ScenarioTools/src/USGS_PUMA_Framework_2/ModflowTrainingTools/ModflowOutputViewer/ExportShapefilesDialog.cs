﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using USGS.Puma.Core;
using USGS.Puma.IO;
using USGS.Puma.NTS;
using USGS.Puma.NTS.Features;
using USGS.Puma.FiniteDifference;
using USGS.Puma.UI;
using USGS.Puma.UI.MapViewer;
using GeoAPI.Geometries;

namespace ModflowOutputViewer
{
    public partial class ExportShapefilesDialog : Form
    {
        private bool _OK = false;

        public ExportShapefilesDialog() : this("") { }
        public ExportShapefilesDialog(string exportDirectory)
        {
            InitializeComponent();
            txtModelGridOutline.Text = "GridOutline";
            txtModelGridlines.Text = "Gridlines";
            txtContourLines.Text = "Contours";
            txtCurrentCellValues.Text = "CurrentValues";
            txtAnalysisCellValues.Text = "AnalysisValues";
            txtExportDirectory.Text = exportDirectory;
            _OK = false;
        }

        public string ExportDirectory
        {
            get { return txtExportDirectory.Text.Trim(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                { txtExportDirectory.Text = ""; }
                else
                { txtExportDirectory.Text = value.Trim(); }
            }

        }
        private FeatureLayer _GridOutlineLayer = null;
        public FeatureLayer GridOutlineLayer
        {
            get { return _GridOutlineLayer; }
            set 
            { 
                _GridOutlineLayer = value;
            }
        }

        private FeatureLayer _GridlinesLayer = null;
        public FeatureLayer GridlinesLayer
        {
            get { return _GridlinesLayer; }
            set { _GridlinesLayer = value; }
        }

        private FeatureLayer _ContourLayer = null;
        public FeatureLayer ContourLayer
        {
            get { return _ContourLayer; }
            set { _ContourLayer = value; }
        }

        private FeatureLayer _CurrentValuesLayer = null;
        public FeatureLayer CurrentValuesLayer
        {
            get { return _CurrentValuesLayer; }
            set { _CurrentValuesLayer = value; }
        }

        private FeatureLayer _AnalysisValuesLayer = null;
        public FeatureLayer AnalysisValuesLayer
        {
            get { return _AnalysisValuesLayer; }
            set { _AnalysisValuesLayer = value; }
        }


        private void ExportShapefiles()
        {
            int count = 0;
            string directory = txtExportDirectory.Text.Trim();

            textboxStatus.Text = "";
            textboxStatus.Refresh();

            if (chkGridOutline.Checked && GridOutlineLayer != null)
            {
                textboxStatus.Text += "Exporting grid outline shapefile ..." + Environment.NewLine;
                textboxStatus.Refresh();
                try
                {
                    ExportFeatureLayerAsShapefile(GridOutlineLayer, directory, txtModelGridOutline.Text.Trim(), false);
                    count += 1;
                }
                catch (Exception e)
                {
                    textboxStatus.Text += e.Message + Environment.NewLine;
                }
            }

            if (chkGridlines.Checked && GridlinesLayer!=null)
            {
                textboxStatus.Text += "Exporting grid outline shapefile ..." + Environment.NewLine;
                textboxStatus.Refresh();
                try
                {
                    ExportFeatureLayerAsShapefile(GridlinesLayer, directory, txtModelGridlines.Text.Trim(), false);
                    count += 1;
                }
                catch (Exception e)
                {
                    textboxStatus.Text += e.Message + Environment.NewLine;
                }
            }

            if (chkContours.Checked && ContourLayer != null)
            {
                textboxStatus.Text += "Exporting contour lines shapefile ..." + Environment.NewLine;
                textboxStatus.Refresh();
                try
                {
                    ExportFeatureLayerAsShapefile(ContourLayer, directory, txtContourLines.Text.Trim(), false);
                    count += 1;
                }
                catch (Exception e)
                {
                    textboxStatus.Text += e.Message + Environment.NewLine;
                }
            }

            if (chkCellValues.Checked && CurrentValuesLayer != null)
            {
                textboxStatus.Text += "Exporting current values shapefile ..." + Environment.NewLine;
                textboxStatus.Refresh();
                try
                {
                    ExportFeatureLayerAsShapefile(CurrentValuesLayer, directory, txtCurrentCellValues.Text.Trim(), true);
                    count += 1;
                }
                catch (Exception e)
                {
                    textboxStatus.Text += e.Message + Environment.NewLine;
                }
            }

            if (chkAnalysisCellValues.Checked && AnalysisValuesLayer != null)
            {
                textboxStatus.Text += "Exporting analysis values shapefile ..." + Environment.NewLine;
                textboxStatus.Refresh();
                try
                {
                    ExportFeatureLayerAsShapefile(AnalysisValuesLayer, directory, txtAnalysisCellValues.Text.Trim(), true);
                    count += 1;
                }
                catch (Exception e)
                {
                    textboxStatus.Text += e.Message + Environment.NewLine;
                }
            }

            textboxStatus.Text += count.ToString() + " shapefiles were exported.";
            textboxStatus.Refresh();

        }
        private void ExportFeatureLayerAsShapefile(FeatureLayer layer, string directory, string basename, bool useRenderArray)
        {
            if (layer != null)
            {
                if (layer.FeatureCount > 0)
                {
                    FeatureCollection fc = layer.GetFeatures();
                    if (useRenderArray)
                    {
                        bool hasValueField = false;
                        string[] names = fc[0].Attributes.GetNames();
                        for (int i = 0; i < names.Length; i++)
                        {
                            if (names[i] == "Value")
                            {
                                hasValueField = true;
                                break;
                            }
                        }
                        if (hasValueField)
                        {
                            ColorRampRenderer renderer = (ColorRampRenderer)(layer.Renderer);
                            for (int i = 0; i < fc.Count; i++)
                            {
                                fc[i].Attributes["Value"] = renderer.RenderArray[i];
                            }
                        }
                        else
                        { return; }
                    }
                    try
                    {
                        EsriShapefileIO.Export(fc, directory, basename);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception("No features to export.");
                }
            }
            else
            {
                throw new ArgumentNullException("The feature layer does not exist.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportShapefiles();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private bool DeleteFile(string filename)
        {
            try
            {
                System.IO.FileInfo finfo = new System.IO.FileInfo(filename);
                finfo.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DeleteShapefile(string directory, string basename)
        {
            string name = System.IO.Path.Combine(directory, basename);

            string filename = System.IO.Path.Combine(name, ".dbf");
            if (System.IO.File.Exists(filename))
            {
                if (!DeleteFile(filename))
                { return false; }
            }

            filename = System.IO.Path.Combine(name, ".sbn");
            if (System.IO.File.Exists(filename))
            {
                if (!DeleteFile(filename))
                { return false; }
            }

            filename = System.IO.Path.Combine(name, ".prj");
            if (System.IO.File.Exists(filename))
            {
                if (!DeleteFile(filename))
                { return false; }
            }

            filename = System.IO.Path.Combine(name, ".shx");
            if (System.IO.File.Exists(filename))
            {
                if (!DeleteFile(filename))
                { return false; }
            }

            filename = System.IO.Path.Combine(name, ".shp");
            if (System.IO.File.Exists(filename))
            {
                if (!DeleteFile(filename))
                { return false; }
            }

            filename = System.IO.Path.Combine(name, ".shp.xml");
            if (System.IO.File.Exists(filename))
            {
                if (!DeleteFile(filename))
                { return false; }
            }

            return true;


        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(ExportDirectory))
            { dialog.SelectedPath = ExportDirectory; }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ExportDirectory = dialog.SelectedPath;
            }
        }
    }
}
