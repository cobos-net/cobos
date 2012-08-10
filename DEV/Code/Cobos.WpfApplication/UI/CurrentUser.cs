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
		public UserLoginDetails ShowLogin( string prompt, string username )
		{
			LoginDialog dlg = new LoginDialog();

			if ( !string.IsNullOrEmpty( prompt ) )
			{
				dlg._prompt.Text = prompt;
			}

			if ( !string.IsNullOrEmpty( username ) )
			{
				dlg._user.Text = username;
			}

			bool? result = dlg.ShowDialog();

			if ( !result.GetValueOrDefault( false ) )
			{
				return null;
			}

			return new UserLoginDetails( dlg._user.Text, dlg._password.Password );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="username"></param>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public DatabaseLoginDetails ShowDatabaseLogin( string prompt, string username, string hostname, int? port )
		{
			DatabaseLoginDialog dlg = new DatabaseLoginDialog();

			if ( !string.IsNullOrEmpty( prompt ) )
			{
				dlg._prompt.Text = prompt;
			}

			if ( !string.IsNullOrEmpty( username ) )
			{
				dlg._user.Text = username;
			}

			if ( !string.IsNullOrEmpty( hostname ) )
			{
				dlg._hostname.Text = hostname;
			}

			if ( port.HasValue )
			{
				dlg._port.Text = port.Value.ToString();
			}

			bool? result = dlg.ShowDialog();

			if ( !result.GetValueOrDefault( false ) )
			{
				return null;
			}

			return new DatabaseLoginDetails( dlg._user.Text, dlg._password.Password, dlg._hostname.Text, int.Parse( dlg._port.Text ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public DatabaseSettingsDetails ShowDatabaseSettings( string prompt, string hostname, int? port )
		{
			DatabaseSettingsDialog dlg = new DatabaseSettingsDialog();

			if ( !string.IsNullOrEmpty( prompt ) )
			{
				dlg._prompt.Text = prompt;
			}

			if ( !string.IsNullOrEmpty( hostname ) )
			{
				dlg._hostname.Text = hostname;
			}

			if ( port.HasValue )
			{
				dlg._port.Text = port.Value.ToString();
			}

			bool? result = dlg.ShowDialog();

			if ( !result.GetValueOrDefault( false ) )
			{
				return null;
			}

			return new DatabaseSettingsDetails( dlg._hostname.Text, int.Parse( dlg._port.Text ) );
		}

		#endregion
	}
}
