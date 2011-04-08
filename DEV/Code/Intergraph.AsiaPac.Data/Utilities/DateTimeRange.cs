using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Intergraph.IPS.Utility;

namespace Intergraph.AsiaPac.Data.Utilities
{
	public partial class DateTimeRange
	{
		/// <summary>
		/// Default constructor for serialization
		/// </summary>
		public DateTimeRange()
		{
		}

		/// <summary>
		/// Construct from nullable types
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		public DateTimeRange( DateTime? startDate, DateTime? endDate )
		{
			StartDateSpecified = startDate.HasValue;

			if ( StartDateSpecified )
			{
				StartDate = startDate.Value;
			}

			EndDateSpecified = endDate.HasValue;

			if ( EndDateSpecified )
			{
				EndDate = endDate.Value;
			}

			// check the dates are specified in the right order
			if ( StartDateSpecified && EndDateSpecified )
			{
				if ( EndDate < StartDate )
				{
					DateTime temp = EndDate;
					EndDate = StartDate;
					StartDate = temp;
				}
			}
		}

		/// <summary>
		/// Check that this has any data
		/// </summary>
		public bool HasValue
		{
			get
			{
				return StartDateSpecified || EndDateSpecified;
			}
		}

		/// <summary>
		/// Convert the date time range to a CAD DTS where clause comparison
		/// </summary>
		/// <param name="dbColumn"></param>
		/// <returns></returns>
		public string ToString( string dbColumn )
		{
			StringBuilder s = new StringBuilder( 64 );

			if ( StartDateSpecified )
			{
				s.Append( dbColumn );
				s.Append( " >= '" );
				s.Append( DateFormatter.ConvertToDTS( StartDate ) );
				s.Append( "'" );
			}

			if ( EndDateSpecified )
			{
				if ( StartDateSpecified )
				{
					s.Append( " AND " );
				}

				s.Append( dbColumn );
				s.Append( " <= '" );
				s.Append( DateFormatter.ConvertToDTS( EndDate ) );
				s.Append( "'" );
			}

			return s.ToString();
		}
	}
}