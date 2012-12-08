using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cobos.Geometry
{
	public abstract class Surface : Geometry
	{
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
