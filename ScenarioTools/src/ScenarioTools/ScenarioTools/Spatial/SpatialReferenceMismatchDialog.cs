using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.Spatial
{
    public partial class SpatialReferenceMismatchDialog : Form
    {
        #region Fields
        private SpatialReference _spatialReference1;
        private SpatialReference _spatialReference2;
        private string _errMsg;
        #endregion Fields

        public SpatialReferenceMismatchDialog()
        {
            InitializeComponent();
            _spatialReference1 = null;
            _spatialReference2 = null;
            _errMsg = "";
        }
        public SpatialReferenceMismatchDialog(SpatialReference spatialReference1, SpatialReference spatialReference2, string errMsg)
            : this()
        {
            _spatialReference1 = spatialReference1;
            _spatialReference2 = spatialReference2;
            _errMsg = errMsg;
        }

        private void SpatialReferenceMismatchDialog_Load(object sender, EventArgs e)
        {
            if (_spatialReference1 != null)
            {
                if (_spatialReference1.GeoreferencedObject != null)
                {
                    label1.Text = _spatialReference1.GeoreferencedObject.ToString();
                }
                textBoxWkt1.Text = _spatialReference1.GetWktString();
            }
            if (_spatialReference2 != null)
            {
                if (_spatialReference2.GeoreferencedObject != null)
                {
                    label2.Text = _spatialReference2.GeoreferencedObject.ToString();
                }
                textBoxWkt2.Text = _spatialReference2.GetWktString();
            }
            textBoxError.Text = _errMsg;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
