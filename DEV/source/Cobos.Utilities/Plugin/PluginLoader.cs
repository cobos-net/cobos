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
    using System.Reflection;
    using System.Security.Permissions;

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
            IPluginClient plugin = null;

            foreach (string folder in folders)
            {
                if ((plugin = PluginLoader.LoadPluginFromFolder(folder, typeName)) != null)
                {
                    break;
                }
            }

            return plugin;
        }

        /// <summary>
        /// Load a plugin from the specified assembly.
        /// </summary>
        /// <param name="path">The assembly path.</param>
        /// <param name="typeName">The Type of the plugin class.</param>
        /// <returns>The plugin instance if found.</returns>
        public static IPluginClient LoadPluginFromAssembly(string path, string typeName)
        {
            if (!File.Exists(path))
            {
                throw new Exception(string.Format("The plugin file {0} does not exist", path));
            }

            try
            {
                FileIOPermission perm = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
                perm.Assert();

                Assembly asm = Assembly.Load(AssemblyName.GetAssemblyName(path));

                if (asm == null)
                {
                    return null;
                }

                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    object[] attrs = type.GetCustomAttributes(typeof(PluginAttribute), true);

                    if (attrs.Length > 0)
                    {
                        if (string.Compare(type.Name, typeName, true) == 0)
                        {
                            IPluginClient client = Activator.CreateInstance(type) as IPluginClient;

                            if (client != null)
                            {
                                return client;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }
    }
}
