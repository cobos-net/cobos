using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Utilities.Threading.Resource
{
	public interface IResourcePool<T> : IDisposable
	{
		/// <summary>
		/// Acquire the next available resource from the pool.
		/// Resources are returned to the pool by calling the
		/// IResource.Dispose method.
		/// </summary>
		/// <returns></returns>
		IResource<T> AcquireResource();

		/// <summary>
		/// Query information about the state of the pool.
		/// </summary>
		/// <returns></returns>
		ResourcePoolStatistics Statistics
		{
			get;
		}
	}
}
