// ----------------------------------------------------------------------------
// <copyright file="IPluginHost.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Plugin
{
    /// <summary>
    /// The plugin host interface.
    /// </summary>
    public interface IPluginHost
    {
        /// <summary>
        /// Register a plugin.
        /// May throw PluginNotSupportedException.
        /// </summary>
        /// <param name="plugin">The plugin to register.</param>
        void Register(IPluginClient plugin);
    }
}
