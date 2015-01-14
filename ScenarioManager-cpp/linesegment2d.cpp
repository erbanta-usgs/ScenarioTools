#include <algorithm>

#include "linesegment2d.h"
#include "range1d.h"

LineSegment2D::LineSegment2D(Point2D p1, Point2D p2) {
    P1 = p1;
    P2 = p2;
}

Point2D LineSegment2D::midpoint() {
    return Point2D((P1.X + P2.X) * 0.5, (P1.Y + P2.Y) * 0.5);
}

double LineSegment2D::length() {
    return P1.distanceToPoint(P2);
}

Range1D LineSegment2D::xRange() {
    if (P1.X < P2.X) {
        return Range1D(P1.X, P2.X);
    }
    else {
        return Range1D(P2.X, P1.X);
    }
}

Range1D LineSegment2D::yRange() {
    if (P1.Y < P2.Y) {
        return Range1D(P1.Y, P2.Y);
    }
    else {
        return Range1D(P2.Y, P1.Y);
    }
}

double LineSegment2D::slope() {
    return (P2.Y - P1.Y) / (P2.X - P2.X);
}

double LineSegment2D::distanceToPoint(Point2D p) {
    double dx = P2.X - P1.X;
    double dy = P2.Y - P1.Y;

    // If the endpoints of this segment are the same, return the distance to this point.
    if (dx == 0.0 && dy == 0.0) {
        return p.distanceToPoint(P1);
    }

    // Calculate U (the ratio along this segment at which intersection of the normal occurs).
    double u = ((p.X - P1.X) * dx + (p.Y - P1.Y) * dy) / (dx * dx + dy * dy);

    // Find the point on this segment that is closest to the specified point.
    Point2D closestPoint;
    if (u < 0) {
        closestPoint = P1;
    }
    else if (u > 1) {
        closestPoint = P2;
    }
    else {
        closestPoint = Point2D(P1.X + u * dx, P1.Y + u * dy);
    }

    // Return the distance to the closest point.
    return closestPoint.distanceToPoint(p);
}

double LineSegment2D::distanceToSegment(LineSegment2D segment) {
    // If this segment intersects the specified segment, return zero.
    if (this->intersectsSegment(segment)) {
        return 0.0;
    }

    // Otherwise, return the minimum distance between the endpoints of this and the other segment.
    else {
        double minDistance = this->distanceToPoint(segment.P1);
        minDistance = std::min(minDistance, this->distanceToPoint(segment.P1));
        minDistance = std::min(minDistance, segment.distanceToPoint(P1));
        minDistance = std::min(minDistance, segment.distanceToPoint(P2));

        return minDistance;
    }
}

double LineSegment2D::uToPoint(Point2D p) {
    double dx = P2.X - P1.X;
    double dy = P2.Y - P1.Y;

    if (dx == 0.0 && dy == 0.0) {
        return 0.0;
    }

    else {
        return ((p.X - P1.X) * dx + (p.Y - P1.Y) * dy) / (dx * dx + dy * dy);
    }
}

bool LineSegment2D::uToSegment(LineSegment2D segment, double &ua, double &ub) {
    Point2D p3 = segment.P1;
    Point2D p4 = segment.P2;

    double denom = (p4.Y - p3.Y) * (P2.X - P1.X) - (p4.X - p3.X) * (P2.Y - P1.Y);

    if (denom == 0.0) {
        ua = 0.0;
        ub = 0.0;
        return false;
    }

    double numA = (p4.X - p3.X) * (P1.Y - p3.Y) - (p4.Y - p3.Y) * (P1.Y - p3.Y);
    ua = numA / denom;

    double numB = (P2.X - P1.X) * (P1.Y - p3.Y) - (P2.Y - P1.Y) * (P1.X - p3.X);
    ub = numB / denom;

    return true;
}

bool LineSegment2D::intersectsSegment(LineSegment2D segment) {
    Point2D p3 = segment.P1;
    Point2D p4 = segment.P2;

    double denom = (p4.Y - p3.Y) * (P2.X - P1.X) - (p4.X - p3.X) * (P2.Y - P1.Y);

    if (denom == 0.0) {
        return false;
    }

    double numA = (p4.X - p3.X) * (P1.Y - p3.Y) - (p4.Y - p3.Y) * (P1.X - p3.X);
    double ua = numA / denom;

    double numB = (P2.X - P1.X) * (P1.Y - p3.Y) - (P2.Y - P1.Y) * (P1.X - p3.X);
    double ub = numB / denom;

    return ((ua >= 0 && ua <= 1) && (ub >= 0 && ub <= 1));
}

Point2D LineSegment2D::intersectionPoint(LineSegment2D segment)
{
    Point2D p3 = segment.P1;
    Point2D p4 = segment.P2;

    double denom = (p4.Y - p3.Y) * (P2.X - P1.X) - (p4.X - p3.X) * (P2.Y - P1.Y);

    if (denom == 0.0) {
        return Point2D();
    }

    double numA = (p4.X - p3.X) * (P1.Y - p3.Y) - (p4.Y - p3.Y) * (P1.X - p3.X);
    double ua = numA / denom;

    double x = P1.X + ua * (P2.X - P1.X);
    double y = P1.Y + ua * (P2.Y - P1.Y);

    return Point2D(x, y);
}
