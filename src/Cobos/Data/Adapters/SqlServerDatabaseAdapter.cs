// ----------------------------------------------------------------------------
// <copyright file="SqlServerDatabaseAdapter.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Adapters
{
    using System.Data.SqlClient;

    /// <summary>
    /// Represents a connection to SQL Server database.
    /// </summary>
    public class SqlServerDatabaseAdapter : AnsiDatabaseAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDatabaseAdapter"/> class.
        /// </summary>
        public SqlServerDatabaseAdapter()
            : base(SqlClientFactory.Instance)
        {
        }
    }
}
