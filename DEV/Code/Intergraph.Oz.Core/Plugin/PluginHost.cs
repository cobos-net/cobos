using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.Oz.Core.Plugin
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPluginHost
	{
		/// <summary>
		/// May throw PluginNotSupportedException
		/// </summary>
		/// <param name="plugin"></param>
		void Register( IPluginClient plugin );
	}

	/// <summary>
	/// 
	/// </summary>
	public class PluginNotSupportedException : System.Exception
	{
		public PluginNotSupportedException( string message )
			: base( message )
		{
		}
	}
}
