using System;

using ScenarioTools.DataClasses;

namespace ScenarioTools.ModflowReaders
{
    public class StressPeriod
    {
        double perlen;
        int nstp;
        double tsmult;
        SsTr sstr;
        TimeSpan timeSpan;

        public int Nstp
        {
            get
            {
                return nstp;
            }
        }

        public StressPeriod(double perlen, int nstp, double tsmult, SsTr sstr)
        {
            this.perlen = perlen;
            this.nstp = nstp;
            this.tsmult = tsmult;
            this.sstr = sstr;
        }
        public bool isTransient()
        {
            return sstr == SsTr.TR;
        }
        public static StressPeriod parse(String s)
        {
            double perlen = double.NaN;
            int nstp = -1;
            double tsmult = double.NaN;
            SsTr sstr = SsTr.None;
            string strSstr;
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    // Split the input string.
                    // First, try fixed-format.
                    String[] split;
                    //if (i == 0)
                    //split = ReadUtil.fixedFormatSplit(s, new int[] { 10, 3, 10, 4 });
                    //else
                    split = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // ReadUtil.freeFormatSplit(s);

                    // Get the elements from the split.
                    perlen = Convert.ToDouble(split[0]); //1.0d;   //Float.parseFloat(split[0]);
                    nstp = Convert.ToInt32(split[1]); // 1;  //Integer.parseInt(split[1]);
                    tsmult = Convert.ToDouble(split[2]); // 1.0d;  // Float.parseFloat(split[2]);
                    strSstr = split[3].ToUpper();
                    sstr = SsTr.TR; //SsTr.parse(split[3]);
                    if (strSstr == "SS")
                    {
                        sstr = SsTr.SS;
                    }

                    // If good values were obtained (only have to check sstr, because the 
                    // others would have thrown exceptions), return a stress period object. 
                    if (sstr != SsTr.None)
                    {
                        return new StressPeriod(perlen, nstp, tsmult, sstr);
                    }
                }
                catch
                {
                    Console.WriteLine("unable to parse stress period");
                }
            }

            // Both failed. Return null.
            return null;
        }
        public TimeSpan getTimeSpan()
        {
            return timeSpan;
        }

        public void setTimeSpan(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        public void setTimeSpan(ModflowTimeUnit modflowTimeUnit)
        {
            this.timeSpan = TimeHelper.GetTimeSpan(perlen, modflowTimeUnit);
        }

        public enum SsTr
        {
            SS,
            TR,
            None
        }
        public double getPerlen()
        {
            return perlen;
        }
    }
}
