// ----------------------------------------------------------------------------
// <copyright file="IDatabaseAdapterProvider.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Adapters
{
    /// <summary>
    /// Represents a Database Adapter provider.
    /// </summary>
    public interface IDatabaseAdapterProvider
    {
        /// <summary>
        /// Register all provided Database Adapter types with the factory.
        /// </summary>
        /// <param name="factory">The factory to register all types with.</param>
        void RegisterDatabaseAdapters(DatabaseAdapterFactory factory);
    }
}
