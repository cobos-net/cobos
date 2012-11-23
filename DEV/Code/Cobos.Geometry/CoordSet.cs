using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cobos.Geometry
{
	/// <summary>
	/// Simple implementation of a curve (i.e. coordset) to include a bounding extent.
	/// This exposes the full functionality of the List class but if any base class
	/// methods are called that change the contents of the object, then the client
	/// must call RecalculateExtent to refresh the bounding extent.
	/// </summary>
	public sealed class CoordSet
	{
		#region Instance data

		Envelope _envelope = new Envelope( true );

		List<Coord> _coords;

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the System.Collections.Generic.List<T> class
		/// that is empty and has the default initial capacity.
		/// </summary>
		public CoordSet()
		{
			_coords = new List<Coord>();
		}
		
		/// <summary>
		/// Initializes a new instance of the System.Collections.Generic.List<T> class 
		/// that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">the number of elements that the new list can initially store.</param>
		public CoordSet( int capacity )
		{
			_coords = new List<Coord>( capacity );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coords"></param>
		public CoordSet( IEnumerable<Coord> coords )
		{
			_coords = new List<Coord>( coords );
		}

		#endregion

		#region Coordinate manipulation

		/// <summary>
		/// Add a point and update the bounding extent
		/// </summary>
		/// <param name="point"></param>
		public void Add( Coord point )
		{
			_coords.Add( point );
			_envelope.Stretch( point.X, point.Y );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="points"></param>
		public void Add( Coord[] points )
		{
			_coords.AddRange( points );

			Envelope env = new Envelope();

			for ( int p = 0; p < points.Length; ++p )
			{
				env.Stretch( points[ p ].X, points[ p ].Y );
			}

			_envelope.Stretch( env );
		}

		/// <summary>
		/// Add a point and update the bounding extent
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void Add( float x, float y )
		{
			_coords.Add( new Coord( x, y ) );
			_envelope.Stretch( x, y );
		}

		/// <summary>
		/// Add a number of coords and update the bounding extent
		/// </summary>
		/// <param name="point"></param>
		public void AddRange( IEnumerable<Coord> coords )
		{
			_coords.AddRange( coords );

			foreach ( Coord coord in coords )
			{
				_envelope.Stretch( coord.X, coord.Y );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			_coords.Clear();
			_envelope.SetEmpty();
		}

		/// <summary>
		/// Access the underlying coords without modifying the content
		/// </summary>
		public ReadOnlyCollection<Coord> Coords
		{
			get
			{
				return _coords.AsReadOnly();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Coord this[ int index ]
		{
			get
			{
				return _coords[ index ];
			}
		}

		/// <summary>
		/// Count of coord
		/// </summary>
		public int NumCoords
		{
			get
			{
				return _coords.Count;
			}
		}

		#endregion

		#region Bounding rectangle

		/// <summary>
		/// 
		/// </summary>
		public Envelope Bounds
		{
			get
			{
				return _envelope;
			}
		}

		/// <summary>
		/// Trivial accept/reject for testing point in polygon
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public bool IsInBounds( Coord pt )
		{
			return _envelope.Contains( pt );
		}

		/// <summary>
		/// If a change is made to the underlying List, then recalculate the bounding extent.
		/// Derived classes should call this method if the List<> class is accessed directly.
		/// </summary>
		private void RecalculateExtent()
		{
			_envelope.SetEmpty();

			for ( int p = 0; p < _coords.Count; ++p )
			{
				Coord point = _coords[ p ];
				_envelope.Stretch( point.X, point.Y );
			}
		}

		#endregion

		#region Geometry methods

		public bool IsClosed
		{
			get
			{
				if ( _coords.Count < 2 )
				{
					throw new InvalidOperationException( "The coordset has less than two points." );
				}

				return _coords[ 0 ] == _coords[ _coords.Count - 1 ];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsSelfIntersecting
		{
			get
			{
				throw new NotImplementedException();
			}
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
			bool isIn = false;

			if ( IsInBounds( pt ) )
			{
				int i, j = 0;
				for ( i = 0, j = _coords.Count - 1; i < _coords.Count; j = i++ )
				{
					Coord p0 = _coords[ i ];
					Coord p1 = _coords[ j ];

					if ( (((p0.Y <= pt.Y) && (pt.Y < p1.Y)) || ((p1.Y <= pt.Y) && (pt.Y < p0.Y)))
							&& (pt.X < (p1.X - p0.X) * (pt.Y - p0.Y) / (p1.Y - p0.Y) + p0.X) )
					{
						isIn = !isIn;
					}
				}
			}

			return isIn;
		}

		/// <summary>
		/// 
		/// </summary>
		public double Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public double Area
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Coord Centroid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
