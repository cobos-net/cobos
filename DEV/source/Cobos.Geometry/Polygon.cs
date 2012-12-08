using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.ObjectModel;

namespace Cobos.Geometry
{
	[Serializable]
	public class Polygon : Surface
	{
		#region Instance data

		protected LineString _exterior;

		protected List<LineString> _interiors;

		#endregion

		public Polygon()
		{
		}

		#region OpenGIS implementation

		#region Geometry

		/// <summary>
		/// A Surface is defined as a two-dimensional geometry.
		/// </summary>
		public override DimensionType Dimension
		{
			get 
			{ 
				return _exterior == null || _exterior.NumCoords == 0  ? DimensionType.Empty : DimensionType.Dim2d; 
			}
		}

		/// <summary>
		/// Coverage extent for the polygon
		/// </summary>
		public override Envelope Bounds
		{
			get
			{
				return _exterior.Bounds;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsEmpty
		{
			get 
			{
				if ( _exterior == null || _exterior.NumCoords == 0 )
				{
					return true;
				}
				return false;
			}
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
			get { return "POLYGON"; }
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

		#region Surface

		/// <summary>
		/// 
		/// </summary>
		public override double Area
		{
			get 
			{
				return _exterior.Coords.Area;
			}
		}

		public override Coord Centroid
		{
			get
			{
				return _exterior.Coords.Centroid;
			}
		}

		public override Coord PointOnSurface
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// 
		/// </summary>
		public LineString Exterior
		{
			get
			{
				return _exterior;
			}
		}

		#endregion

		#region Polygon

		public LineString ExteriorRing
		{
			get
			{
				return _exterior;
			}
		}

		public int NumInteriorRing
		{
			get
			{
				return _interiors.Count;
			}
		}

		public LineString InteriorRingN( int n )
		{
			return _interiors[ n ];
		}

		#endregion

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public bool Contains( Coord pt )
		{
			return _exterior.Contains( pt );
		}
	}
}
