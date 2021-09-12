// ----------------------------------------------------------------------------
// <copyright file="DatabaseAdapterProviderAttribute.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Adapters
{
    using System;

    /// <summary>
    /// Mark a class as a Database Adapter provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseAdapterProviderAttribute : Attribute
    {
    }
}
