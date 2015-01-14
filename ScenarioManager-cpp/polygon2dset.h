#ifndef POLYGON2DSET_H
#define POLYGON2DSET_H

class Polygon2D;

class Polygon2DSet
{
public:
    Polygon2DSet();
    Polygon2DSet(Polygon2D **polygons, int numPolygons);
    void initFromShapefile(const char *filename);
    ~Polygon2DSet();

    Polygon2D *polygon(int index);
    int numPolygons();
    int shapeType();
    double minBounds(int dimension);
    double maxBounds(int dimension);

private:
    Polygon2D **_polygons;
    int _numPolygons;
    int _shapeType;
    double _minBounds[4];
    double _maxBounds[4];

};

#endif // POLYLINE2DSET_H
