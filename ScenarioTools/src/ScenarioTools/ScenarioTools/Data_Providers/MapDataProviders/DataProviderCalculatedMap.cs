using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using ScenarioTools.DataClasses;
using ScenarioTools.Geometry;
using ScenarioTools.Reporting;
using ScenarioTools.Xml;

namespace ScenarioTools.Data_Providers
{
    public class DataProviderCalculatedMap : IDataProvider
    {
        private enum TermOperator
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        private const string XML_KEY_OBJECT_KEY = "key";
        private const string XML_KEY_EXPRESSION = "expression";

        private const int TYPE_FUNCTION = 0;
        private const int TYPE_LAYER = 1;
        private const int TYPE_PARENS = 2;
        private const int TYPE_OPERATOR = 3;
        private const int TYPE_NUMBER = 4;

        #region Fields
        private ReportElementSTMap _parentSTMap;
        private string _expression;
        private object _dataset;
        private long _key;
        #endregion Fields

        #region Constructors
        public DataProviderCalculatedMap()
        {   
            // Set the data array to null.
            _dataset = null;

            // Set map to null and initialize expression
            _parentSTMap = null;
            _expression = "";
            //_modificationTime = DateTime.Now;
        }
        public DataProviderCalculatedMap(IReportMap parentReportMap)
            : base()
        {
            // Generate a key.
            _key = WorkspaceUtil.GetUniqueIdentifier();

            if (parentReportMap is ReportElementSTMap)
            {
                this._parentSTMap = (ReportElementSTMap)parentReportMap;
            }

            DataModificationTime = DateTime.MinValue;
        }
        public DataProviderCalculatedMap(ReportElementSTMap parentSTMap) : base()
        {
            // Generate a key.
            _key = WorkspaceUtil.GetUniqueIdentifier();

            // Store a reference to the parent map.
            this._parentSTMap = parentSTMap;
        }
        #endregion Constructors

        #region Properties
        public string Expression
        {
            get
            {
                if (_expression == null)
                {
                    _expression = "";
                }
                return _expression;
            }
            set
            {
                _expression = value;
            }
        }
        public ReportElementSTMap ParentSTMap
        {
            get
            {
                return _parentSTMap;
            }
        }
        public DateTime DataModificationTime { get; set; }
        public bool ConvertFlowToFlux { get; set; }
        #endregion Properties

        #region Methods
        public object EvaluateExpression()
        {
            // Evaluate the top-level expression.
            return evaluateExpression(_expression);
        }
        private object evaluateExpression(string expression)
        {
            // Trim all whitespace from the expression.
            expression = expression.Trim();

            string errMessage = "";
            // If the expression is atomic, return the value.
            if (isAtomic(expression, out errMessage))
            {
                return evaluateAtomic(expression);
            }

            // Otherwise, break the expression into two pieces, evaluate both pieces, and return the result.
            else
            {
                // Break the expression into two pieces.
                string term1, term2;
                TermOperator termOperator;
                breakExpression(expression, out term1, out term2, out termOperator);

                // Show the pieces.
                Console.WriteLine("term1: ~~" + term1 + "~~  \tterm2: ~~" + term2 + "~~  \toperator: " + getString(termOperator));

                // Evaluate both pieces.
                object term1result = evaluateExpression(term1);
                object term2result = evaluateExpression(term2);

                // Return the appropriate binary evaluation of the result.
                if (term1result is GeoMap && term2result is GeoMap)
                {
                    return evaluateBinary((GeoMap)term1result, (GeoMap)term2result, termOperator);
                }
                else if (term1result is GeoMap && term2result is float)
                {
                    return evaluateBinary((GeoMap)term1result, (float)term2result, termOperator);
                }
                else if (term1result is float && term2result is GeoMap)
                {
                    return evaluateBinary((float)term1result, (GeoMap)term2result, termOperator);
                }
                else if (term1result is float && term2result is float)
                {
                    return evaluateBinary((float)term1result, (float)term2result, termOperator);
                }
                else
                {
                    throw new ArgumentException("The types of the expressions are invalid.");
                }
            }
        }
        private GeoMap evaluateBinary(GeoMap geoMap1, GeoMap geoMap2, TermOperator termOperator)
        {
            switch (termOperator)
            {
                case TermOperator.Add:
                    return GeoMapOps.Sum(geoMap1, geoMap2, geoMap1);
                case TermOperator.Subtract:
                    return GeoMapOps.Difference(geoMap1, geoMap2, geoMap1);
                case TermOperator.Multiply:
                    return GeoMapOps.Product(geoMap1, geoMap2, geoMap1);
                case TermOperator.Divide:
                    return GeoMapOps.Quotient(geoMap1, geoMap2, geoMap1);
                default:
                    throw new ArgumentException("The operator " + termOperator + " is invalid.");
            }
        }
        private GeoMap evaluateBinary(GeoMap geoMap, float value, TermOperator termOperator)
        {
            switch (termOperator)
            {
                case TermOperator.Add:
                    return GeoMapOps.Sum(geoMap, value);
                case TermOperator.Subtract:
                    return GeoMapOps.Difference(geoMap, value);
                case TermOperator.Multiply:
                    return GeoMapOps.Product(geoMap, value);
                case TermOperator.Divide:
                    return GeoMapOps.Quotient(geoMap, value);
                default:
                    throw new ArgumentException("The operator " + termOperator + " is invalid.");
            }
        }
        private GeoMap evaluateBinary(float value, GeoMap geoMap, TermOperator termOperator)
        {
            switch (termOperator)
            {
                case TermOperator.Add:
                    return GeoMapOps.Sum(value, geoMap);
                case TermOperator.Subtract:
                    return GeoMapOps.Difference(value, geoMap);
                case TermOperator.Multiply:
                    return GeoMapOps.Product(value, geoMap);
                case TermOperator.Divide:
                    return GeoMapOps.Quotient(value, geoMap);
                default:
                    throw new ArgumentException("The operator " + termOperator + " is invalid.");
            }
        }
        private float evaluateBinary(float value1, float value2, TermOperator termOperator)
        {
            switch (termOperator)
            {
                case TermOperator.Add:
                    return value1 + value2;
                case TermOperator.Subtract:
                    return value1 - value2;
                case TermOperator.Multiply:
                    return value1 * value2;
                case TermOperator.Divide:
                    return value1 / value2;
                default:
                    throw new ArgumentException("The operator " + termOperator + " is invalid.");
            }
        }
        private bool isAtomic(string expression, out string errMessage) {
            // Strip outer parentheses from the expression.
            expression = stripFullyEnclosingParentheses(expression);

            // Break the expression into pieces.
            string[] expressions;
            TermOperator[] termOperators;
            errMessage = "";
            breakIntoTopLevelPieces(expression, out expressions, out termOperators, out errMessage);

            // The expression is atomic iff there is only one top-level expression.
            bool isAtomic = expressions.Length == 1;

            Console.WriteLine("The expression ~~" + expression + "~~ is " + (isAtomic ? "" : "not ") + "atomic.");
            return isAtomic;
        }
        private object evaluateAtomic(string expression)
        {
            // If this is a parseable numerical value, return the value.
            float value;
            if (float.TryParse(expression, out value))
            {
                return value;
            }

            // Otherwise, strip the braces and get the data series from the map.
            else
            {
                // Strip the braces from the expression.
                expression = expression.Substring(1, expression.Length - 2);
                return _parentSTMap.GetDataSeries(expression);
            }
        }
        private void breakExpression(string expression, out string term1, out string term2, out TermOperator termOperator)
        {
            // First, strip any whitespace and fully enclosing parentheses.
            expression = stripFullyEnclosingParentheses(expression.Trim());

            // Break the expression into top-level pieces.
            string[] expressions;
            TermOperator[] termOperators;
            string errMessage = "";
            breakIntoTopLevelPieces(expression, out expressions, out termOperators, out errMessage);

            // Show the pieces.
            Console.WriteLine("EXPRESSION PIECES:");
            for (int i = 0; i < expressions.Length; i++)
            {
                Console.WriteLine(expressions[i]);
                if (i < termOperators.Length)
                {
                    Console.WriteLine(getString(termOperators[i]));
                }
            }

            // First, try to find a weak operator (+ or -).
            int termIndex = -1;
            for (int i = 0; i < termOperators.Length && termIndex < 0; i++)
            {
                if (termOperators[i] == TermOperator.Add || termOperators[i] == TermOperator.Subtract)
                {
                    termIndex = i;
                }
            }

            // Otherwise, find a strong operator.
            for (int i = 0; i < termOperators.Length && termIndex < 0; i++) {
                if (termOperators[i] == TermOperator.Multiply || termOperators[i] == TermOperator.Divide)
                {
                    termIndex = i;
                }
            }

            // If the term index is not defined, throw an exception.
            if (termIndex < 0)
            {
                throw new ArgumentException("No suitable operator can be found in the expression " + expression + ".");
            }

            // Assemble the first term.
            StringBuilder term1sb = new StringBuilder();
            for (int i = 0; i <= termIndex; i++)
            {
                term1sb.Append(expressions[i]);
                if (i != termIndex)
                {
                    term1sb.Append(" " + getString(termOperators[i]) + " ");
                }
            }
            term1 = term1sb.ToString();

            // Assemble the second term.
            StringBuilder term2sb = new StringBuilder();
            for (int i = termIndex + 1; i < expressions.Length; i++)
            {
                term2sb.Append(expressions[i]);
                if (i != expressions.Length - 1)
                {
                    term2sb.Append(" " + getString(termOperators[i]) + " ");
                }
            }
            term2 = term2sb.ToString();

            // Assigne the term operator.
            termOperator = termOperators[termIndex];
        }
        private string getString(TermOperator termOperator)
        {
            switch (termOperator)
            {
                case TermOperator.Add:
                    return "+";
                case TermOperator.Subtract:
                    return "-";
                case TermOperator.Multiply:
                    return "*";
                case TermOperator.Divide:
                    return "/";
                default:
                    throw new ArgumentException(termOperator + " is not a valid term operator.");
            }
        }
        private void breakIntoTopLevelPieces(string expression, out string[] expressions, out TermOperator[] termOperators, out string errMessage)
        {
            int prevTokenType = -1;
            errMessage = "";
            // Make the list for the expressions and the operators.
            List<string> expressionList = new List<string>();
            List<TermOperator> termOperatorList = new List<TermOperator>();

            // Pull the pieces off the list.
            while (expression.Length > 0)
            {
                // Trim the expression.
                expression = expression.Trim();

                // Look at the first character. It is either:
                // 1) A function -- this will be a letter.
                // 2) A number -- this will be a digit from 0-9, inclusive, or a period.
                // 3) A layer -- this will be an open brace.
                // 4) An operator -- one of { +, -, *, / }.
                // 5) A complex expression -- '('.

                char firstChar = expression[0];

                // This is the case for a function.
                if ((firstChar >= 'a' && firstChar <= 'z') || firstChar >= 'A' && firstChar <= 'Z')
                {
                    // Expression may not contain consecutive functions
                    if (prevTokenType == TYPE_FUNCTION)
                    {
                        errMessage = "Invalid Expression: Parse error.";
                    }
                    prevTokenType = TYPE_FUNCTION;
                    // Scan along until the closing parenthesis of the function is found.
                    int index = 0;
                    int numOpenParentheses = 0;
                    int numCloseParenteses = 0;
                    while (numOpenParentheses < 1 || numOpenParentheses != numCloseParenteses)
                    {
                        if (index < expression.Length)
                        {
                            if (expression[index] == '(')
                            {
                                numOpenParentheses++;
                            }
                            else if (expression[index] == ')')
                            {
                                numCloseParenteses++;
                            }
                            index++;
                        }
                        else
                        {
                            errMessage = "Invalid expression: Improper function or function syntax.";
                            expressions = new string [0];
                            termOperators = new TermOperator[0];
                            return;
                        }
                    }

                    // Pull the function from the expression and add it to the expression list.
                    expressionList.Add(expression.Substring(0, index + 1));
                    expression = expression.Substring(index + 1, expression.Length - index - 1);
                    Console.WriteLine("The new expression is: " + expression + ".");
                }

                // This is the case for a number.
                else if ((firstChar >= '0' && firstChar <= '9') || firstChar == '.')
                {
                    // Expression may not contain consecutive numbers
                    if (prevTokenType == TYPE_NUMBER)
                    {
                        errMessage = "Invalid Expression: Parse error.";
                    }
                    prevTokenType = TYPE_NUMBER;
                    int index = 0;
                    int numDecimalPoints = 0;

                    // The index stops on the last character that composes the number.

                    bool foundIndexOfLast = false;
                    while (!foundIndexOfLast)
                    {
                        // Check the current character.
                        bool isDigit = expression[index] >= '0' && expression[index] <= '9';
                        bool isDecimal = expression[index] == '.';
                        
                        // If this is a decimal point, increment the number of decimal points.
                        if (isDecimal)
                        {
                            numDecimalPoints++;
                            
                            // If this is the second decimal point, set the index one back and stop.
                            if (numDecimalPoints > 1)
                            {
                                index--;
                                foundIndexOfLast = true;
                            }
                        }

                        // If this is a digit or decimal, and the end has not been reached, increment.
                        if ((isDigit || isDecimal) && index < expression.Length - 1 && numDecimalPoints <= 1)
                        {
                            index++;
                        }
                        else
                        {
                            // If this character is good, simply flag to stop.
                            if ((isDigit || isDecimal) && numDecimalPoints <= 1)
                            {
                                foundIndexOfLast = true;
                            }
                            // Otherwise, flag and step back one.
                            else
                            {
                                foundIndexOfLast = true;
                                index--;
                            }
                        }
                    }

                    // Pull the number from the expression and add it to the expression list.
                    expressionList.Add(expression.Substring(0, index + 1));
                    expression = expression.Substring(index + 1, expression.Length - index - 1);
                    Console.WriteLine("The new expression is: " + expression + ".");
                }

                // This is the case for a layer.
                else if (firstChar == '[')
                {
                    // Expression may not contain consecutive layers
                    if (prevTokenType == TYPE_LAYER)
                    {
                        errMessage = "Invalid Expression: Parse error.";
                    }
                    prevTokenType = TYPE_LAYER;
                    int index = 0;
                    while (expression[index] != ']')
                    {
                        index++;
                    }

                    // Pull the layer from the expression and add it to the expression list.
                    expressionList.Add(expression.Substring(0, index + 1));
                    expression = expression.Substring(index + 1, expression.Length - index - 1);
                    Console.WriteLine("The new expression is: " + expression + ".");
                }

                // This is the case for an operator.
                else if (firstChar == '+' || firstChar == '-' || firstChar == '*' || firstChar == '/')
                {
                    // Expression may not contain consecutive operators
                    if (prevTokenType == TYPE_OPERATOR)
                    {
                        errMessage = "Invalid Expression: Parse error.";
                    }
                    prevTokenType = TYPE_OPERATOR;
                    // Pull the operator from the expression and add it to the operator list.
                    termOperatorList.Add(parseTermOperator(firstChar));
                    expression = expression.Substring(1, expression.Length - 1);
                    Console.WriteLine("The new expression is: " + expression + ".");
                }

                // This is the case for a contained expression.
                else if (firstChar == '(')
                {
                    // Expression MAY contain consecutive parentheses
                    prevTokenType = TYPE_PARENS;
                    int numOpenParentheses = 0;
                    int numCloseParentheses = 0;

                    int index = 0;
                    while (numOpenParentheses == 0 || numOpenParentheses != numCloseParentheses)
                    {
                        if (expression[index] == '(')
                        {
                            numOpenParentheses++;
                        }
                        else if (expression[index] == ')')
                        {
                            numCloseParentheses++;
                        }
                        index++;
                    }

                    // Pull the complex expression from the expression and add it to the expression list.
                    expressionList.Add(expression.Substring(0, index));
                    expression = expression.Substring(index, expression.Length - index);
                    Console.WriteLine("The new expression is: " + expression + ".");
                }

                else
                {
                    throw new Exception("Invalid starting character: " + firstChar);
                }
            }

            // Set the output variables.
            expressions = expressionList.ToArray();
            termOperators = termOperatorList.ToArray();
        }
        private TermOperator parseTermOperator(char character)
        {
            switch (character)
            {
                case '+':
                    return TermOperator.Add;
                case '-':
                    return TermOperator.Subtract;
                case '*':
                    return TermOperator.Multiply;
                case '/':
                    return TermOperator.Divide;
                default:
                    throw new Exception(character + " is not a valid operator");
            }
        }
        private string stripFullyEnclosingParentheses(string expression)
        {
            // Strip the whitespace from the expression.
            expression = expression.Trim();

            // If the first character is a parenthesis, and its partner is the last character, remove both and recurse.
            if (expression[0] == '(')
            {
                int numParentheses = 1;
                int index = 1;
                while (numParentheses > 0)
                {
                    if (expression[index] == '(')
                    {
                        numParentheses++;
                    }
                    else if (expression[index] == ')')
                    {
                        numParentheses--;
                    }
                    index++;
                }
                index--;
                Console.WriteLine("index of matching: " + index);

                if (index == expression.Length - 1)
                {
                    return stripFullyEnclosingParentheses(expression.Substring(1, expression.Length - 2));
                }
                else
                {
                    return expression;
                }
            }

            // Otherwise, return the expression unaltered.
            else
            {
                return expression;
            }
        }
        #endregion Methods

        #region IDataProvider Members
        public object GetData(out int dataStatus)
        {
            // Determine the data status. For now, we'll assign it as either refresing (if not present) or good (if present).
            if (_dataset == null)
            {
                dataStatus = DataStatus.DATASET_NEEDS_REFRESH;
            }
            else
            {
                dataStatus = DataStatus.DATA_AVAILABLE_CACHE_PRESENT;
            }

            return _dataset;
        }
        public int GetDataStatus()
        {
            int dataStatus;
            GetData(out dataStatus);
            return dataStatus;
        }
        public object GetDataSynchronous()
        {
            if (_dataset == null)
            {
                try
                {
                    _dataset = EvaluateExpression();
                    DataModificationTime = DateTime.Now;
                }
                catch
                {
                    _dataset = null;
                }
            }
            return _dataset;
        }
        public void InvalidateDataset()
        {
            _dataset = null;
        }
        public bool SupportsDataConsumerType(DataConsumerTypeEnum dataConsumerType)
        {
            if (dataConsumerType == DataConsumerTypeEnum.Map || dataConsumerType == DataConsumerTypeEnum.STMap)
            {
                return true;
            }
            return false;
        }
        public string[] GetResultMessage()
        {
            if (_dataset == null)
            {
                return new string[] { "Invalid data set" };
            }
            else
            {
                return new string[] { "Valid data set" };
            }
        }
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderCalculatedMap");

            // Set the attributes.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, _key + "", "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_EXPRESSION, this.Expression, "");

            // Return the result.
            return element;
        }
        public void InitFromXml(XmlElement element, string sourceFileName)
        {
            // Get the key.
            _key = XmlUtil.SafeGetLongAttribute(element, XML_KEY_OBJECT_KEY, WorkspaceUtil.GetUniqueIdentifier());

            // Get the expression.
            this.Expression = XmlUtil.SafeGetStringAttribute(element, XML_KEY_EXPRESSION, "");
        }
        public string[] GetMetadata()
        {
            return new string[] {
                "Data Category: " + this.ToString(),
                "Expression: " + this.Expression
            };
        }
        public long Key
        {
            get
            {
                return _key;
            }
        }
        public void LoadCacheFromStream(System.IO.Stream stream, DateTime streamDateTime)
        {
            // Load the cache.
            _dataset = WorkspaceUtil.GetCachedGeoMap(stream);
            DataModificationTime = streamDateTime;
        }
        #endregion

        #region IHasUniqueIdentifier Members

        public long GetUniqueIdentifier()
        {
            return _key;
        }

        public void UpdateUniqueIdentifier()
        {
            _key = WorkspaceUtil.GetUniqueIdentifier();
        }

        public bool ValidateExpression(string expression, out string errMessage)
        {
            try
            {
                // Trim all whitespace from the expression.
                expression = expression.Trim();
                errMessage = "";

                // If the expression is atomic, return the value.
                if (isAtomic(expression, out errMessage))
                {
                    if (errMessage == "")
                    {
                        object evaluatedExpression = evaluateAtomic(expression);
                        if (evaluatedExpression != null)
                        {
                            return true;
                        }
                        else
                        {
                            errMessage = "Invalid Expression: Data series unknown.";
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                // Otherwise, break the expression into two pieces, evaluate both pieces, and return the result.
                else
                {
                    if (errMessage == "")
                    {
                        // Break the expression into two pieces.
                        string term1, term2;
                        TermOperator termOperator;
                        breakExpression(expression, out term1, out term2, out termOperator);

                        // Show the pieces.
                        Console.WriteLine("term1: ~~" + term1 + "~~  \tterm2: ~~" + term2 + "~~  \toperator: " + getString(termOperator));

                        // Evaluate both pieces.
                        object term1result = evaluateExpression(term1);
                        object term2result = evaluateExpression(term2);

                        // Return the appropriate binary evaluation of the result.
                        if (term1result is GeoMap && term2result is GeoMap)
                        {
                            evaluateBinary((GeoMap)term1result, (GeoMap)term2result, termOperator);
                            return true;
                        }
                        else if (term1result is GeoMap && term2result is float)
                        {
                            evaluateBinary((GeoMap)term1result, (float)term2result, termOperator);
                            return true;
                        }
                        else if (term1result is float && term2result is GeoMap)
                        {
                            evaluateBinary((float)term1result, (GeoMap)term2result, termOperator);
                            return true;
                        }
                        else if (term1result is float && term2result is float)
                        {
                            evaluateBinary((float)term1result, (float)term2result, termOperator);
                            return true;
                        }
                        else
                        {
                            errMessage = "Invalid Expression: Expression cannot be evaluated.";
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                errMessage = "Invalid Expression: Unable to interpret expression.";
                return false;
            }
        }

        public void ValidateUniqueIdentifier(List<long> uniqueIdentifiers)
        {
            // If this object's identifier is already in the list, update the identifier.
            if (uniqueIdentifiers.Contains(GetUniqueIdentifier()))
            {
                UpdateUniqueIdentifier();
            }

            // Add this object's identifier to the list.
            uniqueIdentifiers.Add(GetUniqueIdentifier());
        }

        #endregion

        #region IDeepCloneable Members
        public object DeepClone()
        {
            DataProviderCalculatedMap dataProviderCalcMapCopy = new DataProviderCalculatedMap();
            dataProviderCalcMapCopy._expression = this._expression;
            dataProviderCalcMapCopy.DataModificationTime = new DateTime(this.DataModificationTime.Ticks);
            dataProviderCalcMapCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            return dataProviderCalcMapCopy;
        }
        public void AssignParent(object parent)
        {
            if (parent is ReportElementSTMap)
            {
                _parentSTMap = (ReportElementSTMap)parent;
            }
        }
        #endregion IDeepCloneable Member

        public override string ToString()
        {
            return "Data Set Calculator";
        }

        public Extent GetExtent()
        {
            return null;
        }
    }
}
