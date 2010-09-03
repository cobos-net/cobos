using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Intergraph.Oz.Utilities.Text;
using Xunit;

namespace Intergraph.Oz.Utilities.Tests.Text
{
	public class MacroExpanderTests
	{
		[Fact]
		public void Simple_macro_expansion_succeeds()
		{
			MacroExpander expander = new MacroExpander();

			expander.Add( "Macro1", "an expanded macro 1" );
			expander.Add( "Macro2", "an expanded macro 2" );
			expander.Add( "Macro3", "an expanded macro 3" );

			// do some explicit checking
			string replaced = expander.Expand( "This is text with Macro1 in the middle" );
			Assert.Equal( "This is text with an expanded macro 1 in the middle", replaced );

			replaced = expander.Expand( "This is text with Macro2 in the middle" );
			Assert.Equal( "This is text with an expanded macro 2 in the middle", replaced );

			replaced = expander.Expand( "This is text with Macro3 in the middle" );
			Assert.Equal( "This is text with an expanded macro 3 in the middle", replaced );

			replaced = expander.Expand( "This contains Macro1 then Macro2 and finally Macro3 at the end" );
			Assert.Equal( "This contains an expanded macro 1 then an expanded macro 2 and finally an expanded macro 3 at the end", replaced );

			replaced = expander.Expand( "This contains Macro1 once and then Macro1 again" );
			Assert.Equal( "This contains an expanded macro 1 once and then an expanded macro 1 again", replaced );

			replaced = expander.Expand( "This contains Macro1 and then Macro4 which won't change" );
			Assert.Equal( "This contains an expanded macro 1 and then Macro4 which won't change", replaced );


		}

		[Fact]
		public void Formatted_macro_expansion_succeeds()
		{
			// test copying an expander works properly
			MacroExpander original = new MacroExpander( "$(_TOKEN_)" );

			original.Add( "Macro1", "an expanded macro 1" );
			original.Add( "Macro2", "an expanded macro 2" );

			MacroExpander expander = new MacroExpander( original );
			expander.Add( "Macro3", "an expanded macro 3" );

			// do some explicit checking
			string replaced = expander.Expand( "This is text with $(Macro1) in the middle" );
			Assert.Equal( "This is text with an expanded macro 1 in the middle", replaced );

			replaced = expander.Expand( "This is text with unformatted Macro1 in the middle" );
			Assert.Equal( "This is text with unformatted Macro1 in the middle", replaced );

			replaced = expander.Expand( "This is text with $(Macro2) in the middle" );
			Assert.Equal( "This is text with an expanded macro 2 in the middle", replaced );

			replaced = expander.Expand( "This is text with $(Macro3) in the middle" );
			Assert.Equal( "This is text with an expanded macro 3 in the middle", replaced );

			replaced = expander.Expand( "This contains $(Macro1) then $(Macro2) and finally $(Macro3) at the end" );
			Assert.Equal( "This contains an expanded macro 1 then an expanded macro 2 and finally an expanded macro 3 at the end", replaced );

			replaced = expander.Expand( "This contains $(Macro1) once and then $(Macro1) again" );
			Assert.Equal( "This contains an expanded macro 1 once and then an expanded macro 1 again", replaced );

			replaced = expander.Expand( "This contains $(Macro1) and then $(Macro4) which won't change" );
			Assert.Equal( "This contains an expanded macro 1 and then $(Macro4) which won't change", replaced );
		}
	}
}
