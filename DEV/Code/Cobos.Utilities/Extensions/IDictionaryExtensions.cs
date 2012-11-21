// ============================================================================
// Filename: IDictionaryExtensions.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cobos.Utilities.Extensions
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
