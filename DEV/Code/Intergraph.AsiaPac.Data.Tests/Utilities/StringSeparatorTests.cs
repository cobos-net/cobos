using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Intergraph.AsiaPac.Data.Utilities;

namespace Intergraph.AsiaPac.Data.Tests.Utilities
{
	public class StringSeparatorTests
	{
		[Fact]
		public void can_separate_strings()
		{
			string source = "0|1|2|3|4|5";

			for ( int i = 0; i <= 5; ++i )
			{
				string token = StringSeparator.GetTokenAt( source, '|', i );
				Assert.NotNull( token );
				Assert.Equal<string>( i.ToString(), token );
			}
		}

		[Fact]
		public void can_handle_mixed_separators()
		{
			string source = "0|1|2,3|4|5";

			string token = StringSeparator.GetTokenAt( source, ',', 0 );
			Assert.Equal<string>( "0|1|2", token );
			
			token = StringSeparator.GetTokenAt( source, ',', 1 );
			Assert.Equal<string>( "3|4|5", token );

			token = StringSeparator.GetTokenAt( source, '|', 0 );
			Assert.Equal<string>( "0", token );

			token = StringSeparator.GetTokenAt( source, '|', 1 );
			Assert.Equal<string>( "1", token );

			token = StringSeparator.GetTokenAt( source, '|', 2 );
			Assert.Equal<string>( "2,3", token );

			token = StringSeparator.GetTokenAt( source, '|', 3 );
			Assert.Equal<string>( "4", token );

			token = StringSeparator.GetTokenAt( source, '|', 4 );
			Assert.Equal<string>( "5", token );
		}

		[Fact]
		public void can_handle_invalid_separator_character()
		{
			string source = "0|1|2|3|4|5";

			string token = StringSeparator.GetTokenAt( source, ',', 0 );
			Assert.Equal<string>( source, token );

			token = StringSeparator.GetTokenAt( source, ',', 1 );
			Assert.Null( token );
		}

		[Fact]
		public void can_handle_invalid_index()
		{
			string source = "0|1|2|3|4|5";

			Assert.Null( StringSeparator.GetTokenAt( source, '|', -1 ) );
			Assert.NotNull( StringSeparator.GetTokenAt( source, '|', 0 ) );
			Assert.NotNull( StringSeparator.GetTokenAt( source, '|', 5 ) );
			Assert.Null( StringSeparator.GetTokenAt( source, '|', 6 ) );
		}

		[Fact]
		public void can_handle_invalid_input()
		{
			Assert.Null( StringSeparator.GetTokenAt( null, '|', 0 ) );
			Assert.Null( StringSeparator.GetTokenAt( "", '|', 0 ) );
			Assert.NotNull( StringSeparator.GetTokenAt( " ", '|', 0 ) );
			Assert.NotNull( StringSeparator.GetTokenAt( " ", ' ', 0 ) );
		}

	}
}
