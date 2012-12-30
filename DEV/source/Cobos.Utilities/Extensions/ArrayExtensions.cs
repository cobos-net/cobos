// ============================================================================
// Filename: ArrayExtensions.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
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
using System.Text;

namespace Cobos.Utilities.Extensions
{
	public static class ArrayExtensions
	{
		/// <summary>
		/// Appends the contents of a2 into a1.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns>The new a1 array with the appended result</returns>
		public static T[] Append<T>( this T[] a1, T[] a2 )
		{
			return a1.Concat( a2 ).Cast<T>().ToArray();
		}

		/// <summary>
		/// Appends the object obj into a1.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns>The new a1 array with the appended result</returns>
		public static T[] Append<T>( this T[] a1, T obj )
		{
			return a1.Concat( new T[] { obj } ).Cast<T>().ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="multi"></param>
		/// <returns></returns>
		public static T[] ConcatenateAll<T>( T[][] multi )
		{
			int totalLength = 0;

			for ( int m = 0; m < multi.Length; ++m )
			{
				T[] current = multi[ m ];

				if ( current != null )
				{
					totalLength += current.Length;
				}
			}

			if ( totalLength == 0 )
			{
				return null;
			}

			T[] result = new T[ totalLength ];

			int index = 0;

			for ( int m = 0; m < multi.Length; ++m )
			{
				T[] current = multi[ m ]; 

				if ( current != null )
				{
					int currentLength = current.Length;

					Array.Copy( current, 0, result, index, currentLength );
					
					index += currentLength;
				}
			}

			return result;
		}

		/// <summary>
		/// Find the index of the specified object in the array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static int IndexOf<T>( this T[] a, T obj )
		{
			for ( int i = 0; i < a.Length; ++i )
			{
				if ( Object.ReferenceEquals( a[ i ], obj ) )
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Returns a one-level deep copy of a portion of an array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a">The array to extract the values from</param>
		/// <param name="begin">Zero-based index at which to begin extraction.
		/// As a negative index, start indicates an offset from the end of the sequence. slice(-2) extracts the second-to-last element and the last element in the sequence.</param>
		/// <param name="end">Zero-based index at which to end extraction. slice extracts up to but not including end.
		/// As a negative index, end indicates an offset from the end of the sequence. slice(2,-1) extracts the third element through the second-to-last element in the sequence.
		/// If end is null, slice extracts to the end of the sequence.</param>
		/// <returns>Returns a one-level deep copy of a portion of an array.</returns>
		public static T[] Slice<T>( this T[] a, int begin, int? end )
		{
			if ( begin >= a.Length )
			{
				throw new System.ArgumentOutOfRangeException();
			}

			// normalise begin
			if ( begin < 0 )
			{
				begin = a.Length + begin;

				if ( begin < 0 )
				{
					begin = 0;
				}
			}

			// normalise end
			if ( end == null )
			{
				end = a.Length;
			}
			else if ( end < 0 )
			{
				end = a.Length + end;

				if ( end < 0 )
				{
					end = 0;
				}
			}
			else if ( end > a.Length )
			{
				end = a.Length;
			}

			if ( end <= begin )
			{
				return null; // nothing to copy
			}

			// create the copy
			T[] copy = new T[ (int)end - begin ];

			System.Array.Copy( a, begin, copy, 0, (int)end - begin );

			return copy;
		}

		/// <summary>
		/// Changes the content of an array, adding new elements while removing old elements.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a">The array to modify</param>
		/// <param name="index">Index at which to start changing the array. If negative, will begin that many elements from the end.</param>
		/// <param name="howMany">An integer indicating the number of old array elements to remove. 
		/// If howMany is 0, no elements are removed. In this case, you should specify at least one new element.</param>
		/// <param name="elements">The elements to add to the array. If you don't specify any elements, splice simply removes elements from the array.</param>
		/// <returns>The modified array a</returns>
		public static T[] Splice<T>( this T[] a, int index, int howMany, params T[] elements )
		{
			if ( index >= a.Length )
			{
				throw new System.ArgumentOutOfRangeException();
			}

			// normalise howMany
			if ( howMany < 0 )
			{
				howMany = 0;
			}

			if ( howMany == 0 && elements.Length == 0 )
			{
				return a;
			}

			// normalise index
			if ( index < 0 )
			{
				index = a.Length + index;

				if ( index < 0 )
				{
					index = 0;
				}
			}

			// calculate the start and end indices for removal
			int removeStart = index, removeEnd = index + howMany;

			if ( removeEnd > a.Length )
			{
				removeEnd = a.Length - index;
			}

			// calculate the new size of the array
			int newSize = a.Length - (removeEnd - removeStart) + elements.Length;

			if ( newSize == 0 )
			{
				return null;
			}

			T[] newArray = new T[ newSize ];

			if ( removeStart > 0 )
			{
				System.Array.Copy( a, 0, newArray, 0, removeStart );
			}

			if ( elements.Length > 0 )
			{
				System.Array.Copy( elements, 0, newArray, removeStart, elements.Length );
			}

			if ( a.Length - removeEnd > 0 )
			{
				System.Array.Copy( a, removeEnd, newArray, removeStart + elements.Length, a.Length - removeEnd );
			}

			return newArray;
		}

	}
}
