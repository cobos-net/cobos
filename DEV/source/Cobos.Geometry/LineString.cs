// ============================================================================
// Filename: LineString.cs
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
using System.Linq;

namespace Cobos.Geometry
{
	/// <summary>
	/// A LineString is a Curve with linear interpolation between points. 
    /// Each consecutive pair of points defines a line segment.
	/// </summary>
	public class LineString : Curve
	{
		#region Construction

        /// <summary>
        /// Construct a new LineString from the list of coordinates.
        /// </summary>
        /// <param name="coords"></param>
		public LineString( IEnumerable<Coord> coords )
		{
            this.coords = Enumerable.ToArray<Coord>( coords );

            if ( this.coords.Length < 2 )
            {
                throw new ValidationException( "Cannot construct a LineString with only " + this.coords.Length + " point(s)" );
            }

            this.boundingBox = new BoundingBox();
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
        public override Geometry.DimensionType Dimension
		{
			get
			{
				return DimensionType.Dim1d;
			}
		}

        /// <summary>
        /// The minimum bounding box for this Geometry, returned as a Geometry. 
        /// The polygon is defined by the corner points of the bounding box 
        /// ((MINX, MINY), (MAXX, MINY), (MAXX, MAXY), (MINX, MAXY), (MINX, MINY))
        /// </summary>
        public override Geometry Envelope
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the name of the instantiable subtype of Geometry of which this
        /// Geometry instance is a member. The name of the instantiable subtype of 
        /// Geometry is returned as a string.
        /// </summary>
        public override string GeometryType
        {
            get { return "LINESTRING"; }
        }

        /// <summary>
        /// Returns true if this Geometry is the empty geometry. If true, then 
        /// this Geometry represents the empty point set, Ø, for the coordinate 
        /// space.
        /// </summary>
        public override bool IsEmpty
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Returns true if this Geometry has no anomalous geometric points, 
        /// such as self intersection or self tangency. The description of each 
        /// instantiable geometric class will include the specific conditions 
        /// that cause an instance of that class to be classified as not simple.
        /// </summary>
        public override bool IsSimple
        {
            get { throw new NotImplementedException(); }
        }

		#endregion

		#region Curve

        /// <summary>
        /// The end point of this Curve.
        /// </summary>
        public override Coord EndPoint
		{
			get
			{
				return this.coords[ this.coords.Length - 1 ];
			}
		}

        /// <summary>
        /// Returns true if this Curve is closed (StartPoint ( ) = EndPoint ( )).
        /// </summary>
        public override bool IsClosed
		{
			get
			{
				return this.coords[ 0 ] == this.coords[ this.coords.Length - 1 ];
			}
		}

        /// <summary>
        /// Returns true if this Curve is closed (StartPoint ( ) = EndPoint ( ))
        /// and this Curve is simple.
        /// </summary>
        public override bool IsRing
		{
			get
			{
                return IsClosed && IsSimple;
			}
		}

        /// <summary>
        /// The length of this Curve in its associated spatial reference.
        /// </summary>
        public override double Length
		{
			get
			{
                return Segment.Length( this.coords );
			}
		}

        /// <summary>
        /// The start point of this Curve.
        /// </summary>
        public override Coord StartPoint
        {
            get
            {
                return this.coords[ 0 ];
            }
        }

		#endregion

		#region LineString

		/// <summary>
        /// The number of points in this LineString.
		/// </summary>
		public int NumPoints
		{
			get
			{
				return this.coords.Length;
			}
		}

		/// <summary>
        /// Returns the specified point N in this Linestring.
		/// </summary>
		/// <param name="n">The Nth point.</param>
        /// <returns>Returns the specified point N in this Linestring.</returns>
		public Coord PointN( int n )
		{
			return this.coords[ n ];
		}

		#endregion 

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
                return this.coords.BoundingBox;
            }
        }

        /// <summary>
        /// http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        /// 
        /// This is an expansion of the answer in the comp.graphics.algorithms FAQ 
        /// question 2.03, How do I find if a point lies within a polygon? 
        /// 
        /// Run a semi-infinite ray horizontally (increasing x, fixed y) out from the
        /// test point, and count how many edges it crosses.  At each crossing, the 
        /// ray switches between inside and outside. 
        /// 
        /// This is called the Jordan curve theorem.
        /// </summary>
        /// <param name="coord">The point to test for</param>
        /// <returns></returns>
        public bool Contains( Coord coord )
        {
            if ( !this.boundingBox.Contains( coord ) )
            {
                return false;
            }

            bool isIn = false;

            int i, j = 0;
            for ( i = 0, j = this.coords.Length - 1; i < this.coords.Length; j = i++ )
            {
                Coord p0 = this.coords[ i ];
                Coord p1 = this.coords[ j ];

                if ( (((p0.Y <= coord.Y) && (coord.Y < p1.Y)) || ((p1.Y <= coord.Y) && (coord.Y < p0.Y)))
                        && (coord.X < (p1.X - p0.X) * (coord.Y - p0.Y) / (p1.Y - p0.Y) + p0.X) )
                {
                    isIn = !isIn;
                }
            }

            return isIn;
        }

        /// <summary>
        /// A Line is a LineString with exactly 2 points.
        /// </summary>
		public bool IsLine
		{
			get
			{
				return this.coords.Length == 2;
			}
		}

        #endregion

        #region Internal methods

        /// <summary>
        /// Calculate the extent of the coords
        /// </summary>
        private void RecalculateExtent()
        {
            this.boundingBox.SetEmpty();

            for ( int p = 0; p < this.coords.Length; ++p )
            {
                Coord point = this.coords[ p ];
                this.boundingBox.Stretch( point.X, point.Y );
            }
        }

        #endregion

        #region Instance data

        /// <summary>
        /// The coordinates may not be modified.  
        /// Once constructed the geometry is immutable.
        /// </summary>
        private readonly Coord[] coords;

        /// <summary>
        /// Minimum Bounding Rectangle
        /// </summary>
        private readonly BoundingBox boundingBox;

        #endregion
	}

}
