using System;
using System.Collections.Generic;
using System.Text;

namespace Cobos.Geometry
{
	public abstract class MultiSurface : GeometryCollection
	{
		#region Construction

		protected MultiSurface()
			: base()
		{
		}

		protected MultiSurface( int capacity )
			: base( capacity )
		{
		}

		protected MultiSurface( IEnumerable<Geometry> geometries )
			: base( geometries )
		{
		}

		#endregion

		#region OpenGIS implementation

		public abstract double Area
		{
			get;
		}

		public abstract Coord Centroid
		{
			get;
		}

		public abstract Coord PointOnSurface
		{
			get;
		}

		#endregion
	}
}
