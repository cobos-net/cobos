// ----------------------------------------------------------------------------
// <copyright file="SqlDatabaseAdapter.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
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

namespace Cobos.Data.SqlServer
{
    using System;
    using System.Data.SqlClient;

    /// <summary>
    /// Represents a connection to a SQL Server database.
    /// </summary>
    public class SqlDatabaseAdapter : DatabaseAdapter<SqlConnection, SqlCommand, SqlDataAdapter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseAdapter"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public SqlDatabaseAdapter(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Test the connection to the database to ensure that the adapter 
        /// is correctly configured.
        /// </summary>
        /// <returns>True if the test was successful; Otherwise false.</returns>
        public override bool TestConnection()
        {
            bool result = false;

            try
            {
                using (var connection = this.GetConnection())
                {
                    connection.Open();

                    using (var command = this.GetCommand(connection))
                    {
                        command.CommandText = "select 1";

                        object @object = command.ExecuteScalar();

                        if (@object != null && @object.GetType() == typeof(long))
                        {
                            result = (long)@object == 1;
                        }
                    }

                    connection.Close();

                    return result;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// SQL to query the DB metadata to get the columns for the specified tables.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="tableNames">The table names to query for.</param>
        /// <returns>A platform specific SQL query.</returns>
        protected override string GetMetadataColumnsSQL(string schema, string tableNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SQL to query the DB metadata to get the constraints for the specified tables.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="tableNames">The table names to query for.</param>
        /// <returns>A platform specific SQL query.</returns>
        protected override string GetMetadataConstraintsSQL(string schema, string tableNames)
        {
            throw new NotImplementedException();
        }
    }
}
