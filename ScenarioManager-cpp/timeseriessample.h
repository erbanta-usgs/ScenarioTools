#ifndef TIMESERIESSAMPLE_H
#define TIMESERIESSAMPLE_H

#include <QDateTime>

class TimeSeriesSample
{
public:
    TimeSeriesSample(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, double value = 0.0);
    TimeSeriesSample(QDateTime time, double value);
    QDateTime Time;
    double Value;
};

#endif // TIMESERIESSAMPLE_H
