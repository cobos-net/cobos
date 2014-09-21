// ----------------------------------------------------------------------------
// <copyright file="DatabaseToXsdTests.cs" company="Cobos SDK">
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

namespace Cobos.Build.Targets.Tests
{
    using System;
    using System.IO;
    using Cobos.Build.Targets;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="CobosDatabaseToXsd"/> class.
    /// </summary>
    [TestFixture]
    public class DatabaseToXsdTests
    {
        /// <summary>
        /// Test case source.
        /// </summary>
        private static object[] dataSource =
        {
            ////new object[] { "Oracle", "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT= 1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XE)));User Id=Northwind;Password=oracle" },
            new object[] { "MySQL", "Server=localhost;Database=Northwind;Uid=root;Pwd=mysql;" }
        };

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create test schemas for all providers.
        /// </summary>
        /// <param name="platform">The database platform name.</param>
        /// <param name="connectionString">The database connection string.</param>
        [Test]
        [TestCaseSource("dataSource")]
        public void Can_generate_database_schema(string platform, string connectionString)
        {
            string output = TestManager.TestFiles + platform + ".xsd";

            if (File.Exists(output))
            {
                File.Delete(output);
            }

            CobosDatabaseToXsd target = new CobosDatabaseToXsd();

            target.BuildEngine = Substitute.For<IBuildEngine>();
            target.ConnectionString = connectionString;
            target.DatabasePlatform = platform;
            target.DatabaseSchema = "Northwind";
            target.DatabaseTables = new TaskItem[] { new TaskItem("Customers"), new TaskItem("Employees"), new TaskItem("Products") };
            target.OutputFile = output;

            Assert.True(target.Execute());

            FileInfo info = new FileInfo(output);
            Assert.True(info.Exists);
            Assert.AreNotEqual(0, info.Length);
        }
    }
}
