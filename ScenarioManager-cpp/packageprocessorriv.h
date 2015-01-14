#ifndef PACKAGEPROCESSORRIV_H
#define PACKAGEPROCESSORRIV_H

#include <QString>
#include "timespan.h"

class Polyline2DSet;
class ModelElementDIS;
class QTextStream;

class PackageProcessorRIV
{
public:
    PackageProcessorRIV(QString shapefilePath, QString sampleFilePath, ModelElementDIS *discretization, QString idKey, QString widthKey,
                        QString conductivityKey, QString flowLengthKey, QString riverBottomKey, QString clippedShapefilePath, QString riverPackagePath);
    void process();

private:
    void writeRiverPackage(Polyline2DSet *clippedPolylines, ModelElementDIS *discretization, QString identifierKey);
    void writeStressPeriod(QTextStream *ts, int stressPeriodIndex, Polyline2DSet *polylines, QString identifierKey, TimeSpan timeSpan);

    QString _shapefilePath;
    QString _sampleFilePath;
    ModelElementDIS *_discretization;
    QString _idKey;
    QString _widthKey;
    QString _conductivityKey;
    QString _flowLengthKey;
    QString _riverBottomKey;
    QString _clippedShapefilePath;
    QString _riverPackagePath;
};

#endif // PACKAGEPROCESSORRIV_H
