using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Intergraph.AsiaPac.Utilities.Threading;
using System.Threading;

namespace Intergraph.AsiaPac.Utilities.Tests.Threading
{
	public class TestResourcePool
	{
		class TestResource
		{
			int _id;

			public TestResource( int i )
			{
				_id = i;
			}

			public void DoWork( int i )
			{
				Console.WriteLine( "{0}: Thread {1} is working with resource {2}", DateTime.Now.Ticks, i, _id );
				Thread.Sleep( 100 );
			}
		}

		const int _maxThreads = 100;
		const int _maxResources = 5;

		static IResourcePool<TestResource> _resourcePool;

		static AutoResetEvent _allDone = new AutoResetEvent( false );


		[Fact]
		void Threading_pool_can_process_jobs()
		{
			List<TestResource> resources = new List<TestResource>( 5 );

			for ( int i = 1; i <= _maxResources; ++i )
			{
				resources.Add( new TestResource( i ) );
			}

			_resourcePool = ResourcePool<TestResource>.Create( resources );

			for ( int i = 1; i <= _maxThreads; ++i )
			{
				ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadProc ), i  );
				Thread.Sleep( 10 );
			}

			_allDone.WaitOne();

			// allow worker threads to finish
			Thread.Sleep( 100 );

			Assert.Equal( 0, _resourcePool.NumPending );
			Assert.Equal( 5, _resourcePool.NumAvailable );

			_resourcePool.Dispose();

			Assert.Equal( 0, _resourcePool.NumPending );
			Assert.Equal( 0, _resourcePool.NumAvailable );
		}

		// This thread procedure performs the task.
		static void ThreadProc( Object stateInfo )
		{
			IResource<TestResource> resource;

			using ( resource = _resourcePool.AcquireResource() )
			{
				resource.Instance.DoWork( (int)stateInfo );

				Console.WriteLine( "Pool Status: {0} Pending {1} Available", _resourcePool.NumPending, _resourcePool.NumAvailable );
			}

			if ( (int)stateInfo >= _maxThreads )
			{
				_allDone.Set();
			}
		}
	}
}
