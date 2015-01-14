#ifndef TIMESPAN_H
#define TIMESPAN_H

#include <QDateTime>

class TimeSpan
{
public:
    TimeSpan(QDateTime startDate = QDateTime(), QDateTime endDate = QDateTime());

    bool contains(QDateTime date);
    long size();

    QDateTime StartDate;
    QDateTime EndDate;
};

#endif // TIMESPAN_H
