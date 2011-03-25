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
			List<UnitDetail> units = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					UnitDetailDataAdapter adapter = new UnitDetailDataAdapter( database );
					units = adapter.GetData( null, null, null );
				} );

			Assert.NotNull( units );
			Assert.NotEmpty( units );

			Debug.Print( "Number of units found: " + units.Count.ToString() );
		}

		[Fact]
		public void Can_query_unit_dispatch_assigned_from_database()
		{
			List<UnitDispatchAssignedDetail> units = null;
			
			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					UnitDispatchAssignedDetailDataAdapter adapter = new UnitDispatchAssignedDetailDataAdapter( database );
					units = adapter.GetData( null, null, null );
				} );

			Assert.NotNull( units );
			Assert.NotEmpty( units );

			Debug.Print( "Number of dispatch assigned units found: " + units.Count.ToString() );
		}


		[Fact]
		public void Can_query_unit_history_from_database()
		{
			List<UnitHistory> units = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					UnitHistoryDataAdapter adapter = new UnitHistoryDataAdapter( database );
					units = adapter.GetData( null, null, null );
				} );

			Assert.NotNull( units );
			Assert.NotEmpty( units );

			Debug.Print( "Number of units found: " + units.Count.ToString() );
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
