// ============================================================================
// Filename: TestResource.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 21-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

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
