// ============================================================================
// Filename: Coord.cs
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
    /// Representation of a 2D coordinate pair.
    /// </summary>
    public struct Coord
    {
        /// <summary>
        /// The X coordinate value.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y coordinate value.
        /// </summary>
        public double Y;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Coord( double x, double y )
            : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Copy 
        /// </summary>
        /// <param name="rhs"></param>
        public Coord( Coord rhs )
            : this()
        {
            X = rhs.X;
            Y = rhs.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals( object obj )
        {
            return obj is Coord && this == (Coord)obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==( Coord lhs, Coord rhs )
        {
            return (lhs.X == rhs.X) && (lhs.Y == rhs.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=( Coord lhs, Coord rhs )
        {
            return !((lhs.X == rhs.X) && (lhs.Y == rhs.Y));
        }

        /// <summary>
        /// The result of a unary + operation on a numeric type is just the 
        /// value of the operand.
        /// </summary>
        /// <param name="coord">The operand.</param>
        /// <returns>The value of the operand.</returns>
        public static Coord operator +( Coord coord )
        {
            return new Coord( coord.X, coord.Y );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Coord operator +( Coord lhs, Coord rhs )
        {
            return new Coord( lhs.X + rhs.X, lhs.Y + rhs.Y );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static Coord operator -( Coord coord )
        {
            return new Coord( 0 - coord.X, 0 - coord.Y );
        }
    }
}
