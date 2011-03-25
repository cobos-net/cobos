﻿using System;
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
			List<IncidentTimes> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					IncidentTimesDataAdapter adapter = new IncidentTimesDataAdapter( database );
					items = adapter.GetData( null, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Debug.Print( "Number of incident times found: " + items.Count.ToString() );
		}

		[Fact]
		public void Can_query_Person()
		{
			List<Person> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					PersonDataAdapter adapter = new PersonDataAdapter( database );
					items = adapter.GetData( null, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Debug.Print( "Number of person items found: " + items.Count.ToString() );
		}

		[Fact]
		public void Can_query_Propt()
		{
			List<Property> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					PropertyDataAdapter adapter = new PropertyDataAdapter( database );
					items = adapter.GetData( null, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Debug.Print( "Number of property items found: " + items.Count.ToString() );
		}

		[Fact]
		public void Can_query_TowVehicle()
		{
			List<TowVehicle> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					TowVehicleDataAdapter adapter = new TowVehicleDataAdapter( database );
					items = adapter.GetData( null, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Debug.Print( "Number of tow vehicle items found: " + items.Count.ToString() );
		}

		[Fact]
		public void Can_query_Vehicle()
		{
			List<Vehicle> items = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
					VehicleDataAdapter adapter = new VehicleDataAdapter( database );
					items = adapter.GetData( null, null, null );
				} );

			//Assert.NotNull( items );
			//Assert.NotEmpty( items );

			Debug.Print( "Number of vehicle items found: " + items.Count.ToString() );
		}
	}
}
