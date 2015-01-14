#ifndef TIMESERIES_H
#define TIMESERIES_H

#include <QString>

#include <vector>
#include "timeseriessample.h"
#include "timespan.h"

class TimeSeries
{
public:
    TimeSeries(const char *name, std::vector<TimeSeriesSample> *samples);

    static std::vector<TimeSeries *> *fromSMPFile(const QString filename);

    QString name();
    int numSamples();

    double getLinearInterpolationValue(QDateTime date);

    double valueInTimeSpan(TimeSpan timeSpan);

private:
    QString _name;
    std::vector<TimeSeriesSample> *_samples;
    int _numSamples;
    long *_dateLongs;

    TimeSpan _lastQueriedTimeSpan;
    double _lastReturnedValue;

    int indexOfDate(QDateTime date);
};

#endif // TIMESERIES_H
