﻿// ----------------------------------------------------------------------------
// <copyright file="SqlSelectTemplateTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests.Statements
{
    using System;
    using Cobos.Data.Statements;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="SqlSelectTemplate"/> class.
    /// </summary>
    [TestClass]
    public class SqlSelectTemplateTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the invalid cases.
        /// </summary>
        [TestMethod]
        public void Invalid_parameters_throws_exception()
        {
            new SqlSelectTemplate();

            Assert.ThrowsException<InvalidOperationException>(() => new SqlSelectTemplate().ToString());

            Assert.ThrowsException<InvalidOperationException>(() => new SqlSelectTemplate().ToString(null, null, null, null, null, null, null));

            Assert.ThrowsException<InvalidOperationException>(() => new SqlSelectTemplate(null, null, null, null, null, null, null, true));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the simple case.
        /// </summary>
        [TestMethod]
        public void Simple_select_succeeds()
        {
            var select = new SqlSelectTemplate("COL", null, null, null, null, null, null);
            select.ToString();
            select.ToString(null);
            select.ToString(null, null);
            select.ToString(null, null, null);

            select = new SqlSelectTemplate();
            select.ToString("COL", null, null, null, null, null, null);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the select parameters in isolation.
        /// </summary>
        [TestMethod]
        public void Select_parameters_return_correct_query()
        {
            var select = new SqlSelectTemplate("COL", null, null, null, null, null, null);

            Assert.AreEqual("SELECT COL", select.ToString());
            Assert.AreEqual("SELECT COL, COL", select.ToString("COL", null, null, null, null, null, null));

            select = new SqlSelectTemplate();

            Assert.AreEqual("SELECT COL", select.ToString("COL", null, null, null, null, null, null));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the from parameters.
        /// </summary>
        [TestMethod]
        public void From_parameters_return_correct_query()
        {
            var select = new SqlSelectTemplate("COL", "TABLE", null, null, null, null, null);

            Assert.AreEqual("SELECT COL FROM TABLE", select.ToString());
            Assert.AreEqual("SELECT COL FROM TABLE, TABLE2", select.ToString(null, "TABLE2", null, null, null, null, null));

            select = new SqlSelectTemplate();

            Assert.AreEqual("SELECT COL FROM TABLE", select.ToString("COL", "TABLE", null, null, null, null, null));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the inner join parameters.
        /// </summary>
        [TestMethod]
        public void Inner_join_parameters_return_correct_query()
        {
            var innerJoin1 = new string[] { "TABLE1" };
            var innerJoin2 = new string[] { "TABLE2", "TABLE3" };
            var innerJoin3 = new string[] { "TABLE4", "TABLE5", "TABLE6" };

            var select = new SqlSelectTemplate("COL", null, innerJoin1, null, null, null, null);

            Assert.AreEqual("SELECT COL INNER JOIN TABLE1", select.ToString());
            Assert.AreEqual("SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE1", select.ToString(null, null, innerJoin1, null, null, null, null));
            Assert.AreEqual("SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString(null, null, innerJoin2, null, null, null, null));
            Assert.AreEqual("SELECT COL INNER JOIN TABLE1 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString(null, null, innerJoin3, null, null, null, null));

            select = new SqlSelectTemplate("COL", null, innerJoin2, null, null, null, null);

            Assert.AreEqual("SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString());
            Assert.AreEqual("SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE1", select.ToString(null, null, innerJoin1, null, null, null, null));
            Assert.AreEqual("SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString(null, null, innerJoin2, null, null, null, null));
            Assert.AreEqual("SELECT COL INNER JOIN TABLE2 INNER JOIN TABLE3 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString(null, null, innerJoin3, null, null, null, null));

            select = new SqlSelectTemplate("COL", null, innerJoin3, null, null, null, null);

            Assert.AreEqual("SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString());
            Assert.AreEqual("SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE1", select.ToString(null, null, innerJoin1, null, null, null, null));
            Assert.AreEqual("SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE2 INNER JOIN TABLE3", select.ToString(null, null, innerJoin2, null, null, null, null));
            Assert.AreEqual("SELECT COL INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6 INNER JOIN TABLE4 INNER JOIN TABLE5 INNER JOIN TABLE6", select.ToString(null, null, innerJoin3, null, null, null, null));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the LEFT OUTER JOIN parameters.
        /// </summary>
        [TestMethod]
        public void Outer_join_parameters_return_correct_query()
        {
            var outerJoin1 = new string[] { "TABLE1" };
            var outerJoin2 = new string[] { "TABLE2", "TABLE3" };
            var outerJoin3 = new string[] { "TABLE4", "TABLE5", "TABLE6" };

            var select = new SqlSelectTemplate("COL", null, null, outerJoin1, null, null, null);

            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE1", select.ToString());
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE1 LEFT OUTER JOIN TABLE1", select.ToString(null, null, null, outerJoin1, null, null, null));
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE1 LEFT OUTER JOIN TABLE2 LEFT OUTER JOIN TABLE3", select.ToString(null, null, null, outerJoin2, null, null, null));
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE1 LEFT OUTER JOIN TABLE4 LEFT OUTER JOIN TABLE5 LEFT OUTER JOIN TABLE6", select.ToString(null, null, null, outerJoin3, null, null, null));

            select = new SqlSelectTemplate("COL", null, null, outerJoin2, null, null, null);

            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE2 LEFT OUTER JOIN TABLE3", select.ToString());
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE2 LEFT OUTER JOIN TABLE3 LEFT OUTER JOIN TABLE1", select.ToString(null, null, null, outerJoin1, null, null, null));
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE2 LEFT OUTER JOIN TABLE3 LEFT OUTER JOIN TABLE2 LEFT OUTER JOIN TABLE3", select.ToString(null, null, null, outerJoin2, null, null, null));
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE2 LEFT OUTER JOIN TABLE3 LEFT OUTER JOIN TABLE4 LEFT OUTER JOIN TABLE5 LEFT OUTER JOIN TABLE6", select.ToString(null, null, null, outerJoin3, null, null, null));

            select = new SqlSelectTemplate("COL", null, null, outerJoin3, null, null, null);

            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE4 LEFT OUTER JOIN TABLE5 LEFT OUTER JOIN TABLE6", select.ToString());
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE4 LEFT OUTER JOIN TABLE5 LEFT OUTER JOIN TABLE6 LEFT OUTER JOIN TABLE1", select.ToString(null, null, null, outerJoin1, null, null, null));
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE4 LEFT OUTER JOIN TABLE5 LEFT OUTER JOIN TABLE6 LEFT OUTER JOIN TABLE2 LEFT OUTER JOIN TABLE3", select.ToString(null, null, null, outerJoin2, null, null, null));
            Assert.AreEqual("SELECT COL LEFT OUTER JOIN TABLE4 LEFT OUTER JOIN TABLE5 LEFT OUTER JOIN TABLE6 LEFT OUTER JOIN TABLE4 LEFT OUTER JOIN TABLE5 LEFT OUTER JOIN TABLE6", select.ToString(null, null, null, outerJoin3, null, null, null));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the where parameters.
        /// </summary>
        [TestMethod]
        public void Where_parameters_return_correct_query()
        {
            var whereClause1 = new string[] { "CLAUSE1" };
            var whereClause2 = new string[] { "CLAUSE2", "CLAUSE3" };
            var whereClause3 = new string[] { "CLAUSE4", "CLAUSE5", "CLAUSE6" };

            var select = new SqlSelectTemplate("COL", null, null, null, whereClause1, null, null);

            Assert.AreEqual("SELECT COL WHERE (CLAUSE1)", select.ToString());
            Assert.AreEqual("SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString(null, null, null, null, whereClause1, null, null));
            Assert.AreEqual("SELECT COL WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE3)", select.ToString(null, null, null, null, whereClause2, null, null));
            Assert.AreEqual("SELECT COL WHERE (CLAUSE1) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString(null, null, null, null, whereClause3, null, null));

            // test the shortcut methods
            Assert.AreEqual("SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString(whereClause1, null, null));
            Assert.AreEqual("SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString(whereClause1, null));
            Assert.AreEqual("SELECT COL WHERE (CLAUSE1) AND (CLAUSE1)", select.ToString(whereClause1));

            select = new SqlSelectTemplate("COL", null, null, null, whereClause2, null, null);

            Assert.AreEqual("SELECT COL WHERE (CLAUSE2) AND (CLAUSE3)", select.ToString());
            Assert.AreEqual("SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE1)", select.ToString(null, null, null, null, whereClause1, null, null));
            Assert.AreEqual("SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE2) AND (CLAUSE3)", select.ToString(null, null, null, null, whereClause2, null, null));
            Assert.AreEqual("SELECT COL WHERE (CLAUSE2) AND (CLAUSE3) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString(null, null, null, null, whereClause3, null, null));

            select = new SqlSelectTemplate("COL", null, null, null, whereClause3, null, null);

            Assert.AreEqual("SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString());
            Assert.AreEqual("SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE1)", select.ToString(null, null, null, null, whereClause1, null, null));
            Assert.AreEqual("SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE2) AND (CLAUSE3)", select.ToString(null, null, null, null, whereClause2, null, null));
            Assert.AreEqual("SELECT COL WHERE (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6) AND (CLAUSE4) AND (CLAUSE5) AND (CLAUSE6)", select.ToString(null, null, null, null, whereClause3, null, null));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the group by parameters.
        /// </summary>
        [TestMethod]
        public void GroupBy_parameters_return_correct_query()
        {
            var select = new SqlSelectTemplate("COL", null, null, null, null, "GROUPCOL", null);

            Assert.AreEqual("SELECT COL GROUP BY GROUPCOL", select.ToString());
            Assert.AreEqual("SELECT COL GROUP BY GROUPCOL, GROUPCOL2", select.ToString(null, null, null, null, null, "GROUPCOL2", null));

            select = new SqlSelectTemplate();
            Assert.AreEqual("SELECT COL GROUP BY GROUPCOL", select.ToString("COL", null, null, null, null, "GROUPCOL", null));

            select = new SqlSelectTemplate("COL", null, null, null, null, null, null);
            Assert.AreEqual("SELECT COL GROUP BY GROUPCOL", select.ToString(null, "GROUPCOL", null));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the order by parameters.
        /// </summary>
        [TestMethod]
        public void OrderBy_parameters_return_correct_query()
        {
            var select = new SqlSelectTemplate("COL", null, null, null, null, null, "ORDERCOL");

            Assert.AreEqual("SELECT COL ORDER BY ORDERCOL", select.ToString());
            Assert.AreEqual("SELECT COL ORDER BY ORDERCOL, ORDERCOL2", select.ToString(null, null, null, null, null, null, "ORDERCOL2"));

            select = new SqlSelectTemplate();
            Assert.AreEqual("SELECT COL ORDER BY ORDERCOL", select.ToString("COL", null, null, null, null, null, "ORDERCOL"));

            select = new SqlSelectTemplate("COL", null, null, null, null, null, null);
            Assert.AreEqual("SELECT COL ORDER BY ORDERCOL", select.ToString(null, null, "ORDERCOL"));
            Assert.AreEqual("SELECT COL ORDER BY ORDERCOL", select.ToString(null, "ORDERCOL"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test all parameters.
        /// </summary>
        [TestMethod]
        public void Full_query_returns_correct_query()
        {
            string[] innerJoin = new string[] { "INNER_JOIN1", "INNER_JOIN2" };
            string[] outerJoin = new string[] { "OUTER_JOIN1", "OUTER_JOIN2" };
            string[] whereClause = new string[] { "CLAUSE1", "CLAUSE2" };

            SqlSelectTemplate select = new SqlSelectTemplate("COL", "TABLE1", innerJoin, outerJoin, whereClause, "GROUPCOL", "ORDERCOL");

            Assert.AreEqual("SELECT COL FROM TABLE1 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString());
            Assert.AreEqual("SELECT COL, COL2 FROM TABLE1 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString("COL2", null, null, null, null, null, null));
            Assert.AreEqual("SELECT COL FROM TABLE1, TABLE2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString(null, "TABLE2", null, null, null, null, null));
            Assert.AreEqual("SELECT COL FROM TABLE1 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString(null, null, innerJoin, null, null, null, null));
            Assert.AreEqual("SELECT COL FROM TABLE1 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString(null, null, null, outerJoin, null, null, null));
            Assert.AreEqual("SELECT COL FROM TABLE1 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString(null, null, null, null, whereClause, null, null));
            Assert.AreEqual("SELECT COL FROM TABLE1 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL, GROUPCOL2 ORDER BY ORDERCOL", select.ToString(null, null, null, null, null, "GROUPCOL2", null));
            Assert.AreEqual("SELECT COL FROM TABLE1 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL, ORDERCOL2", select.ToString(null, null, null, null, null, null, "ORDERCOL2"));

            select = new SqlSelectTemplate();
            Assert.AreEqual("SELECT COL FROM TABLE1 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL ORDER BY ORDERCOL", select.ToString("COL", "TABLE1", innerJoin, outerJoin, whereClause, "GROUPCOL", "ORDERCOL"));

            select = new SqlSelectTemplate("COL", null, null, null, null, null, null);
            Assert.AreEqual("SELECT COL, COL2 FROM TABLE2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString("COL2", "TABLE2", innerJoin, outerJoin, whereClause, "GROUPCOL2", "ORDERCOL2"));

            select = new SqlSelectTemplate(null, "TABLE1", null, null, null, null, null);
            Assert.AreEqual("SELECT COL2 FROM TABLE1, TABLE2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString("COL2", "TABLE2", innerJoin, outerJoin, whereClause, "GROUPCOL2", "ORDERCOL2"));

            select = new SqlSelectTemplate(null, null, innerJoin, null, null, null, null);
            Assert.AreEqual("SELECT COL2 FROM TABLE2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString("COL2", "TABLE2", innerJoin, outerJoin, whereClause, "GROUPCOL2", "ORDERCOL2"));

            select = new SqlSelectTemplate(null, null, null, outerJoin, null, null, null);
            Assert.AreEqual("SELECT COL2 FROM TABLE2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString("COL2", "TABLE2", innerJoin, outerJoin, whereClause, "GROUPCOL2", "ORDERCOL2"));

            select = new SqlSelectTemplate(null, null, null, null, whereClause, null, null);
            Assert.AreEqual("SELECT COL2 FROM TABLE2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) AND (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL2", select.ToString("COL2", "TABLE2", innerJoin, outerJoin, whereClause, "GROUPCOL2", "ORDERCOL2"));

            select = new SqlSelectTemplate(null, null, null, null, null, "GROUPCOL", null);
            Assert.AreEqual("SELECT COL2 FROM TABLE2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL, GROUPCOL2 ORDER BY ORDERCOL2", select.ToString("COL2", "TABLE2", innerJoin, outerJoin, whereClause, "GROUPCOL2", "ORDERCOL2"));

            select = new SqlSelectTemplate(null, null, null, null, null, null, "ORDERCOL");
            Assert.AreEqual("SELECT COL2 FROM TABLE2 INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2) GROUP BY GROUPCOL2 ORDER BY ORDERCOL, ORDERCOL2", select.ToString("COL2", "TABLE2", innerJoin, outerJoin, whereClause, "GROUPCOL2", "ORDERCOL2"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test empty arrays don't cause the template to fail.
        /// </summary>
        [TestMethod]
        public void Empty_arrays_dont_fail()
        {
            var empty = new string[] { };
            var innerJoin = new string[] { "INNER_JOIN1", "INNER_JOIN2" };
            var outerJoin = new string[] { "OUTER_JOIN1", "OUTER_JOIN2" };
            var where = new string[] { "CLAUSE1", "CLAUSE2" };

            var select = new SqlSelectTemplate("COL", null, empty, empty, empty, null, null);

            select.ToString();
            select.ToString(null, null, empty, empty, empty, null, null);
            select.ToString(empty);
            select.ToString(empty, null);
            select.ToString(empty, null, null);

            Assert.AreEqual("SELECT COL", select.ToString());
            Assert.AreEqual("SELECT COL", select.ToString(null, null, empty, empty, empty, null, null));
            Assert.AreEqual("SELECT COL INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2)", select.ToString(null, null, innerJoin, outerJoin, where, null, null));

            select = new SqlSelectTemplate("COL", null, innerJoin, outerJoin, where, null, null);

            Assert.AreEqual("SELECT COL INNER JOIN INNER_JOIN1 INNER JOIN INNER_JOIN2 LEFT OUTER JOIN OUTER_JOIN1 LEFT OUTER JOIN OUTER_JOIN2 WHERE (CLAUSE1) AND (CLAUSE2)", select.ToString(null, null, empty, empty, empty, null, null));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test the buffered query is constructed correctly.
        /// </summary>
        [TestMethod]
        public void Buffered_query_returns_same_result()
        {
            var innerJoin = new string[] { "INNER_JOIN1", "INNER_JOIN2" };
            var outerJoin = new string[] { "OUTER_JOIN1", "OUTER_JOIN2" };
            var whereClause = new string[] { "CLAUSE1", "CLAUSE2" };

            var select = new SqlSelectTemplate("COL", "TABLE1", innerJoin, outerJoin, whereClause, "GROUPCOL", "ORDERCOL");
            var buffered = new SqlSelectTemplate("COL", "TABLE1", innerJoin, outerJoin, whereClause, "GROUPCOL", "ORDERCOL", true);

            Assert.AreEqual(buffered.ToString(), select.ToString());
        }
    }
}
