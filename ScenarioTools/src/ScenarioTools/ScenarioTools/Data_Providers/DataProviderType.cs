using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Data_Providers
{
    public enum DataConsumerTypeEnum
    {
        Map,
        STMap,
        Table,
        Chart,
        None
    }
    public enum DataSeriesTypeEnum
    {
        ColorFillMapSeries,
        ContourMapSeries,
        ChartSeries,
        TableSeries,
        None
    }
}