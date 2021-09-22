// ----------------------------------------------------------------------------
// <copyright file="SqlPredicateVisitorTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests.Statements
{
    using System;
    using Cobos.Codegen.Tests.Northwind;
    using Cobos.Data.Filter;
    using Cobos.Data.Statements;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="SqlPredicateVisitor{T}"/> class.
    /// </summary>
    [TestClass]
    public class SqlPredicateVisitorTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test Binary Comparison conversion to SQL.
        /// 2. Test a string literal property.
        /// 3. Test a number property.
        /// </summary>
        [TestMethod]
        public void BinaryComparisonOp_to_SQL()
        {
            TestComparison<PropertyIsEqualTo>("=");
            TestComparison<PropertyIsGreaterThan>(">");
            TestComparison<PropertyIsGreaterThanOrEqualTo>(">=");
            TestComparison<PropertyIsLessThan>("<");
            TestComparison<PropertyIsLessThanOrEqualTo>("<=");
            TestComparison<PropertyIsNotEqualTo>("!=");
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test PropertyIsLike conversion to SQL.
        /// 2. Test a string literal property.
        /// 3. Test a number property.
        /// </summary>
        [TestMethod]
        public void PropertyIsLike_to_SQL()
        {
            var predicate = new PropertyIsLike
            {
                PropertyName = new PropertyName { Value = "OrderID" },
                Literal = new Literal { Value = 1.ToString() },
            };

            Assert.AreEqual($"Orders.OrderID LIKE 1", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsLike
            {
                PropertyName = new PropertyName { Value = "CustomerID" },
                Literal = new Literal { Value = "Nick" },
            };

            Assert.AreEqual($"Orders.CustomerID LIKE 'Nick'", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test PropertyIsNull conversion to SQL.
        /// </summary>
        [TestMethod]
        public void PropertyIsNull_to_SQL()
        {
            var predicate = new PropertyIsNull
            {
                PropertyName = new PropertyName { Value = "OrderID" },
            };

            Assert.AreEqual($"Orders.OrderID IS NULL", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test PropertyIsNull conversion to SQL.
        /// </summary>
        [TestMethod]
        public void PropertyIsBetween_to_SQL()
        {
            var predicate = new PropertyIsBetween
            {
                Expression = new PropertyName { Value = "OrderID" },
                LowerBoundary = new LowerBoundary
                {
                    Expression = new Literal { Value = "20" },
                },
                UpperBoundary = new UpperBoundary
                {
                    Expression = new Literal { Value = "200" },
                },
            };

            Assert.AreEqual($"(Orders.OrderID >= 20 AND Orders.OrderID <= 200)", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsBetween
            {
                Expression = new PropertyName { Value = "CustomerID" },
                LowerBoundary = new LowerBoundary
                {
                    Expression = new Literal { Value = "20" },
                },
                UpperBoundary = new UpperBoundary
                {
                    Expression = new Literal { Value = "200" },
                },
            };

            Assert.AreEqual($"(Orders.CustomerID >= '20' AND Orders.CustomerID <= '200')", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test nested properties resolve correctly.
        /// </summary>
        [TestMethod]
        public void Nested_property_comparisons()
        {
            var predicate = new PropertyIsEqualTo
            {
                Left = new PropertyName { Value = "EmployeeID" },
                Right = new PropertyName { Value = "Employment.Extension" },
            };

            Assert.AreEqual($"Employees.EmployeeID = Employees.Extension", SqlPredicateVisitor<Employee>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsEqualTo
            {
                Left = new PropertyName { Value = "EmployeeID" },
                Right = new PropertyName { Value = "Territory" },
            };

            Assert.AreEqual($"EmployeeTerritories.EmployeeID = Territories.TerritoryDescription", SqlPredicateVisitor<EmployeeTerritory>.FilterToSql(new Filter { Predicate = predicate }));
        }

        private static void TestComparison<TComparison>(string operand)
            where TComparison : BinaryComparisonOp, new()
        {
            var predicate = new TComparison
            {
                Left = new PropertyName { Value = "OrderID" },
                Right = new Literal { Value = 1.ToString() },
            };

            Assert.AreEqual($"Orders.OrderID {operand} 1", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new Literal { Value = 1.ToString() },
                Right = new PropertyName { Value = "OrderID" },
            };

            Assert.AreEqual($"1 {operand} Orders.OrderID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = "CustomerID" },
                Right = new Literal { Value = "Nick" },
            };

            Assert.AreEqual($"Orders.CustomerID {operand} 'Nick'", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new Literal { Value = "Nick" },
                Right = new PropertyName { Value = "CustomerID" },
            };

            Assert.AreEqual($"'Nick' {operand} Orders.CustomerID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = "OrderID" },
                Right = new Literal { Value = null },
            };

            Assert.AreEqual($"Orders.OrderID {operand} NULL", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = "CustomerID" },
                Right = new Literal { Value = null },
            };

            Assert.AreEqual($"Orders.CustomerID {operand} NULL", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = "CustomerID" },
                Right = new PropertyName { Value = "OrderID" },
            };

            Assert.AreEqual($"Orders.CustomerID {operand} Orders.OrderID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));
        }
    }
}
