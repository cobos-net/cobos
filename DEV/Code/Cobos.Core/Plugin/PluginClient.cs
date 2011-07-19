using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.AsiaPac.Utilities.File;

namespace Intergraph.AsiaPac.Core.Plugin
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
