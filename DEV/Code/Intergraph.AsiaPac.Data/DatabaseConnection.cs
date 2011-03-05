using System;
using System.Data;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;
using Oracle.DataAccess.Client;
using Intergraph.AsiaPac.Utilities;
using Intergraph.AsiaPac.Utilities.Xml;

namespace Intergraph.AsiaPac.Data
{
	using AsyncDBTask = AsyncTask<CadDataSet, DatabaseConnection.QueryDatabaseAsync>;

	public class DatabaseConnection : IDisposable
	{
		#region Private data

		#endregion

		#region Construction

		public DatabaseConnection( string connectionString )
		{
			ConnectionString = connectionString;
		}

		#endregion

		#region Database Connection

		public string ConnectionString
		{
			get;
			set;
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public delegate DataTable QueryDatabaseAsync( string sql, string tableName );

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
							dataTable = new DataTable( tableName );
							adapter.Fill( dataTable );
						}
					}

					oracle.Close();

#if DEBUG
					timer.Stop();
					System.Diagnostics.Debug.Print( "{0}: {1} ({2})", timer.ElapsedMilliseconds, tableName, sql );
#endif

					return dataTable;
				}

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
							dataTable = new DataTableType();
							adapter.Fill( dataTable );
						}

					}

					oracle.Close();

#if DEBUG
					timer.Stop();
					System.Diagnostics.Debug.Print( "{0}: {1} ({2})", timer.ElapsedMilliseconds, dataTable.TableName, sql );
#endif

					return dataTable;
				}
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
		/// Fill a strongly typed datatable
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
			dataSet.Results.Tables.Add( dataTable );

			return dataSet;
		}

		public CadDataSet Execute( DatabaseQuery query, string dataSetName )
		{
			return Execute( query.Sql, query.TableName, dataSetName );
		}

		public CadDataSet Execute( DatabaseQuery[] queries, string dataSetName )
		{
			CadDataSet dataset = new CadDataSet( dataSetName );

			// initiate the queries
			for ( int i = 0; i != queries.Length; ++i )
			{
				DatabaseQuery query = queries[i];

				DataTable result = Execute( query.Sql, query.TableName );

				if ( result != null )
				{
					dataset.Results.Tables.Add( result );
				}
			}
			
			return dataset;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queries"></param>
		/// <param name="dataSetName"></param>
		/// <returns></returns>
		public CadDataSet ExecuteAsynch( DatabaseQuery[] queries, string dataSetName )
		{
			CadDataSet dataset = new CadDataSet( dataSetName );

			// create a new helpers list
			AsyncDBTask[] tasks = new AsyncDBTask[ queries.Length ];

			// initiate the queries
			for ( int i = 0; i != queries.Length; ++i )
			{
				DatabaseQuery q = queries[ i ];

				// create a new query helper
				tasks[ i ] = new AsyncDBTask();
				tasks[ i ].Object = dataset;
				tasks[ i ].Caller = Execute;
				tasks[ i ].AsyncResult = tasks[ i ].Caller.BeginInvoke( q.Sql, q.TableName, null, null );
			}

			// get the results from the queries
			foreach ( AsyncDBTask task in tasks )
			{
				DataTable result = task.Caller.EndInvoke( task.AsyncResult );

				if ( result != null )
				{
					task.Object.Results.Tables.Add( result );
				}
			}

			return dataset;
		}

		public void GetTableSchema( string schema, string[] tables, Stream result )
		{
			//string desc = "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH,"
			//              + "DATA_PRECISION, DATA_SCALE, NULLABLE, DATA_DEFAULT, CHAR_LENGTH "
			//              + "FROM ALL_TAB_COLUMNS "
			//              + "WHERE OWNER='EADEV' AND TABLE_NAME IN ('AEVEN' ) "
			//              + "ORDER BY TABLE_NAME ASC, COLUMN_ID ASC";

			string desc = "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH,"
							  + "DATA_PRECISION, DATA_SCALE, NULLABLE, DATA_DEFAULT, CHAR_LENGTH "
							  + "FROM ALL_TAB_COLUMNS "
							  + "WHERE UPPER( OWNER ) = '" + schema.ToUpper() + "' " 
							  + "AND UPPER( TABLE_NAME ) IN ('" + string.Join( "', '", tables ).ToUpper() + "') "
							  + "ORDER BY TABLE_NAME ASC, COLUMN_ID ASC";

			CadDataSet dataset = Execute( desc, "COLUMN", "TABLE_COLUMNS" );

			XslCompiledTransform xslTableToXsd = XsltHelper.Load( "DatabaseSchema.xslt", "Intergraph.AsiaPac.Data.Stylesheets" );

			if ( xslTableToXsd != null )
			{
				dataset.ToXml( xslTableToXsd, null, result );
				//dataset.ToXml().Save( @"c:\temp\dump.xml" );
			}
			else
			{
				dataset.ToXml( result );
			}
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

		#region IDisposable

		~DatabaseConnection()
		{
			Dispose( false );
		}

		private bool _disposed = false;

		public void Dispose()
		{
			Dispose( true );
		}

		protected void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				return;
			}

			if ( disposing )
			{
				GC.SuppressFinalize( this );
			}

			// free non managed resources

			_disposed = true;
		}


		#endregion
	}
}
