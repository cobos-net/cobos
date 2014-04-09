// ----------------------------------------------------------------------------
// <copyright file="DateTimePrecise.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
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
        /// The internal System.Diagnostics.Stopwatch used by this instance.
        /// </summary>
        public readonly Stopwatch Timer;

        /// <summary>
        /// The frequency of the DateTime tick (100 nanoseconds).
        /// </summary>
        private const long ClockTickFrequency = 10000000;

        /// <summary>
        /// The elapsed time period in stop-watch ticks to re-synchronize.
        /// </summary>
        private long synchronizePeriodStopwatchTicks;

        /// <summary>
        /// The elapsed time period in clock ticks to re-synchronize.
        /// </summary>
        private long synchronizePeriodClockTicks;
        
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
        /// Gets an ISO compliant date/time stamp
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
            /// The last time observed.
            /// </summary>
            internal readonly DateTime Last;
            
            /// <summary>
            /// The baseline time.
            /// </summary>
            internal readonly DateTime Baseline;
            
            /// <summary>
            /// The number of ticks elapsed.
            /// </summary>
            internal readonly long TicksElapsed;
            
            /// <summary>
            /// The StopWatch frequency.
            /// </summary>
            internal readonly long Frequency;

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
        }
    }
}
