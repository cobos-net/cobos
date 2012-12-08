using System;
using System.Collections.Generic;
using System.Text;

namespace Cobos.Geometry
{
	public abstract class MultiCurve : GeometryCollection
	{
		#region Construction

		protected MultiCurve()
			: base()
		{
		}

		protected MultiCurve( int capacity )
			: base( capacity )
		{
		}

		protected MultiCurve( IEnumerable<Geometry> geometries )
			: base( geometries )
		{
		}

		#endregion

		#region OpenGIS implementation

		public abstract bool IsClosed
		{
			get;
		}

		public abstract double Length
		{
			get;
		}

		#endregion

	}
}
