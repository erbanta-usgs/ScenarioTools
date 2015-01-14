#include <stdlib.h>
#include <QDebug>

#include "orthogrid.h"
#include "point2d.h"
#include "polygon2d.h"
#include "polygon2dset.h"
#include "attributekeys.h"

OrthoGrid::OrthoGrid()
{
    _nCols = 0;
    _nRows = 0;
    _cellWidths = NULL;
    _cellHeights = NULL;
}

OrthoGrid::OrthoGrid(int nCols, int nRows, double cellSize, Point2D anchor)
{
    // Make the cell widths array.
    _nCols = nCols;
    _cellWidths = new double[_nCols];
    for (int i = 0; i < _nCols; i++)
    {
        _cellWidths[i] = cellSize;
    }

    // Make the cell heights array.
    _nRows = nRows;
    _cellHeights = new double[_nRows];
    for (int i = 0; i < _nRows; i++)
    {
        _cellHeights[i] = cellSize;
    }

    // Store the anchor.
    _anchor = anchor;

    // Make the cell bounds arrays.
    makeCellBoundsArrays();
}

void OrthoGrid::makeCellBoundsArrays()
{
    // Make the cell bounds x-array.
    _cellBoundsX = new double[_nCols + 1];
    _cellBoundsX[0] = _anchor.X;
    for (int i = 0; i < _nCols; i++)
    {
        _cellBoundsX[i + 1] = _cellBoundsX[i] + _cellWidths[i];
    }

    // Make the cell bounds y-array.
    _cellBoundsY = new double[_nRows + 1];
    _cellBoundsY[0] = _anchor.Y;
    for (int i = 0; i < _nRows; i++)
    {
        _cellBoundsY[i + 1] = _cellBoundsY[i] + _cellHeights[i];
    }
}
float OrthoGrid::averageCellWidth() {
    // Find the sum of all cell widths.
    double sumCellWidth = 0.0f;
    for (int i = 0; i < _nCols; i++) {
        sumCellWidth += _cellWidths[i];
    }

    // Return the average.
    return sumCellWidth / _nCols;
}
float OrthoGrid::averageCellHeight() {
    // Find the sum of all cell widths.
    double sumCellHeight = 0.0f;
    for (int i = 0; i < _nRows; i++) {
        sumCellHeight += _cellHeights[i];
    }

    // Return the average.
    return sumCellHeight / _nRows;
}

Polygon2DSet *OrthoGrid::createPolygonSet()
{
    // Make the array for the polygons.
    int numPolygons = _nCols * _nRows;
    Polygon2D **polygons = new Polygon2D*[numPolygons];

    // Make the polygons.
    int index = 0;
    for (int j = 0; j < _nRows; j++)
    {
        for (int i = 0; i < _nCols; i++)
        {
            double x0 = _cellBoundsX[i];
            double x1 = _cellBoundsX[i + 1];
            double y0 = _cellBoundsY[_nRows - j];
            double y1 = _cellBoundsY[_nRows - j - 1];

            Point2D *vertices = new Point2D[4];
            vertices[0] = Point2D(x0, y0);
            vertices[1] = Point2D(x1, y0);
            vertices[2] = Point2D(x1, y1);
            vertices[3] = Point2D(x0, y1);

            // This is where we would rotate the vertices for a rotated grid.

            Polygon2D *cell = new Polygon2D(vertices, 4);
            cell->setIntegerAttribute(COL_KEY, i + 1);
            cell->setIntegerAttribute(ROW_KEY, j + 1);

            polygons[index++] = cell;
        }
    }

    // Return a set of the polygons.
    return new Polygon2DSet(polygons, numPolygons);
}

OrthoGrid::~OrthoGrid()
{
    if (_cellWidths != NULL)
    {
        delete[] _cellWidths;
    }

    if (_cellHeights != NULL)
    {
        delete[] _cellHeights;
    }

    if (_cellBoundsX != NULL)
    {
        delete[] _cellBoundsX;
    }

    if (_cellBoundsY != NULL)
    {
        delete[] _cellBoundsY;
    }
}

int OrthoGrid::nCols() {
    return _nCols;
}

int OrthoGrid::nRows() {
    return _nRows;
}


