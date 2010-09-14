using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using Intergraph.Oz.WpfApplication.UI;
using Intergraph.Oz.Core.UI;

namespace Intergraph.Oz.WpfApplication.Tests.UI
{
	public class ProgressDialogTest
	{
		[Fact]
		public void Progress_can_show_and_hide()
		{
			IProgressBar dlg = new ProgressDialog();

			dlg.Maximum = 20;
			dlg.Value = 10;
			dlg.Prompt = "Initialised halfway and will close in 2 seconds";

			dlg.Visible = true;
			Assert.True( dlg.Visible );

			Thread.Sleep( 2000 );

			dlg.Visible = false;
			Assert.False( dlg.Visible );
		}

		public void Progress_can_update()
		{
			IProgressBar dlg = new ProgressDialog();

			dlg.Maximum = 20;
			dlg.Prompt = "Will go from 0-20 in 10 seconds...";

			dlg.Visible = true;
			Assert.True( dlg.Visible );

			for ( int i = 1; i <= 20; ++i )
			{
				dlg.PerformStep();
				Assert.Equal<int>( i, dlg.Value );
				Thread.Sleep( 500 );
			}

			dlg.Visible = false;
			Assert.False( dlg.Visible );
		}

	}
}
