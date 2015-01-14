#include <stdlib.h>
#include <algorithm>
#include <QDebug>

#include "polygon2d.h"
#include "point2d.h"

#include "attributekeys.h"

#define MIN(a,b) ((a)<(b)?(a):(b))
#define MAX(a,b) ((a)>(b)?(a):(b))

using namespace std;

Polygon2D::Polygon2D(Point2D *vertices, int numVertices)
{
    // Store the vertices.
    _vertices = vertices;

    // Store the number of vertices.
    _numVertices = numVertices;

    // Compute the range.
    computeRange();

    // Make the attribute maps.
    _dataAttributes = new QMap<QString, void *>();
    _doubleAttributes = new QMap<QString, double>();
    _integerAttributes = new QMap<QString, int>();
    _stringAttributes = new QMap<QString, QString>();
}

void Polygon2D::setDataAttribute(QString key, void *value) {
    _dataAttributes->insert(key, value);
}
void Polygon2D::setDoubleAttribute(QString key, double value) {
    _doubleAttributes->insert(key, value);
}
void Polygon2D::setIntegerAttribute(QString key, int value) {
    _integerAttributes->insert(key, value);
}
void Polygon2D::setStringAttribute(QString key, QString value) {
    _stringAttributes->insert(key, value);
}

void *Polygon2D::getDataAttribute(QString key) {
    if (_dataAttributes->contains(key)) {
        return _dataAttributes->find(key).value();
    }
    else {
        return NULL;
    }
}
double Polygon2D::getDoubleAttribute(QString key) {
    if (_doubleAttributes->contains(key)) {
        return _doubleAttributes->find(key).value();
    }
    else {
        return 0.0;
    }
}
int Polygon2D::getIntegerAttribute(QString key) {
    if (_integerAttributes->contains(key)) {
       return _integerAttributes->find(key).value();
    }
    else {
        return 0;
    }
}
QString Polygon2D::getStringAttribute(QString key) {
    if (_stringAttributes->contains(key)) {
        return _stringAttributes->find(key).value();
    }
    else {
        return "";
    }
}

bool Polygon2D::contains(Point2D point)
{
    int numIntersections = 0;

    for (int i = 0; i < _numVertices; i++)
    {
        Point2D p1 = _vertices[i];
        Point2D p2 = _vertices[(i + 1) % _numVertices];

        if (point.Y > MIN(p1.Y, p2.Y))
        {
            if (point.Y <= MAX(p1.Y, p2.Y))
            {
                if (point.X <= MAX(p1.X, p2.X))
                {
                    if (p1.Y != p2.Y)
                    {
                        double xIntersection = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                        if (p1.X == p2.X || point.X <= xIntersection)
                        {
                            numIntersections++;
                        }
                    }
                }
            }
        }
    }

    return numIntersections % 2 == 1;
}

void Polygon2D::computeRange()
{
    if (_numVertices == 0)
    {
        _range = Range2D(Range1D(), Range1D());
    }
    else
    {
        double xMin = _vertices[0].X;
        double xMax = xMin;
        double yMin = _vertices[0].Y;
        double yMax = yMin;

        for (int i = 1; i < _numVertices; i++)
        {
            xMin = MIN(xMin, _vertices[i].X);
            xMax = MAX(xMax, _vertices[i].X);
            yMin = MIN(yMin, _vertices[i].Y);
            yMax = MAX(yMax, _vertices[i].Y);
        }

        _range = Range2D(Range1D(xMin, xMax), Range1D(yMin, yMax));
        //qDebug() << "computed range " << xMin << " - " << xMax << " : " << yMin << " - " << yMax;
    }
}

static bool sortXAscending(Point2D p1, Point2D p2)
{
    return p1.X < p2.X;
}
static bool sortXDescending(Point2D p1, Point2D p2)
{
    return p1.X > p2.X;
}
static bool sortYAscending(Point2D p1, Point2D p2)
{
    return p1.Y < p2.Y;
}
static bool sortYDescending(Point2D p1, Point2D p2)
{
    return p1.Y > p2.Y;
}

vector<Point2D> *Polygon2D::createIntersectionPoints(LineSegment2D segment)
{
    vector<Point2D> *intersectionPoints = new vector<Point2D>();

    // Find the intersections with of all sides with the specified segment.
    for (int i = 0; i < _numVertices; i++)
    {
        LineSegment2D side = LineSegment2D(_vertices[i], _vertices[(i + 1) % _numVertices]);
        if (side.intersectsSegment(segment))
        {
            intersectionPoints->push_back(side.intersectionPoint(segment));
        }
    }

    // Order the intersection points.
    if (segment.P1.X < segment.P2.X)
    {
        sort(intersectionPoints->begin(), intersectionPoints->end(), sortXAscending);
    }
    else if (segment.P1.X > segment.P2.X)
    {
        sort(intersectionPoints->begin(), intersectionPoints->end(), sortXDescending);
    }
    else if (segment.P1.Y < segment.P2.Y)
    {
        sort(intersectionPoints->begin(), intersectionPoints->end(), sortYAscending);
    }
    else
    {
        sort(intersectionPoints->begin(), intersectionPoints->end(), sortYDescending);
    }

    return intersectionPoints;
}

vector<LineSegment2D> *Polygon2D::createSplitSegments(LineSegment2D segmentToSplit)
{
    //qDebug() << "going to clip the segment by the polygons";

    // Get the intersection points. Add the end of the segment to this list for the purpose of assembling segments.
    vector<Point2D> *intersectionPoints = createIntersectionPoints(segmentToSplit);
    intersectionPoints->push_back(segmentToSplit.P2);

    // Make segments from these points.
    vector<LineSegment2D> *segments = new vector<LineSegment2D>();
    Point2D start = segmentToSplit.P1;
    int numPoints = intersectionPoints->size();
    for (int i = 0; i < numPoints; i++)
    {
        // Make the segment. If it is in this polygon, add it to the list of segments.
        LineSegment2D segment = LineSegment2D(start, intersectionPoints->at(i));
        int row = this->getIntegerAttribute(ROW_KEY);
        int col = this->getIntegerAttribute(COL_KEY);
        if (this->contains(segment.midpoint()))
        {
            //qDebug() << "accepting segment " << segment.length() << " -- " << row << ", " << col <<
                  //  " (" << segmentToSplit.P1.X << ", " << segmentToSplit.P1.Y << ") : " <<
                  //  " (" << segmentToSplit.P2.X << ", " << segmentToSplit.P2.Y << ")";
            segments->push_back(segment);
        }
        else {
            //qDebug() << "rejecting segment " << segment.length() << " -- " << row << ", " << col;
        }

        // Advance the point to create the next segment.
        start = intersectionPoints->at(i);
    }

    // Clean up the intersection points.
    if (intersectionPoints != NULL) {
        delete intersectionPoints;
    }

    // Return the result.
    return segments;
}

Range2D Polygon2D::range()
{
    return _range;
}

Polygon2D::~Polygon2D()
{
    // Delete the vertex array.
    if (_vertices != NULL) {
        delete[] _vertices;
    }
}
