#ifndef SHAPEPOINT2DSET_H
#define SHAPEPOINT2DSET_H

#include <vector>

class ShapePoint2D;
class Polygon2DSet;

class ShapePoint2DSet
{
public:
    ShapePoint2DSet();
    ShapePoint2DSet(ShapePoint2D **points, int numPoints);
    void initFromShapefile(const QString filename);
    ~ShapePoint2DSet();

    ShapePoint2D *point(int index);
    int numPoints();
    int shapeType();
    double minBounds(int dimension);
    double maxBounds(int dimension);

    ShapePoint2DSet *createClippedSetWithPolygons(Polygon2DSet *polygons);
    void assignGridCells(Polygon2DSet *polygons);
    void writeToShapefile(const QString filename);

private:
    ShapePoint2D **_points;
    int _numPoints;
    int _shapeType;
    double _minBounds[4];
    double _maxBounds[4];

    QList<QString> getDoubleAttributeKeys();
    QList<QString> getIntegerAttributeKeys();
    QList<QString> getStringAttributeKeys();
};

#endif // SHAPEPOINT2DSET_H

