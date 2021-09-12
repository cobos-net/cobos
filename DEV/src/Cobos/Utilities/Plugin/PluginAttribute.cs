// ----------------------------------------------------------------------------
// <copyright file="PluginAttribute.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Plugin
{
    using System;

    /// <summary>
    /// Mark a class as a plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PluginAttribute : Attribute
    {
    }
}
