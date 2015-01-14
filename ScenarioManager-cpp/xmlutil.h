#ifndef XMLUTIL_H
#define XMLUTIL_H

#include "timescale.h"

#include <QDateTime>
#include <QString>
#include <qdom.h>

class XMLUtil
{
public:
    XMLUtil();

    static QDateTime safeGetDateAttributeByName(QDomNode node, const QString attributeName, const QDateTime defaultValue = QDateTime());
    static double safeGetDoubleAttributeByName(QDomNode node, const QString attributeName, const double defaultValue = 0.0);
    static int safeGetIntegerAttributeByName(QDomNode node, const QString attributeName, const int defaultValue = 0);
    static QString safeGetStringAttributeByName(QDomNode node, const QString attributeName, const QString defaultValue = "");
    static Timescale safeGetTimescaleAttributeByName(QDomNode node, const QString attributeName, const Timescale timescale = TIMESCALE_DAILY);
};

#endif // XMLUTIL_H
