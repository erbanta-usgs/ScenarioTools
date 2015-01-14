#ifndef SHAPEFILEUTIL_H
#define SHAPEFILEUTIL_H

#include <QList>
#include <QString>

class ShapefileUtil
{
public:
    static QList<QString> getAttributeNames(QString shapefilePath);
};

#endif // SHAPEFILEUTIL_H
