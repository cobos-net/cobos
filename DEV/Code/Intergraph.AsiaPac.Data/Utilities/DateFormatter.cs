using System;
using System.Text;
using System.Text.RegularExpressions;
using Intergraph.AsiaPac.Data;
using Properties = Intergraph.AsiaPac.Data.Properties;

namespace Intergraph.IPS.Utility
{
	/// <summary>
	/// Summary description for DateFormatter.
	/// </summary>
	public static class DateFormatter
	{
		private static readonly Regex _parseDTS = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})(?<timezone>\S{2})", RegexOptions.Compiled);
		private const string DtsFormat = "yyyyMMddHHmmss";
		private const string UtcSuffix = "UT";
		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Key for use in AppDomain.GetData and AppDomain.SetData.
		/// This provides access to a boolean value that affects the
		/// string format of the date time stamps.
		/// </summary>
		/// <remarks>
		/// If the value stored is true, then the format of the strings
		/// will be UTC.  The default of false will leave the strings
		/// in local time.
		/// </remarks>
		public const string UseUtcKey = "DateFormatter.UseUtc";

		private static string DaylightSuffix
		{
			get
			{
				string suffix = TimeZone.CurrentTimeZone.DaylightName.Substring(0, 2);
				if (suffix == TimeZone.CurrentTimeZone.StandardName.Substring(0, 2))
					suffix = suffix.Substring(0, 1) + 'D';
				return suffix;
			}
		}

		private static string StandardSuffix
		{
			get
			{
				string suffix = TimeZone.CurrentTimeZone.StandardName.Substring(0, 2);
				if (suffix == TimeZone.CurrentTimeZone.DaylightName.Substring(0, 2))
					suffix = suffix.Substring(0, 1) + 'S';
				return suffix;
			}
		}

		private static bool UsingUtc
		{
			get
			{
					 object useUtc = AppDomain.CurrentDomain.GetData(UseUtcKey);
					 if (useUtc != null)
					 {
						  return Convert.ToBoolean(useUtc);
					 }
					 else
					 {
						  throw new InvalidOperationException(Properties.Resources.UtcUninitialized);
					 }
			}
		}

		/// <summary>
		/// Converts a CAD date time stamp into a DateTime object.
		/// </summary>
		/// <remarks>This function will convert the CAD time into the correct local time
		/// </remarks>
		/// <param name="dts">Stirng containing date time stamp</param>
		/// <returns>The date and time in a DateTime structure</returns>
		public static DateTime ConvertFromDTS(string dts)
		{
			if(dts.Length != 16)
				return DateTime.MinValue;
			// Regex match
			Match  match = _parseDTS.Match(dts);
			DateTime time = new DateTime(Convert.ToInt32(match.Groups["year"].Value),
				Convert.ToInt32(match.Groups["month"].Value),
				Convert.ToInt32(match.Groups["day"].Value),
				Convert.ToInt32(match.Groups["hour"].Value),
				Convert.ToInt32(match.Groups["minute"].Value),
				Convert.ToInt32(match.Groups["second"].Value));

				if (match.Groups["timezone"].Value == UtcSuffix)
					 time = DateTime.SpecifyKind(time, DateTimeKind.Utc);
				else
					 time = DateTime.SpecifyKind(time, DateTimeKind.Local);
				return time.ToLocalTime();
		}

		/// <summary>
		/// Converts a CAD date time stamp into a Utc DateTime object.
		/// </summary>
		/// <remarks>At present this function does not take into account different timeszones.
		/// </remarks>
		/// <param name="dts">Stirng containing date time stamp</param>
		/// <returns>The date and time in a DateTime structure</returns>
		public static DateTime ConvertFromDTSUtc(string dts)
		{
			if(dts.Length != 16)
				return DateTime.MinValue;
			// Regex match
			Match  match = _parseDTS.Match(dts);
			DateTime time = new DateTime(Convert.ToInt32(match.Groups["year"].Value),
				Convert.ToInt32(match.Groups["month"].Value),
				Convert.ToInt32(match.Groups["day"].Value),
				Convert.ToInt32(match.Groups["hour"].Value),
				Convert.ToInt32(match.Groups["minute"].Value),
				Convert.ToInt32(match.Groups["second"].Value));

				if (match.Groups["timezone"].Value == UtcSuffix)
					 time = DateTime.SpecifyKind(time, DateTimeKind.Utc);
				else
					 time = DateTime.SpecifyKind(time, DateTimeKind.Local);
				return time.ToUniversalTime();
		}
		
		/// <summary>
		/// Converts a date value in epoch seconds to a local DateTime object
		/// </summary>
		/// <param name="epochSeconds">Int32 containing a date in epoch seconds</param>
		/// <returns>The data and time in a DateTime structure</returns>
		public static DateTime ConvertFromEpochSeconds(int epochSeconds)
		{
			if ( epochSeconds <= 0 )
			{
				return DateTime.MinValue;
			}
			else
			{
				return _epoch.AddSeconds(epochSeconds).ToLocalTime();
			}
		}

		/// <summary>
		/// Converts a date value in epoch seconds to a Utc DateTime object
		/// </summary>
		/// <param name="epochSeconds">Int32 containing a date in epoch seconds</param>
		/// <returns>The data and time in a DateTime structure</returns>
		public static DateTime ConvertFromEpochSecondsUtc(int epochSeconds)
		{
			if ( epochSeconds <= 0 )
			{
				return DateTime.MinValue;
			}
			else
			{
				return _epoch.AddSeconds(epochSeconds);
			}
		}
	

		/// <summary>
		/// Converts a date value in epoch seconds to a DateTime object
		/// </summary>
		/// <param name="epochSeconds">string containing a date in epoch seconds</param>
		/// <returns>The data and time in a DateTime structure</returns>
		public static DateTime ConvertFromEpochSeconds(string epochSeconds)
		{
			if ( epochSeconds.Length == 0 )
				return DateTime.MinValue;
			else
				return DateFormatter.ConvertFromEpochSeconds(Convert.ToInt32(epochSeconds));
		}

		/// <summary>
		/// Converts a date value in epoch seconds to a Utc DateTime object
		/// </summary>
		/// <param name="epochSeconds">string containing a date in epoch seconds</param>
		/// <returns>The data and time in a DateTime structure</returns>
		public static DateTime ConvertFromEpochSecondsUtc(string epochSeconds)
		{
			if ( epochSeconds.Length == 0 )
				return DateTime.MinValue;
			else
				return DateFormatter.ConvertFromEpochSecondsUtc(Convert.ToInt32(epochSeconds));
		}

		/// <summary>
		/// Converts a DateTime structure to CAD DTS format
		/// </summary>
		/// <param name="time">The DateTime structure to convert</param>
		/// <returns>The CAD date time stramp</returns>
		public static string ConvertToDTS(DateTime time)
		{
				DateTime dbTime;
			if ( time == DateTime.MinValue )
			{
				return string.Empty;
			}
				else if (UsingUtc)
				{
					 if (time.Kind == DateTimeKind.Unspecified)
					 {
						  time = DateTime.SpecifyKind(time, DateTimeKind.Utc);
					 }
					 dbTime = time.ToUniversalTime();
					 return dbTime.ToString(DtsFormat) + UtcSuffix;
				}
				else
				{
					 if (time.Kind == DateTimeKind.Unspecified)
					 {
						  time = DateTime.SpecifyKind(time, DateTimeKind.Local);
					 }
					 dbTime = time.ToLocalTime();
					 string dts = dbTime.ToString(DtsFormat);
					 if (TimeZone.IsDaylightSavingTime(dbTime, TimeZone.CurrentTimeZone.GetDaylightChanges(time.Year)))
						  dts += DaylightSuffix;
					 else
						  dts += StandardSuffix;
					 return dts;
				}
		}

		/// <summary>
		/// Converts a local DateTime structure to 24H HH:mm
		/// </summary>
		/// <param name="time">The local DateTime structure to convert</param>
		/// <returns>The timestamp in 24H HH:mm format</returns>
		public static string ConvertToTS(DateTime time)
		{
			if (time == DateTime.MinValue)
				return string.Empty;
			else
			{
				return time.ToString("HH:mm");
			}
		}

		/// <summary>
		/// Converts a locate time into a TimeSpan that represents the
		/// fraction of the day elapsed since midnight.
		/// </summary>
		/// <param name="time">A time in HH:mm format</param>
		/// <returns>A TimeSpan measured since midnight local time</returns>
		public static TimeSpan ConvertFromTS(string time)
		{
			return new TimeSpan(Convert.ToInt32(time.Substring(0, 2)), Convert.ToInt32(time.Substring(3, 2)), 0);
		}
		
		/// <summary>
		/// Converts a DateTime structure to epoch seconds
		/// </summary>
		/// <param name="time">The DateTime structure to convert</param>
		/// <returns>The date and time in epoch seconds</returns>
		public static long ConvertToEpochSeconds(DateTime time)
		{
			if ( time == DateTime.MinValue )
			{
				return 0;
			}
				if (time.Kind == DateTimeKind.Unspecified)
				{
					 if (UsingUtc)
						  time = DateTime.SpecifyKind(time, DateTimeKind.Utc);
					 else
						  time = DateTime.SpecifyKind(time, DateTimeKind.Local);
				}
			return Convert.ToInt64((time.ToUniversalTime() - _epoch).TotalSeconds);
		}

		/// <summary>
		/// Converts a CAD day of the week to the corresponding .NET DayOfWeek enum
		/// </summary>
		/// <param name="dow">CAD Day of Week value</param>
		/// <returns>.NET DayOfWeek value</returns>
		public static DayOfWeek ParseCADDayOfWeek(int dow)
		{
			return (DayOfWeek) (--dow);
		}

		/// <summary>
		/// Converts a CAD day of the week to the corresponding .NET DayOfWeek enum
		/// </summary>
		/// <param name="dow">CAD Day of Week value as a string</param>
		/// <returns>.NET DayOfWeek value</returns>
		public static DayOfWeek ParseCADDayOfWeek(string dow)
		{
			return ParseCADDayOfWeek(Convert.ToInt32(dow));
		}

		/// <summary>
		/// Converts the .NET DayOfWeek enum value to corresponding CAD value
		/// </summary>
		/// <param name="day">.NET DayOfWeek value</param>
		/// <returns>CAD value</returns>
		public static int GetCADDayOfWeek(DayOfWeek day)
		{
			int dow = (int) day;
			return ++dow;
		}
	}
}
