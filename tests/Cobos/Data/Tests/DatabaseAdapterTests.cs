// ----------------------------------------------------------------------------
// <copyright file="DatabaseAdapterTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Cobos.Codegen.Tests.Northwind;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="IDatabaseAdapter"/> class.
    /// </summary>
    [TestClass]
    public class DatabaseAdapterTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1) Assert that we perform asynchronous queries to simultaneously fill multiple tables in a DataSet.
        /// </summary>
        [TestMethod]
        public void Can_execute_asynchronous_database_queries()
        {
            var model = new NorthwindDataModel();
            model.EnforceConstraints = false;

            var queries = new DatabaseQuery[]
            {
                new DatabaseQuery(CustomerDataAdapter.SelectTemplate.ToString(null, null), model.Customer),
                new DatabaseQuery(CustomerOrderDataAdapter.SelectTemplate.ToString(null, null), model.CustomerOrder),
                new DatabaseQuery(OrderDetailsDataAdapter.SelectTemplate.ToString(null, null), model.OrderDetails),
            };

            foreach (var database in TestManager.DataSource)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                database.FillAsynch(queries);

                timer.Stop();
                Console.WriteLine("Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString());

                Assert.IsTrue(model.Customer.Rows.Count > 0);
                Assert.IsTrue(model.CustomerOrder.Rows.Count > 0);
                Assert.IsTrue(model.OrderDetails.Rows.Count > 0);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1) Assert that we perform asynchronous queries to simultaneously fill multiple tables in a DataSet.
        /// 2) Test that the dataset relationships are set correctly and that we can find nested rows.
        /// 3) Test that the multiplicity of the nested rows matches our expectations.  (this is partially data dependent).
        /// </summary>
        [TestMethod]
        public void Can_execute_nested_queries()
        {
            var model = new NorthwindDataModel();

            // Disable the contraints when performing asynchronous queries,
            // otherwise, if the main table isn't filled, the child tables
            // will violate the key constraints. This also improves performance.
            model.EnforceConstraints = false;

            var queries = new DatabaseQuery[]
            {
                new DatabaseQuery(CustomerDataAdapter.SelectTemplate.ToString(null, null), model.Customer),
                new DatabaseQuery(CustomerOrderDataAdapter.SelectTemplate.ToString(null, null), model.CustomerOrder),
                new DatabaseQuery(OrderDetailsDataAdapter.SelectTemplate.ToString(null, null), model.OrderDetails),
            };

            foreach (var database in TestManager.DataSource)
            {
                var timer = new Stopwatch();
                timer.Start();
                database.FillAsynch(queries);

                timer.Stop();
                Console.WriteLine("Asynchronous queries took: " + timer.ElapsedMilliseconds.ToString());

                // get the first row in the comments, use this to find an event with comments
                var customerOrder = model.CustomerOrder[0];

                // find the event that owns this comment
                var customer = model.Customer.FindByCustomerID(customerOrder.CustomerID);
                Assert.IsNotNull(customer);

                var customerOrders = customer.GetOrders();
                Assert.IsNotNull(customerOrders);
                Assert.IsTrue(customerOrders.Length > 0);
                Console.WriteLine("Found " + customerOrders.Length.ToString() + " comments for " + customer.CustomerID);

                CollectionAssert.Contains(customerOrders, customerOrder);

                // get the first row in the disposition, use this to find an event with disposition
                var orderDetail = model.OrderDetails[0];

                customerOrder = model.CustomerOrder.FindByOrderID(orderDetail.OrderID);

                var orderDetails = customerOrder.GetDetails();

                Assert.IsNotNull(orderDetails);
                Assert.IsTrue(orderDetails.Length > 0);
                CollectionAssert.Contains(orderDetails, orderDetail);
            }
        }

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
