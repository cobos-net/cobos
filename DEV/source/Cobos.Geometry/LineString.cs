using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cobos.Geometry
{
	/// <summary>
	/// 
	/// </summary>
	
	public class LineString : Curve
	{
		/// <summary>
		/// 
		/// </summary>
		public class ValidationError : Exception
		{
			ValidationError( string msg, params object[] args )
				: base( string.Format( msg, args ) )
			{
			}
		}

		#region Instance data

		/// <summary>
		/// The coords may be modified directly by derived classes as
		/// long as the extent is properly recalculated.
		/// </summary>
		protected CoordSet _coords;

		/// <summary>
		/// Nullable type so that we can determine whether the value has been calculated.
		/// Using lazy evaluation, so delay calculation until we need the value.
		/// </summary>
		[NonSerialized]
		protected bool? _isSimple;

		#endregion

		#region Construction

		/// <summary>
		/// 
		/// </summary>
		/// <param name="closed"></param>
		public LineString()
		{
			_coords = new CoordSet();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity"></param>
		public LineString( int capacity )
		{
			_coords = new CoordSet( capacity );
		}

		public LineString( IEnumerable<Coord> coords )
		{
			_coords = new CoordSet( coords );
		}

		#endregion

		#region OpenGIS implementation

		#region Geometry

		/// <summary>
		/// A Curve is defined as a one-dimensional geometry
		/// </summary>
		public override Geometry.DimensionType Dimension
		{
			get
			{
				return _coords.NumCoords > 0 ? DimensionType.Dim1d : DimensionType.Empty;
			}
		}

		/// <summary>
		/// Get the bounding extent of the curve's coordinates
		/// </summary>
		public override Envelope Bounds
		{
			get
			{
				return _coords.Bounds;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				return _coords.NumCoords == 0;
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
			get { return "LINESTRING"; }
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

		#region Curve

		public override Coord StartPoint
		{
			get
			{
				if ( _coords.NumCoords < 2 )
				{
					throw new InvalidOperationException( "The curve has less than two points." );
				}

				return _coords[ 0 ];
			}
		}

		public override Coord EndPoint
		{
			get
			{
				if ( _coords.NumCoords < 2 )
				{
					throw new InvalidOperationException( "The curve has less than two points." );
				}

				return _coords[ _coords.NumCoords - 1 ];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool? IsClosed
		{
			get
			{
				if ( _coords.NumCoords < 2 )
				{
					throw new InvalidOperationException( "The curve has less than two points." );
				}

				return _coords[ 0 ] == _coords[ _coords.NumCoords - 1 ];
			}
		}

		/// <summary>
		/// A Curve is simple if it does not pass through the same point twice.
		/// </summary>
		public override bool IsSimple
		{
			get
			{
				if ( !_isSimple.HasValue )
				{
					_isSimple = !_coords.IsSelfIntersecting;
				}
				return _isSimple.Value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsRing
		{
			get
			{
				bool? isClosed = IsClosed;

				if ( !isClosed.HasValue )
				{
					return false;
				}

				return isClosed.Value && IsSimple;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public override double Length
		{
			get
			{
				return _coords.Length;
			}
		}

		#endregion

		#region LineString

		/// <summary>
		/// 
		/// </summary>
		public int NumPoints
		{
			get
			{
				return _coords.NumCoords;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public Coord PointN( int n )
		{
			return _coords[ n ];
		}

		#endregion 

		#endregion


		#region Coordinate manipulation

		/// <summary>
		/// Access the underlying coords without modifying the content
		/// </summary>
		public CoordSet Coords
		{
			get
			{
				return _coords;
			}
		}

		/// <summary>
		/// Count of coord
		/// </summary>
		public int NumCoords
		{
			get
			{
				return _coords.NumCoords;
			}
		}


		#endregion

		/// <summary>
		/// Add a point and update the bounding extent
		/// </summary>
		/// <param name="point"></param>
		public void Add( Coord coord )
		{
			_coords.Add( coord );
			_isSimple = null;  // force re-evaluation if needed in the future
		}

		/// <summary>
		/// Add a point and update the bounding extent
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void Add( float x, float y )
		{
			_coords.Add( x, y );
			_isSimple = null;  // force re-evaluation if needed in the future
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="points"></param>
		public void Add( Coord[] coords )
		{
			_coords.Add( coords );
			_isSimple = null;  // force re-evaluation if needed in the future
		}

		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			_coords.Clear();
		}

		/// <summary>
		/// Taken from http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
		/// This is an expansion of the answer in the comp.graphics.algorithms FAQ question 2.03, How do I find if a point lies within a polygon? 
		/// Run a semi-infinite ray horizontally (increasing x, fixed y) out from the test point, and count how many edges it crosses. 
		/// At each crossing, the ray switches between inside and outside. This is called the Jordan curve theorem.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public bool Contains( Coord pt )
		{
			return _coords.Contains( pt );
		}


		public bool IsLine
		{
			get
			{
				if ( _coords.NumCoords < 2 )
				{
					throw new InvalidOperationException( "The curve has less than two points." );
				}

				return _coords.NumCoords == 2;
			}
		}
	}

}
