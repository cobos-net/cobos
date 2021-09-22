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
        /// 1. Create a Binary Comparison from a list of literal values.
        /// </summary>
        [TestMethod]
        public void Can_deserialize_PropertyIsEqualTo() => TestFilter<PropertyIsEqualTo>();

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a Binary Logic operation from a list of literal values.
        /// </summary>
        [TestMethod]
        public void Can_deserialize_And() => TestFilter<And>();

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a Binary Logic operation from a list of literal values.
        /// </summary>
        [TestMethod]
        public void Can_deserialize_Or() => TestFilter<Or>();

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a Unary Logic operation from a list of literal values.
        /// </summary>
        [TestMethod]
        public void Can_deserialize_Not() => TestFilter<Not>();

        private static Filter TestFilter<TPredicate>()
        {
            var filename = TestManager.ProjectDirectory + $@"..\..\.cobos\Examples\{typeof(TPredicate).Name}.xml";

            Console.WriteLine("File: " + System.IO.Path.GetFileName(filename));
            var xml = System.IO.File.ReadAllText(filename);

            var filter = Filter.Deserialize(xml);
            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.Predicate);
            Assert.AreSame(typeof(TPredicate), filter.Predicate.GetType());

            return filter;
        }
    }
}
