// ----------------------------------------------------------------------------
// <copyright file="PluginLoader.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Plugin Loader utility class
    /// </summary>
    public static class PluginLoader
    {
        /// <summary>
        /// Load a plugin from the specified folder.
        /// </summary>
        /// <param name="path">The folder path.</param>
        /// <param name="typeName">The Type of the plugin class.</param>
        /// <returns>The plugin instance if found.</returns>
        public static IPluginClient LoadPluginFromFolder(string path, string typeName)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception(string.Format("The plugin folder {0} does not exist", path));
            }

            List<IPluginClient> plugins = new List<IPluginClient>();

            string[] pluginFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

            foreach (string p in pluginFiles)
            {
                IPluginClient client = LoadPluginFromAssembly(p, typeName);

                if (client != null)
                {
                    return client;
                }
            }

            return null;
        }

        /// <summary>
        /// Load a plugin from an assembly in one of the specified folders.
        /// </summary>
        /// <param name="folders">The list of folders to look in.</param>
        /// <param name="typeName">The Type of the plugin class.</param>
        /// <returns>The plugin instance if found.</returns>
        public static IPluginClient LoadPluginFromFolders(string[] folders, string typeName)
        {
            foreach (string folder in folders)
            {
                var plugin = PluginLoader.LoadPluginFromFolder(folder, typeName);

                if (plugin != null)
                {
                    return plugin;
                }
            }

            return null;
        }

        /// <summary>
        /// Load a plugin from the specified assembly.
        /// </summary>
        /// <param name="path">The assembly path.</param>
        /// <param name="typeName">The Type of the plugin class.</param>
        /// <returns>The plugin instance if found.</returns>
        public static IPluginClient LoadPluginFromAssembly(string path, string typeName)
        {
            var assembly = AssemblyExtensions.LoadAssembly(path);

            var types = assembly.GetAllTypesWithAttribute<PluginAttribute>();

            var type = types.FirstOrDefault(t => string.Compare(t.Name, typeName) == 0);

            if (type == null)
            {
                return null;
            }

            return Activator.CreateInstance(type) as IPluginClient;
        }
    }
}
