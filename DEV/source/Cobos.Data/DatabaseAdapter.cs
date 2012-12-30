// ============================================================================
// Filename: DatabaseAdapter.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;

using Cobos.Utilities;
using Cobos.Utilities.Xml;

namespace Cobos.Data
{
	public abstract class DatabaseAdapter<ConnectionType, CommandType, DataAdapterType> : IDatabaseAdapter
		where ConnectionType : IDbConnection, new()
		where CommandType : IDbCommand, new()
		where DataAdapterType : DbDataAdapter, IDbDataAdapter, new()
	{
		#region Private data

		#endregion

		#region Construction

		public DatabaseAdapter( string connectionString )
		{
			ConnectionString = connectionString;
		}

		#endregion

		#region Database Connection

		public readonly string ConnectionString;

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public delegate void QueryDatabaseAsync( string sql, DataTable table );

		/// <summary>
		/// Can be ignored for most DB connection types.
		/// </summary>
		public bool ReadOnly
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the DB Connection.  Can be overridden to provide
		/// custom connection opening behaviour.
		/// </summary>
		/// <returns></returns>
		protected virtual ConnectionType GetConnection()
		{
			ConnectionType connection = new ConnectionType();
			connection.ConnectionString = ConnectionString;
			return connection;
		}

		/// <summary>
		/// Gets a DB command object.  Can be overriden to provide
		/// custom command behaviour.
		/// </summary>
		/// <returns></returns>
		protected virtual CommandType GetCommand( ConnectionType connection )
		{
			CommandType command = new CommandType();
			command.Connection = connection;
			return command;
		}

		/// <summary>
		/// Get the Data Adapter type.  Can be overridden to provide
		/// custom data adapter behaviour.
		/// </summary>
		/// <returns></returns>
		protected virtual DataAdapterType GetDataAdapter()
		{
			return new DataAdapterType();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public DataTable Fill( string sql, string tableName )
		{
			DataTable dataTable = null;

			try
			{
				dataTable = new DataTable( tableName );
				
				Fill( sql, dataTable );
				
				return dataTable;
			}
			catch ( Exception )
			{
				if ( dataTable != null )
				{
					dataTable.Dispose();
				}
				throw;
			}
		}

		public virtual void Fill( string sql, DataTable result )
		{
			using ( ConnectionType connection = GetConnection() )
			{
				connection.Open();

				using ( CommandType command = GetCommand( connection ) )
				{
					command.CommandText = sql;

					using ( DataAdapterType adapter = GetDataAdapter() )
					{
						((IDbDataAdapter)adapter).SelectCommand = command;
						adapter.Fill( result );
					}
				}

				connection.Close();
			}
		}

		/// <summary>
		/// Fill a strongly typed datatable
		/// </summary>
		/// <typeparam name="DataTableType"></typeparam>
		/// <param name="sql"></param>
		/// <returns></returns>
		public DataTableType Fill<DataTableType>( string sql ) where DataTableType : DataTable, new()
		{
			DataTableType dataTable = default( DataTableType );

			try
			{
				dataTable = new DataTableType();
				
				Fill( sql, dataTable );

				return dataTable;
			}
			catch ( Exception )
			{
				if ( dataTable != null )
				{
					dataTable.Dispose();
				}
				throw;
			}
		}

		/// <summary>
		/// Fill a strongly typed set
		/// </summary>
		/// <typeparam name="DataTableType"></typeparam>
		/// <param name="sql"></param>
		/// <returns></returns>
		public DataSetType Fill<DataSetType>( string sql, string tableName ) where DataSetType : DataSet, new()
		{
			DataSetType dataSet = default( DataSetType );

			try
			{
				Fill( sql, tableName, dataSet );
				
				return dataSet;
			}
			catch ( Exception )
			{
				if ( dataSet != null )
				{
					dataSet.Dispose();
				}
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="tableName"></param>
		/// <param name="dataSetName"></param>
		/// <returns></returns>
		public SimpleDataSet Fill( string sql, string tableName, string dataSetName )
		{
			DataTable dataTable = Fill( sql, tableName );

			SimpleDataSet dataSet = new SimpleDataSet( dataSetName );
			dataSet.Tables.Add( dataTable );

			return dataSet;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queries"></param>
		/// <param name="dataset"></param>
		public void Fill( DatabaseQuery[] queries )
		{
			for ( int q = 0; q != queries.Length; ++q )
			{
				Fill( queries[ q ].Sql, queries[ q ].Table );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queries"></param>
		/// <param name="dataset"></param>
		public void Fill( DatabaseQuery query )
		{
			Fill( query.Sql, query.Table );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="dataset"></param>
		public void Fill( string sql, string tableName, DataSet dataset )
		{
			DataTable table = dataset.Tables[ tableName ];

			if ( table == null )
			{
				table = Fill( sql, tableName );

				if ( table != null )
				{
					dataset.Tables.Add( table );
				}
			}
			else
			{
				Fill( sql, table );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queries"></param>
		/// <param name="dataSetName"></param>
		/// <returns></returns>
		public void FillAsynch( DatabaseQuery[] queries )
		{
			// create a new helpers list
			AsyncTask<DataTable, DatabaseAdapter<ConnectionType, CommandType, DataAdapterType>.QueryDatabaseAsync>[] tasks = new AsyncTask<DataTable, DatabaseAdapter<ConnectionType, CommandType, DataAdapterType>.QueryDatabaseAsync>[ queries.Length ];

			// initiate the queries
			for ( int i = 0; i != queries.Length; ++i )
			{
				DatabaseQuery q = queries[ i ];
								
				// create a new query helper
				tasks[ i ] = new AsyncTask<DataTable, DatabaseAdapter<ConnectionType, CommandType, DataAdapterType>.QueryDatabaseAsync>();
				tasks[ i ].Object = q.Table;
				tasks[ i ].Caller = Fill;
				tasks[ i ].AsyncResult = tasks[ i ].Caller.BeginInvoke( q.Sql, q.Table, null, null );
			}

			// get the results from the queries
			foreach ( AsyncTask<DataTable, DatabaseAdapter<ConnectionType, CommandType, DataAdapterType>.QueryDatabaseAsync> task in tasks )
			{
				task.Caller.EndInvoke( task.AsyncResult );
			}
		}

		/// <summary>
		/// Get a raw Xml description of the specified tables
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="tables"></param>
		/// <param name="result"></param>
		public void GetTableMetadata( string schema, string[] tables, Stream result )
		{
			SimpleDataSet dataset = TableMetadata( schema, tables );
			dataset.ToXml( result );
		}

		/// <summary>
		/// Get an XSD schema document for the specified tables.
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="tables"></param>
		/// <param name="result"></param>
		public void GetTableSchema( string schema, string[] tables, Stream result )
		{
			SimpleDataSet dataset = TableMetadata( schema, tables );

			XslCompiledTransform xslTableToXsd = XsltHelper.Load( "Database/Oracle/DatabaseSchema.xslt", "Cobos.Data.Stylesheets" );
			
			if ( xslTableToXsd != null )
			{
				dataset.ToXml( xslTableToXsd, null, result );
			}
			else
			{
				dataset.ToXml( result );
			}
		}

		/// <summary>
		/// Querying metadata varies between platforms, each platform specific derived class must provide
		/// its own implementation of this method.
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="tables"></param>
		/// <returns></returns>
		protected abstract SimpleDataSet TableMetadata( string schema, string[] tables );

		/// <summary>
		/// Test that a connection to the database can be made
		/// </summary>
		/// <returns>True if the test succeeded, otherwise false.</returns>
		public abstract bool TestConnection();
	}
}
