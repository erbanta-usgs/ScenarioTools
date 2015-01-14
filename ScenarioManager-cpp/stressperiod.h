#ifndef STRESSPERIOD_H
#define STRESSPERIOD_H

class StressPeriod
{
public:
    StressPeriod();
    StressPeriod(double periodLength);
    double periodLength();

private:
    double _periodLength;
};

#endif // STRESSPERIOD_H
