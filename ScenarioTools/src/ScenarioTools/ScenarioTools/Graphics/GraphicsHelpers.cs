using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using GeoAPI.Geometries;

using ScenarioTools.Util;

using USGS.Puma.UI.MapViewer;

namespace ScenarioTools.Graphics
{
    public class GraphicsHelpers
    {
        public static Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            bmpImage.Dispose();
            return (Image)(bmpCrop);
        }

        /// <summary>
        /// Returns an image of a scale bar
        /// </summary>
        /// <param name="envelope">Extent, in geographic coordinates</param>
        /// <param name="imageExtentWidth">Width of extent, in image coordinates</param>
        /// <param name="imageWidth">Width of image to be generated</param>
        /// <param name="imageHeight">Height of image to be generated</param>
        /// <returns></returns>
        public static Image MakeScaleBarImage(IEnvelope envelope, double imageExtentWidth, 
            int imageWidth, int imageHeight, string lengthUnit)
        {
            if (envelope == null)
            {
                return null;
            }

            // Determine properties of scale bar
            double geoExtentWidth = envelope.MaxX - envelope.MinX;
            double scaleRatio = imageExtentWidth / geoExtentWidth;
            double maxGeoScaleLength = 0.5 * geoExtentWidth;
            double logMaxGeoScaleLength = Math.Log10(maxGeoScaleLength);
            int exponent = (int)Math.Truncate(logMaxGeoScaleLength);
            double mantissa = logMaxGeoScaleLength - Convert.ToDouble(exponent);
            double factor = Math.Pow(10.0, mantissa);
            double scaleMaxValue;

            // Define ticks
            int numIntermediateTicks = 0;
            if (factor >= 5.0)
            {
                scaleMaxValue = Math.Truncate(factor) * Math.Pow(10.0, exponent);
                numIntermediateTicks = (int)Math.Truncate(factor) - 1;
            }
            else if (factor >= 2.0)
            {
                double factorMult2 = factor * 2.0;
                scaleMaxValue = (Math.Truncate(factorMult2) / 2.0) * Math.Pow(10.0, exponent);
                numIntermediateTicks = (int)Math.Truncate(factorMult2) - 1;
            }
            else
            {
                double factorMult5 = factor * 5.0;
                scaleMaxValue = (Math.Truncate(factorMult5) / 5.0) * Math.Pow(10.0, exponent);
                numIntermediateTicks = (int)Math.Truncate(factorMult5) - 1;                 
            }
            double imageScaleLength = scaleMaxValue * scaleRatio;
            double imageTickInterval = imageScaleLength/(Convert.ToDouble(numIntermediateTicks+1));

            // Draw the scale bar            
            Bitmap bitmap = new Bitmap(imageWidth, imageHeight);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                Pen pen = new Pen(Color.Black);
                pen.Width = 2.0f;
                int iy0 = Convert.ToInt32(Convert.ToDouble(imageHeight) * 0.8); // Y coordinate of scale bar line
                double x0 = imageWidth * 0.2; // X coordinate of left end of scale bar line
                double x1 = x0 + imageScaleLength; // X coordinate of right end of scale bar line
                int ix0 = Convert.ToInt32(x0);
                int ix1 = Convert.ToInt32(x1);
                Point p0 = new Point(ix0, iy0);
                Point p1 = new Point(ix1, iy0);

                // Draw base of scale bar
                g.DrawLine(pen, p0, p1);

                // Draw end ticks
                int largeTickHeight = 12;
                int iy1 = iy0 - largeTickHeight;
                Point t0 = new Point(ix0, iy1);
                Point t1 = new Point(ix1, iy1);
                g.DrawLine(pen, p0, t0);
                g.DrawLine(pen, p1, t1);

                // Draw intermediate ticks
                int smallTickHeight = largeTickHeight / 2;
                double x = x0;
                int iy = iy0 - smallTickHeight;
                int ix;
                for (int i = 0; i < numIntermediateTicks; i++)
                {
                    x = x + imageTickInterval;
                    ix = Convert.ToInt32(x);
                    g.DrawLine(pen, ix, iy0, ix, iy);
                }

                // Define font for tick labels
                float fontHeightMedium = 14.0f;
                Font fontMedium;
                fontMedium = new Font(FontFamily.GenericSansSerif, fontHeightMedium, FontStyle.Regular);

                // Prepare to draw tick labels
                float yAdjust = 28.0f; // Adjustment of label Y coordinate, relative to tick end
                float fy = Convert.ToSingle(iy1) - yAdjust;
                SolidBrush brush = new SolidBrush(Color.Black);

                // Draw label at left ("0") end of scale bar
                SizeF sizeText = g.MeasureString("0", fontMedium);
                float xAdjust0 = sizeText.Width / 2.0f; ; // Adjustment of left label X coordinate, relative to tick
                float fx = Convert.ToSingle(x0) - xAdjust0;
                PointF pt0 = new PointF(fx, fy);
                g.DrawString("0", fontMedium, brush, pt0);

                // Draw label at right end of scale bar
                string text = scaleMaxValue.ToString();
                string textWithCommas = StringUtil.CustomNumberFormat(text);
                sizeText = g.MeasureString(textWithCommas, fontMedium);
                float xAdjust1 = sizeText.Width / 2.0f;
                text = textWithCommas + " " + lengthUnit;
                fx = Convert.ToSingle(ix1) - xAdjust1;
                PointF pt1 = new PointF(fx, fy);
                g.DrawString(text, fontMedium, brush, pt1);
            }
            return (Image)bitmap;
        }

        /// <summary>
        /// Generate an image from two images, stacked one on the other
        /// </summary>
        /// <param name="upperImage"></param>
        /// <param name="lowerImage"></param>
        /// <returns></returns>
        public static Image StackImages(Image upperImage, Image lowerImage)
        {
            int newImageWidth = Math.Max(upperImage.Width, lowerImage.Width);
            int newImageHeight = upperImage.Height + lowerImage.Height;
            Bitmap bitmap = new Bitmap(newImageWidth, newImageHeight);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                int y = 0;
                int x = 0;
                g.DrawImageUnscaled(upperImage, x, y);
                y = y + upperImage.Height;
                g.DrawImageUnscaled(lowerImage, x, y);
            }
            return (Image)bitmap;
        }

        /// <summary>
        /// Generate an image from two images, one to the right of the other
        /// </summary>
        /// <param name="leftImage"></param>
        /// <param name="rightImage"></param>
        /// <returns></returns>
        public static Image AppendImages(Image leftImage, Image rightImage)
        {
            int newImageWidth = leftImage.Width + rightImage.Width;
            int newImageHeight = Math.Max(leftImage.Height, rightImage.Height);
            Bitmap bitmap = new Bitmap(newImageWidth, newImageHeight);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                int y = 0;
                int x = 0;
                g.DrawImageUnscaled(leftImage, x, y);
                x = x + leftImage.Width;
                g.DrawImageUnscaled(rightImage, x, y);
            }
            return (Image)bitmap;
        }

        /// <summary>
        /// Draw symbol representing feature layer in a rectangle
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="g"></param>
        /// <param name="rectangle"></param>
        public static int DrawFeatureLayerSymbol(MapLegendItem mapLegendItem, Bitmap bitmap, System.Drawing.Graphics g,
                                                 float x0, float y0, float symbolWidth, Font font, ref float labelX, 
                                                 ref float labelY, ref float labelYCenter, ref float labelYTop, 
                                                 ref float labelYBottom)
        {            
            float symbolHeight;
            float labelSpace = 10.0f;
            int iSymbolHeight = 0;
            labelX = 0.0f;
            labelY = 0.0f;
            float indent = 20.0f;
            if (mapLegendItem.MapLayer is FeatureLayer)
            {
                FeatureLayer featureLayer = (FeatureLayer)mapLegendItem.MapLayer;
                if (featureLayer.Renderer is SingleSymbolRenderer)
                {
                    symbolHeight = 20.0f;
                    labelX = x0 + symbolWidth + labelSpace;
                    labelY = y0 + symbolHeight / 2.0f - Convert.ToSingle(font.Height) / 2.0f;
                    labelYTop = labelY;
                    labelYBottom = labelY + Convert.ToSingle(font.Height) / 2.0f;
                    RectangleF rectangleF = new RectangleF(x0, y0, symbolWidth, symbolHeight);
                    #region Draw single symbol
                    SingleSymbolRenderer renderer = (SingleSymbolRenderer)featureLayer.Renderer;
                    SymbolType symbolType = renderer.SymbolType;
                    ISymbol symbol = renderer.Symbol;
                    switch (symbolType)
                    {
                        case SymbolType.FillSymbol:
                            {
                                symbolHeight = 20.0f;
                                // Draw a black rectangle filled with the symbol color
                                SolidFillSymbol fillSymbol = (SolidFillSymbol)symbol;
                                SolidBrush brush = new SolidBrush(fillSymbol.Color);
                                g.FillRectangle(brush, rectangleF);
                                Pen pen = new Pen(Color.Black);
                                g.DrawRectangle(pen, rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
                                iSymbolHeight = Convert.ToInt32(symbolHeight);
                                labelYCenter = rectangleF.Y + rectangleF.Height / 2.0f;
                                break;
                            }
                        case SymbolType.LineSymbol:
                            {
                                symbolHeight = Convert.ToSingle(font.Height);
                                // Draw a line of the symbol color and width
                                LineSymbol lineSymbol = (LineSymbol)symbol;
                                Pen pen = new Pen(lineSymbol.Color, lineSymbol.Width);
                                float x1 = rectangleF.X;
                                float x2 = rectangleF.X + rectangleF.Width;
                                float y = rectangleF.Y + rectangleF.Height / 2.0f;
                                g.DrawLine(pen, x1, y, x2, y);
                                iSymbolHeight = Convert.ToInt32(symbolHeight);
                                labelYCenter = y + pen.Width / 2.0f;
                                break;
                            }
                        case SymbolType.PointSymbol:
                            {
                                // Draw a point symbol of the appropriate color and type

                                // Ned TODO: Code for DrawFeatureLayerSymbol is probably messed up for SymbolType.PointSymbol

                                //SimplePointSymbol pointsymbol = (SimplePointSymbol)symbol;
                                //Pen outlinePen = new Pen(pointsymbol.OutlineColor);
                                //SolidBrush fillBrush = new SolidBrush(pointsymbol.Color);
                                //int symbolHeight = Convert.ToInt32(rectangle.Height);
                                //Size size = new Size(symbolHeight,symbolHeight);

                                RendererHelper rh = new RendererHelper();
                                int symbolSize = 16;
                                rh.RenderLegendSymbol(symbol, g, new System.Drawing.Size(symbolSize, symbolSize));

                                //Viewport vp = new Viewport(size);
                                //ISymbolDrawingTool tool = CreateDrawingTool(symbol);

                                //if (symbol is SimplePointSymbol)
                                //{
                                //    PointF pt = new PointF(size.Width / 2.0f, size.Height / 2.0f);
                                //    SimplePointSymbol ptSymbol = symbol as SimplePointSymbol;
                                //    if (ptSymbol.SymbolType == PointSymbolTypes.Circle)
                                //    {
                                //        vp.DrawCirclePoint(pt, g, tool.Pen, tool.FillBrush, ptSymbol.Size);
                                //    }
                                //    else if (ptSymbol.SymbolType == PointSymbolTypes.Square)
                                //    {
                                //        vp.DrawSquarePoint(pt, g, tool.Pen, tool.FillBrush, ptSymbol.Size);
                                //    }
                                //}

                                iSymbolHeight = symbolSize;
                                break;
                            }
                    }
                    #endregion Draw single symbol
                }
                else if (featureLayer.Renderer is ColorRampRenderer)
                {
                    symbolHeight = 100.0f;
                    labelY = y0 + symbolHeight / 2.0f - Convert.ToSingle(font.Height) / 2.0f;
                    iSymbolHeight = Convert.ToInt32(symbolHeight);
                    int itemHeight = iSymbolHeight + font.Height + 4;
                    #region Draw color ramp
                    if (mapLegendItem.Symbology != null)
                    {
                        // Draw color bar
                        System.Windows.Forms.Panel panel = mapLegendItem.Symbology;
                        int x = Convert.ToInt32(x0);
                        int y = Convert.ToInt32(y0) + font.Height / 2;
                        int iSymbolWidth = Convert.ToInt32(symbolWidth);
                        System.Drawing.Rectangle r = new Rectangle(x, y, iSymbolWidth, iSymbolHeight);
                        IColorRampRenderer renderer = (IColorRampRenderer)featureLayer.Renderer;
                        int numColors = renderer.ColorRamp.Colors.GetLength(0);

                        // Populate a Color array and ColorRamp with reversed color order to 
                        // make legend symbol with color representing largest value at the top
                        Color[] reverseColors = new Color[numColors];
                        int j = numColors;
                        for (int i = 0; i < numColors; i++)
                        {
                            j--;
                            reverseColors[j] = renderer.ColorRamp.Colors[i];
                        }
                        ColorRamp colorRamp = new ColorRamp(reverseColors);

                        ColorBar colorBar = new ColorBar(colorRamp, 100);
                        colorBar.Width = iSymbolWidth;
                        colorBar.Height = iSymbolHeight;
                        colorBar.DrawToBitmap(bitmap, r);
                        Pen pen = new Pen(Color.Black);
                        g.DrawRectangle(pen, r);

                        // Draw min and max values next to color bar
                        double minValue = renderer.MinimumValue;
                        double maxValue = renderer.MaximumValue;
                        string minValueText = minValue.ToString("0.0000E+00");
                        string maxValueText = maxValue.ToString("0.0000E+00");
                        SolidBrush brush = new SolidBrush(Color.Black);
                        float xf = Convert.ToSingle(x + iSymbolWidth + 5);
                        float yf = Convert.ToSingle(y - font.Height / 2);
                        labelYTop = yf + Convert.ToSingle(font.Height) / 2.0f;
                        g.DrawString(maxValueText, font, brush, xf, yf);
                        float textWidth1 = g.MeasureString(maxValueText, font).Width;
                        yf = yf + symbolHeight;
                        labelYBottom = yf - Convert.ToSingle(font.Height) / 2.0f;
                        g.DrawString(minValueText, font, brush, xf, yf);
                        float textWidth2 = g.MeasureString(minValueText, font).Width;
                        //labelX = xf + Math.Max(textWidth1, textWidth2);
                        labelX = xf + indent;
                        labelY = Convert.ToSingle(y) + symbolHeight/2.0f - Convert.ToSingle(font.Height) / 2.0f;
                        iSymbolHeight = itemHeight;
                        labelYCenter = Convert.ToSingle(y) + symbolHeight / 2.0f;
                    }
                    #endregion Draw color ramp
                }
            }
            return iSymbolHeight;
        }

        public static Bitmap ApplyBrightness(Bitmap sourceBitmap, int brightness)
        {
            int A, R, G, B;
            Color pixelColor;
            Bitmap newBitmap = new Bitmap(sourceBitmap);
            if (brightness == 0)
            {
                return newBitmap;
            }

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    pixelColor = sourceBitmap.GetPixel(x, y);
                    A = pixelColor.A;
                    R = pixelColor.R + brightness;
                    if (R > 255)
                    {
                        R = 255;
                    }
                    else if (R < 0)
                    {
                        R = 0;
                    }

                    G = pixelColor.G + brightness;
                    if (G > 255)
                    {
                        G = 255;
                    }
                    else if (G < 0)
                    {
                        G = 0;
                    }

                    B = pixelColor.B + brightness;
                    if (B > 255)
                    {
                        B = 255;
                    }
                    else if (B < 0)
                    {
                        B = 0;
                    }

                    newBitmap.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                }
            }
            return newBitmap;
        }
        /// <summary>
        /// Return array of lines, parsed to best fit within specified width in specified font
        /// </summary>
        /// <param name="s">String to be parsed into lines</param>
        /// <param name="width">Max. width per line</param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static string[] GetLines(string s, int width, Font font)
        {
            List<string> lineList = new List<string>();
            char[] separators = { ' ' };
            string[] words = s.Split(separators);
            List<string> wordList = new List<string>(words);
            int numWords = words.Length;
            int nextWord = 0;
            int finalWord = numWords - 1;
            StringBuilder currentSection = new StringBuilder();
            SizeF sizeF = new SizeF();
            int tempHeight = 200;
            Bitmap bitmap = new Bitmap(width, tempHeight);
            string tempString = s;
            int lastWord = 0;
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                lastWord = finalWord;
                while (nextWord <= finalWord)
                {
                    tempString = makeLine(wordList, nextWord, lastWord);
                    sizeF = g.MeasureString(tempString, font);
                    if (sizeF.Width <= width)
                    {
                        lineList.Add(tempString);
                        nextWord = lastWord + 1;
                        lastWord = finalWord;
                    }
                    else
                    {
                        lastWord--;
                    }
                }
            }
            string[] lines = lineList.ToArray();
            return lines;
        }
        private static string makeLine(List<string> words, int firstWord, int lastWord)
        {
            string line = "";
            for (int i = firstWord; i <= lastWord; i++)
            {
                line = line + words[i];
                if (i < lastWord)
                {
                    line = line + " ";
                }
            }
            return line;
        }
    }
}
