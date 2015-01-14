#include <QDebug>

#include "timeseriessample.h"

TimeSeriesSample::TimeSeriesSample(int year, int month, int day, int hour, int minute, int second, double value)
{
    // Store the time.
    QString dateString = QString("%1-%2-%3T%4:%5:%6").arg(year, 4, 10, QLatin1Char(' ')).arg(month, 2, 10, QLatin1Char('0')).arg(day, 2, 10, QLatin1Char('0')).arg(
            hour, 2, 10, QLatin1Char('0')).arg(minute, 2, 10, QLatin1Char('0')).arg(second, 2, 10, QLatin1Char('0'));
    Time = QDateTime::fromString(dateString, "yyyy-MM-ddThh:mm:ss");

    // Store the value.
    Value = value;
}

TimeSeriesSample::TimeSeriesSample(QDateTime time, double value) {
    Time = time;
    Value = value;
}
