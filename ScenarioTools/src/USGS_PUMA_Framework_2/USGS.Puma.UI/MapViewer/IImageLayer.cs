using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace USGS.Puma.UI.MapViewer
{
    public interface IImageLayer : IGraphicLayer
    {
        void RenderImage(Graphics g, Viewport vp);
        void Update();
    }
}
