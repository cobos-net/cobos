// ============================================================================
// Filename: BoundingBox.cs
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
    /// Represents a minimum bounding rectangle (MBR).
    /// </summary>
	public struct BoundingBox
	{
        #region Construction

        /// <summary>
		/// Initialises Extent to invalid state
		/// </summary>
		public BoundingBox()
		{
			// Set the corners to opposite so that the extent is 
            // automatically stretched when compared against another extent.
			this.lowerBound.X = double.MaxValue;
			this.lowerBound.Y = double.MaxValue;
			this.upperBound.X = double.MinValue;
			this.upperBound.Y = double.MinValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		public BoundingBox( double x1, double y1, double x2, double y2 )
		{
			this.lowerBound = new Coord( x1, y1 );
			this.upperBound = new Coord( x2, y2 );

			CheckCorners();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lower"></param>
		/// <param name="upper"></param>
		public BoundingBox( Coord lower, Coord upper )
		{
			this.lowerBound = lower;
			this.upperBound = upper;

			CheckCorners();
		}

        #endregion

        #region Public properties

        /// <summary>
        /// 
        /// </summary>
        public Coord Centre
        {
            get
            {
                return new Coord( this.lowerBound.X + ((this.upperBound.X - this.lowerBound.X) / 2), this.lowerBound.Y + ((this.upperBound.Y - this.lowerBound.Y) / 2) );
            }
        }

        /// <summary>
        /// The minimum bounding box for this Geometry, returned as a Geometry. 
        /// The polygon is defined by the corner points of the bounding box 
        /// ((MINX, MINY), (MAXX, MINY), (MAXX, MAXY), (MINX, MAXY), (MINX, MINY))
        /// </summary>
        public Geometry Envelope
        {
            get
            {
                LineString exteriorRing = new LineString( new Coord[] { 
                                        new Coord( this.lowerBound.X, this.lowerBound.Y ),
                                        new Coord( this.upperBound.X, this.lowerBound.Y ),
                                        new Coord( this.upperBound.X, this.upperBound.Y ),
                                        new Coord( this.lowerBound.X, this.upperBound.Y ),
                                        new Coord( this.lowerBound.X, this.lowerBound.Y ) } );

                return new Polygon( exteriorRing, false );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.lowerBound.X == double.MaxValue &&
                         this.lowerBound.Y == double.MaxValue &&
                         this.upperBound.X == double.MinValue &&
                         this.upperBound.Y == double.MinValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Height
        {
            get
            {
                return this.upperBound.Y - this.lowerBound.Y;
            }
        }

        /// <summary>
        /// The lower bound of the bounding box.
        /// </summary>
        public Coord LowerBound
        {
            get
            {
                return this.lowerBound;
            }
        }

        /// <summary>
        /// The upper bound of the bounding box.
        /// </summary>
        public Coord UpperBound
        {
            get
            {
                return this.upperBound;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Width
        {
            get
            {
                return this.upperBound.X - this.lowerBound.X;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Always maintain the correct lower/upper values
        /// </summary>
        private void CheckCorners()
        {
            double temp;

            if ( this.lowerBound.X > this.upperBound.X )
            {
                temp = this.lowerBound.X;
                this.lowerBound.X = this.upperBound.X;
                this.upperBound.X = temp;
            }

            if ( this.lowerBound.Y > this.upperBound.Y )
            {
                temp = this.lowerBound.Y;
                this.lowerBound.Y = this.upperBound.Y;
                this.upperBound.Y = temp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void GetCorners( ref double x1, ref double y1, ref double x2, ref double y2 )
        {
            x1 = this.lowerBound.X;
            y1 = this.lowerBound.Y;
            x2 = this.upperBound.X;
            y2 = this.upperBound.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void SetCorners( double x1, double y1, double x2, double y2 )
        {
            this.lowerBound.X = x1;
            this.lowerBound.Y = y1;
            this.upperBound.X = x2;
            this.upperBound.Y = y2;

            CheckCorners();
        }

        /// <summary>
        /// Set the corners to opposite so that the extent is automatically stretched when compared against another extent.
        /// </summary>
        public void SetEmpty()
        {
            this.lowerBound.X = double.MaxValue;
            this.lowerBound.Y = double.MaxValue;
            this.upperBound.X = double.MinValue;
            this.upperBound.Y = double.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetLowerBound( double x, double y )
        {
            this.lowerBound.X = x;
            this.lowerBound.Y = y;

            CheckCorners();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetUpperBound( double x, double y )
        {
            this.upperBound.X = x;
            this.upperBound.Y = y;

            CheckCorners();
        }

        #endregion

        #region Comparison

        /// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool Equals( object obj )
		{
			return obj is BoundingBox && this == (BoundingBox)obj;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return LowerBound.GetHashCode() ^ UpperBound.GetHashCode();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool operator ==( BoundingBox lhs, BoundingBox rhs )
		{
			return (lhs.LowerBound == rhs.LowerBound) && (lhs.upperBound== rhs.UpperBound);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool operator !=( BoundingBox lhs, BoundingBox rhs )
		{
			return !((lhs.LowerBound == rhs.LowerBound) && (lhs.upperBound== rhs.UpperBound));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anotherExtent"></param>
        /// <returns></returns>
        public bool Overlaps( BoundingBox anotherExtent )
        {
            return this.lowerBound.X < anotherExtent.upperBound.X &&
                     this.upperBound.X > anotherExtent.lowerBound.X &&
                     this.lowerBound.Y < anotherExtent.upperBound.Y &&
                     this.upperBound.Y > anotherExtent.lowerBound.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anotherExtent"></param>
        /// <returns></returns>
        public bool Contains( BoundingBox anotherExtent )
        {
            return this.lowerBound.X <= anotherExtent.lowerBound.X &&
                     this.lowerBound.Y <= anotherExtent.lowerBound.Y &&
                     this.upperBound.X >= anotherExtent.upperBound.X &&
                     this.upperBound.Y >= anotherExtent.upperBound.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public bool Contains( Coord coord )
        {
            return coord.X >= this.lowerBound.X &&
                     coord.Y >= this.lowerBound.Y &&
                     coord.X <= this.upperBound.X &&
                     coord.Y <= this.upperBound.Y;
        }

        #endregion

        #region Additive

        /// <summary>
        /// Calculate the combined bounding box of the two bounding boxes.
        /// </summary>
        /// <param name="lhs">The first box.</param>
        /// <param name="rhs">The second box.</param>
        /// <returns>The combined bounding box.</returns>
        public static BoundingBox operator +( BoundingBox lhs, BoundingBox rhs )
        {
            BoundingBox extent = new BoundingBox( lhs.LowerBound, lhs.upperBound );

            extent.Stretch( rhs );

            return extent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extent"></param>
        public void Stretch( BoundingBox extent )
        {
            Stretch( extent.lowerBound.X, extent.lowerBound.Y );
            Stretch( extent.upperBound.X, extent.upperBound.Y );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Stretch( double x, double y )
        {
            if ( x < this.lowerBound.X )
            {
                this.lowerBound.X = x;
            }
            if ( x > this.upperBound.X )
            {
                this.upperBound.X = x;
            }
            if ( y < this.lowerBound.Y )
            {
                this.lowerBound.Y = y;
            }
            if ( y > this.upperBound.Y )
            {
                this.upperBound.Y = y;
            }
        }

        #endregion

        #region Transformation

        /// <summary>
        /// Rotate the bounding box around the centre point
        /// </summary>
        /// <param name="theta">The angle of rotation in radians.</param>
        /// <param name="centre">Optional centre of rotation.  If null, the centre of the box is used.</param>
        public void Rotate( double theta, Coord? centre )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scale the bounding box.
        /// </summary>
        /// <param name="sx">The X scale factor.</param>
        /// <param name="sy">The Y scale factor.</param>
        /// <param name="centre">Optional centre of scale.  If null, the centre of the box is used.</param>
        public void Scale( double sx, double sy, Coord? centre )
        {
            Coord d = centre.GetValueOrDefault( Centre );

            this.lowerBound.X = (this.lowerBound.X - d.X) * sx + d.X;
            this.lowerBound.Y = (this.lowerBound.Y - d.Y) * sy + d.Y;
            this.upperBound.X = (this.upperBound.X - d.X) * sx + d.X;
            this.upperBound.Y = (this.upperBound.Y - d.Y) * sy + d.Y;
        }

        /// <summary>
        /// Scale the bounding box.
        /// </summary>
        /// <param name="s">The scale factor.</param>
        /// <param name="centre">Optional centre of scale.  If null, the centre of the box is used.</param>
        public void Scale( double s, Coord? centre )
        {
            Scale( s, s, centre );
        }

        /// <summary>
        /// Translate the bounding box.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Translate( double dx, double dy )
        {
            this.lowerBound.X += dx;
            this.lowerBound.Y += dy;
            this.upperBound.X += dx;
            this.upperBound.Y += dy;
        }

        #endregion

        #region Instance data

        private Coord lowerBound;
        private Coord upperBound;

        #endregion
    }
}
