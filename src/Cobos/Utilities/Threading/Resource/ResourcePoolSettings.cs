// ----------------------------------------------------------------------------
// <copyright file="ResourcePoolSettings.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Resource
{
    /// <summary>
    /// Manage the settings for the resource pool.
    /// </summary>
    /// <typeparam name="T">The type of resource in the pool.</typeparam>
    public struct ResourcePoolSettings<T>
    {
        /// <summary>
        /// The minimum size of the pool. May be 0.
        /// </summary>
        public uint MinPoolSize;

        /// <summary>
        /// The maximum size of the pool.  The number of
        /// resources will grow to this size on demand.
        /// Once the maximum size is reached, all further
        /// requests will be queued.
        /// </summary>
        public uint MaxPoolSize;

        /// <summary>
        /// NOT YET SUPPORTED.
        /// The maximum lifetime (in seconds) for a resource in the pool.
        /// When a connection is returned to the pool, its creation time is compared with the current time,
        /// and the resource is disposed if that time span (in seconds) exceeds the value.
        /// A value of zero causes pooled connections to have the maximum connection time-out.
        /// </summary>
        public uint ResourceLifetime;

        /// <summary>
        /// User supplied allocator for creating new resources to add
        /// to the pool as demand increases up to _maxPoolSize.
        /// </summary>
        public IResourceAllocator<T> Allocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePoolSettings{T}"/> struct.
        /// </summary>
        /// <param name="minPoolSize">The minimum size of the pool.</param>
        /// <param name="maxPoolSize">The maximum size of the pool.</param>
        /// <param name="allocator">The allocator for creating new resources.</param>
        public ResourcePoolSettings(uint minPoolSize, uint maxPoolSize, IResourceAllocator<T> allocator)
        {
            this.MinPoolSize = minPoolSize;
            this.MaxPoolSize = maxPoolSize;
            this.ResourceLifetime = 0;
            this.Allocator = allocator;
        }

        /// <summary>
        /// Get a unique key to identify these settings.
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
