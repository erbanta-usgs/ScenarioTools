using System;
using System.Globalization;

namespace SoftwareProductions.Utilities
{
	/// <summary>
	/// Converter is a wrapper for some of the the System.Convert functions that 
	/// include the CultureInfo.CurrentCulture. 
	/// </summary>
	public sealed class Converter
	{
		private Converter()
		{
		}

		/// <summary>
		/// Converts the value of the specified Object to a Decimal number.
		/// </summary>
		/// <param name="value">An Object that implements the IConvertible interface or a null reference (Nothing in Visual Basic).</param>
		/// <returns>A Decimal number equivalent to the value of value, or zero if value is a null reference (Nothing in Visual Basic).</returns>
		public static decimal ToNumber(object value) {
			return Convert.ToDecimal(value, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Converts the value of the specified Object to a Double number.
		/// </summary>
		/// <param name="value">An Object that implements the IConvertible interface or a null reference (Nothing in Visual Basic).</param>
		/// <returns>A Decimal number equivalent to the value of value, or zero if value is a null reference (Nothing in Visual Basic).</returns>
		public static double ToDouble(object value) 
		{
			return Convert.ToDouble(value, CultureInfo.CurrentCulture);
		}
	}
}
