using System;
using System.Data;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;
using Oracle.DataAccess.Client;
using Intergraph.Oz.Utilities;
using Intergraph.Oz.Utilities.Xml;

namespace Intergraph.Oz.Data
{
	using AsyncDBTask = AsyncTask<CadDataSet, Database.QueryDatabaseAsync>;

	public class Database : IDisposable
	{
		#region Private data

		XslCompiledTransform _xslTableToXsd;

		#endregion

		#region Singleton Instance

		private static Database _instance = null;

		private Database()
		{
			_xslTableToXsd = XsltHelper.Load( "DBSchema.xslt", "Intergraph.Oz.Data.Stylesheets" );
		}

		public static Database Instance
		{
			get
			{
				if ( _instance == null )
				{
					_instance = new Database();
				}
				else if ( _instance._disposed )
				{
					throw new ObjectDisposedException( "CadWpf.Data.Database", "The database has already been disposed." );
				}
				return _instance;
			}
		}

		#endregion

		#region Database Connection

		OracleConnection _oracle;

		public OracleConnection Connection
		{
			get { return _oracle; }
		}

		public void Connect( string connection )
		{
			if ( _oracle != null )
			{
				return;
			}

			try
			{
				_oracle = new OracleConnection( connection );
				_oracle.Open();
			}
			catch ( Exception )
			{
				_oracle = null;
				throw;
			}
		}

		public void Disconnect()
		{
			if ( _oracle == null )
			{
				return;
			}

			_oracle.Close();
			_oracle.Dispose();
			_oracle = null;
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
				Stopwatch timer = new Stopwatch();
				timer.Start();

				using ( OracleCommand command = new OracleCommand() )
				{
					command.Connection = _oracle;
					command.CommandText = sql;

					dataTable = new DataTable( tableName );

					using ( OracleDataAdapter adapter = new OracleDataAdapter( command ) )
					{
						adapter.Fill( dataTable );
					}

					timer.Stop();
					System.Diagnostics.Debug.Print( "{0}: {1} ({2})", timer.ElapsedMilliseconds, tableName, sql );

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

		public void GetTableSchema( string schema, string table, Stream result )
		{
			//string desc = "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH,"
			//              + "DATA_PRECISION, DATA_SCALE, NULLABLE, DATA_DEFAULT, CHAR_LENGTH "
			//              + "FROM ALL_TAB_COLUMNS "
			//              + "WHERE OWNER='EADEV' AND TABLE_NAME IN ('AEVEN' ) "
			//              + "ORDER BY TABLE_NAME ASC, COLUMN_ID ASC";

			string desc = "SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH,"
							  + "DATA_PRECISION, DATA_SCALE, NULLABLE, DATA_DEFAULT, CHAR_LENGTH "
							  + "FROM ALL_TAB_COLUMNS "
							  + "WHERE UPPER( OWNER ) = '" + schema.ToUpper() + "' AND UPPER( TABLE_NAME ) = '" + table.ToUpper() + "' "
							  + "ORDER BY TABLE_NAME ASC, COLUMN_ID ASC";

			CadDataSet dataset = Execute( desc, "COLUMN", "TABLE" );

			if ( _xslTableToXsd != null )
			{
				dataset.ToXml( _xslTableToXsd, null, result );
			}
			else
			{
				dataset.ToXml( result );
			}
		}

		#region IDisposable

		~Database()
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
				Disconnect();

				GC.SuppressFinalize( this );
			}

			// free non managed resources

			_disposed = true;
		}


		#endregion
	}
}
