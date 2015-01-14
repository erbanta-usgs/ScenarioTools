using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ScenarioTools.Geometry
{
    public static class GeometryHelper
    {
        public static Range2D CalculateRange(Point2D[] vertices)
        {
            // If the vertex array is empty, return a NaN range.
            if (vertices.Length == 0)
            {
                return new Range2D(new Range1D(double.NaN, double.NaN), new Range1D(double.NaN, double.NaN));
            }

            // Calculate the range.
            double xMin = vertices[0].X;
            double xMax = xMin;
            double yMin = vertices[0].Y;
            double yMax = yMin;
            for (int i = 1; i < vertices.Length; i++)
            {
                xMin = Math.Min(xMin, vertices[i].X);
                xMax = Math.Max(xMax, vertices[i].X);
                yMin = Math.Min(yMin, vertices[i].Y);
                yMax = Math.Max(yMax, vertices[i].Y);
            }

            // Return the result.
            return new Range2D(new Range1D(xMin, xMax), new Range1D(yMin, yMax));
        }
        public static Rectangle MakeRectangle(Point point0, Point point1)
        {
            // Find upper left corner and size
            int minX = Math.Min(point0.X, point1.X);
            int minY = Math.Min(point0.Y, point1.Y);
            int width = Math.Abs(point0.X - point1.X);
            int height = Math.Abs(point0.Y - point1.Y);

            return new Rectangle(minX, minY, width, height);
        }
    }
}
