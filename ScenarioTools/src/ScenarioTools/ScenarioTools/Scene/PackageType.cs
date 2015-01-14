namespace ScenarioTools.Scene
{
    public enum PackageType
    {
        // Each package type value has to agree with
        // index of corresponding icon in imageList1
        // of FeatureSetForm
        NoType = 0,
        WellType = 1,
        RiverType = 2,
        ChdType = 3,
        RchType = 4,
        GhbType = 5
    }

    public enum LayerMethod
    {
        Uniform = 0,
        ByAttribute = 1,
        ByCellTops = 2,         // Applies to WEL package
        UppermostActiveCell = 3 // Applies to RCH package
    }

    public static class Helpers
    {
        public static string PackageTypeToString(PackageType packageType)
        {
            switch (packageType)
            {
                case PackageType.NoType:
                    return "";
                case PackageType.ChdType:
                    return "CHD";
                case PackageType.RchType:
                    return "RCH";
                case PackageType.RiverType:
                    return "RIV";
                case PackageType.WellType:
                    return "WEL";
                case PackageType.GhbType:
                    return "GHB";
                default:
                    return "";
            }
        }
    }

}
