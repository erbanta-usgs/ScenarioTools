using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Geometry;

namespace ScenarioTools.Gui
{
    public partial class ExtentDialog : Form
    {
        private Extent _extent;
        private List<Extent> _localExtents;

        public ExtentDialog(Extent extent, List<Extent> localExtents)
        {
            InitializeComponent();

            _extent = extent;
            _localExtents = localExtents;

            // Populate text boxes
            textBoxName.Text = extent.Name;
            textBoxNorth.Text = String.Format("{0:0.0}", extent.North);
            textBoxWest.Text = String.Format("{0:0.0}", extent.West);
            textBoxEast.Text = String.Format("{0:0.0}", extent.East);
            textBoxSouth.Text = String.Format("{0:0.0}", extent.South);

            btnSave.Enabled = textBoxName.Text != "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string msg = "";
            if (textBoxName.Text == "")
            {
                msg = "Error: Extent name not provided";
                MessageBox.Show(msg);
            }
            else
            {
                try
                {
                    // Check to see if extent name is already being used
                    bool OK = true;
                    for (int i = 0; i < _localExtents.Count; i++)
                    {
                        if (textBoxName.Text == _localExtents[i].Name)
                        {
                            OK = false;
                            break;
                        }
                    }

                    if (OK)
                    {
                        _extent.Name = textBoxName.Text;
                        _extent.North = Convert.ToDouble(textBoxNorth.Text);
                        _extent.West = Convert.ToDouble(textBoxWest.Text);
                        _extent.East = Convert.ToDouble(textBoxEast.Text);
                        _extent.South = Convert.ToDouble(textBoxSouth.Text);

                        // Save extent
                        Extent newExtent = new Extent(_extent);
                        _localExtents.Add(newExtent);

                        this.DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        msg = "Error: Extent name is already in use.";
                        MessageBox.Show(msg);
                    }
                }
                catch
                {
                    msg = "Error converting coordinate to number.";
                    MessageBox.Show(msg);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = textBoxName.Text != "";
        }
    }
}
