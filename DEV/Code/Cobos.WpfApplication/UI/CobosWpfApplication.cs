using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cobos.Core.UI;

namespace Cobos.WpfApplication.UI
{
	public class CobosWpfApplication : CobosApplication
	{
		/// <summary>
		/// Initialise the Cobos application using WPF.
		/// </summary>
		public virtual void Initialise()
		{
			CommandLineArgs args = new CommandLineArgs();

			string home = args[ "home" ];
			string settings = args[ "settings" ];

			if ( home == null )
			{
				home = AppDomain.CurrentDomain.BaseDirectory;
			}

			base.Initialise( new CurrentCursor(), new MessageBoxHandler(), new ProgressDialog(), new CurrentUser(), home );
		}
	}
}
