using System;
using System.Collections.Generic;
using System.Text;

namespace Cobos.Geometry
{
	public struct Segment
	{
		/// <summary>
		/// 
		/// </summary>
		public Coord P0;

		/// <summary>
		/// 
		/// </summary>
		public Coord P1;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p0"></param>
		/// <param name="p1"></param>
		public Segment( Coord p0, Coord p1 )
			: this()
		{
			P0 = p0;
			P1 = p1;
		}

		/// <summary>
		/// 
		/// </summary>
		public Envelope Envelope
		{
			get
			{
				return new Envelope( P0, P1 );
			}
		}
	}
}
