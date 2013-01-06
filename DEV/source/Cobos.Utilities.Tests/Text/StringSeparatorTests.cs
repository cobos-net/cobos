// ----------------------------------------------------------------------------
// <copyright file="StringSeperatorTests.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
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

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Cobos.Utilities.Text;

namespace Cobos.Utilities.Tests.Text
{
    [TestFixture]
    public class StringSeparatorTests
    {
        [TestCase]
        public void can_separate_strings()
        {
            string source = "0|1|2|3|4|5";

            for (int i = 0; i <= 5; ++i)
            {
                string token = StringSeparator.GetTokenAt(source, '|', i);
                Assert.NotNull(token);
                Assert.AreEqual(i.ToString(), token);
            }
        }

        [TestCase]
        public void can_handle_mixed_separators()
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

        [TestCase]
        public void can_handle_invalid_separator_character()
        {
            string source = "0|1|2|3|4|5";

            string token = StringSeparator.GetTokenAt(source, ',', 0);
            Assert.AreEqual(source, token);

            token = StringSeparator.GetTokenAt(source, ',', 1);
            Assert.Null(token);
        }

        [TestCase]
        public void can_handle_invalid_index()
        {
            string source = "0|1|2|3|4|5";

            Assert.Null(StringSeparator.GetTokenAt(source, '|', -1));
            Assert.NotNull(StringSeparator.GetTokenAt(source, '|', 0));
            Assert.NotNull(StringSeparator.GetTokenAt(source, '|', 5));
            Assert.Null(StringSeparator.GetTokenAt(source, '|', 6));
        }

        [TestCase]
        public void can_handle_invalid_input()
        {
            Assert.Null(StringSeparator.GetTokenAt(null, '|', 0));
            Assert.Null(StringSeparator.GetTokenAt("", '|', 0));
            Assert.NotNull(StringSeparator.GetTokenAt(" ", '|', 0));
            Assert.NotNull(StringSeparator.GetTokenAt(" ", ' ', 0));
        }

    }
}
