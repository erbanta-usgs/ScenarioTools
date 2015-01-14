using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.DataClasses
{
    public class DataStatus
    {
        public const int DATA_AVAILABLE_CACHE_PRESENT = 0;
        public const int DATASET_NEEDS_REFRESH = 1;
        public const int DATA_UNAVAILABLE_CACHE_PRESENT = 2;
        public const int DATA_UNAVAILABLE_CACHE_MISSING = 3;

        public static int GetDataStatus(bool dataAvailable, bool cachePresent, bool datasetNeedsRefresh)
        {
            // If the dataset needs to be refresh, return a value to indicate such.
            if (datasetNeedsRefresh)
            {
                return DATASET_NEEDS_REFRESH;
            }

            if (dataAvailable && !datasetNeedsRefresh)
            {
                if (cachePresent)
                {
                    return DATA_AVAILABLE_CACHE_PRESENT;
                }
                else
                {
                    return DATASET_NEEDS_REFRESH;
                }
            }
            else
            {
                if (cachePresent)
                {
                    return DATA_UNAVAILABLE_CACHE_PRESENT;
                }
                else
                {
                    return DATA_UNAVAILABLE_CACHE_MISSING;
                }
            }
        }
    }
}
