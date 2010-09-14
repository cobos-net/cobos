using System;
using System.Collections.Generic;
using System.Linq;

namespace Intergraph.Oz.Utilities.Extensions
{
	public static class ObjectExtensions
	{
		/// <summary>
		/// extend the object class to support casting to an anonymous type
		/// </summary>
		/// <typeparam name="T">The anonymous type to cast to</typeparam>
		/// <param name="obj">The 'this' object reference</param>
		/// <param name="example">An instance of an anonymous type</param>
		/// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
		public static T CastByExample<T>( this object obj, T example )
		{
			try
			{
				return (T)obj;
			}
			catch ( InvalidCastException )
			{
				return default( T );
			}
		}
	}
}
