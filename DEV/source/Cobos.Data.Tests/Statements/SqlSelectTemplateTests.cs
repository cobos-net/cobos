// ============================================================================
// Filename: SqlSelectTemplateTests.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 21-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
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
using System.Linq;
using System.Text;
using NUnit.Framework;
using Cobos.Data.Statements;

namespace Cobos.Data.Tests.Statements
{
	[TestFixture]
	public class SqlSelectTemplateTests
	{
		[TestCase]
		public void Invalid_parameters_throws_exception()
		{
			Assert.DoesNotThrow(
				delegate
				{
					new SqlSelectTemplate();
				} );

			Assert.Throws<InvalidOperationException>(
				delegate
				{
					new SqlSelectTemplate().ToString();
				} );


			Assert.Throws<InvalidOperationException>(
				delegate
				{
					new SqlSelectTemplate().ToString( null, null, null, null, null, null );
				} );

			Assert.Throws<InvalidOperationException>(
				delegate
				{
					new SqlSelectTemplate( null, null, null, null, null, null, true );
				} );
		}

		[TestCase]
		public void Simple_select_succeeds()
		{

			Assert.DoesNotThrow(
				delegate
				{
					SqlSelectTemplate select = new SqlSelectTemplate( "COL", null, null, null, null, null );
					select.ToString();
					select.ToString( null );
					select.ToString( null, null );
					select.ToString( null, null, null );
					
					select = new SqlSelectTemplate();
					select.ToString( "COL", null, null, null, null, null );
				} );
		}

		[TestCase]
		public void Select_parameters_return_correct_query()
		{
			SqlSelectTemplate select = new SqlSelectTemplate( "COL", null, null, null, null, null );

			Assert.AreEqual( "SELECT COL", select.ToString() );
			Assert.AreEqual( "SELECT COL, COL", select.ToString( "COL", null, null, null, null, null ) );

			select = new SqlSelectTemplate();

			Assert.AreEqual( "SELECT COL", select.ToString( "COL", null, null, null, null, null ) );
		}

		[TestCase]
		public void From_parameters_return_correct_query()
		{
			SqlSelectTemplate select = new SqlSelectTemplate( "COL", "TABLE", null, null, null, null );

			Assert.AreEqual( "SELECT COL FROM TABLE", select.ToString() );
			Assert.AreEqual( "SELECT COL FROM TABLE, TABLE2", select.ToString( null, "TABLE2", null, null, null, null ) );

			select = new SqlSelectTemplate();

			Assert.AreEqual( "SELECT COL FROM TABLE", select.ToString( "COL", "TABLE", null, null, null, null ) );
		}

		[TestCase]
		public void Inner_join_parameters_return_correct_query()
		{
			string[] innerJoin1 = new string[] { "TABLE1" };
			string[] innerJoin2 = new string[] { "TABLE2", "TABLE3" };
			string[] innerJoin3 = new string[] { "TABLE4", "TABLE5", "TABLE6" };

			SqlSelectTemplate select = new SqlSelectTemplate( "COL", null, innerJoin1, null, null, null );

			Assert.AreEqual( "SELECT COL INNER JOIN TABLE1", select.ToString() );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE1", select.ToString( null, null, innerJoin1, null, null, null ) );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString( null, null, innerJoin2, null, null, null ) );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString( null, null, innerJoin3, null, null, null ) );

			select = new SqlSelectTemplate( "COL", null, innerJoin2, null, null, null );

			Assert.AreEqual( "SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString() );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE1", select.ToString( null, null, innerJoin1, null, null, null ) );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString( null, null, innerJoin2, null, null, null ) );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString( null, null, innerJoin3, null, null, null ) );

			select = new SqlSelectTemplate( "COL", null, innerJoin3, null, null, null );

			Assert.AreEqual( "SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString() );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE1", select.ToString( null, null, innerJoin1, null, null, null ) );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString( null, null, innerJoin2, null, null, null ) );
			Assert.AreEqual( "SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString( null, null, innerJoin3, null, null, null ) );
		}

		[TestCase]
		public void Where_parameters_return_correct_query()
		{
			string[] whereClause1 = new string[] { "CLAUSE1" };
			string[] whereClause2 = new string[] { "CLAUSE2", "CLAUSE3" };
			string[] whereClause3 = new string[] { "CLAUSE4", "CLAUSE5", "CLAUSE6" };

			SqlSelectTemplate select = new SqlSelectTemplate( "COL", null, null, whereClause1, null, null );

			Assert.AreEqual( "SELECT COL WHERE (CLAUSE1)", select.ToString() );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString( null, null, null, whereClause1, null, null ) );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE3)", select.ToString( null, null, null, whereClause2, null, null ) );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString( null, null, null, whereClause3, null, null ) );

			// test the shortcut methods
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString( whereClause1, null, null ) );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString( whereClause1, null ) );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString( whereClause1 ) );

			select = new SqlSelectTemplate( "COL", null, null, whereClause2, null, null );

			Assert.AreEqual( "SELECT COL WHERE (CLAUSE2) AND (CLAUSE3)", select.ToString() );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE1)", select.ToString( null, null, null, whereClause1, null, null ) );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE2) AND (CLAUSE3)", select.ToString( null, null, null, whereClause2, null, null ) );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString( null, null, null, whereClause3, null, null ) );

			select = new SqlSelectTemplate( "COL", null, null, whereClause3, null, null );

			Assert.AreEqual( "SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString() );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE1)", select.ToString( null, null, null, whereClause1, null, null ) );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE2) AND (CLAUSE3)", select.ToString( null, null, null, whereClause2, null, null ) );
			Assert.AreEqual( "SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString( null, null, null, whereClause3, null, null ) );
		}

		[TestCase]
		public void GroupBy_parameters_return_correct_query()
		{
			SqlSelectTemplate select = new SqlSelectTemplate( "COL", null, null, null, "GROUPCOL", null );

			Assert.AreEqual( "SELECT COL GROUP BY GROUPCOL", select.ToString() );
			Assert.AreEqual( "SELECT COL GROUP BY GROUPCOL, GROUPCOL2", select.ToString( null, null, null, null, "GROUPCOL2", null ) );

			select = new SqlSelectTemplate();
			Assert.AreEqual( "SELECT COL GROUP BY GROUPCOL", select.ToString( "COL", null, null, null, "GROUPCOL", null ) );

			select = new SqlSelectTemplate( "COL", null, null, null, null, null );
			Assert.AreEqual( "SELECT COL GROUP BY GROUPCOL", select.ToString( null, "GROUPCOL", null ) );
		}

		[TestCase]
		public void OrderBy_parameters_return_correct_query()
		{
			SqlSelectTemplate select = new SqlSelectTemplate( "COL", null, null, null, null, "ORDERCOL" );

			Assert.AreEqual( "SELECT COL ORDER BY ORDERCOL", select.ToString() );
			Assert.AreEqual( "SELECT COL ORDER BY ORDERCOL, ORDERCOL2", select.ToString( null, null, null, null, null, "ORDERCOL2" ) );

			select = new SqlSelectTemplate();
			Assert.AreEqual( "SELECT COL ORDER BY ORDERCOL", select.ToString( "COL", null, null, null, null, "ORDERCOL" ) );

			select = new SqlSelectTemplate( "COL", null, null, null, null, null );
			Assert.AreEqual( "SELECT COL ORDER BY ORDERCOL", select.ToString( null, null, "ORDERCOL" ) );
			Assert.AreEqual( "SELECT COL ORDER BY ORDERCOL", select.ToString( null, "ORDERCOL" ) );
		}

		[TestCase]
		public void Full_query_returns_correct_query()
		{
			string[] innerJoin = new string[] { "JOIN1", "JOIN2" };
			string[] whereClause = new string[] { "CLAUSE1", "CLAUSE2" };

			SqlSelectTemplate select = new SqlSelectTemplate( "COL", "TABLE1", innerJoin, whereClause, "GROUPCOL", "ORDERCOL" );

			Assert.AreEqual( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString() );
			Assert.AreEqual( "SELECT COL, COL2 FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( "COL2", null, null, null, null, null ) );
			Assert.AreEqual( "SELECT COL FROM TABLE1, TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( null, "TABLE2", null, null, null, null ) );
			Assert.AreEqual( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( null, null, innerJoin, null, null, null ) );
			Assert.AreEqual( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( null, null, null, whereClause, null, null ) );
			Assert.AreEqual( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL, GROUPCOL2 ORDER BY ORDERCOL", select.ToString( null, null, null, null, "GROUPCOL2", null ) );
			Assert.AreEqual( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL, ORDERCOL2", select.ToString( null, null, null, null, null, "ORDERCOL2" ) );

			select = new SqlSelectTemplate();
			Assert.AreEqual( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( "COL", "TABLE1", innerJoin, whereClause, "GROUPCOL", "ORDERCOL" ) );
			
			select = new SqlSelectTemplate( "COL", null, null, null, null, null );
			Assert.AreEqual( "SELECT COL, COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelectTemplate( null, "TABLE1", null, null, null, null );
			Assert.AreEqual( "SELECT COL2 FROM TABLE1, TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelectTemplate( null, null, innerJoin, null, null, null );
			Assert.AreEqual( "SELECT COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelectTemplate( null, null, null, whereClause, null, null );
			Assert.AreEqual( "SELECT COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelectTemplate( null, null, null, null, "GROUPCOL", null );
			Assert.AreEqual( "SELECT COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL, GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelectTemplate( null, null, null, null, null, "ORDERCOL" );
			Assert.AreEqual( "SELECT COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL, ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
		}

		[TestCase]
		public void Empty_arrays_dont_fail()
		{
			string[] empty = new string[] { };
			string[] innerJoin = new string[] { "JOIN1", "JOIN2" };
			string[] where = new string[] { "CLAUSE1", "CLAUSE2" };

			SqlSelectTemplate select = new SqlSelectTemplate( "COL", null, empty, empty, null, null );

			Assert.DoesNotThrow(
				delegate
				{
					select.ToString();
					select.ToString( null, null, empty, empty, null, null );
					select.ToString( empty );
					select.ToString( empty, null );
					select.ToString( empty, null, null );
				} );

			Assert.AreEqual( "SELECT COL", select.ToString() );
			Assert.AreEqual( "SELECT COL", select.ToString( null, null, empty, empty, null, null ) );
			Assert.AreEqual( "SELECT COL INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2)", select.ToString( null, null, innerJoin, where, null, null ) );

			select = new SqlSelectTemplate( "COL", null, innerJoin, where, null, null );

			Assert.AreEqual( "SELECT COL INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2)", select.ToString( null, null, empty, empty, null, null ) );
		}

		[TestCase]
		public void Buffered_query_returns_same_result()
		{
			string[] innerJoin = new string[] { "JOIN1", "JOIN2" };
			string[] whereClause = new string[] { "CLAUSE1", "CLAUSE2" };

			SqlSelectTemplate select = new SqlSelectTemplate( "COL", "TABLE1", innerJoin, whereClause, "GROUPCOL", "ORDERCOL" );
			SqlSelectTemplate buffered = new SqlSelectTemplate( "COL", "TABLE1", innerJoin, whereClause, "GROUPCOL", "ORDERCOL", true );

			Assert.AreEqual( buffered.ToString(), select.ToString() );
		}

	}
}
