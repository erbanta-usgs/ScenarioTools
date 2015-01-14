using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ScenarioTools;
using ScenarioTools.DataClasses;
using ScenarioTools.Dialogs;
using ScenarioTools.Geometry;
using ScenarioTools.Reporting;

namespace ScenarioAnalyzer
{
    public partial class DocumentMenu : Form
    {
        #region Fields
        private SADocument _document;
        private bool _okToClose;
        private ScenarioTools.DataClasses.LengthReference.ModflowLengthUnit _modflowLengthUnit;

        public TemporalReference TemporalRef;
        public string BackgroundImageFile;
        public string GridShapefilePath;
        #endregion Fields

        #region Constructor
        public DocumentMenu(SADocument document)
        {
            InitializeComponent();

            // Store a reference to the report.
            this._document = document;
        }
        #endregion Constructor

        private void DocumentMenu_Load(object sender, EventArgs e)
        {
            _okToClose = true;

            // Populate the text boxes.
            // Name and author
            if (_document.ReportName != null)
            {
                textBoxName.Text = _document.ReportName;
            }
            if (_document.Author != null)
            {
                textBoxAuthor.Text = _document.Author;
            }

            // Global variables
            if (GlobalStaticVariables.BackgroundImageFile != null)
            {
                textBoxImageFilename.Text = GlobalStaticVariables.BackgroundImageFile;
            }
            if (ScenarioTools.Spatial.StaticObjects.GridShapefilePath != null)
            {
                textBoxGridShapefileName.Text = ScenarioTools.Spatial.StaticObjects.GridShapefilePath;
            }

            // Simulation start time and Modflow time and length units
            _modflowLengthUnit = _document.ModflowLengthUnit;
            TemporalRef = new TemporalReference(GlobalStaticVariables.GlobalTemporalReference);
            textBoxSimulationStartTime.Text = TemporalRef.SimulationStartTime.ToString();
            try
            {
                comboBoxTimeUnit.SelectedIndex = (int)TemporalRef.ModelTimeUnit - 1;
                comboBoxLengthUnit.SelectedIndex = (int)_document.ModflowLengthUnit - 1;
            }
            catch
            {
                comboBoxTimeUnit.SelectedIndex = 0;
                comboBoxLengthUnit.SelectedIndex = 1;
            }

            // Special numeric values
            textBoxHnoflo.Text = _document.HnofloText;
            textBoxHdry.Text = _document.HdryText;
            textBoxCinact.Text = _document.CinactText;

            // Populate the elements list box with the report elements.
            for (int i = 0; i < _document.NumElements; i++)
            {
                listBoxElements.Items.Add(_document.GetElement(i));
            }

            // Populate the extents list view
            int numExtentsNotInUse = 0;
            for (int i = 0; i < _document.Extents.Count; i++)
            {
                ListViewItem item = new ListViewItem(_document.GetExtent(i).ToString());
                item.Tag = _document.GetExtent(i);
                if (_document.IsInUse(_document.GetExtent(i)))
                {
                    item.ForeColor = Color.Silver;
                }
                else
                {
                    item.ForeColor = Color.Black;
                    numExtentsNotInUse++;
                }
                listViewExtents.Items.Add(item);
            }
            if (numExtentsNotInUse == 0)
            {
                labelInUse.Text = "All extents listed are in use. None may be deleted.";
            }
            else
            {
                labelInUse.Text = "Extents in gray are in use and may not be deleted.";
            }

            // Set up the Inactive Areas tab
            switch (_document.BlankingMode)
            {
                case MapEnums.BlankingMode.None:
                    {
                        radioButtonAsIs.Checked = true;
                        break;
                    }
                case MapEnums.BlankingMode.BySpecifiedLayer:
                    {
                        radioButtonBlankSpecifiedIboundLayer.Checked = true;
                        break;
                    }
                case MapEnums.BlankingMode.AnyLayerInactive:
                    {
                        radioButtonBlankIfAnyLayerInactive.Checked = true;
                        break;
                    }
                case MapEnums.BlankingMode.AllLayersInactive:
                    {
                        radioButtonBlankIfAllLayersInactive.Checked = true;
                        break;
                    }
            }
            numericUpDownBlankingLayer.Value = _document.BlankingLayer;
            numericUpDownBlankingLayer.Enabled = _document.BlankingMode == MapEnums.BlankingMode.BySpecifiedLayer;
            textBoxNameFile.Text = _document.NameFile;
            buttonBrowseForNameFile.Enabled = !radioButtonAsIs.Checked;
            textBoxNameFile.Enabled = !radioButtonAsIs.Checked;
            labelModflowNameFile.ForeColor = Color.Black;
            if (radioButtonAsIs.Checked)
            {
                labelModflowNameFile.ForeColor = Color.Gray;
            }
            _document.ChartsNeedRecompute = false;
            _document.MapsNeedRecompute = false;
            _document.TablesNeedRecompute = false;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Cursor cursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            enableControls(false);

            // Check validity of HNOFLO, HDRY, and CINACT
            try
            {
                float hnoflo = Convert.ToSingle(textBoxHnoflo.Text);
            }
            catch
            {
                MessageBox.Show("Error: Entry provided for HNOFLO cannot be converted to a number");
                _okToClose = false;
            }
            try
            {
                float hdry = Convert.ToSingle(textBoxHdry.Text);
            }
            catch
            {
                MessageBox.Show("Error: Entry provided for HDRY cannot be converted to a number");
                _okToClose = false;
            }
            try
            {
                float cinact = Convert.ToSingle(textBoxCinact.Text);
            }
            catch
            {
                MessageBox.Show("Error: Entry provided for CINACT cannot be converted to a number");
                _okToClose = false;
            }

            if (!radioButtonAsIs.Checked)
            {
                _document.NameFile = textBoxNameFile.Text;
                _okToClose = _document.ModflowBasicDataDefined;
            }

            if (_okToClose)
            {
                // Save HNOFLO, HDRY and CINACT
                _document.HnofloText = textBoxHnoflo.Text;
                _document.HdryText = textBoxHdry.Text;
                _document.CinactText = textBoxCinact.Text;

                // Save the name and author and file names.
                _document.ReportName = textBoxName.Text;
                _document.Author = textBoxAuthor.Text;

                // Save the global variables
                BackgroundImageFile = textBoxImageFilename.Text;
                GridShapefilePath = textBoxGridShapefileName.Text;

                // Assign Modflow length unit
                _document.ModflowLengthUnit = _modflowLengthUnit;

                // Assign settings related to blanking of inactive cells on maps
                if (radioButtonAsIs.Checked)
                {
                    _document.BlankingMode = MapEnums.BlankingMode.None; // 0
                }
                else if (radioButtonBlankSpecifiedIboundLayer.Checked)
                {
                    _document.BlankingMode = MapEnums.BlankingMode.BySpecifiedLayer; // 1
                    _document.BlankingLayer = Convert.ToInt32(numericUpDownBlankingLayer.Value);
                }
                else if (radioButtonBlankIfAnyLayerInactive.Checked)
                {
                    _document.BlankingMode = MapEnums.BlankingMode.AnyLayerInactive; // 2
                }
                else if (radioButtonBlankIfAllLayersInactive.Checked)
                {
                    _document.BlankingMode = MapEnums.BlankingMode.AllLayersInactive; // 3
                }
                GlobalStaticVariables.DefineBlanking(_document.BlankingMode, _document.BlankingLayer);

                // Make form invisible before generating image and importing grid, which
                // can be time-consuming.  This lets user see progress bar on main form.
                this.Visible = false;

                // Save the elements.
                _document.ClearElements();
                for (int i = 0; i < listBoxElements.Items.Count; i++)
                {
                    _document.AddElement((IReportElement)listBoxElements.Items[i]);
                }

                // Save the extents
                _document.ClearExtents();
                bool modelGridLoaded = false;
                string modelGridExtentName = "";
                if (_document.ModelGridExtent != null)
                {
                    _document.AddExtent(_document.ModelGridExtent);
                    modelGridExtentName = _document.ModelGridExtent.Name;
                    modelGridLoaded = true;
                }
                _document.AddExtentOfBackgroundImage();
                for (int i = 0; i < listViewExtents.Items.Count; i++)
                {
                    if ((!modelGridLoaded || ((Extent)listViewExtents.Items[i].Tag).Name != modelGridExtentName) && 
                        ((Extent)listViewExtents.Items[i].Tag).Name != WorkspaceUtil.BACKGROUND_IMAGE_EXTENT_NAME)
                    {
                        _document.AddExtent((Extent)listViewExtents.Items[i].Tag);
                    }
                }
            }

            Cursor.Current = cursor;
            enableControls(true);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            // If there is a selected element in the list box (not at index zero), move it up one index.
            if (listBoxElements.SelectedIndex > 0)
            {
                int index = listBoxElements.SelectedIndex;
                object item = listBoxElements.Items[index];
                listBoxElements.Items.RemoveAt(index);
                listBoxElements.Items.Insert(index - 1, item);

                // Select the item that was previously selected.
                listBoxElements.SelectedIndex = index - 1;
            }
        }

        private void buttonRemoveElement_Click(object sender, EventArgs e)
        {
            // If there is an item selected in the list box, remove it.
            if (listBoxElements.SelectedIndex >= 0)
            {
                listBoxElements.Items.RemoveAt(listBoxElements.SelectedIndex);
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            // If there is a selected element in the list box (not at the bottom of the list), move it up one index.
            if (listBoxElements.SelectedIndex < listBoxElements.Items.Count - 1)
            {
                int index = listBoxElements.SelectedIndex;
                object item = listBoxElements.Items[index];
                listBoxElements.Items.RemoveAt(index);
                listBoxElements.Items.Insert(index + 1, item);

                // Select the item that was previously selected.
                listBoxElements.SelectedIndex = index + 1;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "";
            if (textBoxImageFilename.Text != "")
            {
                string path = textBoxImageFilename.Text;
                string folder = Path.GetDirectoryName(path);
                if (Directory.Exists(folder))
                {
                    openFileDialog1.InitialDirectory = folder;
                    if (File.Exists(path))
                    {
                        openFileDialog1.FileName = path;
                    }
                }
            }
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                textBoxImageFilename.Text = openFileDialog1.FileName;
            }
        }

        private void buttonBrowseGrid_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "ESRI shapefiles (*.shp)|*.shp";
            if (textBoxGridShapefileName.Text != "")
            {
                string path = textBoxGridShapefileName.Text;
                string folder = Path.GetDirectoryName(path);
                if (Directory.Exists(folder))
                {
                    openFileDialog1.InitialDirectory = folder;
                    if (File.Exists(path))
                    {
                        openFileDialog1.FileName = path;
                    }
                }
            }
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                textBoxGridShapefileName.Text = openFileDialog1.FileName;
            }
        }

        private void enableControls(bool enable)
        {
            Enabled = enable;
        }

        private void DocumentMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_okToClose)
            {
                e.Cancel = true;
                _okToClose = true;
            }
        }

        private void btnRemoveExtents_Click(object sender, EventArgs e)
        {
            // Remove selected extents.
            if (listViewExtents.SelectedIndices.Count > 0)
            {
                // Remove items in reverse order to avoid index changes as items are removed.
                for (int i = listViewExtents.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    listViewExtents.Items.RemoveAt(listViewExtents.SelectedIndices[i]);
                }
            }
        }

        private void comboBoxTimeUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.TemporalRef.ModelTimeUnit = (TemporalReference.ModflowTimeUnit)(comboBoxTimeUnit.SelectedIndex + 1);
            _document.ChartsNeedRecompute = true;
            _document.TablesNeedRecompute = true;
        }

        private void buttonSimulationStartTime_Click(object sender, EventArgs e)
        {
            // Open DateTime dialog and assign _simulationStartTime
            ScenarioTools.Dialogs.DateTimeDialog dateTimeDialog = new DateTimeDialog();
            dateTimeDialog.DateTime = TemporalRef.SimulationStartTime;
            DialogResult dr = dateTimeDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                TemporalRef.SimulationStartTime = dateTimeDialog.DateTime;
                textBoxSimulationStartTime.Text = TemporalRef.SimulationStartTime.ToString();
            }
        }

        private void comboBoxLengthUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            _modflowLengthUnit = (LengthReference.ModflowLengthUnit)(comboBoxLengthUnit.SelectedIndex + 1);
            _document.MapsNeedRecompute = true;
        }

        private void radioButtonBlankSpecifiedIboundLayer_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownBlankingLayer.Enabled = radioButtonBlankSpecifiedIboundLayer.Checked;
            _document.MapsNeedRecompute = true;
        }

        private void radioButtonAsIs_CheckedChanged(object sender, EventArgs e)
        {
            textBoxNameFile.Enabled = !radioButtonAsIs.Checked;
            buttonBrowseForNameFile.Enabled = !radioButtonAsIs.Checked;
            if (radioButtonAsIs.Checked)
            {
                labelModflowNameFile.ForeColor = Color.Gray;
            }
            else
            {
                labelModflowNameFile.ForeColor = Color.Black;
            }
            _document.MapsNeedRecompute = true;
        }

        private void buttonBrowseForNameFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "MODFLOW Name files (*.nam)|*.nam|All files (*.*)|*.*";
            if (textBoxNameFile.Text != "")
            {
                string path = textBoxNameFile.Text;
                string folder = Path.GetDirectoryName(path);
                if (Directory.Exists(folder))
                {
                    openFileDialog1.InitialDirectory = folder;
                    if (File.Exists(path))
                    {
                        openFileDialog1.FileName = path;
                    }
                }
            }
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                textBoxNameFile.Text = openFileDialog1.FileName;
            }
        }

        private void textBoxSimulationStartTime_TextChanged(object sender, EventArgs e)
        {
            _document.ChartsNeedRecompute = true;
            _document.TablesNeedRecompute = true;
        }

        private void textBoxGridShapefileName_TextChanged(object sender, EventArgs e)
        {
            _document.MapsNeedRecompute = true;
        }

        private void textBoxImageFilename_TextChanged(object sender, EventArgs e)
        {
            _document.MapsNeedRecompute = true;
        }

        private void textBoxHnoflo_TextChanged(object sender, EventArgs e)
        {
            _document.ChartsNeedRecompute = true;
            _document.MapsNeedRecompute = true;
            _document.TablesNeedRecompute = true;
        }

        private void textBoxHdry_TextChanged(object sender, EventArgs e)
        {
            _document.ChartsNeedRecompute = true;
            _document.MapsNeedRecompute = true;
            _document.TablesNeedRecompute = true;
        }

        private void textBoxCinact_TextChanged(object sender, EventArgs e)
        {
            _document.ChartsNeedRecompute = true;
            _document.MapsNeedRecompute = true;
            _document.TablesNeedRecompute = true;
        }

        private void textBoxNameFile_TextChanged(object sender, EventArgs e)
        {
            _document.MapsNeedRecompute = true;
        }

        private void numericUpDownBlankingLayer_ValueChanged(object sender, EventArgs e)
        {
            _document.MapsNeedRecompute = true;
        }

        private void radioButtonBlankIfAnyLayerInactive_CheckedChanged(object sender, EventArgs e)
        {
            _document.MapsNeedRecompute = true;
        }

        private void radioButtonBlankIfAllLayersInactive_CheckedChanged(object sender, EventArgs e)
        {
            _document.MapsNeedRecompute = true;
        }

        private void listViewExtents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewExtents.SelectedIndices.Count > 0)
            {
                // Using SelectedIndices[0] makes sense only because 
                // MultiSelect=false for this control
                int indx = listViewExtents.SelectedIndices[0];
                if (indx >= 0)
                {
                    if (_document.IsInUse((Extent) listViewExtents.Items[indx].Tag))
                    {
                        btnRemoveExtents.Enabled = false;
                        listViewExtents.SelectedItems.Clear();
                    }
                    else
                    {
                        btnRemoveExtents.Enabled = true;
                    }
                }
            }
            else
            {
                btnRemoveExtents.Enabled = false;
            }
        }
    }
}
