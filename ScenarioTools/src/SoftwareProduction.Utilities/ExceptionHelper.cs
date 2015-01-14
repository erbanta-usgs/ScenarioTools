using System;

namespace SoftwareProductions.Utilities
{
	/// <summary>
	/// Contains methods to check for invalid data and throw exceptions.
	/// </summary>
	public sealed class ExceptionHelper
	{
		private ExceptionHelper()
		{
			
		}
	
		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if the specified value is null (Nothing in Visual Basic).
		/// </summary>
		/// <param name="value">The value of the parameter to check for null.</param>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <exception cref="ArgumentNullException">Thrown if the specified value is null (Nothing in Visual Basic).</exception>
		public static void ExceptionIfNull(object value, string parameterName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

	}
}
