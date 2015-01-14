using System;
using System.Data;

namespace SoftwareProductions.Utilities
{
	/// <summary>
	/// TypeChecker is a utility class that helps us work out if a given type is numeric
	/// and if a given type is a logical primitive or a compound type.
	/// </summary>
	public sealed class TypeChecker
	{

		//Private constructor
		private TypeChecker()
		{
			
		}

		/// <summary>
		/// Works out which types to treat as attibutes and which the treat as child objects.
		/// </summary>
		/// <param name="type">The Type to check.</param>
		/// <returns>true if the Type is atomic (e.g. string, date, enum or number), false if it is a compound sub-object.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <i>type</i> is null (Nothing in Visual Basic).</exception>
		public static bool TypeIsAtomic(Type type) 
		{
			ExceptionHelper.ExceptionIfNull(type, "type");

			if (type == typeof(string)) 
			{
				return true;				
			}
			else if (TypeIsNumeric(type)) 
			{
				return true;				
			}
			else if (type == typeof(bool)) 
			{
				return true;				
			}
			else if (type == typeof(DateTime)) 
			{
				return true;				
			}
			else if (type == typeof(TimeSpan)) 
			{
				return true;				
			}
			else if (type == typeof(char)) 
			{
				return true;				
			}
			else if (type == typeof(byte)) 
			{
				return true;				
			}
			else if (type.IsSubclassOf(typeof(Enum))) 
			{
				return true;				
			}
			else if (type == typeof(Guid)) 
			{
				return true;
			}	

			return false;
		}

		/// <summary>
		/// Returns true if the specified type is one of the numeric types
		/// (Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Decimal)
		/// </summary>
		/// <param name="type">The Type to check.</param>
		/// <returns>
		/// true if the specified type is one of the numeric types
		/// (Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Decimal)
		/// </returns>
		public static bool TypeIsNumeric(Type type) 
		{
			if (type == typeof(Int16)) 
			{
				return true;				
			}
			else if (type == typeof(Int32)) 
			{
				return true;				
			}
			else if (type == typeof(Int64)) 
			{
				return true;				
			}
			else if (type == typeof(float)) 
			{
				return true;				
			}
			else if (type == typeof(double)) 
			{
				return true;				
			}
			else if (type == typeof(decimal)) 
			{
				return true;				
			}
			else if (type == typeof(UInt16)) 
			{
				return true;				
			}
			else if (type == typeof(UInt32)) 
			{
				return true;				
			}
			else if (type == typeof(UInt64)) 
			{
				return true;				
			}

			return false;
		}

		/// <summary>
		/// Returns true if the specified type is a collection type, i.e. if it
		/// Implements ICollection. This includes IList and IDictionary
		/// </summary>
		/// <param name="type">The Type to Check.</param>
		/// <returns>true if the Type implements ICollection, false otherwise.</returns>
		public static bool TypeIsCollection(Type type)
		{
			//if (type != null && (type.GetInterface("ICollection") != null || type == typeof(DataTable)))
			if (type != null && type.GetInterface("ICollection") != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


	}
}
