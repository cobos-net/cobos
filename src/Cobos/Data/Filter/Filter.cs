// ----------------------------------------------------------------------------
// <copyright file="Filter.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    using Cobos.Utilities.Xml;

    /// <summary>
    /// Partial specification and implementation of <see cref="Filter"/>.
    /// </summary>
    public partial class Filter
    {
        /// <summary>
        /// Deserialize a filter from a string representation.
        /// </summary>
        /// <param name="filter">The string representation of the filter.</param>
        /// <returns>The deserialized entity.</returns>
        public static Filter Deserialize(string filter)
        {
            return DataContractHelper.Deserialize<Filter>(filter, null);
        }
    }
}
