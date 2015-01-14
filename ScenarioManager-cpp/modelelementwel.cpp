#include <QDebug>

#include "modelelementwel.h"

#define DEFAULT_ELEMENT_NAME "Unnamed Well Package"

ModelElementWEL::ModelElementWEL(QObject *parent) : ModelElement(parent)
{
}

int ModelElementWEL::modelElementType() {
    return MODEL_ELEMENT_TYPE_WEL;
}

void ModelElementWEL::setName(QString name) {
    static int defaultIndex = 1;

    _name = name.trimmed();
    if (_name.isEmpty()) {
        _name = QString(DEFAULT_ELEMENT_NAME).append(" %1").arg(defaultIndex++);
    }
}

QList<QString> ModelElementWEL::information() {
    qDebug() << "building information";

    // Build the information list.
    QList<QString> information;
    information.append(QString("Well Package: %1").arg(_name));
    information.append(QString("shapefile path: %1").arg(_shapefilePath));
    information.append(QString("sample file path: %1").arg(_sampleFilePath));
    information.append(QString("key field: %1").arg(_keyField));

    // Return the list.
    return information;
}

void ModelElementWEL::setShapefilePath(QString shapefilePath) {
    _shapefilePath = shapefilePath;
}
void ModelElementWEL::setSampleFilePath(QString sampleFilePath) {
    _sampleFilePath = sampleFilePath;
}
void ModelElementWEL::setKeyField(QString keyField) {
    _keyField = keyField;
}

QString ModelElementWEL::shapefilePath() {
    return _shapefilePath;
}
QString ModelElementWEL::sampleFilePath() {
    return _sampleFilePath;
}
QString ModelElementWEL::keyField() {
    return _keyField;
}
