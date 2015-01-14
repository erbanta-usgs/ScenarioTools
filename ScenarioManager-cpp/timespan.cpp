#include "timespan.h"

TimeSpan::TimeSpan(QDateTime startDate, QDateTime endDate)
{
    StartDate = startDate;
    EndDate = endDate;
}

bool TimeSpan::contains(QDateTime date) {
    return date >= StartDate && date <= EndDate;
}

long TimeSpan::size() {
    return EndDate.toTime_t() - StartDate.toTime_t();
}
