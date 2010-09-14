using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.Oz.Core.Plugin
{
	public interface IPluginClient
	{
		string Name
		{
			get;
		}

		void Configure( IPluginHost host, string name, string configPath );
	}

	[AttributeUsage( AttributeTargets.Class )]
	public sealed class PluginAttribute : Attribute
	{ 
	}
}
