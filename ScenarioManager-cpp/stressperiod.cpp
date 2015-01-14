#include "stressperiod.h"

StressPeriod::StressPeriod()
{
    _periodLength = 1.0;
}

StressPeriod::StressPeriod(double periodLength)
{
    _periodLength = periodLength;
}

double StressPeriod::periodLength()
{
    return _periodLength;
}


