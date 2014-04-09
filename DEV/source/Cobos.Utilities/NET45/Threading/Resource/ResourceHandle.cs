// ----------------------------------------------------------------------------
// <copyright file="ResourceHandle.cs" company="Cobos SDK">
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
        private ResourcePoolImpl<T> pool;

        /// <summary>
        /// Reference to the pool-managed resource.
        /// </summary>
        private T resource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceHandle{T}"/> class.
        /// </summary>
        /// <param name="pool">Owner pool</param>
        /// <param name="resource">Pool-managed resource</param>
        public ResourceHandle(ResourcePoolImpl<T> pool, T resource)
        {
            this.pool = pool;
            this.resource = resource;
        }

        /// <summary>
        /// Gets the instance of the managed resource
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

    /// <summary>
    /// Implements IDisposable via <see cref="IResource{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of resource.</typeparam>
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
