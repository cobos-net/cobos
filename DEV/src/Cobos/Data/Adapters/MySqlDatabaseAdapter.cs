// ----------------------------------------------------------------------------
// <copyright file="MySqlDatabaseAdapter.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Adapters
{
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Represents a connection to MySQL database.
    /// </summary>
    public class MySqlDatabaseAdapter : AnsiDatabaseAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDatabaseAdapter"/> class.
        /// </summary>
        public MySqlDatabaseAdapter()
            : base(MySqlClientFactory.Instance)
        {
        }
    }
}
