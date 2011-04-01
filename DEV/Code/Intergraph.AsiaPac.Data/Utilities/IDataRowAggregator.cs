using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intergraph.AsiaPac.Data.Utilities
{
	/// <summary>
	/// Client provided custom row aggregator
	/// </summary>
	public interface IDataRowAggregator
	{
		/// <summary>
		/// Perform custom aggregation on the row group.
		/// The row collection is guaranteed to contain at least two rows.
		/// </summary>
		/// <param name="grouped"></param>
		/// <returns></returns>
		void Aggregate( List<DataRow> rows, DataRow result );

		/// <summary>
		/// Rows are aggregated based on key values.  Clients must  
		/// provide their own custom key generation behaviour.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		string GetKey( DataRow row );

		/// <summary>
		/// Process the 
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		DataTable Process( DataTable table );
	}
}
