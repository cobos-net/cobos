// ----------------------------------------------------------------------------
// <copyright file="ArrayExtensionsTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.Extensions
{
    using Cobos.Utilities.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="ArrayExtensions"/> class.
    /// </summary>
    [TestClass]
    public class ArrayExtensionsTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test concatenating multi-dimensional arrays.
        /// </summary>
        [TestMethod]
        public void Can_concatenate_multiple_arrays()
        {
            int[][] multi = new int[][]
            {
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5, 6 },
                new int[] { 7, 8 },
                new int[] { 9 },
            };

            int[] result = multi.ConcatenateAll();

            CollectionAssert.AreEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, result);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test concatenating multi-dimensional arrays that are all null.
        /// </summary>
        [TestMethod]
        public void Can_concatenate_multiple_null_arrays()
        {
            int[][] multi = new int[][]
            {
                null,
                null,
                null,
                null,
            };

            int[] result = multi.ConcatenateAll();

            Assert.IsNull(result);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test concatenating multi-dimensional arrays that are a mix of null and non-null.
        /// </summary>
        [TestMethod]
        public void Can_concatenate_multiple_arrays_with_some_nulls()
        {
            int[][] multi = new int[][]
            {
                new int[] { 0, 1, 2 },
                null,
                new int[] { 7, 8 },
                null,
            };

            int[] result = multi.ConcatenateAll();

            CollectionAssert.AreEqual(new int[] { 0, 1, 2, 7, 8 }, result);
        }
    }
}
