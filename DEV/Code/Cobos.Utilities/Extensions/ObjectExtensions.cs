﻿// ============================================================================
// Filename: ObjectExtensions.cs
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if INTERGRAPH_BRANDING
namespace Intergraph.AsiaPac.Utilities.Extensions
#else
namespace Cobos.Utilities.Extensions
#endif
{
	public static class ObjectExtensions
	{
		/// <summary>
		/// extend the object class to support casting to an anonymous type
		/// </summary>
		/// <typeparam name="T">The anonymous type to cast to</typeparam>
		/// <param name="obj">The 'this' object reference</param>
		/// <param name="example">An instance of an anonymous type</param>
		/// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
		public static T CastByExample<T>( this object obj, T example )
		{
			try
			{
				return (T)obj;
			}
			catch ( InvalidCastException )
			{
				return default( T );
			}
		}

		/// <summary>
		/// Serialize the object (usually a struct with StructLayout attributes)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static byte[] ConvertToByteArray( this object obj )
		{
			int size = Marshal.SizeOf( obj );

			IntPtr buffer = Marshal.AllocHGlobal( size );
			Marshal.StructureToPtr( obj, buffer, false );

			byte[] data = new byte[ size ];
			Marshal.Copy( buffer, data, 0, size );
			Marshal.FreeHGlobal( buffer );
			
			return data;
		}

		public static T ConvertTo<T>( this byte[] data )
		{
			Type type = typeof( T );

			int size = Marshal.SizeOf( type );

			if ( size > data.Length )
			{
				return default(T);
			}

			IntPtr buffer = Marshal.AllocHGlobal( size );
			Marshal.Copy( data, 0, buffer, size );

			T obj = (T)Marshal.PtrToStructure( buffer, type );
			Marshal.FreeHGlobal( buffer );

			return obj;
		} 
	}
}