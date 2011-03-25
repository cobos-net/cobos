using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.Events
{
	public class EventsTests
	{
		[Fact]
		public void Can_query_events_from_database()
		{
			List<AgencyEvent> events = null;

			Assert.DoesNotThrow( 
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					AgencyEventDataAdapter adapter = new AgencyEventDataAdapter( database );
					events = adapter.GetData( null, null, null );
				} );

			Assert.NotNull( events );
			Assert.NotEmpty( events );

			Debug.Print( "Number of events found: " + events.Count.ToString() );
		}

		[Fact]
		public void Can_query_event_summary_from_database()
		{
			List<AgencyEventSummary> events = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					AgencyEventSummaryDataAdapter adapter = new AgencyEventSummaryDataAdapter( database );
					events = adapter.GetData( null, null, null );
				} );

			Assert.NotNull( events );
			Assert.NotEmpty( events );

			Debug.Print( "Number of events found: " + events.Count.ToString() );
		}

		[Fact]
		public void Can_query_agencies_from_database()
		{
			List<Agencies> agencies = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					AgenciesDataAdapter adapter = new AgenciesDataAdapter( database );
					agencies = adapter.GetData( null, null, null );
				} );

			Assert.NotNull( agencies );
			Assert.NotEmpty( agencies );

			Debug.Print( "Number of agencies found: " + agencies.Count.ToString() );
		}
	}
}
