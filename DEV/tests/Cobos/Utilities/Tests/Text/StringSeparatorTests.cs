// ----------------------------------------------------------------------------
// <copyright file="StringSeparatorTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.Text
{
    using Cobos.Utilities.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="StringSeparator"/> class.
    /// </summary>
    [TestClass]
    public class StringSeparatorTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test basic separator.
        /// </summary>
        [TestMethod]
        public void Can_separate_strings()
        {
            string source = "0|1|2|3|4|5";

            for (int i = 0; i <= 5; ++i)
            {
                string token = StringSeparator.GetTokenAt(source, '|', i);
                Assert.IsNotNull(token);
                Assert.AreEqual(i.ToString(), token);
            }
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a string with a mix of separators.
        /// 2. Test that we can extract the values correctly.
        /// </summary>
        [TestMethod]
        public void Can_handle_mixed_separators()
        {
            string source = "0|1|2,3|4|5";

            string token = StringSeparator.GetTokenAt(source, ',', 0);
            Assert.AreEqual("0|1|2", token);

            token = StringSeparator.GetTokenAt(source, ',', 1);
            Assert.AreEqual("3|4|5", token);

            token = StringSeparator.GetTokenAt(source, '|', 0);
            Assert.AreEqual("0", token);

            token = StringSeparator.GetTokenAt(source, '|', 1);
            Assert.AreEqual("1", token);

            token = StringSeparator.GetTokenAt(source, '|', 2);
            Assert.AreEqual("2,3", token);

            token = StringSeparator.GetTokenAt(source, '|', 3);
            Assert.AreEqual("4", token);

            token = StringSeparator.GetTokenAt(source, '|', 4);
            Assert.AreEqual("5", token);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test invalid separator characters.
        /// </summary>
        [TestMethod]
        public void Can_handle_invalid_separator_character()
        {
            string source = "0|1|2|3|4|5";

            string token = StringSeparator.GetTokenAt(source, ',', 0);
            Assert.AreEqual(source, token);

            token = StringSeparator.GetTokenAt(source, ',', 1);
            Assert.IsNull(token);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test invalid index values.
        /// </summary>
        [TestMethod]
        public void Can_handle_invalid_index()
        {
            string source = "0|1|2|3|4|5";

            Assert.IsNull(StringSeparator.GetTokenAt(source, '|', -1));
            Assert.IsNotNull(StringSeparator.GetTokenAt(source, '|', 0));
            Assert.IsNotNull(StringSeparator.GetTokenAt(source, '|', 5));
            Assert.IsNull(StringSeparator.GetTokenAt(source, '|', 6));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test all invalid input.
        /// </summary>
        [TestMethod]
        public void Can_handle_invalid_input()
        {
            Assert.IsNull(StringSeparator.GetTokenAt(null, '|', 0));
            Assert.IsNull(StringSeparator.GetTokenAt(string.Empty, '|', 0));
            Assert.IsNotNull(StringSeparator.GetTokenAt(" ", '|', 0));
            Assert.IsNotNull(StringSeparator.GetTokenAt(" ", ' ', 0));
        }
    }
}
