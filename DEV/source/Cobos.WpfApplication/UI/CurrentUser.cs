// ----------------------------------------------------------------------------
// <copyright file="CurrentUser.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.WpfApplication.UI
{
    using Cobos.WpfApplication.Interfaces;
    using Cobos.WpfApplication.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Text;

	public class CurrentUser : ICurrentUser
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
