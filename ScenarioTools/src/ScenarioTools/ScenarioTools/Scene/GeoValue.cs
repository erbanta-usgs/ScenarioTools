namespace ScenarioTools.Scene
{
    public enum GeoValueType
    {
        Uniform = 0,
        Attribute = 1
    }

    public class GeoValue
    {
        #region Properties
        public string Descriptor { get; set; }
        public string Attribute { get; set; }
        public double UniformValue { get; set; }
        public GeoValueType GeoValueType { get; set; }
        #endregion Properties

        #region Constructors
        public GeoValue()
        {
            Descriptor = "";
            GeoValueType = GeoValueType.Uniform;
            Attribute = "";
            UniformValue = double.NaN;
        }
        public GeoValue(string attribute) : this()
        {
            GeoValueType = GeoValueType.Attribute;
            Attribute = attribute;
        }
        public GeoValue(double uniformValue) : this()
        {
            GeoValueType = GeoValueType.Uniform;
            UniformValue = uniformValue;
        }
        public GeoValue(GeoValueType geoValueType, string attribute, double uniformValue)
        {
            Descriptor = "";
            GeoValueType = geoValueType;
            Attribute = attribute;
            UniformValue = uniformValue;
        }
        public GeoValue(GeoValue geoValue)
        {
            Descriptor = geoValue.Descriptor;
            GeoValueType = geoValue.GeoValueType;
            Attribute = geoValue.Attribute;
            UniformValue = geoValue.UniformValue;
        }
        #endregion Constructors
    }
}
