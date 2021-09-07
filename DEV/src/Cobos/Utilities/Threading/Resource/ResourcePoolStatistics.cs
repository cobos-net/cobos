﻿// ----------------------------------------------------------------------------
// <copyright file="ResourcePoolStatistics.cs" company="Cobos SDK">
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

    /// <summary>
    /// Statistics about the pool for monitoring and diagnostics
    /// </summary>
    public struct ResourcePoolStatistics
    {
        /// <summary>
        /// Gets or sets the number of pending requests for resources
        /// </summary>
        public long NumPendingRequests;

        /// <summary>
        /// Gets or sets the number of free resources available
        /// </summary>
        public long NumAvailableResources;

        /// <summary>
        /// The current size of the pool.
        /// </summary>
        private long sizePool;

        /// <summary>
        /// Gets or sets the current size of the pool
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