#include "polyline2d.h"
#include "polygon2d.h"
#include "polyline2dset.h"
#include "polygon2dset.h"
#include "shapelib/shapefil.h"
#include "point2d.h"
#include "linesegment2d.h"

#include <stdlib.h>
#include <vector>

#include <QDebug>

using namespace std;

Polyline2DSet::Polyline2DSet()
{
    _polylines = NULL;
    _numPolylines = 0;

    for (int i = 0; i < 4; i++)
    {
        _minBounds[i] = _maxBounds[i] = 0.0;
    }
}

Polyline2DSet::Polyline2DSet(Polyline2D **polylines, int numPolylines) {
    _polylines = polylines;
    _numPolylines = numPolylines;
}

Polyline2DSet::~Polyline2DSet()
{
    if (_polylines != NULL)
    {
        delete[] _polylines;
    }
}

void Polyline2DSet::initFromShapefile(const QString filename)
{
    // Open the shapefile and get the shapefile info.
    SHPHandle shapefile = SHPOpen(filename.toAscii(), "rb");
    SHPGetInfo(shapefile, &_numPolylines, &_shapeType, _minBounds, _maxBounds);

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
    _polylines = new Polyline2D*[_numPolylines];
    for (int j = 0; j < _numPolylines; j++) {
        // Read the shape.
        SHPObject *shape = SHPReadObject(shapefile, j);

        // Make the vertex array.
        int numVertices = shape->nVertices;
        Point2D *vertices = new Point2D[numVertices];

        // Read the vertices into the polyline.
        for (int i = 0; i < numVertices; i++) {
            vertices[i] = Point2D(shape->padfX[i], shape->padfY[i]);
        }

        // Make the polyline.
        Polyline2D *polyline = new Polyline2D(vertices, numVertices);

        // Attach the attributes to the polyline.
        for (int i = 0; i < numFields; i++) {
            int fieldType = fieldTypes[i];
            QString key = fieldNames[i];
            if (fieldType == FTDouble) {
                double value = DBFReadDoubleAttribute(dbfFile, j, i);
                polyline->setDoubleAttribute(key, value);
            }
            else if (fieldType == FTInteger) {
                int value = DBFReadIntegerAttribute(dbfFile, j, i);
                polyline->setIntegerAttribute(key, value);
            }
            else if (fieldType == FTString) {
                QString value = DBFReadStringAttribute(dbfFile, j, i);
                polyline->setStringAttribute(key, value);
            }
        }

        // Store the polyline in the array.
        _polylines[j] = polyline;

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

Polyline2DSet *Polyline2DSet::createClippedSetWithPolygons(Polygon2DSet *polygons)
{
    static int count = 0;
    qDebug() << "going to clip the set by the polygons " << count++;

    vector<Polyline2D *> clippedPolylines;

    // j < 5
    for (int j = 0; j < _numPolylines; j++)
    {
        qDebug() << "going to process polyline " << j;

        // Get the polyline from this set.
        Polyline2D *polyline = this->polyline(j);
        Range2D polylineRange = polyline->range();

        // Try to segment the polyline against every polygon in the set.
        for (int i = 0; i < polygons->numPolygons(); i++)
        {
            if (polygons->polygon(i)->range().overlaps(polylineRange))
            {
                //qDebug() << "potential intersection between polygon " << i << " and polyline " << j;

                // Get the segments.
                vector<Polyline2D *> *segments = polyline->createPolylinesWithinPolygon(polygons->polygon(i));
                //qDebug() << "got " << segments->size() << " polylines from polyline " << j;

                // Add the segments to the list of clipped polylines.
                if (segments != NULL) {
                    int numSegments = segments->size();
                    for (int k = 0; k < numSegments; k++)
                    {
                        clippedPolylines.push_back(segments->at(k));
                    }

                    // Delete the polyline segment vector.
                    delete segments;
                }
            }
        }
    }

    // Return a polyline set of the clipped polylines.
    int numPolylines = clippedPolylines.size();
    Polyline2D **polylineArray = new Polyline2D*[numPolylines];
    for (int i = 0; i < numPolylines; i++)
    {
        polylineArray[i] = clippedPolylines.at(i);
    }
    return new Polyline2DSet(polylineArray, numPolylines);
}

void Polyline2DSet::writeToShapefile(const QString filename)
{
    int shapeType = SHPT_ARC;

    // Open the shapefile and get the shapefile info.
    SHPHandle shapefile = SHPCreate(filename.toAscii(), shapeType);

    // Write the shapes to the shapefile.
    for (int j = 0; j < _numPolylines; j++)
    {
        qDebug() << "going to write polyline " << j;

        // Get the shape and make the vertex arrays.
        Polyline2D *polyline = this->polyline(j);
        int numVertices = polyline->numVertices();
        double *xVertices = new double[numVertices];
        double *yVertices = new double[numVertices];
        for (int i = 0; i < numVertices; i++)
        {
            Point2D point = polyline->vertex(i);
            xVertices[i] = point.X;
            yVertices[i] = point.Y;
        }

        // Create the shape and write it to the file.
        SHPObject *shape = SHPCreateSimpleObject(shapeType, numVertices, xVertices, yVertices, NULL);
        SHPWriteObject(shapefile, -1, shape);

        // Clean up memory.
        SHPDestroyObject(shape);
        delete[] xVertices;
        delete[] yVertices;
    }

    qDebug() << "done writing polylines to shapefile: " << filename;

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

    for (int j = 0; j < _numPolylines; j++) {
        Polyline2D *polyline = _polylines[j];

        // Write the double attribute values for this polyline.
        for (int i = 0; i < doubleAttributeKeys.length(); i++) {
            QString key = doubleAttributeKeys[i];
            double value = polyline->getDoubleAttribute(key);
            DBFWriteDoubleAttribute(dbfFile, j, attributeFieldNumbers[key], value);
        }

        // Write the integer attribute values for this polyline.
        for (int i = 0; i < integerAttributeKeys.length(); i++) {
            QString key = integerAttributeKeys[i];
            int value = polyline->getIntegerAttribute(key);
            DBFWriteIntegerAttribute(dbfFile, j, attributeFieldNumbers[key], value);
        }

        // Write the string attribute values for this polyline.
        for (int i = 0; i < stringAttributeKeys.length(); i++) {
            QString key = stringAttributeKeys[i];
            QString value = polyline->getStringAttribute(key);
            DBFWriteStringAttribute(dbfFile, j, attributeFieldNumbers[key], value.toAscii());
        }

        //DBFWriteIntegerAttribute(dbfFile, i, fieldNumber, i);
    }

    qDebug() << "wrote DBF";

    DBFClose(dbfFile);

    qDebug() << "wrote " << _numPolylines << " shapes to shapefile " << filename;
}

QList<QString> Polyline2DSet::getStringAttributeKeys() {
    // Create the list.
    QList<QString> stringAttributeKeys;

    // Add all unique keys from the polylines.
    for (int i = 0; i < _numPolylines; i++) {
        QList<QString> polylineKeys = _polylines[i]->getStringAttributeKeys();

        for (int j = 0; j < polylineKeys.length(); j++) {
            QString key = polylineKeys[j];
            if (!stringAttributeKeys.contains(key)) {
                stringAttributeKeys.append(key);
            }
        }
    }

    // Return the result.
    qDebug() << "returning " << stringAttributeKeys.length() << " keys";
    return stringAttributeKeys;
}
QList<QString> Polyline2DSet::getIntegerAttributeKeys() {
    // Create the list.
    QList<QString> integerAttributeKeys;

    // Add all unique keys from the polylines.
    for (int i = 0; i < _numPolylines; i++) {
        QList<QString> polylineKeys = _polylines[i]->getIntegerAttributeKeys();

        for (int j = 0; j < polylineKeys.length(); j++) {
            QString key = polylineKeys[j];
            if (!integerAttributeKeys.contains(key)) {
                integerAttributeKeys.append(key);
            }
        }
    }

    // Return the result.
    qDebug() << "returning " << integerAttributeKeys.length() << " keys";
    return integerAttributeKeys;
}
QList<QString> Polyline2DSet::getDoubleAttributeKeys() {
    // Create the list.
    QList<QString> doubleAttributeKeys;

    // Add all unique keys from the polylines.
    for (int i = 0; i < _numPolylines; i++) {
        QList<QString> polylineKeys = _polylines[i]->getDoubleAttributeKeys();

        for (int j = 0; j < polylineKeys.length(); j++) {
            QString key = polylineKeys[j];
            if (!doubleAttributeKeys.contains(key)) {
                doubleAttributeKeys.append(key);
            }
        }
    }

    // Return the result.
    qDebug() << "returning " << doubleAttributeKeys.length() << " keys";
    return doubleAttributeKeys;
}

Polyline2D *Polyline2DSet::polyline(int index)
{
    return _polylines[index];
}

int Polyline2DSet::numPolylines()
{
    return _numPolylines;
}

int Polyline2DSet::shapeType()
{
    return _shapeType;
}

double Polyline2DSet::minBounds(int dimension)
{
    return _minBounds[dimension];
}

double Polyline2DSet::maxBounds(int dimension)
{
    return _maxBounds[dimension];
}
