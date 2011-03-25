using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using Intergraph.AsiaPac.Data.Tests.Events;

namespace Intergraph.AsiaPac.Data.Tests
{
	public class DatabaseAdapterTests
	{
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

			DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
			database.ExecuteAsynch( queries, model );

			timer.Stop();
			Debug.Print( "Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString() );

			Assert.NotEmpty( model.AgencyEvent.Rows );
			Assert.NotEmpty( model.EventComment.Rows );
			Assert.NotEmpty( model.Disposition.Rows );
			Assert.Empty( model.Agencies.Rows );
			Assert.Empty( model.AgencyEventSummary.Rows );
		}

		[Fact]
		public void Can_execute_nested_queries()
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

			DatabaseAdapter database = new DatabaseAdapter( TestManager.DatabaseConnection );
			database.ExecuteAsynch( queries, model );

			timer.Stop();
			Debug.Print( "Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString() );

			EventDataModel.EventCommentRow eventComment = model.EventComment[ 0 ];

			EventDataModel.AgencyEventRow agencyEvent = (EventDataModel.AgencyEventRow)model.AgencyEvent.Rows.Find( eventComment.AgencyEventId );

			Assert.NotNull( agencyEvent );
		}
	}
}
