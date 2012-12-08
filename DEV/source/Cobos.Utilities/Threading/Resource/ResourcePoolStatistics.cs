// ============================================================================
// Filename: ResourcePoolStatistics.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
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

namespace Cobos.Utilities.Threading.Resource
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
