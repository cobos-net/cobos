// ----------------------------------------------------------------------------
// <copyright file="MacroExpanderTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.Text
{
    using Cobos.Utilities.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="MacroExpander"/> class.
    /// </summary>
    [TestClass]
    public class MacroExpanderTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test simple macro expansion.
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that unformatted macros are ignored.
        /// </summary>
        [TestMethod]
        public void Only_formatted_macro_expansion_succeeds()
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
