using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using ScenarioTools.DataClasses;
using ScenarioTools.Data_Providers;
using ScenarioTools.Geometry;
using ScenarioTools.Reporting;
using ScenarioTools.Xml;

namespace ScenarioTools.DatasetCalculator
{
    public class DataProviderCalculatedSeries : IDataProvider
    {
        private enum TermOperator
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        #region Constants
        private const string XML_KEY_OBJECT_KEY = "key";
        private const string XML_KEY_EXPRESSION = "expression";
        private const int TYPE_FUNCTION = 0;
        private const int TYPE_LAYER = 1;
        private const int TYPE_PARENS = 2;
        private const int TYPE_OPERATOR = 3;
        private const int TYPE_NUMBER = 4;
        #endregion Constants

        #region Fields
        private IReportElement _parentElement;
        private string _expression;
        private string _errorMessage;
        private long _key;
        private object _dataset;
        private bool _datasetNeedsRefresh;
        private bool _currentDatasetObtainable;
        #endregion Fields

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
        public long Key
        {
            get
            {
                return _key;
            }
        }
        public DateTime DataModificationTime { get; set; }
        public bool ConvertFlowToFlux { get; set; }
        #endregion Properties

        #region Constructor
        private DataProviderCalculatedSeries()
        {
            _key = WorkspaceUtil.GetUniqueIdentifier();
            DataModificationTime = DateTime.MinValue;
            _errorMessage = "";
            _currentDatasetObtainable = true;
        }
        public DataProviderCalculatedSeries(IReportElement parentElement) : base()
        {
            // Store a reference to the parent element.
            this._parentElement = parentElement;
            _errorMessage = "";
            _currentDatasetObtainable = true;
        }
        #endregion Constructor

        #region Public Methods
        public IReportElement ParentElement
        {
            get
            {
                return _parentElement;
            }
        }
        public object EvaluateExpression()
        {
            _errorMessage = "";
            _currentDatasetObtainable = true;
            // Evaluate the top-level expression.
            return evaluateExpression(_expression);
        }
        public override string ToString()
        {
            return "Data Set Calculator";
        }
        public Extent GetExtent()
        {
            return null;
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
                        object atomicObject = evaluateAtomic(expression);
                        if (atomicObject is DataSeries)
                        {
                            DataSeries dataSeries = (DataSeries)atomicObject;
                            if (dataSeries != null)
                            {
                                return true;
                            }
                        }
                        else if (atomicObject is TimeSeries)
                        {
                            TimeSeries timeSeries = (TimeSeries)atomicObject;
                            if (timeSeries != null)
                            {
                                return true;
                            }
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
                        if (term1result is TimeSeries && term2result is TimeSeries)
                        {
                            evaluateBinary((TimeSeries)term1result, (TimeSeries)term2result, termOperator);
                            return true;
                        }
                        else if (term1result is TimeSeries && term2result is float)
                        {
                            evaluateBinary((TimeSeries)term1result, (float)term2result, termOperator);
                            return true;
                        }
                        else if (term1result is float && term2result is TimeSeries)
                        {
                            evaluateBinary((float)term1result, (TimeSeries)term2result, termOperator);
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
                return false;
            }
            catch (Exception ex)
            {
                errMessage = "Invalid Expression: Unable to interpret expression.";
                return false;
            }
        }
        #endregion Public Methods

        #region Private Methods
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
                if (term1result is TimeSeries && term2result is TimeSeries)
                {
                    return evaluateBinary((TimeSeries)term1result, (TimeSeries)term2result, termOperator);
                }
                else if (term1result is TimeSeries && term2result is float)
                {
                    return evaluateBinary((TimeSeries)term1result, (float)term2result, termOperator);
                }
                else if (term1result is float && term2result is TimeSeries)
                {
                    return evaluateBinary((float)term1result, (TimeSeries)term2result, termOperator);
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
        private TimeSeries evaluateBinary(TimeSeries timeSeries1, TimeSeries timeSeries2, TermOperator termOperator)
        {
            switch (termOperator)
            {
                case TermOperator.Add:
                    return TimeSeriesOps.Sum(timeSeries1, timeSeries2, timeSeries1);
                case TermOperator.Subtract:
                    return TimeSeriesOps.Difference(timeSeries1, timeSeries2, timeSeries1);
                case TermOperator.Multiply:
                    return TimeSeriesOps.Product(timeSeries1, timeSeries2, timeSeries1);
                case TermOperator.Divide:
                    return TimeSeriesOps.Quotient(timeSeries1, timeSeries2, timeSeries1);
                default:
                    throw new ArgumentException("The operator " + termOperator + " is invalid.");
            }
        }
        private TimeSeries evaluateBinary(TimeSeries timeSeries, float value, TermOperator termOperator)
        {
            switch (termOperator)
            {
                case TermOperator.Add:
                    return TimeSeriesOps.Sum(timeSeries, value);
                case TermOperator.Subtract:
                    return TimeSeriesOps.Difference(timeSeries, value);
                case TermOperator.Multiply:
                    return TimeSeriesOps.Product(timeSeries, value);
                case TermOperator.Divide:
                    return TimeSeriesOps.Quotient(timeSeries, value);
                default:
                    throw new ArgumentException("The operator " + termOperator + " is invalid.");
            }
        }
        private TimeSeries evaluateBinary(float value, TimeSeries timeSeries, TermOperator termOperator)
        {
            switch (termOperator)
            {
                case TermOperator.Add:
                    return TimeSeriesOps.Sum(value, timeSeries);
                case TermOperator.Subtract:
                    return TimeSeriesOps.Difference(value, timeSeries);
                case TermOperator.Multiply:
                    return TimeSeriesOps.Product(value, timeSeries);
                case TermOperator.Divide:
                    return TimeSeriesOps.Quotient(value, timeSeries);
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
            // Strip outer parentheses from the expression.
            expression = stripFullyEnclosingParentheses(expression);

            // If this is a parseable numerical value, return the value.
            float value;
            if (float.TryParse(expression, out value))
            {
                return value;
            }
            else
            {
                // If this is a function, evaluate it and return the value
                // Ned TODO: Add code in DataProviderCalculatedSeries.evaluateAtomic to evaluate a function
                string functionName = extractFunctionName(expression).ToLower();
                if (isSupportedFunction(functionName))
                {
                    string functionArgument = extractFunctionArgument(expression);
                    string[] functionArguments = parseFunctionArguments(expression);
                    if (functionArguments.Length == 1)
                    {
                        object argument = evaluateAtomic(functionArgument);
                        if (argument is TimeSeries)
                        {
                            TimeSeries timeSeries = (TimeSeries)argument;
                            object functionResult = evaluateFunction(functionName, timeSeries);
                            return functionResult;
                        }
                    }
                    else if (functionArguments.Length > 1)
                    {
                        List<TimeSeries> timeSeriesList = new List<TimeSeries>();
                        for (int i = 0; i < functionArguments.Length; i++)
                        {
                            object argument = evaluateAtomic(functionArguments[i]);
                            if (argument is TimeSeries)
                            {
                                timeSeriesList.Add((TimeSeries)argument);
                            }
                        }
                        object functionResult = evaluateFunction(functionName, timeSeriesList);
                        return functionResult;
                    }
                }
            }

            // Otherwise, strip the braces and get the data series from the graph.
            {
                // Strip the brackets from the expression.

                expression = stripBrackets(expression); // expression.Substring(1, expression.Length - 2);

                // Get the data series from the graph.
                object dataSeriesObject = _parentElement.GetDataSeries(expression);
                if (dataSeriesObject == null)
                {
                    _errorMessage = "Error: Unable to evaluate expression '" + expression + "'";
                    _currentDatasetObtainable = false;
                }
                return dataSeriesObject;
            }
        }
        private string stripBrackets(string expression)
        {
            string argument = "";
            int indexLeft = -1;
            int indexRight = -1;
            for (int i = 0; i < expression.Length; i++)
            {
                if (indexLeft == -1 && expression[i] == '[')
                {
                    indexLeft = i;
                }
                if (expression[i] == ']')
                {
                    indexRight = i;
                }
            }
            if (indexLeft >= 0 && indexRight > indexLeft)
            {
                argument = expression.Substring(indexLeft + 1, indexRight - indexLeft - 1);
            }
            return argument;
        }
        private bool isSupportedFunction(string functionName)
        {
            string lowerFunction = functionName.ToLower();
            switch (lowerFunction)
            {
                case "minimum":
                case "maximum":
                case "average":
                case "sum":
                case "difference":
                    return true;
                default:
                    return false;
            }
        }
        private object evaluateFunction(string functionName, TimeSeries timeSeries)
        {
            // This method supports functions that take one TimeSeries as an argument
            switch (functionName)
            {
                case ("minimum"):
                    return minimum(timeSeries);
                case ("maximum"):
                    return maximum(timeSeries);
                case ("average"):
                    return average(timeSeries);
                //case ("sum"):
                //    return sum(timeSeries);
                default:
                    return null;
            }
        }
        private object evaluateFunction(string functionName, List<TimeSeries> timeSeriesList)
        {
            // This method supports functions that take a list of TimeSeries as an argument
            switch (functionName)
            {
                case ("sum"):
                    return sum(timeSeriesList);
                case ("difference"):
                    return difference(timeSeriesList);
                default:
                    return null;
            }
        }
        private TimeSeries minimum(TimeSeries timeSeries)
        {
            int numRecords = timeSeries.Length;
            TimeSeriesSample[] timeSeriesMinSamples = new TimeSeriesSample[numRecords];
            if (numRecords > 0)
            {
                float minVal = timeSeries[0].Value;
                // Find minimum value of time series
                for (int i = 1; i < numRecords; i++)
                {
                    if (timeSeries[i].Value < minVal)
                    {
                        minVal = timeSeries[i].Value;
                    }
                }
                // Populate array of TimeSeriesSample with records containing minimum value
                for (int i = 0; i < numRecords; i++)
                {
                    TimeSeriesSample newTimeSeriesSample = new TimeSeriesSample(timeSeries[i].Date, minVal);
                    timeSeriesMinSamples[i] = newTimeSeriesSample;
                }
            }
            TimeSeries timeSeriesMin = new TimeSeries(timeSeriesMinSamples);
            return timeSeriesMin;
        }
        private TimeSeries maximum(TimeSeries timeSeries)
        {
            int numRecords = timeSeries.Length;
            TimeSeriesSample[] timeSeriesMaxSamples = new TimeSeriesSample[numRecords];
            if (numRecords > 0)
            {
                float maxVal = timeSeries[0].Value;
                // Find maximum value of time series
                for (int i = 1; i < numRecords; i++)
                {
                    if (timeSeries[i].Value > maxVal)
                    {
                        maxVal = timeSeries[i].Value;
                    }
                }
                // Populate array of TimeSeriesSample with records containing maximum value
                for (int i = 0; i < numRecords; i++)
                {
                    TimeSeriesSample newTimeSeriesSample = new TimeSeriesSample(timeSeries[i].Date, maxVal);
                    timeSeriesMaxSamples[i] = newTimeSeriesSample;
                }
            }
            TimeSeries timeSeriesMax = new TimeSeries(timeSeriesMaxSamples);
            return timeSeriesMax;
        }
        private TimeSeries sum(List<TimeSeries> timeSeriesList)
        {
            int numSeries = timeSeriesList.Count;
            if (numSeries > 0)
            {
                int numRecords = timeSeriesList[0].Length;
                TimeSeriesSample[] timeSeriesSumSamples = new TimeSeriesSample[numRecords];
                float sumVal = 0.0f;
                DateTime dateTime;
                if (numRecords > 0)
                {
                    // Find sum of time series values
                    for (int i = 0; i < numRecords; i++)
                    {
                        dateTime = timeSeriesList[0][i].Date;
                        sumVal = 0.0f;
                        for (int j = 0; j < numSeries; j++)
                        {
                            sumVal = sumVal + timeSeriesList[j].GetValue(dateTime);
                        }
                        TimeSeriesSample newTimeSeriesSample = new TimeSeriesSample(dateTime, sumVal);
                        timeSeriesSumSamples[i] = newTimeSeriesSample;
                    }
                }
                TimeSeries timeSeriesSum = new TimeSeries(timeSeriesSumSamples);
                return timeSeriesSum;
            }
            return null;
        }
        private TimeSeries difference(List<TimeSeries> timeSeriesList)
        {
            int numSeries = timeSeriesList.Count;
            if (numSeries == 2)
            {
                int numRecords = timeSeriesList[0].Length;
                TimeSeriesSample[] timeSeriesDiffSamples = new TimeSeriesSample[numRecords];
                DateTime dateTime;
                float diff;
                if (numRecords > 0)
                {
                    // Find sum of time series values
                    for (int i = 0; i < numRecords; i++)
                    {
                        dateTime = timeSeriesList[0][i].Date;
                        diff = timeSeriesList[0][i].Value - timeSeriesList[1].GetValue(dateTime);
                        TimeSeriesSample newTimeSeriesSample = new TimeSeriesSample(dateTime, diff);
                        timeSeriesDiffSamples[i] = newTimeSeriesSample;
                    }
                }
                TimeSeries timeSeriesDiff = new TimeSeries(timeSeriesDiffSamples);
                return timeSeriesDiff;
            }
            return null;
        }
        private TimeSeries average(TimeSeries timeSeries)
        {
            int numRecords = timeSeries.Length;
            TimeSeriesSample[] timeSeriesAverageSamples = new TimeSeriesSample[numRecords];
            float sumVal = 0.0f;
            if (numRecords > 0)
            {
                // Find sum of time series values
                for (int i = 0; i < numRecords; i++)
                {
                    sumVal = sumVal + timeSeries[i].Value;
                }
                // Find average value
                float averageVal = sumVal / Convert.ToSingle(numRecords);
                // Populate array of TimeSeriesSample with records containing average value
                for (int i = 0; i < numRecords; i++)
                {
                    TimeSeriesSample newTimeSeriesSample = new TimeSeriesSample(timeSeries[i].Date, averageVal);
                    timeSeriesAverageSamples[i] = newTimeSeriesSample;
                }
            }
            TimeSeries timeSeriesAverage = new TimeSeries(timeSeriesAverageSamples);
            return timeSeriesAverage;
        }
        private string extractFunctionArgument(string expression)
        {
            string argument = "";
            int indexLeft = -1;
            int indexRight = -1;
            for (int i = 0; i < expression.Length; i++)
            {
                if (indexLeft == -1 && expression[i] == '(')
                {
                    indexLeft = i;
                }
                if (expression[i] == ')')
                {
                    indexRight = i;
                }
            }
            if (indexLeft >= 0 && indexRight > indexLeft)
            {
                argument = expression.Substring(indexLeft + 1, indexRight - indexLeft - 1);
            }
            return argument;
        }
        private string[] parseFunctionArguments(string expression)
        {
            bool OK = true;
            string[] arguments = null;
            try
            {
                // Discard function name and outside parenetheses
                string argument = extractFunctionArgument(expression);

                // Keep track of delimiters and nesting depth of () and [] pairs.
                List<string> argumentsList = new List<string>();
                int parensDepth = 0;
                int bracketDepth = 0;
                List<char> delimitChars = new List<char>();
                List<int> commaIndices = new List<int>();
                char character;
                for (int i = 0; i < argument.Length; i++)
                {
                    character = argument[i];
                    if (character == '(')
                    {
                        parensDepth++;
                        delimitChars.Add(character);
                    }
                    if (character == ')')
                    {
                        parensDepth--;
                        if (delimitChars[delimitChars.Count - 1] == '(')
                        {
                            delimitChars.RemoveAt(delimitChars.Count - 1);
                        }
                        else
                        {
                            OK = false;
                        }
                    }
                    if (character == '[')
                    {
                        bracketDepth++;
                        delimitChars.Add(character);
                    }
                    if (character == ']')
                    {
                        bracketDepth--;
                        if (delimitChars[delimitChars.Count - 1] == '[')
                        {
                            delimitChars.RemoveAt(delimitChars.Count - 1);
                        }
                        else
                        {
                            OK = false;
                        }
                    }
                    if (character == ',')
                    {
                        // Recognize a comma only if not enclosed in () or [] pairs
                        if (parensDepth == 0 && bracketDepth == 0 && delimitChars.Count == 0)
                        {
                            commaIndices.Add(i);
                        }
                    }
                }

                // Process delimiter information to generate list of arguments
                if (parensDepth == 0 && bracketDepth == 0 && delimitChars.Count == 0)
                {
                    if (commaIndices.Count == 0)
                    {
                        argumentsList.Add(expression);
                    }
                    else
                    {
                        int startIndex = -1;
                        int endIndex;
                        string arg;
                        for (int i = 0; i < commaIndices.Count; i++)
                        {
                            endIndex = commaIndices[i];
                            arg = argument.Substring(startIndex + 1, endIndex - startIndex - 1);
                            argumentsList.Add(arg);
                            startIndex = endIndex;
                        }
                        endIndex = argument.Length;
                        arg = argument.Substring(startIndex + 1, endIndex - startIndex - 1);
                        argumentsList.Add(arg);
                    }
                }
                else
                {
                    OK = false;
                }

                // Convert List to an array
                if (OK)
                {
                    arguments = argumentsList.ToArray();
                }
            }
            catch
            {
                OK = false;
            }
            return arguments;
        }
        private string extractFunctionName(string expression)
        {
            string functionName = "";
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i]=='(')
                {
                    functionName = expression.Substring(0, i);                    
                    break;
                }
            }
            return functionName;
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
                            expressions = new string[0];
                            termOperators = new TermOperator[0];
                            return;
                        }
                    }

                    // Pull the function from the expression and add it to the expression list.
                    if (index > 0)
                    {
                        index--;
                        expressionList.Add(expression.Substring(0, index + 1));
                        expression = expression.Substring(index + 1, expression.Length - index - 1);
                        Console.WriteLine("The new expression is: " + expression + ".");
                    }
                    else
                    {
                        errMessage = "Invalid Expression: Parse error.";
                    }
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

                    // Special case: If first character is "-" and expressionList is empty, add "0" as first expression
                    // to allow subtraction from zero for unary "-" operator.
                    if (firstChar == '-' && expressionList.Count == 0)
                    {
                        expressionList.Add("0");
                    }
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
        #endregion Private Methods

        #region IDataProvider Members
        public object GetData(out int dataStatus)
        {
            // Determine the data status. For now, we'll assign it as either refreshing (if not present) or good (if present).
            if (_dataset == null)
            {
                if (_currentDatasetObtainable)
                {
                    dataStatus = DataStatus.DATASET_NEEDS_REFRESH;
                }
                else
                {
                    dataStatus = DataStatus.DATA_UNAVAILABLE_CACHE_MISSING;
                }
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
            _currentDatasetObtainable = true;
            _errorMessage = "";
            _dataset = null;
        }
        public bool SupportsDataConsumerType(DataConsumerTypeEnum dataConsumerType)
        {
            if (dataConsumerType == DataConsumerTypeEnum.Chart)
            {
                return true;
            }
            return false;
        }
        public string[] GetResultMessage()
        {
            int result;
            return new string[] {
                GetData(out result) == null ? "Invalid data set" : "Valid data set"
            };
        }
        public XmlNode GetXmlNode(System.Xml.XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderCalculatedSeries");

            // Set the attributes.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, _key + "", "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_EXPRESSION, this.Expression, "");

            // Return the result.
            return element;
        }

        public void InitFromXml(System.Xml.XmlElement element, string sourceFileName)
        {
            // Get the key.
            _key = XmlUtil.SafeGetLongAttribute(element, XML_KEY_OBJECT_KEY, WorkspaceUtil.GetUniqueIdentifier());

            // Get the expression.
            this.Expression = XmlUtil.SafeGetStringAttribute(element, XML_KEY_EXPRESSION, "");
            //_modificationTime = DateTime.Now;
        }

        public string[] GetMetadata()
        {
            return new string[] {
                "Data Category: " + this.ToString(),
                "Expression: " + this.Expression
            };
        }

        public void LoadCacheFromStream(Stream stream, DateTime streamDateTime)
        {
            // Load the cache.
            _dataset = WorkspaceUtil.GetCachedTimeSeriesOrFloat(stream);

            // If the dataset is no longer null, switch off the update flag.
            if (_dataset != null)
            {
                _datasetNeedsRefresh = false;
                DataModificationTime = streamDateTime;
            }
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
            DataProviderCalculatedSeries dataProvCalcSeriesCopy = new DataProviderCalculatedSeries();
            dataProvCalcSeriesCopy._expression = this._expression;
            dataProvCalcSeriesCopy._dataset = null;
            dataProvCalcSeriesCopy._datasetNeedsRefresh = true;
            dataProvCalcSeriesCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            return dataProvCalcSeriesCopy;
        }
        public void AssignParent(object parent)
        {
            if (parent is IReportElement)
            {
                this._parentElement = (IReportElement)parent;
            }
        }
        #endregion IDeepCloneable Members
    }
}
