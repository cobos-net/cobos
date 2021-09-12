// ----------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="DateTime"/> class.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Extension method to get the Unix time from a DateTime object.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The unix seconds.</returns>
        public static long ToEpochSeconds(this DateTime self)
        {
            DateTime objUTC = self.ToUniversalTime();
            return (objUTC.Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Convert Unix time to a DateTime object.
        /// </summary>
        /// <param name="epoch">The epoch seconds.</param>
        /// <returns>The DateTime representation.</returns>
        public static DateTime FromEpochSeconds(long epoch)
        {
            return new DateTime((epoch * 10000000) + 621355968000000000);
        }

        /// <summary>
        /// Gets the date for a number of business days past a date.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="numberOfBusinessDays">The number of business days from the date.</param>
        /// <returns>The date for a number of business days past the original date.</returns>
        /// <remarks>
        /// A business day is considered to be any day excluding Saturday and Sunday.
        /// This does not take into account public holidays.
        /// </remarks>
        public static DateTime GetBusinessDaysFrom(this DateTime self, int numberOfBusinessDays)
        {
            DateTime result = self;

            for (int i = 0; i < numberOfBusinessDays; ++i)
            {
                result = result.AddDays(1);
                DayOfWeek dow = result.DayOfWeek;

                while (dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday)
                {
                    result = result.AddDays(1);
                    dow = result.DayOfWeek;
                }
            }

            return result;
        }

        /// <summary>
        /// Truncate a DateTime value.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="timeSpan">The timespan value to truncate.</param>
        /// <returns>The truncated DateTime object.</returns>
        /// <example>
        /// <para>
        /// dateTime = dateTime.Truncate(TimeSpan.FromMilliseconds(1)); // Truncate to whole millisecond
        /// dateTime = dateTime.Truncate(TimeSpan.FromSeconds(1)); // Truncate to whole second
        /// dateTime = dateTime.Truncate(TimeSpan.FromMinutes(1)); // Truncate to whole minute.
        /// </para>
        /// <c>http://stackoverflow.com/questions/1004698/how-to-truncate-milliseconds-off-of-a-net-datetime</c>.
        /// </example>
        public static DateTime Truncate(this DateTime self, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return self;
            }

            return self.AddTicks(-(self.Ticks % timeSpan.Ticks));
        }

        /// <summary>
        /// Convenience method to truncate to a specified time interval.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="truncate">The time interval to truncate to.</param>
        /// <returns>The truncated DateTime object.</returns>
        public static DateTime Truncate(this DateTime self, DateTimeTruncateTo truncate)
        {
            switch (truncate)
            {
                case DateTimeTruncateTo.Millisecond:
                    return Truncate(self, TimeSpan.FromMilliseconds(1));

                case DateTimeTruncateTo.Second:
                    return Truncate(self, TimeSpan.FromSeconds(1));

                case DateTimeTruncateTo.Minute:
                    return Truncate(self, TimeSpan.FromMinutes(1));

                default:
                    throw new ArgumentException("Unsupported value of DateTimeTruncateTo");
            }
        }
    }
}
