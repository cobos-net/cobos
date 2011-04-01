using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.Events
{
	public class DispositionTests
	{
		[Fact]
		public void Can_query_disposition_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query disposition data from the database using the typed data adapter.

			List<Disposition> dispo = null;

			Assert.DoesNotThrow( 
				delegate
				{
					dispo = DispositionDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			Assert.NotNull( dispo );
			Assert.NotEmpty( dispo );

			Console.WriteLine( "Number of items found: " + dispo.Count.ToString() );
		}
	}
}
