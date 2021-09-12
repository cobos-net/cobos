// ----------------------------------------------------------------------------
// <copyright file="DataRowColumnComparer.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Compares tow DataRow objects by column value.
    /// </summary>
    public class DataRowColumnComparer : IComparer<DataRow>
    {
        /// <summary>
        /// The requested sort order.
        /// </summary>
        private readonly SortOrderEnum sortOrder;

        /// <summary>
        /// Column order for sort comparison.
        /// </summary>
        private readonly DataColumnDescriptor[] columns;

        /// <summary>
        /// Ignore case for string comparisons.
        /// </summary>
        private readonly bool ignoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowColumnComparer"/> class.
        /// </summary>
        /// <param name="sortColumns">The columns to sort by.</param>
        /// <param name="sortOrder">The order to sort the columns by.</param>
        /// <param name="ignoreCase">Indicates whether comparisons should be case-insensitive.</param>
        public DataRowColumnComparer(DataColumnDescriptor[] sortColumns, SortOrderEnum sortOrder, bool ignoreCase)
        {
            this.columns = sortColumns;
            this.sortOrder = sortOrder;
            this.ignoreCase = ignoreCase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowColumnComparer"/> class.
        /// </summary>
        /// <param name="sortColumns">The columns to sort by.</param>
        /// <param name="sortOrder">The order to sort the columns by.</param>
        /// <param name="ignoreCase">Indicates whether comparisons should be case-insensitive.</param>
        public DataRowColumnComparer(DataColumn[] sortColumns, SortOrderEnum sortOrder, bool ignoreCase)
            : this(DataColumnDescriptor.ConstructFrom(sortColumns), sortOrder, ignoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowColumnComparer"/> class.
        /// </summary>
        /// <remarks>
        /// Simple constructor for creating a case-sensitive ascending sort.
        /// </remarks>
        /// <param name="sortColumns">The columns to sort by.</param>
        public DataRowColumnComparer(DataColumn[] sortColumns)
            : this(sortColumns, SortOrderEnum.Ascending, false)
        {
        }

        /// <summary>
        /// Specify the sort order.
        /// </summary>
        public enum SortOrderEnum
        {
            /// <summary>
            /// Sort ascending.
            /// </summary>
            Ascending,

            /// <summary>
            /// Sort descending.
            /// </summary>
            Descending,
        }

        /// <summary>
        /// Compare the two rows for sort order based on the instance's sort columns.
        /// This assumes that both rows either belong to the same table or that
        /// they have the same column format.
        /// </summary>
        /// <param name="lhs">The first object to compare.</param>
        /// <param name="rhs">The second object to compare.</param>
        /// <returns>
        /// <para>
        /// A signed integer that indicates the relative values of x and y, as shown
        /// in the following table:
        /// </para>
        /// <para>
        /// Value               | Meaning
        /// --------------------|------------------------
        /// Less than zero      | x is less than y.
        /// Zero                | x equals y.
        /// Greater than zero   | x is greater than y.
        /// </para>
        /// </returns>
        public int Compare(DataRow lhs, DataRow rhs)
        {
            for (int c = 0; c < this.columns.Length; ++c)
            {
                DataColumnDescriptor column = this.columns[c];
                int ordinal = column.Ordinal;

                int result;

                // check for nulls
                bool lhsIsNull = lhs.IsNull(ordinal);
                bool rhsIsNull = rhs.IsNull(ordinal);

                if (lhsIsNull && rhsIsNull)
                {
                    continue;
                }
                else if (lhsIsNull)
                {
                    result = -1;
                }
                else if (rhsIsNull)
                {
                    result = 1;
                }
                else if (column.DataType == typeof(string))
                {
                    // special case for strings, use the case sensitive matching if required.
                    result = string.Compare((string)lhs[ordinal], (string)rhs[ordinal], this.ignoreCase);
                }
                else
                {
                    // Any other primitive type (including DateTime).
                    // Both objects *should* be of the same type based on the assumptions listed
                    // in the method summary.
                    if (lhs[ordinal] is IComparable comparable)
                    {
                        result = comparable.CompareTo(rhs[ordinal]);
                    }
                    else
                    {
                        // all primitive types implement IComparable, so if we get here,
                        // all that we can do is do a ToString comparison.
                        result = string.Compare(lhs[ordinal].ToString(), rhs[ordinal].ToString(), this.ignoreCase);
                    }
                }

                if (result != 0)
                {
                    return this.sortOrder == SortOrderEnum.Ascending ? result : -result;
                }
            }

            return 0; // rows are equal based on column comparison
        }
    }
}
