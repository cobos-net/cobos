// ============================================================================
// Filename: DatabaseQuery.cs
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

using System;
using System.Data;

using Cobos.Data.Utilities;

namespace Cobos.Data
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
