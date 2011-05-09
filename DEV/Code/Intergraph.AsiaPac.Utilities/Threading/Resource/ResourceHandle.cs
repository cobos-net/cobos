using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Utilities.Threading.Resource
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
		public ResourceHandle( ResourcePoolImpl<T> pool, T resource )
		{
			_pool = pool;
			_resource = resource;
		}

		~ResourceHandle()
		{
			Dispose( false );
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
			Dispose( true );
		}

		void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				return;
			}

			if ( disposing )
			{
				_pool.ReleaseResource( this );

				GC.SuppressFinalize( this );
			}

			_disposed = true;
		}

		bool _disposed = false;

		#endregion
	}
}
