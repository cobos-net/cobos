using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.SupplementalInformation
{
	public class SupplementalInformationTests
	{
		[Fact]
		public void Can_query_IncidentTimes()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query incident times supp info from the database using the typed data adapter.

			List<IncidentTimes> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					items = IncidentTimesDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Console.WriteLine( "Number of incident times found: " + items.Count.ToString() );
		}

		[Fact]
		public void Can_query_Person()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query person supp info from the database using the typed data adapter.
			
			List<Person> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					items = PersonDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Console.WriteLine( "Number of person items found: " + items.Count.ToString() );
		}

		[Fact]
		public void Can_query_Propt()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query property supp info from the database using the typed data adapter.

			List<Property> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					items = PropertyDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Console.WriteLine( "Number of property items found: " + items.Count.ToString() );
		}

		[Fact]
		public void Can_query_TowVehicle()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query tow vehicle supp info from the database using the typed data adapter.

			List<TowVehicle> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					items = TowVehicleDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Console.WriteLine( "Number of tow vehicle items found: " + items.Count.ToString() );
		}

		[Fact]
		public void Can_query_Vehicle()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query vehicle supp info from the database using the typed data adapter.

			List<Vehicle> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					items = VehicleDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Console.WriteLine( "Number of vehicle items found: " + items.Count.ToString() );
		}
	}
}
