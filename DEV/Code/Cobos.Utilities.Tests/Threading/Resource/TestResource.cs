using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cobos.Utilities.Threading.Resource;

namespace Cobos.Utilities.Tests.Threading.Resource
{
	class TestResource
	{
		long _id;

		public readonly int WorkPeriodMs;

		public TestResource( long i, int workPeriodMs )
		{
			_id = i;

			// make the threads work within a tolerance of the work period 
			// to avoid the situation where all threads execute in an 
			// orderly fashion.  We want to introduce as much uncertainty 
			// as possible.
			double tolerance = (double)workPeriodMs * 0.15; // +/- 15% max

			Random rand = new Random();

			int delta = rand.Next( (int)Math.Floor( tolerance ) );

			if ( rand.Next( 1 ) == 1 )
			{
				WorkPeriodMs = workPeriodMs + delta;
			}
			else
			{
				WorkPeriodMs = workPeriodMs - delta;
			}
		}

		public void DoWork()
		{
			Console.WriteLine( "Working with resource {0}", _id );
			Thread.Sleep( WorkPeriodMs );
		}
	}

	class TestResourceAllocator : IResourceAllocator<TestResource>
	{
		static long _resourceId;

		readonly int _workPeriodMs;

		public TestResourceAllocator( int workPeriodMs )
		{
			_workPeriodMs = workPeriodMs;
		}

		public TestResource Create()
		{
			return new TestResource( Interlocked.Increment( ref _resourceId ), _workPeriodMs );
		}
	}
}
