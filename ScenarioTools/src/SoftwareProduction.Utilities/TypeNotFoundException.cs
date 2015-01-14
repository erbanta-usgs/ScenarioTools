using System;
using System.Runtime.Serialization;

namespace SoftwareProductions.Utilities
{

	/// <summary>
	/// This Exception is thrown when the XmlSerializer cannot find a type
	/// that is required during deserialisation.
	/// </summary>
	[Serializable]
	public class TypeNotFoundException : Exception
	{
		internal const string MessageText = "The type {0} could not be found.";

		/// <summary>
		/// Default Constructor
		/// </summary>
		public TypeNotFoundException()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">The message</param>
		public TypeNotFoundException(string message) : base(message)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">The message</param>
		/// <param name="innerException">The Inner Exception</param>
		public TypeNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Serialisation Constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected TypeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

	}
}
