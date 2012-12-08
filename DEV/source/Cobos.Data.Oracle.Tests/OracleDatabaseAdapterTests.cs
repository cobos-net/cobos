using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;
using Cobos.Data.Adapters;

namespace Cobos.Data.Tests
{
	[TestFixture]
	public class DatabaseAdapterTests
	{
		//[TestCase]
		//public void Can_execute_asynchronous_database_queries()
		//{
		//   // Strategy:
		//   // ---------
		//   // 1) Assert that we perform asynchronous queries to simultaneously fill multiple tables in a DataSet.

		//   EventDataModel model = new EventDataModel();
		//   model.EnforceConstraints = false;

		//   DatabaseQuery[] queries = new DatabaseQuery[] 
		//   { 
		//      AgencyEventDataAdapter.GetQuery( model.AgencyEvent, null, null ),
		//      EventCommentDataAdapter.GetQuery( model.EventComment, null, null ),
		//      DispositionDataAdapter.GetQuery( model.Disposition, null, null )
		//   };

		//   Stopwatch timer = new Stopwatch();
		//   timer.Start();

		//   DatabaseAdapter database = new DatabaseAdapter( TestManager.ConnectionString );
		//   database.ExecuteAsynch( queries );

		//   timer.Stop();
		//   Console.WriteLine( "Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString() );

		//   Assert.NotEmpty( model.AgencyEvent.Rows );
		//   Assert.NotEmpty( model.EventComment.Rows );
		//   Assert.NotEmpty( model.Disposition.Rows );
		//   Assert.Empty( model.Agencies.Rows );
		//   Assert.Empty( model.AgencyEventSummary.Rows );
		//}

		//[TestCase]
		//public void Can_execute_nested_queries()
		//{
		//   // Strategy:
		//   // ---------
		//   // 1) Assert that we perform asynchronous queries to simultaneously fill multiple tables in a DataSet.
		//   // 2) Test that the dataset relationships are set correctly and that we can find nested rows for comments and disposition.
		//   // 3) Test that the multiplicity of the nested rows matches our expectations.  (this is partially data dependent).

		//   EventDataModel model = new EventDataModel();

		//   // disable the contraints when performing asynchronous queries,
		//   // otherwise, if the main table isn't filled, the child tables
		//   // will violate the key constraints.
		//   // this also improves performance.
		//   model.EnforceConstraints = false;

		//   DatabaseQuery[] queries = new DatabaseQuery[] 
		//   { 
		//      AgencyEventDataAdapter.GetQuery( model.AgencyEvent, null, null ),
		//      AgencyEventCommentDataAdapter.GetQuery( model.AgencyEventComment, null, null ),
		//      DispositionDataAdapter.GetQuery( model.Disposition, null, null )
		//   };

		//   Stopwatch timer = new Stopwatch();
		//   timer.Start();

		//   DatabaseAdapter database = new DatabaseAdapter( TestManager.ConnectionString );
		//   database.ExecuteAsynch( queries );

		//   timer.Stop();
		//   Console.WriteLine( "Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString() );

		//   // get the first row in the comments, use this to find an event with comments
		//   EventDataModel.AgencyEventCommentRow eventComment = model.AgencyEventComment[ 0 ];

		//   // find the event that owns this comment
		//   EventDataModel.AgencyEventRow agencyEvent = model.AgencyEvent.FindByAgencyEventId( eventComment.AgencyEventId );

		//   Assert.NotNull( agencyEvent );

		//   EventDataModel.AgencyEventCommentRow[] comments = agencyEvent.GetComments();
		//   Console.WriteLine( "Found " + comments.Length.ToString() + " comments for " + agencyEvent.AgencyEventId );

		//   Assert.NotNull( comments );
		//   Assert.NotEmpty( comments );
		//   Assert.Contains<EventDataModel.AgencyEventCommentRow>( eventComment, comments );

		//   // get the first row in the disposition, use this to find an event with disposition
		//   EventDataModel.DispositionRow disposition = model.Disposition[ 0 ];

		//   agencyEvent = model.AgencyEvent.FindByAgencyEventId( disposition.AgencyEventId );

		//   EventDataModel.DispositionRow[] dispositions = agencyEvent.GetDisposition();

		//   Assert.NotNull( dispositions );
		//   Assert.NotEmpty( dispositions );
		//   Assert.Contains<EventDataModel.DispositionRow>( disposition, dispositions );
			
		//   // Disposition is a one-to-one relationship
		//   Assert.AreEqual( 1, dispositions.Length );
		//}

		[TestCase]
		public void Can_query_table_metadata()
		{
			// Strategy:
			// 1. Query the database metadata without generating an exception.
			// 2. Confirm that the data appears to be written to the file properly.

			IDatabaseAdapter database = new Oracle.OracleDatabaseAdapter( TestManager.ConnectionString );

			string output = TestManager.TestFiles + "dbmetadata.xml";

			Assert.DoesNotThrow(
				delegate
				{
					using ( FileStream fstream = new FileStream( output, FileMode.Create ) )
					{
						database.GetTableMetadata( "EADEV", new string[] { "AEVEN", "LINEUP", "CD_UNITS", "DISPASS_EVENT" }, fstream );
					}
				} );

			FileInfo info = new FileInfo( output );
			Assert.True( info.Exists );
			Assert.AreNotEqual( 0, info.Length );
		}

		[TestCase]
		public void Can_query_table_schema()
		{
			// Strategy:
			// 1. Query the database metadata without generating an exception.
			// 2. Confirm that the data appears to be written to the file properly.

			IDatabaseAdapter database = new Oracle.OracleDatabaseAdapter( TestManager.ConnectionString );

			string output = TestManager.TestFiles + "dbschema.xml";

			Assert.DoesNotThrow(
				delegate
				{
					using ( FileStream fstream = new FileStream( output, FileMode.Create ) )
					{
						database.GetTableSchema( "EADEV", new string[] { "AEVEN", "LINEUP", "CD_UNITS" }, fstream );
					}
				} );

			FileInfo info = new FileInfo( output );
			Assert.True( info.Exists );
			Assert.AreNotEqual( 0, info.Length );
		}
	}
}
