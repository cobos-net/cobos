// ============================================================================
// Filename: Polygon.cs
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cobos.Geometry
{
    /// <summary>
    /// A Polygon is a planar Surface, defined by 1 exterior boundary and 0 or 
    /// more interior boundaries. Each interior boundary defines a hole in the 
    /// Polygon.
    /// 
    /// The assertions for polygons (the rules that define valid polygons) are:
    /// 
    /// 1. Polygons are topologically closed.
    /// 
    /// 2. The boundary of a Polygon consists of a set of LinearRings that make 
    ///    up its exterior and interior boundaries.
    /// 
    /// 3. No two rings in the boundary cross, the rings in the boundary of a 
    ///    Polygon may intersect at a Point but only as a tangent.
    /// 
    /// 4. A Polygon may not have cut lines, spikes or punctures.
    /// 
    /// 5. The Interior of every Polygon is a connected point set.
    /// 
    /// 6. The Exterior of a Polygon with 1 or more holes is not connected. 
    ///    Each hole defines a connected component of the Exterior.
    /// 
    /// In the above assertions, Interior, Closure and Exterior have the 
    /// standard topological definitions. The combination of 1 and 3 make a 
    /// Polygon a Regular Closed point set.
    /// 
    /// Polygons are simple geometries.
    /// </summary>
    public class Polygon : Surface
    {
        /// <summary>
        /// Construct the polygon with a single exterior boundary.
        /// 
        /// Performs validation if required.  If you can guarantee
        /// that the rings are valid then you can opt not to perform
        /// validation.
        /// </summary>
        /// <param name="exterior">The exterior boundary.</param>
        /// <param name="validate">Whether to perform validation or not.</param>
        public Polygon( LineString exterior, bool validate )
            : this( exterior, Enumerable.Empty<LineString>(), validate )
        {
        }

        /// <summary>
        /// Construct the polygon with a single exterior boundary
        /// and any number of interior boundaries (holes).
        /// 
        /// Performs validation if required.  If you can guarantee
        /// that the rings are valid then you can opt not to perform
        /// validation.
        /// </summary>
        /// <param name="exterior">The exterior boundary</param>
        /// <param name="interiors">The interior boundaries</param>
        /// <param name="validate">Whether to perform validation or not.</param>
        public Polygon( LineString exterior, IEnumerable<LineString> interiors, bool validate )
        {
            this.exterior = exterior;

            this.interiors = new List<LineString>( interiors );

            if ( validate )
            {
                Validate();
            }
        }

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
        public override DimensionType Dimension
        {
            get
            {
                return this.exterior == null || this.exterior.NumPoints == 0 ? DimensionType.Empty : DimensionType.Dim2d;
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
            get { return "POLYGON"; }
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
                if ( this.exterior == null || this.exterior.NumPoints == 0 )
                {
                    return true;
                }
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

        #region Surface

        /// <summary>
        /// The area of this Surface, as measured in the spatial reference 
        /// system of this Surface.
        /// </summary>
        public override double Area
        {
            get
            {
                return this.exterior.Coords.Area;
            }
        }

        /// <summary>
        /// The mathematical centroid for this Surface as a Point. 
        /// The result is not guaranteed to be on this Surface.
        /// </summary>
        public override Coord Centroid
        {
            get
            {
                return this.exterior.Coords.Centroid;
            }
        }

        /// <summary>
        /// A point guaranteed to be on this Surface.
        /// </summary>
        public override Coord PointOnSurface
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Polygon

        /// <summary>
        /// Returns the exterior ring of this Polygon.
        /// </summary>
        public LineString ExteriorRing
        {
            get
            {
                return this.exterior;
            }
        }

        /// <summary>
        /// Returns the number of interior rings in this Polygon.
        /// </summary>
        public int NumInteriorRing
        {
            get
            {
                return this.interiors.Count;
            }
        }

        /// <summary>
        /// Returns the Nth interior ring for this Polygon as a LineString.
        /// </summary>
        /// <param name="n">The Nth interior.</param>
        /// <returns></returns>
        public LineString InteriorRingN( int n )
        {
            return this.interiors[ n ];
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
                return this.exterior.BoundingBox;
            }
        }

        /// <summary>
        /// Test whether the geometry contains the point.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public bool Contains( Coord coord )
        {
            if ( !this.exterior.Contains( coord ) )
            {
                return false;
            }

            foreach ( LineString interior in this.interiors )
            {
                if ( interior.Contains( coord ) )
                {
                    return false;
                }
            }

            return true;
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

        #region Internal methods

        /// <summary>
        /// Validate that this is a simple geometry.
        /// </summary>
        private void Validate()
        {
            if ( !this.exterior.IsRing )
            {
                throw new ValidationException( "The exterior boundary of the polygon is not a ring." );
            }

            // Test each interior is a ring and it doesn't cross the exterior.
            for ( int i = 0; i < this.interiors.Count; ++i )
            {
                LineString interior = interiors[ i ];

                if ( !interior.IsRing )
                {
                    throw new ValidationException( "An interior boundary of the polygon is not a ring." );
                }

                if ( !this.exterior.Contains( interior ) )
                {
                    throw new ValidationException( "An interior boundary overlaps the exterior ring." );
                }
            }

            // Compare each interior ring with each other to test for overlapping.
            for ( int i = 0; i < this.interiors.Count; ++i )
            {
                LineString interior = interiors[ i ];

                for ( int j = i + 1; j < this.interiors.Count - 1; ++j )
                {
                    if ( interior.Overlaps( this.interiors[ j ] ) )
                    {
                        throw new ValidationException( "The interior boundary at " + i + " overlaps the interior ring at " + j );
                    }
                }
            }
        }

        #endregion

        #region Instance data

        /// <summary>
        /// The exterior boundary of the polygon.
        /// </summary>
        private LineString exterior;

        /// <summary>
        /// The interior boundaries of the polygon.
        /// </summary>
        private List<LineString> interiors;

        #endregion
    }
}
