﻿// ============================================================================
// Filename: GenericWrapper.cs
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
using System.Linq;
using System.Text;

namespace Cobos.Utilities.Wrappers
{
	/// <summary>
	/// Can be used to implement generic classes that are designed to be used with
	/// unrelated reference types that cannot satisfy a 'where' constaint with
	/// multiple definitions.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GenericWrapper<T> : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		T _object = default( T );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		GenericWrapper( T obj )
		{
			_object = obj;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			IDisposable idisp = _object as IDisposable;
			
			if ( idisp != null )
			{
				idisp.Dispose();
			}
			
			_object = default( T );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="C"></typeparam>
		/// <returns></returns>
		public C Cast<C>() where C : class
		{
			return _object as C;
		}
	}
}