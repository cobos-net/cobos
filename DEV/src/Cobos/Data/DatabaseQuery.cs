// ----------------------------------------------------------------------------
// <copyright file="DatabaseQuery.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data
{
    using System.Data;
    using Cobos.Data.Transforms;

    /// <summary>
    /// Allow multiple table queries to be processed asynchronously.
    /// </summary>
    public class DatabaseQuery
    {
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
        /// Gets the statement to run to fill the Table object.
        /// </summary>
        public string CommandText { get; }

        /// <summary>
        /// Gets the data table to populate with the query result.
        /// </summary>
        public DataTable Table { get; }

        /// <summary>
        /// Gets the custom transform to the query.
        /// </summary>
        public IDataTableTransform Transform { get; }

        /// <summary>
        /// Get the table result.
        /// If an transform is provided, it will return the transformed table.
        /// </summary>
        /// <returns>An object representing. </returns>
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
