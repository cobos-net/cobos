﻿// ----------------------------------------------------------------------------
// <copyright file="DataRowColumnAggregator.cs" company="Nicholas Davis">
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
    /// Implementation of IDataRowAggregator for string columns.
    /// </summary>
    public class DataRowColumnAggregator : IDataRowAggregator
    {
        /// <summary>
        /// Specifies the column to aggregate on.
        /// </summary>
        private readonly DataColumnDescriptor aggregateOn;

        /// <summary>
        /// Specifies the column order to perform the groupings for key generation.
        /// </summary>
        private readonly DataColumnDescriptor[] groupBy;

        /// <summary>
        /// Specifies the column order to sort the groupings by. Can be null if
        /// the columns are already sorted by the database query.
        /// </summary>
        private readonly DataRowColumnComparer sortBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowColumnAggregator"/> class.
        /// </summary>
        /// <param name="aggregateOn">The column to aggregate on.</param>
        /// <param name="groupBy">The columns to group the aggregation by.</param>
        /// <param name="sortBy">The sort order for performing the aggregation.</param>
        public DataRowColumnAggregator(DataColumnDescriptor aggregateOn, DataColumnDescriptor[] groupBy, DataRowColumnComparer sortBy)
        {
            if (aggregateOn.DataType != typeof(string))
            {
                throw new InvalidOperationException("DataColumnAggregator.DataColumnAggregator: Can only aggregate on string columns.  Column is type: " + aggregateOn.DataType.Name);
            }

            this.aggregateOn = aggregateOn;
            this.groupBy = groupBy;
            this.sortBy = sortBy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowColumnAggregator"/> class.
        /// </summary>
        /// <param name="aggregateOn">The column to aggregate on.</param>
        /// <param name="groupBy">The columns to group the aggregation by.</param>
        /// <param name="sortBy">The sort order for performing the aggregation.</param>
        public DataRowColumnAggregator(DataColumn aggregateOn, DataColumn[] groupBy, DataRowColumnComparer sortBy)
            : this(new DataColumnDescriptor(aggregateOn), DataColumnDescriptor.ConstructFrom(groupBy), sortBy)
        {
        }

        /// <summary>
        /// Transform the DataTable to produce an aggregated result.
        /// </summary>
        /// <param name="table">The input DataTable.</param>
        /// <returns>The transformed result.</returns>
        public DataTable Transform(DataTable table)
        {
            return DataRowHelper.Aggregate(table, this.aggregateOn, this, true);
        }

        /// <summary>
        /// Called by DataRowHelper class to generate the row key.
        /// </summary>
        /// <param name="row">The row to generate the key for.</param>
        /// <returns>The string key for the row.</returns>
        public string GetKey(DataRow row)
        {
            return DataRowHelper.GenerateRowKey(row, this.groupBy);
        }

        /// <summary>
        /// Called by DataRowHelper to aggregate the row collection.
        /// The row collection is guaranteed to contain at least two rows.
        /// </summary>
        /// <param name="rows">The grouped rows.</param>
        /// <param name="result">The row to store the aggregated result.</param>
        public void Aggregate(List<DataRow> rows, DataRow result)
        {
            if (this.sortBy != null)
            {
                rows.Sort(this.sortBy);
            }

            int ordinal = this.aggregateOn.Ordinal;

            // construct the aggregate string.  each string value is trimmed
            // and the values are concatenated with a delimiting ' ' space.
            StringBuilder aggregate = new StringBuilder(512);

            for (int r = 0; r < rows.Count; ++r)
            {
                DataRow row = rows[r];

                if (row.IsNull(ordinal))
                {
                    continue;
                }

                if (aggregate.Length > 0)
                {
                    aggregate.Append(" ");
                }

                // we know this column is a string, this is checked in the constructor.
                string columnValue = (string)row[ordinal];

                aggregate.Append(columnValue.Trim());
            }

            // copy values to the result row
            result.ItemArray = (object[])rows[0].ItemArray.Clone();

            // replace the aggregated column with the result
            result[ordinal] = aggregate.ToString();
        }
    }
}
