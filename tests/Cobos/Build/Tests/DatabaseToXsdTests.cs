// ----------------------------------------------------------------------------
// <copyright file="DatabaseToXsdTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Build.Tests
{
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    /// <summary>
    /// Unit tests for the <see cref="CobosDatabaseToXsd"/> class.
    /// </summary>
    [TestClass]
    public class DatabaseToXsdTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create test schemas for all providers.
        /// </summary>
        [TestMethod]
        public void Can_generate_database_schema()
        {
            foreach (var (providerName, connectionString) in TestManager.DataSource)
            {
                string output = TestManager.TestFiles + providerName + ".xsd";

                if (File.Exists(output))
                {
                    File.Delete(output);
                }

                var tables = new[] { "categories", "customercustomerdemo", "customerdemographics", "customers", "employees", "employeeterritories", "orderdetails", "orders", "products", "region", "shippers", "suppliers", "territories", "territorysales" };

                CobosDatabaseToXsd target = new CobosDatabaseToXsd
                {
                    BuildEngine = Substitute.For<IBuildEngine>(),
                    ConnectionString = connectionString,
                    DatabasePlatform = providerName,
                    DatabaseSchema = "Northwind",
                    DatabaseTables = tables.Select(t => new TaskItem(t)).ToArray(),
                    OutputFile = output,
                };

                Assert.IsTrue(target.Execute());

                FileInfo info = new FileInfo(output);
                Assert.IsTrue(info.Exists);
                Assert.AreNotEqual(0, info.Length);
            }
        }
    }
}
