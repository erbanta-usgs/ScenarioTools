using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OSGeo.GDAL;
using OSGeo.MapServer;
using OSGeo.OGR;
using OSGeo.OSR;

namespace GdalTest
{
    public partial class Form1 : Form
    {
        OSGeo.GDAL.Gdal gdal;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gdal = new Gdal();
        }
    }
}
