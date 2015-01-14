#include "shapepoint2d.h"
#include "shapepoint2dset.h"
#include "polygon2d.h"
#include "polygon2dset.h"
#include "shapelib/shapefil.h"
#include "point2d.h"
#include "linesegment2d.h"
#include "attributekeys.h"

#include <stdlib.h>
#include <vector>

#include <QDebug>

using namespace std;

ShapePoint2DSet::ShapePoint2DSet()
{
    _points = NULL;
    _numPoints = 0;

    for (int i = 0; i < 4; i++)
    {
        _minBounds[i] = _maxBounds[i] = 0.0;
    }
}

ShapePoint2DSet::ShapePoint2DSet(ShapePoint2D **points, int numPoints) {
    _points = points;
    _numPoints = numPoints;
}

ShapePoint2DSet::~ShapePoint2DSet()
{
    if (_points != NULL)
    {
        delete[] _points;
    }
}

void ShapePoint2DSet::initFromShapefile(const QString filename)
{
    // Open the shapefile and get the shapefile info.
    SHPHandle shapefile = SHPOpen(filename.toAscii(), "rb");
    SHPGetInfo(shapefile, &_numPoints, &_shapeType, _minBounds, _maxBounds);

    // Open the DBF.
    string dbfName = string(filename.toAscii());
    int length = dbfName.length();
    dbfName[length - 3] = 'd';
    dbfName[length - 2] = 'b';
    dbfName[length - 1] = 'f';
    DBFHandle dbfFile = DBFOpen(dbfName.c_str(), "rb");

    // Get the field names and types.
    int numFields = DBFGetFieldCount(dbfFile);
    char fieldName[12];
    QString *fieldNames = new QString[numFields];
    int *fieldTypes = new int[numFields];
    for (int i = 0; i < numFields; i++) {
        fieldTypes[i] = DBFGetFieldInfo(dbfFile, i, fieldName, NULL, NULL);
        fieldNames[i] = fieldName;
        qDebug() << fieldNames[i];
    }
    qDebug() << "the DBF has " << numFields << " fields";

    // Read the shapes from the file.
    _points = new ShapePoint2D*[_numPoints];
    for (int j = 0; j < _numPoints; j++) {
        // Read the shape.
        SHPObject *shape = SHPReadObject(shapefile, j);

        // Read the point.
        Point2D p(shape->padfX[0], shape->padfY[0]);

        // Make the point.
        ShapePoint2D *point = new ShapePoint2D(p);

        // Attach the attributes to the point.
        for (int i = 0; i < numFields; i++) {
            int fieldType = fieldTypes[i];
            QString key = fieldNames[i];
            if (fieldType == FTDouble) {
                double value = DBFReadDoubleAttribute(dbfFile, j, i);
                point->setDoubleAttribute(key, value);
            }
            else if (fieldType == FTInteger) {
                int value = DBFReadIntegerAttribute(dbfFile, j, i);
                point->setIntegerAttribute(key, value);
            }
            else if (fieldType == FTString) {
                QString value = DBFReadStringAttribute(dbfFile, j, i);
                point->setStringAttribute(key, value);
            }
        }

        // Store the point in the array.
        _points[j] = point;

        // Dispose of the shape and the vertex array.
        SHPDestroyObject(shape);
    }

    // Close the shapefile and the DBF.
    SHPClose(shapefile);
    DBFClose(dbfFile);

    // Free the arrays for the field names and the field types.
    delete[] fieldNames;
    delete[] fieldTypes;

    qDebug() << "done initializing from the shapefile";
}
void ShapePoint2DSet::assignGridCells(Polygon2DSet *polygons) {
    // Iterate over all points in the set.
    int numPolygons = polygons->numPolygons();
    for (int j = 0; j < _numPoints; j++)
    {
        // Get the point from this set.
        ShapePoint2D *point = this->point(j);

        // Check for containment in each polygon.
        for (int i = 0; i < numPolygons; i++) {
            // Get the polygon from the set.
            Polygon2D *polygon = polygons->polygon(i);
            Range2D range = polygon->range();

            // If the point is contained in the polygon, assign the column and row keys and exit the polygon loop.
            if (point->X() >= range.xRange().min() && point->X() <= range.xRange().max() &&
                point->Y() >= range.yRange().min() && point->Y() <= range.yRange().max()) {
               point->setIntegerAttribute(COL_KEY, polygon->getIntegerAttribute(COL_KEY));
               point->setIntegerAttribute(ROW_KEY, polygon->getIntegerAttribute(ROW_KEY));
               i = numPolygons;  // Exit the loop.
            }
        }
    }
}

ShapePoint2DSet *ShapePoint2DSet::createClippedSetWithPolygons(Polygon2DSet *polygons)
{
    /*
    static int count = 0;
    qDebug() << "going to clip the set by the polygons " << count++;

    vector<ShapePoint2D *> clippedPoints;

    // j < 5
    for (int j = 0; j < _numPoints; j++)
    {
        qDebug() << "going to process point " << j;

        // Get the point from this set.
        ShapePoint2D *point = this->point(j);
        Range2D pointRange = point->range();

        // Try to segment the point against every polygon in the set.
        for (int i = 0; i < polygons->numPolygons(); i++)
        {
            if (polygons->polygon(i)->range().overlaps(pointRange))
            {
                //qDebug() << "potential intersection between polygon " << i << " and point " << j;

                // Get the segments.
                vector<ShapePoint2D *> *segments = point->createPointsWithinPolygon(polygons->polygon(i));
                //qDebug() << "got " << segments->size() << " points from point " << j;

                // Add the segments to the list of clipped points.
                if (segments != NULL) {
                    int numSegments = segments->size();
                    for (int k = 0; k < numSegments; k++)
                    {
                        clippedPoints.push_back(segments->at(k));
                    }

                    // Delete the point segment vector.
                    delete segments;
                }
            }
        }
    }

    // Return a point set of the clipped points.
    int numPoints = clippedPoints.size();
    ShapePoint2D **pointArray = new ShapePoint2D*[numPoints];
    for (int i = 0; i < numpoints; i++)
    {
        pointArray[i] = clippedpoints.at(i);
    }
    return new ShapePoint2DSet(pointArray, numpoints);
    */
    return NULL;
}

void ShapePoint2DSet::writeToShapefile(const QString filename)
{
    int shapeType = SHPT_POINT;

    // Open the shapefile and get the shapefile info.
    SHPHandle shapefile = SHPCreate(filename.toAscii(), shapeType);

    // Write the shapes to the shapefile.
    for (int j = 0; j < _numPoints; j++)
    {
        qDebug() << "going to write point " << j;

        // Get the shape and make the vertex arrays.
        ShapePoint2D *point = this->point(j);
        double *xVertices = new double[1];
        double *yVertices = new double[1];
        xVertices[0] = point->X();
        yVertices[0] = point->Y();

        // Create the shape and write it to the file.
        SHPObject *shape = SHPCreateSimpleObject(shapeType, 1, xVertices, yVertices, NULL);
        SHPWriteObject(shapefile, -1, shape);

        // Clean up memory.
        SHPDestroyObject(shape);
        delete[] xVertices;
        delete[] yVertices;
    }

    qDebug() << "done writing points to shapefile: " << filename;

    // Close the shapefile.
    SHPClose(shapefile);

    // Determine the DBF filename.
    QString dbfFilename = filename;

    qDebug() << "determined the DBF name";

    // Determine the attributes.
    QList<QString> stringAttributeKeys = getStringAttributeKeys();
    QList<QString> integerAttributeKeys = getIntegerAttributeKeys();
    QList<QString> doubleAttributeKeys = getDoubleAttributeKeys();

    // Create the DBF file.
    DBFHandle dbfFile = DBFCreate(dbfFilename.toAscii());

    qDebug() << "created DBF";

    // Create the attributes in the DBF file.
    QMap<QString, int> attributeFieldNumbers;
    for (int i = 0; i < doubleAttributeKeys.length(); i++) {
        QString key = doubleAttributeKeys[i];
        int fieldNumber = DBFAddField(dbfFile, key.toAscii(), FTDouble, 10, 5);
        attributeFieldNumbers.insert(key, fieldNumber);
        qDebug() << "the field number for key " << key << " is " << fieldNumber;
    }
    for (int i = 0; i < integerAttributeKeys.length(); i++) {
        QString key = integerAttributeKeys[i];
        int fieldNumber = DBFAddField(dbfFile, key.toAscii(), FTInteger, 10, 0);
        attributeFieldNumbers.insert(key, fieldNumber);
        qDebug() << "the field number for key " << key << " is " << fieldNumber;
    }
    for (int i = 0; i < stringAttributeKeys.length(); i++) {
        QString key = stringAttributeKeys[i];
        int fieldNumber = DBFAddField(dbfFile, key.toAscii(), FTString, 10, 0);
        attributeFieldNumbers.insert(key, fieldNumber);
        qDebug() << "the field number for key " << key << " is " << fieldNumber;
    }

    for (int j = 0; j < _numPoints; j++) {
        ShapePoint2D *point = _points[j];

        // Write the double attribute values for this point.
        for (int i = 0; i < doubleAttributeKeys.length(); i++) {
            QString key = doubleAttributeKeys[i];
            double value = point->getDoubleAttribute(key);
            DBFWriteDoubleAttribute(dbfFile, j, attributeFieldNumbers[key], value);
        }

        // Write the integer attribute values for this point.
        for (int i = 0; i < integerAttributeKeys.length(); i++) {
            QString key = integerAttributeKeys[i];
            int value = point->getIntegerAttribute(key);
            DBFWriteIntegerAttribute(dbfFile, j, attributeFieldNumbers[key], value);
        }

        // Write the string attribute values for this point.
        for (int i = 0; i < stringAttributeKeys.length(); i++) {
            QString key = stringAttributeKeys[i];
            QString value = point->getStringAttribute(key);
            DBFWriteStringAttribute(dbfFile, j, attributeFieldNumbers[key], value.toAscii());
        }

        //DBFWriteIntegerAttribute(dbfFile, i, fieldNumber, i);
    }

    qDebug() << "wrote DBF";

    DBFClose(dbfFile);

    qDebug() << "wrote " << _numPoints << " shapes to shapefile " << filename;
}

QList<QString> ShapePoint2DSet::getStringAttributeKeys() {
    // Create the list.
    QList<QString> stringAttributeKeys;

    // Add all unique keys from the points.
    for (int i = 0; i < _numPoints; i++) {
        QList<QString> pointKeys = _points[i]->getStringAttributeKeys();

        for (int j = 0; j < pointKeys.length(); j++) {
            QString key = pointKeys[j];
            if (!stringAttributeKeys.contains(key)) {
                stringAttributeKeys.append(key);
            }
        }
    }

    // Return the result.
    qDebug() << "returning " << stringAttributeKeys.length() << " keys";
    return stringAttributeKeys;
}
QList<QString> ShapePoint2DSet::getIntegerAttributeKeys() {
    // Create the list.
    QList<QString> integerAttributeKeys;

    // Add all unique keys from the points.
    for (int i = 0; i < _numPoints; i++) {
        QList<QString> pointKeys = _points[i]->getIntegerAttributeKeys();

        for (int j = 0; j < pointKeys.length(); j++) {
            QString key = pointKeys[j];
            if (!integerAttributeKeys.contains(key)) {
                integerAttributeKeys.append(key);
            }
        }
    }

    // Return the result.
    qDebug() << "returning " << integerAttributeKeys.length() << " keys";
    return integerAttributeKeys;
}
QList<QString> ShapePoint2DSet::getDoubleAttributeKeys() {
    // Create the list.
    QList<QString> doubleAttributeKeys;

    // Add all unique keys from the points.
    for (int i = 0; i < _numPoints; i++) {
        QList<QString> pointKeys = _points[i]->getDoubleAttributeKeys();

        for (int j = 0; j < pointKeys.length(); j++) {
            QString key = pointKeys[j];
            if (!doubleAttributeKeys.contains(key)) {
                doubleAttributeKeys.append(key);
            }
        }
    }

    // Return the result.
    qDebug() << "returning " << doubleAttributeKeys.length() << " keys";
    return doubleAttributeKeys;
}

ShapePoint2D *ShapePoint2DSet::point(int index)
{
    return _points[index];
}

int ShapePoint2DSet::numPoints()
{
    return _numPoints;
}

int ShapePoint2DSet::shapeType()
{
    return _shapeType;
}

double ShapePoint2DSet::minBounds(int dimension)
{
    return _minBounds[dimension];
}

double ShapePoint2DSet::maxBounds(int dimension)
{
    return _maxBounds[dimension];
}
