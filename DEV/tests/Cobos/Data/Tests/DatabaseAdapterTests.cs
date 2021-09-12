// ----------------------------------------------------------------------------
// <copyright file="DatabaseAdapterTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="IDatabaseAdapter"/> class.
    /// </summary>
    [TestClass]
    public class DatabaseAdapterTests
    {
        ////[TestMethod]
        ////public void Can_execute_asynchronous_database_queries()
        ////{
        ////   // Strategy:
        ////   // ---------
        ////   // 1) Assert that we perform asynchronous queries to simultaneously fill multiple tables in a DataSet.

        ////   EventDataModel model = new EventDataModel();
        ////   model.EnforceConstraints = false;

        ////   DatabaseQuery[] queries = new DatabaseQuery[]
        ////   {
        ////      AgencyEventDataAdapter.GetQuery( model.AgencyEvent, null, null ),
        ////      EventCommentDataAdapter.GetQuery( model.EventComment, null, null ),
        ////      DispositionDataAdapter.GetQuery( model.Disposition, null, null )
        ////   };

        ////   Stopwatch timer = new Stopwatch();
        ////   timer.Start();

        ////   DatabaseAdapter database = new DatabaseAdapter( TestManager.ConnectionString );
        ////   database.ExecuteAsynch( queries );

        ////   timer.Stop();
        ////   Console.WriteLine( "Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString() );

        ////   Assert.NotEmpty( model.AgencyEvent.Rows );
        ////   Assert.NotEmpty( model.EventComment.Rows );
        ////   Assert.NotEmpty( model.Disposition.Rows );
        ////   Assert.Empty( model.Agencies.Rows );
        ////   Assert.Empty( model.AgencyEventSummary.Rows );
        ////}

        ////[TestMethod]
        ////public void Can_execute_nested_queries()
        ////{
        ////   // Strategy:
        ////   // ---------
        ////   // 1) Assert that we perform asynchronous queries to simultaneously fill multiple tables in a DataSet.
        ////   // 2) Test that the dataset relationships are set correctly and that we can find nested rows for comments and disposition.
        ////   // 3) Test that the multiplicity of the nested rows matches our expectations.  (this is partially data dependent).

        ////   EventDataModel model = new EventDataModel();

        ////   // disable the contraints when performing asynchronous queries,
        ////   // otherwise, if the main table isn't filled, the child tables
        ////   // will violate the key constraints.
        ////   // this also improves performance.
        ////   model.EnforceConstraints = false;

        ////   DatabaseQuery[] queries = new DatabaseQuery[]
        ////   {
        ////      AgencyEventDataAdapter.GetQuery( model.AgencyEvent, null, null ),
        ////      AgencyEventCommentDataAdapter.GetQuery( model.AgencyEventComment, null, null ),
        ////      DispositionDataAdapter.GetQuery( model.Disposition, null, null )
        ////   };

        ////   Stopwatch timer = new Stopwatch();
        ////   timer.Start();

        ////   DatabaseAdapter database = new DatabaseAdapter( TestManager.ConnectionString );
        ////   database.ExecuteAsynch( queries );

        ////   timer.Stop();
        ////   Console.WriteLine( "Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString() );

        ////   // get the first row in the comments, use this to find an event with comments
        ////   EventDataModel.AgencyEventCommentRow eventComment = model.AgencyEventComment[ 0 ];

        ////   // find the event that owns this comment
        ////   EventDataModel.AgencyEventRow agencyEvent = model.AgencyEvent.FindByAgencyEventId( eventComment.AgencyEventId );

        ////   Assert.IsNotNull( agencyEvent );

        ////   EventDataModel.AgencyEventCommentRow[] comments = agencyEvent.GetComments();
        ////   Console.WriteLine( "Found " + comments.Length.ToString() + " comments for " + agencyEvent.AgencyEventId );

        ////   Assert.IsNotNull( comments );
        ////   Assert.NotEmpty( comments );
        ////   Assert.Contains<EventDataModel.AgencyEventCommentRow>( eventComment, comments );

        ////   // get the first row in the disposition, use this to find an event with disposition
        ////   EventDataModel.DispositionRow disposition = model.Disposition[ 0 ];

        ////   agencyEvent = model.AgencyEvent.FindByAgencyEventId( disposition.AgencyEventId );

        ////   EventDataModel.DispositionRow[] dispositions = agencyEvent.GetDisposition();

        ////   Assert.IsNotNull( dispositions );
        ////   Assert.NotEmpty( dispositions );
        ////   Assert.Contains<EventDataModel.DispositionRow>( disposition, dispositions );

        ////   // Disposition is a one-to-one relationship
        ////   Assert.AreEqual( 1, dispositions.Length );
        ////}

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Confirm that we can connect to the database.
        /// </summary>
        [DataTestMethod]
        public void Can_connect_to_database()
        {
            foreach (var database in TestManager.DataSource)
            {
                Assert.IsTrue(database.TestConnection());
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Query the database metadata without generating an exception.
        /// 2. Confirm that the data appears to be written to the file properly.
        /// </summary>
        [DataTestMethod]
        public void Can_query_table_metadata()
        {
            foreach (var database in TestManager.DataSource)
            {
                string output = TestManager.TestFiles + "dbmetadata.xml";

                using (FileStream fstream = new FileStream(output, FileMode.Create))
                {
                    database.GetTableMetadata("Northwind", new string[] { "customers", "employees", "products" }, fstream);
                }

                FileInfo info = new FileInfo(output);
                Assert.IsTrue(info.Exists);
                Assert.AreNotEqual(0, info.Length);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Query the database metadata without generating an exception.
        /// 2. Confirm that the data appears to be written to the file properly.
        /// </summary>
        [DataTestMethod]
        public void Can_query_table_schema()
        {
            foreach (var database in TestManager.DataSource)
            {
                string output = TestManager.TestFiles + "dbschema.xml";

                using (FileStream stream = new FileStream(output, FileMode.Create))
                {
                    database.GetTableSchema("Northwind", new string[] { "customers", "employees", "products" }, stream);
                }

                FileInfo info = new FileInfo(output);
                Assert.IsTrue(info.Exists);
                Assert.AreNotEqual(0, info.Length);
            }
        }
    }
}
