#ifndef LINESEGMENT2D_H
#define LINESEGMENT2D_H

#include "point2d.h"

class Range1D;

class LineSegment2D
{
public:
    LineSegment2D(Point2D p1 = Point2D(), Point2D p2 = Point2D());
    Point2D P1;
    Point2D P2;

    Point2D midpoint();
    double length();
    Range1D xRange();
    Range1D yRange();
    double slope();
    double distanceToPoint(Point2D p);
    double distanceToSegment(LineSegment2D segment);
    double uToPoint(Point2D p);
    bool uToSegment(LineSegment2D segment, double &ua, double &ub);
    bool intersectsSegment(LineSegment2D segment);
    Point2D intersectionPoint(LineSegment2D segment);
};

#endif // LINESEGMENT2D_H
