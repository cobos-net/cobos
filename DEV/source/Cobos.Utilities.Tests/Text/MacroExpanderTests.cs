// ----------------------------------------------------------------------------
// <copyright file="MacroExpanderTests.cs" company="Cobos SDK">
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
using System.Text.RegularExpressions;
using Cobos.Utilities.Text;
using NUnit.Framework;

namespace Cobos.Utilities.Tests.Text
{
    [TestFixture]
    public class MacroExpanderTests
    {
        [TestCase]
        public void Simple_macro_expansion_succeeds()
        {
            MacroExpander expander = new MacroExpander();

            expander.Add("Macro1", "an expanded macro 1");
            expander.Add("Macro2", "an expanded macro 2");
            expander.Add("Macro3", "an expanded macro 3");

            // do some explicit checking
            string replaced = expander.Expand("This is text with Macro1 in the middle");
            Assert.AreEqual("This is text with an expanded macro 1 in the middle", replaced);

            replaced = expander.Expand("This is text with Macro2 in the middle");
            Assert.AreEqual("This is text with an expanded macro 2 in the middle", replaced);

            replaced = expander.Expand("This is text with Macro3 in the middle");
            Assert.AreEqual("This is text with an expanded macro 3 in the middle", replaced);

            replaced = expander.Expand("This contains Macro1 then Macro2 and finally Macro3 at the end");
            Assert.AreEqual("This contains an expanded macro 1 then an expanded macro 2 and finally an expanded macro 3 at the end", replaced);

            replaced = expander.Expand("This contains Macro1 once and then Macro1 again");
            Assert.AreEqual("This contains an expanded macro 1 once and then an expanded macro 1 again", replaced);

            replaced = expander.Expand("This contains Macro1 and then Macro4 which won't change");
            Assert.AreEqual("This contains an expanded macro 1 and then Macro4 which won't change", replaced);


        }

        [TestCase]
        public void Formatted_macro_expansion_succeeds()
        {
            // test copying an expander works properly
            MacroExpander original = new MacroExpander("$(_TOKEN_)");

            original.Add("Macro1", "an expanded macro 1");
            original.Add("Macro2", "an expanded macro 2");

            MacroExpander expander = new MacroExpander(original);
            expander.Add("Macro3", "an expanded macro 3");

            // do some explicit checking
            string replaced = expander.Expand("This is text with $(Macro1) in the middle");
            Assert.AreEqual("This is text with an expanded macro 1 in the middle", replaced);

            replaced = expander.Expand("This is text with unformatted Macro1 in the middle");
            Assert.AreEqual("This is text with unformatted Macro1 in the middle", replaced);

            replaced = expander.Expand("This is text with $(Macro2) in the middle");
            Assert.AreEqual("This is text with an expanded macro 2 in the middle", replaced);

            replaced = expander.Expand("This is text with $(Macro3) in the middle");
            Assert.AreEqual("This is text with an expanded macro 3 in the middle", replaced);

            replaced = expander.Expand("This contains $(Macro1) then $(Macro2) and finally $(Macro3) at the end");
            Assert.AreEqual("This contains an expanded macro 1 then an expanded macro 2 and finally an expanded macro 3 at the end", replaced);

            replaced = expander.Expand("This contains $(Macro1) once and then $(Macro1) again");
            Assert.AreEqual("This contains an expanded macro 1 once and then an expanded macro 1 again", replaced);

            replaced = expander.Expand("This contains $(Macro1) and then $(Macro4) which won't change");
            Assert.AreEqual("This contains an expanded macro 1 and then $(Macro4) which won't change", replaced);
        }
    }
}
