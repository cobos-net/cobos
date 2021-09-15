// ----------------------------------------------------------------------------
// <copyright file="DateTimePrecise.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Diagnostics
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// DateTimePrecise provides a way to get a DateTime that exhibits the
    /// relative precision of System.Diagnostics.Stopwatch, and the absolute
    /// accuracy of DateTime.Now.
    /// <para>
    /// Courtesy of James Brock <c>(http://www.codeproject.com/KB/cs/DateTimePrecise.aspx)</c>.
    /// </para>
    /// </summary>
    public class DateTimePrecise
    {
        /// <summary>
        /// The frequency of the DateTime tick (100 nanoseconds).
        /// </summary>
        private const long ClockTickFrequency = 10000000;

        /// <summary>
        /// The elapsed time period in stop-watch ticks to re-synchronize.
        /// </summary>
        private readonly long synchronizePeriodStopwatchTicks;

        /// <summary>
        /// The elapsed time period in clock ticks to re-synchronize.
        /// </summary>
#pragma warning disable IDE0052 // Remove unread private members
        private readonly long synchronizePeriodClockTicks;
#pragma warning restore IDE0052 // Remove unread private members

        /// <summary>
        /// The internal counter for synchronizing.
        /// </summary>
        private Counter counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimePrecise"/> class.
        /// </summary>
        /// <param name="synchronizePeriodSeconds">The number of seconds after which the DateTimePrecise will synchronize itself with the system clock.</param>
        /// <remarks>
        /// A large value of synchronizePeriodSeconds may cause arithmetic overflow
        /// exceptions to be thrown. A small value may cause the time to be unstable.
        /// A good value is 10.
        /// </remarks>
        public DateTimePrecise(long synchronizePeriodSeconds)
        {
            this.Timer = Stopwatch.StartNew();
            this.Timer.Start();

            DateTime now = DateTime.UtcNow;
            this.counter = new Counter(now, now, this.Timer.ElapsedTicks, Stopwatch.Frequency);

            this.synchronizePeriodStopwatchTicks = synchronizePeriodSeconds * Stopwatch.Frequency;
            this.synchronizePeriodClockTicks = synchronizePeriodSeconds * ClockTickFrequency;
        }

        /// <summary>
        /// Gets the internal System.Diagnostics.Stopwatch used by this instance.
        /// </summary>
        public Stopwatch Timer { get; }

        /// <summary>
        /// Gets the current date and time, just like <c>DateTime.UtcNow</c>.
        /// </summary>
        public DateTime UtcNow
        {
            get
            {
                long elapsedTicks = this.Timer.ElapsedTicks;

                if (elapsedTicks < this.counter.TicksElapsed + this.synchronizePeriodStopwatchTicks)
                {
                    return this.counter.Baseline.AddTicks(((elapsedTicks - this.counter.TicksElapsed) * ClockTickFrequency) / this.counter.Frequency);
                }
                else
                {
                    DateTime now = DateTime.UtcNow;

                    DateTime newBaseline = this.counter.Baseline.AddTicks(((elapsedTicks - this.counter.TicksElapsed) * ClockTickFrequency) / this.counter.Frequency);

                    this.counter = new Counter(
                                            now,
                                            newBaseline,
                                            elapsedTicks,
                                            ((elapsedTicks - this.counter.TicksElapsed) * ClockTickFrequency * 2) / (now.Ticks - this.counter.Last.Ticks + now.Ticks + now.Ticks - newBaseline.Ticks - this.counter.Last.Ticks));

                    return newBaseline;
                }
            }
        }

        /// <summary>
        /// Gets the current date and time, just like DateTime.Now.
        /// </summary>
        public DateTime Now
        {
            get
            {
                return this.UtcNow.ToLocalTime();
            }
        }

        /// <summary>
        /// Gets an ISO compliant date/time stamp.
        /// </summary>
        public string Iso8601
        {
            get
            {
                return this.UtcNow.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
            }
        }

        /// <summary>
        /// Internal helper class to maintain a count.
        /// </summary>
        internal sealed class Counter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Counter"/> class.
            /// </summary>
            /// <param name="lastTime">The last time observed.</param>
            /// <param name="baseline">The baseline time.</param>
            /// <param name="ticksObserved">The number of ticks observed.</param>
            /// <param name="frequency">The StopWatch frequency.</param>
            internal Counter(DateTime lastTime, DateTime baseline, long ticksObserved, long frequency)
            {
                this.Last = lastTime;
                this.Baseline = baseline;
                this.TicksElapsed = ticksObserved;
                this.Frequency = frequency;
            }

            /// <summary>
            /// Gets the last time observed.
            /// </summary>
            internal DateTime Last { get; }

            /// <summary>
            /// Gets the baseline time.
            /// </summary>
            internal DateTime Baseline { get; }

            /// <summary>
            /// Gets the number of ticks elapsed.
            /// </summary>
            internal long TicksElapsed { get; }

            /// <summary>
            /// Gets the StopWatch frequency.
            /// </summary>
            internal long Frequency { get; }
        }
    }
}
