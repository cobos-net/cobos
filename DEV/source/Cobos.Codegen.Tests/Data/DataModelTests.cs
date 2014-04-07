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
    using System.Diagnostics;
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
            var table = new CustomerTableAdapter(database.GetConnection);
            table.Fill(database.GetDataAdapter());

            var customers = table.GetEntities();
            Assert.IsNotNull(customers);
            Assert.IsNotEmpty(customers);
        }
    }
}
