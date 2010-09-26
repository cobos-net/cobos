using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.AsiaPac.WpfApplication.UI;
using Xunit;

namespace Intergraph.AsiaPac.WpfApplication.Tests.UI
{
	public class MessageChoiceTest
	{
		[Fact]
		public void MessageChoice_cancels()
		{
			string prompt = "Choose nothing, press cancel";
			string[] options = new string[] { "Option 1", "Option 2", "Option 3" };

			int? result = MessageChoice.Show( prompt, options );

			Assert.Null( result );
		}

		[Fact]
		public void MessageChoice_selects()
		{
			string prompt = "Choose 'Option 2', press OK";
			string[] options = new string[] { "Option 1", "Option 2", "Option 3" };

			int? result = MessageChoice.Show( prompt, options );

			Assert.NotNull( result );
			Assert.True( result.HasValue );
			Assert.True( result == 1 );
		}

		[Fact]
		public void MessageChoice_selects_but_cancels()
		{
			string prompt = "Choose 'Option 2', press cancel";
			string[] options = new string[] { "Option 1", "Option 2", "Option 3" };

			int? result = MessageChoice.Show( prompt, options );

			Assert.Null( result );
		}
	}
}
