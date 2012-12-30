// ============================================================================
// Filename: Geometry.cs
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
    /// Geometry is the root class of the hierarchy. Geometry is an abstract 
    /// (non-instantiable) class.
    /// 
    /// The instantiable subclasses of Geometry defined in this specification 
    /// are restricted to 0, 1 and two dimensional geometric objects that exist 
    /// in two-dimensional coordinate space (R2).
    /// 
    /// All instantiable geometry classes described in this specification are 
    /// defined so that valid instances of a geometry class are topologically 
    /// closed (i.e. all defined geometries include their boundary).
    /// </summary>
	public abstract class Geometry
	{
        #region Public enumerations

        /// <summary>
        /// A geometry can have a dimension of –1, 0, 1, or 2:
        /// –1 for an empty geometry.
        /// 0 for a geometry with no length and no area.
        /// 1 for a geometry with non-zero length and zero area.
        /// 2 for a geometry with non-zero area.
        /// </summary>
        public enum DimensionType
        {
            Empty = -1,
            Dim0d,
            Dim1d,
            Dim2d
        }

        /// <summary>
        /// Well-known Binary Representation for Geometry.
        /// </summary>
        public enum WKBGeometryType
        {
            Point = 1,
            LineString = 2,
            Polygon = 3,
            MultiPoint = 4,
            MultiLineString = 5,
            MultiPolygon = 6,
            GeometryCollection = 7
        };

        /// <summary>
        /// Well-known Binary Representation byte order.
        /// </summary>
        public enum WKBByteOrder
        {
            XDR = 0, // Big Endian
            NDR = 1 // Little Endian
        };

        #endregion

        #region OpenGIS specification

        /// <summary>
        /// Exports this Geometry to a specific well-known binary representation of Geometry.
        /// </summary>
        /// <returns>WKB representation</returns>
        public abstract byte[] AsBinary();

        /// <summary>
        /// Exports this Geometry to a specific well-known text representation of Geometry.
        /// </summary>
        /// <returns>WKT representation.</returns>
        public abstract string AsText();

		/// <summary>
        /// Returns the closure of the combinatorial boundary of this Geometry.
		/// </summary>
		public abstract Geometry Boundary
		{
			get;
		}

        /// <summary>
        /// The inherent dimension of this Geometry object, which must be less 
        /// than or equal to the coordinate dimension. This specification is 
        /// restricted to geometries in two-dimensional coordinate space.
        /// </summary>
        public abstract DimensionType Dimension
        {
            get;
        }

        /// <summary>
        /// The minimum bounding box for this Geometry, returned as a Geometry. 
        /// The polygon is defined by the corner points of the bounding box 
        /// ((MINX, MINY), (MAXX, MINY), (MAXX, MAXY), (MINX, MAXY), (MINX, MINY))
        /// </summary>
        public abstract Geometry Envelope
        {
            get;
        }

		/// <summary>
		/// Returns the name of the instantiable subtype of Geometry of which this
        /// Geometry instance is a member. The name of the instantiable subtype of 
        /// Geometry is returned as a string.
		/// </summary>
		public abstract string GeometryType
		{
			get;
		}

        /// <summary>
        /// Returns true if this Geometry is the empty geometry. If true, then 
        /// this Geometry represents the empty point set, Ø, for the coordinate 
        /// space.
        /// </summary>
        public abstract bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Returns true if this Geometry has no anomalous geometric points, 
        /// such as self intersection or self tangency. The description of each 
        /// instantiable geometric class will include the specific conditions 
        /// that cause an instance of that class to be classified as not simple.
        /// </summary>
        public abstract bool IsSimple
        {
            get;
        }

        /// <summary>
        /// Returns the Spatial Reference System ID for this Geometry.		
        /// </summary>
        public int SRID
        {
            get;
            set;
        }

        #endregion

        #region Spatial Relations

        /// <summary>
        /// Test if this Geometry is ‘spatially equal’ to anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry is ‘spatially equal’ to anotherGeometry.</returns>
        public abstract bool Equals( Geometry anotherGeometry );

        /// <summary>
        /// Test if this Geometry is ‘spatially disjoint’ from anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry is ‘spatially disjoint’ from anotherGeometry.</returns>
        public abstract bool Disjoint( Geometry anotherGeometry );

        /// <summary>
        /// Test if this Geometry ‘spatially intersects’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially intersects’ anotherGeometry.</returns>
        public abstract bool Intersects( Geometry anotherGeometry );

        /// <summary>
        /// Test if this Geometry ‘spatially touches’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially touches’ anotherGeometry.</returns>
        public abstract bool Touches( Geometry anotherGeometry );

        /// <summary>
        /// Test if this Geometry ‘spatially crosses’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially crosses’ anotherGeometry.</returns>
        public abstract bool Crosses( Geometry anotherGeometry );

        /// <summary>
        /// Test if this Geometry is ‘spatially within’ from anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry is ‘spatially within’ from anotherGeometry.</returns>
        public abstract bool Within( Geometry anotherGeometry );

        /// <summary>
        /// Test if this Geometry ‘spatially contains’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially contains’ anotherGeometry.</returns>
        public abstract bool Contains( Geometry anotherGeometry );

        /// <summary>
        /// Test if this Geometry ‘spatially overlaps’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially overlaps’ anotherGeometry.</returns>
        public abstract bool Overlaps( Geometry anotherGeometry );

        /// <summary>
        /// if this Geometry is spatially related to anotherGeometry, by 
        /// testing for intersections between the Interior, Boundary and 
        /// Exterior of the two geometries as specified by the values in 
        /// the intersectionPatternMatrix.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <param name="intersectionPatternMatrix">The intersections to test.</param>
        /// <returns>True if this Geometry is spatially related to anotherGeometry.</returns>
        public abstract bool Relate( Geometry anotherGeometry, string intersectionPatternMatrix );

        #endregion

        #region Spatial Analysis

        /// <summary>
        /// Returns the shortest distance between any two points in the two 
        /// geometries as calculated in the spatial reference system of this 
        /// Geometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>The shortest distance</returns>
        public abstract double Distance( Geometry anotherGeometry );

        /// <summary>
        /// Returns a geometry that represents all points whose distance from 
        /// this Geometry is less than or equal to distance.
        /// </summary>
        /// <param name="distance">The buffer distance.</param>
        /// <returns>The geometry buffer.</returns>
        public abstract Geometry Buffer( double distance );

        /// <summary>
        /// Returns a geometry that represents the convex hull of this Geometry.
        /// </summary>
        /// <returns>A geometry that represents the convex hull of this Geometry.</returns>
        public abstract Geometry ConvexHull();

        /// <summary>
        /// Returns a geometry that represents the point set intersection of 
        /// this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>Point set intersection.</returns>
        public abstract Geometry Intersection( Geometry anotherGeometry );

        /// <summary>
        /// Returns a geometry that represents the point set union of 
        /// this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>Point set union.</returns>
        public abstract Geometry Union( Geometry anotherGeometry );

        /// <summary>
        /// Returns a geometry that represents the point set difference of 
        /// this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>Point set difference.</returns>
        public abstract Geometry Difference( Geometry anotherGeometry );

        /// <summary>
        /// Returns a geometry that represents the point set symmetric difference of 
        /// this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>Point set symmetric difference.</returns>
        public abstract Geometry SymDifference( Geometry anotherGeometry );

        #endregion

        #region Extensions

        /// <summary>
		/// Non standard OpenGIS extension.  More efficient method of querying 
        /// the minimum bounding rectangle than provided by the Envelope property.
		/// 
		/// Can use Geometry.BoundingBox.IsInBounds( point ) as a trivial accept/reject 
        /// for locating geometries.
		/// </summary>
		public abstract BoundingBox BoundingBox
		{
			get;
        }

        #endregion
    }
}
