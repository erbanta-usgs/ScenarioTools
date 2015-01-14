using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.DataClasses
{
    public class LengthReference
    {
        public enum ModflowLengthUnit
        {
            feet = 1,
            meters = 2,
            centimeters = 3
        }

        public static string ConvertLengthUnitToString(ModflowLengthUnit modflowLengthUnit)
        {
            switch (modflowLengthUnit)
            {
                case ModflowLengthUnit.feet:
                    return "FEET";
                case ModflowLengthUnit.meters:
                    return "METERS";
                case ModflowLengthUnit.centimeters:
                    return "CENTIMETERS";
                default:
                    return "";
            }
        }
    }
}
