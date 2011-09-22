// ============================================================================
// Filename: SqlDatabaseAdapter.cs
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
using System.Data.SqlClient;

#if INTERGRAPH_BRANDING
namespace Intergraph.AsiaPac.Data.Adapters
#else
namespace Cobos.Data.Adapters
#endif
{
	public class SqlDatabaseAdapter : DatabaseAdapter<SqlConnection, SqlCommand, SqlDataAdapter>
	{
		public SqlDatabaseAdapter( string connectionString )
			: base( connectionString )
		{
		}

		/// <summary>
		/// SqlServer specific implementation of the metadata query.
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="tables"></param>
		/// <returns></returns>
		protected override CobosDataSet TableMetadata( string schema, string[] tables )
		{
			throw new NotImplementedException();
		}
	}
}
