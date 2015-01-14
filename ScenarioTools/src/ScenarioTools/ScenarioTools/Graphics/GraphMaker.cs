using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ScenarioTools.Graphics
{
    class GraphMaker
    {
        public static void MakePdfGraph(double[] timeSeries)
        {
        }
        public static Image MakeRasterGraph(double[] timeSeries)
        {
            int width = 500;
            int height = 200;
            Bitmap image = new Bitmap(width, height);

            return image;
        }
    }
}
