using System;
using System.Collections.Generic;
using System.Text;

namespace USGS.Puma.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class PumaGlobal
    {
        #region Structures
        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public struct ArrayComparisonStats
        {
            /// <summary>
            /// 
            /// </summary>
            private int _TotalCount;
            /// <summary>
            /// Gets or sets the total count.
            /// </summary>
            /// <value>The total count.</value>
            /// <remarks></remarks>
            public int TotalCount
            {
                get { return _TotalCount; }
                set { _TotalCount = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            private int _PositiveCount;
            /// <summary>
            /// Gets or sets the positive count.
            /// </summary>
            /// <value>The positive count.</value>
            /// <remarks></remarks>
            public int PositiveCount
            {
                get { return _PositiveCount; }
                set { _PositiveCount = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            private int _NegativeCount;
            /// <summary>
            /// Gets or sets the negative count.
            /// </summary>
            /// <value>The negative count.</value>
            /// <remarks></remarks>
            public int NegativeCount
            {
                get { return _NegativeCount; }
                set { _NegativeCount = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            private int _NoDataCount;
            /// <summary>
            /// Gets or sets the no data count.
            /// </summary>
            /// <value>The no data count.</value>
            /// <remarks></remarks>
            public int NoDataCount
            {
                get { return _NoDataCount; }
                set { _NoDataCount = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            private int _UndefinedCount;
            /// <summary>
            /// Gets or sets the undefined count.
            /// </summary>
            /// <value>The undefined count.</value>
            /// <remarks></remarks>
            public int UndefinedCount
            {
                get { return _UndefinedCount; }
                set { _UndefinedCount = value; }
            }
        }
        #endregion

        #region Enumerations
        #endregion


    }
}
