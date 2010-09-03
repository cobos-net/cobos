using System;
using System.Diagnostics;

namespace Intergraph.Oz.Utilities.Diagnostics
{
	/// <summary>
	/// DateTimePrecise provides a way to get a DateTime that exhibits the
	/// relative precision of
	/// System.Diagnostics.Stopwatch, and the absolute accuracy of DateTime.Now.
	/// 
	/// Courtesy of James Brock (http://www.codeproject.com/KB/cs/DateTimePrecise.aspx)
	/// </summary>
	public class DateTimePrecise
	{
		/// <summary>
		/// Creates a new instance of DateTimePrecise.
		/// A large value of synchronizePeriodSeconds may cause arithmetic overthrow
		/// exceptions to be thrown. A small value may cause the time to be unstable.
		/// A good value is 10.
		/// synchronizePeriodSeconds = The number of seconds after which the
		/// DateTimePrecise will synchronize itself with the system clock.
		/// </summary>
		/// <param name="synchronizePeriodSeconds"></param>
		public DateTimePrecise( long synchronizePeriodSeconds )
		{
			Stopwatch = Stopwatch.StartNew();
			this.Stopwatch.Start();

			DateTime t = DateTime.UtcNow;
			_immutable = new DateTimePreciseSafeImmutable( t, t, Stopwatch.ElapsedTicks, Stopwatch.Frequency );

			_synchronizePeriodSeconds = synchronizePeriodSeconds;
			_synchronizePeriodStopwatchTicks = synchronizePeriodSeconds * Stopwatch.Frequency;
			_synchronizePeriodClockTicks = synchronizePeriodSeconds * _clockTickFrequency;
		}

		/// <summary>
		/// Returns the current date and time, just like DateTime.UtcNow.
		/// </summary>
		public DateTime UtcNow
		{
			get
			{
				long s = this.Stopwatch.ElapsedTicks;
				DateTimePreciseSafeImmutable immutable = _immutable;

				if ( s < immutable._s_observed + _synchronizePeriodStopwatchTicks )
				{
					return immutable._t_base.AddTicks( ((s - immutable._s_observed) * _clockTickFrequency) / (immutable._stopWatchFrequency) );
				}
				else
				{
					DateTime t = DateTime.UtcNow;

					DateTime t_base_new = immutable._t_base.AddTicks( (( s - immutable._s_observed) * _clockTickFrequency) / (immutable._stopWatchFrequency) );

					_immutable = new DateTimePreciseSafeImmutable( t, t_base_new, s,
																					((s - immutable._s_observed) * _clockTickFrequency * 2) /
																					(t.Ticks - immutable._t_observed.Ticks + t.Ticks + t.Ticks - t_base_new.Ticks - immutable._t_observed.Ticks) );

					return t_base_new;
				}
			}
		}

		/// <summary>
		/// Returns the current date and time, just like DateTime.Now.
		/// </summary>
		public DateTime Now
		{
			get
			{
				return this.UtcNow.ToLocalTime();
			}
		}

		/// <summary>
		/// Get an ISO compliant date/time stamp
		/// </summary>
		public string Iso8601
		{
			get
			{
				return this.UtcNow.ToLocalTime().ToString( "yyyy-MM-ddTHH:mm:ss.fffffffzzz" );
			}
		}

		/// The internal System.Diagnostics.Stopwatch used by this instance.
		public Stopwatch Stopwatch;

		private long _synchronizePeriodStopwatchTicks;
		private long _synchronizePeriodSeconds;
		private long _synchronizePeriodClockTicks;
		private const long _clockTickFrequency = 10000000;
		private DateTimePreciseSafeImmutable _immutable;
	}

	internal sealed class DateTimePreciseSafeImmutable
	{
		internal DateTimePreciseSafeImmutable( DateTime t_observed, DateTime t_base, long s_observed, long stopWatchFrequency )
		{
			_t_observed = t_observed;
			_t_base = t_base;
			_s_observed = s_observed;
			_stopWatchFrequency = stopWatchFrequency;
		}
		internal readonly DateTime _t_observed;
		internal readonly DateTime _t_base;
		internal readonly long _s_observed;
		internal readonly long _stopWatchFrequency;
	}
}
