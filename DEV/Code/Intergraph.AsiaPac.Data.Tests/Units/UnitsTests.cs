using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.Units
{
	public class UnitsTests
	{
		[Fact]
		public void Can_query_units_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query units from the database using the typed data adapter.

			List<UnitDetail> units = null;

			Assert.DoesNotThrow(
				delegate
				{
					units = UnitDetailDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			Assert.NotNull( units );
			Assert.NotEmpty( units );

			Console.WriteLine( "Number of units found: " + units.Count.ToString() );
		}

		[Fact]
		public void Can_query_unit_dispatch_assigned_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query dispatch assinged data from the database using the typed data adapter.

			List<UnitDispatchAssignedDetail> units = null;
			
			Assert.DoesNotThrow(
				delegate
				{
					units = UnitDispatchAssignedDetailDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			Assert.NotNull( units );
			Assert.NotEmpty( units );

			Console.WriteLine( "Number of dispatch assigned units found: " + units.Count.ToString() );
		}


		[Fact]
		public void Can_query_unit_history_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query unit history from the database using the typed data adapter.

			List<UnitHistory> units = null;

			Assert.DoesNotThrow(
				delegate
				{
					units = UnitHistoryDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			Assert.NotNull( units );
			Assert.NotEmpty( units );

			Console.WriteLine( "Number of units found: " + units.Count.ToString() );
		}


		[Fact]
		public void Can_query_agencies_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query unit agencies from the database using the typed data adapter.

			List<Agencies> agencies = null;

			Assert.DoesNotThrow(
				delegate
				{
					agencies = AgenciesDataAdapter.GetData( TestManager.DatabaseAdapter, null, null );
				} );

			Assert.NotNull( agencies );
			Assert.NotEmpty( agencies );

			Console.WriteLine( "Number of agencies found: " + agencies.Count.ToString() );
		}
	}
}
