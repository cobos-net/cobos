using System;
using System.Collections.Generic;
using System.Text;
using Cobos.WpfApplication.UI;
using NUnit.Framework;

namespace Cobos.WpfApplication.Tests.UI
{
	[TestFixture]
	public class MessageChoiceTest
	{
		[TestCase]
		public void MessageChoice_cancels()
		{
			string prompt = "Choose nothing, press cancel";
			string[] options = new string[] { "Option 1", "Option 2", "Option 3" };

			int? result = MessageChoice.Show( prompt, options );

			Assert.Null( result );
		}

		[TestCase]
		public void MessageChoice_selects()
		{
			string prompt = "Choose 'Option 2', press OK";
			string[] options = new string[] { "Option 1", "Option 2", "Option 3" };

			int? result = MessageChoice.Show( prompt, options );

			Assert.NotNull( result );
			Assert.True( result.HasValue );
			Assert.True( result == 1 );
		}

		[TestCase]
		public void MessageChoice_selects_but_cancels()
		{
			string prompt = "Choose 'Option 2', press cancel";
			string[] options = new string[] { "Option 1", "Option 2", "Option 3" };

			int? result = MessageChoice.Show( prompt, options );

			Assert.Null( result );
		}
	}
}
