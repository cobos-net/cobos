﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cobos.Core.Plugin;
using Xunit;

namespace Cobos.Core.Plugin.Test
{
	[PluginAttribute]
	public class TestPluginClient1 : IPluginClient
	{
		public string Name
		{
			get
			{
				return _name;
			}
		}

		string _name;

		public void Configure( IPluginHost host, string name, string configPath )
		{
			_name = name;

			host.Register( this );
		}
	}
	
	[PluginAttribute]
	public class TestPluginClient2 : IPluginClient
	{
		public string Name
		{
			get
			{
				return _name;
			}
		}

		string _name;

		public void Configure( IPluginHost host, string name, string configPath )
		{
			_name = name;

			host.Register( this );
		}
	}
}
