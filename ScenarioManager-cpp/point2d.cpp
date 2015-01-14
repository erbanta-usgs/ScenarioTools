#include "point2d.h"
#include "math.h"

Point2D::Point2D(double x, double y)
{
    X = x;
    Y = y;
}

double Point2D::distanceToPoint(Point2D p) {
    double dx = X - p.X;
    double dy = Y - p.Y;
    return sqrt(dx * dx + dy * dy);
}


