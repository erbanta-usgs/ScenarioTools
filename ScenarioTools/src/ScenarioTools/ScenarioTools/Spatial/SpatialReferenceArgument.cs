using System;
using System.Collections.Generic;
using System.Text;

using ScenarioTools.Numerical;

namespace ScenarioTools.Spatial
{
    public enum SpatialReferenceArgumentType
    {
        StringType = 0,
        DoubleType = 1,
        ParameterType = 2,
        Unknown = 3
    }

    public class SpatialReferenceArgument
    {
        #region Fields
        private SpatialReferenceArgumentType _type;
        private string _argumentString;
        private string _argErrMsg;
        private double _value;
        private bool _matchFound;
        private SpatialReferenceParameter _parameter;
        #endregion Fields

        #region Constructors
        public SpatialReferenceArgument()
        {
            _type = SpatialReferenceArgumentType.Unknown;
            _argumentString = "";
            _argErrMsg = "";
            _value = double.NaN;
            _matchFound = true;
            _parameter = null;
        }
        public SpatialReferenceArgument(string argument)
            : this()
        {
            SpatialReferenceParameter newParameter = null;
            // Try to create a SpatialReferenceParameter from the argument
            try
            {
                newParameter = new SpatialReferenceParameter(argument);
            }
            catch
            {
            }

            // If successful, assign type as ParameterType
            if (newParameter.Keyword != "")
            {
                _parameter = newParameter;
                _type = SpatialReferenceArgumentType.ParameterType;
            }
            // If not, it's either a number or a string
            else
            {
                try
                {
                    _value = Convert.ToDouble(argument);
                    _type = SpatialReferenceArgumentType.DoubleType;
                }
                catch
                {
                    _argumentString = argument;
                    _type = SpatialReferenceArgumentType.StringType;
                }
            }
        }
        #endregion Constructors

        #region Properties
        public SpatialReferenceArgumentType Type
        {
            get
            {
                return _type;
            }
        }
        public string ArgumentString
        {
            get
            {
                return _argumentString;
            }
        }
        public string ArgErrorMessage
        {
            get
            {
                return _argErrMsg;
            }
            set
            {
                _argErrMsg = value;
            }
        }
        public double DoubleValue
        {
            get
            {
                return _value;
            }
        }
        public bool MatchFound
        {
            get
            {
                return _matchFound;
            }
            set
            {
                _matchFound = value;
            }
        }
        public SpatialReferenceParameter Parameter
        {
            get
            {
                return _parameter;
            }
        }
        #endregion Properties

        #region Public methods
        public object GetArgument()
        {
            switch (_type)
            {
                case SpatialReferenceArgumentType.DoubleType:
                    return _value;
                case SpatialReferenceArgumentType.ParameterType:
                    return _parameter;
                case SpatialReferenceArgumentType.StringType:
                    return _argumentString;
                case SpatialReferenceArgumentType.Unknown:
                    return null;  
                default:
                    return null;
            }
        }
        public SpatialRefCompareResult Compare(SpatialReferenceArgument otherArgument)
        {
            SpatialRefCompareResult result = SpatialRefCompareResult.Same;

            // If "this" argument is ParameterType with keyword AUTHORITY or TOWGS84,
            // no need to check, just return Same
            if (this.Type == SpatialReferenceArgumentType.ParameterType)
            {
                string keywordA = this.Parameter.Keyword;
                if (keywordA == "AUTHORITY" || keywordA == "TOWGS84")
                {
                    return result;
                }
            }

            if (otherArgument.Type == this.Type)
            {
                switch (Type)
                {
                    case SpatialReferenceArgumentType.DoubleType:
                        if (double.IsNaN(this.DoubleValue) || double.IsNaN(otherArgument.DoubleValue))
                        {
                            if (!double.IsNaN(this.DoubleValue) || !double.IsNaN(otherArgument.DoubleValue))
                            {
                                result = SpatialRefCompareResult.Different;
                            }
                        }
                        else
                        {
                            double eps = 1.0e-6;
                            if (!MathUtil.Fuzzy_Equals(this.DoubleValue, otherArgument.DoubleValue, eps))
                            {
                                result = SpatialRefCompareResult.Different;
                            }
                        }
                        break;
                    case SpatialReferenceArgumentType.ParameterType:
                        result = this.Parameter.Compare(otherArgument.Parameter);
                        break;
                    case SpatialReferenceArgumentType.StringType:
                        if (!SpatialReferenceHelpers.Synonymous(this.ArgumentString, otherArgument.ArgumentString))
                        {
                            result = SpatialRefCompareResult.Different;
                        }
                        break;
                }
            }
            else
            {
                result = SpatialRefCompareResult.Different;
            }
            return result;
        }
        public void AppendErrorMessage(ref string errorMessage)
        {
            switch (_type)
            {
                case SpatialReferenceArgumentType.DoubleType:
                    errorMessage = errorMessage + _argErrMsg;
                    break;
                case SpatialReferenceArgumentType.ParameterType:
                    _parameter.AppendErrorMessage(ref errorMessage);
                    break;
                case SpatialReferenceArgumentType.StringType:
                    errorMessage = errorMessage + _argErrMsg;
                    break;
                default:
                    break;
            }
        }
        public override string ToString()
        {
            switch (_type)
            {
                case SpatialReferenceArgumentType.DoubleType:
                    return _value.ToString();
                case SpatialReferenceArgumentType.ParameterType:
                    return _parameter.Keyword;
                case SpatialReferenceArgumentType.StringType:
                    return _argumentString;
                case SpatialReferenceArgumentType.Unknown:
                    return "argument unknown";
                default:
                    return "argument unknown";
            }
        }
        #endregion Public methods
    }
}
