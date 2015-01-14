using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScenarioManager
{
    /// <summary>
    /// A UserControl containing a GroupBox designed to allow user to control
    /// layer assignment for a layer-based package such as RCH, EVT, or ETS.
    /// </summary>
    public partial class GroupBoxLayerAssignment : UserControl
    {
        #region Fields
        private int _npkgop = 1;
        private bool _changed = false;
        private bool _mustBeMaster = false;
        private string _layerAttribute = "";
        private ComboBox _cmbxLayerAttribute;
        #endregion Fields

        #region Properties
        public int Npkgop
        {
            get
            {
                return _npkgop;
            }
            set
            {
                _npkgop = value;
                cmbxNpkgop.SelectedIndex = _npkgop - 1;
            }
        }
        public bool Changed
        {
            get
            {
                return _changed;
            }
            set
            {
                _changed = value;
            }
        }
        public bool MustBeMaster
        {
            get
            {
                return _mustBeMaster;
            }
            set
            {
                _mustBeMaster = value;
            }
        }
        public bool UseAsMasterFeatureSet
        {
            get
            {
                return ckbxUseAsMaster.Checked;
            }
            set
            {
                ckbxUseAsMaster.Checked = value;
            }
        }
        public string LayerAttribute
        {
            get
            {
                return _layerAttribute;
            }
            set
            {
                _layerAttribute = value;
            }
        }
        public ComboBox CmbxLayerAttribute
        {
            get
            {
                return _cmbxLayerAttribute;
            }
            private set
            {
                _cmbxLayerAttribute = value;
            }
        }
        #endregion Properties

        public GroupBoxLayerAssignment()
        {
            InitializeComponent();
            CmbxLayerAttribute = (ComboBox)groupBox1.Controls["cmbxLayerAttribute"];
        }

        private void cbxNpkgop_SelectedIndexChanged(object sender, EventArgs e)
        {
            Npkgop = cmbxNpkgop.SelectedIndex + 1;
            SetupPkgLayerAssignment(Npkgop);
            Changed = true;
        }

        public void SetupPkgLayerAssignment(int npkgop)
        {
            Npkgop = npkgop;
            cmbxNpkgop.Enabled = ckbxUseAsMaster.Checked;
            int cmbxNpkgopIndex = npkgop - 1;
            if (cmbxNpkgop.SelectedIndex != cmbxNpkgopIndex)
            {
                cmbxNpkgop.SelectedIndex = cmbxNpkgopIndex;
            }
            else
            {
                SetupCmbxLayerAttribute(cmbxNpkgopIndex);
            }
        }

        private void SetupCmbxLayerAttribute(int index)
        {
            switch (index)
            {
                case 0:
                    goto default;
                case 1:
                    cmbxLayerAttribute.Enabled = ckbxUseAsMaster.Checked;
                    break;
                case 2:
                // fall through
                default:
                    cmbxLayerAttribute.Enabled = false;
                    break;
            }
        }

        private void GroupBoxLayerAssignment_Load(object sender, EventArgs e)
        {
            // Set up Use As Master check box
            if (_mustBeMaster)
            {
                ckbxUseAsMaster.Checked = true;
                ckbxUseAsMaster.Enabled = false;
            }
            else
            {
                ckbxUseAsMaster.Enabled = true;
            }

            // Set up Layer Attribute combo box
            SetupPkgLayerAssignment(_npkgop);
            //cmbxLayerAttribute.Enabled = ckbxUseAsMaster.Checked && this._npkgop == 2;
            if (cmbxLayerAttribute.Items.Count > 0)
            {
                for (int i = 0; i < cmbxLayerAttribute.Items.Count; i++)
                {
                    if (cmbxLayerAttribute.Items[i].ToString().Equals(LayerAttribute))
                    {
                        cmbxLayerAttribute.SelectedIndex = i;
                        cmbxLayerAttribute.SelectedItem = LayerAttribute;
                        break;
                    }
                    cmbxLayerAttribute.SelectedIndex = -1;
                }
            }
            else
            {
                cmbxLayerAttribute.Text = "";
            }

            // Set up Package Option combo box
            cmbxNpkgop.Enabled = ckbxUseAsMaster.Checked;
            cmbxNpkgop.SelectedIndex = Npkgop - 1;

            Changed = false;
        }

        private void cmbxLayerAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbxLayerAttribute.SelectedItem != null)
            {
                LayerAttribute = cmbxLayerAttribute.SelectedItem.ToString();
                Changed = true;
            }
        }

        private void ckbxUseAsMaster_CheckedChanged(object sender, EventArgs e)
        {
            SetupPkgLayerAssignment(_npkgop);
            Changed = true;
        }
    }
}
