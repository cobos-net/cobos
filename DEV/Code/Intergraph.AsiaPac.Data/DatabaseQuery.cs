using System;
using System.Data;
using Intergraph.AsiaPac.Data.Utilities;

namespace Intergraph.AsiaPac.Data
{
	/// <summary>
	/// Allow multiple table queries to be processed ansynchronously
	/// </summary>
	public class DatabaseQuery
	{
		/// <summary>
		/// The sql statement to run to fill the Table object
		/// </summary>
		public readonly string Sql;

		/// <summary>
		/// The data table to populate with the query result.
		/// </summary>
		public readonly DataTable Table;

		/// <summary>
		/// Attach a custom aggregator to the query
		/// </summary>
		public readonly IDataRowAggregator Aggregator;

		/// <summary>
		/// Simple constructor to create a query
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="table"></param>
		public DatabaseQuery( string sql, DataTable table )
		{
			Sql = sql;
			Table = table;
		}

		/// <summary>
		/// Constructor to create a query with a custom aggregator
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="table"></param>
		/// <param name="aggregator"></param>
		public DatabaseQuery( string sql, DataTable table, IDataRowAggregator aggregator )
			: this( sql, table )
		{
			Aggregator = aggregator;
		}

		/// <summary>
		/// Get the table result.  If an aggregator is provided, it will
		/// return the processed table.
		/// </summary>
		/// <returns></returns>
		public DataTable GetResult()
		{
			if ( Aggregator != null )
			{
				return Aggregator.Process( Table );
			}
			else
			{
				return Table;
			}
		}
	}
}
