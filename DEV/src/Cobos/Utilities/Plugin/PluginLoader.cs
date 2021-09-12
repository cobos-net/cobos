// ----------------------------------------------------------------------------
// <copyright file="PluginLoader.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Plugin Loader utility class.
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

            _ = new List<IPluginClient>();

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

            var types = assembly.GetAllTypesWithCustomAttribute<PluginAttribute>();

            var type = types.FirstOrDefault(t => string.Compare(t.Name, typeName) == 0);

            if (type == null)
            {
                return null;
            }

            return Activator.CreateInstance(type) as IPluginClient;
        }
    }
}
