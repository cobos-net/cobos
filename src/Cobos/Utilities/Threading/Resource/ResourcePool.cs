// ----------------------------------------------------------------------------
// <copyright file="ResourcePool.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Resource
{
    /// <summary>
    /// Class factory for resource pools.
    /// </summary>
    /// <typeparam name="T">The type of resource managed by the pool.</typeparam>
    public static class ResourcePool<T>
    {
        /// <summary>
        /// Create a new resource pool defined by the provided settings.
        /// </summary>
        /// <param name="settings">The resource pool settings.</param>
        /// <returns>The newly created resource pool.</returns>
        public static IResourcePool<T> Create(ResourcePoolSettings<T> settings)
        {
            return new ResourcePoolImpl<T>(settings);
        }
    }
}
