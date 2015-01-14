#ifndef ORTHOGRID_H
#define ORTHOGRID_H

#include "point2d.h"

class Polygon2DSet;

class OrthoGrid
{
public:
    OrthoGrid();
    OrthoGrid(int nCols, int nRows, double cellSize, Point2D anchor);
    ~OrthoGrid();
    int nCols();
    int nRows();
    Polygon2DSet *createPolygonSet();
    float averageCellWidth();
    float averageCellHeight();

private:
    void makeCellBoundsArrays();

    int _nCols;
    int _nRows;
    double *_cellWidths;
    double *_cellHeights;
    double *_cellBoundsX;
    double *_cellBoundsY;
    Point2D _anchor;
};

#endif // ORTHOGRID_H
