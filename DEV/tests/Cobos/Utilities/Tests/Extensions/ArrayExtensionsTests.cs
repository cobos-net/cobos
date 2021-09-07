// ----------------------------------------------------------------------------
// <copyright file="ArrayExtensionsTests.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Tests.Extensions
{
    using System;
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
                new int[] { 9 }
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
                null
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
                null
            };

            int[] result = multi.ConcatenateAll();

            CollectionAssert.AreEqual(new int[] { 0, 1, 2, 7, 8 }, result);
        }
    }
}
