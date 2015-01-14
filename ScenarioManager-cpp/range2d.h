#ifndef RANGE2D_H
#define RANGE2D_H

#include "range1d.h"

class Range2D
{
public:
    Range2D(Range1D xRange = Range1D(), Range1D yRange = Range1D());

    Range1D xRange();
    Range1D yRange();
    bool overlaps(Range2D range);

private:
    Range1D _xRange;
    Range1D _yRange;
};

#endif // RANGE2D_H
