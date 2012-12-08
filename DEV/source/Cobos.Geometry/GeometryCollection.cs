using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cobos.Geometry
{
	public class GeometryCollection : Geometry
	{
		#region Instance data

		private Envelope _envelope = new Envelope( true );

		private readonly List<Geometry> _geometries;

		#endregion

		#region Construction

		public GeometryCollection()
		{
			_geometries = new List<Geometry>();
		}

		public GeometryCollection( int capacity )
		{
			_geometries = new List<Geometry>( capacity );
		}

		public GeometryCollection( IEnumerable<Geometry> geometries )
		{
			if ( geometries != null )
			{
				_geometries = new List<Geometry>( geometries );
				RecalculateExtent();
			}
		}

		#endregion
		
		#region OpenGIS implementation

		#region Geometry

		/// <summary>
		/// 
		/// </summary>
		public override Envelope Bounds
		{
			get { return _envelope; }
		}

		public override DimensionType Dimension
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsEmpty
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsSimple
		{
			get { throw new NotImplementedException(); }
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
			get { return "GEOMETRYCOLLECTION"; }
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

		#region GeometryCollection

		/// <summary>
		/// 
		/// </summary>
		public int NumGeometries
		{
			get
			{
				return _geometries.Count;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Geometry GeometryN( int n )
		{
			return _geometries[ n ];
		}

		/// <summary>
		/// 
		/// </summary>
		public ReadOnlyCollection<Geometry> Geometries
		{
			get
			{
				return _geometries.AsReadOnly();
			}
		}

		#endregion

		#endregion

		#region Collection methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="geometry"></param>
		public virtual void Add( Geometry geometry )
		{
			AssertValid( geometry );

			_geometries.Add( geometry );

			_envelope.Stretch( geometry.Bounds );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="geometries"></param>
		public virtual void Add( IEnumerable<Geometry> geometries )
		{
			AssertValid( geometries );

			_geometries.AddRange( geometries );

			foreach ( Geometry geometry in geometries )
			{
				_envelope.Stretch( geometry.Bounds );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Clear()
		{
			_geometries.Clear();
			_envelope.SetEmpty();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="index"></param>
		public virtual void Insert( Geometry geometry, int index )
		{
			AssertValid( geometry );

			_geometries.Insert( index, geometry );

			_envelope.Stretch( geometry.Bounds );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="geometries"></param>
		/// <param name="index"></param>
		public virtual void Insert( IEnumerable<Geometry> geometries, int index )
		{
			AssertValid( geometries );

			_geometries.InsertRange( index, geometries );

			foreach ( Geometry geometry in geometries )
			{
				_envelope.Stretch( geometry.Bounds );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public virtual void Remove( int index )
		{
			_geometries.RemoveAt( index );

			RecalculateExtent();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public virtual void Remove( int index, int count  )
		{
			_geometries.RemoveRange( index, count );

			RecalculateExtent();
		}

		public virtual void Remove( Predicate<Geometry> match )
		{
			_geometries.RemoveAll( match );

			RecalculateExtent();
		}

		#endregion

		#region Collection Validation

		protected virtual void AssertValid( Geometry geometry )
		{
			// NOP.  Override in derived classes to assert that the container has valid geometry.
		}

		protected virtual void AssertValid( IEnumerable<Geometry> geometries )
		{
			// NOP.  Override in derived classes to assert that the container has valid geometry.
		}

		#endregion

		#region Class internal methods

		/// <summary>
		/// If a change is made to the underlying List, then recalculate the bounding extent.
		/// Derived classes should call this method if the List<> class is accessed directly.
		/// </summary>
		protected void RecalculateExtent()
		{
			_envelope.SetEmpty();

			for ( int g = 0; g < _geometries.Count; ++g )
			{
				_envelope.Stretch( _geometries[ g ].Bounds );
			}
		}

		#endregion

	}
}
