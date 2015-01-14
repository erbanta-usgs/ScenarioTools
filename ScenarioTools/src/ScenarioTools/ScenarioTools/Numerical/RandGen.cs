using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ScenarioTools.Numerical
{
    public class RandGen
    {
        private static RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();

        public static long NextUID()
        {
            // Remove a time-dependent number of values.
            int cycle = DateTime.Now.Millisecond % 50;
            byte[] discard = new byte[cycle];
            rng.GetBytes(discard);

            // Produce and return a long int value.
            byte[] buffer = new byte[sizeof(long)];
            rng.GetBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}