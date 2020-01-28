// ----------------------------------------------------------------------------
// <copyright file="DataModelTests.cs" company="Cobos SDK">
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

namespace Cobos.Codegen.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Cobos.Codegen.Tests.Northwind;
    using Cobos.Data;
    using Cobos.Data.Filter;
    using Cobos.Utilities.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the DataModel class.
    /// </summary>
    [TestClass]
    public class DataModelTests
    {
        /// <summary>
        /// Epoch for date related tests.
        /// </summary>
        private static readonly DateTime Epoch = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Serialize the customer entities.
        /// </summary>
        [TestMethod]
        public void Can_serialize_customers()
        {
            foreach (var database in TestManager.DataSource)
            {
                TestManager.Serialize(new NorthwindDataModelAdapter(database).GetCustomer(null, null));
            }
        }

        /// <summary>
        /// Assumptions:
        /// ------------
        /// 1. When querying a single entity, the tables are only populated
        ///    with records that are related to the single parent entity.
        /// Strategy:
        /// ---------
        /// 1. Query a single customer entity.
        /// 2. Check that the child tables are only populated with the entity data.
        ///    This verifies that only related data is queried from the database.
        /// </summary>
        [TestMethod]
        public void Nested_entity_queries_only_contain_related_data()
        {
            foreach (var database in TestManager.DataSource)
            {
                var filter = new Filter
                {
                    Predicate = new PropertyIsEqualTo() { ValueReference = "CustomerID", Literal = "ALFKI" },
                };

                var model = new NorthwindDataModelAdapter(database);
                var customers = model.GetCustomer(filter, null);

                Assert.IsNotNull(customers);
                Assert.AreEqual(1, customers.Count);
                Assert.AreEqual(1, model.Customer.Table.Rows.Count);

                object countOrders = database.ExecuteScalar("select count(*) from Orders where CustomerID = 'ALFKI'");
                Assert.AreEqual(countOrders, (long)customers[0].Orders.Count);
                Assert.AreEqual(countOrders, (long)model.CustomerOrder.Table.Rows.Count);

                var result = new DataTable();
                database.Fill("select OrderID from Orders where CustomerID = 'ALFKI'", result);
                var values = Cobos.Data.Utilities.DataTableHelper.GetColumnValues<int>(result, "OrderID");

                foreach (var order in customers[0].Orders)
                {
                    CollectionAssert.Contains(values, order.OrderID);
                }

#if NET35
                var inValues = string.Join(",", values.Select(i => i.ToString()).ToArray());
#else
                var inValues = string.Join(",", values);
#endif

                object countOrderDetails = database.ExecuteScalar("select count(*) from OrderDetails where OrderID in (" + inValues + ")");
                Assert.AreEqual(countOrderDetails, (long)model.OrderDetails.Table.Rows.Count);

                result = new DataTable();
                database.Fill("select ProductID from OrderDetails where OrderID in (" + inValues + ")", result);
                values = Cobos.Data.Utilities.DataTableHelper.GetColumnValues<int>(result, "ProductID");

                foreach (var order in customers[0].Orders)
                {
                    foreach (var orderDetails in order.Details)
                    {
                        CollectionAssert.Contains(values, orderDetails.Product.ID);
                    }
                }

                var fullModel = new NorthwindDataModelAdapter(database);
                fullModel.GetCustomer(null, null);

                Assert.IsTrue(model.Customer.Table.Rows.Count < fullModel.Customer.Table.Rows.Count);
                Assert.IsTrue(model.CustomerOrder.Table.Rows.Count < fullModel.CustomerOrder.Table.Rows.Count);
                Assert.IsTrue(model.OrderDetails.Table.Rows.Count < fullModel.OrderDetails.Table.Rows.Count);

                TestManager.Serialize(customers);
            }
        }
    }
}
