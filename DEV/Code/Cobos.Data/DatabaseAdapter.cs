// ============================================================================
// Filename: DatabaseAdapter.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
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

// 05-Feb-11 N.Davis
// -----------------
// Rebranded from "Cobos" to "Intergraph.AsiaPac" in preparation for use in the Generic CAD Interoperability project

using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;

#if INTERGRAPH_BRANDING
using Intergraph.AsiaPac.Utilities;
using Intergraph.AsiaPac.Utilities.Xml;

namespace Intergraph.AsiaPac.Data
#else
using Cobos.Utilities;
using Cobos.Utilities.Xml;

namespace Cobos.Data
#endif
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

		public void Fill( string sql, DataTable result )
		{
			using ( ConnectionType connection = new ConnectionType() )
			{
				connection.ConnectionString = ConnectionString;
				connection.Open();

				using ( CommandType command = new CommandType() )
				{
					command.Connection = connection;
					command.CommandText = sql;

					using ( DataAdapterType adapter = new DataAdapterType() )
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
				using ( ConnectionType connection = new ConnectionType() )
				{
					connection.ConnectionString = ConnectionString;
					connection.Open();

					using ( CommandType command = new CommandType() )
					{
						command.Connection = connection;
						command.CommandText = sql;

						using ( DataAdapterType adapter = new DataAdapterType() )
						{
							((IDbDataAdapter)adapter).SelectCommand = command;
							dataSet = new DataSetType();
							adapter.Fill( dataSet, tableName );
						}

					}

					connection.Close();

					return dataSet;
				}
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
		public CobosDataSet Fill( string sql, string tableName, string dataSetName )
		{
			DataTable dataTable = Fill( sql, tableName );

			CobosDataSet dataSet = new CobosDataSet( dataSetName );
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
			CobosDataSet dataset = TableMetadata( schema, tables );
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
			CobosDataSet dataset = TableMetadata( schema, tables );

#if INTERGRAPH_BRANDING
			XslCompiledTransform xslTableToXsd = XsltHelper.Load( "Database/Oracle/DatabaseSchema.xslt", "Intergraph.AsiaPac.Data.Stylesheets" );
#else
			XslCompiledTransform xslTableToXsd = XsltHelper.Load( "Database/Oracle/DatabaseSchema.xslt", "Cobos.Data.Stylesheets" );
#endif
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
		protected abstract CobosDataSet TableMetadata( string schema, string[] tables );

		/// <summary>
		/// Test that a connection to the database can be made
		/// </summary>
		/// <returns>True if the test succeeded, otherwise false.</returns>
		bool TestConnection()
		{
			try
			{
				using ( ConnectionType connection = new ConnectionType() )
				{
					connection.ConnectionString = ConnectionString;
					connection.Open();
					connection.Close();
					return true;
				}
			}
			catch ( Exception )
			{
				return false;
			}
		}
	}
}
