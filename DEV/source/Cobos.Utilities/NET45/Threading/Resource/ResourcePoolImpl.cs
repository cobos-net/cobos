// ----------------------------------------------------------------------------
// <copyright file="ResourcePoolImpl.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
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
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Implementation of a resource pool.  The class is marked as internal
    /// to hide the ReleaseResource method to force clients to use the 
    /// correct IResource.Dispose method.
    /// </summary>
    /// <typeparam name="T">The type of the resource to be managed.</typeparam>
    internal partial class ResourcePoolImpl<T> : IResourcePool<T>
    {
        /// <summary>
        /// The settings for this pool.
        /// </summary>
        private ResourcePoolSettings<T> settings;

        /// <summary>
        /// The current state of the pool.
        /// </summary>
        private ResourcePoolStatistics statistics;

        /// <summary>
        /// List of all resources managed by the pool.
        /// </summary>
        private List<T> resources;

        /// <summary>
        /// List of all available resources in the pool.
        /// </summary>
        private LinkedList<T> freeList;

        /// <summary>
        /// Synchronized access to the limited resources.
        /// </summary>
        private Semaphore semaphore;

        /// <summary>
        /// Control access to the resource and free list.
        /// </summary>
        private object lockResource = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePoolImpl{T}"/> class.
        /// </summary>
        /// <param name="settings">The settings for the resource pool.</param>
        public ResourcePoolImpl(ResourcePoolSettings<T> settings)
        {
            this.settings = settings;
            this.resources = new List<T>((int)this.settings.MaxPoolSize);
            this.freeList = new LinkedList<T>();
            this.statistics = new ResourcePoolStatistics();

            if (this.settings.MinPoolSize > 0)
            {
                lock (this.lockResource)
                {
                    for (int i = 0; i < this.settings.MinPoolSize; ++i)
                    {
                        this.AddNewResource();
                    }
                }
            }

            this.semaphore = new Semaphore((int)this.settings.MaxPoolSize, (int)this.settings.MaxPoolSize);
        }

        /// <summary>
        /// Gets information about the state of the pool.
        /// </summary>
        /// <returns>The current state of the pool.</returns>
        public ResourcePoolStatistics Statistics
        {
            get
            {
                return this.statistics;
            }
        }

        /// <summary>
        /// Get the next available resource.
        /// </summary>
        /// <returns>A handle to a managed resource.</returns>
        public IResource<T> AcquireResource()
        {
            lock (this.lockDisposed)
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("ResourcePool", "The pool is already disposed.");
                }

                if (this.disposeInProgress)
                {
                    return null;
                }
            }

            Interlocked.Increment(ref this.statistics.NumPendingRequests);

            this.semaphore.WaitOne();

            Interlocked.Decrement(ref this.statistics.NumPendingRequests);

            return this.GetFirstAvailableResource();
        }

        /// <summary>
        /// Called by the IResource.Dispose method to return a resource to the pool.
        /// Enable an invalid resource to be removed from the pool
        /// by setting IResource.Invalid = true before disposing of the IResource object.
        /// An example might be a database connection that is no longer valid.
        /// This requires that the resource is first acquired from the pool.
        /// </summary>
        /// <param name="resource">A resource acquired from this pool.</param>
        internal void ReleaseResource(IResource<T> resource)
        {
            T instance = resource.Instance;

            // check this belongs to us
            if (!this.resources.Contains(instance))
            {
                throw new Exception("Attempted to realease a resource that doesn't belong to this resource pool");
            }

            lock (this.lockResource)
            {
                if (resource.Invalid)
                {
                    // the client has marked this resource as invalid, remove from the pool
                    this.RemoveResource(instance);

                    // maintain the minimum number of resources
                    for (int i = this.resources.Count; i < this.settings.MinPoolSize; ++i)
                    {
                        this.AddNewResource();
                    }
                }
                else
                {
                    this.freeList.AddLast(instance);
                }

                this.statistics.NumAvailableResources = this.freeList.Count;
                this.statistics.SizePool = this.resources.Count;
            }

            bool readyToDispose = false;

            lock (this.lockDisposed)
            {
                if (this.disposeInProgress)
                {
                    // if this was the last job then signal that
                    // we can finally dispose the object
                    if (this.freeList.Count == this.resources.Count)
                    {
                        readyToDispose = true;
                    }
                }
            }

            this.semaphore.Release();

            if (readyToDispose)
            {
                this.waitingToDispose.Set();
            }
        }

        /// <summary>
        /// Manages acquisition of a resource.  If no resource is free a new resource is created up to MaxPoolSize.
        /// </summary>
        /// <returns>A handle to the resource.</returns>
        private ResourceHandle<T> GetFirstAvailableResource()
        {
            ResourceHandle<T> resource = null;

            lock (this.lockResource)
            {
                // Check that we have a resource ready in the free list...
                if (this.freeList.Count == 0)
                {
                    // ...if not then we can add a new resource up to this.settings.MaxPoolSize.
                    this.AddNewResource();
                }

                resource = new ResourceHandle<T>(this, this.freeList.First.Value);

                this.freeList.RemoveFirst();

                this.statistics.NumAvailableResources = this.freeList.Count;
            }

            return resource;
        }

        /// <summary>
        /// Add a new resource to the pool.
        /// </summary>
        /// <remarks>
        /// WARNING: this.lockResource must be acquired before calling this method.
        /// This method should not be called directly, resources should be 
        /// accessed via GetFirstAvailableResource.
        /// </remarks>
        private void AddNewResource()
        {
            // If the number of resources in use has reached the maximum AND the semaphore is signalled
            // then this indicates a problem with returning resources to the pool.
            // The idea is that the Semaphore is synchronised with the this.settings.MaxPoolSize member.
            // In principle, if resources are returned via the ReleaseResource method then this shouldn't happen.
            if (this.resources.Count >= this.settings.MaxPoolSize)
            {
                throw new Exception("An error occured in the resource pool, some resources have not been returned to the pool.");
            }

            T resource = this.settings.Allocator.Create();

            this.resources.Add(resource);
            this.freeList.AddLast(resource);

            this.statistics.SizePool = this.resources.Count;
            this.statistics.NumAvailableResources = this.freeList.Count;
        }

        /// <summary>
        /// Remove a resource from the pool.
        /// </summary>
        /// <param name="resource">The resource to remove.</param>
        /// <remarks>
        /// WARNING: this.lockResource must be acquired before calling this method.
        /// </remarks>
        private void RemoveResource(T resource)
        {
            this.resources.Remove(resource);

            if (this.freeList.Contains(resource))
            {
                this.freeList.Remove(resource);
            }
        }

        /// <summary>
        /// Helper class to manage a resource
        /// </summary>
        private class ResourceItem
        {
            /// <summary>
            /// Access the underlying instance
            /// </summary>
            public readonly T Instance;

            /// <summary>
            /// Epoch seconds when this item was created
            /// </summary>
            public readonly long Created;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResourceItem"/> class.
            /// </summary>
            /// <param name="instance">The underlying instance this item refers to.</param>
            public ResourceItem(T instance)
            {
                this.Instance = instance;
                this.Created = DateTime.Now.Ticks;
            }
        }
    }

    /// <summary>
    /// Implements IDisposable via <see cref="IResourcePool{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the resource to be managed.</typeparam>
    internal partial class ResourcePoolImpl<T>
    {
        /// <summary>
        /// Indicates whether the object is disposed.
        /// </summary>
        private volatile bool disposed = false;

        /// <summary>
        /// Indicates whether the object is currently disposing.
        /// </summary>
        private volatile bool disposeInProgress = false;

        /// <summary>
        /// Finish all pending jobs when trying to dispose.
        /// Don't accept any new jobs while we're disposing.
        /// </summary>
        private object lockDisposed = new object();

        /// <summary>
        /// Wait for a signal from worker threads to indicate that all
        /// pending jobs using the resources have completed.
        /// </summary>
        private AutoResetEvent waitingToDispose = null;

        /// <summary>
        /// Finalizes an instance of the <see cref="ResourcePoolImpl{T}"/> class.
        /// </summary>
        ~ResourcePoolImpl()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Free all resources and stop the pool.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose the current instance.
        /// </summary>
        /// <param name="disposing">true if the object is disposing; otherwise false if the object is finalizing.</param>
        private void Dispose(bool disposing)
        {
            lock (this.lockDisposed)
            {
                if (this.disposed)
                {
                    return;
                }

                if (this.disposeInProgress)
                {
                    return;
                }
                
                this.disposeInProgress = true;
            }

            if (disposing)
            {
                this.DiposeImmediatelyOrWait();

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// If no jobs are pending then clean up all resources.
        /// Otherwise, wait until all jobs are complete and then clean up.
        /// </summary>
        private void DiposeImmediatelyOrWait()
        {
            lock (this.lockResource)
            {
                if (this.freeList.Count == this.resources.Count)
                {
                    // nothing outstanding, clear all resources
                    this.DisposeAllResources();
                    return;
                }

                this.waitingToDispose = new AutoResetEvent(false);
            }

            // set a timeout, if some resources haven't been properly
            // released then we don't want to lock up.
            this.waitingToDispose.WaitOne(10000);

            this.DisposeAllResources();
        }

        /// <summary>
        /// Clean up all resources managed by the pool.
        /// </summary>
        private void DisposeAllResources()
        {
            lock (this.lockDisposed)
            {
                foreach (T resource in this.resources)
                {
                    IDisposable dispose = resource as IDisposable;

                    if (dispose != null)
                    {
                        dispose.Dispose();
                    }
                }

                this.resources.Clear();
                this.freeList.Clear();
                this.statistics.SizePool = 0;
                this.statistics.NumAvailableResources = 0;

                this.disposed = true;
                this.disposeInProgress = false;
            }
        }
    }
}
