using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cobos.Core.UI;

namespace Cobos.WpfApplication.UI
{
	public static class PhoneViewWpfApplication
	{
		/// <summary>
		/// Initialise the PhoneView WPF application.
		/// </summary>
		public static void Initialise()
		{
			CommandLineArgs args = new CommandLineArgs();

			string home = args[ "home" ];
			string settings = args[ "settings" ];

			if ( home == null )
			{
				home = AppDomain.CurrentDomain.BaseDirectory;
			}

			CobosApplication.Current.Initialise( new CurrentCursor(), new MessageBoxHandler(), new ProgressDialog(), new CurrentUser(), home );
		}

		/// <summary>
		/// Terminate the application and database connection
		/// </summary>
		public static void Terminate()
		{
			CobosApplication.Current.Dispose();
		}
	}
}
