using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Intergraph.AsiaPac.Utilities.Threading.Resource
{
	/// <summary>
	/// Statistics about the pool for monitoring and diagnostics
	/// </summary>
	public struct ResourcePoolStatistics
	{
		/// <summary>
		/// The number of pending requests for resources
		/// </summary>
		public long NumPendingRequests;

		/// <summary>
		/// The number of free resources available
		/// </summary>
		public long NumAvailableResources;

		/// <summary>
		/// The current size of the pool
		/// </summary>
		public long SizePool
		{
			get
			{
				return _sizePool;
			}
			set
			{
				_sizePool = value;

				if ( _sizePool > MaxSizePool )
				{
					MaxSizePool = _sizePool;
				}
			}
		}

		long _sizePool;

		/// <summary>
		/// The maxiumn size that the pool has reached
		/// </summary>
		public long MaxSizePool
		{
			get;
			private set;
		}
	}
}
