// ----------------------------------------------------------------------------
// <copyright file="DataColumnDescriptor.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Utilities
{
    using System;
    using System.Data;

    /// <summary>
    /// An abstracted descriptor for a DataColumn.
    /// </summary>
    public struct DataColumnDescriptor
    {
        /// <summary>
        /// The column ordinal.
        /// </summary>
        public readonly int Ordinal;

        /// <summary>
        /// The data type of the column.
        /// </summary>
        public readonly Type DataType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataColumnDescriptor"/> struct.
        /// </summary>
        /// <param name="ordinal">The ordinal of the column.</param>
        /// <param name="type">The data type of the column.</param>
        public DataColumnDescriptor(int ordinal, Type type)
        {
            this.Ordinal = ordinal;
            this.DataType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataColumnDescriptor"/> struct.
        /// </summary>
        /// <param name="column">The column to create the descriptor from.</param>
        public DataColumnDescriptor(DataColumn column)
        {
            this.Ordinal = column.Ordinal;
            this.DataType = column.DataType;
        }

        /// <summary>
        /// Helper method to convert an array of DataColumn objects.
        /// </summary>
        /// <param name="columns">The columns to convert.</param>
        /// <returns>An array of descriptors.</returns>
        public static DataColumnDescriptor[] ConstructFrom(DataColumn[] columns)
        {
            if (columns == null)
            {
                return null;
            }

            DataColumnDescriptor[] result = new DataColumnDescriptor[columns.Length];

            for (int c = 0; c < columns.Length; ++c)
            {
                result[c] = new DataColumnDescriptor(columns[c]);
            }

            return result;
        }
    }
}
