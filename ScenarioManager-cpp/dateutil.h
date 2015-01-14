#ifndef DATEUTIL_H
#define DATEUTIL_H

#include <QDateTime>

class DateUtil
{
public:
    static QDateTime CreateDateYMD(int year, int month, int day);
};

#endif // DATEUTIL_H
