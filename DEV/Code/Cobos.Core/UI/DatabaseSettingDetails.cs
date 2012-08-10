using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Core.UI
{
	public class DatabaseSettingsDetails
	{
		public string Hostname;

		public int Port;

		public DatabaseSettingsDetails( string hostname, int port )
		{
			Hostname = hostname;
			Port = port;
		}
	}
}
