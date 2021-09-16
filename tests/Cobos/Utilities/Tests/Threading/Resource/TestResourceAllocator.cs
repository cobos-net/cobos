// ----------------------------------------------------------------------------
// <copyright file="TestResourceAllocator.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.Threading.Resource
{
    using System.Threading;
    using Cobos.Utilities.Threading.Resource;

    /// <summary>
    /// Allocates new instances for a thread pool.
    /// </summary>
    internal class TestResourceAllocator : IResourceAllocator<TestResource>
    {
        /// <summary>
        /// The id of the next resource.
        /// </summary>
        private static long resourceId;

        /// <summary>
        /// The work period in milliseconds for the resource.
        /// </summary>
        private readonly int workPeriodMs;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResourceAllocator"/> class.
        /// </summary>
        /// <param name="workPeriodMs">The work period in milliseconds for the resource.</param>
        public TestResourceAllocator(int workPeriodMs)
        {
            this.workPeriodMs = workPeriodMs;
        }

        /// <summary>
        /// Create a new resource.
        /// </summary>
        /// <returns>An object representing a new resource.</returns>
        public TestResource Create()
        {
            return new TestResource(Interlocked.Increment(ref resourceId), this.workPeriodMs);
        }
    }
}
