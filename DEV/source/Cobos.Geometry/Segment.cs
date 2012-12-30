// ============================================================================
// Filename: Segment.cs
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
    /// Helper utility class for geometric operations on segments.
    /// </summary>
	public static class Segment
	{
        /// <summary>
        /// Test whether the point is within the segment.
        /// </summary>
        /// <param name="pt">The point to test for.</param>
        /// <param name="p0">The segment start point.</param>
        /// <param name="p1">The segment end point.</param>
        /// <returns>True if the point is within the segment.</returns>
        public static bool CoordWithin( Coord pt, Coord p0, Coord p1 )
        {
            // This code is based upon 'A fast 2d point-on-line test - graphics gems I' p 49

            Double Px = p0.X;
            Double Py = p0.Y;
            Double Qx = p1.X;
            Double Qy = p1.Y;
            Double Tx = pt.X;
            Double Ty = pt.Y;
            Double dx = Qx - Px;
            Double dy = Qy - Py;
            Double fdx = Math.Abs( dx );
            Double fdy = Math.Abs( dy );
            Double md = fdx > fdy ? fdx : fdy;

            if ( Math.Abs( dy * (Tx - Px) - (Ty - Py) * dx ) >= md )
            {
                return false;
            }

            if ( (Qx < Px && Px < Tx) || (Qy < Py && Py < Ty)
              || (Tx < Px && Px < Qx) || (Ty < Py && Py < Qy) )
            {
                return false; // on open ray P->
            }

            if ( (Px < Qx && Qx < Tx) || (Py < Qy && Qy < Ty)
              || (Tx < Qx && Qx < Px) || (Ty < Qy && Qy < Py) )
            {
                return false; // on open ray Q->
            }

            return true;
        }

        /// <summary>
        /// Calculate the Length of a single segment.
        /// </summary>
        /// <param name="p0">The segment start point.</param>
        /// <param name="p1">The segment end point.</param>
        /// <returns>The length of the segment.</returns>
        public static double Length( Coord p0, Coord p1 )
        {
            double dx = p1.X - p0.X;
            double dy = p1.Y - p0.Y;

            return Math.Sqrt( (dx * dx) + (dy * dy) );
        }

        /// <summary>
        /// Calculate the overall length of a list of segments.
        /// </summary>
        /// <param name="coords">The collection of points defining the segments.</param>
        /// <returns>The overall length of the list of segments.</returns>
        public static double Length( Coord[] coords )
        {
            double length = 0.0;

            for ( int c = 0; c < coords.Length - 1; ++c )
            {
                length += Length( coords[ c ], coords[ c + 1 ] );
            }

            return length;
        }
	}
}
