using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Data
{
	/// <summary>
	/// Allow multiple queries to be processed into the same dataset
	/// </summary>
	public class DatabaseQuery
	{
		public readonly string Sql;
		public readonly string TableName;

		public DatabaseQuery( string sql, string tableName )
		{
			Sql = sql;
			TableName = tableName;
		}
	}
}
