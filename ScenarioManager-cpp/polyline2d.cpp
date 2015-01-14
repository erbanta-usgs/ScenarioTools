#include <stdlib.h>
#include <vector>
#include <QDebug>

#include "polygon2d.h"
#include "polyline2d.h"
#include "point2d.h"
#include "linesegment2d.h"
#include "attributekeys.h"

#define MIN(a,b) ((a)<(b)?(a):(b))
#define MAX(a,b) ((a)>(b)?(a):(b))

using namespace std;

Polyline2D::Polyline2D(Point2D *vertices, int numVertices)
{    
    // Store the vertices.
    _vertices = vertices;

    // Store the number of vertices.
    _numVertices = numVertices;

    // Compute the range and the length.
    computeRange();
    computeLength();

    // Make the attribute maps.
    _dataAttributes = new QMap<QString, void *>();
    _doubleAttributes = new QMap<QString, double>();
    _integerAttributes = new QMap<QString, int>();
    _stringAttributes = new QMap<QString, QString>();
}

Polyline2D::~Polyline2D()
{
    static int count = 0;

    qDebug() << "going to delete vertices of shape with " << _numVertices << "::" << count++;
    // Delete the vertex array.
    if (_vertices != NULL) {
        delete[] _vertices;
    }
    qDebug() << "deleted vertices";
}

void Polyline2D::setDataAttribute(QString key, void *value) {
    _dataAttributes->insert(key, value);
}
void Polyline2D::setDoubleAttribute(QString key, double value) {
    _doubleAttributes->insert(key, value);
}
void Polyline2D::setIntegerAttribute(QString key, int value) {
    _integerAttributes->insert(key, value);
}
void Polyline2D::setStringAttribute(QString key, QString value) {
    _stringAttributes->insert(key, value);
}

void *Polyline2D::getDataAttribute(QString key) {
    if (_dataAttributes->contains(key)) {
        return _dataAttributes->find(key).value();
    }
    else {
        return NULL;
    }
}
double Polyline2D::getDoubleAttribute(QString key) {
    if (_doubleAttributes->contains(key)) {
        return _doubleAttributes->find(key).value();
    }
    else {
        return 0.0;
    }
}
int Polyline2D::getIntegerAttribute(QString key) {
    if (_integerAttributes->contains(key)) {
        return _integerAttributes->find(key).value();
    }
    else {
        return 0;
    }
}
QString Polyline2D::getStringAttribute(QString key) {
    if (_stringAttributes->contains(key)) {
        return _stringAttributes->find(key).value();
    }
    else {
        return "";
    }
}
QString Polyline2D::getAttribute(QString key) {
    if (_stringAttributes->contains(key)) {
        return _stringAttributes->find(key).value();
    }
    else if (_integerAttributes->contains(key)) {
        return QString("%1").arg(_integerAttributes->find(key).value());
    }
    else if (_doubleAttributes->contains(key)) {
        return QString("%1").arg(_doubleAttributes->find(key).value());
    }
    else {
        return "";
    }
}

void Polyline2D::computeRange()
{
    if (_numVertices == 0)
    {
        _range = Range2D(Range1D(0.0, 0.0), Range1D(0.0, 0.0));
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
    }

    // qDebug() << "done computing range";
}
void Polyline2D::computeLength()
{
    _length = 0.0;
    for (int i = 0; i < _numVertices - 1; i++)
    {
        _length += _vertices[i].distanceToPoint(_vertices[i + 1]);
    }
}

Range2D Polyline2D::range()
{
    return _range;
}

double Polyline2D::length()
{
    return _length;
}

int Polyline2D::numVertices()
{
    return _numVertices;
}
Point2D Polyline2D::vertex(int index)
{
    return _vertices[index];
}

QList<QString> Polyline2D::getDoubleAttributeKeys() {
    return _doubleAttributes->keys();
}
QList<QString> Polyline2D::getIntegerAttributeKeys() {
    return _integerAttributes->keys();
}
QList<QString> Polyline2D::getStringAttributeKeys() {
    return _stringAttributes->keys();
}

vector<Polyline2D *> *Polyline2D::createPolylinesWithinPolygon(Polygon2D *polygon)
{
    // Make a list for the polyline segments.
    vector<Polyline2D *> *clippedPolylines = new vector<Polyline2D *>();

    //qDebug() << "going to split a polyline with " << this->numVertices() << " vertices";

    // Iterate over all segments in this polyline.
    for (int j = 0; j < _numVertices - 1; j++)
    {
        // Make the segment.
        Point2D start = _vertices[j];
        Point2D end = _vertices[j + 1];
        LineSegment2D segment = LineSegment2D(start, end);

        // Split the segment.
        vector<LineSegment2D> *segments = polygon->createSplitSegments(segment);    // polygon->createSplitSegments(segment, &numSegmentsFromCurrentSegment);

        // Add the segments to the list of clipped polylines.
        for (uint i = 0; i < segments->size(); i++)
        {
            // Make the clipped polyline.
            Point2D *vertices = new Point2D[2];
            vertices[0] = segments->at(i).P1;
            vertices[1] = segments->at(i).P2;
            Polyline2D *clippedPolyline = new Polyline2D(vertices, 2);

            // Copy the attributes from the source (this) polyline to the clipped polyline and attach the row and column.
            copyAttributes(clippedPolyline);
            clippedPolyline->setIntegerAttribute(COL_KEY, polygon->getIntegerAttribute(COL_KEY));
            clippedPolyline->setIntegerAttribute(ROW_KEY, polygon->getIntegerAttribute(ROW_KEY));

            // Compute and attach the conductance.
            double conductanceRatio = clippedPolyline->getDoubleAttribute(CONDUCTANCE_RATIO_KEY);
            double conductance = clippedPolyline->length() * conductanceRatio;
            clippedPolyline->setDoubleAttribute(CONDUCTANCE_KEY, conductance);

            // Add the clipped polyline to the vector.
            clippedPolylines->push_back(clippedPolyline);
        }

        //  Delete the segment array.
        if (segments != NULL)
        {
            delete segments;
        }
    }

    return clippedPolylines;
}

void Polyline2D::copyAttributes(Polyline2D *target) {
    // Copy the data attributes.
    QMap<QString, void *>::const_iterator dataIterator = _dataAttributes->constBegin();
    while (dataIterator != _dataAttributes->constEnd()) {
        target->setDataAttribute(dataIterator.key(), dataIterator.value());
        dataIterator++;
    }

    // Copy the double attributes.
    QMap<QString, double>::const_iterator doubleIterator = _doubleAttributes->constBegin();
    while (doubleIterator != _doubleAttributes->constEnd()) {
        target->setDoubleAttribute(doubleIterator.key(), doubleIterator.value());
        doubleIterator++;
    }

    // Copy the integer attributes.
    QMap<QString, int>::const_iterator integerIterator = _integerAttributes->constBegin();
    while (integerIterator != _integerAttributes->constEnd()) {
        target->setIntegerAttribute(integerIterator.key(), integerIterator.value());
        integerIterator++;
    }

    // Copy the string attributes.
    QMap<QString, QString>::const_iterator stringIterator = _stringAttributes->constBegin();
    while (stringIterator != _stringAttributes->constEnd()) {
        target->setStringAttribute(stringIterator.key(), stringIterator.value());
        stringIterator++;
    }
}
