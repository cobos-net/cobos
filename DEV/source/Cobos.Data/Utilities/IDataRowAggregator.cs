// ============================================================================
// Filename: IDataRowAggregator.cs
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
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Cobos.Data.Utilities
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
