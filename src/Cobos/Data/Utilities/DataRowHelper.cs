﻿// ----------------------------------------------------------------------------
// <copyright file="DataRowHelper.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;

    /// <summary>
    /// Abstract template class for custom aggregation of rows in a table.
    /// Similar in concept to aggregate functions and GROUP BY clauses in SQL.
    /// Typically used to aggregate string columns.
    /// </summary>
    public static class DataRowHelper
    {
        /// <summary>
        /// Aggregate a standard DataTable object.
        /// </summary>
        /// <param name="table">The DataTable object containing the rows to aggregate.</param>
        /// <param name="aggregateOn">The column that the aggregation is being performed on.</param>
        /// <param name="aggregator">The client aggregator.</param>
        /// <param name="copy">Indicates whether to aggregate the original table or create an aggregated copy.  Creating a copy is roughly twice as fast as aggregating the original.</param>
        /// <returns>An DataTable containing aggregated rows.</returns>
        public static DataTable Aggregate(DataTable table, DataColumnDescriptor aggregateOn, IDataRowAggregator aggregator, bool copy)
        {
            Dictionary<string, List<DataRow>> groups = FindGroups(table, aggregator);

            DataTable result;
            if (copy)
            {
                result = table.Clone();
            }
            else
            {
                result = table;
            }

            // If we are aggregating strings then make sure we don't overflow
            // the maximum string length for the aggregated result.
            result.Columns[aggregateOn.Ordinal].MaxLength = -1;

            foreach (List<DataRow> group in groups.Values)
            {
                if (group.Count == 1)
                {
                    if (copy)
                    {
                        result.ImportRow(group[0]);
                    }
                    //// else just leave the row in the original table.
                }
                else
                {
                    DataRow row = result.NewRow();
                    aggregator.Aggregate(group, row);

                    // if not copying, mark all aggregated rows as deleted
                    // before inserting the result
                    if (!copy)
                    {
                        for (int g = 0; g < group.Count; ++g)
                        {
                            group[g].Delete();
                        }
                    }

                    result.Rows.Add(row);
                }
            }

            if (!copy)
            {
                result.AcceptChanges();
            }

            return result;
        }

        /// <summary>
        /// Utility function to build a row key.  Useful for grouping and sorting rows.
        /// </summary>
        /// <param name="row">The row to build the key from.</param>
        /// <param name="keyColumns">The columns to create the key from.</param>
        /// <returns>The key for the row.</returns>
        public static string GenerateRowKey(DataRow row, DataColumnDescriptor[] keyColumns)
        {
            StringBuilder key = new StringBuilder(128);

            for (int g = 0; g < keyColumns.Length; ++g)
            {
                if (g > 0)
                {
                    key.Append(',');
                }

                DataColumnDescriptor column = keyColumns[g];

                if (row.IsNull(column.Ordinal))
                {
                    // space character precedes all other characters in string comparisons.
                    key.Append(" ");
                }
                else
                {
                    // special case for DateTime, get the date as a sortable string
                    if (column.DataType == typeof(DateTime))
                    {
                        key.Append(((DateTime)row[column.Ordinal]).ToString("s"));
                    }
                    else if (column.DataType == typeof(DateTimeOffset))
                    {
                        key.Append(((DateTimeOffset)row[column.Ordinal]).ToString("u"));
                    }
                    else
                    {
                        key.Append(row[column.Ordinal]);
                    }
                }
            }

            return key.ToString();
        }

        /// <summary>
        /// Internal helper to group the table rows according to the implemented rules.
        /// </summary>
        /// <param name="table">The table containing the rows to aggregate.</param>
        /// <param name="aggregator">The aggregator to use to for the rows.</param>
        /// <returns>The DataTable rows grouped by key value.</returns>
        private static Dictionary<string, List<DataRow>> FindGroups(DataTable table, IDataRowAggregator aggregator)
        {
            DataRowCollection rows = table.Rows;

            Dictionary<string, List<DataRow>> groups = new Dictionary<string, List<DataRow>>(rows.Count / 2);

            // aggregate all rows according to the grouping columns
            for (int r = 0; r < rows.Count; ++r)
            {
                DataRow row = rows[r];

                string key = aggregator.GetKey(row);

                if (key == null)
                {
                    key = string.Empty;
                }
                else
                {
                    key = key.ToLower();
                }

                if (!groups.TryGetValue(key, out List<DataRow> found))
                {
                    found = new List<DataRow>();
                    groups[key] = found;
                }

                found.Add(row);
            }

            return groups;
        }
    }
}
