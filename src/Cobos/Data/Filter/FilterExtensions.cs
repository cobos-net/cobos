// <copyright file="FilterExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>

namespace Cobos.Data.Filter
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for filters and predicates.
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        /// Convert an enumerable to a values list.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <returns>A values list.</returns>
        public static PropertyIsInList.ValuesType ToPropertyIsInListValues(this IEnumerable<string> values)
        {
            var result = new PropertyIsInList.ValuesType();
            result.AddRange(values);
            return result;
        }

        /// <summary>
        /// Convert an enumerable to a values list.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <returns>A values list.</returns>
        public static PropertyIsInList.ValuesType ToPropertyIsInListValues(this IEnumerable<object> values)
        {
            var result = new PropertyIsInList.ValuesType();
            result.AddRange(values.Select(v => v.ToString()));
            return result;
        }
    }
}
