// ----------------------------------------------------------------------------
// <copyright file="IPluginClient.cs" company="Cobos SDK">
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
