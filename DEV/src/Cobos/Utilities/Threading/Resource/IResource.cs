// ----------------------------------------------------------------------------
// <copyright file="IResource.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Resource
{
    using System;

    /// <summary>
    /// Wraps a resource managed by a thread pool.  The thread pool
    /// returns the wrapped resource for use by the current thread.
    /// Use the Dispose method to return the resource to the pool.
    /// </summary>
    /// <typeparam name="T">The type of resource.</typeparam>
    public interface IResource<T> : IDisposable
    {
        /// <summary>
        /// Gets a reference to the underlying resource.
        /// </summary>
        T Instance
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is invalid.
        /// </summary>
        /// <remarks>
        /// Mark the resource as invalid before returning it to the
        /// pool so that it may be removed.  An example of a
        /// resource that might be invalid is a database connection
        /// that has been closed after not being used for a while.
        /// </remarks>
        bool Invalid
        {
            get;
            set;
        }
    }
}
