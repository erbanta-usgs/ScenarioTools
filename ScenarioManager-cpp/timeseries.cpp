#include "timeseries.h"
#include "stringutil.h"

#include <iostream>
#include <fstream>
#include <sstream>
#include <QDebug>

using namespace std;

TimeSeries::TimeSeries(const char *name, vector<TimeSeriesSample> *samples) {
    _name = name;
    _samples = samples;
    _numSamples = _samples->size();

    _dateLongs = new long[_numSamples];
    for (int i = 0; i < _numSamples; i++) {
        _dateLongs[i] = _samples->at(i).Time.toTime_t();
    }
}

QString TimeSeries::name() {
    return _name;
}

int TimeSeries::indexOfDate(QDateTime date) {
    // Get long integer respresentaiton of the date.
    long dateL = date.toTime_t();

    // This is a modified binary search that will find the largest index for which the value at that index is less than the search value.
    int low = 0;
    int high = _numSamples - 1;
    int mid;
    while (low <= high) {
        mid = (low + high) / 2;

        // if midpoint is too low
        if (_dateLongs[mid] < dateL) {
            // If the point is contained between mid index and next index, return mid.
            if (_dateLongs[mid + 1] >= dateL) {
                return mid;
            }

            // Otherwise, move low to mid + 1
            else {
                low = mid + 1;
            }
        }
        // if midpoint is too high
        else if (_dateLongs[mid] > dateL) {
            // if point is contained between mid index and previous index, return mid - 1
            if (_dateLongs[mid - 1] <= dateL) {
                return mid - 1;
            }
            // otherwise, move high to mid - 1
            else {
                high = mid - 1;
            }
        }
        else {
            return mid;
        }
    }

    // If value is outside the array, return -1.
    return -1;
}

double TimeSeries::getLinearInterpolationValue(QDateTime date) {
    // This is the case if date is on or before the beginning of the time series.
    // Return the value of the first sample in the series.
    int numSamples = _samples->size();
    if (date <= _samples->at(0).Time) {
        return _samples->at(0).Value;
    }

    // This is the case if date is on or after the end of the time series.
    // Return the value of the last sample in the series.
    else if (date >= _samples->at(numSamples - 1).Time) {
        return _samples->at(numSamples - 1).Value;
    }

    // This is the case if date is within the time series.
    else {
        // Find the index of sample that is most recently before this date.
        int i0 = indexOfDate(date);
        int i1 = i0 + 1;

        // If the interpolation type is step, return the value of the most recently
        // previous sample.
        //if (interpolationType == InterpolationType.STEP) {
        //return samples[i0].getValue();
        //}

        // For linear interpolation, return the linear average.
        //else {

        // Get the times at the upper, lower, and to-find dates.
        long t0 = _samples->at(i0).Time.toTime_t();
        long t1 = _samples->at(i1).Time.toTime_t();
        long t = date.toTime_t();

        // Determine the time span; if zero, return the unweighted average of both values.
        double span = t1 - t0;
        if (span == 0.0) {
            return (_samples->at(i0).Value + _samples->at(i1).Value) * 0.5;
        }

        // Find the weighted average according to the distance to neighbors.
        double proportionLower = (t1 - t) / span;
        double proportionUpper = (t - t0) / span;
        return _samples->at(i0).Value * proportionLower +
                _samples->at(i1).Value * proportionUpper;
    }
}
double timeWeightedSumOfSamples(TimeSeriesSample sample1, TimeSeriesSample sample2) {
    TimeSpan timeSpan(sample1.Time, sample2.Time);
    return (sample1.Value + sample2.Value) * timeSpan.size() * 0.5;
}
double averageTimeWeightedValue(vector<TimeSeriesSample> samples) {
    // Make an accumulator for the sum.
    double sumTimeWeightedValue = 0.0;

    // Add the area under each segment to the accumulator.
    for (uint i = 0; i < samples.size() - 1; i++) {
        sumTimeWeightedValue += timeWeightedSumOfSamples(samples.at(i), samples.at(i + 1));
    }

    // Return the accumulated value divided by the total size of the span.
    TimeSpan timeSpan(samples.at(0).Time, samples.at(samples.size() - 1).Time);
    return sumTimeWeightedValue / timeSpan.size();
}

double TimeSeries::valueInTimeSpan(TimeSpan timeSpan) {
    // If we're querying the same time span as was last queried, return the last computed value.
    if (timeSpan.StartDate == _lastQueriedTimeSpan.StartDate && timeSpan.EndDate == _lastQueriedTimeSpan.EndDate) {
        return _lastReturnedValue;
    }

    // Make a list for the interpolation values.
    vector<TimeSeriesSample> samplesInRange;

    // Add the value at the lower end of the range to the list.
    samplesInRange.push_back(TimeSeriesSample(timeSpan.StartDate, getLinearInterpolationValue(timeSpan.StartDate)));

    // Add all values contained in the range to the list.
    int numSamples = _samples->size();
    for (int i = 0; i < numSamples; i++) {
        if (timeSpan.contains(_samples->at(i).Time)) {
            samplesInRange.push_back(_samples->at(i));
        }
    }

    // add value at the upper end of the range to the list
    samplesInRange.push_back(TimeSeriesSample(timeSpan.EndDate, getLinearInterpolationValue(timeSpan.EndDate)));

    // return the average value (time-weighted) under the sample points provided
    double value = averageTimeWeightedValue(samplesInRange);
    _lastReturnedValue = value;
    _lastQueriedTimeSpan = timeSpan;
    return value;
}

vector<TimeSeries *> *TimeSeries::fromSMPFile(const QString filename)
{
    // Make a vector for the time series.
    vector<TimeSeries *> *timeSeries = new vector<TimeSeries *>();

    // Make a vector for the samples.
    vector<TimeSeriesSample> *samples = new vector<TimeSeriesSample>();

    // Try to make a sample from each line and add to the vector.
    ifstream file;
    file.open(filename.toAscii());
    string line;
    string previousName = "";
    string name = "";
    while (!file.eof()) {
        // Read the line from the file.
        getline(file, line);

        // Split the line.
        vector<string> tokens;
        StringUtil::tokenize(line, tokens, "/: \t");

        // S-166_HEAD  02/17/1947    00:00:00      0.37344527

        // If the line has the potential to be valid, process it.
        if (tokens.size() == 8) {
            // Get the data from the string.
            name = tokens.at(0);

            int month = atoi(tokens.at(1).c_str());
            int day = atoi(tokens.at(2).c_str());
            int year = atoi(tokens.at(3).c_str());
            int hour = atoi(tokens.at(4).c_str());
            int minute = atoi(tokens.at(5).c_str());
            int second = atoi(tokens.at(6).c_str());
            double value = atof(tokens.at(7).c_str());



            // If this is a new name and the sample vector is non-empty, push a time series to the time series vector and reset the sample vector.
            if (name.compare(previousName) != 0 && samples->size() > 0) {
                qDebug() << "adding time series " << previousName.c_str() << " to list";
                timeSeries->push_back(new TimeSeries(previousName.c_str(), samples));
                samples = new vector<TimeSeriesSample>();
            }

            // Add the current sample to the sample vector.
            samples->push_back(TimeSeriesSample(year, month, day, hour, minute, second, value));

            // Store the previous name.
            previousName = name;
        }
    }
    file.close();

    // Push the last time series to the time series vector.
    if (samples->size() > 0) {
        qDebug() << "adding final time series " << previousName.c_str() << " to list";
        timeSeries->push_back(new TimeSeries(previousName.c_str(), samples));
    }
    else {
        delete samples;
    }

    return timeSeries;
}

int TimeSeries::numSamples()
{
    return _samples->size();
}
