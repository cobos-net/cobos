// ----------------------------------------------------------------------------
// <copyright file="IResourcePool.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Resource
{
    using System;

    /// <summary>
    /// Represents a resource pool.
    /// </summary>
    /// <typeparam name="T">The type of resource managed by the pool.</typeparam>
    public interface IResourcePool<T> : IDisposable
    {
        /// <summary>
        /// Gets information about the state of the pool.
        /// </summary>
        ResourcePoolStatistics Statistics
        {
            get;
        }

        /// <summary>
        /// Acquire the next available resource from the pool.
        /// Resources are returned to the pool by calling the
        /// IResource.Dispose method.
        /// </summary>
        /// <returns>A handle to the newly acquired resource.</returns>
        IResource<T> AcquireResource();
    }
}
