// ============================================================================
// Filename: GeometryCollection.cs
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
using System.Text;

namespace Cobos.Geometry
{
    /// <summary>
    /// A GeometryCollection is a geometry that is a collection of 1 or more 
    /// geometries.
    /// 
    /// All the elements in a GeometryCollection must be in the same Spatial 
    /// Reference. This is also the Spatial Reference for the GeometryCollection.
    /// 
    /// GeometryCollection places no other constraints on its elements. Subclasses 
    /// of GeometryCollection may restrict membership based on dimension and may 
    /// also place other constraints on the degree of spatial overlap between elements.
    /// </summary>
	public class GeometryCollection : Geometry, IEnumerable<Geometry>
	{
		#region Construction

        /// <summary>
        /// Create an empty collection.
        /// </summary>
		public GeometryCollection()
		{
			this.geometries = new List<Geometry>();
		}

        /// <summary>
        /// Create an empty collection with a reserved capacity.
        /// </summary>
        /// <param name="capacity">The capacity to reserve.</param>
		public GeometryCollection( int capacity )
		{
			this.geometries = new List<Geometry>( capacity );
		}

        /// <summary>
        /// Create a geometry collection from another collection.
        /// </summary>
        /// <param name="geometries"></param>
		public GeometryCollection( IEnumerable<Geometry> geometries )
		{
            AssertValid( geometries );

			this.geometries = new List<Geometry>( geometries );
				
            RecalculateExtent();
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
			get { throw new NotImplementedException(); }
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
            get { return "GEOMETRYCOLLECTION"; }
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
                return this.geometries.Count == 0; 
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

		#region GeometryCollection

		/// <summary>
        /// Returns the number of geometries in this GeometryCollection.
		/// </summary>
		public int NumGeometries
		{
			get
			{
				return this.geometries.Count;
			}
		}

		/// <summary>
        /// Returns the Nth geometry in this GeometryCollection.
		/// </summary>
		/// <param name="n">The Nth.</param>
		/// <returns>The Nth geometry.</returns>
		public Geometry GeometryN( int n )
		{
			return this.geometries[ n ];
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
            get { return this.boundingBox; }
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

        #region Collection methods

        /// <summary>
		/// Add the geometry to the collection.
		/// </summary>
		/// <param name="geometry">The geometry to add.</param>
		public virtual void Add( Geometry geometry )
		{
			AssertValid( geometry );

			this.geometries.Add( geometry );

			this.boundingBox.Stretch( geometry.BoundingBox );
		}

		/// <summary>
		/// Add the collection of geometries to this collection.
		/// </summary>
		/// <param name="geometries">The collection to add.</param>
		public virtual void Add( IEnumerable<Geometry> geometries )
		{
			AssertValid( geometries );

			this.geometries.AddRange( geometries );

			foreach ( Geometry geometry in geometries )
			{
				this.boundingBox.Stretch( geometry.BoundingBox );
			}
		}

		/// <summary>
		/// Clear all geometries from the collection.
		/// </summary>
		public virtual void Clear()
		{
			this.geometries.Clear();
			this.boundingBox.SetEmpty();
		}

		/// <summary>
		/// Insert a geometry at the specified position.
		/// </summary>
		/// <param name="geometry">The geometry to insert.</param>
		/// <param name="index">The position to insert.</param>
		public virtual void Insert( Geometry geometry, int index )
		{
			AssertValid( geometry );

			this.geometries.Insert( index, geometry );

			this.boundingBox.Stretch( geometry.BoundingBox );
		}

		/// <summary>
		/// Insert the collection of geometries at the specified position.
		/// </summary>
		/// <param name="geometries">The collection to insert.</param>
		/// <param name="index">The position to insert.</param>
		public virtual void Insert( IEnumerable<Geometry> geometries, int index )
		{
			AssertValid( geometries );

			this.geometries.InsertRange( index, geometries );

			foreach ( Geometry geometry in geometries )
			{
				this.boundingBox.Stretch( geometry.BoundingBox );
			}
		}

		/// <summary>
		/// Remove the geometry at the specified position.
		/// </summary>
		/// <param name="index">The position to remove.</param>
		public virtual void Remove( int index )
		{
			this.geometries.RemoveAt( index );

			RecalculateExtent();
		}

		/// <summary>
		/// Remove a number of geometries at the specified position.
		/// </summary>
		/// <param name="index">The position to start removal.</param>
		/// <param name="count">The number of geometries to remove.</param>
		public virtual void Remove( int index, int count )
		{
			this.geometries.RemoveRange( index, count );

			RecalculateExtent();
		}

        /// <summary>
        /// Remove all geometries that match the predicate.
        /// </summary>
        /// <param name="match">The predicate to match.</param>
		public virtual void Remove( Predicate<Geometry> match )
		{
			this.geometries.RemoveAll( match );

			RecalculateExtent();
		}

		#endregion

        #region IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Geometry> GetEnumerator()
        {
            return this.geometries.GetEnumerator();
        }

        #endregion

        #region Collection Validation

        /// <summary>
        /// NOP.  Override in derived classes to assert that the container has 
        /// valid geometry.
        /// </summary>
        /// <param name="geometry">The geometry to test.</param>
		protected virtual void AssertValid( Geometry geometry )
		{
		}

        /// <summary>
        /// NOP.  Override in derived classes to assert that the container has 
        /// valid geometry.
        /// </summary>
        /// <param name="geometry">The geometries to test.</param>
        protected virtual void AssertValid( IEnumerable<Geometry> geometries )
		{
		}

		#endregion

		#region Class internal methods

		/// <summary>
		/// If a change is made to the underlying List, then recalculate the bounding extent.
		/// </summary>
		private void RecalculateExtent()
		{
			this.boundingBox.SetEmpty();

			for ( int g = 0; g < this.geometries.Count; ++g )
			{
				this.boundingBox.Stretch( this.geometries[ g ].BoundingBox );
			}
		}

		#endregion

        #region Instance data

        /// <summary>
        /// The minimum bounding rectangle for the collection.
        /// </summary>
        private BoundingBox boundingBox = new BoundingBox( true );

        /// <summary>
        /// The internal collection of geometries.
        /// </summary>
        private readonly List<Geometry> geometries;

        #endregion
	}
}
