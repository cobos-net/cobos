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

			// disable the contraints when performing asynchronous queries,
			// otherwise, if the main table isn't filled, the child tables
			// will violate the key constraints.
			// this also improves performance.
			model.EnforceConstraints = false;

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
			//database.Execute( queries, model );

			timer.Stop();
			Debug.Print( "Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString() );

			// get the first row in the comments, use this to find an event with comments
			EventDataModel.EventCommentRow eventComment = model.EventComment[ 0 ];

			// find the event that owns this comment
			EventDataModel.AgencyEventRow agencyEvent = (EventDataModel.AgencyEventRow)model.AgencyEvent.Rows.Find( eventComment.AgencyEventId );

			Assert.NotNull( agencyEvent );

			EventDataModel.EventCommentRow[] comments = agencyEvent.GetEventComment();
			Debug.Print( "Found " + comments.Length.ToString() + " comments for " + agencyEvent.AgencyEventId );

			Assert.NotNull( comments );
			Assert.NotEmpty( comments );
			Assert.Contains<EventDataModel.EventCommentRow>( eventComment, comments );

			// get the first row in the disposition, use this to find an event with disposition
			EventDataModel.DispositionRow disposition = model.Disposition[ 0 ];

			agencyEvent = (EventDataModel.AgencyEventRow)model.AgencyEvent.Rows.Find( disposition.AgencyEventId );

			EventDataModel.DispositionRow[] dispositions = agencyEvent.GetDisposition();

			Assert.NotNull( dispositions );
			Assert.NotEmpty( dispositions );
			Assert.Contains<EventDataModel.DispositionRow>( disposition, dispositions );
			
			// Disposition is a one-to-one relationship
			Assert.Equal<int>( 1, dispositions.Length );
		}
	}
}
