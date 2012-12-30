// ============================================================================
// Filename: MultiSurface.cs
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
    /// A MultiSurface is a two-dimensional geometric collection whose elements 
    /// are surfaces. The interiors of any two surfaces in a MultiSurface may 
    /// not intersect. The boundaries of any two elements in a MultiSurface may 
    /// intersect at most at a finite number of points.
    /// 
    /// MultiSurface is a non-instantiable class in this specification, it 
    /// defines a set of methods for its subclasses
    /// and is included for reasons of extensibility. The instantiable subclass 
    /// of MultiSurface is MultiPolygon, corresponding to a collection of 
    /// Polygons.
    /// </summary>
	public abstract class MultiSurface : GeometryCollection
	{
		#region Construction

        /// <summary>
        /// Create an empty surface set.
        /// </summary>
        protected MultiSurface()
			: base()
		{
		}

        /// <summary>
        /// Create an empty surface set with reserved capacity.
        /// </summary>
        /// <param name="capacity">The capacity to reserve.</param>
		protected MultiSurface( int capacity )
			: base( capacity )
		{
		}

        /// <summary>
        /// Create a curve set containing the geometries.
        /// </summary>
        /// <param name="geometries">The geometries to add to the set.</param>
        protected MultiSurface( IEnumerable<Geometry> geometries )
			: base( geometries )
		{
		}

		#endregion

		#region OpenGIS specification

        /// <summary>
        /// The area of this MultiSurface, as measured in the spatial reference 
        /// system of this MultiSurface.
        /// </summary>
		public abstract double Area
		{
			get;
		}

        /// <summary>
        /// The mathematical centroid for this MultiSurface. The result is not 
        /// guaranteed to be on this MultiSurface.
        /// </summary>
		public abstract Coord Centroid
		{
			get;
		}

        /// <summary>
        /// A Point guaranteed to be on this MultiSurface.
        /// </summary>
		public abstract Coord PointOnSurface
		{
			get;
		}

		#endregion
	}
}
