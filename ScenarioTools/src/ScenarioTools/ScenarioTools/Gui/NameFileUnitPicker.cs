using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ScenarioTools.ModflowReaders;

namespace ScenarioTools.Gui
{
    /// <summary>
    /// A dialog to allow user to select a unit number associated with
    /// a file in the name file.  Clicking on a file name/unit will
    /// close the dialog and return an int that is the unit number.
    /// </summary>
    public partial class NameFileUnitPicker : Form
    {
        #region Fields
        private NamefileInfo _nameFileInfo;
        #endregion Fields

        public NameFileUnitPicker(NamefileInfo nameFileInfo, int unitNumber)
        {
            InitializeComponent();
            _nameFileInfo = nameFileInfo;
            Unit = unitNumber;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        #region Properties
        public int Unit;
        #endregion Properties

        private void dataGridView1_SizeChanged(object sender, EventArgs e)
        {
            dataGridView1.Columns[0].Width = dataGridView1.Width - dataGridView1.Columns[1].Width;
        }

        private void NameFileUnitPicker_Load(object sender, EventArgs e)
        {
            if (_nameFileInfo == null)
            {
                string errMsg = "Master name file must be defined.  To define the master name file, use menu item Project | Settings.";
                MessageBox.Show(errMsg);
                this.Close();
            }
            else
            {
                string label0 = "Select a file from the list above. These files were extracted from MODFLOW name file \"";
                string label1 = ".\"  The list only includes files with file type DATA(BINARY) and file status REPLACE.";
                textBox1.Text = label0 + _nameFileInfo.Namefile + label1;

                // Get name file entries that are DATA(BINARY) output ("REPLACE") files
                List<NameFileEntry> entryList = new List<NameFileEntry>();
                for (int i = 0; i < _nameFileInfo.Items.Count; i++)
                {
                    NameFileEntry entry = _nameFileInfo.Items[i];
                    string type = entry.Type.ToLower();
                    if (type == "data(binary)" && entry.Access == InOutAccess.Output)
                    {
                        entryList.Add(entry);
                    }
                }

                // Add records to table
                if (entryList.Count > 0)
                {
                    for (int i = 0; i < entryList.Count; i++)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dataGridView1);
                        row.Cells[0].Value = entryList[i].Filename;
                        row.Cells[1].Value = entryList[i].Unit;
                        dataGridView1.Rows.Add(row);

                        // If unit number equals this.Unit, select the row
                        if (entryList[i].Unit == this.Unit)
                        {
                            dataGridView1.Rows[i].Selected = true;
                        }
                    }

                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataGridView1.SelectedRows;            
            Unit = (int)rows[0].Cells[1].Value;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
