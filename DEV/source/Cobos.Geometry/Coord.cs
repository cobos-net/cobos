using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Cobos.Geometry
{
	[Serializable]
	public struct Coord
	{
		/// <summary>
		/// 
		/// </summary>
		public float X;

		/// <summary>
		/// 
		/// </summary>
		public float Y;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Coord( float x, float y )
			: this()
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rhs"></param>
		public Coord( ref Coord rhs )
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
		/// 
		/// </summary>
		/// <param name="coord"></param>
		/// <returns></returns>
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
