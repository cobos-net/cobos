using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.HospitalDiversion
{
	public class HospitalDiversionTests
	{
		[Fact]
		public void Can_query_comments_from_database()
		{
			List<HospitalDiversionMessage> messages = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					HospitalDiversionMessageDataAdapter adapter = new HospitalDiversionMessageDataAdapter( database );
					messages = adapter.GetData( null, null, null );
				} );

			Assert.NotNull( messages );
			Assert.NotEmpty( messages );

			Debug.Print( "Number of messages found: " + messages.Count.ToString() );
		}
	}
}
