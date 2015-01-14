#ifndef POLYGON2D_H
#define POLYGON2D_H

#include "range2d.h"
#include "linesegment2d.h"
#include <vector>
#include <QString>
#include <QMap>

class Point2D;

class Polygon2D
{
public:
    Polygon2D(Point2D *vertices, int numVertices);
    Range2D range();
    ~Polygon2D();
    std::vector<LineSegment2D> *createSplitSegments(LineSegment2D segment);
    std::vector<Point2D> *createIntersectionPoints(LineSegment2D segment);
    bool contains(Point2D point);

    void setDataAttribute(QString key, void *value);
    void setDoubleAttribute(QString key, double value);
    void setIntegerAttribute(QString key, int value);
    void setStringAttribute(QString key, QString value);

    void *getDataAttribute(QString key);
    double getDoubleAttribute(QString key);
    int getIntegerAttribute(QString key);
    QString getStringAttribute(QString key);

    QList<QString> getDataAttributeKeys();
    QList<QString> getDoubleAttributeKeys();
    QList<QString> getIntegerAttributeKeys();
    QList<QString> getStringAttributeKeys();

private:
    void computeRange();

    Range2D _range;
    Point2D *_vertices;
    int _numVertices;

    QMap<QString, void *> *_dataAttributes;
    QMap<QString, double> *_doubleAttributes;
    QMap<QString, int> *_integerAttributes;
    QMap<QString, QString> *_stringAttributes;
};

#endif // POLYGON2D_H
