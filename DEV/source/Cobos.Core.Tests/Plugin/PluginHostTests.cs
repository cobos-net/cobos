// ============================================================================
// Filename: PluginHostTests.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cobos.Core.Plugin;
using NUnit.Framework;
using System.IO;

namespace Cobos.Core.Plugin.Test
{
	public class TestPluginHost : IPluginHost
	{
		List<IPluginClient> _plugins = new List<IPluginClient>();

		[TestCase]
		public void Plugin_host_works()
		{
			string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

			UriBuilder uri = new UriBuilder( codeBase );
			string path = Uri.UnescapeDataString( uri.Path );
			string directory = Path.GetDirectoryName( path );

			IPluginClient plugin1 = PluginLoader.LoadPluginFromFolder( directory, "TestPluginClient1" );
			Assert.True( plugin1 != null );

			IPluginClient plugin2 = PluginLoader.LoadPluginFromFolder( directory, "TestPluginClient2" );
			Assert.True( plugin2 != null );

			plugin1.Configure( this, "Plugin1 Name", null );
			plugin2.Configure( this, "Plugin2 Name", null );

			Assert.True( _plugins.Count == 2 );
		}

		public void Register( IPluginClient plugin )
		{
			_plugins.Add( plugin );
		}
	}
}
