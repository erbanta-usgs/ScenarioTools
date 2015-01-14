#include "range2d.h"

Range2D::Range2D(Range1D xRange, Range1D yRange)
{
    _xRange = xRange;
    _yRange = yRange;
}

Range1D Range2D::xRange()
{
    return _xRange;
}

Range1D Range2D::yRange()
{
    return _yRange;
}

bool Range2D::overlaps(Range2D range)
{
    return _xRange.overlaps(range.xRange()) && _yRange.overlaps(range.yRange());
}
