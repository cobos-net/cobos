﻿// ============================================================================
// Filename: OracleDatabaseAdapter.cs
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
using System.Data;
using Oracle.DataAccess.Client;

#if INTERGRAPH_BRANDING
namespace Intergraph.AsiaPac.Data.Adapters
#else
namespace Cobos.Data.Adapters
#endif
{
	public class OracleDatabaseAdapter : DatabaseAdapter<OracleConnection, OracleCommand, OracleDataAdapter>
	{
		public OracleDatabaseAdapter( string connectionString )
			: base( connectionString )
		{
		}

		/// <summary>
		/// Oracle specific implementation of the metadata query.
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="tables"></param>
		/// <returns></returns>
		protected override CobosDataSet TableMetadata( string schema, string[] tables )
		{
			CobosDataSet result = new CobosDataSet( "TABLE_METADATA" );

			DataTable table = new DataTable( "TABLE" );
			table.Columns.Add( new DataColumn( "NAME", Type.GetType( "System.String" ) ) );

			foreach ( string t in tables )
			{
				DataRow row = table.NewRow();
				row[ "NAME" ] = t;
				table.Rows.Add( row );
			}

			result.Tables.Add( table );

			string columns = @"SELECT table_name, column_name, data_type, data_length, "
							  + "data_precision, data_scale, nullable, data_default, char_length "
							  + "FROM all_tab_columns "
							  + "WHERE UPPER( owner ) = '" + schema.ToUpper() + "' "
							  + "AND UPPER( table_name ) IN ('" + string.Join( "', '", tables ).ToUpper() + "') "
							  + "ORDER BY table_name ASC, column_id ASC";

			Fill( columns, "COLUMN", result );

			string constraints = "SELECT cols.table_name, cols.column_name, cols.position, cons.constraint_type, cons.constraint_name, cons.status "
										+ "FROM all_constraints cons, all_cons_columns cols "
										+ "WHERE cols.table_name IN ('" + string.Join( "', '", tables ).ToUpper() + "') "
										+ "AND cons.constraint_type IN ('P', 'R', 'U') "
										+ "AND cons.constraint_name = cols.constraint_name "
										+ "AND cons.owner = '" + schema.ToUpper() + "' "
										+ "AND cons.owner = cols.owner "
										+ "ORDER BY cols.table_name, cols.position";

			Fill( constraints, "CONSTRAINT", result );

			CobosDataSet.Relationship[] relations = new CobosDataSet.Relationship[]
			{
				new CobosDataSet.Relationship( "COLUMNS", "TABLE", "NAME", "COLUMN", "TABLE_NAME" ),
				new CobosDataSet.Relationship( "CONTSTRAINTS", "TABLE", "NAME", "CONSTRAINT", "TABLE_NAME" )
			};

			result.CreateRelationships( relations );

			return result;
		}
	}
}