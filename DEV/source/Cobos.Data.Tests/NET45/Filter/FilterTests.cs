// ----------------------------------------------------------------------------
// <copyright file="FilterTests.cs" company="Cobos SDK">
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

namespace Cobos.Data.Tests.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cobos.Data.Filter;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="Filter"/> class.
    /// </summary>
    [TestFixture]
    public class FilterTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that filter expressions can be deserialized.
        /// </summary>
        [TestCase]
        public void Can_deserialize_filter_expressions()
        {
            string error;
            string[] examples;

            Cobos.Utilities.IO.FileUtility.TryFindAllFiles(TestManager.ProjectDirectory + @"..\..\Cobos.Codegen\Schemas\examples", "*.xml", false, out examples, out error); 

            Assert.DoesNotThrow(() =>
                {
                    foreach (var example in examples)
                    {
                        Console.WriteLine("File: " + System.IO.Path.GetFileName(example));
                        var xml = System.IO.File.ReadAllText(example);

                        var filter = Filter.Deserialize(xml);
                        Assert.NotNull(filter);
                        Assert.NotNull(filter.Predicate);
                        Assert.AreSame(typeof(PropertyIsEqualTo), filter.Predicate.GetType()); 
                    }
                });
        }
    }
}
