// ----------------------------------------------------------------------------
// <copyright file="DataModelTests.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
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
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using Cobos.Codegen.Tests.Northwind;
    using Cobos.Data;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the DataModel class.
    /// </summary>
    [TestFixture]
    public class DataModelTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform simple queries to populate the data model.
        /// </summary>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_populate_the_data_model(IDatabaseAdapter database)
        {
            var customerData = new CustomerDataAdapter(database.ConnectionString, database.ProviderFactory);
            customerData.Load(null, null);

            var customers = customerData.GetEntities();
            Assert.IsNotNull(customers);
            Assert.IsNotEmpty(customers);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform simple CRUD operations.
        /// </summary>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_perform_CRUD_operations(IDatabaseAdapter database)
        {
            database.ExecuteNonQuery("delete from customers where customerid='COBOS'");

            var customerData = new CustomerDataAdapter(database.ConnectionString, database.ProviderFactory);
            var observer = new DataAdapterObserver(customerData, "COBOS");

            customerData.Load(null, null);
            observer.Reset();

            var customer = customerData.NewCustomer();
            PopulateCustomer(customer, "COBOS");
            customerData.AddCustomer(customer);

            Assert.AreEqual(1, observer.Added);
            Assert.AreEqual(1, observer.Changed);
            Assert.AreEqual(1, observer.Changing);
            Assert.AreEqual(0, observer.Deleted);
            Assert.AreEqual(0, observer.Deleting);

            Assert.DoesNotThrow(() => customerData.AcceptChanges());

            customerData.Load(null, null);
            observer.Reset();

            customer = customerData.GetEntityByCustomerID("COBOS");

            Assert.NotNull(customer);
            Assert.AreEqual("Cobos SDK's", customer.CompanyName);
            Assert.AreEqual("Davis", customer.Contact.Name);
            Assert.AreEqual("Mr", customer.Contact.Title);
            Assert.AreEqual("Address", customer.Address.Address);
            Assert.AreEqual("City", customer.Address.City);
            Assert.AreEqual("Country", customer.Address.Country);
            Assert.AreEqual("Fax", customer.Address.Fax);
            Assert.AreEqual("Phone", customer.Address.Phone);
            Assert.AreEqual("Postal", customer.Address.PostalCode);
            Assert.AreEqual("Region", customer.Address.Region);

            customer.CompanyName = "Updated Company";
            customer.Address.Address = "Updated Address";
            customer.Contact.Name = "Updated Name";

            Assert.AreEqual(0, observer.Added);
            Assert.AreEqual(3, observer.Changed);
            Assert.AreEqual(3, observer.Changing);
            Assert.AreEqual(0, observer.Deleted);
            Assert.AreEqual(0, observer.Deleting);

            Assert.DoesNotThrow(() => customerData.AcceptChanges());

            customerData.Load(null, null);
            observer.Reset();

            customer = customerData.GetEntityByCustomerID("COBOS");

            Assert.NotNull(customer);
            Assert.AreEqual("Updated Company", customer.CompanyName);
            Assert.AreEqual("Updated Name", customer.Contact.Name);
            Assert.AreEqual("Mr", customer.Contact.Title);
            Assert.AreEqual("Updated Address", customer.Address.Address);
            Assert.AreEqual("City", customer.Address.City);
            Assert.AreEqual("Country", customer.Address.Country);
            Assert.AreEqual("Fax", customer.Address.Fax);
            Assert.AreEqual("Phone", customer.Address.Phone);
            Assert.AreEqual("Postal", customer.Address.PostalCode);
            Assert.AreEqual("Region", customer.Address.Region);

            customer.DataRowSource.Delete();

            Assert.AreEqual(0, observer.Added);
            Assert.AreEqual(0, observer.Changed);
            Assert.AreEqual(0, observer.Changing);
            Assert.AreEqual(0, observer.Deleted);
            Assert.AreEqual(1, observer.Deleting);

            Assert.DoesNotThrow(() => customerData.AcceptChanges());

            customerData.Load(null, null);
            customer = customerData.GetEntityByCustomerID("COBOS");

            Assert.IsNull(customer);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform multiple CRUD operations at the same time.
        /// </summary>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_perform_multiple_CRUD_operations(IDatabaseAdapter database)
        {
            string[] ids = new string[] { "COB1", "COB2", "COB3" };
            string idsIn = string.Join(",", ids.Select(i => "'" + i + "'").ToArray());

            database.ExecuteNonQuery("delete from customers where customerid in (" + idsIn + ")");

            var customerData = new CustomerDataAdapter(database.ConnectionString, database.ProviderFactory);
            customerData.Load(null, null);

            for (int i = 0; i < 2; ++i)
            {
                var customer = customerData.NewCustomer();
                PopulateCustomer(customer, ids[i]);
                customerData.AddCustomer(customer);
            }

            customerData.AcceptChanges();
            customerData.Load(null, null);

            for (int i = 0; i < 2; ++i)
            {
                Assert.NotNull(customerData.GetEntityByCustomerID(ids[i]));
            }

            var customer2 = customerData.NewCustomer();
            PopulateCustomer(customer2, ids[2]);
            customerData.AddCustomer(customer2);

            var customer0 = customerData.GetEntityByCustomerID(ids[0]);
            customer0.CompanyName = "Updated Company";
            customer0.Address.Address = "Updated Address";
            customer0.Contact.Name = "Updated Name";

            var customer1 = customerData.GetEntityByCustomerID(ids[1]);
            customerData.DeleteCustomer(customer1);

            Assert.DoesNotThrow(() => customerData.AcceptChanges());
            customerData.Load(null, null);

            Assert.NotNull(customerData.GetEntityByCustomerID(ids[0]));
            Assert.IsNull(customerData.GetEntityByCustomerID(ids[1]));
            Assert.NotNull(customerData.GetEntityByCustomerID(ids[2]));

            customer0 = customerData.GetEntityByCustomerID(ids[0]);
            Assert.NotNull(customer0);
            Assert.AreEqual("Updated Company", customer0.CompanyName);
            Assert.AreEqual("Updated Name", customer0.Contact.Name);
            Assert.AreEqual("Mr", customer0.Contact.Title);
            Assert.AreEqual("Updated Address", customer0.Address.Address);
        }

        /// <summary>
        /// Populate a customer object with test data.
        /// </summary>
        /// <param name="customer">The customer object.</param>
        private static void PopulateCustomer(Customer customer, string id)
        {
            customer.CustomerID = id;
            customer.CompanyName = "Cobos SDK's";   // testing apostrophe

            customer.Contact.Title = "Mr";
            customer.Contact.Name = "Davis";

            customer.Address.Address = "Address";
            customer.Address.City = "City";
            customer.Address.Country = "Country";
            customer.Address.Fax = "Fax";
            customer.Address.Phone = "Phone";
            customer.Address.PostalCode = "Postal";
            customer.Address.Region = "Region";
        }

        /// <summary>
        /// Observes changes to Data Adapter class.
        /// </summary>
        private class DataAdapterObserver
        {
            /// <summary>
            /// Number of objects added.
            /// </summary>
            public int Added;

            /// <summary>
            /// Number of objects changing.
            /// </summary>
            public int Changing;

            /// <summary>
            /// Number of objects changed.
            /// </summary>
            public int Changed;

            /// <summary>
            /// Number of objects deleting.
            /// </summary>
            public int Deleting;

            /// <summary>
            /// Number of objects deleted.
            /// </summary>
            public int Deleted;

            /// <summary>
            /// Represents the customer ID that we are observing.
            /// </summary>
            private string customerId;

            /// <summary>
            /// Initalizes a new instance of the <see cref="DataAdapterObserver"/> class.
            /// </summary>
            /// <param name="adapter">The adapter.</param>
            /// <param name="customerId">The customer ID that we are observing.</param>
            public DataAdapterObserver(CustomerDataAdapter adapter, string customerId)
            {
                this.customerId = customerId;

                adapter.MonitorChanges();
                adapter.OnAddedCustomer += OnAddedCustomer;
                adapter.OnChangedCustomer += OnChangedCustomer;
                adapter.OnChangingCustomer += OnChangingCustomer;
                adapter.OnDeletedCustomer += OnDeletedCustomer;
                adapter.OnDeletingCustomer += OnDeletingCustomer;
            }

            /// <summary>
            /// Reset all counters.
            /// </summary>
            public void Reset()
            {
                this.Added = 0;
                this.Changed = 0;
                this.Changing = 0;
                this.Deleted = 0;
                this.Deleting = 0;
            }

            /// <summary>
            /// Event delegate.
            /// </summary>
            /// <param name="obj">The customer object.</param>
            private void OnDeletingCustomer(Customer obj)
            {
                Assert.AreEqual(this.customerId, obj.CustomerID);
                ++this.Deleting;
            }

            /// <summary>
            /// Event delegate.
            /// </summary>
            /// <param name="obj">The customer object.</param>
            private void OnDeletedCustomer(Customer obj)
            {
                Assert.AreEqual(this.customerId, obj.CustomerID);
                ++this.Deleted;
            }

            /// <summary>
            /// Event delegate.
            /// </summary>
            /// <param name="obj">The customer object.</param>
            private void OnChangingCustomer(Customer obj)
            {
                ++this.Changing;
            }

            /// <summary>
            /// Event delegate.
            /// </summary>
            /// <param name="obj">The customer object.</param>
            private void OnChangedCustomer(Customer obj)
            {
                ++this.Changed;
            }

            /// <summary>
            /// Event delegate.
            /// </summary>
            /// <param name="obj">The customer object.</param>
            private void OnAddedCustomer(Customer obj)
            {
                ++this.Added;
            }
        }
    }
}
