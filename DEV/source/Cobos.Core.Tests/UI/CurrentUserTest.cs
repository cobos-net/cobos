// ============================================================================
// Filename: CurrentUserTest.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 21-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

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
