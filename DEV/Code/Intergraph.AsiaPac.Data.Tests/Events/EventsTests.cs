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
					using ( DatabaseConnection dbconn = new DatabaseConnection( TestManager.DatabaseConnection ) )
					{
						AgencyEventDataAdapter adapter = new AgencyEventDataAdapter( dbconn );
						events = adapter.GetData( null, null, null );
					}
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
					using ( DatabaseConnection dbconn = new DatabaseConnection( TestManager.DatabaseConnection ) )
					{
						AgencyEventSummaryDataAdapter adapter = new AgencyEventSummaryDataAdapter( dbconn );
						events = adapter.GetData( null, null, null );
					}
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
					using ( DatabaseConnection dbconn = new DatabaseConnection( TestManager.DatabaseConnection ) )
					{
						AgenciesDataAdapter adapter = new AgenciesDataAdapter( dbconn );
						agencies = adapter.GetData( null, null, null );
					}
				} );

			Assert.NotNull( agencies );
			Assert.NotEmpty( agencies );

			Debug.Print( "Number of agencies found: " + agencies.Count.ToString() );
		}

		[Fact]
		public void Can_execute_asynchronous_database_queries()
		{
			EventDataModel model = new EventDataModel();

			DatabaseQuery[] queries = new DatabaseQuery[] 
			{ 
				new DatabaseQuery( AgencyEventDataAdapter.Select.ToString(), model.AgencyEvent.TableName ),
				new DatabaseQuery( EventCommentDataAdapter.Select.ToString(), model.EventComment.TableName ),
				new DatabaseQuery( DispositionDataAdapter.Select.ToString(), model.Disposition.TableName )
			};

			Stopwatch timer = new Stopwatch();
			timer.Start();

			using ( DatabaseConnection dbconn = new DatabaseConnection( TestManager.DatabaseConnection ) )
			{
				dbconn.ExecuteAsynch( queries, model );
			}

			timer.Stop();
			Debug.Print( "Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString() );

			Assert.NotEmpty( model.AgencyEvent.Rows );
			Assert.NotEmpty( model.EventComment.Rows );
			Assert.NotEmpty( model.Disposition.Rows );
		}
	}
}
