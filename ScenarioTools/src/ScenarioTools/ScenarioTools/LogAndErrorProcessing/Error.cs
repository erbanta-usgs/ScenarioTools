using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.LogAndErrorProcessing
{
    public class Error
    {
        public static void Report(string error)
        {
            Console.WriteLine("ERROR: " + error);
        }
    }
}
