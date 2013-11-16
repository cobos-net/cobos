// ----------------------------------------------------------------------------
// <copyright file="DatabaseAdapter.cs" company="Cobos SDK">
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

namespace Cobos.Data.Adapters
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Xml.Xsl;
    using Cobos.Utilities;
    using Cobos.Utilities.Xml;

    /// <summary>
    /// Base class implementation of <see cref="IDatabaseAdapter"/>.
    /// </summary>
    /// <typeparam name="ConnectionType">The Database connection type.</typeparam>
    /// <typeparam name="CommandType">The Database command type.</typeparam>
    /// <typeparam name="DataAdapterType">The Data Adapter type.</typeparam>
    public abstract class DatabaseAdapter<ConnectionType, CommandType, DataAdapterType> : IDatabaseAdapter
        where ConnectionType : IDbConnection, new()
        where CommandType : IDbCommand, new()
        where DataAdapterType : DbDataAdapter, IDbDataAdapter, new()
    {
        /// <summary>
        /// The connection string
        /// </summary>
        public readonly string ConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseAdapter{ConnectionType,CommandType,DataAdapterType}"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public DatabaseAdapter(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is read only.
        /// </summary>
        /// <remarks>
        /// In some implementations this may result in performance 
        /// improvements if read-write access is not required.
        /// Set via the connection string. Not supported on all platforms.
        /// </remarks>
        public bool ReadOnly
        {
            get;
            protected set;
        }

        /// <summary>
        /// Executes an SQL statement against the Connection object of a .NET Framework
        /// data provider, and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command to execute.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText)
        {
            using (var connection = this.GetConnection())
            {
                int result;
                connection.Open();

                using (var command = this.GetCommand())
                {
                    command.Connection = connection;
                    command.CommandText = commandText;
                    result = command.ExecuteNonQuery();
                }

                connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Executes the <c>System.Data.IDbCommand.CommandText</c> against the <c>System.Data.IDbCommand.Connection</c>
        /// and builds an <c>System.Data.IDataReader</c>.
        /// </summary>
        /// <param name="commandText">The command to execute.</param>
        /// <returns>An <c>System.Data.IDataReader</c> object.</returns>
        public IDataReader ExecuteReader(string commandText)
        {
            using (var connection = this.GetConnection())
            {
                IDataReader result = null;
                connection.Open();

                using (var command = this.GetCommand())
                {
                    command.Connection = connection;
                    command.CommandText = commandText;
                    result = command.ExecuteReader();
                }

                connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the
        /// result set returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <param name="commandText">The command to execute.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        public object ExecuteScalar(string commandText)
        {
            using (var connection = this.GetConnection())
            {
                object result = null;
                connection.Open();

                using (var command = this.GetCommand())
                {
                    command.Connection = connection;
                    command.CommandText = commandText;
                    result = command.ExecuteScalar();
                }

                connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The SQL script.</param>
        public void ExecuteScript(string script)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fill a DataTable with the query result.
        /// </summary>
        /// <param name="commandText">The query to execute.</param>
        /// <param name="result">The table to fill with the query result.</param>
        public virtual void Fill(string commandText, DataTable result)
        {
            using (var connection = this.GetConnection())
            {
                connection.Open();

                using (var command = this.GetCommand())
                {
                    command.Connection = connection;
                    command.CommandText = commandText;

                    using (var adapter = (DbDataAdapter)this.GetDataAdapter())
                    {
                        ((IDbDataAdapter)adapter).SelectCommand = command;
                        adapter.Fill(result);
                    }
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Fill the DatabaseQuery objects synchronously.
        /// </summary>
        /// <param name="queries">The queries to process.</param>
        public void Fill(DatabaseQuery[] queries)
        {
            foreach (var query in queries)
            {
                this.Fill(query.CommandText, query.Table);
            }
        }

        /// <summary>
        /// Fill the DatabaseQuery objects asynchronously.
        /// </summary>
        /// <param name="queries">The queries to process.</param>
        public void FillAsynch(DatabaseQuery[] queries)
        {
            Action<DatabaseQuery> action = q => this.Fill(q.CommandText, q.Table);

            var results = new IAsyncResult[queries.Length];

            for (int i = 0; i < results.Length; ++i)
            {
                results[i] = action.BeginInvoke(queries[i], null, null);
            }

            for (int i = 0; i < results.Length; ++i)
            {
                action.EndInvoke(results[i]);
            }
        }

        /// <summary>
        /// Gets a new database connection object.
        /// </summary>
        /// <returns>An object representing a valid database connection.</returns>
        public virtual IDbConnection GetConnection()
        {
            ConnectionType connection = new ConnectionType();
            connection.ConnectionString = this.ConnectionString;
            return connection;
        }

        /// <summary>
        /// Get a new database command object.
        /// </summary>
        /// <returns>An object representing a valid database command.</returns>
        public virtual IDbCommand GetCommand()
        {
            return new CommandType();
        }

        /// <summary>
        /// Get a new database adapter object.
        /// </summary>
        /// <returns>An object representing a valid database adapter.</returns>
        public virtual IDbDataAdapter GetDataAdapter()
        {
            return new DataAdapterType();
        }

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
        public void GetTableMetadata(string schema, string[] tables, Stream result)
        {
            SimpleDataSet dataset = this.TableMetadata(schema, tables);
            dataset.ToXml(result);
        }

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
        public void GetTableSchema(string schema, string[] tables, Stream result)
        {
            SimpleDataSet dataset = this.TableMetadata(schema, tables);

            XslCompiledTransform transform = XsltHelper.Load("Database/DatabaseSchema.xslt", "Cobos.Data.Stylesheets");

            if (transform != null)
            {
                dataset.ToXml(transform, null, result);
            }
            else
            {
                dataset.ToXml(result);
            }
        }

        /// <summary>
        /// Test the connection to the database to ensure that the adapter 
        /// is correctly configured.
        /// </summary>
        /// <returns>True if the test was successful; Otherwise false.</returns>
        public abstract bool TestConnection();

        /// <summary>
        /// SQL to query the DB metadata to get the columns for the specified tables.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="tableNames">The table names to query for.</param>
        /// <returns>A platform specific SQL query.</returns>
        protected abstract string GetMetadataColumnsSQL(string schema, string tableNames);

        /// <summary>
        /// SQL to query the DB metadata to get the constraints for the specified tables.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="tableNames">The table names to query for.</param>
        /// <returns>A platform specific SQL query.</returns>
        protected abstract string GetMetadataConstraintsSQL(string schema, string tableNames);

        /// <summary>
        /// Query the table metadata for the specified schema.
        /// </summary>
        /// <param name="schema">The schema that owns the tables.</param>
        /// <param name="tables">The table names to query for.</param>
        /// <returns>A dataset containing column and constraint definitions for the tables.</returns>
        private SimpleDataSet TableMetadata(string schema, string[] tables)
        {
            schema = schema.ToUpper();
            var tableNames = string.Join("', '", tables).ToUpper();

            var result = new SimpleDataSet("TABLE_METADATA");

            var table = new DataTable("TABLE");
            var column = new DataTable("COLUMN");
            var constraint = new DataTable("CONSTRAINT");

            result.Tables.Add(table);
            result.Tables.Add(column);
            result.Tables.Add(constraint);

            table.Columns.Add(new DataColumn("NAME", typeof(string)));

            foreach (string t in tables)
            {
                var row = table.NewRow();
                row["NAME"] = t;
                table.Rows.Add(row);
            }

            this.Fill(this.GetMetadataColumnsSQL(schema, tableNames), column);
            this.Fill(this.GetMetadataConstraintsSQL(schema, tableNames), constraint);

            SimpleDataSet.Relationship[] relations = new SimpleDataSet.Relationship[]
            {
                new SimpleDataSet.Relationship("COLUMNS", "TABLE", "NAME", "COLUMN", "TABLE_NAME"),
                new SimpleDataSet.Relationship("CONSTRAINTS", "TABLE", "NAME", "CONSTRAINT", "TABLE_NAME")
            };

            result.CreateRelationships(relations);

            return result;
        }
    }
}
