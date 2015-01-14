#ifndef MODELELEMENTWEL_H
#define MODELELEMENTWEL_H

#include "modelelement.h"

class ModelElementWEL : public ModelElement
{
public:
    explicit ModelElementWEL(QObject *parent = 0);
    int modelElementType();

    void setName(QString name);
    void setShapefilePath(QString shapefilePath);
    void setSampleFilePath(QString sampleFilePath);
    void setKeyField(QString keyField);

    QString shapefilePath();
    QString sampleFilePath();
    QString keyField();

    QList<QString> information();

private:
    QString _shapefilePath;
    QString _sampleFilePath;
    QString _keyField;
};

#endif // MODELELEMENTWEL_H
