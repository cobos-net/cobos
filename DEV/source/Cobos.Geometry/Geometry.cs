using System;
using System.Collections.Generic;
using System.Text;

namespace Cobos.Geometry
{
	[Serializable]
	public abstract class Geometry
	{
		/// <summary>
		/// 
		/// </summary>
		public int SRID
		{
			get;
			set;
		}

		/// <summary>
		/// A geometry can have a dimension of –1, 0, 1, or 2:
		/// –1 for an empty geometry.
		/// 0 for a geometry with no length and no area.
		/// 1 for a geometry with non-zero length and zero area.
		/// 2 for a geometry with non-zero area.
		/// </summary>
		public enum DimensionType
		{
			Empty = -1,
			Dim0d,
			Dim1d,
			Dim2d
 		}

		/// <summary>
		/// 
		/// </summary>
		public abstract DimensionType Dimension
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract bool IsSimple
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract bool IsEmpty
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract Geometry Boundary
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract Geometry Envelope
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract string GeometryType
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract string AsText
		{
			get;
		}

		public abstract byte[] AsBinary
		{
			get;
		}

		/// <summary>
		/// Non standard OpenGIS extension.  More efficient method of querying the minimum 
		/// bounding rectangle than provided by the Envelope property.
		/// 
		/// Can use Geometry.Bounds.IsInBounds( point ) as a trivial accept/reject for locating geometries.
		/// </summary>
		public abstract Envelope Bounds
		{
			get;
		}
	}
}
