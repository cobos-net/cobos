// ----------------------------------------------------------------------------
// <copyright file="FilterTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests.Filter
{
    using System;
    using Cobos.Data.Filter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="Filter"/> class.
    /// </summary>
    [TestClass]
    public class FilterTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that filter expressions can be deserialized.
        /// </summary>
        [TestMethod]
        public void Can_deserialize_filter_expressions()
        {
            Cobos.Utilities.IO.FileUtility.TryFindAllFiles(TestManager.ProjectDirectory + @"..\..\.cobos\net46\Codegen\Schemas\examples", "*.xml", false, out string[] examples, out _);

            foreach (var example in examples)
            {
                Console.WriteLine("File: " + System.IO.Path.GetFileName(example));
                var xml = System.IO.File.ReadAllText(example);

                var filter = Filter.Deserialize(xml);
                Assert.IsNotNull(filter);
                Assert.IsNotNull(filter.Predicate);
                Assert.AreSame(typeof(PropertyIsEqualTo), filter.Predicate.GetType());
            }
        }
    }
}
