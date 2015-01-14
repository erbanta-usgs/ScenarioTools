#include "stdlib.h"
#include "xmlutil.h"

XMLUtil::XMLUtil()
{
}

QDateTime XMLUtil::safeGetDateAttributeByName(QDomNode node, const QString attributeName, const QDateTime defaultValue) {
    QString dateString = node.attributes().namedItem(attributeName).nodeValue();
    //return QDateTime::fromString(dateString, "YYYY-mm-dd");
    return QDateTime::currentDateTime();
}
double XMLUtil::safeGetDoubleAttributeByName(QDomNode node, const QString attributeName, const double defaultValue) {
    return atof(node.attributes().namedItem(attributeName).nodeValue().toAscii());
}
int XMLUtil::safeGetIntegerAttributeByName(QDomNode node, const QString attributeName, const int defaultValue) {
    return atof(node.attributes().namedItem(attributeName).nodeValue().toAscii());
}
QString XMLUtil::safeGetStringAttributeByName(QDomNode node, const QString attributeName, const QString defaultValue) {
    return node.attributes().namedItem(attributeName).nodeValue();
}
Timescale XMLUtil::safeGetTimescaleAttributeByName(QDomNode node, const QString attributeName, const Timescale timescale) {
    return timescale;
}
