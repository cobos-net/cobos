using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Core.UI
{
	public class UserLoginDetails
	{
		public string Username;

		public string Password;

		public UserLoginDetails( string username, string password )
		{
			Username = username;
			Password = password;
		}
	}
}
