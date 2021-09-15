// ----------------------------------------------------------------------------
// <copyright file="PostgreSqlDatabaseAdapter.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Adapters
{
    using Npgsql;

    /// <summary>
    /// Represents a connection to <c>PostgreSQL</c> database.
    /// </summary>
    public class PostgreSqlDatabaseAdapter : AnsiDatabaseAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDatabaseAdapter"/> class.
        /// </summary>
        public PostgreSqlDatabaseAdapter()
            : base(NpgsqlFactory.Instance)
        {
        }
    }
}
