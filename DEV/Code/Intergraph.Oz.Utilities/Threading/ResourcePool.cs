using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Intergraph.Oz.Utilities.Threading
{
	public interface IResourcePool<T> : IDisposable
	{
		/// <summary>
		/// Query availability of the resources in the pool.
		/// </summary>
		int NumAvailable
		{
			get;
		}

		/// <summary>
		/// Query the pending workload waiting for resources.
		/// </summary>
		int NumPending
		{
			get;
		}

		/// <summary>
		/// Acquire the next available resource from the pool.
		/// </summary>
		/// <returns></returns>
		IResource<T> AcquireResource();
	}

	/// <summary>
	/// Wraps a resource managed by a thread pool.  The thread pool 
	/// returns the wrapped resource for use by the currnet thread.
	/// Use the Dispose method to return the resource to the pool.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IResource<T> : IDisposable
	{
		T Instance
		{
			get;
		}
	}

	/// <summary>
	/// Class factory for resource pools
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class ResourcePool<T>
	{
		public static IResourcePool<T> Create( List<T> resources )
		{
			return new ResourcePoolImpl<T>( resources );
		}
	}

	/// <summary>
	/// Implementation of a resource pool.  The class is marked as internal
	/// to hide the ReleaseResource method to force clients to use the 
	/// correct IResource.Dispose method.
	/// </summary>
	/// <typeparam name="T">The type of the resource to be managed.</typeparam>
	public class ResourcePoolImpl<T> : IResourcePool<T>
	{
		/// <summary>
		/// Private class to implement acquire/release of resources.
		/// </summary>
		class ResourceHandle : IResource<T>
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
			public ResourceHandle( ResourcePoolImpl<T> pool, T resource )
			{
				_pool = pool;
				_resource = resource;
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

			/// <summary>
			/// Return the resource to the pool.
			/// </summary>
			public void Dispose()
			{
				lock ( this )
				{
					if ( _disposed )
					{
						throw new ObjectDisposedException( "ResourcePool", "The pool is already disposed." );
					}
					_disposed = true;
				}

				_pool.ReleaseResource( this );

				_disposed = true;
			}

			bool _disposed = false;
		}

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
		Semaphore _resourceAllocator = null;

		/// <summary>
		/// Statistic to show the number of waiting threads.
		/// </summary>
		int _numPending = 0;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="resources">Resources to be managed.  Class takes ownership.</param>
		public ResourcePoolImpl( List<T> resources )
		{
			_resources = new List<T>( resources );
			_freeList = new LinkedList<T>();

			foreach ( T r in _resources )
			{
				_freeList.AddLast( r );
			}

			_resourceAllocator = new Semaphore( _resources.Count, _resources.Count );
		}

		/// <summary>
		/// Number of waiting requests.
		/// </summary>
		public int NumPending
		{
			get
			{
				return _numPending;
			}
		}

		/// <summary>
		/// Number of resources available for a request.
		/// </summary>
		public int NumAvailable
		{
			get
			{
				lock ( _freeList )
				{
					return _freeList.Count;
				}
			}
		}

		/// <summary>
		/// Get the next available resource.
		/// </summary>
		/// <returns>A handle to a managed resource.</returns>
		public IResource<T> AcquireResource()
		{
			lock ( this )
			{
				if ( _disposed )
				{
					throw new ObjectDisposedException( "ResourcePool", "The pool is already disposed." );
				}
				if ( _disposing )
				{
					return null;
				}
			}

			IResource<T> resource = null;

			Interlocked.Increment( ref _numPending );

			_resourceAllocator.WaitOne();

			Interlocked.Decrement( ref _numPending );

			lock ( _freeList )
			{
				if ( _freeList.Count == 0 )
				{
					throw new IntergraphError( "An error occured in the resource pool, some resources have not been returned to the pool." );
				}

				resource = new ResourceHandle( this, _freeList.First.Value );
				_freeList.RemoveFirst();
			}

			return resource;
		}

		/// <summary>
		/// Called by the IResource.Dispose method to return a resource to the pool.
		/// </summary>
		/// <param name="resource"></param>
		public void ReleaseResource( IResource<T> resource )
		{
			T instance = resource.Instance;

			// check this belongs to us
			bool found = false;

			foreach ( T r in _resources )
			{
				if ( Object.ReferenceEquals( instance, r ) )
				{
					found = true;
					break;
				}
			}

			if ( !found )
			{
				throw new IntergraphError( "Attempted to realease a resource that doesn't belong to this resource pool" );
			}

			lock ( _freeList )
			{
				_freeList.AddLast( instance );
			}

			bool readyToDispose = false;

			lock ( this )
			{
				if ( _disposing )
				{
					// if this was the last job then signal that
					// we can finally dispose the object
					if ( _freeList.Count == _resources.Count )
					{
						readyToDispose = true;
					}
				}
			}

			int count = _resourceAllocator.Release();

			if ( readyToDispose )
			{
				_waitingToDispose.Set();
			}
		}

		/// <summary>
		/// Free all resources and stop the pool.
		/// </summary>
		public void Dispose()
		{
			lock ( this )
			{
				if ( _disposed )
				{
					throw new ObjectDisposedException( "ResourcePool", "The pool is already disposed." );
				}
				if ( _disposing )
				{
					return;
				}
				_disposing = true;
			}

			lock ( _freeList )
			{
				if ( _freeList.Count == _resources.Count )
				{
					// nothing outstanding, clear all resources
					DisposeAllResources();
					return;
				}

				_waitingToDispose = new AutoResetEvent( false );
			}

			_waitingToDispose.WaitOne();

			DisposeAllResources();
		}

		void DisposeAllResources()
		{
			lock ( this )
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
				_disposing = false;
			}
		}

		/// <summary>
		/// Private members for managing the disposal of the pool.
		/// </summary>
		bool _disposing = false;
		bool _disposed = false;

		/// <summary>
		/// Wait for a signal from worker threads to indicate that all
		/// pending jobs using the resources have completed.
		/// </summary>
		AutoResetEvent _waitingToDispose = null;
	}
}
