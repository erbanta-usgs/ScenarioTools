using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ScenarioTools.Util
{
    public static class StringUtil
    {
        /// <summary>
        /// Fill blanks in filledString with fillChar
        /// </summary>
        /// <param name="str"></param>
        /// <param name="fillChar"></param>
        /// <returns></returns>
        public static string FillBlanks(string str, char fillChar)
        {
            string filledString = "";
            string oneChar;
            if (str != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    oneChar = str.Substring(i, 1);
                    if (oneChar == " ")
                    {
                        filledString = filledString + fillChar;
                    }
                    else
                    {
                        filledString = filledString + oneChar;
                    }
                }
            }
            return filledString;
        }

        public static string DoubleQuoteIfNeeded(string str)
        {
            char quot = '"';
            string output = "";
            if (str.Contains(" "))
            {
                output = quot.ToString() + str + quot.ToString();
            }
            else
            {
                output = str;
            }
            return output;
        }

        /// <summary>
        /// Write a double right-justified in a string of specified length
        /// </summary>
        /// <param name="x"></param>
        /// <param name="fieldWidth"></param>
        /// <returns></returns>
        public static string DoubleToString(double x, int fieldWidth)
        {
            string fmt, str;
            if (fieldWidth < 1)
            {
                return "";
            }
            if (x == 0.0d)
            {
                if (fieldWidth == 1)
                {
                    str = "0";
                }
                else if (fieldWidth == 2)
                {
                    str = "0.";
                }
                else
                {
                    str = "0.0";
                    str = PadLeftToLength(str, fieldWidth);
                }
                return str;
            }
            int ix;
            double log = Math.Log10(Math.Abs(x));
            int exp = Convert.ToInt32(log);
            int numDecDigits;
            int sign = 0;
            if (x < 0.0d)
            {
                sign = 1;
            }
            int break0 = 100;
            int break1 = 10;
            int break2 = 1;
            int break3 = -4;
            int break4 = -9;
            int break5 = -99;
            if (exp >= break0)
            {
                if (exp > fieldWidth - 3)
                {
                    numDecDigits = fieldWidth - 6 - sign;
                }
                else
                {
                    numDecDigits = fieldWidth - sign;
                }
                fmt = "G" + numDecDigits.ToString();
                str = x.ToString(fmt, CultureInfo.InvariantCulture);
                str = PadLeftToLength(str, fieldWidth);
            }
            else if (exp >= break1)
            {
                if (exp > fieldWidth - 3)
                {
                    numDecDigits = fieldWidth - 5 - sign;
                }
                else
                {
                    numDecDigits = fieldWidth - sign;
                }
                fmt = "G" + numDecDigits.ToString();
                str = x.ToString(fmt, CultureInfo.InvariantCulture);
                str = PadLeftToLength(str, fieldWidth);
            }
            else if (exp >= break2)
            {
                if (exp > (fieldWidth - 2))
                {
                    numDecDigits = fieldWidth - sign;
                    ix = Convert.ToInt32(x);
                    str = ix.ToString();
                    if (str.Length < fieldWidth)
                    {
                        str = str + ".";
                    }
                }
                else
                {
                    numDecDigits = fieldWidth - 1 - sign;
                    fmt = "G" + numDecDigits.ToString();
                    str = x.ToString(fmt,CultureInfo.InvariantCulture);
                }
                str = PadLeftToLength(str, fieldWidth);
            }
            else if (exp >= break3)
            {
                numDecDigits = fieldWidth - 2 - sign;
                fmt = "F" + numDecDigits.ToString();
                str = x.ToString(fmt, CultureInfo.InvariantCulture);
                str = PadLeftToLength(str, fieldWidth);
            }
            else if (exp >= break4)
            {
                numDecDigits = fieldWidth - 5 - sign;
                fmt = "G" + numDecDigits.ToString();
                str = x.ToString(fmt, CultureInfo.InvariantCulture);
                str = PadLeftToLength(str, fieldWidth);
            }
            else if (exp >= break5)
            {
                numDecDigits = fieldWidth - 5 - sign;
                fmt = "G" + numDecDigits.ToString();
                str = x.ToString(fmt, CultureInfo.InvariantCulture);
                str = PadLeftToLength(str, fieldWidth);
            }
            else
            {
                numDecDigits = fieldWidth - 6 - sign;
                fmt = "G" + numDecDigits.ToString();
                str = x.ToString(fmt, CultureInfo.InvariantCulture);
                str = PadLeftToLength(str, fieldWidth);
            }
            return str;
        }

        /// <summary>
        /// Write a string to a StreamWriter as one or more MODFLOW comment lines
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="commentString"></param>
        public static void InsertAsModflowCommentLines(StreamWriter sw, string commentString)
        {
            if (commentString != "")
            {
                // Insert commentString into input file.
                List<string> commentList = StringUtil.MakeList(commentString, 65, "# ");
                for (int i = 0; i < commentList.Count; i++)
                {
                    sw.WriteLine(commentList[i]);
                }
            }
        }

        /// <summary>
        /// Write an integer right-justified in a string of specified length
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldLength"></param>
        /// <returns></returns>
        public static string IntToString(int i, int fieldWidth)
        {
            string str = i.ToString();
            str = PadLeftToLength(str, fieldWidth);
            return str;
        }

        /// <summary>
        /// Returns a string generated by concatenating 
        /// specified range of elements in a string array
        /// </summary>
        /// <param name="words"></param>
        /// <param name="firstWord"></param>
        /// <param name="lastWord"></param>
        /// <returns></returns>
        public static string MakeString(string[] words, int firstWord, int lastWord)
        {
            string testString = words[firstWord];
            for (int i = firstWord + 1; i <= lastWord; i++)
            {
                testString = testString + " " + words[i];
            }
            return testString;
        }

        /// <summary>
        /// Add blanks to beginning of string as needed to obtain desired length
        /// </summary>
        /// <param name="oldString"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string PadLeftToLength(string oldString, int length)
        {
            string newString = oldString;
            if (oldString.Length < length)
            {
                string space = " ";
                while (newString.Length < length)
                {
                    newString = space + newString;
                }
            }
            return newString;
        }

        /// <summary>
        /// Add blanks to end of string as needed to obtain desired length
        /// </summary>
        /// <param name="oldString"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string PadRightToLength(string oldString, int length)
        {
            string newString = oldString;
            if (oldString.Length < length)
            {
                string spaces50 = "                                                  ";
                while (newString.Length < length)
                {
                    newString = newString + spaces50;
                }
                newString = newString.Remove(length);
            }
            return newString;
        }

        public static string[] ParseLine(string line)
        {
            char[] delimiters = { ' ', '\t' };
            string[] contents = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            return contents;
        }        

        public static string SingleQuoteIfNeeded(string str)
        {
            string output = "";
            if (str.Contains(" "))
            {
                output = "'" + str + "'";
            }
            else
            {
                output = str;
            }
            return output;
        }

        /// <summary>
        /// Convert a string to a List<string> using a
        /// maximum line length and leading string for each line
        /// </summary>
        /// <param name="inputString">Input string</param>
        /// <param name="lineMaxLen">Maximum line length</param>
        /// <param name="leadingString">Leading string</param>
        /// <returns></returns>
        public static List<string> MakeList(string inputString, int lineMaxLen,
                                            string leadingString)
        {
            string input = inputString;
            string line;
            string tempLine;
            List<string> list = new List<string>();
            if (input != "")
            {
                char[] delimiters = { ' ', '\t', '\n', '\r' };
                string[] words = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                int remainingWords = words.Length;
                int index = 0;
                while (remainingWords > 0)
                {
                    line = leadingString;
                    if (index < words.Length)
                    {
                        tempLine = line + words[index];                        
                        while (tempLine.Length < lineMaxLen && index < words.Length)
                        {
                            line = tempLine;
                            remainingWords--;
                            index++;
                            if (index < words.Length)
                            {
                                tempLine = line + " " + words[index];
                            }
                        }
                        if (line != leadingString)
                        {
                            // Ensure that line starts with leadingString.
                            if (line.IndexOf(leadingString) != 0)
                            {
                                line = leadingString + line;
                            }
                            list.Add(line);
                        }
                    }
                    else
                    {
                        remainingWords = 0;
                    }
                }
            }
            return list;
        }

        public static string DateNowAsString(bool includeTime)
        {
            DateTime now = DateTime.Now;
            string monthDayM = now.ToString("M");
            string year = now.Year.ToString();
            string returnValue = monthDayM + ", " + year;
            if (includeTime)
            {
                returnValue = returnValue + "  " + now.ToShortTimeString();
            }
            return returnValue;
        }

        public static string CustomNumberFormat(string myNumber)
        {
            string strNumberWithoutDecimals = "";
            string strNumberDecimals = "";
            int dotIndex = myNumber.IndexOf(".");
            if (dotIndex > -1)
            {
                strNumberWithoutDecimals = myNumber.Substring(0, dotIndex);
                strNumberDecimals = myNumber.Substring(dotIndex);
                strNumberWithoutDecimals = Convert.ToInt32(strNumberWithoutDecimals).ToString("#,##0");
                return strNumberWithoutDecimals + strNumberDecimals;
            }
            else
            {
                strNumberWithoutDecimals = Convert.ToInt32(myNumber).ToString("#,##0");
                return strNumberWithoutDecimals;
            }

        }

        public static bool StringToBool(string bString)
        {
            string caps = bString.ToLower();
            if (caps == "true" || caps == "t")
            {
                return true;
            }
            return false;
        }

        public static string RemoveOuterQuotes(string quotedString)
        {
            string singleQuote = "\'";
            string doubleQuote = "\"";
            string tempString = quotedString.Trim();
            int lastChar = tempString.Length - 1;
            if (tempString.Substring(0, 1) == singleQuote && tempString.Substring(lastChar, 1) == singleQuote)
            {
                return tempString.Substring(1, tempString.Length - 2);
            }
            if (tempString.Substring(0, 1) == doubleQuote && tempString.Substring(lastChar, 1) == doubleQuote)
            {
                return tempString.Substring(1, tempString.Length - 2);
            }
            return tempString;            
        }
    }
}