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
        /// Write an integer right-justified in a string of specified length
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldLength"></param>
        /// <returns></returns>
        public static string IntToString(int i, int fieldLength)
        {
            string str = i.ToString();
            str = PadLeftToLength(str, fieldLength);
            return str;
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

    }
}