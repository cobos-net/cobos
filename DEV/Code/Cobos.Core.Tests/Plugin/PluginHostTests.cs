﻿using System;
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
