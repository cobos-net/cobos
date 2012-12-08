using System;
using System.Collections.Generic;
using System.Linq;

namespace Cobos.Geometry
{
	public class MultiLineString : MultiCurve
	{
		#region Construction

		public MultiLineString()
			: base()
		{
		}

		public MultiLineString( int capacity )
			: base( capacity )
		{
		}

		public MultiLineString( IEnumerable<Geometry> geometries )
			: base( geometries.Count() )
		{
			if ( geometries != null )
			{
				foreach ( Geometry geometry in geometries )
				{
					if ( !(geometry is LineString) )
					{
						throw new ArgumentException( "Cannot create a MultiLineString containing an object of type: " + geometry.GetType().Name );
					}
				}
			}

			base.Add( geometries );
		}

		#endregion

		#region OpenGIS implementation

		#region Geometry

		public override string GeometryType
		{
			get { return "MULTILINESTRING"; }
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

		#region MultiCurve

		public override bool IsClosed
		{
			get { throw new NotImplementedException(); }
		}

		public override double Length
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region Extension methods

		public int NumLineStrings
		{
			get
			{
				return NumGeometries;
			}
		}

		public LineString LineStringN( int n )
		{
			return GeometryN( n ) as LineString;
		}

		#endregion

		#endregion

		#region Collection validation

		protected override void AssertValid( Geometry geometry )
		{
			if ( !(geometry is LineString) )
			{
				throw new ArgumentException( "MultiLineString cannot contain an object of type: " + geometry.GetType().Name );
			}
		}

		protected override void AssertValid( IEnumerable<Geometry> geometries )
		{
			if ( geometries != null )
			{
				foreach ( Geometry geometry in geometries )
				{
					if ( !(geometry is LineString) )
					{
						throw new ArgumentException( "MultiLineString cannot contain an object of type: " + geometry.GetType().Name );
					}
				}
			}
		}

		#endregion
	}
}
