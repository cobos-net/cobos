// ============================================================================
// Filename: Point.cs
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
	public class Point : Geometry
	{
		#region Construction

        /// <summary>
        /// Construct an "unlocated" Point.
        /// </summary>
		public Point()
		{
			Position.X = double.NaN;
			Position.Y = double.NaN;
		}

		/// <summary>
		/// Create a point at the specified point.
		/// </summary>
		/// <param name="x">The X coordinate value.</param>
		/// <param name="y">The Y coordinate value.</param>
		public Point( double x, double y )
		{
			Position.X = x;
			Position.Y = y;
		}

		/// <summary>
		/// Create a Point at the specified position.
		/// </summary>
		/// <param name="coord">The position to create the Point.</param>
		public Point( Coord coord )
		{
			Position = coord;
		}

		#endregion

		#region OpenGIS specification

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
        /// Returns the closure of the combinatorial boundary of this Geometry.
        /// </summary>
        public override Geometry Boundary
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// The inherent dimension of this Geometry object, which must be less 
        /// than or equal to the coordinate dimension. This specification is 
        /// restricted to geometries in two-dimensional coordinate space.
        /// </summary>
        public override DimensionType Dimension
		{
			get 
			{
				return Position.X == double.NaN || Position.Y == double.NaN ? DimensionType.Empty : DimensionType.Dim0d;
			}
		}

        /// <summary>
        /// The minimum bounding box for this Geometry, returned as a Geometry. 
        /// The polygon is defined by the corner points of the bounding box 
        /// ((MINX, MINY), (MAXX, MINY), (MAXX, MAXY), (MINX, MAXY), (MINX, MINY))
        /// </summary>
        public override Geometry Envelope
        {
            get 
            {
                LineString exterior = new LineString( new Coord[] { 
                                                            new Coord( Position.X, Position.Y ),
                                                            new Coord( Position.X, Position.Y ),
                                                            new Coord( Position.X, Position.Y ),
                                                            new Coord( Position.X, Position.Y ),
                                                            new Coord( Position.X, Position.Y ) } );

                return new Polygon( exterior, false );
            }
        }


        /// <summary>
        /// Returns the name of the instantiable subtype of Geometry of which this
        /// Geometry instance is a member. The name of the instantiable subtype of 
        /// Geometry is returned as a string.
        /// </summary>
        public override string GeometryType
        {
            get { return "POINT"; }
        }

        /// <summary>
        /// Returns true if this Geometry is the empty geometry. If true, then 
        /// this Geometry represents the empty point set, Ø, for the coordinate 
        /// space.
        /// </summary>
        public override bool IsEmpty
        {
            get { return true; }
        }

        /// <summary>
        /// Returns true if this Geometry has no anomalous geometric points, 
        /// such as self intersection or self tangency. The description of each 
        /// instantiable geometric class will include the specific conditions 
        /// that cause an instance of that class to be classified as not simple.
        /// </summary>
        public override bool IsSimple
		{
			get { return true; }
		}

		#endregion

        #region Point

        /// <summary>
        /// The x-coordinate value for this Point.
        /// </summary>
        double X
        {
            get
            {
                return Position.X;
            }
        }

        /// <summary>
        /// The y-coordinate value for this Point.
        /// </summary>
        double Y
        {
            get
            {
                return Position.Y;
            }
        }

        #endregion

        #endregion

        #region Extensions

        /// <summary>
        /// Non standard OpenGIS extension.  More efficient method of querying 
        /// the minimum bounding rectangle than provided by the Envelope property.
        /// 
        /// Can use Geometry.BoundingBox.IsInBounds( point ) as a trivial accept/reject 
        /// for locating geometries.
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get
            {
                return Position.X == double.NaN || Position.Y == double.NaN ? new BoundingBox( true ) : new BoundingBox( new Coord( Position.X, Position.Y ), new Coord( Position.X, Position.Y ) );
            }
        }

        #endregion

        #region Spatial Relations

        /// <summary>
        /// Test if this Geometry is ‘spatially equal’ to anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry is ‘spatially equal’ to anotherGeometry.</returns>
        public override bool Equals( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Test if this Geometry is ‘spatially disjoint’ from anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry is ‘spatially disjoint’ from anotherGeometry.</returns>
        public override bool Disjoint( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Test if this Geometry ‘spatially intersects’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially intersects’ anotherGeometry.</returns>
        public override bool Intersects( Geometry anotherGeometry )
        {
            return !anotherGeometry.Disjoint( this );
        }

        /// <summary>
        /// Test if this Geometry ‘spatially touches’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially touches’ anotherGeometry.</returns>
        public override bool Touches( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Test if this Geometry ‘spatially crosses’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially crosses’ anotherGeometry.</returns>
        public override bool Crosses( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Test if this Geometry is ‘spatially within’ from anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry is ‘spatially within’ from anotherGeometry.</returns>
        public override bool Within( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Test if this Geometry ‘spatially contains’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially contains’ anotherGeometry.</returns>
        public override bool Contains( Geometry anotherGeometry )
        {
            return anotherGeometry.Within( this );
        }

        /// <summary>
        /// Test if this Geometry ‘spatially overlaps’ anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>True if this Geometry ‘spatially overlaps’ anotherGeometry.</returns>
        public override bool Overlaps( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// if this Geometry is spatially related to anotherGeometry, by 
        /// testing for intersections between the Interior, Boundary and 
        /// Exterior of the two geometries as specified by the values in 
        /// the intersectionPatternMatrix.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <param name="intersectionPatternMatrix">The intersections to test.</param>
        /// <returns>True if this Geometry is spatially related to anotherGeometry.</returns>
        public override bool Relate( Geometry anotherGeometry, string intersectionPatternMatrix )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Spatial Analysis

        /// <summary>
        /// Returns the shortest distance between any two points in the two 
        /// geometries as calculated in the spatial reference system of this 
        /// Geometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>The shortest distance</returns>
        public override double Distance( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents all points whose distance from 
        /// this Geometry is less than or equal to distance.
        /// </summary>
        /// <param name="distance">The buffer distance.</param>
        /// <returns>The geometry buffer.</returns>
        public override Geometry Buffer( double distance )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the convex hull of this Geometry.
        /// </summary>
        /// <returns>A geometry that represents the convex hull of this Geometry.</returns>
        public override Geometry ConvexHull()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set intersection of 
        /// this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>Point set intersection.</returns>
        public override Geometry Intersection( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set union of 
        /// this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>Point set union.</returns>
        public override Geometry Union( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set difference of 
        /// this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>Point set difference.</returns>
        public override Geometry Difference( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set symmetric difference of 
        /// this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="anotherGeometry">The geometry to compare with.</param>
        /// <returns>Point set symmetric difference.</returns>
        public override Geometry SymDifference( Geometry anotherGeometry )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Operators

        /// <summary>
        /// Test for Equality.
        /// </summary>
        /// <param name="object">The object to compare to.</param>
        /// <returns>True if the objects are equal.</returns>
		public override bool Equals( object @object )
		{
			return @object is Point && this == (Point)@object;
		}

        /// <summary>
        /// Overrided GetHashCode.
        /// </summary>
        /// <returns>The Hash Code</returns>
        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

		/// <summary>
		/// Test for equality.
		/// </summary>
		/// <param name="lhs">The first operand.</param>
		/// <param name="rhs">The second operand.</param>
		/// <returns>True if the points are equal.</returns>
		public static bool operator ==( Point lhs, Point rhs )
		{
			return lhs.Position == rhs.Position;
		}

		/// <summary>
		/// Test for inequality.
		/// </summary>
		/// <param name="lhs">The first operand.</param>
		/// <param name="rhs">The second operand.</param>
		/// <returns>True if the points are not equal</returns>
		public static bool operator !=( Point lhs, Point rhs )
		{
			return lhs.Position != rhs.Position;
		}

		#endregion

        #region Instance data

        /// <summary>
        /// The position of this point.
        /// </summary>
        public Coord Position;

        #endregion
	}
}
