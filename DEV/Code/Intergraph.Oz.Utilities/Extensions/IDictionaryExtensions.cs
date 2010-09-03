using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intergraph.Oz.Utilities.Extensions
{
	public static class IDictionaryExtensions
	{
		/// <summary>
		/// Extend the dictionary class to support casting to an anonymous type
		/// </summary>
		/// <typeparam name="T">The anonymous type to cast to</typeparam>
		/// <param name="dict">The 'this' dictionary reference</param>
		/// <param name="example">An instance of an anonymous type</param>
		/// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
		public static T CastByExample<T>( this IDictionary<string, object> dict, T example )
		{
			// get the sole constructor
			var ctor = example.GetType().GetConstructors().Single();

			// conveniently named constructor parameters make this all possible...
			var args = from p in ctor.GetParameters()
						  let val = dict.GetValueOrDefault( p.Name )
						  select val != null && p.ParameterType.IsAssignableFrom( val.GetType() ) ? (object)val : null;

			return (T)ctor.Invoke( args.ToArray() );
		}

		/// <summary>
		/// Helper method to extract values from a dictionary to support the CastByExample method
		/// </summary>
		/// <typeparam name="TKey">The key type</typeparam>
		/// <typeparam name="TValue">The value type</typeparam>
		/// <param name="dict">The 'this' dictionary reference</param>
		/// <param name="key">The key of the value</param>
		/// <returns></returns>
		public static TValue GetValueOrDefault<TKey, TValue>( this IDictionary<TKey, TValue> dict, TKey key )
		{
			TValue result;
			dict.TryGetValue( key, out result );
			return result;
		}
	}
}
