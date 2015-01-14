#include "range1d.h"

Range1D::Range1D(double min, double max)
{
    _min = min;
    _max = max;
}

double Range1D::min()
{
    return _min;
}

double Range1D::max()
{
    return _max;
}

bool Range1D::overlaps(Range1D range) {
    if (_min < range.min())
    {
        return (_max >= range.min());
    }
    else
    {
        return (range.max() >= _min);
    }
}
