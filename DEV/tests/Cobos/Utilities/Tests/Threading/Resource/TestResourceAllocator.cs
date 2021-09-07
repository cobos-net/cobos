// ----------------------------------------------------------------------------
// <copyright file="TestResourceAllocator.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Tests.Threading.Resource
{
    using System;
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
