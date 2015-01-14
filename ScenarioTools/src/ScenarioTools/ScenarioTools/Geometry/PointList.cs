using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ScenarioTools.Numerical;

namespace ScenarioTools.Geometry
{
    public class PointList
    {
        List<PointF> points;

        List<Point2D> currentLine;
        List<Polyline2D> polylines;

        public PointList()
        {
            // Make a list for the points.
            points = new List<PointF>();

            // Make a list for the current line.
            currentLine = new List<Point2D>();

            // Make a list for the lines.
            polylines = new List<Polyline2D>();

            x0Prev = float.NaN;
            y0Prev = float.NaN;
            x1Prev = float.NaN;
            y1Prev = float.NaN;
        }

        float x0Prev;
        float y0Prev;
        float x1Prev;
        float y1Prev;
        float currentContourValue;

        public void DrawLine(float x0, float y0, float x1, float y1, float contourValue)
        {
            // If the previous line connects to this line, add the second point of this line to the polyline.
            if (x1 == x0Prev && y1 == y0Prev)
            {
                currentLine.Add(new Point2D(x0, y0));
            }
            // If the previous line connects in the wrong direction, throw an exception.
            else if (x0 == x1Prev && y0 == y1Prev)
            {
                throw new Exception();
            }
            // If the previous line does not connect, create a new line.
            else
            {
                // If the current line is not empty, add it to the list.
                if (currentLine.Count > 0)
                {
                    Polyline2D polyline = new Polyline2D(currentLine.ToArray());
                    polyline.SetAttribute("contour", currentContourValue);
                    polylines.Add(polyline);
                }

                // Make a new line.
                currentLine = new List<Point2D>();
                currentLine.Add(new Point2D(x1, y1));
                currentLine.Add(new Point2D(x0, y0));
                currentContourValue = contourValue;
            }

            // Add the points to the traditional point list.
            points.Add(new PointF(x0, y0));
            points.Add(new PointF(x1, y1));

            x0Prev = x0;
            y0Prev = y0;
            x1Prev = x1;
            y1Prev = y1;
        }

        public Image GetImage(int width, int height)
        {
            // Make the image.
            Image image = new Bitmap(width, height);

            if (points.Count > 0)
            {
                // Find the minimum and maximum x and y values.
                float xMin = points[0].X;
                float xMax = points[0].X;
                float yMin = points[0].Y;
                float yMax = points[0].Y;
                for (int i = 1; i < points.Count; i++)
                {
                    xMin = Math.Min(xMin, points[i].X);
                    xMax = Math.Max(xMax, points[i].X);
                    yMin = Math.Min(yMin, points[i].Y);
                    yMax = Math.Max(yMax, points[i].Y);
                }
                float xRange = xMax - xMin;
                float yRange = yMax - yMin;

                if (xRange > 0.0f && yRange > 0.0f)
                {
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    for (int i = 0; i < points.Count; i += 2)
                    {
                        float x0 = (points[i].X - xMin) / xRange * width;
                        float y0 = (points[i].Y - yMin) / yRange * height;
                        float x1 = (points[i + 1].X - xMin) / xRange * width;
                        float y1 = (points[i + 1].Y - yMin) / yRange * height;
                        g.DrawLine(Pens.Blue, new Point((int)x0, (int)y0), new Point((int)x1, (int)y1));
                    }

                    g.Dispose();
                }
            }


            return image;
        }

        public int GetNumLines()
        {
            return points.Count / 2;
        }

        public void GetRange(out float xMin, out float xMax, out float yMin, out float yMax)
        {
            // Set the minimum and maximum values to NaN. We'll be using the NaN-aware minimum and maximum functions, so these will be replaced by
            // the first non-NaN values encountered.
            xMin = float.NaN;
            xMax = float.NaN;
            yMin = float.NaN;
            yMax = float.NaN;

            // Determine the minimum and maximum values of the set of points.
            for (int i = 0; i < points.Count; i++)
            {
                xMin = MathUtil.NanAwareMin(xMin, points[i].X);
                xMax = MathUtil.NanAwareMax(xMax, points[i].X);
                yMin = MathUtil.NanAwareMin(yMin, points[i].Y);
                yMax = MathUtil.NanAwareMax(yMax, points[i].Y);
            }
        }

        public Polyline2D[] GetPolylines()
        {
            // If the current line is not empty, add it to the list.
            if (currentLine.Count > 0)
            {
                Polyline2D polyline = new Polyline2D(currentLine.ToArray());
                polyline.SetAttribute("contour", currentContourValue);
                polylines.Add(polyline);

                currentLine = new List<Point2D>();
            }

            Console.WriteLine("There are " + polylines.Count + " polylines in the set.");

            // Return the polylines.
            return polylines.ToArray();
        }

        public Line GetLine(int index)
        {
            return new Line(points[index * 2].X, points[index * 2].Y, points[index * 2 + 1].X, points[index * 2 + 1].Y);
        }
    }
}
