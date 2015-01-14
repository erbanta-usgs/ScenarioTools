#include "dateutil.h"

QDateTime DateUtil::CreateDateYMD(int year, int month, int day) {
    QString dateString = QString("%1-%2-%3T00:00:00").arg(year, 4, 10, QLatin1Char(' ')).arg(month, 2, 10, QLatin1Char('0')).arg(day, 2, 10, QLatin1Char('0'));
    return QDateTime::fromString(dateString, "yyyy-MM-ddThh:mm:ss");
}
