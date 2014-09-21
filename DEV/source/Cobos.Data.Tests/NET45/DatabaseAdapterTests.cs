// ----------------------------------------------------------------------------
// <copyright file="DatabaseAdapterTests.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="DatabaseAdapter"/> class.
    /// </summary>
    [TestFixture]
    public class DatabaseAdapterTests
    {
        ////[TestCase]
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

        ////[TestCase]
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

        ////   Assert.NotNull( agencyEvent );

        ////   EventDataModel.AgencyEventCommentRow[] comments = agencyEvent.GetComments();
        ////   Console.WriteLine( "Found " + comments.Length.ToString() + " comments for " + agencyEvent.AgencyEventId );

        ////   Assert.NotNull( comments );
        ////   Assert.NotEmpty( comments );
        ////   Assert.Contains<EventDataModel.AgencyEventCommentRow>( eventComment, comments );

        ////   // get the first row in the disposition, use this to find an event with disposition
        ////   EventDataModel.DispositionRow disposition = model.Disposition[ 0 ];

        ////   agencyEvent = model.AgencyEvent.FindByAgencyEventId( disposition.AgencyEventId );

        ////   EventDataModel.DispositionRow[] dispositions = agencyEvent.GetDisposition();

        ////   Assert.NotNull( dispositions );
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
        /// <param name="database">The database adapter.</param>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_connect_to_database(IDatabaseAdapter database)
        {
            Assert.True(database.TestConnection());
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Query the database metadata without generating an exception.
        /// 2. Confirm that the data appears to be written to the file properly.
        /// </summary>
        /// <param name="database">The database adapter.</param>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_query_table_metadata(IDatabaseAdapter database)
        {
            string output = TestManager.TestFiles + "dbmetadata.xml";

            Assert.DoesNotThrow(() =>
                {
                    using (FileStream fstream = new FileStream(output, FileMode.Create))
                    {
                        database.GetTableMetadata("Northwind", new string[] { "customers", "employees", "products" }, fstream);
                    }
                });

            FileInfo info = new FileInfo(output);
            Assert.True(info.Exists);
            Assert.AreNotEqual(0, info.Length);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Query the database metadata without generating an exception.
        /// 2. Confirm that the data appears to be written to the file properly.
        /// </summary>
        /// <param name="database">The database adapter.</param>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_query_table_schema(IDatabaseAdapter database)
        {
            string output = TestManager.TestFiles + "dbschema.xml";

            Assert.DoesNotThrow(() =>
                {
                    using (FileStream fstream = new FileStream(output, FileMode.Create))
                    {
                        database.GetTableSchema("Northwind", new string[] { "customers", "employees", "products" }, fstream);
                    }
                });

            FileInfo info = new FileInfo(output);
            Assert.True(info.Exists);
            Assert.AreNotEqual(0, info.Length);
        }
    }
}
