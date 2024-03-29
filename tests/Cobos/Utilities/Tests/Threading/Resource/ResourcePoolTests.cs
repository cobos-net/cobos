﻿// ----------------------------------------------------------------------------
// <copyright file="ResourcePoolTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.Threading.Resource
{
    using System;
    using System.Threading;
    using Cobos.Utilities.Threading.Resource;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="IResourcePool{T}"/> class.
    /// </summary>
    [TestClass]
    public class ResourcePoolTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create 100 jobs and add them to the thread pool for processing
        /// 2. Wait for all jobs to complete
        /// 3. Check the pool cleaned up OK.
        /// </summary>
        [TestMethod]
        public void Threading_pool_can_process_jobs()
        {
            JobStateInfo jobState = new JobStateInfo(100, 300, 0, 5);

            jobState.WaitForAllJobs();

            ResourcePoolStatistics stats = jobState.Pool.Statistics;

            Assert.AreEqual(0, stats.NumPendingRequests);

            Console.WriteLine("Max Size: " + stats.MaxSizePool);

            jobState.Pool.Dispose();

            stats = jobState.Pool.Statistics;

            Assert.AreEqual(0, stats.NumPendingRequests);
            Assert.AreEqual(0, stats.NumAvailableResources);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Dispose a pool and then try to acquire a resource.
        /// </summary>
        [TestMethod]
        public void Cannot_acquire_a_resource_from_a_disposed_pool()
        {
            JobStateInfo jobState = new JobStateInfo(100, 300, 0, 5);

            jobState.Pool.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => jobState.Pool.AcquireResource());
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a pool with a min size of 0.
        /// 2. Acquire one resource and check the number of available and the number in the pool.
        /// 3. Acquire another resource and check the pool state.
        /// 4. Release one resource and check the pool state.
        /// 5. Release the remaining resource and check the pool state.
        /// </summary>
        [TestMethod]
        public void Resources_are_correctly_managed()
        {
            JobStateInfo jobState = new JobStateInfo(2, 0, 0, 5);

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(0, jobState.Pool.Statistics.SizePool);

            // acquire two resources
            IResource<TestResource> r1 = jobState.Pool.AcquireResource();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(1, jobState.Pool.Statistics.SizePool);

            IResource<TestResource> r2 = jobState.Pool.AcquireResource();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            r1.Dispose();

            Assert.AreEqual(1, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            r2.Dispose();

            Assert.AreEqual(2, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            jobState.Pool.Dispose();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(0, jobState.Pool.Statistics.SizePool);
            Assert.AreEqual(2, jobState.Pool.Statistics.MaxSizePool);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a pool with a min size of 0.
        /// 2. Acquire one resource and check the number of available and the number in the pool.
        /// 3. Acquire another resource and check the pool state.
        /// 4. Release one invalid resource and check the pool state.
        /// 5. Release the remaining invalid resource and check the pool state.
        /// </summary>
        [TestMethod]
        public void Minimum_pool_size_of_0_is_maintained()
        {
            JobStateInfo jobState = new JobStateInfo(2, 0, 0, 5);

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(0, jobState.Pool.Statistics.SizePool);

            // acquire two resources
            IResource<TestResource> r1 = jobState.Pool.AcquireResource();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(1, jobState.Pool.Statistics.SizePool);

            IResource<TestResource> r2 = jobState.Pool.AcquireResource();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            r1.Invalid = true;
            r1.Dispose();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(1, jobState.Pool.Statistics.SizePool);

            r2.Invalid = true;
            r2.Dispose();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(0, jobState.Pool.Statistics.SizePool);

            jobState.Pool.Dispose();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(0, jobState.Pool.Statistics.SizePool);
            Assert.AreEqual(2, jobState.Pool.Statistics.MaxSizePool);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a pool with a min size of 0.
        /// 2. Acquire one resource and check the number of available and the number in the pool.
        /// 3. Acquire another resource and check the pool state.
        /// 4. Acquire another resource and check the pool state.
        /// 5. Release one invalid resource and check the pool state.
        /// 6. Release the remaining invalid resources and check the pool state.
        /// </summary>
        [TestMethod]
        public void Minimum_pool_size_of_2_is_maintained()
        {
            JobStateInfo jobState = new JobStateInfo(2, 0, 2, 5);

            Assert.AreEqual(2, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            // acquire two resources
            IResource<TestResource> r1 = jobState.Pool.AcquireResource();

            Assert.AreEqual(1, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            IResource<TestResource> r2 = jobState.Pool.AcquireResource();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            IResource<TestResource> r3 = jobState.Pool.AcquireResource();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(3, jobState.Pool.Statistics.SizePool);

            r1.Invalid = true;
            r1.Dispose();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            r2.Invalid = true;
            r2.Dispose();

            Assert.AreEqual(1, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            r3.Invalid = true;
            r3.Dispose();

            Assert.AreEqual(2, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(2, jobState.Pool.Statistics.SizePool);

            jobState.Pool.Dispose();

            Assert.AreEqual(0, jobState.Pool.Statistics.NumAvailableResources);
            Assert.AreEqual(0, jobState.Pool.Statistics.SizePool);
            Assert.AreEqual(3, jobState.Pool.Statistics.MaxSizePool);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a pool with a max size of 2.
        /// 2. Acquire one resource and check the number of available and the number in the pool.
        /// 3. Acquire another resource and check the pool state.
        /// 4. Acquire another resource and check the pool state.
        /// 5. Release one invalid resource and check the pool state.
        /// 6. Release the remaining invalid resources and check the pool state.
        /// </summary>
        [TestMethod]
        public void Maximum_pool_size_is_not_breached()
        {
            JobStateInfo jobState = new JobStateInfo(10, 300, 2, 2);

            jobState.WaitForAllJobs();

            ResourcePoolStatistics stats = jobState.Pool.Statistics;

            Assert.AreEqual(2, stats.MaxSizePool);
        }

        /// <summary>
        /// Thread pool delegate to do work on test resources.
        /// </summary>
        /// <param name="stateInfo">The job state.</param>
        private static void ThreadProc(object stateInfo)
        {
            JobStateInfo jobState = (JobStateInfo)stateInfo;

            ResourcePoolStatistics stats = jobState.Pool.Statistics;

            Console.WriteLine("Pool Status: {0} Pending {1} Available", stats.NumPendingRequests, stats.NumAvailableResources);

            using (IResource<TestResource> resource = jobState.Pool.AcquireResource())
            {
                // signal the jobs are complete before doing the work.
                // when the resource pool is disposed, it will force
                // it to wait for this job to complete before terminating.
                jobState.JobDone();

                resource.Instance.DoWork();
            }
        }

        /// <summary>
        /// Helper class to manage state information for a thread.
        /// </summary>
        private class JobStateInfo
        {
            /// <summary>
            /// Event set when all jobs are complete.
            /// </summary>
            private readonly AutoResetEvent allJobsCompleted = new AutoResetEvent(false);

            /// <summary>
            /// The maximum number of jobs to run.
            /// </summary>
            private readonly uint maxJobs;

            /// <summary>
            /// The number of jobs done.
            /// </summary>
            private int jobsDone;

            /// <summary>
            /// Initializes a new instance of the <see cref="JobStateInfo"/> class.
            /// </summary>
            /// <param name="maxJobs">The maximum number of jobs.</param>
            /// <param name="jobPeriod">The time period for a job.</param>
            /// <param name="minPoolSize">The initial size of the resource pool.</param>
            /// <param name="maxPoolSize">The maximum size that the resource pool is allowed to increase to.</param>
            public JobStateInfo(uint maxJobs, int jobPeriod, uint minPoolSize, uint maxPoolSize)
            {
                this.maxJobs = maxJobs;

                var settings = new ResourcePoolSettings<TestResource>
                {
                    Allocator = new TestResourceAllocator(jobPeriod),
                    MaxPoolSize = maxPoolSize,
                    MinPoolSize = minPoolSize,
                };

                this.Pool = ResourcePool<TestResource>.Create(settings);

                this.allJobsCompleted.Reset();
            }

            /// <summary>
            /// Gets the resource pool for the job state.
            /// </summary>
            public IResourcePool<TestResource> Pool { get; }

            /// <summary>
            /// Called to indicate that a job has completed.
            /// </summary>
            public void JobDone()
            {
                Interlocked.Increment(ref this.jobsDone);

                if (this.jobsDone >= this.maxJobs)
                {
                    this.allJobsCompleted.Set();
                }
            }

            /// <summary>
            /// Wait for all jobs to complete.
            /// </summary>
            public void WaitForAllJobs()
            {
                for (int i = 1; i <= this.maxJobs; ++i)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), this);
                    Thread.Sleep(10);
                }

                this.allJobsCompleted.WaitOne();
            }
        }
    }
}
