using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intergraph.AsiaPac.Data
{
	public interface IDatabaseAdapter
	{
		DataTable Execute( string sql, string tableName );

		void Execute( string sql, DataTable result );

		DataTableType Execute<DataTableType>( string sql ) where DataTableType : DataTable, new();

		DataSetType Execute<DataSetType>( string sql, string tableName ) where DataSetType : DataSet, new();

		void Execute( string sql, string tableName, DataSet dataset );
		
		void Execute( DatabaseQuery query );

		void Execute( DatabaseQuery[] queries );

		void ExecuteAsynch( DatabaseQuery[] queries );
	}
}
