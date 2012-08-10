using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Core.UI
{
	public class DatabaseLoginDetails
	{
		public string Username;

		public string Password;

		public string Hostname;

		public int Port;

		public DatabaseLoginDetails( string username, string password, string hostname, int port )
		{
			Username = username;
			Password = password;
			Hostname = hostname;
			Port = port;
		}

		public DatabaseLoginDetails( string username, string password, DatabaseSettingsDetails settings )
			: this( username, password, settings.Hostname, settings.Port )
		{
		}

	}
}
