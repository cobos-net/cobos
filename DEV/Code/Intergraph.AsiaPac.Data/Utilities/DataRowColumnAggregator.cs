// ============================================================================
// Filename: DataRowColumnAggregator.cs
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
using System.Collections.Generic;
using System.Data;
using System.Text;

#if INTERGRAPH_BRANDING
namespace Intergraph.AsiaPac.Data.Utilities
#else
namespace Cobos.Data.Utilities
#endif
{
	/// <summary>
	/// 
	/// </summary>
	public class DataRowColumnAggregator : IDataRowAggregator
	{
		/// <summary>
		/// Specifies the column to aggregate on.
		/// </summary>
		readonly DataColumnDescriptor AggregateOn;

		/// <summary>
		/// Specifies the column order to perform the groupings for key generation.
		/// </summary>
		readonly DataColumnDescriptor[] GroupBy;

		/// <summary>
		/// Specifies the column order to sort the groupings by. Can be null if
		/// the columns are already sorted by the database query.
		/// </summary>
		readonly DataRowColumnComparer SortBy;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aggregateOn">The column to aggregate on.</param>
		/// <param name="groupBy">The columns to group the aggregation by.</param>
		/// <param name="sortBy">The sort order for performing the aggregation.</param>
		public DataRowColumnAggregator( DataColumnDescriptor aggregateOn, DataColumnDescriptor[] groupBy, DataRowColumnComparer sortBy )
		{
			if ( aggregateOn.DataType != typeof( string ) )
			{
				throw new InvalidOperationException( "DataColumnAggregator.DataColumnAggregator: Can only aggregate on string columns.  Column is type: " + aggregateOn.DataType.Name );
			}

			AggregateOn = aggregateOn;
			GroupBy = groupBy;
			SortBy = sortBy;
		}

		/// <summary>
		/// Convenience constructor for creating from DataTable objects
		/// </summary>
		/// <param name="aggregateOn"></param>
		/// <param name="groupBy"></param>
		/// <param name="sortBy"></param>
		public DataRowColumnAggregator( DataColumn aggregateOn, DataColumn[] groupBy, DataRowColumnComparer sortBy )
			: this( new DataColumnDescriptor( aggregateOn ), DataColumnDescriptor.ConstructFrom( groupBy ), sortBy )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public DataTable Process( DataTable table )
		{
			return DataRowHelper.Aggregate( table, AggregateOn, this, true );
		}

		/// <summary>
		/// Called by DataRowHelper class to generate the row key.
		/// </summary>
		/// <param name="row">The row to generate the key for</param>
		/// <returns>The string key for the row.</returns>
		public string GetKey( DataRow row )
		{
			return DataRowHelper.GenerateRowKey( row, GroupBy );
		}

		/// <summary>
		/// Called by DataRowHelper to aggregate the row collection.  
		/// The row collection is guaranteed to contain at least two rows.
		/// </summary>
		/// <param name="rows">The grouped rows.</param>
		/// <param name="result">The row to store the aggregated result.</param>
		public void Aggregate( List<DataRow> rows, DataRow result )
		{
			if ( SortBy != null )
			{
				rows.Sort( SortBy );
			}

			int ordinal = AggregateOn.Ordinal;

			// construct the aggregate string.  each string value is trimmed
			// and the values are concatenated with a delimiting ' ' space.
			StringBuilder aggregate = new StringBuilder( 512 );

			for ( int r = 0; r < rows.Count; ++r )
			{
				DataRow row = rows[ r ];

				if ( row.IsNull( ordinal ) )
				{
					continue;
				}

				if ( aggregate.Length > 0 )
				{
					aggregate.Append( " " );
				}

				// we know this column is a string, this is checked in the constructor.
				string columnValue = (string)row[ ordinal ];

				aggregate.Append( columnValue.Trim() );
			}

			// copy values to the result row
			result.ItemArray = (object[])rows[ 0 ].ItemArray.Clone();
			
			// replace the aggregated column with the result
			result[ ordinal ] = aggregate.ToString();
		}

	}
}
