// ----------------------------------------------------------------------------
// <copyright file="IDatabaseAdapter.cs" company="Cobos SDK">
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

namespace Cobos.Data
{
    using System;
    using System.Data;
    using System.IO;

    /// <summary>
    /// Represents a vendor agnostic set of database utility methods.
    /// </summary>
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// Gets a value indicating whether the connection is read only.
        /// </summary>
        /// <remarks>
        /// In some implementations this may result in performance 
        /// improvements if read-write access is not required.
        /// Set via the connection string. Not supported on all platforms.
        /// </remarks>
        bool ReadOnly
        {
            get;
        }
        
        /// <summary>
        /// Executes an SQL statement against the Connection object of a .NET Framework
        /// data provider, and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command to execute.</param>
        /// <returns>The number of rows affected.</returns>
        int ExecuteNonQuery(string commandText);
        
        /// <summary>
        /// Executes the <c>System.Data.IDbCommand.CommandText</c> against the <c>System.Data.IDbCommand.Connection</c>
        /// and builds an <c>System.Data.IDataReader</c>.
        /// </summary>
        /// <param name="commandText">The command to execute.</param>
        /// <returns>An <c>System.Data.IDataReader</c> object.</returns>
        IDataReader ExecuteReader(string commandText);
        
        /// <summary>
        /// Executes the query, and returns the first column of the first row in the
        /// result set returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <param name="commandText">The command to execute.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        object ExecuteScalar(string commandText);

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The SQL script.</param>
        void ExecuteScript(string script);

        /// <summary>
        /// Fill a DataTable with the query result.
        /// </summary>
        /// <param name="commandText">The query to execute.</param>
        /// <param name="result">The table to fill with the query result.</param>
        void Fill(string commandText, DataTable result);

        /// <summary>
        /// Fill the DatabaseQuery objects synchronously.
        /// </summary>
        /// <param name="queries">The queries to process.</param>
        void Fill(DatabaseQuery[] queries);

        /// <summary>
        /// Fill the DatabaseQuery objects asynchronously.
        /// </summary>
        /// <param name="queries">The queries to process.</param>
        void FillAsynch(DatabaseQuery[] queries);

        /// <summary>
        /// Gets a new database connection object.
        /// </summary>
        /// <returns>An object representing a valid database connection.</returns>
        IDbConnection GetConnection();

        /// <summary>
        /// Get a new database command object.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <returns>An object representing a valid database command.</returns>
        IDbCommand GetCommand();

        /// <summary>
        /// Get a new database adapter object.
        /// </summary>
        /// <returns>An object representing a valid database adapter.</returns>
        IDbDataAdapter GetDataAdapter();

        /// <summary>
        /// Get the basic metadata from the RDBMS for the specified tables.
        /// </summary>
        /// <remarks>
        /// This queries the column types and constraints for the specified
        /// tables from the RDBMS.  The result is written to the stream
        /// in a basic XML format.
        /// </remarks>
        /// <param name="schema">The schema that owns the tables.</param>
        /// <param name="tables">The table names to query for.</param>
        /// <param name="result">The stream to write the result to.</param>
        void GetTableMetadata(string schema, string[] tables, Stream result);

        /// <summary>
        /// Get an XSD document from the RDBMS for the specified tables.
        /// </summary>
        /// <remarks>
        /// This queries the column types and constraints for the specified
        /// tables from the RDBMS.  The result is written to the stream
        /// in an XSD format.
        /// </remarks>
        /// <param name="schema">The schema that owns the tables.</param>
        /// <param name="tables">The table names to query for.</param>
        /// <param name="result">The stream to write the result to.</param>
        void GetTableSchema(string schema, string[] tables, Stream result);

        /// <summary>
        /// Test the connection to the database to ensure that the adapter 
        /// is correctly configured.
        /// </summary>
        /// <returns>True if the test was successful; Otherwise false.</returns>
        bool TestConnection();
    }
}
