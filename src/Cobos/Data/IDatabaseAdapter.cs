﻿// ----------------------------------------------------------------------------
// <copyright file="IDatabaseAdapter.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.IO;

    /// <summary>
    /// Represents a vendor agnostic set of database utility methods.
    /// </summary>
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Gets the Database Provider factory instance.
        /// </summary>
        DbProviderFactory ProviderFactory { get; }

        /// <summary>
        /// Gets the type that represents an integer (typically 32 bit) value.
        /// </summary>
        Type IntegerType { get; }

        /// <summary>
        /// Gets the type that represents an integer (typically 64 bit) value.
        /// </summary>
        Type BigIntegerType { get; }

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
