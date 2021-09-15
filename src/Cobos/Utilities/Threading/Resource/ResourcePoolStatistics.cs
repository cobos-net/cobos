// ----------------------------------------------------------------------------
// <copyright file="ResourcePoolStatistics.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Resource
{
    /// <summary>
    /// Statistics about the pool for monitoring and diagnostics.
    /// </summary>
    public struct ResourcePoolStatistics
    {
        /// <summary>
        /// Gets or sets the number of pending requests for resources.
        /// </summary>
        public long NumPendingRequests;

        /// <summary>
        /// Gets or sets the number of free resources available.
        /// </summary>
        public long NumAvailableResources;

        /// <summary>
        /// The current size of the pool.
        /// </summary>
        private long sizePool;

        /// <summary>
        /// Gets or sets the current size of the pool.
        /// </summary>
        public long SizePool
        {
            get
            {
                return this.sizePool;
            }

            set
            {
                this.sizePool = value;

                if (this.sizePool > this.MaxSizePool)
                {
                    this.MaxSizePool = this.sizePool;
                }
            }
        }

        /// <summary>
        /// Gets The maximum size that the pool has reached.
        /// </summary>
        public long MaxSizePool
        {
            get;
            private set;
        }
    }
}
