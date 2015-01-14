using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioTools.DataClasses
{
    public class MapEnums
    {
        public enum BlankingMode
        {
            None = 0,
            BySpecifiedLayer = 1,
            AnyLayerInactive = 2,
            AllLayersInactive = 3
        }
    }
}
