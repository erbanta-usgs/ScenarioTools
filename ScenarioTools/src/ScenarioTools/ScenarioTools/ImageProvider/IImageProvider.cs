using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ScenarioTools.ImageProvider
{
    public interface IImageProvider
    {
        Image GetImage();
        Image GetImage(float minValue, float maxValue, long minTicks, long maxTicks, 
            double xcMin, double xcMax, double ycMin, double ycMax, bool isStandalone);
        void ClearImage();
    }
}
