using System;
using System.Collections.Generic;
using System.Text;
using Cobos.Core.UI;

namespace Cobos.WpfApplication.UI
{
	class CurrentUser : ICurrentUser
	{
		#region ICurrentUser implementation

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool? ShowLogin( string prompt )
		{
			LogonDialog dlg = new LogonDialog();
			dlg._prompt.Text = prompt;

			bool? result = dlg.ShowDialog();

			if ( !result.HasValue || result == false )
			{
				return null;
			}

			Username = dlg._user.Text;
			Password = dlg._password.Password;

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Username
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Password
		{
			get;
			private set;
		}

		#endregion
	}
}
