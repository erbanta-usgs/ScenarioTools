#ifndef RANGE1D_H
#define RANGE1D_H

class Range1D
{
public:
    Range1D(double min = 0.0, double max = 0.0);
    bool overlaps(Range1D range);
    double min();
    double max();

private:
    double _min;
    double _max;
};

#endif // RANGE1D_H
