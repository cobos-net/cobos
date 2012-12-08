using System;
using System.Diagnostics;
using Cobos.Core.UI;

namespace Cobos.Core.Tests.UI
{
	public class CurrentUserTest : ICurrentUser
	{
		/// <summary>
		/// Special test value to simulate logon/logoff success or failure
		/// </summary>
		public static UserLoginDetails NextShowLoginResult;

		public static DatabaseLoginDetails NextShowDatabaseConnectResult;

		public static DatabaseSettingsDetails NextShowDatabaseSettingsResult;

		public UserLoginDetails ShowLogin( string prompt, string username )
		{
			return NextShowLoginResult;
		}

		public DatabaseLoginDetails ShowDatabaseLogin( string prompt, string username, string hostname, int? port )
		{
			return NextShowDatabaseConnectResult;
		}

		public DatabaseSettingsDetails ShowDatabaseSettings( string prompt, string hostname, int? port )
		{
			return NextShowDatabaseSettingsResult;
		}

	}
}
