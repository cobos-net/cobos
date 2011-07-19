﻿// ============================================================================
// Filename: IResource.cs
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

// 05-Feb-11 N.Davis
// -----------------
// Rebranded from "Cobos" to "Intergraph.AsiaPac" in preparation for use in the Generic CAD Interoperability project

using System;

#if INTERGRAPH_BRANDING
namespace Intergraph.AsiaPac.Utilities.Threading.Resource
#else
namespace Cobos.Utilities.Threading.Resource
#endif
{
	/// <summary>
	/// Wraps a resource managed by a thread pool.  The thread pool 
	/// returns the wrapped resource for use by the currnet thread.
	/// Use the Dispose method to return the resource to the pool.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IResource<T> : IDisposable
	{
		/// <summary>
		/// Obtain a reference to the underlying resource
		/// </summary>
		T Instance
		{
			get;
		}

		/// <summary>
		/// Mark the resouce as invalid before returning it to the 
		/// pool so that it may be removed.  An example of a 
		/// resource that might be invalid is a database connection
		/// that has been closed after not being used for a while.
		/// </summary>
		bool Invalid
		{
			get;
			set;
		}
	}
}