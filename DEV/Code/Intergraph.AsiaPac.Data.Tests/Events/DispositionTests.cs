﻿using System;
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
			List<Disposition> dispo = null;

			Assert.DoesNotThrow( 
				delegate
				{
					using ( DatabaseConnection dbconn = new DatabaseConnection( TestManager.DatabaseConnection ) )
					{
						DispositionDataAdapter adapter = new DispositionDataAdapter( dbconn );
						dispo = adapter.GetData( null, null, null );
					}
				} );

			Assert.NotNull( dispo );
			Assert.NotEmpty( dispo );

			Debug.Print( "Number of items found: " + dispo.Count.ToString() );
		}
	}
}
