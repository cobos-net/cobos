using System;

namespace Intergraph.AsiaPac.Utilities.Threading.Resource
{
	/// <summary>
	/// Wraps a resource managed by a thread pool.  The thread pool 
	/// returns the wrapped resource for use by the currnet thread.
	/// Use the Dispose method to return the resource to the pool.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IResource<T> : IDisposable
	{
		/// <summary>
		/// Obtain a reference to the underlying resource
		/// </summary>
		T Instance
		{
			get;
		}

		/// <summary>
		/// Mark the resouce as invalid before returning it to the 
		/// pool so that it may be removed.  An example of a 
		/// resource that might be invalid is a database connection
		/// that has been closed after not being used for a while.
		/// </summary>
		bool Invalid
		{
			get;
			set;
		}
	}
}
