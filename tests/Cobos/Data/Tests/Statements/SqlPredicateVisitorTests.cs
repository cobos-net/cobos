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
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
                Literal = new Literal { Value = 1.ToString() },
            };

            Assert.AreEqual($"Orders.OrderID LIKE 1", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsLike
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Literal = new Literal { Value = "Nick%" },
            };

            Assert.AreEqual($"Orders.CustomerID LIKE 'Nick%'", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsLike
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Literal = new Literal { Value = "Davis%" },
                Options = new PropertyIsLikeOptions
                {
                    MatchCase = true,
                },
            };

            Assert.AreEqual($"Orders.CustomerID LIKE 'Davis%'", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsLike
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Literal = new Literal { Value = "Davis%" },
                Options = new PropertyIsLikeOptions
                {
                    MatchCase = false,
                },
            };

            Assert.AreEqual($"UPPER(Orders.CustomerID) LIKE UPPER('Davis%')", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));
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
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
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
                Expression = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
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
                Expression = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
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
        /// 1. Test PropertyIsInList conversion to SQL.
        /// </summary>
        [TestMethod]
        public void PropertyIsInList_to_SQL()
        {
            var predicate = new PropertyIsInList
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Values = new PropertyIsInList.ValuesType { "a", "b", "c" },
            };

            Assert.AreEqual($"Orders.CustomerID IN ('a', 'b', 'c')", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsInList
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Values = new PropertyIsInList.ValuesType { "a", "b", "c" },
                Options = new PropertyIsInListOptions
                {
                    MatchCase = true,
                },
            };

            Assert.AreEqual($"Orders.CustomerID IN ('a', 'b', 'c')", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsInList
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Values = new PropertyIsInList.ValuesType { "a", "b", "c" },
                Options = new PropertyIsInListOptions
                {
                    MatchCase = false,
                },
            };

            Assert.AreEqual($"UPPER(Orders.CustomerID) IN (UPPER('a'), UPPER('b'), UPPER('c'))", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsInList
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
                Values = new PropertyIsInList.ValuesType { "1", "2", "3" },
            };

            Assert.AreEqual($"Orders.OrderID IN (1, 2, 3)", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsInList
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
                Values = new PropertyIsInList.ValuesType { "1", "2", "3" },
                Options = new PropertyIsInListOptions
                {
                    MatchCase = true,
                },
            };

            Assert.AreEqual($"Orders.OrderID IN (1, 2, 3)", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsInList
            {
                PropertyName = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
                Values = new PropertyIsInList.ValuesType { "1", "2", "3" },
                Options = new PropertyIsInListOptions
                {
                    MatchCase = false,
                },
            };

            Assert.AreEqual($"Orders.OrderID IN (1, 2, 3)", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));
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
                Left = new PropertyName { Value = nameof(Employee.EmployeeID) },
                Right = new PropertyName { Value = $"{nameof(Employee.Employment)}.{nameof(Employee.Employment.Extension)}" },
            };

            Assert.AreEqual($"Employees.EmployeeID = Employees.Extension", SqlPredicateVisitor<Employee>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new PropertyIsEqualTo
            {
                Left = new PropertyName { Value = nameof(EmployeeTerritory.EmployeeID) },
                Right = new PropertyName { Value = nameof(EmployeeTerritory.Territory) },
            };

            Assert.AreEqual($"EmployeeTerritories.EmployeeID = Territories.TerritoryDescription", SqlPredicateVisitor<EmployeeTerritory>.FilterToSql(new Filter { Predicate = predicate }));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test case insensitive .
        /// </summary>
        [TestMethod]
        public void Case_insensitive_property_comparisons()
        {
            TestComparisonCaseSensitive<PropertyIsEqualTo>("=");
            TestComparisonCaseSensitive<PropertyIsGreaterThan>(">");
            TestComparisonCaseSensitive<PropertyIsGreaterThanOrEqualTo>(">=");
            TestComparisonCaseSensitive<PropertyIsLessThan>("<");
            TestComparisonCaseSensitive<PropertyIsLessThanOrEqualTo>("<=");
            TestComparisonCaseSensitive<PropertyIsNotEqualTo>("!=");
        }

        private static void TestComparison<TComparison>(string operand)
            where TComparison : BinaryComparisonOp, new()
        {
            var predicate = new TComparison
            {
                Left = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
                Right = new Literal { Value = 1.ToString() },
            };

            Assert.AreEqual($"Orders.OrderID {operand} 1", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new Literal { Value = 1.ToString() },
                Right = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
            };

            Assert.AreEqual($"1 {operand} Orders.OrderID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Right = new Literal { Value = "Nick" },
            };

            Assert.AreEqual($"Orders.CustomerID {operand} 'Nick'", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new Literal { Value = "Nick" },
                Right = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
            };

            Assert.AreEqual($"'Nick' {operand} Orders.CustomerID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
                Right = new Literal { Value = null },
            };

            Assert.AreEqual($"Orders.OrderID {operand} NULL", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Right = new Literal { Value = null },
            };

            Assert.AreEqual($"Orders.CustomerID {operand} NULL", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Right = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
            };

            Assert.AreEqual($"Orders.CustomerID {operand} Orders.OrderID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));
        }

        private static void TestComparisonCaseSensitive<TComparison>(string operand)
            where TComparison : BinaryComparisonOp, new()
        {
            var predicate = new TComparison
            {
                Left = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Right = new Literal { Value = "Davis" },
            };

            Assert.AreEqual($"Orders.CustomerID {operand} 'Davis'", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Right = new Literal { Value = "Davis" },
                Options = new BinaryComparisonOpOptions
                {
                    MatchCase = true,
                },
            };

            Assert.AreEqual($"Orders.CustomerID {operand} 'Davis'", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new PropertyName { Value = nameof(CustomerOrder.CustomerID) },
                Right = new Literal { Value = "Davis" },
                Options = new BinaryComparisonOpOptions
                {
                    MatchCase = false,
                },
            };

            Assert.AreEqual($"UPPER(Orders.CustomerID) {operand} UPPER('Davis')", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new Literal { Value = 1.ToString() },
                Right = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
            };

            Assert.AreEqual($"1 {operand} Orders.OrderID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new Literal { Value = 1.ToString() },
                Right = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
                Options = new BinaryComparisonOpOptions
                {
                    MatchCase = false,
                },
            };

            Assert.AreEqual($"1 {operand} Orders.OrderID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));

            predicate = new TComparison
            {
                Left = new Literal { Value = 1.ToString() },
                Right = new PropertyName { Value = nameof(CustomerOrder.OrderID) },
                Options = new BinaryComparisonOpOptions
                {
                    MatchCase = true,
                },
            };

            Assert.AreEqual($"1 {operand} Orders.OrderID", SqlPredicateVisitor<CustomerOrder>.FilterToSql(new Filter { Predicate = predicate }));
        }
    }
}