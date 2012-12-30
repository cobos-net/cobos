// ============================================================================
// Filename: Surface.cs
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

namespace Cobos.Geometry
{
    /// <summary>
    /// A Surface is a two-dimensional geometric object.
    /// 
    /// The OpenGIS Abstract Specification defines a simple Surface as 
    /// consisting of a single ‘patch’ that is associated with one 
    /// ‘exterior boundary’ and 0 or more ‘interior’ boundaries.
    /// </summary>
	public abstract class Surface : Geometry
	{
		#region OpenGIS specification

        /// <summary>
        /// The area of this Surface, as measured in the spatial reference 
        /// system of this Surface.
        /// </summary>
		public abstract double Area
		{
			get;
		}

        /// <summary>
        /// The mathematical centroid for this Surface as a Point. 
        /// The result is not guaranteed to be on this Surface.
        /// </summary>
		public abstract Coord Centroid
		{
			get;
		}

        /// <summary>
        /// A point guaranteed to be on this Surface.
        /// </summary>
		public abstract Coord PointOnSurface
		{
			get;
		}

		#endregion
	}
}
