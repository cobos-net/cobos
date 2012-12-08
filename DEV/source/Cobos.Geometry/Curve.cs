using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Cobos.Geometry
{
	/// <summary>
	/// Simple implementation of a curve (i.e. coordset) to include a bounding extent.
	/// This exposes the full functionality of the List class but if any base class
	/// methods are called that change the contents of the object, then the client
	/// must call RecalculateExtent to refresh the bounding extent.
	/// </summary>
	[Serializable]
	public abstract class Curve : Geometry
	{
		#region OpenGIS implementation

		public abstract Coord StartPoint
		{
			get;
		}

		public abstract Coord EndPoint
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract bool? IsClosed
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract bool IsRing
		{
			get;
		}


		/// <summary>
		/// 
		/// </summary>
		public abstract double Length
		{
			get;
		}

		#endregion
	}
}
