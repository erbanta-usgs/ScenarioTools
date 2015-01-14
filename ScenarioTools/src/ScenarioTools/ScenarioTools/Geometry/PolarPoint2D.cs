using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Geometry
{
    public class PolarPoint2D
    {
        private double r;
        private double theta;

        public PolarPoint2D(double r, double theta)
        {
            this.r = r;
            this.theta = theta;
        }
        public double getR()
        {
            return r;
        }
        public double getTheta()
        {
            return theta;
        }
        public String toString()
        {
            return r + "," + theta;
        }
        public Point2D toCartesian()
        {
            double x = r * Math.Cos(theta);
            double y = r * Math.Sin(theta);
            return new Point2D(x, y);
        }
    }

}
