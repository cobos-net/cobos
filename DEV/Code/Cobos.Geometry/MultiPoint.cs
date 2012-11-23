using System;
using System.Collections.Generic;
using System.Linq;

namespace Cobos.Geometry
{
	public class MultiPoint : GeometryCollection
	{
		#region Construction

		public MultiPoint()
			: base()
		{
		}

		public MultiPoint( int capacity )
			: base( capacity )
		{
		}

		public MultiPoint( IEnumerable<Geometry> geometries )
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
			get { return "MULTIPOINT"; }
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


		#region Extension methods

		public int NumPoints
		{
			get
			{
				return NumGeometries;
			}
		}

		public Point PointN( int n )
		{
			return GeometryN( n ) as Point;
		}

		#endregion

		#endregion

		#region Collection validation

		protected override void AssertValid( Geometry geometry )
		{
			if ( !(geometry is Point) )
			{
				throw new ArgumentException( "MultiPoint cannot contain an object of type: " + geometry.GetType().Name );
			}
		}

		protected override void AssertValid( IEnumerable<Geometry> geometries )
		{
			if ( geometries != null )
			{
				foreach ( Geometry geometry in geometries )
				{
					if ( !(geometry is Point) )
					{
						throw new ArgumentException( "MultiPoint cannot contain an object of type: " + geometry.GetType().Name );
					}
				}
			}
		}

		#endregion
	}
}
