#include "modelelement.h"
#include <QDebug>

#define DEFAULT_ELEMENT_NAME "Unnamed Element"

ModelElement::ModelElement(QObject *parent) :
    QObject(parent)
{
    _name = "";
}

QString ModelElement::name() {
    return _name;
}

void ModelElement::setName(QString name) {
    static int defaultIndex = 1;

    _name = name.trimmed();
    if (_name.isEmpty()) {
        _name = QString(DEFAULT_ELEMENT_NAME).append(" %1").arg(defaultIndex++);
    }
}

int ModelElement::modelElementType() {
    return MODEL_ELEMENT_TYPE_PLAIN;
}

QList<QString> ModelElement::information() {
    QList<QString> information;
    return information;
}
