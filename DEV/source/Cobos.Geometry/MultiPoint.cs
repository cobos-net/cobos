// ============================================================================
// Filename: MultiPoint.cs
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
    /// 
    /// </summary>
	public class MultiPoint : GeometryCollection
	{
		#region Construction

        /// <summary>
        /// Create an empty set.
        /// </summary>
		public MultiPoint()
			: base()
		{
		}

        /// <summary>
        /// Create an empty set with an initial reserved capacity.
        /// </summary>
        /// <param name="capacity">The capacity to reserve.</param>
		public MultiPoint( int capacity )
			: base( capacity )
		{
		}

        /// <summary>
        /// Create a collection containing the geometries.
        /// </summary>
        /// <param name="geometries">The geometries to add to the collection.</param>
		public MultiPoint( IEnumerable<Geometry> geometries )
			: base( geometries )
		{
		}

		#endregion

		#region OpenGIS specification

		#region Geometry

        /// <summary>
        /// Returns the name of the instantiable subtype of Geometry of which this
        /// Geometry instance is a member. The name of the instantiable subtype of 
        /// Geometry is returned as a string.
        /// </summary>
		public override string GeometryType
		{
			get { return "MULTIPOINT"; }
		}

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

		#endregion

		#endregion

		#region Collection validation

        /// <summary>
        /// Assert that the geometry about to be added is a Point.
        /// </summary>
        /// <param name="geometry">The geometry to check.</param>
		protected override void AssertValid( Geometry geometry )
		{
			if ( !(geometry is Point) )
			{
                throw new ValidationException( "MultiPoint cannot contain an object of type: " + geometry.GeometryType );
			}
		}

        /// <summary>
        /// Assert that the geometries about to be added are Points.
        /// </summary>
        /// <param name="geometry">The geometries to check.</param>
        protected override void AssertValid( IEnumerable<Geometry> geometries )
		{
			if ( geometries != null )
			{
				foreach ( Geometry geometry in geometries )
				{
					if ( !(geometry is Point) )
					{
                        throw new ValidationException( "MultiPoint cannot contain an object of type: " + geometry.GeometryType );
					}
				}
			}
		}

		#endregion
	}
}
