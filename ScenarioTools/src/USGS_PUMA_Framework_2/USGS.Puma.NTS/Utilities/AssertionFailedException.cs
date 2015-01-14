using System;
using System.Collections;
using System.Text;

namespace USGS.Puma.NTS.Utilities
{
   /// <summary>
   /// 
   /// </summary>
    public class AssertionFailedException : ApplicationException 
    {
        /// <summary>
        /// 
        /// </summary>
        public AssertionFailedException() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public AssertionFailedException(string message) : base(message) { }
    }
}
