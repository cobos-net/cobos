// ----------------------------------------------------------------------------
// <copyright file="DataRowColumnAggregatorTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests.Utilities
{
    using System;
    using System.Data;
    using Cobos.Data.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="DataRowColumnAggregator"/> class.
    /// </summary>
    [TestClass]
    public class DataRowColumnAggregatorTests
    {
        /// <summary>
        /// Test row data.
        /// Grouping is performed on Col2, Col3 and Col5.
        /// The different data types are included to test sorting and grouping by key values.
        /// Some columns have extra spaces to check the trimming and concatenation.
        /// Some of the keys in Col5 are lower case to check case insensitive comparisons.
        /// </summary>
        private static readonly object[][] RowData =
        {
            ////          | ID    | Col1            | Col2          | Col3      | Col4                                  | Col5              | Col6
            //// ----------------------------------------------------------------------------------------------------------------------------------------------
            new object[] { 1,       "String1",          1,            1.0f,        DateTime.Now,                        "first 4 items",    true },
            new object[] { 2,       "String2",          2,            1.0f,        DateTime.Now.AddMilliseconds(-1.0),  "First 4 Items",    false },
            new object[] { 3,       "String3",          3,            1.0f,        DateTime.Now.AddSeconds(-1.0),       "First 4 Items",    true },
            new object[] { 4,       "String4",          4,            1.0f,        DateTime.Now.AddMinutes(-1.0),       "FIRST 4 ITEMS",    false },
            new object[] { 5,       "String5",          2,            1.0f,        DateTime.Now.AddHours(-1.0),         "last 4 items",     true },
            new object[] { 6,       "String6 ",         4,            1.0f,        DateTime.Now.AddDays(-1.0),          "Last 4 Items",     false },
            new object[] { 7,       " String7",         4,            1.0f,        DateTime.Now.AddMonths(-1),          "Last 4 Items",     true },
            new object[] { 8,       "  String8  ",      4,            1.0f,        DateTime.Now.AddYears(-1),           "LAST 4 ITEMS",     false },
        };

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Assert that we can do a simple aggregation on a single text column.
        /// </summary>
        [TestMethod]
        public void Can_aggregate_on_single_column()
        {
            DataTable table = CreateTable(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            DataColumn aggregateOn = table.Columns["Col1"];
            DataColumn[] groupBy = new DataColumn[] { table.Columns["Col5"] };

            DataRowColumnAggregator aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, null);

            DataTable aggregated = aggregator.Transform(table);

            // should have aggregated into two groups: "First 4 Items" and "Last 4 Items"
            Assert.AreEqual(2, aggregated.Rows.Count);

            Assert.AreEqual("String1 String2 String3 String4", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String5 String6 String7 String8", (string)aggregated.Rows[1]["Col1"]);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Assert that we can do an aggregation on multiple columns, both of which are not text columns.
        /// </summary>
        [TestMethod]
        public void Can_aggregate_on_multiple_columns()
        {
            DataTable table = CreateTable(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            DataColumn aggregateOn = table.Columns["Col1"];

            // the group by order is important, grouping on col3 then col2
            DataColumn[] groupBy = new DataColumn[] { table.Columns["Col3"], table.Columns["Col2"] };

            DataRowColumnAggregator aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, null);

            DataTable aggregated = aggregator.Transform(table);

            // should have aggregated into four groups: 1, 2, 3, 4
            Assert.AreEqual(4, aggregated.Rows.Count);

            Assert.AreEqual("String1", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String2 String5", (string)aggregated.Rows[1]["Col1"]);
            Assert.AreEqual("String3", (string)aggregated.Rows[2]["Col1"]);
            Assert.AreEqual("String4 String6 String7 String8", (string)aggregated.Rows[3]["Col1"]);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Assert that we can do an aggregation on multiple columns and sort by a single column.
        /// 2. Repeat 1) but sort descending to produce the opposite output.
        /// 3. Repeat 1) but sort on multiple columns.  The first sort column must belong to a group
        /// of values to test that the sorting is broken into groups.
        /// 4. Repeat 2) but use case sensitive matching to produce a different output.
        /// </summary>
        [TestMethod]
        public void Can_aggregate_and_sort_columns()
        {
            DataTable table = CreateTable(new int[] { 7, 6, 5, 4, 3, 2, 1, 0 });

            DataColumn aggregateOn = table.Columns["Col1"];

            // the group by order is important, grouping on col3 then col6
            DataColumn[] groupBy = new DataColumn[] { table.Columns["Col3"], table.Columns["Col6"] };

            //////////////////////////////////////////////////////////////////////
            //// 1. sort by single column, case sensitive, ascending

            // sorting by the dates, they are in reverse order
            DataColumn[] sortBy = new DataColumn[] { table.Columns["Col4"] };

            DataRowColumnComparer comparer = new DataRowColumnComparer(sortBy);

            DataRowColumnAggregator aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, comparer);
            DataTable aggregated = aggregator.Transform(table);

            // should have aggregated into two groups: true and false
            Assert.AreEqual(2, aggregated.Rows.Count);

            Assert.AreEqual("String7 String5 String3 String1", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String8 String6 String4 String2", (string)aggregated.Rows[1]["Col1"]);

            //////////////////////////////////////////////////////////////////////
            //// 2. sort by single column, case sensitive, descending

            comparer = new DataRowColumnComparer(sortBy, DataRowColumnComparer.SortOrderEnum.Descending, false);
            aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, comparer);
            aggregated = aggregator.Transform(table);

            // should have aggregated into two groups: true and false
            Assert.AreEqual(2, aggregated.Rows.Count);

            Assert.AreEqual("String1 String3 String5 String7", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String2 String4 String6 String8", (string)aggregated.Rows[1]["Col1"]);

            //////////////////////////////////////////////////////////////////////
            //// 3. sort by multiple columns, case insensitive, ascending

            // try sorting by two columns, the first half of the list has priority, ignoring case
            sortBy = new DataColumn[] { table.Columns["Col5"], table.Columns["Col4"] };

            comparer = new DataRowColumnComparer(sortBy, DataRowColumnComparer.SortOrderEnum.Ascending, true);
            aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, comparer);
            aggregated = aggregator.Transform(table);

            // again, should have aggregated into two groups: true and false
            Assert.AreEqual(2, aggregated.Rows.Count);

            Assert.AreEqual("String3 String1 String7 String5", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String4 String2 String8 String6", (string)aggregated.Rows[1]["Col1"]);

            //////////////////////////////////////////////////////////////////////
            //// 4. sort by multiple columns, case sensitive, descending

            comparer = new DataRowColumnComparer(sortBy, DataRowColumnComparer.SortOrderEnum.Descending, false);
            aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, comparer);
            aggregated = aggregator.Transform(table);

            // again, should have aggregated into two groups: true and false
            Assert.AreEqual(2, aggregated.Rows.Count);

            Assert.AreEqual("String7 String5 String3 String1", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String8 String6 String4 String2", (string)aggregated.Rows[1]["Col1"]);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Assert that attempting to aggregate on any column type other than string
        /// results in an exception being thrown.
        /// </summary>
        [TestMethod]
        public void Cant_aggregate_on_non_string_columns()
        {
            using (var table = CreateTable(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }))
            {
                Assert.ThrowsException<InvalidOperationException>(() => new DataRowColumnAggregator(table.Columns["ID"], null, null));

                Assert.ThrowsException<InvalidOperationException>(() => new DataRowColumnAggregator(table.Columns["Col2"], null, null));

                Assert.ThrowsException<InvalidOperationException>(() => new DataRowColumnAggregator(table.Columns["Col3"], null, null));

                Assert.ThrowsException<InvalidOperationException>(() => new DataRowColumnAggregator(table.Columns["Col4"], null, null));

                Assert.ThrowsException<InvalidOperationException>(() => new DataRowColumnAggregator(table.Columns["Col6"], null, null));
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Assert that we can do an aggregation on a column containing null values.
        /// We expect the null values to be omitted from the result strings.
        /// </summary>
        [TestMethod]
        public void Can_handle_null_aggregate_column()
        {
            DataTable table = CreateTable(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            table.Rows[0]["Col1"] = null;
            table.Rows[7]["Col1"] = null;

            DataColumn aggregateOn = table.Columns["Col1"];
            DataColumn[] groupBy = new DataColumn[] { table.Columns["Col5"] };

            DataRowColumnAggregator aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, null);

            DataTable aggregated = aggregator.Transform(table);

            // should have aggregated into two groups: "First 4 Items" and "Last 4 Items"
            Assert.AreEqual(2, aggregated.Rows.Count);

            Assert.AreEqual("String2 String3 String4", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String5 String6 String7", (string)aggregated.Rows[1]["Col1"]);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Assert that we can do an aggregation with null values in the grouping columns.
        /// We expect that the null values will be grouped together in a new null group.
        /// </summary>
        [TestMethod]
        public void Can_handle_null_grouping_columns()
        {
            DataTable table = CreateTable(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            table.Rows[0]["Col5"] = null;
            table.Rows[7]["Col5"] = null;

            DataColumn aggregateOn = table.Columns["Col1"];
            DataColumn[] groupBy = new DataColumn[] { table.Columns["Col5"] };

            DataRowColumnAggregator aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, null);

            DataTable aggregated = aggregator.Transform(table);

            // should have aggregated into three groups: null, "First 4 Items" and "Last 4 Items"
            Assert.AreEqual(3, aggregated.Rows.Count);

            Assert.AreEqual("String1 String8", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String2 String3 String4", (string)aggregated.Rows[1]["Col1"]);
            Assert.AreEqual("String5 String6 String7", (string)aggregated.Rows[2]["Col1"]);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Assert that we can do an aggregation and sorting with null values in the sorting columns.
        /// We expect that the null values will be sorted at the beginning of the sort order.
        /// </summary>
        [TestMethod]
        public void Can_handle_null_sorting_columns()
        {
            DataTable table = CreateTable(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            table.Rows[1]["Col4"] = DBNull.Value;
            table.Rows[6]["Col4"] = DBNull.Value;

            DataColumn aggregateOn = table.Columns["Col1"];
            DataColumn[] groupBy = new DataColumn[] { table.Columns["Col5"] };

            // sorting by the dates, they are in reverse order
            DataColumn[] sortBy = new DataColumn[] { table.Columns["Col4"] };

            DataRowColumnComparer comparer = new DataRowColumnComparer(sortBy);

            DataRowColumnAggregator aggregator = new DataRowColumnAggregator(aggregateOn, groupBy, comparer);

            DataTable aggregated = aggregator.Transform(table);

            // should have aggregated into two groups: "First 4 Items" and "Last 4 Items"
            Assert.AreEqual(2, aggregated.Rows.Count);

            Assert.AreEqual("String2 String4 String3 String1", (string)aggregated.Rows[0]["Col1"]);
            Assert.AreEqual("String7 String8 String6 String5", (string)aggregated.Rows[1]["Col1"]);
        }

        /// <summary>
        /// Simple data table to use for testing.
        /// </summary>
        /// <param name="rowOrder">The order to create the rows in.</param>
        /// <returns>A data table for testing.</returns>
        private static DataTable CreateTable(int[] rowOrder)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Col1", typeof(string));
            table.Columns.Add("Col2", typeof(int));
            table.Columns.Add("Col3", typeof(float));
            table.Columns.Add("Col4", typeof(DateTime));
            table.Columns.Add("Col5", typeof(string));
            table.Columns.Add("Col6", typeof(bool));

            for (int i = 0; i < rowOrder.Length; ++i)
            {
                DataRow row = table.NewRow();

                row.ItemArray = (object[])RowData[i].Clone();

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
