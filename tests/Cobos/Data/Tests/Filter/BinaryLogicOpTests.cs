// ----------------------------------------------------------------------------
// <copyright file="BinaryLogicOpTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests.Filter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Cobos.Codegen.Tests.Northwind;
    using Cobos.Data.Filter;
    using Cobos.Data.Statements;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="BinaryLogicOp"/> class.
    /// </summary>
    [TestClass]
    public class BinaryLogicOpTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a Binary Logic operation from a list of literal values.
        /// 2. Confirm one item returns a single comparison.
        /// 3. Test even and odd numbers.
        /// 4. Confirm that the NOT is generated properly.
        /// </summary>
        [TestMethod]
        public void Test_Logical_And() => TestLogicOperation<And>();

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a Binary Logic operation from a list of literal values.
        /// 2. Confirm one item returns a single comparison.
        /// 3. Test even and odd numbers.
        /// 4. Confirm that the NOT is generated properly.
        /// </summary>
        [TestMethod]
        public void Test_Logical_Or() => TestLogicOperation<Or>();

        private static void TestLogicOperation<TLogic>()
            where TLogic : BinaryLogicOp, new()
        {
            var operand = typeof(TLogic).Name.ToUpper();

            var predicate = BinaryLogicOp.Compose<TLogic, PropertyIsEqualTo, int>("OrderID", Enumerable.Range(1, 1));

            var actual = SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate });
            var expected = $"Orders.OrderID = 1";
            Assert.AreEqual(expected, actual);

            predicate = BinaryLogicOp.Compose<TLogic, PropertyIsEqualTo, int>("OrderID", Enumerable.Range(1, 2));

            actual = SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate });
            expected = $"(Orders.OrderID = 1 {operand} Orders.OrderID = 2)";
            Assert.AreEqual(expected, actual);

            predicate = BinaryLogicOp.Compose<TLogic, PropertyIsEqualTo, int>("OrderID", Enumerable.Range(1, 3));

            actual = SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate });
            expected = $"(Orders.OrderID = 1 {operand} (Orders.OrderID = 2 {operand} Orders.OrderID = 3))";
            Assert.AreEqual(expected, actual);

            predicate = BinaryLogicOp.Compose<TLogic, PropertyIsEqualTo, int>("OrderID", Enumerable.Range(1, 4));

            actual = SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate });
            expected = $"(Orders.OrderID = 1 {operand} (Orders.OrderID = 2 {operand} (Orders.OrderID = 3 {operand} Orders.OrderID = 4)))";
            Assert.AreEqual(expected, actual);

            actual = SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = new Not { Condition = predicate } });
            Assert.AreEqual($"NOT ({expected})", actual);
        }
    }
}
