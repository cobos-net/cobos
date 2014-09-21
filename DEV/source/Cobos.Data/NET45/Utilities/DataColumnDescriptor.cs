// ----------------------------------------------------------------------------
// <copyright file="DataColumnDescriptor.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
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
