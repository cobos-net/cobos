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
		public bool? NextShowLoginResult = null;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool? ShowLogin( string prompt )
		{
			Debug.WriteLine( string.Format( "Show login = {0}", prompt ) );

			return NextShowLoginResult;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Username
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Password
		{
			get;
			set;
		}
		
	}
}
