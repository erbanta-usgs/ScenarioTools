#ifndef POLYLINE2D_H
#define POLYLINE2D_H

#include <vector>
#include <QMap>
#include <QString>
#include <QList>

#include "range2d.h"
#include "attribute.h"

class Point2D;
class Polygon2D;

class Polyline2D
{
public:
    Polyline2D();
    Polyline2D(Point2D *vertices, int numVertices);
    Range2D range();
    double length();
    int numVertices();
    Point2D vertex(int index);

    std::vector<Polyline2D *> *createPolylinesWithinPolygon(Polygon2D *polygon);

    void setDataAttribute(QString key, void *value);
    void setDoubleAttribute(QString key, double value);
    void setIntegerAttribute(QString key, int value);
    void setStringAttribute(QString key, QString value);

    void *getDataAttribute(QString key);
    double getDoubleAttribute(QString key);
    int getIntegerAttribute(QString key);
    QString getStringAttribute(QString key);
    QString getAttribute(QString key);

    QList<QString> getDataAttributeKeys();
    QList<QString> getDoubleAttributeKeys();
    QList<QString> getIntegerAttributeKeys();
    QList<QString> getStringAttributeKeys();

    ~Polyline2D();

private:
    void computeRange();
    void computeLength();

    Point2D *_vertices;
    int _numVertices;
    Range2D _range;
    double _length;

    void copyAttributes(Polyline2D *target);

    QMap<QString, void *> *_dataAttributes;
    QMap<QString, double> *_doubleAttributes;
    QMap<QString, int> *_integerAttributes;
    QMap<QString, QString> *_stringAttributes;
};

#endif // POLYLINE2D_H
