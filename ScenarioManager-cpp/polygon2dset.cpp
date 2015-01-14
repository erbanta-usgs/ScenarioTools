#include "polygon2dset.h"
#include "polygon2d.h"
#include "shapelib/shapefil.h"
#include "point2d.h"

#include <stdlib.h>

#include <QDebug>

Polygon2DSet::Polygon2DSet()
{
    _polygons = NULL;
    _numPolygons = 0;

    for (int i = 0; i < 4; i++)
    {
        _minBounds[i] = _maxBounds[i] = 0.0;
    }
}
Polygon2DSet::Polygon2DSet(Polygon2D **polygons, int numPolygons)
{
    _polygons = polygons;
    _numPolygons = numPolygons;
}

Polygon2DSet::~Polygon2DSet()
{
    qDebug() << "deleting polygon set";

    if (_polygons != NULL)
    {
        delete[] _polygons;
    }

    qDebug() << "done deleting polygon set";
}

void Polygon2DSet::initFromShapefile(const char *filename)
{
    // NOTE: Not currently reading attributes from the shapefile.

    // Open the shapefile and get the shapefile info.
    SHPHandle shapefile = SHPOpen(filename, "rb");
    SHPGetInfo(shapefile, &_numPolygons, &_shapeType, _minBounds, _maxBounds);
    qDebug() << "There are " << _numPolygons << " shapes in the shapefile.";

    // Read the shapes from the file.
    Polygon2D **polygons = new Polygon2D*[_numPolygons];
    for (int j = 0; j < _numPolygons; j++) {
        // Read the shape.
        SHPObject *shape = SHPReadObject(shapefile, j);

        // Make the vertex array.
        int numVertices = shape->nVertices;
        Point2D *vertices = new Point2D[numVertices];

        //qDebug() << "shape: " << j;

        // Read the vertices into the polyline.
        for (int i = 0; i < numVertices; i++) {
            //qDebug() << "vertex: " << i << " = " << shape->padfX[i];
            vertices[i] = Point2D(shape->padfX[i], shape->padfY[i]);
        }

        // Make the polyline and store it in the array.
        polygons[j] = new Polygon2D(vertices, numVertices);

        // Dispose of the shape and the vertex array.
        SHPDestroyObject(shape);
        delete[] vertices;
    }

    // Close the shapefile.
    SHPClose(shapefile);
}

Polygon2D *Polygon2DSet::polygon(int index)
{
    return _polygons[index];
}

int Polygon2DSet::numPolygons()
{
    return _numPolygons;
}

int Polygon2DSet::shapeType()
{
    return _shapeType;
}

double Polygon2DSet::minBounds(int dimension)
{
    return _minBounds[dimension];
}

double Polygon2DSet::maxBounds(int dimension)
{
    return _maxBounds[dimension];
}
