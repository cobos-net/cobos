﻿// ----------------------------------------------------------------------------
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            foreach (var source in TestManager.DataSource)
            {
                string output = TestManager.TestFiles + source.ProviderName + ".xsd";

                if (File.Exists(output))
                {
                    File.Delete(output);
                }

                CobosDatabaseToXsd target = new CobosDatabaseToXsd();

                target.BuildEngine = Substitute.For<IBuildEngine>();
                target.ConnectionString = source.ConnectionString;
                target.DatabasePlatform = source.ProviderName;
                target.DatabaseSchema = "Northwind";
                target.DatabaseTables = new TaskItem[] { new TaskItem("Customers"), new TaskItem("Employees"), new TaskItem("Products"), new TaskItem("Order Details") };
                target.OutputFile = output;

                Assert.IsTrue(target.Execute());

                FileInfo info = new FileInfo(output);
                Assert.IsTrue(info.Exists);
                Assert.AreNotEqual(0, info.Length);
            }
        }
    }
}