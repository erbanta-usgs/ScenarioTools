using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.LogAndErrorProcessing
{
    public static class Logging
    {
        public const int ERROR_00_HIGHEST_PRIORITY = 0;
        public const int ERROR_10_FATAL_ERROR = 10;

        public const int ERROR_50_SERIOUS_ERROR_NO_IMMEDIATE_DATA_LOSS = 50;
        public const int ERROR_90_INFORMATION_ONLY = 90;
        public const int ERROR_99_LOWEST_PRIORITY = 99;

        private static List<ILogger> registeredLoggers = new List<ILogger>();

        public static void RegisterLogger(ILogger logger)
        {
            registeredLoggers.Add(logger);
        }
        public static void Update(string message, int priority)
        {
            foreach (ILogger logger in registeredLoggers)
            {
                logger.Update(message, priority);
            }
            Console.WriteLine("ERROR: " + message);
        }
    }
}
