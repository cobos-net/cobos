// ============================================================================
// Filename: ResourcePoolImpl.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cobos.Utilities.Extensions;

namespace Cobos.Utilities.Threading.Resource
{
	/// <summary>
	/// Implementation of a resource pool.  The class is marked as internal
	/// to hide the ReleaseResource method to force clients to use the 
	/// correct IResource.Dispose method.
	/// </summary>
	/// <typeparam name="T">The type of the resource to be managed.</typeparam>
	internal class ResourcePoolImpl<T> : IResourcePool<T>
	{
		/// <summary>
		/// Helper class to manage a resource
		/// </summary>
		class ResourceItem
		{
			/// <summary>
			/// Access the underlying instance
			/// </summary>
			public readonly T Instance;

			/// <summary>
			/// Epoch seconds when this item was created
			/// </summary>
			public readonly long Created;

			public ResourceItem( T instance )
			{
				Instance = instance;
				Created = DateTime.Now.ToEpochSeconds();
			}
		}

		/// <summary>
		/// The settings for this pool.
		/// </summary>
		ResourcePoolSettings<T> _settings;

		/// <summary>
		/// The current state of the pool.
		/// </summary>
		ResourcePoolStatistics _statistics = new ResourcePoolStatistics();

		/// <summary>
		/// List of all resources managed by the pool.
		/// </summary>
		List<T> _resources;

		/// <summary>
		/// List of all available resources in the pool.
		/// </summary>
		LinkedList<T> _freeList;

		/// <summary>
		/// Synchronised access to the limited resources.
		/// </summary>
		Semaphore _semaphore;

		/// <summary>
		/// Control access to the resource and free list.
		/// </summary>
		object _lockResource = new object();

		/// <summary>
		/// Finish all pending jobs when trying to dispose.
		/// Don't accept any new jobs while we're disposing.
		/// </summary>
		object _lockDisposed = new object();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="minPoolSize"></param>
		/// <param name="maxPoolSize">The maximum size of the pool.</param>
		/// <param name="allocator"></param>
		public ResourcePoolImpl( ResourcePoolSettings<T> settings )
		{
			_settings = settings;

			_resources = new List<T>( (int)_settings.MaxPoolSize );
			_freeList = new LinkedList<T>();

			if ( _settings.MinPoolSize > 0 )
			{
				lock ( _lockResource )
				{
					for ( int i = 0; i < _settings.MinPoolSize; ++i )
					{
						AddNewResource();
					}
				}
			}

			_semaphore = new Semaphore( (int)_settings.MaxPoolSize, (int)_settings.MaxPoolSize );
		}

		#region IResourcePool implementation

		/// <summary>
		/// Get the next available resource.
		/// </summary>
		/// <returns>A handle to a managed resource.</returns>
		public IResource<T> AcquireResource()
		{
			lock ( _lockDisposed )
			{
				if ( _disposed )
				{
					throw new ObjectDisposedException( "ResourcePool", "The pool is already disposed." );
				}
				if ( _disposeInProgress )
				{
					return null;
				}
			}

			Interlocked.Increment( ref _statistics.NumPendingRequests );

			_semaphore.WaitOne();

			Interlocked.Decrement( ref _statistics.NumPendingRequests );

			return GetFirstAvailableResource();
		}

		/// <summary>
		/// Called by the IResource.Dispose method to return a resource to the pool.
		/// Enable an invalid resource to be removed from the pool
		/// by setting IResource.Invalid = true before disposing of the IResource object.
		/// An example might be a database connection that is no longer valid.
		/// This requires that the resource is first acquired from the pool.
		/// </summary>
		/// <param name="resource">A resource acquired from this pool.</param>
		internal void ReleaseResource( IResource<T> resource )
		{
			T instance = resource.Instance;

			// check this belongs to us
			if ( !_resources.Contains( instance ) )
			{
				throw new Exception( "Attempted to realease a resource that doesn't belong to this resource pool" );
			}

			lock ( _lockResource )
			{
				if ( resource.Invalid )
				{
					// the client has marked this resource as invalid, remove from the pool
					RemoveResource( instance );

					// maintain the minimum number of resources
					for ( int i = _resources.Count; i < _settings.MinPoolSize; ++i )
					{
						AddNewResource();
					}
				}
				else
				{
					_freeList.AddLast( instance );
				}
				
				_statistics.NumAvailableResources = _freeList.Count;
				_statistics.SizePool = _resources.Count;
			}

			bool readyToDispose = false;

			lock ( _lockDisposed )
			{
				if ( _disposeInProgress )
				{
					// if this was the last job then signal that
					// we can finally dispose the object
					if ( _freeList.Count == _resources.Count )
					{
						readyToDispose = true;
					}
				}
			}

			_semaphore.Release();

			if ( readyToDispose )
			{
				_waitingToDispose.Set();
			}
		}


		/// <summary>
		/// Query information about the state of the pool.
		/// </summary>
		/// <returns></returns>
		public ResourcePoolStatistics Statistics
		{
			get
			{
				return _statistics;
			}
		}

		#endregion

		#region Resource acquisition control methods

		/// <summary>
		/// Manages acquisition of a resource.  If no 
		/// </summary>
		/// <returns></returns>
		ResourceHandle<T> GetFirstAvailableResource()
		{
			ResourceHandle<T> resource = null;

			lock ( _lockResource )
			{
				// Check that we have a resource ready in the free list...
				if ( _freeList.Count == 0 )
				{
					// ...if not then we can add a new resource up to _settings.MaxPoolSize.
					AddNewResource();
				}

				resource = new ResourceHandle<T>( this, _freeList.First.Value );

				_freeList.RemoveFirst();

				_statistics.NumAvailableResources = _freeList.Count;
			}

			return resource;
		}
	
		/// <summary>
		/// WARNING: _lockResource must be aquired before calling this method.
		/// This method should not be called directly, resources should be 
		/// accessed via GetFirstAvailableResource.
		/// </summary>
		void AddNewResource()
		{
			// If the number of resources in use has reached the maximum AND the semaphore is signalled
			// then this indicates a problem with returning resources to the pool.
			// The idea is that the Semaphore is synchronised with the _settings.MaxPoolSize member.
			// In principle, if resources are returned via the ReleaseResource method then this shouldn't happen.
			if ( _resources.Count >= _settings.MaxPoolSize )
			{
				throw new Exception( "An error occured in the resource pool, some resources have not been returned to the pool." );
			}

			T resource = _settings.Allocator.Create();
			
			_resources.Add( resource );
			_freeList.AddLast( resource );

			_statistics.SizePool = _resources.Count;
			_statistics.NumAvailableResources = _freeList.Count;
		}

		/// <summary>
		/// WARNING: _lockResource must be aquired before calling this method.
		/// </summary>
		/// <param name="resource"></param>
		void RemoveResource( T resource )
		{
			_resources.Remove( resource );

			if ( _freeList.Contains( resource ) )
			{
				_freeList.Remove( resource );
			}
		}

		#endregion



		~ResourcePoolImpl()
		{
			Dispose( false );
		}

		/// <summary>
		/// Free all resources and stop the pool.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
		}

		public void Dispose( bool disposing )
		{
			lock ( _lockDisposed )
			{
				if ( _disposed )
				{
					return;
				}
				if ( _disposeInProgress )
				{
					return;
				}
				_disposeInProgress = true;
			}

			if ( disposing )
			{
				DiposeImmediatelyOrWait();

				GC.SuppressFinalize( this );
			}
		}

		/// <summary>
		/// If no jobs are pending then clean up all resources.
		/// Otherwise, wait until all jobs are complete and then clean up.
		/// </summary>
		void DiposeImmediatelyOrWait()
		{
			lock ( _lockResource )
			{
				if ( _freeList.Count == _resources.Count )
				{
					// nothing outstanding, clear all resources
					DisposeAllResources();
					return;
				}

				_waitingToDispose = new AutoResetEvent( false );
			}

			// set a timeout, if some resources haven't been properly
			// released then we don't want to lock up.
			_waitingToDispose.WaitOne( 10000 );

			DisposeAllResources();
		}

		/// <summary>
		/// Clean up all resources managed by the pool.
		/// </summary>
		void DisposeAllResources()
		{
			lock ( _lockDisposed )
			{
				foreach ( T r in _resources )
				{
					IDisposable disp = r as IDisposable;

					if ( disp != null )
					{
						disp.Dispose();
					}
				}

				_resources.Clear();
				_freeList.Clear();

				_disposed = true;
				_disposeInProgress = false;

				_statistics.SizePool = 0;
				_statistics.NumAvailableResources = 0;
			}
		}

		/// <summary>
		/// Private members for managing the disposal of the pool.
		/// </summary>
		bool _disposed = false;
		bool _disposeInProgress = false;

		/// <summary>
		/// Wait for a signal from worker threads to indicate that all
		/// pending jobs using the resources have completed.
		/// </summary>
		AutoResetEvent _waitingToDispose = null;
	}
}
