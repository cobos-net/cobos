using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intergraph.AsiaPac.Data
{
	public interface IDatabaseAdapter
	{
		/// <summary>
		/// In some situations this may result in performance 
		/// improvements, if read-write access is not required.
		/// Set via the connection string. 
		/// Not supported on all platforms.
		/// </summary>
		bool ReadOnly
		{
			get;
		}

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
