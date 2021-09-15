// ----------------------------------------------------------------------------
// <copyright file="IPluginClient.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Plugin
{
    /// <summary>
    /// Plugin client interface.
    /// </summary>
    public interface IPluginClient
    {
        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Configure this plugin.
        /// </summary>
        /// <param name="host">The host that has registered the plugin.</param>
        /// <param name="name">The name of this instance of the plugin.</param>
        /// <param name="configPath">The path to the configuration file for this plugin.</param>
        void Configure(IPluginHost host, string name, string configPath);
    }
}
