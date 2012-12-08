using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Cobos.Geometry
{
	public class MultiPolygon : MultiSurface
	{
		#region Construction

		public MultiPolygon()
			: base()
		{
		}

		public MultiPolygon( int capacity )
			: base( capacity )
		{
		}

		public MultiPolygon( IEnumerable<Geometry> geometries )
			: base( geometries.Count() )
		{
			AssertValid( geometries );

			base.Add( geometries );
		}

		#endregion

		#region OpenGIS implementation

		#region Geometry

		public override string GeometryType
		{
			get { return "MULTIPOLYGON"; }
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

		#region MultiSurface

		public override double Area
		{
			get { throw new NotImplementedException(); }
		}

		public override Coord Centroid
		{
			get { throw new NotImplementedException(); }
		}

		public override Coord PointOnSurface
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region Extension methods

		public int NumPolygons
		{
			get
			{
				return NumGeometries;
			}
		}

		public Polygon PolygonN( int n )
		{
			return GeometryN( n ) as Polygon;
		}

		#endregion

		#endregion



		#region Collection validation

		protected override void AssertValid( Geometry geometry )
		{
			if ( !(geometry is Polygon) )
			{
				throw new ArgumentException( "MultiPolygon cannot contain an object of type: " + geometry.GetType().Name );
			}
		}

		protected override void AssertValid( IEnumerable<Geometry> geometries )
		{
			if ( geometries != null )
			{
				foreach ( Geometry geometry in geometries )
				{
					if ( !(geometry is Polygon) )
					{
						throw new ArgumentException( "MultiPolygon cannot contain an object of type: " + geometry.GetType().Name );
					}
				}
			}
		}

		#endregion

	}
}
