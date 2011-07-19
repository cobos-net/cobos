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
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;
using Oracle.DataAccess.Client;

#if INTERGRAPH_BRANDING
using Intergraph.AsiaPac.Utilities;
using Intergraph.AsiaPac.Utilities.Xml;

namespace Intergraph.AsiaPac.Data
#else
using Cobos.Utilities;
using Cobos.Utilities.Xml;

//namespace Cobos.Data
#endif
{
	using AsyncDataSetTask = AsyncTask<DataTable, DatabaseAdapter.QueryDatabaseAsync>;

	public class DatabaseAdapter : IDatabaseAdapter
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
		/// Ignored for straight Oracle connections.
		/// </summary>
		public bool ReadOnly
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public DataTable Execute( string sql, string tableName )
		{
			DataTable dataTable = null;

			try
			{
				dataTable = new DataTable( tableName );
				
				Execute( sql, dataTable );
				
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

		public void Execute( string sql, DataTable result )
		{
			using ( OracleConnection oracle = new OracleConnection( ConnectionString ) )
			{
#if DEBUG
				Stopwatch timer = new Stopwatch();
				timer.Start();
#endif

				oracle.Open();

				using ( OracleCommand command = new OracleCommand() )
				{
					command.Connection = oracle;
					command.CommandText = sql;

					using ( OracleDataAdapter adapter = new OracleDataAdapter( command ) )
					{
						adapter.Fill( result );
					}
				}

				oracle.Close();

#if DEBUG
				timer.Stop();
				System.Diagnostics.Debug.Print( "{0}: {1} ({2})", timer.ElapsedMilliseconds, result.TableName, sql );
#endif
			}
		}

		/// <summary>
		/// Fill a strongly typed datatable
		/// </summary>
		/// <typeparam name="DataTableType"></typeparam>
		/// <param name="sql"></param>
		/// <returns></returns>
		public DataTableType Execute<DataTableType>( string sql ) where DataTableType : DataTable, new()
		{
			DataTableType dataTable = default( DataTableType );

			try
			{
				dataTable = new DataTableType();
				
				Execute( sql, dataTable );

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
		public DataSetType Execute<DataSetType>( string sql, string tableName ) where DataSetType : DataSet, new()
		{
			DataSetType dataSet = default( DataSetType );

			try
			{
				using ( OracleConnection oracle = new OracleConnection( ConnectionString ) )
				{
#if DEBUG
					Stopwatch timer = new Stopwatch();
					timer.Start();
#endif

					oracle.Open();

					using ( OracleCommand command = new OracleCommand() )
					{
						command.Connection = oracle;
						command.CommandText = sql;

						using ( OracleDataAdapter adapter = new OracleDataAdapter( command ) )
						{
							dataSet = new DataSetType();
							adapter.Fill( dataSet, tableName );
						}

					}

					oracle.Close();

#if DEBUG
					timer.Stop();
					System.Diagnostics.Debug.Print( "{0}: {1} ({2})", timer.ElapsedMilliseconds, tableName, sql );
#endif

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
		public CadDataSet Execute( string sql, string tableName, string dataSetName )
		{
			DataTable dataTable = Execute( sql, tableName );

			CadDataSet dataSet = new CadDataSet( dataSetName );
			dataSet.Tables.Add( dataTable );

			return dataSet;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queries"></param>
		/// <param name="dataset"></param>
		public void Execute( DatabaseQuery[] queries )
		{
			for ( int q = 0; q != queries.Length; ++q )
			{
				Execute( queries[ q ].Sql, queries[ q ].Table );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queries"></param>
		/// <param name="dataset"></param>
		public void Execute( DatabaseQuery query )
		{
			Execute( query.Sql, query.Table );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="dataset"></param>
		public void Execute( string sql, string tableName, DataSet dataset )
		{
			DataTable table = dataset.Tables[ tableName ];

			if ( table == null )
			{
				table = Execute( sql, tableName );

				if ( table != null )
				{
					dataset.Tables.Add( table );
				}
			}
			else
			{
				Execute( sql, table );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queries"></param>
		/// <param name="dataSetName"></param>
		/// <returns></returns>
		public void ExecuteAsynch( DatabaseQuery[] queries )
		{
			// create a new helpers list
			AsyncDataSetTask[] tasks = new AsyncDataSetTask[ queries.Length ];

			// initiate the queries
			for ( int i = 0; i != queries.Length; ++i )
			{
				DatabaseQuery q = queries[ i ];
								
				// create a new query helper
				tasks[ i ] = new AsyncDataSetTask();
				tasks[ i ].Object = q.Table;
				tasks[ i ].Caller = Execute;
				tasks[ i ].AsyncResult = tasks[ i ].Caller.BeginInvoke( q.Sql, q.Table, null, null );
			}

			// get the results from the queries
			foreach ( AsyncDataSetTask task in tasks )
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
			CadDataSet dataset = TableMetadata( schema, tables );

			XslCompiledTransform xslTableToXsd = XsltHelper.Load( "DatabaseSchema.xslt", "Intergraph.AsiaPac.Data.Stylesheets" );

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
			CadDataSet dataset = TableMetadata( schema, tables );

			XslCompiledTransform xslTableToXsd = XsltHelper.Load( "DatabaseSchema.xslt", "Intergraph.AsiaPac.Data.Stylesheets" );

			if ( xslTableToXsd != null )
			{
				dataset.ToXml( xslTableToXsd, null, result );
			}
			else
			{
				dataset.ToXml( result );
			}
		}

		CadDataSet TableMetadata( string schema, string[] tables )
		{
			CadDataSet result = new CadDataSet( "TABLE_METADATA" );

			DataTable table = new DataTable( "TABLE" );
			table.Columns.Add( new DataColumn( "NAME", Type.GetType("System.String") ) );

			foreach ( string t in tables )
			{
				DataRow row = table.NewRow();
				row[ "NAME" ] = t;
				table.Rows.Add( row );
			}

			result.Tables.Add( table );

			string columns = @"SELECT table_name, column_name, data_type, data_length, "
							  + "data_precision, data_scale, nullable, data_default, char_length "
							  + "FROM all_tab_columns "
							  + "WHERE UPPER( owner ) = '" + schema.ToUpper() + "' "
							  + "AND UPPER( table_name ) IN ('" + string.Join( "', '", tables ).ToUpper() + "') "
							  + "ORDER BY table_name ASC, column_id ASC";

			Execute( columns, "COLUMN", result );

			string constraints = "SELECT cols.table_name, cols.column_name, cols.position, cons.constraint_type, cons.constraint_name, cons.status "
										+ "FROM all_constraints cons, all_cons_columns cols "
										+ "WHERE cols.table_name IN ('" + string.Join( "', '", tables ).ToUpper() + "') "
										+ "AND cons.constraint_type IN ('P', 'R', 'U') "
										+ "AND cons.constraint_name = cols.constraint_name "
										+ "AND cons.owner = '" + schema.ToUpper() + "' "
										+ "AND cons.owner = cols.owner "
										+ "ORDER BY cols.table_name, cols.position";

			Execute( constraints, "CONSTRAINT", result );

			CadDataSet.Relationship[] relations = new CadDataSet.Relationship[]
			{
				new CadDataSet.Relationship( "COLUMNS", "TABLE", "NAME", "COLUMN", "TABLE_NAME" ),
				new CadDataSet.Relationship( "CONTSTRAINTS", "TABLE", "NAME", "CONSTRAINT", "TABLE_NAME" )
			};

			result.CreateRelationships( relations );

			return result;
		}

		/// <summary>
		/// Test that a connection to the database can be made
		/// </summary>
		/// <returns>True if the test succeeded, otherwise false.</returns>
		bool TestConnection()
		{
			try
			{
				using ( OracleConnection oracle = new OracleConnection( ConnectionString ) )
				{
					oracle.Open();
					oracle.Close();
					return true;
				}
			}
			catch ( OracleException )
			{
				return false;
			}
			// any other exceptions should not be ignored
		}
	}
}
