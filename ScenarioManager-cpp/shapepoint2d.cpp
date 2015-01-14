#include <stdlib.h>
#include <vector>
#include <QDebug>

#include "polygon2d.h"
#include "shapepoint2d.h"
#include "linesegment2d.h"
#include "attributekeys.h"

#define MIN(a,b) ((a)<(b)?(a):(b))
#define MAX(a,b) ((a)>(b)?(a):(b))

using namespace std;

ShapePoint2D::ShapePoint2D(Point2D vertex)
{
    // Store the vertex.
    _vertex = vertex;

    // Compute the range and the length.
    computeRange();
    computeLength();

    // Make the attribute maps.
    _dataAttributes = new QMap<QString, void *>();
    _doubleAttributes = new QMap<QString, double>();
    _integerAttributes = new QMap<QString, int>();
    _stringAttributes = new QMap<QString, QString>();
}

ShapePoint2D::~ShapePoint2D()
{
}

void ShapePoint2D::setDataAttribute(QString key, void *value) {
    _dataAttributes->insert(key, value);
}
void ShapePoint2D::setDoubleAttribute(QString key, double value) {
    _doubleAttributes->insert(key, value);
}
void ShapePoint2D::setIntegerAttribute(QString key, int value) {
    _integerAttributes->insert(key, value);
}
void ShapePoint2D::setStringAttribute(QString key, QString value) {
    _stringAttributes->insert(key, value);
}

void *ShapePoint2D::getDataAttribute(QString key) {
    if (_dataAttributes->contains(key)) {
        return _dataAttributes->find(key).value();
    }
    else {
        return NULL;
    }
}
double ShapePoint2D::getDoubleAttribute(QString key) {
    if (_doubleAttributes->contains(key)) {
        return _doubleAttributes->find(key).value();
    }
    else {
        return 0.0;
    }
}
int ShapePoint2D::getIntegerAttribute(QString key) {
    if (_integerAttributes->contains(key)) {
        return _integerAttributes->find(key).value();
    }
    else {
        return 0;
    }
}
QString ShapePoint2D::getStringAttribute(QString key) {
    if (_stringAttributes->contains(key)) {
        return _stringAttributes->find(key).value();
    }
    else {
        return "";
    }
}
QString ShapePoint2D::getAttribute(QString key) {
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

double ShapePoint2D::X() {
    return _vertex.X;
}
double ShapePoint2D::Y() {
    return _vertex.Y;
}

void ShapePoint2D::computeRange()
{
    _range = Range2D(Range1D(_vertex.X, _vertex.X), Range1D(_vertex.Y, _vertex.Y));
}
void ShapePoint2D::computeLength()
{
    _length = 0.0;
}

Range2D ShapePoint2D::range()
{
    return _range;
}

double ShapePoint2D::length()
{
    return _length;
}

int ShapePoint2D::numVertices()
{
    return 1;
}
Point2D ShapePoint2D::vertex(int index)
{
    return _vertex;
}

QList<QString> ShapePoint2D::getDoubleAttributeKeys() {
    return _doubleAttributes->keys();
}
QList<QString> ShapePoint2D::getIntegerAttributeKeys() {
    return _integerAttributes->keys();
}
QList<QString> ShapePoint2D::getStringAttributeKeys() {
    return _stringAttributes->keys();
}

vector<ShapePoint2D *> *ShapePoint2D::createPolylinesWithinPolygon(Polygon2D *polygon)
{
    /*
    // Make a list for the polyline segments.
    vector<ShapePoint2D *> *clippedPolylines = new vector<ShapePoint2D *>();

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
            ShapePoint2D *clippedPolyline = new ShapePoint2D(vertices, 2);

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
    */
    return NULL;
}

void ShapePoint2D::copyAttributes(ShapePoint2D *target) {
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
