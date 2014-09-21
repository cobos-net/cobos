// ----------------------------------------------------------------------------
// <copyright file="DatabaseQuery.cs" company="Cobos SDK">
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

namespace Cobos.Data
{
    using System;
    using System.Data;
    using Cobos.Data.Transforms;

    /// <summary>
    /// Allow multiple table queries to be processed asynchronously
    /// </summary>
    public class DatabaseQuery
    {
        /// <summary>
        /// The statement to run to fill the Table object
        /// </summary>
        public readonly string CommandText;

        /// <summary>
        /// The data table to populate with the query result.
        /// </summary>
        public readonly DataTable Table;

        /// <summary>
        /// Attach a custom transform to the query.
        /// </summary>
        public readonly IDataTableTransform Transform;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseQuery"/> class.
        /// </summary>
        /// <param name="commandText">The query to execute.</param>
        /// <param name="table">The DataTable to fill with the query result.</param>
        public DatabaseQuery(string commandText, DataTable table)
        {
            this.CommandText = commandText;
            this.Table = table;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseQuery"/> class with a custom aggregator.
        /// </summary>
        /// <param name="commandText">The query to execute.</param>
        /// <param name="table">The table to fill with the result.</param>
        /// <param name="transform">The transform to use on the result.</param>
        public DatabaseQuery(string commandText, DataTable table, IDataTableTransform transform)
            : this(commandText, table)
        {
            this.Transform = transform;
        }

        /// <summary>
        /// Get the table result.  
        /// If an transform is provided, it will return the transformed table.
        /// </summary>
        /// <returns>An object representing </returns>
        public DataTable GetResult()
        {
            if (this.Transform != null)
            {
                return this.Transform.Transform(this.Table);
            }
            else
            {
                return this.Table;
            }
        }
    }
}
