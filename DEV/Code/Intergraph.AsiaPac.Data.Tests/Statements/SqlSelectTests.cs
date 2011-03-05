﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Intergraph.AsiaPac.Data.Statements;

namespace Intergraph.AsiaPac.Data.Tests.Statements
{
	public class SqlSelectTests
	{
		[Fact]
		public void Invalid_parameters_throws_exception()
		{
			SqlSelect select = new SqlSelect();

			Assert.Throws<InvalidOperationException>(
				delegate
				{
					select.ToString();
				} );


			Assert.Throws<InvalidOperationException>(
				delegate
				{
					select.ToString( null, null, null, null, null, null );
				} );
		}

		[Fact]
		public void Simple_select_succeeds()
		{
			SqlSelect select = new SqlSelect( "COL", null, null, null, null, null );

			Assert.DoesNotThrow(
				delegate
				{
					select.ToString();
				} );

			select = new SqlSelect();

			Assert.DoesNotThrow(
				delegate
				{
					select.ToString( "COL", null, null, null, null, null );
				} );
		}

		[Fact]
		public void Select_parameters_return_correct_query()
		{
			SqlSelect select = new SqlSelect( "COL", null, null, null, null, null );

			Assert.Equal<string>( "SELECT COL", select.ToString() );
			Assert.Equal<string>( "SELECT COL, COL", select.ToString( "COL", null, null, null, null, null ) );

			select = new SqlSelect();

			Assert.Equal<string>( "SELECT COL", select.ToString( "COL", null, null, null, null, null ) );
		}

		[Fact]
		public void From_parameters_return_correct_query()
		{
			SqlSelect select = new SqlSelect( "COL", "TABLE", null, null, null, null );

			Assert.Equal<string>( "SELECT COL FROM TABLE", select.ToString() );
			Assert.Equal<string>( "SELECT COL FROM TABLE, TABLE2", select.ToString( null, "TABLE2", null, null, null, null ) );

			select = new SqlSelect();

			Assert.Equal<string>( "SELECT COL FROM TABLE", select.ToString( "COL", "TABLE", null, null, null, null ) );
		}

		[Fact]
		public void Inner_join_parameters_return_correct_query()
		{
			string[] innerJoin1 = new string[] { "TABLE1" };
			string[] innerJoin2 = new string[] { "TABLE2", "TABLE3" };
			string[] innerJoin3 = new string[] { "TABLE4", "TABLE5", "TABLE6" };

			SqlSelect select = new SqlSelect( "COL", null, innerJoin1, null, null, null );

			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE1", select.ToString() );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE1", select.ToString( null, null, innerJoin1, null, null, null ) );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString( null, null, innerJoin2, null, null, null ) );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString( null, null, innerJoin3, null, null, null ) );

			select = new SqlSelect( "COL", null, innerJoin2, null, null, null );

			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString() );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE1", select.ToString( null, null, innerJoin1, null, null, null ) );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString( null, null, innerJoin2, null, null, null ) );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString( null, null, innerJoin3, null, null, null ) );

			select = new SqlSelect( "COL", null, innerJoin3, null, null, null );

			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString() );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE1", select.ToString( null, null, innerJoin1, null, null, null ) );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString( null, null, innerJoin2, null, null, null ) );
			Assert.Equal<string>( "SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString( null, null, innerJoin3, null, null, null ) );
		}

		[Fact]
		public void Where_parameters_return_correct_query()
		{
			string[] whereClause1 = new string[] { "CLAUSE1" };
			string[] whereClause2 = new string[] { "CLAUSE2", "CLAUSE3" };
			string[] whereClause3 = new string[] { "CLAUSE4", "CLAUSE5", "CLAUSE6" };

			SqlSelect select = new SqlSelect( "COL", null, null, whereClause1, null, null );

			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE1)", select.ToString() );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString( null, null, null, whereClause1, null, null ) );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE3)", select.ToString( null, null, null, whereClause2, null, null ) );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE1) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString( null, null, null, whereClause3, null, null ) );

			select = new SqlSelect( "COL", null, null, whereClause2, null, null );

			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE2) AND (CLAUSE3)", select.ToString() );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE1)", select.ToString( null, null, null, whereClause1, null, null ) );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE2) AND (CLAUSE3)", select.ToString( null, null, null, whereClause2, null, null ) );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString( null, null, null, whereClause3, null, null ) );

			select = new SqlSelect( "COL", null, null, whereClause3, null, null );

			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString() );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE1)", select.ToString( null, null, null, whereClause1, null, null ) );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE2) AND (CLAUSE3)", select.ToString( null, null, null, whereClause2, null, null ) );
			Assert.Equal<string>( "SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString( null, null, null, whereClause3, null, null ) );
		}

		[Fact]
		public void GroupBy_parameters_return_correct_query()
		{
			SqlSelect select = new SqlSelect( "COL", null, null, null, "GROUPCOL", null );

			Assert.Equal<string>( "SELECT COL GROUP BY GROUPCOL", select.ToString() );
			Assert.Equal<string>( "SELECT COL GROUP BY GROUPCOL, GROUPCOL2", select.ToString( null, null, null, null, "GROUPCOL2", null ) );

			select = new SqlSelect();

			Assert.Equal<string>( "SELECT COL GROUP BY GROUPCOL", select.ToString( "COL", null, null, null, "GROUPCOL", null ) );
		}

		[Fact]
		public void OrderBy_parameters_return_correct_query()
		{
			SqlSelect select = new SqlSelect( "COL", null, null, null, null, "ORDERCOL" );

			Assert.Equal<string>( "SELECT COL ORDER BY ORDERCOL", select.ToString() );
			Assert.Equal<string>( "SELECT COL ORDER BY ORDERCOL, ORDERCOL2", select.ToString( null, null, null, null, null, "ORDERCOL2" ) );

			select = new SqlSelect();

			Assert.Equal<string>( "SELECT COL ORDER BY ORDERCOL", select.ToString( "COL", null, null, null, null, "ORDERCOL" ) );
		}

		[Fact]
		public void Full_query_returns_correct_query()
		{
			string[] innerJoin = new string[] { "JOIN1", "JOIN2" };
			string[] whereClause = new string[] { "CLAUSE1", "CLAUSE2" };

			SqlSelect select = new SqlSelect( "COL", "TABLE1", innerJoin, whereClause, "GROUPCOL", "ORDERCOL" );

			Assert.Equal<string>( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString() );
			Assert.Equal<string>( "SELECT COL, COL2 FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( "COL2", null, null, null, null, null ) );
			Assert.Equal<string>( "SELECT COL FROM TABLE1, TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( null, "TABLE2", null, null, null, null ) );
			Assert.Equal<string>( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( null, null, innerJoin, null, null, null ) );
			Assert.Equal<string>( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( null, null, null, whereClause, null, null ) );
			Assert.Equal<string>( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL, GROUPCOL2 ORDER BY ORDERCOL", select.ToString( null, null, null, null, "GROUPCOL2", null ) );
			Assert.Equal<string>( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL, ORDERCOL2", select.ToString( null, null, null, null, null, "ORDERCOL2" ) );

			select = new SqlSelect();
			Assert.Equal<string>( "SELECT COL FROM TABLE1 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString( "COL", "TABLE1", innerJoin, whereClause, "GROUPCOL", "ORDERCOL" ) );
			
			select = new SqlSelect( "COL", null, null, null, null, null );
			Assert.Equal<string>( "SELECT COL, COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelect( null, "TABLE1", null, null, null, null );
			Assert.Equal<string>( "SELECT COL2 FROM TABLE1, TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelect( null, null, innerJoin, null, null, null );
			Assert.Equal<string>( "SELECT COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelect( null, null, null, whereClause, null, null );
			Assert.Equal<string>( "SELECT COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelect( null, null, null, null, "GROUPCOL", null );
			Assert.Equal<string>( "SELECT COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL, GROUPCOL2 ORDER BY ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
			
			select = new SqlSelect( null, null, null, null, null, "ORDERCOL" );
			Assert.Equal<string>( "SELECT COL2 FROM TABLE2 INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL, ORDERCOL2", select.ToString( "COL2", "TABLE2", innerJoin, whereClause, "GROUPCOL2", "ORDERCOL2" ) );
		}

		[Fact]
		public void Empty_arrays_dont_fail()
		{
			string[] empty = new string[] { };
			string[] innerJoin = new string[] { "JOIN1", "JOIN2" };
			string[] where = new string[] { "CLAUSE1", "CLAUSE2" };

			SqlSelect select = new SqlSelect( "COL", null, empty, empty, null, null );

			Assert.DoesNotThrow(
				delegate
				{
					select.ToString();
				} );

			Assert.DoesNotThrow(
				delegate
				{
					select.ToString( null, null, empty, empty, null, null );
				} );

			Assert.Equal<string>( "SELECT COL", select.ToString() );
			Assert.Equal<string>( "SELECT COL", select.ToString( null, null, empty, empty, null, null ) );
			Assert.Equal<string>( "SELECT COL INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2)", select.ToString( null, null, innerJoin, where, null, null ) );

			select = new SqlSelect( "COL", null, innerJoin, where, null, null );

			Assert.Equal<string>( "SELECT COL INNER JOIN JOIN1 INNER JOIN JOIN2 WHERE (CLAUSE1) AND (CLAUSE2)", select.ToString( null, null, empty, empty, null, null ) );
		}
	}
}