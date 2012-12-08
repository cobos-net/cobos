using System;
using System.Collections.Generic;
using System.Text;

namespace Cobos.Geometry
{
	[Serializable]
	public struct Envelope
	{
		// Called lower and upper bound rather bottom left etc
		// so that the extent can be used for screen and world
		// coordinate extents.
		public Coord LowerBound
		{
			get
			{
				return _lowerBound;
			}
		}

		public Coord UpperBound
		{
			get
			{
				return _upperBound;
			}
		}

		Coord _lowerBound;
		Coord _upperBound;

		/// <summary>
		/// Initialises Extent to invalid state
		/// </summary>
		public Envelope( bool setEmpty )
		{
			_lowerBound = default( Coord );
			_upperBound = default( Coord );

			if ( setEmpty )
			{
				// Set the corners to opposite so that the extent is automatically stretched when 
				// compared against another extent.
				_lowerBound.X = float.MaxValue;
				_lowerBound.Y = float.MaxValue;
				_upperBound.X = float.MinValue;
				_upperBound.Y = float.MinValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		public Envelope( float x1, float y1, float x2, float y2 )
		{
			_lowerBound = new Coord( x1, y1 );
			_upperBound = new Coord( x2, y2 );

			CheckCorners();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lower"></param>
		/// <param name="upper"></param>
		public Envelope( Coord lower, Coord upper )
		{
			_lowerBound = lower;
			_upperBound = upper;

			CheckCorners();
		}

		public static Envelope operator + ( Envelope lhs, Envelope rhs )
		{
			Envelope extent = new Envelope( lhs.LowerBound, lhs.UpperBound );
			extent.Stretch( rhs );
			
			return extent;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool Equals( object obj )
		{
			return obj is Envelope && this == (Envelope)obj;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool operator ==( Envelope lhs, Envelope rhs )
		{
			return (lhs.LowerBound == rhs.LowerBound) && (lhs.UpperBound == rhs.UpperBound);
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
		public static bool operator !=( Envelope lhs, Envelope rhs )
		{
			return !((lhs.LowerBound == rhs.LowerBound) && (lhs.UpperBound == rhs.UpperBound));
		}

		/// <summary>
		/// Always maintain the correct lower/upper values
		/// </summary>
		void CheckCorners()
		{
			float temp;

			if ( _lowerBound.X > _upperBound.X )
			{
				temp = _lowerBound.X;
				_lowerBound.X = _upperBound.X;
				_upperBound.X = temp;
			}

			if ( _lowerBound.Y > _upperBound.Y )
			{
				temp = _lowerBound.Y;
				_lowerBound.Y = _upperBound.Y;
				_upperBound.Y = temp;
			}
		}

		/// <summary>
		/// Set the corners to opposite so that the extent is automatically stretched when compared against another extent.
		/// </summary>
		public void SetEmpty()
		{
			_lowerBound.X = float.MaxValue;
			_lowerBound.Y = float.MaxValue;
			_upperBound.X = float.MinValue;
			_upperBound.Y = float.MinValue;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return _lowerBound.X == float.MaxValue &&
						 _lowerBound.Y == float.MaxValue &&
						 _upperBound.X == float.MinValue &&
						 _upperBound.Y == float.MinValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void SetLowerBound( float x, float y )
		{
			_lowerBound.X = x;
			_lowerBound.Y = y;

			CheckCorners();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void SetUpperBound( float x, float y )
		{
			_upperBound.X = x;
			_upperBound.Y = y;

			CheckCorners();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public float Height
		{
			get
			{
				return _upperBound.Y - _lowerBound.Y;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float Width
		{
			get
			{
				return _upperBound.X - _lowerBound.X;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		public void SetCorners( float x1, float y1, float x2, float y2 )
		{
			_lowerBound.X = x1;
			_lowerBound.Y = y1;
			_upperBound.X = x2;
			_upperBound.Y = y2;

			CheckCorners();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		public void GetCorners( ref float x1, ref float y1, ref float x2, ref float y2 )
		{
			x1 = _lowerBound.X;
			y1 = _lowerBound.Y;
			x2 = _upperBound.X;
			y2 = _upperBound.Y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="extent"></param>
		public void Stretch( Envelope extent )
		{
			Stretch( extent._lowerBound.X, extent._lowerBound.Y );
			Stretch( extent._upperBound.X, extent._upperBound.Y );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void Stretch( float x, float y )
		{
			if ( x < _lowerBound.X )
			{
				_lowerBound.X = x;
			}
			if ( x > _upperBound.X )
			{
				_upperBound.X = x;
			}
			if ( y < _lowerBound.Y )
			{
				_lowerBound.Y = y;
			}
			if ( y > _upperBound.Y )
			{
				_upperBound.Y = y;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Coord Centre
		{
			get
			{
				return new Coord( _lowerBound.X + ((_upperBound.X - _lowerBound.X) / 2), _lowerBound.Y + ((_upperBound.Y - _lowerBound.Y) / 2) );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="anotherExtent"></param>
		/// <returns></returns>
		public bool Overlaps( Envelope anotherExtent )
		{
			return _lowerBound.X < anotherExtent._upperBound.X &&
					 _upperBound.X > anotherExtent._lowerBound.X &&
					 _lowerBound.Y < anotherExtent._upperBound.Y &&
					 _upperBound.Y > anotherExtent._lowerBound.Y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="anotherExtent"></param>
		/// <returns></returns>
		public bool Contains( Envelope anotherExtent )
		{
			return _lowerBound.X <= anotherExtent._lowerBound.X &&
					 _lowerBound.Y <= anotherExtent._lowerBound.Y &&
					 _upperBound.X >= anotherExtent._upperBound.X &&
					 _upperBound.Y >= anotherExtent._upperBound.Y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coord"></param>
		/// <returns></returns>
		public bool Contains( Coord coord )
		{
			return coord.X >= _lowerBound.X &&
					 coord.Y >= _lowerBound.Y &&
					 coord.X <= _upperBound.X &&
					 coord.Y <= _upperBound.Y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dx"></param>
		/// <param name="dy"></param>
		public void Translate( float dx, float dy )
		{
			_lowerBound.X += dx;
			_lowerBound.Y += dy;
			_upperBound.X += dx;
			_upperBound.Y += dy;
		}
	}
}
