using System;
using System.Collections.Generic;
using System.Text;
using ScenarioTools.Util;

namespace ScenarioTools.Geometry
{
    public class LineSegment2D
    {

        Point2D p1, p2;

        public LineSegment2D(Point2D p1, Point2D p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
        public Point2D midpoint()
        {
            return new Point2D((p1.GetX() + p2.GetX()) / 2, (p1.GetY() + p2.GetY()) / 2);
        }
        public Range1D GetXRange()
        {
            double x0 = (p1.GetX() < p2.GetX() ? p1.GetX() : p2.GetX());
            double x1 = (p1.GetX() > p2.GetX() ? p1.GetX() : p2.GetX());

            return new Range1D(x0, x1);
        }
        public Range1D GetYRange()
        {
            double y0 = (p1.GetY() < p2.GetY() ? p1.GetY() : p2.GetY());
            double y1 = (p1.GetY() > p2.GetY() ? p1.GetY() : p2.GetY());

            return new Range1D(y0, y1);
        }
        public override string ToString()
        {
            return p1 + ";" + p2;
        }
        public double slope()
        {
            return (p2.GetY() - p1.GetY()) / (p2.GetX() - p1.GetX());
        }
        public Point2D getP1()
        {
            return p1;
        }
        public Point2D getP2()
        {
            return p2;
        }
        public Point2D getClosestPoint(Point2D p)
        {
            double dx = p2.GetX() - p1.GetX();
            double dy = p2.GetY() - p1.GetY();

            if (dx == 0 && dy == 0)
                return p1;

            double u = ((p.GetX() - p1.GetX()) * dx + (p.GetY() - p1.GetY()) * dy) /
            (dx * dx + dy * dy);

            Point2D closestPoint;
            if (u < 0)
                closestPoint = p1;
            else if (u > 1)
                closestPoint = p2;
            else
                closestPoint = new Point2D(p1.GetX() + u * dx, p1.GetY() + u * dy);

            return closestPoint;
        }
        public double distance(Point2D p)
        {
            Point2D closestPoint = this.getClosestPoint(p);
            
            return closestPoint.distance(p);
        }
        public double getU(Point2D p)
        {
            double dx = p2.GetX() - p1.GetX();
            double dy = p2.GetY() - p1.GetY();

            if (dx == 0 && dy == 0)
                return 0.0;

            else return ((p.GetX() - p1.GetX()) * dx + (p.GetY() - p1.GetY()) * dy) /
            (dx * dx + dy * dy);
        }
        public bool intersects(LineSegment2D segment)
        {
            Point2D p = intersection(segment, true);

            return p != null;
        }
        public double distance(LineSegment2D segment)
        {
            // If this segment intersects the specified segment, return zero.
            if (this.intersects(segment))
                return 0.0;

            // Otherwise, return the minimum distance between the endpoints of this and the
            // specified segment and the other segment.
            else
            {
                Double minDistance = this.distance(segment.getP1());
                minDistance = Math.Min(minDistance, this.distance(segment.getP2()));
                minDistance = Math.Min(minDistance, segment.distance(this.getP1()));
                minDistance = Math.Min(minDistance, segment.distance(this.getP2()));

                return minDistance;
            }
        }
        public Point2D intersection(LineSegment2D segment, bool testSegment)
        {
            Point2D p1 = getP1();
            Point2D p2 = getP2();
            Point2D p3 = segment.getP1();
            Point2D p4 = segment.getP2();

            double denom = (p4.GetY() - p3.GetY()) * (p2.GetX() - p1.GetX()) -
            (p4.GetX() - p3.GetX()) * (p2.GetY() - p1.GetY());

            if (denom == 0)
                return null;

            double numA = (p4.GetX() - p3.GetX()) * (p1.GetY() - p3.GetY()) -
            (p4.GetY() - p3.GetY()) * (p1.GetX() - p3.GetX());
            double ua = numA / denom;

            double numB = (p2.GetX() - p1.GetX()) * (p1.GetY() - p3.GetY()) -
            (p2.GetY() - p1.GetY()) * (p1.GetX() - p3.GetX());
            double ub = numB / denom;

            double x = p1.GetX() + ua * (p2.GetX() - p1.GetX());
            double y = p1.GetY() + ua * (p2.GetY() - p1.GetY());

            if (testSegment)
                if ((ua >= 0 && ua <= 1) && (ub >= 0 && ub <= 1))
                    return new Point2D(x, y);
                else
                    return null;

            return new Point2D(x, y);
        }
        public Pair<Double, Double> getU(LineSegment2D segment)
        {
            Point2D p1 = getP1();
            Point2D p2 = getP2();
            Point2D p3 = segment.getP1();
            Point2D p4 = segment.getP2();

            double denom = (p4.GetY() - p3.GetY()) * (p2.GetX() - p1.GetX()) -
            (p4.GetX() - p3.GetX()) * (p2.GetY() - p1.GetY());

            if (denom == 0)
                return null;

            double numA = (p4.GetX() - p3.GetX()) * (p1.GetY() - p3.GetY()) -
            (p4.GetY() - p3.GetY()) * (p1.GetX() - p3.GetX());
            double ua = numA / denom;

            double numB = (p2.GetX() - p1.GetX()) * (p1.GetY() - p3.GetY()) -
            (p2.GetY() - p1.GetY()) * (p1.GetX() - p3.GetX());
            double ub = numB / denom;

            return new Pair<Double, Double>(ua, ub);
        }
        public double getLength()
        {
            return p1.distance(p2);
        }

        public Point2D Midpoint()
        {
            return new Point2D((p1.X + p2.X) / 2.0, (p1.Y + p2.Y) / 2.0);
        }

        public Polyline2D ToPolyline()
        {
            Point2D[] vertices = { p1, p2 };
            return new Polyline2D(vertices);
        }
    }
}
