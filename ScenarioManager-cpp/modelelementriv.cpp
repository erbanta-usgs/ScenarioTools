#include "modelelementriv.h"
#include <QDebug>
#include <qdom.h>
#include <QFile>
#include "xmlutil.h"

void ModelElementRIV::setAllNull()
{
    // Set all private variables to null.
    _shapefilePath = QString::null;
    _timeSeriesPath = QString::null;
    _discretizationName = QString::null;
    _timeSeriesKey = QString::null;
    _widthKey = QString::null;
    _conductivityKey = QString::null;
    _flowLengthKey = QString::null;
    _riverBottomKey = QString::null;
    _clippedShapefilePath = QString::null;
    _riverPackagePath = QString::null;
}

ModelElementRIV::ModelElementRIV()
{
    setAllNull();
}

int ModelElementRIV::modelElementType() {
    return MODEL_ELEMENT_TYPE_RIV;
}


ModelElementRIV::ModelElementRIV(QString shapefilePath, QString timeSeriesPath, QString discretizationName, QString timeSeriesKey, QString widthKey,
                                 QString conductivityKey, QString flowLengthKey, QString riverBottomKey, QString clippedShapefilePath, QString riverPackagePath)
{
    // Clear the state.
    setAllNull();

    // Store the configuration.
    _shapefilePath = shapefilePath;
    _timeSeriesPath = timeSeriesPath;
    _discretizationName = discretizationName;
    _timeSeriesKey = timeSeriesKey;
    _widthKey = widthKey;
    _conductivityKey = conductivityKey;
    _flowLengthKey = flowLengthKey;
    _riverBottomKey = riverBottomKey;
    _clippedShapefilePath = clippedShapefilePath;
    _riverPackagePath = riverPackagePath;
}


QDomNode ModelElementRIV::toXMLNode()
{
    return QDomNode();
}

ModelElementRIV *ModelElementRIV::createFromXMLNode(QDomNode node)
{
    // Get the element name from the node.
    QString elementName = node.nodeName();
    qDebug() << node.nodeName();

    // Get the other data from the node attributes.
    QString shapefilePath = XMLUtil::safeGetStringAttributeByName(node, "shapefilePath");
    QString timeSeriesPath = XMLUtil::safeGetStringAttributeByName(node, "timeSeriesPath");
    QString discretizationName = XMLUtil::safeGetStringAttributeByName(node, "discretization");
    QString timeSeriesKey = XMLUtil::safeGetStringAttributeByName(node, "timeSeriesKey");
    QString widthKey = XMLUtil::safeGetStringAttributeByName(node, "widthKey");
    QString conductivityKey = XMLUtil::safeGetStringAttributeByName(node, "conductivityKey");
    QString flowLengthKey = XMLUtil::safeGetStringAttributeByName(node, "flowLengthKey");
    QString riverBottomKey = XMLUtil::safeGetStringAttributeByName(node, "riverBottomKey");
    QString clippedShapefilePath = XMLUtil::safeGetStringAttributeByName(node, "clippedShapefilePath");
    QString riverPackagePath = XMLUtil::safeGetStringAttributeByName(node, "riverPackagePath");

    qDebug() << "The shapefile path is " << shapefilePath;
    qDebug() << "The time series path is " << timeSeriesPath;
    qDebug() << "The discretization is " << discretizationName;
    qDebug() << "The time series key is " << timeSeriesKey;
    qDebug() << "The width key is " << widthKey;
    qDebug() << "The conductivity key is " << conductivityKey;
    qDebug() << "The flow length key is " << flowLengthKey;
    qDebug() << "The river bottom key is " << riverBottomKey;
    qDebug() << "The clipped shapefile path is " << clippedShapefilePath;
    qDebug() << "The river package path is " << riverPackagePath;

    return new ModelElementRIV(shapefilePath, timeSeriesPath, discretizationName, timeSeriesKey, widthKey, conductivityKey, flowLengthKey, riverBottomKey,
                               clippedShapefilePath, riverPackagePath);
}
