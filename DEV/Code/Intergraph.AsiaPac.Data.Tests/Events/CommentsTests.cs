using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Intergraph.AsiaPac.Data.Utilities;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.Events
{
	public class CommentsTests
	{
		[Fact]
		public void Can_query_comments_from_database()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can query comments from the database using the typed data adapter.
			// 2) Assert that the comments have been automatically aggregated by confirming there
			//		are no duplicate groups.

			List<EventComment> comments = null;

			Assert.DoesNotThrow(
				delegate
				{
					Stopwatch timer = new Stopwatch();
					timer.Start();

					comments = EventCommentDataAdapter.GetData( TestManager.DatabaseAdapter, EventsTests.OpenEventFilter, null );

					timer.Stop();
					Console.WriteLine( "Comments took: " + timer.ElapsedMilliseconds.ToString() );
				} );

			Assert.NotNull( comments );
			Assert.NotEmpty( comments );

			//HashSet<string> set = new HashSet<string>(); 

			//foreach ( EventComment comment in comments )
			//{
			//   string key = comment.Created.Timestamp.ToString( "s" ) 
			//               + "," + comment.Created.Terminal
			//               + "," + comment.Created.EmployeeId;

			//   bool isNew = set.Add( key );

			//   if ( !isNew )
			//   {
			//      Console.WriteLine( "Duplicate key: " + key );
			//   }
			//}

			Console.WriteLine( "Number of comments found: " + comments.Count.ToString() );
		}

		[Fact]
		public void Can_aggregate_comments()
		{
			// Strategy:
			// ---------
			// 1) Assert that we can aggregate the strongly typed EventCommentDataTable.

			Stopwatch timer = new Stopwatch();
			timer.Start();

			EventDataModel.EventCommentDataTable table = null;

			Assert.DoesNotThrow(
				delegate
				{
					DatabaseAdapter database = new DatabaseAdapter( TestManager.ConnectionString );
					string select = EventCommentDataAdapter.Select.ToString( EventsTests.OpenEventFilter );
					table = database.Execute<EventDataModel.EventCommentDataTable>( select );
				} );

			Assert.NotNull( table );
			Assert.NotEmpty( table.Rows );

			Console.WriteLine( "Number of comments:" + table.Rows.Count.ToString() ); 

			DataRowColumnAggregator aggregator = GetCommentAggregator( table );

			DataTable aggregated = null;

			Assert.DoesNotThrow(
				delegate
				{
					aggregated = aggregator.Process( table );
				} );

			timer.Stop();

			Assert.NotNull( aggregated );
			Assert.NotEmpty( aggregated.Rows );

			Console.WriteLine( "Time elapsed: " + timer.ElapsedMilliseconds.ToString() ); 
		}

		[Fact]
		public void Can_aggregate_comments_and_replace_in_dataset()
		{
			// Strategy:
			// ---------
			// 1) Simulate a GetEventDetail call.  
			// 2) First do the normal call without aggregating the rows and find an example of a multi-line comment.
			// 3) Then aggregate the comments and check that by replacing the aggregated table, we aren't breaking any relationships.
			// 4) Finally, check the aggregated comment against the expected output from the original comment lines.
			
			// check timings for various operations
			Stopwatch timer = new Stopwatch();

			string eventSql = AgencyEventDataAdapter.Select.ToString( EventsTests.OpenEventFilter );
			// order by lin_ord, so we can pick the a set of comments with the largest number of lines for testing
			// there are some funny line order values set for some single line comments, so just get anything with ten lines or less
			string commentSql = EventCommentDataAdapter.Select.ToString( EventsTests.GetOpenEventFilter( "EVCOM.lin_ord <= 10" ), null, "EVCOM.lin_ord" );
			string dispoSql = DispositionDataAdapter.Select.ToString( EventsTests.OpenEventFilter );

			EventDataModel.AgencyEventDataTable tableAgencyEvent = new EventDataModel.AgencyEventDataTable();
			EventDataModel.EventCommentDataTable tableEventComment = new EventDataModel.EventCommentDataTable();
			EventDataModel.DispositionDataTable tableDisposition = new EventDataModel.DispositionDataTable();

			DatabaseQuery[] queries = new DatabaseQuery[]
			{
				new DatabaseQuery( eventSql, tableAgencyEvent ),
				new DatabaseQuery( commentSql, tableEventComment ),
				new DatabaseQuery( dispoSql, tableDisposition )
			};

			timer.Restart();

			DatabaseAdapter database = new DatabaseAdapter( TestManager.ConnectionString );
			database.ExecuteAsynch( queries );

			timer.Stop();
			Console.WriteLine( "Database queries, time elapsed: " + timer.ElapsedMilliseconds.ToString() );

			// create the dataset and relations
			DataSet dataset = CreateEventDataSet( tableAgencyEvent, tableEventComment, tableDisposition );

			// ordered by lin_ord, so the last row should be one of the largest comment groups.
			EventDataModel.EventCommentRow referenceRow = (EventDataModel.EventCommentRow)tableEventComment.Rows[ tableEventComment.Rows.Count - 1 ];
			Console.WriteLine( "Largest comment group: " + referenceRow.LineOrder.ToString() );

			EventDataModel.AgencyEventRow agencyEvent = tableAgencyEvent.FindByAgencyEventId( referenceRow.AgencyEventId );

			EventDataModel.EventCommentRow[] commentsForEvent = agencyEvent.GetComments();
			Assert.NotNull( commentsForEvent );
			Assert.NotEmpty( commentsForEvent );

			// only get those comments that belong to this group
			int numberOfCommentsForEventBefore = commentsForEvent.Length;

			// remember for later
			string referenceTimestamp = referenceRow.CreatedTimestamp;
			string referenceTerminal = referenceRow.CreatedTerminal;
			long referenceLineGroup = referenceRow.LineGroup;

			List<EventDataModel.EventCommentRow> grouped = new List<EventDataModel.EventCommentRow>( commentsForEvent.Length );

			foreach ( EventDataModel.EventCommentRow row in commentsForEvent )
			{
				if ( row.CreatedTimestamp == referenceTimestamp
					&& row.CreatedTerminal == referenceTerminal
					&& row.LineGroup == referenceLineGroup )
				{
					grouped.Add( row );
				}
			}


			Assert.Equal<long>( referenceRow.LineOrder + 1, grouped.Count );

			// construct the expected aggregate message for subsequent comparison
			grouped.Sort(
				delegate ( EventDataModel.EventCommentRow lhs, EventDataModel.EventCommentRow rhs )
				{
					return (int)(lhs.LineOrder - rhs.LineOrder);
				} );
			
			// build the expected aggregated text
			StringBuilder builder = new StringBuilder( 1024 );

			foreach ( EventDataModel.EventCommentRow row in grouped )
			{
				string text = row.Text;

				if ( text == null )
				{
					continue;
				}

				if ( builder.Length > 0 )
				{
					builder.Append( " " );
				}

				builder.Append( text );
			}

			string expectedAggregatedText = builder.ToString();
			long numTotalCommentsBefore = tableEventComment.Rows.Count;

			// copy the original tables for use in the new dataset, dont include this
			// in the timer metrics, this step would not be performed in production.
			EventDataModel.AgencyEventDataTable tableAgencyEventCopy = (EventDataModel.AgencyEventDataTable)tableAgencyEvent.Copy();
			EventDataModel.EventCommentDataTable tableEventCommentCopy = (EventDataModel.EventCommentDataTable)tableEventComment.Copy();
			EventDataModel.DispositionDataTable tableDispositionCopy = (EventDataModel.DispositionDataTable)tableDisposition.Copy();

			// now aggregate the comments
			DataRowColumnAggregator aggregator = GetCommentAggregator( tableEventCommentCopy );

			timer.Restart();
			EventDataModel.EventCommentDataTable tableEventCommentAggregated = (EventDataModel.EventCommentDataTable)aggregator.Process( tableEventCommentCopy );

			long numTotalCommentsAfter = tableEventCommentAggregated.Rows.Count;

			// we know for sure that there are multiline comments
			Assert.True( numTotalCommentsAfter < numTotalCommentsBefore );

			timer.Stop();
			Console.WriteLine( "Aggregation took: " + timer.ElapsedMilliseconds.ToString() );
			timer.Start();

			DataSet aggregatedDataset = CreateEventDataSet( tableAgencyEventCopy, tableEventCommentAggregated, tableDispositionCopy );

			timer.Stop();
			Console.WriteLine( "Aggregation & merge took: " + timer.ElapsedMilliseconds.ToString() );

			// finally get the new aggregated comments
			agencyEvent = tableAgencyEventCopy.FindByAgencyEventId( referenceRow.AgencyEventId );
			commentsForEvent = agencyEvent.GetComments();
			
			Assert.NotNull( commentsForEvent );
			Assert.NotEmpty( commentsForEvent );

			int numberOfCommentsForEventAfter = commentsForEvent.Length;

			// we know for sure that there are multiline comments for this event
			Assert.True( numberOfCommentsForEventAfter < numberOfCommentsForEventBefore );

			// Find all groupings as before, this time there should only be one
			grouped = new List<EventDataModel.EventCommentRow>( commentsForEvent.Length );

			foreach ( EventDataModel.EventCommentRow row in commentsForEvent )
			{
				if ( row.CreatedTimestamp == referenceTimestamp
					&& row.CreatedTerminal == referenceTerminal
					&& row.LineGroup == referenceLineGroup )
				{
					grouped.Add( row );
				}
			}

			Assert.NotEmpty( grouped );
			Assert.Equal<int>( 1, grouped.Count );

			// finally check that the aggregated text is what we expect
			Assert.Equal<string>( expectedAggregatedText, grouped[ 0 ].Text );

			Console.WriteLine( "Aggregated text: " + grouped[ 0 ].Text );

			// finally, get the agency event from the adapter and confirm that the 
			// adapter is aggregating the comment correctly
			List<AgencyEvent> events = AgencyEventDataAdapter.GetData( database, new string[]{ "AEVEN.NUM_1 = '" + agencyEvent.AgencyEventId + "'" }, null );

			Assert.NotNull( events );
			Assert.Equal<int>( 1, events.Count );

			List<EventComment> commentsForAgencyEventOnly = events[ 0 ].Comments;

			// so far the comments we have fetched are not agency event specific.
			// when querying an agency event we only get those comments that 
			int numberCommentsForAgencyEventOnly = 0;
			string agencyId = events[ 0 ].Agency.AgencyId;

			foreach ( EventDataModel.EventCommentRow row in commentsForEvent )
			{
				if ( row.IsScopeNull() || row.Scope == agencyId )
				{
					++numberCommentsForAgencyEventOnly;
				}
			}

			Assert.Equal<int>( numberCommentsForAgencyEventOnly, commentsForAgencyEventOnly.Count );

			// Finally confirm that the aggregated comment is as expected when queried
			// via the agency event data adapter.
			List<EventComment> expectedAgencyEventComment = new List<EventComment>();

			foreach ( EventComment comment in commentsForAgencyEventOnly )
			{
				if ( comment.Created.Timestamp == IPS.Utility.DateFormatter.ConvertFromDTS( referenceTimestamp )
					&& comment.Created.Terminal == referenceTerminal )
				{
					expectedAgencyEventComment.Add( comment );
				}
			}

			// NOTE: we may not match correctly, since we don't expose the LineGroup
			// value through the object interface, so there is a small chance that
			// we might match more than one comment based on timestamp and 
			// terminal alone.  This is unlikely but could happen.

			Assert.Equal<int>( 1, expectedAgencyEventComment.Count );
			Assert.Equal<string>( expectedAggregatedText, expectedAgencyEventComment[ 0 ].Text );
		}

		DataSet CreateEventDataSet( EventDataModel.AgencyEventDataTable tableAgencyEvent, EventDataModel.EventCommentDataTable tableEventComment, EventDataModel.DispositionDataTable tableDisposition )
		{
			DataSet dataset = new DataSet();

			dataset.Tables.Add( tableAgencyEvent );
			dataset.Tables.Add( tableEventComment );
			dataset.Tables.Add( tableDisposition );

			DataRelation relationAgencyEventEventComment = new DataRelation( "AgencyEventEventComment", 
																									tableAgencyEvent.AgencyEventIdColumn, 
																									tableEventComment.AgencyEventIdColumn, false );
			
			relationAgencyEventEventComment.ExtendedProperties.Add( "typedChildren", "GetComments" );
			relationAgencyEventEventComment.ExtendedProperties.Add( "typedParent", "AgencyEvent" );
			
			dataset.Relations.Add( relationAgencyEventEventComment );
			
			DataRelation relationAgencyEventDisposition = new DataRelation( "AgencyEventDisposition", 
																									tableAgencyEvent.AgencyEventIdColumn, 
																									tableDisposition.AgencyEventIdColumn, false );
			
			relationAgencyEventDisposition.ExtendedProperties.Add( "typedChildren", "GetDisposition" );
			relationAgencyEventDisposition.ExtendedProperties.Add( "typedParent", "AgencyEvent" );
			
			dataset.Relations.Add( relationAgencyEventDisposition );

			return dataset;
		}

		/// <summary>
		/// Helper function to construct a comment aggregator
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		DataRowColumnAggregator GetCommentAggregator( EventDataModel.EventCommentDataTable table )
		{
			DataColumn aggregateOn = table.TextColumn;

			DataColumn[] groupBy = new DataColumn[] 
			{ 
				table.CreatedTimestampColumn, 
				table.CreatedTerminalColumn,
				table.LineGroupColumn
			};

			DataColumn[] sortBy = new DataColumn[] 
			{ 
				table.LineOrderColumn 
			};

			DataRowColumnComparer comparer = new DataRowColumnComparer( sortBy );

			return new DataRowColumnAggregator( aggregateOn, groupBy, comparer );
		}


	}
}
