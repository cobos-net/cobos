// ============================================================================
// Filename: MultiPolygon.cs
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
using System.Linq;

namespace Cobos.Geometry
{
    /// <summary>
    /// A MultiPolygon is a MultiSurface whose elements are Polygons.
    /// 
    /// The assertions for MultiPolygons are:
    /// 
    /// 1. The interiors of 2 Polygons that are elements of a MultiPolygon may 
    ///    not intersect.
    /// 
    /// 2. The Boundaries of any 2 Polygons that are elements of a MultiPolygon 
    ///    may not ‘cross’ and may touch at only a finite number of points. 
    ///    (Note that crossing is prevented by assertion 1 above).
    /// 
    /// 3. A MultiPolygon is defined as topologically closed.
    /// 
    /// 4. A MultiPolygon may not have cut lines, spikes or punctures, a 
    ///    MultiPolygon is a Regular, Closed point set.
    ///    
    /// 5. The interior of a MultiPolygon with more than 1 Polygon is not 
    ///    connected, the number of connected components of the interior of a 
    ///    MultiPolygon is equal to the number of Polygons in the MultiPolygon.
    /// </summary>
	public class MultiPolygon : MultiSurface
	{
		#region Construction

		public MultiPolygon()
			: base()
		{
		}

		public MultiPolygon( int capacity )
			: base( capacity )
		{
		}

		public MultiPolygon( IEnumerable<Geometry> geometries )
			: base( geometries.Count() )
		{
			AssertValid( geometries );

			base.Add( geometries );
		}

		#endregion

		#region OpenGIS implementation

		#region Geometry

        /// <summary>
        /// Exports this Geometry to a specific well-known binary representation of Geometry.
        /// </summary>
        /// <returns>WKB representation</returns>
        public override byte[] AsBinary()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exports this Geometry to a specific well-known text representation of Geometry.
        /// </summary>
        /// <returns>WKT representation.</returns>
        public override string AsText()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the name of the instantiable subtype of Geometry of which this
        /// Geometry instance is a member. The name of the instantiable subtype of 
        /// Geometry is returned as a string.
        /// </summary>
        public override string GeometryType
		{
			get { return "MULTIPOLYGON"; }
		}

        #endregion

		#region MultiSurface

        /// <summary>
        /// The area of this MultiSurface, as measured in the spatial reference 
        /// system of this MultiSurface.
        /// </summary>
        public override double Area
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>
        /// The mathematical centroid for this MultiSurface. The result is not 
        /// guaranteed to be on this MultiSurface.
        /// </summary>
        public override Coord Centroid
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>
        /// A Point guaranteed to be on this MultiSurface.
        /// </summary>
        public override Coord PointOnSurface
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#endregion

		#region Collection validation

        /// <summary>
        /// Assert that the geometry about to be added is a Polygon.
        /// </summary>
        /// <param name="geometry">The geometry to check.</param>
        protected override void AssertValid( Geometry geometry )
		{
			if ( !(geometry is Polygon) )
			{
				throw new ArgumentException( "MultiPolygon cannot contain an object of type: " + geometry.GeometryType );
			}
		}

        /// <summary>
        /// Assert that the geometries about to be added are Polygons.
        /// </summary>
        /// <param name="geometry">The geometries to check.</param>
        protected override void AssertValid( IEnumerable<Geometry> geometries )
		{
			if ( geometries != null )
			{
				foreach ( Geometry geometry in geometries )
				{
					if ( !(geometry is Polygon) )
					{
						throw new ArgumentException( "MultiPolygon cannot contain an object of type: " + geometry.GeometryType );
					}
				}
			}
		}

		#endregion

	}
}
