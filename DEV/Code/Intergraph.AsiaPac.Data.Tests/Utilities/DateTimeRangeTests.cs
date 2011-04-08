using System;
using System.Text;
using Intergraph.IPS.Utility;
using Intergraph.AsiaPac.Data.Utilities;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.Utilities
{
	public class DateTimeRangeTests
	{
		public DateTimeRangeTests()
		{
			AppDomain.CurrentDomain.SetData( IPS.Utility.DateFormatter.UseUtcKey, false );
		}

		void GetTestData( out DateTime start, out DateTime end, out string startDts, out string endDts )
		{
			start = DateTime.Now.AddDays( -1 );
			end = DateTime.Now;

			startDts = DateFormatter.ConvertToDTS( start );
			endDts = DateFormatter.ConvertToDTS( end );
		}
		
		[Fact]
		public void Simple_date_range_values_succeed()
		{
			// Strategy
			// ----------------
			// 1) Confirm that a simple date range can be constructed and that the output is a valid where clause.

			DateTime start, end;
			string startDts, endDts;
			GetTestData( out start, out end, out startDts, out endDts );

			DateTimeRange range = new DateTimeRange( start, end );

			Assert.True( range.HasValue );
			Assert.True( range.StartDateSpecified );
			Assert.True( range.EndDateSpecified );

			string expected = "TEST_COL >= '" + startDts +"' AND TEST_COL <= '" + endDts +"'";
			Assert.Equal<string>( expected, range.ToString( "TEST_COL" ) );
		}

		[Fact]
		public void Can_handle_null_values()
		{
			// Strategy
			// ----------------
			// 1) Confirm that creating with null values succeeds and has no effect on output.
			// 2) Confirm that creating with the default constructor succeeds and has no effect on output.
			// 3) Confirm that creating with a null end date succeeds and creates a where clause for dates after a particular date.
			// 4) Confirm that creating with a null start date succeeds and creates a where clause for dates before a particular date.

			DateTime start, end;
			string startDts, endDts;
			GetTestData( out start, out end, out startDts, out endDts );

			DateTimeRange range = new DateTimeRange( null, null );
			Assert.False( range.HasValue );
			Assert.False( range.StartDateSpecified );
			Assert.False( range.EndDateSpecified );
			Assert.Equal<string>( "", range.ToString( "TEST_COL" ) );

			range = new DateTimeRange();
			Assert.False( range.HasValue );
			Assert.False( range.StartDateSpecified );
			Assert.False( range.EndDateSpecified );
			Assert.Equal<string>( "", range.ToString( "TEST_COL" ) );

			range = new DateTimeRange( start, null );
			Assert.True( range.HasValue );
			Assert.True( range.StartDateSpecified );
			Assert.False( range.EndDateSpecified );
			
			string expected = "TEST_COL >= '" + startDts + "'";
			Assert.Equal<string>( expected, range.ToString( "TEST_COL" ) );

			range = new DateTimeRange( null, start );
			Assert.True( range.HasValue );
			Assert.False( range.StartDateSpecified );
			Assert.True( range.EndDateSpecified );
			
			expected = "TEST_COL <= '" + startDts + "'";
			Assert.Equal<string>( expected, range.ToString( "TEST_COL" ) );
		}

		[Fact]
		public void Can_handle_invalid_input()
		{
			// Strategy
			// ----------------
			// 1) Switch the start and end dates and confirm that the result is still a valid time range

			DateTime start, end;
			string startDts, endDts;
			GetTestData( out start, out end, out startDts, out endDts );

			DateTimeRange range = new DateTimeRange( end, start );

			Assert.True( range.HasValue );
			Assert.True( range.StartDateSpecified );
			Assert.True( range.EndDateSpecified );

			string expected = "TEST_COL >= '" + startDts + "' AND TEST_COL <= '" + endDts + "'";
			Assert.Equal<string>( expected, range.ToString( "TEST_COL" ) );
		}

	}
}
