// ----------------------------------------------------------------------------
// <copyright file="DataModelTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests.DataObject
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Linq;
    using Cobos.Codegen.Tests.Northwind;
    using Cobos.Data.Filter;
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
                    Predicate = new PropertyIsEqualTo()
                    {
                        Left = new PropertyName { Value = "CustomerID" },
                        Right = new Literal { Value = "ALFKI" },
                    },
                };

                var model = new NorthwindDataModelAdapter(database);
                var customers = model.GetCustomer(filter, null).ToList();

                Assert.IsNotNull(customers);
                Assert.AreEqual(1, customers.Count);
                Assert.AreEqual(1, model.Customer.Table.Rows.Count);

                var countOrders = Convert.ChangeType(database.ExecuteScalar("select count(*) from Orders where CustomerID = 'ALFKI'"), typeof(int));
                Assert.AreEqual(countOrders, customers[0].Orders.ToList().Count);
                Assert.AreEqual(countOrders, model.CustomerOrder.Table.Rows.Count);

                var result = new DataTable();
                database.Fill("select OrderID from Orders where CustomerID = 'ALFKI'", result);

                IList values = null;

                if (database.IntegerType == typeof(decimal))
                {
                    values = Cobos.Data.Utilities.DataTableHelper.GetColumnValues<decimal>(result, "OrderID");
                }
                else
                {
                    values = Cobos.Data.Utilities.DataTableHelper.GetColumnValues<long>(result, "OrderID");
                }

                foreach (var order in customers[0].Orders)
                {
                    CollectionAssert.Contains(values, Convert.ChangeType(order.OrderID, database.IntegerType));
                }

                var inValues = string.Join(",", values.Cast<object>());

                var countOrderDetails = database.ExecuteScalar("select count(*) from OrderDetails where OrderID in (" + inValues + ")");
                Assert.AreEqual(Convert.ChangeType(countOrderDetails, typeof(long)), (long)model.OrderDetails.Table.Rows.Count);

                result = new DataTable();
                database.Fill("select ProductID from OrderDetails where OrderID in (" + inValues + ")", result);

                values = Cobos.Data.Utilities.DataTableHelper.GetColumnValues<int>(result, "ProductID");

                foreach (var order in customers[0].Orders)
                {
                    foreach (var orderDetails in order.Details)
                    {
                        CollectionAssert.Contains(values, (int)orderDetails.Product.ID);
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
