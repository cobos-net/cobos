using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.Events
{
	public class EventsTests
	{
		public const string OpenEventClause = "AEVEN.open_and_curent = 'T'";

		public static readonly string[] OpenEventFilter = new string[] { OpenEventClause };

		public static string[] GetOpenEventFilter( string where )
		{
			return new string[] { OpenEventClause, where };
		}


		[Fact]
		public void Can_query_events_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query agency events from the database using the typed data adapter.

			Stopwatch timer = new Stopwatch();
			List<AgencyEvent> events = null;

			Assert.DoesNotThrow( 
				delegate
				{
					timer.Start();
					events = AgencyEventDataAdapter.GetData( TestManager.DatabaseAdapter, EventsTests.OpenEventFilter, null );
					timer.Stop();
				} );

			Assert.NotNull( events );
			Assert.NotEmpty( events );

			Console.WriteLine( "Number of events found: " + events.Count.ToString() );
			Console.WriteLine( "Event query took: " + timer.ElapsedMilliseconds.ToString() );
		}

		[Fact]
		public void Can_query_event_detail_comments_and_disposition()
		{


		}

		[Fact]
		public void Can_query_event_summary_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query event summary information from the database using the typed data adapter.

			List<AgencyEventSummary> events = null;

			Assert.DoesNotThrow(
				delegate
				{
					events = AgencyEventSummaryDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			Assert.NotNull( events );
			Assert.NotEmpty( events );

			Console.WriteLine( "Number of events found: " + events.Count.ToString() );
		}

		[Fact]
		public void Can_query_agencies_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query agencies data from the database using the typed data adapter.

			List<Agencies> agencies = null;

			Assert.DoesNotThrow(
				delegate
				{
					agencies = AgenciesDataAdapter.GetData( TestManager.DatabaseAdapter, EventsTests.OpenEventFilter, null );
				} );

			Assert.NotNull( agencies );
			Assert.NotEmpty( agencies );

			Console.WriteLine( "Number of agencies found: " + agencies.Count.ToString() );
		}
	}
}
