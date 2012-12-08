using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Cobos.Utilities.Threading.Resource;

namespace Cobos.Utilities.Tests.Threading.Resource
{
	[TestFixture]
	public class TestResourcePool
	{
		class JobStateInfo
		{
			public readonly IResourcePool<TestResource> Pool;
			
			readonly AutoResetEvent _allJobsCompleted = new AutoResetEvent( false );
			
			readonly uint _maxJobs;
			
			int _jobsDone;

			public JobStateInfo( uint maxJobs, int jobPeriod, uint minPoolSize, uint maxPoolSize )
			{
				_maxJobs = maxJobs;

				ResourcePoolSettings<TestResource> settings = new ResourcePoolSettings<TestResource>();

				settings.Allocator = new TestResourceAllocator( jobPeriod );
				settings.MaxPoolSize = maxPoolSize;
				settings.MinPoolSize = minPoolSize;

				Pool = ResourcePool<TestResource>.Create( settings );

				_allJobsCompleted.Reset();
			}

			public void JobDone()
			{
				Interlocked.Increment( ref _jobsDone );

				if ( _jobsDone >= _maxJobs )
				{
					_allJobsCompleted.Set();
				}
			}

			public void WaitForAllJobs()
			{
				for ( int i = 1; i <= _maxJobs; ++i )
				{
					ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadProc ), this );
					Thread.Sleep( 10 );
				}

				_allJobsCompleted.WaitOne();
			}
		}

		/// <summary>
		/// Thread pool delegate to do work on test resources
		/// </summary>
		/// <param name="stateInfo"></param>
		static void ThreadProc( Object stateInfo )
		{
			JobStateInfo jobState = (JobStateInfo)stateInfo;

			ResourcePoolStatistics stats = jobState.Pool.Statistics;

			Console.WriteLine( "Pool Status: {0} Pending {1} Available", stats.NumPendingRequests, stats.NumAvailableResources );

			using ( IResource<TestResource> resource = jobState.Pool.AcquireResource() )
			{
				// signal the jobs are complete before doing the work.
				// when the resource pool is disposed, it will force
				// it to wait for this job to complete before terminating.
				jobState.JobDone();

				resource.Instance.DoWork();
			}
		}

		[TestCase]
		void Threading_pool_can_process_jobs()
		{
			// Strategy:
			//	---------
			// 1. Create 100 jobs and add them to the thread pool for processing
			// 2. Wait for all jobs to complete
			// 3. Check the pool cleaned up OK.

			JobStateInfo jobState = new JobStateInfo( 100, 300, 0, 5 );

			jobState.WaitForAllJobs();

			ResourcePoolStatistics stats = jobState.Pool.Statistics;

			Assert.AreEqual( 0, stats.NumPendingRequests );
		
			Console.WriteLine( "Max Size: " + stats.MaxSizePool );

			jobState.Pool.Dispose();

			stats = jobState.Pool.Statistics;

			Assert.AreEqual( 0, stats.NumPendingRequests );
			Assert.AreEqual( 0, stats.NumAvailableResources );
		}

		[TestCase]
		void Cannot_acquire_a_resource_from_a_disposed_pool()
		{
			// Strategy:
			//	---------
			// 1. Dispose a pool and then try to acquire a resource.

			JobStateInfo jobState = new JobStateInfo( 100, 300, 0, 5 );

			jobState.Pool.Dispose();

			Assert.Throws<ObjectDisposedException>( delegate { jobState.Pool.AcquireResource(); } );
		}

		[TestCase]
		void Resources_are_correctly_managed()
		{
			// Strategy:
			//	---------
			// 1. Create a pool with a min size of 0.
			// 2. Acquire one resource and check the number of available and the number in the pool.
			// 3. Acquire another resource and check the pool state.
			// 4. Release one resource and check the pool state.
			// 5. Release the remaining resource and check the pool state.

			JobStateInfo jobState = new JobStateInfo( 2, 0, 0, 5 );

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 0, jobState.Pool.Statistics.SizePool );

			// acquire two resources
			IResource<TestResource> r1 = jobState.Pool.AcquireResource();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 1, jobState.Pool.Statistics.SizePool );
									
			IResource<TestResource> r2 = jobState.Pool.AcquireResource();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			r1.Dispose();

			Assert.AreEqual( 1, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			r2.Dispose();

			Assert.AreEqual( 2, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			jobState.Pool.Dispose();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 0, jobState.Pool.Statistics.SizePool );
			Assert.AreEqual( 2, jobState.Pool.Statistics.MaxSizePool );
		}

		[TestCase]
		void Minimum_pool_size_of_0_is_maintained()
		{
			// Strategy:
			//	---------
			// 1. Create a pool with a min size of 0.
			// 2. Acquire one resource and check the number of available and the number in the pool.
			// 3. Acquire another resource and check the pool state.
			// 4. Release one invalid resource and check the pool state.
			// 5. Release the remaining invalid resource and check the pool state.

			JobStateInfo jobState = new JobStateInfo( 2, 0, 0, 5 );

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 0, jobState.Pool.Statistics.SizePool );

			// acquire two resources
			IResource<TestResource> r1 = jobState.Pool.AcquireResource();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 1, jobState.Pool.Statistics.SizePool );

			IResource<TestResource> r2 = jobState.Pool.AcquireResource();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			r1.Invalid = true;
			r1.Dispose();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 1, jobState.Pool.Statistics.SizePool );

			r2.Invalid = true;
			r2.Dispose();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 0, jobState.Pool.Statistics.SizePool );

			jobState.Pool.Dispose();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 0, jobState.Pool.Statistics.SizePool );
			Assert.AreEqual( 2, jobState.Pool.Statistics.MaxSizePool );
		}

		[TestCase]
		void Minimum_pool_size_of_2_is_maintained()
		{
			// Strategy:
			//	---------
			// 1. Create a pool with a min size of 0.
			// 2. Acquire one resource and check the number of available and the number in the pool.
			// 3. Acquire another resource and check the pool state.
			// 4. Acquire another resource and check the pool state.
			// 5. Release one invalid resource and check the pool state.
			// 6. Release the remaining invalid resources and check the pool state.

			JobStateInfo jobState = new JobStateInfo( 2, 0, 2, 5 );

			Assert.AreEqual( 2, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			// acquire two resources
			IResource<TestResource> r1 = jobState.Pool.AcquireResource();

			Assert.AreEqual( 1, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			IResource<TestResource> r2 = jobState.Pool.AcquireResource();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			IResource<TestResource> r3 = jobState.Pool.AcquireResource();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 3, jobState.Pool.Statistics.SizePool );

			r1.Invalid = true;
			r1.Dispose();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			r2.Invalid = true;
			r2.Dispose();

			Assert.AreEqual( 1, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			r3.Invalid = true;
			r3.Dispose();

			Assert.AreEqual( 2, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 2, jobState.Pool.Statistics.SizePool );

			jobState.Pool.Dispose();

			Assert.AreEqual( 0, jobState.Pool.Statistics.NumAvailableResources );
			Assert.AreEqual( 0, jobState.Pool.Statistics.SizePool );
			Assert.AreEqual( 3, jobState.Pool.Statistics.MaxSizePool );
		}

		[TestCase]
		void Maximum_pool_size_is_not_breached()
		{
			// Strategy:
			//	---------
			// 1. Create a pool with a max size of 2.
			// 2. Acquire one resource and check the number of available and the number in the pool.
			// 3. Acquire another resource and check the pool state.
			// 4. Acquire another resource and check the pool state.
			// 5. Release one invalid resource and check the pool state.
			// 6. Release the remaining invalid resources and check the pool state.

			JobStateInfo jobState = new JobStateInfo( 10, 300, 2, 2 );

			jobState.WaitForAllJobs();

			ResourcePoolStatistics stats = jobState.Pool.Statistics;

			Assert.AreEqual( 2, stats.MaxSizePool );
		}


	}
}
