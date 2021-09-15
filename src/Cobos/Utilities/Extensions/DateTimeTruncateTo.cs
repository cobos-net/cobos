// ----------------------------------------------------------------------------
// <copyright file="DateTimeTruncateTo.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Extensions
{
    /// <summary>
    /// Enumeration to indicate how to truncate a DateTime
    /// using DateTimeExtensions.Truncate.
    /// </summary>
    /// <example>
    /// dateTime = dateTime.Truncate(DateTimeTruncateTo.Millisecond).
    /// </example>
    public enum DateTimeTruncateTo
    {
        /// <summary>
        /// Truncate to millisecond.
        /// </summary>
        Millisecond,

        /// <summary>
        /// Truncate to second.
        /// </summary>
        Second,

        /// <summary>
        /// Truncate to minute.
        /// </summary>
        Minute,
    }
}
