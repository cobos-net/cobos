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
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
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
        /// Epoch for date related tests.
        /// </summary>
        private static readonly DateTime Epoch = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform simple queries to populate the data model.
        /// </summary>
        /// <param name="database">The database adapter.</param>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_populate_the_data_model(IDatabaseAdapter database)
        {
            var customerData = new CustomerDataAdapter(database.ConnectionString, database.ProviderFactory);
            customerData.Fill(null, null);

            var customers = customerData.GetEntities();
            Assert.IsNotNull(customers);
            Assert.IsNotEmpty(customers);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform simple CRUD operations.
        /// </summary>
        /// <param name="database">The database adapter.</param>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_perform_CRUD_operations(IDatabaseAdapter database)
        {
            database.ExecuteNonQuery("delete from customers where customerid='COBOS'");

            var customerData = new CustomerDataAdapter(database.ConnectionString, database.ProviderFactory);
            var observer = new DataAdapterObserver(customerData, "COBOS");

            customerData.Fill(null, null);
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

            customerData.Fill(null, null);
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

            customerData.Fill(null, null);
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

            customerData.Fill(null, null);
            customer = customerData.GetEntityByCustomerID("COBOS");

            Assert.IsNull(customer);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform multiple CRUD operations at the same time.
        /// </summary>
        /// <param name="database">The database adapter.</param>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_perform_multiple_CRUD_operations(IDatabaseAdapter database)
        {
            string[] ids = new string[] { "COB1", "COB2", "COB3" };
            string idsIn = string.Join(",", ids.Select(i => "'" + i + "'").ToArray());

            database.ExecuteNonQuery("delete from customers where customerid in (" + idsIn + ")");

            var customerData = new CustomerDataAdapter(database.ConnectionString, database.ProviderFactory);
            customerData.Fill(null, null);

            for (int i = 0; i < 2; ++i)
            {
                var customer = customerData.NewCustomer();
                PopulateCustomer(customer, ids[i]);
                customerData.AddCustomer(customer);
            }

            customerData.AcceptChanges();
            customerData.Fill(null, null);

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
            customerData.Fill(null, null);

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
        /// Strategy:
        /// ---------
        /// 1. Perform multiple updates for date time fields.
        /// </summary>
        /// <param name="database">The database adapter.</param>
        [TestCase]
        [TestCaseSource(typeof(TestManager), "DataSource")]
        public void Can_perform_multiple_updates(IDatabaseAdapter database)
        {
            database.ExecuteNonQuery("delete from employees where employeeid > 9");

            using (var connection = database.ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = database.ConnectionString;

                var adapter = database.ProviderFactory.CreateDataAdapter();

                adapter.SelectCommand = database.ProviderFactory.CreateCommand();
                adapter.SelectCommand.CommandText = "select `employees.employeeid`, `employees.lastname`, `employees.firstname` from employees";
                adapter.SelectCommand.Connection = connection;

                adapter.InsertCommand = database.ProviderFactory.CreateCommand();
                adapter.InsertCommand.CommandText =
                    @"insert into employees (LastName, FirstName) values (?LastName, ?FirstName);
                select employeeid, lastname, firstname from employees where employeeid = LAST_INSERT_ID();";
                adapter.InsertCommand.Connection = connection;

                var parameter = database.ProviderFactory.CreateParameter();
                parameter.ParameterName = "?LastName";
                parameter.DbType = System.Data.DbType.StringFixedLength;
                parameter.Size = 20;
                parameter.SourceColumn = "LastName";

                adapter.InsertCommand.Parameters.Add(parameter);

                parameter = database.ProviderFactory.CreateParameter();
                parameter.ParameterName = "?FirstName";
                parameter.DbType = System.Data.DbType.StringFixedLength;
                parameter.Size = 10;
                parameter.SourceColumn = "FirstName";

                adapter.InsertCommand.Parameters.Add(parameter);

                adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.Both;
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                var employeeData = new EmployeeDataAdapter(database.ConnectionString, database.ProviderFactory);
                employeeData.Fill(null, null);

                var employee = employeeData.NewEmployee();
                PopulateEmployee(employee);
                employeeData.AddEmployee(employee);

                ////employee = employeeData.NewEmployee();
                ////PopulateEmployee(employee);

                ////Action<object, RowUpdatedEventArgs> onUpdated = (s, e) =>
                ////{
                ////    if (e.StatementType == StatementType.Insert)
                ////    {
                ////        e.Status = UpdateStatus.SkipCurrentRow;
                ////    }
                ////};

                var eventInfo = adapter.GetType().GetEvent("RowUpdated");
                var typeofDelegate = eventInfo.EventHandlerType;

                DynamicMethod handler = new DynamicMethod(
                                                        string.Empty,
                                                        null,
                                                        this.GetDelegateParameterTypes(typeofDelegate, this.GetType()),
                                                        this.GetType());

                // Generate a method body. This method loads a string, calls  
                // the Show method overload that takes a string, pops the  
                // return value off the stack (because the handler has no 
                // return type), and returns. 
                ILGenerator ilgen = handler.GetILGenerator();

                var methodInfoHandler = this.GetType().GetMethod("OnRowUpdated", BindingFlags.NonPublic | BindingFlags.Instance);

                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldarg_1);
                ilgen.Emit(OpCodes.Ldarg_2);
                ilgen.Emit(OpCodes.Call, methodInfoHandler);
                ilgen.Emit(OpCodes.Ret);

                Delegate emittedDelegate = handler.CreateDelegate(typeofDelegate, this);

                Delegate @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfoHandler);
                MethodInfo addHandler = eventInfo.GetAddMethod();
                addHandler.Invoke(adapter, new object[] { emittedDelegate });

                System.Data.DataTable changes = employeeData.Table.GetChanges();
                Assert.AreNotEqual(0, changes.Rows.Count);
                adapter.Update(changes);

                employeeData.Table.Merge(changes);
                employeeData.Table.AcceptChanges();

                Console.WriteLine("Rows after merge.");
                foreach (DataRow row in employeeData.Table.Rows)
                {
                    Console.WriteLine("{0}: {1}", row[0], row[1]);
                }
            }

            ////Assert.DoesNotThrow(() => employeeData.AcceptChanges());

            ////employee.Employment.HireDate = Epoch.AddYears(-4);
            ////Assert.DoesNotThrow(() => employeeData.AcceptChanges());

            ////employee.Employment.HireDate = Epoch.AddYears(-3);
            ////Assert.DoesNotThrow(() => employeeData.AcceptChanges());

            ////employee.Employment.HireDate = Epoch.AddYears(-2);
            ////Assert.DoesNotThrow(() => employeeData.AcceptChanges());
        }

        /// <summary>
        /// Populate a customer object with test data.
        /// </summary>
        /// <param name="customer">The customer object.</param>
        /// <param name="id">The id to use.</param>
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
        /// Populate an employee object with test data.
        /// </summary>
        /// <param name="employee">The employee object.</param>
        private static void PopulateEmployee(Employee employee)
        {
            employee.Contact.Address = "Address";
            employee.Contact.City = "City";
            employee.Contact.Country = "Country";
            employee.Contact.HomePhone = "Phone";
            employee.Contact.PostalCode = "Postal";
            employee.Contact.Region = "Region";

            employee.Employment.Extension = "Ext";
            employee.Employment.HireDate = Epoch.AddYears(-5);
            employee.Employment.Notes = "Notes";
            employee.Employment.PhotoPath = "PhotoPath";
            employee.Employment.ReportsTo = 1;

            employee.Personal.BirthDate = Epoch.AddYears(-30);
            employee.Personal.FirstName = "First Name";
            employee.Personal.LastName = "Last Name";
            employee.Personal.Title = "Mr";
            employee.Personal.TitleOfCourtesy = "Esq";
        }

        /// <summary>
        /// Called when a row is updated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRowUpdated(object sender, RowUpdatedEventArgs e)
        {
            if (e.StatementType == StatementType.Insert)
            {
                e.Status = UpdateStatus.SkipCurrentRow;
            }
        }

        /// <summary>
        /// Get the parameter types of a delegate.
        /// </summary>
        /// <param name="d">The type of the delegate.</param>
        /// <param name="target">The target for the delegate.</param>
        /// <returns>The parameter types.</returns>
        private Type[] GetDelegateParameterTypes(Type d, Type target)
        {
            if (d.BaseType != typeof(MulticastDelegate))
            {
                throw new ApplicationException("Not a delegate.");
            }

            MethodInfo invoke = d.GetMethod("Invoke");

            if (invoke == null)
            {
                throw new ApplicationException("Not a delegate.");
            }

            ParameterInfo[] parameters = invoke.GetParameters();
            Type[] typeParameters = new Type[parameters.Length + 1];
            typeParameters[0] = target;

            for (int i = 0; i < parameters.Length; i++)
            {
                typeParameters[i + 1] = parameters[i].ParameterType;
            }

            return typeParameters;
        }

        /// <summary>
        /// Get the return type of a delegate.
        /// </summary>
        /// <param name="d">The delegate type.</param>
        /// <returns>The type of the return.</returns>
        private Type GetDelegateReturnType(Type d)
        {
            if (d.BaseType != typeof(MulticastDelegate))
            {
                throw new ApplicationException("Not a delegate.");
            }

            MethodInfo invoke = d.GetMethod("Invoke");

            if (invoke == null)
            {
                throw new ApplicationException("Not a delegate.");
            }

            return invoke.ReturnType;
        }

        /// <summary>
        /// Observes changes to Data Adapter class.
        /// </summary>
        private class DataAdapterObserver
        {
            /// <summary>
            /// Represents the customer ID that we are observing.
            /// </summary>
            private string customerId;

            /// <summary>
            /// Initializes a new instance of the <see cref="DataAdapterObserver"/> class.
            /// </summary>
            /// <param name="adapter">The adapter.</param>
            /// <param name="customerId">The customer ID that we are observing.</param>
            public DataAdapterObserver(CustomerDataAdapter adapter, string customerId)
            {
                this.customerId = customerId;

                adapter.MonitorChanges();
                adapter.OnAddedCustomer += this.OnAddedCustomer;
                adapter.OnChangedCustomer += this.OnChangedCustomer;
                adapter.OnChangingCustomer += this.OnChangingCustomer;
                adapter.OnDeletedCustomer += this.OnDeletedCustomer;
                adapter.OnDeletingCustomer += this.OnDeletingCustomer;
            }

            /// <summary>
            /// Gets the number of objects added.
            /// </summary>
            public int Added
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the number of objects changing.
            /// </summary>
            public int Changing
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the number of objects changed.
            /// </summary>
            public int Changed
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the number of objects deleting.
            /// </summary>
            public int Deleting
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the number of objects deleted.
            /// </summary>
            public int Deleted
            {
                get;
                private set;
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
