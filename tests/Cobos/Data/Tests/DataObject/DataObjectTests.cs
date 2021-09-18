// ----------------------------------------------------------------------------
// <copyright file="DataObjectTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests.DataObject
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Cobos.Codegen.Tests.Northwind;
    using Cobos.Data.Filter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the DataModel class.
    /// </summary>
    [TestClass]
    public class DataObjectTests
    {
        /// <summary>
        /// Epoch for date related tests.
        /// </summary>
        private static readonly DateTime Epoch = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Fetch Customer records from the database.
        /// </summary>
        [TestMethod]
        public void Can_get_customers()
        {
            foreach (var database in TestManager.DataSource)
            {
                var customerData = new CustomerDataAdapter(database.ConnectionString, database.ProviderFactory);
                customerData.Fill(null, null);

                var customers = customerData.GetEntities();
                Assert.IsNotNull(customers);
                Assert.AreNotEqual(0, customers.Count);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Fetch EmployeeTerritory records from the database.
        /// </summary>
        [TestMethod]
        public void Can_get_territories()
        {
            foreach (var database in TestManager.DataSource)
            {
                var territoryData = new EmployeeTerritoryDataAdapter(database.ConnectionString, database.ProviderFactory);
                territoryData.Fill(null, null);

                var territories = territoryData.GetEntities();
                Assert.IsNotNull(territories);
                Assert.AreNotEqual(0, territories.Count);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Fetch Employee records from the database.
        /// </summary>
        [TestMethod]
        public void Can_get_employees()
        {
            foreach (var database in TestManager.DataSource)
            {
                var employeeData = new EmployeeDataAdapter(database.ConnectionString, database.ProviderFactory);
                employeeData.Fill(null, null);

                var employees = employeeData.GetEntities();
                Assert.IsNotNull(employees);
                Assert.AreNotEqual(0, employees.Count);

                employees.Any(e => e.Territories.Count > 0);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Fetch Active Products from the database.  These entities have a default filter (Discontinued == false).
        /// 2. Apply an additional filter to test against the default filter.
        /// </summary>
        [TestMethod]
        public void Can_get_filtered_entities()
        {
            foreach (var database in TestManager.DataSource)
            {
                var productData = new ActiveProductDataAdapter(database.ConnectionString, database.ProviderFactory);
                productData.Fill(null, null);

                var products = productData.GetEntities();
                Assert.IsNotNull(products);
                Assert.AreNotEqual(0, products.Count);
                TestManager.Serialize(products);

                Assert.IsTrue(products.All(p => (long)Convert.ChangeType(p.Discontinued, typeof(long)) == 0));

                var firstProduct = products[0].Productname;
                var filter = new Filter
                {
                    Predicate = new PropertyIsEqualTo() { ValueReference = "Productname", Literal = firstProduct },
                };

                productData.Table.Clear();
                productData.Fill(filter, null);

                products = productData.GetEntities();
                Assert.IsNotNull(products);
                Assert.AreNotEqual(0, products.Count);
                Assert.AreEqual(1, products.Count);
                Assert.AreEqual(firstProduct, products[0].Productname);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Fetch Sorted Order Details from the database.  These entities have a default sort (UnitPrice, ASC).
        /// 2. Apply an additional sort to the default sort.
        /// </summary>
        [TestMethod]
        public void Can_get_sorted_entities()
        {
            foreach (var database in TestManager.DataSource)
            {
                var orderDetailsData = new OrderDetailsSortedDataAdapter(database.ConnectionString, database.ProviderFactory);
                orderDetailsData.Fill(null, null);
                var orders = orderDetailsData.GetEntities();
                Assert.IsNotNull(orders);
                Assert.AreNotEqual(0, orders.Count);

                for (int i = 1; i < orders.Count; ++i)
                {
                    Assert.IsTrue(orders[i - 1].Unitprice <= orders[i].Unitprice);
                }

                var sort = new SortBy() { new SortProperty() { ValueReference = "Quantity", SortOrder = SortOrder.DESC } };

                orderDetailsData.Table.Clear();
                orderDetailsData.Fill(null, sort);
                orders = orderDetailsData.GetEntities();
                Assert.IsNotNull(orders);
                Assert.AreNotEqual(0, orders.Count);

                for (int i = 1; i < orders.Count; ++i)
                {
                    if (orders[i - 1].Unitprice == orders[i].Unitprice)
                    {
                        Assert.IsTrue(orders[i - 1].Quantity >= orders[i].Quantity);
                    }
                }
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform simple CRUD operations.
        /// </summary>
        [TestMethod]
        public void Can_perform_CRUD_operations()
        {
            foreach (var database in TestManager.DataSource)
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

                customerData.AcceptChanges();

                customerData.Fill(null, null);
                observer.Reset();

                customer = customerData.GetEntityByCustomerID("COBOS");

                Assert.IsNotNull(customer);
                Assert.AreEqual("Cobos SDK's", customer.CompanyName);
                Assert.AreEqual("Davis".ToUpper(), customer.Contact.Name); // Testing UpperCase StringFormat. See Examples/Northwind.xml
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

                customerData.AcceptChanges();

                customerData.Fill(null, null);
                observer.Reset();

                customer = customerData.GetEntityByCustomerID("COBOS");

                Assert.IsNotNull(customer);
                Assert.AreEqual("Updated Company", customer.CompanyName);
                Assert.AreEqual("Updated Name".ToUpper(), customer.Contact.Name); // Testing UpperCase StringFormat. See Examples/Northwind.xml
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

                customerData.AcceptChanges();

                customerData.Fill(null, null);
                customer = customerData.GetEntityByCustomerID("COBOS");

                Assert.IsNull(customer);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform multiple CRUD operations at the same time.
        /// </summary>
        [TestMethod]
        public void Can_perform_multiple_CRUD_operations()
        {
            foreach (var database in TestManager.DataSource)
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
                    Assert.IsNotNull(customerData.GetEntityByCustomerID(ids[i]));
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

                customerData.AcceptChanges();
                customerData.Fill(null, null);

                Assert.IsNotNull(customerData.GetEntityByCustomerID(ids[0]));
                Assert.IsNull(customerData.GetEntityByCustomerID(ids[1]));
                Assert.IsNotNull(customerData.GetEntityByCustomerID(ids[2]));

                customer0 = customerData.GetEntityByCustomerID(ids[0]);
                Assert.IsNotNull(customer0);
                Assert.AreEqual("Updated Company", customer0.CompanyName);
                Assert.AreEqual("Updated Name".ToUpper(), customer0.Contact.Name); // Testing UpperCase StringFormat. See Examples/Northwind.xml
                Assert.AreEqual("Mr", customer0.Contact.Title);
                Assert.AreEqual("Updated Address", customer0.Address.Address);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Perform multiple updates for date time fields.
        /// </summary>
        [TestMethod]
        public void Can_perform_multiple_updates()
        {
            foreach (var database in TestManager.DataSource)
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
                                                            GetDelegateParameterTypes(typeofDelegate, this.GetType()),
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
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Verify the data objects have the correct integer types. This mostly tests the Oracle NUMER(p,s) mappings.
        /// </summary>
        [TestMethod]
        public void DataObjects_have_correct_integer_types()
        {
            Assert.AreEqual(typeof(short), typeof(ActiveProduct).GetProperty(nameof(ActiveProduct.Productid)).PropertyType);
            Assert.AreEqual(typeof(short?), typeof(ActiveProduct).GetProperty(nameof(ActiveProduct.Supplierid)).PropertyType);
            Assert.AreEqual(typeof(byte?), typeof(ActiveProduct).GetProperty(nameof(ActiveProduct.Categoryid)).PropertyType);
            Assert.AreEqual(typeof(decimal?), typeof(ActiveProduct).GetProperty(nameof(ActiveProduct.Unitprice)).PropertyType);
            Assert.AreEqual(typeof(int?), typeof(ActiveProduct).GetProperty(nameof(ActiveProduct.Unitsinstock)).PropertyType);
            Assert.AreEqual(typeof(int?), typeof(ActiveProduct).GetProperty(nameof(ActiveProduct.Unitsonorder)).PropertyType);
            Assert.AreEqual(typeof(int?), typeof(ActiveProduct).GetProperty(nameof(ActiveProduct.Reorderlevel)).PropertyType);
            Assert.AreEqual(typeof(byte), typeof(ActiveProduct).GetProperty(nameof(ActiveProduct.Discontinued)).PropertyType);
            Assert.AreEqual(typeof(long), typeof(CustomerOrder).GetProperty(nameof(CustomerOrder.OrderID)).PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Verify that outer joins return all rows.
        /// </summary>
        [TestMethod]
        public void Can_perform_outer_joins()
        {
            foreach (var database in TestManager.DataSource)
            {
                var territoryCount = Convert.ChangeType(database.ExecuteScalar("select count(*) from territories"), typeof(long));

                var territorySalesData = new TerritorySalesTotalsDataAdapter(database.ConnectionString, database.ProviderFactory);
                territorySalesData.Fill(null, null);

                var territorySales = territorySalesData.GetEntities();

                Assert.AreEqual(territoryCount, (long)territorySales.Count);

                long nullCount = 0;
                long nonNullCount = 0;

                foreach (var territorySale in territorySales)
                {
                    if (territorySale.Total == null)
                    {
                        nullCount++;
                    }
                    else
                    {
                        nonNullCount++;
                    }
                }

                Assert.IsTrue(nullCount > 0);
                Assert.IsTrue(nonNullCount > 0);
                Assert.AreEqual(territoryCount, nullCount + nonNullCount);
            }
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
        /// Get the parameter types of a delegate.
        /// </summary>
        /// <param name="d">The type of the delegate.</param>
        /// <param name="target">The target for the delegate.</param>
        /// <returns>The parameter types.</returns>
        private static Type[] GetDelegateParameterTypes(Type d, Type target)
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
        private static Type GetDelegateReturnType(Type d)
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
        /// Observes changes to Data Adapter class.
        /// </summary>
        private class DataAdapterObserver
        {
            /// <summary>
            /// Represents the customer ID that we are observing.
            /// </summary>
            private readonly string customerId;

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
