using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.PdfWriting
{
    public class DateUtil
    {
        public static String dateToStringMMDDCCYYFormat(DateTime date, String separator)
        {
            int month = date.Month;
            int day = date.Day;
            int year = date.Year;

            String monthString = (month + "").PadLeft(2, '0');
            String dayString = (day + "").PadLeft(2, '0');
            String yearString = (year + "").PadLeft(4, '0');

            return monthString + separator + dayString + separator + yearString;
        }
        public static string dateTimeToStringNoDay(DateTime dateTime)
        {
            string tempDate = dateTime.ToLongDateString();
            int indexFirstSpace = tempDate.IndexOf(' ');
            string shortDate = tempDate.Substring(indexFirstSpace + 1);
            string tempTime = dateTime.ToShortTimeString();
            return shortDate + " " + tempTime;
        }
    }
}
