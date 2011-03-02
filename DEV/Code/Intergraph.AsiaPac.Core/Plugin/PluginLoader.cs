using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security.Permissions;

namespace Intergraph.AsiaPac.Core.Plugin
{
	/// <summary>
	/// Plugin Loader utility class
	/// </summary>
	public static class PluginLoader
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IPluginClient LoadPluginFromDirectory( string path, string typeName )
		{
			if ( !Directory.Exists( path ) )
			{
				throw new Exception( String.Format( "The plugin folder {0} does not exist", path ) );
			}

			List<IPluginClient> plugins = new List<IPluginClient>();

			string[] pluginFiles = Directory.GetFiles( path, "*.dll", SearchOption.AllDirectories );

			foreach ( string p in pluginFiles )
			{
				IPluginClient client = LoadPluginFromAssembly( p, typeName );

				if ( client != null )
				{
					return client;
				}
			}

			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IPluginClient LoadPluginFromAssembly( string path, string typeName )
		{
			if ( !File.Exists( path ) )
			{
				throw new Exception( String.Format( "The plugin file {0} does not exist", path ) );
			}

			try
			{
				FileIOPermission perm = new FileIOPermission( FileIOPermissionAccess.AllAccess, path );
				perm.Assert();

				Assembly asm = Assembly.Load( AssemblyName.GetAssemblyName( path ) );

				if ( asm == null )
				{
					return null;
				}

				foreach ( Type type in asm.GetTypes() )
				{
					if ( type.IsAbstract )
					{
						continue;
					}

					object[] attrs = type.GetCustomAttributes( typeof( PluginAttribute ), true );

					if ( attrs.Length > 0 )
					{
						if ( String.Compare( type.Name, typeName, true ) == 0 )
						{
							IPluginClient client = Activator.CreateInstance( type ) as IPluginClient;

							if ( client != null )
							{
								return client;
							}
						}
					}
				}
			}
			catch ( Exception )
			{
				return null;
			}

			return null;
		}
	}
}
