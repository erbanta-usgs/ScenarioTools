using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.Dialogs
{
    public partial class ImageBox : Form
    {
        public ImageBox(Image image)
        {
            InitializeComponent();
            pictureBox1.Image = image;
        }

        public static void Show(Image image)
        {
            ImageBox imageBox = new ImageBox(image);
            imageBox.ShowDialog();
            imageBox.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
