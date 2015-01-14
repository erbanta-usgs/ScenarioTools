#ifndef POINT2D_H
#define POINT2D_H

class Point2D
{
public:
    Point2D(double x = 0.0, double y = 0.0);
    double distanceToPoint(Point2D p);

    double X;
    double Y;
};

#endif // POINT2D_H


