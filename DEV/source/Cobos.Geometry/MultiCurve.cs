// ============================================================================
// Filename: MultiCurve.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 21-Nov-09
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
using System.Text;

namespace Cobos.Geometry
{
    /// <summary>
    /// A MultiCurve is a one-dimensional GeometryCollection whose elements are 
    /// Curves.
    /// 
    /// MultiCurve is a non-instantiable class in this specification, it defines 
    /// a set of methods for its subclasses and is included for reasons of 
    /// extensibility.
    /// 
    /// A MultiCurve is simple if and only if all of its elements are simple, the 
    /// only intersections between any two elements occur at points that are on 
    /// the boundaries of both elements.
    /// </summary>
	public abstract class MultiCurve : GeometryCollection
	{
		#region Construction

        /// <summary>
        /// Create an empty curve set.
        /// </summary>
		protected MultiCurve()
			: base()
		{
		}

        /// <summary>
        /// Create an empty curve set with reserved capacity.
        /// </summary>
        /// <param name="capacity">The capacity to reserve.</param>
		protected MultiCurve( int capacity )
			: base( capacity )
		{
		}

        /// <summary>
        /// Create a curve set containing the geometries.
        /// </summary>
        /// <param name="geometries">The geometries to add to the set.</param>
		protected MultiCurve( IEnumerable<Geometry> geometries )
			: base( geometries )
		{
		}

		#endregion

		#region OpenGIS implementation

        /// <summary>
        /// Returns true if this MultiCurve is closed (StartPoint () = EndPoint () 
        /// for each curve in this MultiCurve)
        /// </summary>
		public abstract bool IsClosed
		{
			get;
		}

        /// <summary>
        /// The Length of this MultiCurve which is equal to the sum of the 
        /// lengths of the element Curves.
        /// </summary>
		public abstract double Length
		{
			get;
		}

		#endregion

	}
}
