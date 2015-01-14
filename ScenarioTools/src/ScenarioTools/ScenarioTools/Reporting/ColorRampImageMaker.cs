using System;
using System.Drawing;

using USGS.Puma.UI.MapViewer;

namespace ScenarioTools.Reporting
{
    public class ColorRampImageMaker
    {
        public enum RampDirection
        {
            LeftToRight = 0,
            BottomToTop = 1
        }

        #region Fields
        Color _color0;
        Color _color1;
        #endregion Fields

        #region Constructors
        public ColorRampImageMaker()
        {
            _color0 = Color.Black;
            _color1 = Color.White;
        }
        public ColorRampImageMaker(Color color0, Color color1)
        {
            _color0 = color0;
            _color1 = color1;
        }
        #endregion Constructors

        #region Methods
        public Image GetImage(RampDirection rampDirection)
        {
            return GetImage(100, 10, rampDirection, true);
        }
        public Image GetImage(int width, int height, RampDirection rampDirection, bool drawBorder)
        {
            System.Drawing.Image image = null;
            System.Drawing.Graphics g = null;

            // Create a bitmap image
            image = new System.Drawing.Bitmap(width, height);
            
            // Provide a drawing surface
            g = System.Drawing.Graphics.FromImage(image);

            // Clear the background
            g.Clear(Color.White);

            // Assign page units and dimensions
            g.PageUnit = System.Drawing.GraphicsUnit.Pixel;
            int xCoord = 0;
            int yCoord = 0;

            // Define a rectangle in which the image will be drawn
            System.Drawing.Rectangle r = new Rectangle(xCoord, yCoord, width, height);

            // Draw the color ramp
            drawColorRamp(width, height, rampDirection, g);

            // Draw a neat line around the rectangle
            if (drawBorder)
            {
                Pen pen = new Pen(Color.Black);
                g.DrawRectangle(pen, r);
            }

            // Return the image
            return image;
        }
        public ColorRamp GetColorRamp()
        {
            Color[] colors = new Color[] { _color0, _color1 };
            return new ColorRamp(colors);
        }
        public ColorBar GetColorBar()
        {
            return new ColorBar(GetColorRamp(), 100);
        }
        public ColorBar GetColorBar(int sections)
        {
            return new ColorBar(GetColorRamp(), sections);
        }
        private void drawColorRamp(int iWidth, int iHeight, RampDirection rampDirection, System.Drawing.Graphics g)
        {
            float dX;
            float dY;
            float x;
            float y;
            float width = Convert.ToSingle(iWidth);
            float height = Convert.ToSingle(iHeight);

            ColorBar colorBar = GetColorBar(100);
            Color[] colors = colorBar.Colors;
            if (colors != null)
            {
                if (colors.Length > 0)
                {
                    System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    if (rampDirection == RampDirection.LeftToRight)
                    {
                        dX = width / Convert.ToSingle(colors.Length);
                        x = 0.0f;
                        y = 0.0f;
                        for (int i = 0; i < colors.Length; i++)
                        {
                            brush.Color = colors[i];
                            g.FillRectangle(brush, x, y, dX, height);
                            x += dX;
                        }
                    }
                    else
                    {
                        dY = height / Convert.ToSingle(colors.Length);
                        x = 0.0f;
                        y = height - dY;
                        for (int i = 0; i < colors.Length; i++)
                        {
                            brush.Color = colors[i];
                            g.FillRectangle(brush, x, y, width, dY);
                            y -= dY;
                        }
                    }
                    brush.Dispose();
                    brush = null;
                }
            }
        }
        #endregion Methods
    }
}
