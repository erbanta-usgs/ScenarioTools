#ifndef MODELELEMENTRIV_H
#define MODELELEMENTRIV_H

#include "modelelement.h"
#include <qdom.h>

class ModelElementRIV : public ModelElement
{
public:
    explicit ModelElementRIV();
    explicit ModelElementRIV(QString shapefilePath, QString timeSeriesPath, QString discretizationName, QString timeSeriesKey, QString widthKey, QString conductivityKey,
                    QString flowLengthKey, QString riverBottomKey, QString clippedShapefilePath, QString riverPackagePath);
    int modelElementType();

    QDomNode toXMLNode();

    static ModelElementRIV *createFromXMLNode(QDomNode node);

private:
    void setAllNull();

    QString _shapefilePath;
    QString _timeSeriesPath;
    QString _discretizationName;
    QString _timeSeriesKey;
    QString _widthKey;
    QString _conductivityKey;
    QString _flowLengthKey;
    QString _riverBottomKey;
    QString _clippedShapefilePath;
    QString _riverPackagePath;
};

#endif // MODELELEMENTRIV_H
