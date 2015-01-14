#ifndef POLYLINE2DSET_H
#define POLYLINE2DSET_H

#include <vector>

class Polyline2D;
class Polygon2DSet;

class Polyline2DSet
{
public:
    Polyline2DSet();
    Polyline2DSet(Polyline2D **polylines, int numPolylines);
    void initFromShapefile(const QString filename);
    ~Polyline2DSet();

    Polyline2D *polyline(int index);
    int numPolylines();
    int shapeType();
    double minBounds(int dimension);
    double maxBounds(int dimension);

    Polyline2DSet *createClippedSetWithPolygons(Polygon2DSet *polygons);
    void writeToShapefile(const QString filename);

private:
    Polyline2D **_polylines;
    int _numPolylines;
    int _shapeType;
    double _minBounds[4];
    double _maxBounds[4];

    QList<QString> getDoubleAttributeKeys();
    QList<QString> getIntegerAttributeKeys();
    QList<QString> getStringAttributeKeys();
};

#endif // POLYLINE2DSET_H
