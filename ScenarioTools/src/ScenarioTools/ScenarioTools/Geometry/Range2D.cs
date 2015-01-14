using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ScenarioTools.Geometry
{
    public class Range2D
    {
        Range1D xRange, yRange;

        public Range2D(Range1D xRange, Range1D yRange)
        {
            this.xRange = xRange;
            this.yRange = yRange;
        }
        public Range1D getXRange()
        {
            return xRange;
        }
        public Range1D getYRange()
        {
            return yRange;
        }
        public override string ToString()
        {
            return "x:" + xRange + ",y:" + yRange;
        }
        public bool overlaps(Range2D range)
        {
            return getXRange().overlaps(range.getXRange()) &&
            getYRange().overlaps(range.getYRange());
        }
        public static Point2D mapPoint(Point2D p, Range2D fromRange, Range2D toRange, bool flipYRange)
        {
            // Map the one-dimensional values.
            double x = Range1D.mapValue(p.X, fromRange.getXRange(), toRange.getXRange(), false);
            double y = Range1D.mapValue(p.Y, fromRange.getYRange(), toRange.getYRange(), flipYRange);

            // Return the resultant point.
            return new Point2D(x, y);
        }
        public static double mapSize(double size, Range2D fromRange, Range2D toRange)
        {
            // Return the average mapped size of the two dimensions.
            double x = Range1D.mapScale(size, fromRange.getXRange(), toRange.getXRange());
            double y = Range1D.mapScale(size, fromRange.getYRange(), toRange.getYRange());
            return (x + y) / 2;
        }
        public static Range2D buffer(Range2D range, double bufferPercentage)
        {
            double xAdd = range.getXRange().size() * bufferPercentage;
            double yAdd = range.getYRange().size() * bufferPercentage;

            double x0 = range.getXRange().getMin() - xAdd;
            double x1 = range.getXRange().getMax() + xAdd;
            double y0 = range.getYRange().getMin() - yAdd;
            double y1 = range.getYRange().getMax() + yAdd;

            return new Range2D(new Range1D(x0, x1), new Range1D(y0, y1));
        }
        public static Range2D normalize(Range2D toNormalize, Range2D aspectRatio)
        {
            // If either range is null, return null.
            if (toNormalize == null || aspectRatio == null)
                return null;

            // Determine the limiting scale.
            double xScale = aspectRatio.getXRange().size() / toNormalize.getXRange().size();
            double yScale = aspectRatio.getYRange().size() / toNormalize.getYRange().size();
            double scale = Math.Min(xScale, yScale);

            // Scale both ranges for the determined scale.
            double xAdd = ((aspectRatio.getXRange().size() / scale) - toNormalize.getXRange().size()) / 2;
            double yAdd = ((aspectRatio.getYRange().size() / scale) - toNormalize.getYRange().size()) / 2;
            double x0 = toNormalize.getXRange().getMin() - xAdd;
            double x1 = toNormalize.getXRange().getMax() + xAdd;
            double y0 = toNormalize.getYRange().getMin() - yAdd;
            double y1 = toNormalize.getYRange().getMax() + yAdd;

            // Return resultant range.
            return new Range2D(new Range1D(x0, x1), new Range1D(y0, y1));
        }
        public bool contains(Point2D p)
        {
            return getXRange().contains(p.X) && getYRange().contains(p.Y);
        }

        public static Range2D GetExtent(Shape2D[] shapeArray)
        {
            Range2D extent = null;

            for (int i = 0; i < shapeArray.Length; i++)
            {
                // Get the extent of the layer.
                Range2D layerExtent = shapeArray[i].Extent;

                if (extent == null)
                {
                    extent = layerExtent;
                }
                else
                {
                    extent = extent.Union(layerExtent);
                }
            }

            // Return the result.
            return extent;
        }

        public Range2D Union(Range2D extent)
        {
            if (extent == null)
            {
                return this;
            }
            else
            {
                Range1D xRange = this.getXRange().union(extent.getXRange());
                Range1D yRange = this.getYRange().union(extent.getYRange());
                return new Range2D(xRange, yRange);
            }
        }

        public Range2D PadBy(double padRatio)
        {
            Range1D xRange = this.getXRange().PadBy(padRatio);
            Range1D yRange = this.getYRange().PadBy(padRatio);

            return new Range2D(xRange, yRange);
        }

        public Range2D NormalizeTo(Range2D extent)
        {
            // Determine the aspect ratio of this range.
            double targetAspectRatio = extent.getXRange().size() / extent.getYRange().size();

            // Determine the aspect ratio of the other range.
            double sourceAspectRatio = this.getXRange().size() / this.getYRange().size();

            // If the other aspect ratio is less than this (the x-range is comparatively too small), add to the x-range.
            double xAdd, yAdd;
            if (sourceAspectRatio < targetAspectRatio)
            {
                xAdd = ((this.getXRange().size() * targetAspectRatio / sourceAspectRatio) - this.getXRange().size()) / 2.0f;
                yAdd = 0.0;
            }

            // Otherwise, add to the y-range.
            else if (sourceAspectRatio > targetAspectRatio) {
                xAdd = 0.0;
                yAdd = (this.getYRange().size() * sourceAspectRatio / targetAspectRatio - this.getYRange().size()) / 2.0f;
            }

            // If the ratios are exactly the same, do nothing.
            else {
                xAdd = 0.0;
                yAdd = 0.0;
            }

            double xMin = this.getXRange().getMin() - xAdd;
            double xMax = this.getXRange().getMax() + xAdd;
            double yMin = this.getYRange().getMin() - yAdd;
            double yMax = this.getYRange().getMax() + yAdd;

            return new Range2D(new Range1D(xMin, xMax), new Range1D(yMin, yMax));
        }
    }
}
