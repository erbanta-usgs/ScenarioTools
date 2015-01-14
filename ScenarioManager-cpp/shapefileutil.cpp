#include "shapefileutil.h"
#include "shapelib/shapefil.h"

QList<QString> ShapefileUtil::getAttributeNames(QString shapefilePath) {
    /*
    //SHPGetInfo(SHPHandle psSHP, int * pnEntities, int * pnShapeType,
          //     double * padfMinBound, double * padfMaxBound )

    // These are the variables for getting the shapefile information.
    int numPolygons;
    int shapeType;
    double minBounds[4];
    double maxBounds[4];

    // Open the shapefile and get the shapefile info.
    SHPHandle shapefile = SHPOpen(shapefilePath.toAscii(), "rb");
    SHPGetInfo(shapefile, &numPolygons, &shapeType, minBounds, maxBounds);

    // Open the DBF.
    QString dbfName = QString(filename.toAscii());
    int length = dbfName.length();
    dbfName[length - 3] = 'd';
    dbfName[length - 2] = 'b';
    dbfName[length - 1] = 'f';
    DBFHandle dbfFile = DBFOpen(dbfName.c_str(), "rb");

    // Make a list for the attribute names.
    QList<QString> attributeNames;

    // Get the field names.
    int numFields = DBFGetFieldCount(dbfFile);
    char fieldName[12];
    for (int i = 0; i < numFields; i++) {
        fieldTypes[i] = DBFGetFieldInfo(dbfFile, i, fieldName, NULL, NULL);
        fieldNames[i] = fieldName;

        qDebug() << fieldNames[i];
    }
    qDebug() << "the DBF has " << numFields << " fields";

    // Close the shapefile.
    SHPClose(shapefile);

    return attributeNames;
    */
    return QList<QString>();
}
