// ----------------------------------------------------------------------------
// <copyright file="PluginNotSupportedException.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Plugin
{
    /// <summary>
    /// Exception class for plugin errors.
    /// </summary>
    public class PluginNotSupportedException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The reason why the exception was thrown.</param>
        public PluginNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
