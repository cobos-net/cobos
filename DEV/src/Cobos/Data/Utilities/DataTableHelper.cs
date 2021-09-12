// ----------------------------------------------------------------------------
// <copyright file="DataTableHelper.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Utilities
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Class specification and implementation of <see cref="DataTableHelper"/>.
    /// </summary>
    public static class DataTableHelper
    {
        /// <summary>
        /// Gets all of the column value in a table.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="table">The table.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>A list of the column values.</returns>
        public static List<T> GetColumnValues<T>(DataTable table, string columnName)
        {
            var result = new List<T>(table.Rows.Count);
            var ordinal = table.Columns[columnName].Ordinal;

            foreach (DataRow row in table.Rows)
            {
                result.Add(row.Field<T>(ordinal));
            }

            return result;
        }
    }
}
