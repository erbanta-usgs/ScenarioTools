using System;
using System.Collections.Generic;
using System.Text;

using ScenarioTools.Util;

namespace ScenarioTools.Spatial
{
    public class SpatialReferenceParameter
    {
        #region Fields
        private string _keyword;
        private string _argString;
        private List<SpatialReferenceArgument> _arguments;
        private List<string> _argumentStrings;
        #endregion Fields

        #region Constructors
        public SpatialReferenceParameter()
        {
            _keyword = "";
            _argString = "";
            _arguments = new List<SpatialReferenceArgument>();
            _argumentStrings = new List<string>();
        }
        public SpatialReferenceParameter(string wktString)
            : this()
        {
            // Process Well Known Text string to define keyword 
            // and parse arguments to a list of argument strings
            processWktString(wktString);

            // Process each argument string to a SpatialReferenceArgument 
            // and add to list of arguments
            for (int i = 0; i < _argumentStrings.Count; i++)
            {
                SpatialReferenceArgument newArgument = new SpatialReferenceArgument(_argumentStrings[i]);
                _arguments.Add(newArgument);
            }
        }
        #endregion Constructors

        #region Properties
        public string Keyword
        {
            get
            {
                return _keyword;
            }
        }
        public string ArgString
        {
            get
            {
                return _argString;
            }
        }
        public List<SpatialReferenceArgument> Arguments
        {
            get
            {
                return _arguments;
            }
        }
        #endregion Properties

        #region Public methods
        public SpatialRefCompareResult Compare(SpatialReferenceParameter otherParameter)
        {
            SpatialRefCompareResult result = SpatialRefCompareResult.Same;
            SpatialReferenceArgument thisArgument, otherArgument;
            string errMsgLocal = "";

            // Iterate through arguments of this parameter and look for matching 
            // argument of other parameter
            for (int i = 0; i < this.Arguments.Count; i++)
            {
                thisArgument = this.Arguments[i];

                // Look for best match among otherParameter's arguments
                // First, assume the two arguments are Unequal
                SpatialRefCompareResult testCompareResult = SpatialRefCompareResult.Different;
                SpatialRefCompareResult tempResult;

                // Iterate through to look for a better match.  If a Same is found, break.
                for (int j = 0; j < otherParameter.Arguments.Count; j++)
                {
                    otherArgument = otherParameter.Arguments[j];
                    tempResult = thisArgument.Compare(otherArgument);
                    if (tempResult < testCompareResult)
                    {
                        testCompareResult = tempResult;
                        if (tempResult == SpatialRefCompareResult.Same)
                        {
                            thisArgument.MatchFound = true;
                            thisArgument.ArgErrorMessage = "";
                            break;
                        }
                    }
                }

                // If testCompareResult is worse than current result, assign to current result
                if (testCompareResult > result)
                {
                    if (this._keyword == otherParameter._keyword)
                    {
                        errMsgLocal = "No match for " + this._keyword + " argument \"" + thisArgument.ToString() + "\". ";
                        thisArgument.ArgErrorMessage = errMsgLocal;
                        thisArgument.MatchFound = false;
                    }
                    result = testCompareResult;
                }
            }

            return result;
        }
        public void AppendErrorMessage(ref string errorMessage)
        {
            for (int i = 0; i < _arguments.Count; i++)
            {
                if (!_arguments[i].MatchFound)
                {
                    _arguments[i].AppendErrorMessage(ref errorMessage);
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private void processWktString(string wktString)
        {
            char[] leftBrackets = { '[', '(' };
            char[] rightBrackets = { ']', ')' };
            string alphaCaps = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int len;
            string keyword = "";
            string argString;
            string remainingString;

            // Initialize variables
            argString = "";
            remainingString = "";

            // Look for structure ABCD[...] which defines a parameter identified
            // by keyword ABCD. (Standard says keyword must be all caps.)
            int posLeftBracket = wktString.IndexOfAny(leftBrackets);
            int posKeyword = -1;
            string letterString;
            if (posLeftBracket >= 1)
            {
                // Find keyword
                for (int i = posLeftBracket - 1; i >= 0; i--)
                {
                    letterString = wktString.Substring(i, 1);
                    if (!alphaCaps.Contains(letterString))
                    {
                        posKeyword = i + 1;
                        len = posLeftBracket - posKeyword;
                        keyword = wktString.Substring(posKeyword, len);
                        break;
                    }
                    if (i == 0)
                    {
                        posKeyword = 0;
                        len = posLeftBracket;
                        keyword = wktString.Substring(posKeyword, len);
                        break;
                    }
                }

                // If keyword has been found, create a SpatialReferenceParameter 
                // and get what's in brackets following keyword
                if (keyword != "")
                {
                    // First, remove everything up through the first left bracket
                    string tempString = wktString.Remove(0, posLeftBracket + 1);

                    // Find matching right bracket
                    len = tempString.Length;
                    int k = 1;  // Nesting level within brackets
                    for (int i = 0; i < len; i++)
                    {
                        if (tempString.Substring(i, 1).IndexOfAny(leftBrackets) == 0)
                        {
                            k++;
                        }
                        else if (tempString.Substring(i, 1).IndexOfAny(rightBrackets) == 0)
                        {
                            k--;
                        }
                        if (k == 0)
                        {
                            // Found ']' that matches initial '['
                            len = i;
                            argString = tempString.Substring(0, len);
                            remainingString = tempString.Remove(0, len);
                            break;
                        }
                    }
                    this._keyword = keyword;
                    this._argString = argString;
                    this.parseArgumentString();
                }
            }
        }
        private void parseArgumentString()
        {
            const string COMMA = ",";
            const string DOUBLEQUOTE = "\"";
            const string LEFTBRACKET = "[";
            const string LEFTPARENS = "(";
            const string RIGHTBRACKET = "]";
            const string RIGHTPARENS = ")";
            string delimiters = COMMA + DOUBLEQUOTE +
                                LEFTBRACKET + LEFTPARENS +
                                RIGHTBRACKET + RIGHTPARENS;
            bool openQuote = false;
            bool processingArgument = false;
            int bracketDepth = 0;
            int parensDepth = 0;
            int argStartPos = -1;
            int argEndPos = -1;
            int commaPos = -1;
            string currentCharacter = "";

            int len = _argString.Length;

            // Process argument string and store each argument as it is recognized
            for (int i = 0; i < len; i++)
            {
                currentCharacter = _argString.Substring(i, 1);
                if (delimiters.IndexOf(currentCharacter) > -1)
                {
                    // current character is a delimiter
                    switch (currentCharacter)
                    {
                        case COMMA:
                            if (bracketDepth == 0 && parensDepth == 0)
                            {
                                commaPos = i;
                                if (processingArgument)
                                {
                                    argEndPos = i - 1;
                                }
                                else
                                {
                                    argStartPos = i + 1;
                                    processingArgument = true;
                                }
                            }
                            break;
                        case DOUBLEQUOTE:
                            if (!openQuote)
                            {
                                openQuote = true;
                                processingArgument = true;
                                if (bracketDepth == 0 && parensDepth == 0)
                                {
                                    argStartPos = i;
                                }
                            }
                            else
                            {
                                openQuote = false;
                                if (bracketDepth == 0 && parensDepth == 0)
                                {
                                    argEndPos = i;
                                }
                            }
                            break;
                        case LEFTBRACKET:
                            bracketDepth++;
                            processingArgument = true;
                            break;
                        case LEFTPARENS:
                            parensDepth++;
                            processingArgument = true;
                            break;
                        case RIGHTBRACKET:
                            if (bracketDepth > 0)
                            {
                                bracketDepth--;
                                if (bracketDepth == 0 && parensDepth == 0 && !openQuote)
                                {
                                    argEndPos = i;
                                }
                            }
                            break;
                        case RIGHTPARENS:
                            if (parensDepth > 0)
                            {
                                parensDepth--;
                                if (bracketDepth == 0 && parensDepth == 0 && !openQuote)
                                {
                                    argEndPos = i;
                                }
                            }
                            break;
                    }
                }

                // If processing an argument when end of argument string is reached, treat argument as complete.
                if (argStartPos > -1 && i == len - 1)
                {
                    if (bracketDepth == 0 && parensDepth == 0)
                    {
                        argEndPos = i;
                    }
                }

                // If both argStartPos and argEndPos > -1, they define start and end of an argument
                if (argStartPos > -1 && argEndPos > -1 && argEndPos >= argStartPos)
                {
                    int argLen = argEndPos - argStartPos + 1;
                    string newArgString = _argString.Substring(argStartPos, argLen);

                    // Remove enclosing quote marks, if any
                    newArgString = StringUtil.RemoveOuterQuotes(newArgString);

                    // Add argument string to list
                    _argumentStrings.Add(newArgString);

                    // Reset variables for search for next argument
                    argEndPos = -1;
                    argStartPos = -1;
                    processingArgument = false;
                    if (commaPos > -1)
                    {
                        argStartPos = commaPos + 1;
                        processingArgument = true;
                    }
                    commaPos = -1;
                }
            }
        }
        #endregion Private methods
    }
}
