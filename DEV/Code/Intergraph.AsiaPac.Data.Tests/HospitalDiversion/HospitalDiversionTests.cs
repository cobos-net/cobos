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
		public void Can_query_hospital_diversion_warnings_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query hospital diversion warnings from the database using the typed data adapter.

			List<HospitalDiversionMessage> messages = null;

			Assert.DoesNotThrow(
				delegate
				{
					messages = HospitalDiversionMessageDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			Assert.NotNull( messages );
			Assert.NotEmpty( messages );

			Console.WriteLine( "Number of messages found: " + messages.Count.ToString() );
		}
	}
}
