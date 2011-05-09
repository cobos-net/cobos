using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Utilities.Extensions
{
	public static class DateExtensions
	{
		/// <summary>
		/// Extension method to get the Unix time from a DateTime object
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static long ToEpochSeconds( this DateTime datetime )
		{
			DateTime objUTC = datetime.ToUniversalTime();
			return (objUTC.Ticks - 621355968000000000) / 10000;	
		}

		/// <summary>
		/// Convert Unix time to a DateTime object
		/// </summary>
		/// <param name="epoch"></param>
		/// <returns></returns>
		public static DateTime FromEpochSeconds( long epoch )
		{
			return new DateTime( ((epoch * 10000000) + 621355968000000000) );
		}
	}
}
