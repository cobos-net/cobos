using System;
using System.Collections.Generic;
using System.Text;

namespace Cobos.Geometry
{
	[Serializable]
	public class Point : Geometry
	{
		#region Instance data

		public Coord Position;

		#endregion

		#region Construction

		public Point()
		{
			Position.X = float.NaN;
			Position.Y = float.NaN;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Point( float x, float y )
			: this()
		{
			Position.X = x;
			Position.Y = y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rhs"></param>
		public Point( ref Point rhs )
			: this()
		{
			Position.X = rhs.Position.X;
			Position.Y = rhs.Position.Y;
		}

		#endregion

		#region OpenGIS implementation

		#region Geometry

		/// <summary>
		/// 
		/// </summary>
		public override DimensionType Dimension
		{
			get 
			{
				return Position.X == float.NaN || Position.Y == float.NaN ? DimensionType.Empty : DimensionType.Dim0d;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsSimple
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsEmpty
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override Envelope Bounds
		{
			get 
			{
				return Position.X == float.NaN || Position.Y == float.NaN ? new Envelope( true ) : new Envelope( new Coord( Position.X, Position.Y ), new Coord( Position.X, Position.Y ) );
			}
		}

		public override Geometry Boundary
		{
			get { throw new NotImplementedException(); }
		}

		public override Geometry Envelope
		{
			get { throw new NotImplementedException(); }
		}

		public override string GeometryType
		{
			get { return "POINT"; }
		}

		public override byte[] AsBinary
		{
			get { throw new NotImplementedException(); }
		}

		public override string AsText
		{
			get { throw new NotImplementedException(); }
		}


		#endregion

		#endregion

		#region Operators

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool Equals( object obj )
		{
			return obj is Point && this == (Point)obj;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool operator ==( Point lhs, Point rhs )
		{
			return lhs.Position == rhs.Position;
		}

		public override int GetHashCode()
		{
			return Position.GetHashCode();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool operator !=( Point lhs, Point rhs )
		{
			return lhs.Position != rhs.Position;
		}

		#endregion

	}
}
