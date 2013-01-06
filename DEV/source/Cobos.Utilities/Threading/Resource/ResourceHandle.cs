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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Utilities.Threading.Resource
{
    /// <summary>
    /// Private class to implement acquire/release of resources.
    /// </summary>
    class ResourceHandle<T> : IResource<T>
    {
        /// <summary>
        /// Reference to the owner pool for returning the acquired resource.
        /// </summary>
        ResourcePoolImpl<T> _pool;

        /// <summary>
        /// Reference to the pool-managed resource.
        /// </summary>
        T _resource;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pool">Owner pool</param>
        /// <param name="resource">Pool-managed resource</param>
        public ResourceHandle(ResourcePoolImpl<T> pool, T resource)
        {
            _pool = pool;
            _resource = resource;
        }

        ~ResourceHandle()
        {
            Dispose(false);
        }

        /// <summary>
        /// Get the instance of the managed resource
        /// </summary>
        public T Instance
        {
            get
            {
                return _resource;
            }
        }

        public bool Invalid
        {
            get;
            set;
        }

        #region IDisposable implementation

        /// <summary>
        /// Return the resource to the pool.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _pool.ReleaseResource(this);

                GC.SuppressFinalize(this);
            }

            _disposed = true;
        }

        bool _disposed = false;

        #endregion
    }
}
