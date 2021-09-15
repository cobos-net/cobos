// ----------------------------------------------------------------------------
// <copyright file="ResourceHandle.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Resource
{
    using System;

    /// <summary>
    /// Private class to implement acquire/release of resources.
    /// </summary>
    /// <typeparam name="T">The type of resource.</typeparam>
    internal partial class ResourceHandle<T> : IResource<T>
    {
        /// <summary>
        /// Reference to the owner pool for returning the acquired resource.
        /// </summary>
        private readonly ResourcePoolImpl<T> pool;

        /// <summary>
        /// Reference to the pool-managed resource.
        /// </summary>
        private readonly T resource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceHandle{T}"/> class.
        /// </summary>
        /// <param name="pool">Owner pool.</param>
        /// <param name="resource">Pool-managed resource.</param>
        public ResourceHandle(ResourcePoolImpl<T> pool, T resource)
        {
            this.pool = pool;
            this.resource = resource;
        }

        /// <summary>
        /// Gets the instance of the managed resource.
        /// </summary>
        public T Instance
        {
            get
            {
                return this.resource;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the handle is invalid.
        /// </summary>
        public bool Invalid
        {
            get;
            set;
        }
    }

    /// <content>
    /// Implements IDisposable via <see cref="IResource{T}"/>.
    /// </content>
    internal partial class ResourceHandle<T>
    {
        /// <summary>
        /// Indicates whether the resource has been disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Finalizes an instance of the <see cref="ResourceHandle{T}"/> class.
        /// </summary>
        ~ResourceHandle()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Return the resource to the pool.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        /// <param name="disposing">true if the object is disposing; otherwise false if the object is finalizing.</param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.pool.ReleaseResource(this);

                GC.SuppressFinalize(this);
            }

            this.disposed = true;
        }
    }
}
