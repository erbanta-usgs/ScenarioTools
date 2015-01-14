#ifndef SHAPEPOINT2D_H
#define SHAPEPOINT2D_H

#include <vector>
#include <QMap>
#include <QString>
#include <QList>

#include "range2d.h"
#include "attribute.h"
#include "point2d.h"

class Polygon2D;

class ShapePoint2D
{
public:
    ShapePoint2D();
    ShapePoint2D(Point2D vertex);
    Range2D range();
    double length();
    int numVertices();
    Point2D vertex(int index);

    std::vector<ShapePoint2D *> *createPolylinesWithinPolygon(Polygon2D *polygon);

    void setDataAttribute(QString key, void *value);
    void setDoubleAttribute(QString key, double value);
    void setIntegerAttribute(QString key, int value);
    void setStringAttribute(QString key, QString value);

    void *getDataAttribute(QString key);
    double getDoubleAttribute(QString key);
    int getIntegerAttribute(QString key);
    QString getStringAttribute(QString key);
    QString getAttribute(QString key);

    double X();
    double Y();

    QList<QString> getDataAttributeKeys();
    QList<QString> getDoubleAttributeKeys();
    QList<QString> getIntegerAttributeKeys();
    QList<QString> getStringAttributeKeys();

    ~ShapePoint2D();

private:
    void computeRange();
    void computeLength();

    Point2D _vertex;
    Range2D _range;
    double _length;

    void copyAttributes(ShapePoint2D *target);

    QMap<QString, void *> *_dataAttributes;
    QMap<QString, double> *_doubleAttributes;
    QMap<QString, int> *_integerAttributes;
    QMap<QString, QString> *_stringAttributes;
};

#endif // SHAPEPOINT2D_H
