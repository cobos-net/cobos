using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

			Assert.DoesNotThrow( delegate
			{
				using ( DatabaseConnection dbconn = new DatabaseConnection( "Data Source=VEA795DB2.WORLD;User Id=eadev;Password=eadev" ) )
				{
					AgencyEventDataAdapter adapter = new AgencyEventDataAdapter( dbconn );
					events = adapter.GetData( null, null, null );
				}
			} );

			Assert.NotNull( events );
			Assert.NotEmpty( events );

			Debug.Print( events.Count.ToString() );
		}
	}
}
