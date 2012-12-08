﻿// ============================================================================
// Filename: DataColumnDescriptor.cs
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
using System.Data;

namespace Cobos.Data.Utilities
{
	public struct DataColumnDescriptor
	{
		public readonly int Ordinal;

		public readonly Type DataType;

		public DataColumnDescriptor( int ordinal, Type type )
		{
			Ordinal = ordinal;
			DataType = type;
		}

		public DataColumnDescriptor( DataColumn column )
		{
			Ordinal = column.Ordinal;
			DataType = column.DataType;
		}

		/// <summary>
		/// Helper method to convert an array of DataColumn objects.
		/// </summary>
		public static DataColumnDescriptor[] ConstructFrom( DataColumn[] columns )
		{
			if ( columns == null )
			{
				return null;
			}

			DataColumnDescriptor[] result = new DataColumnDescriptor[ columns.Length ];

			for ( int c = 0; c < columns.Length; ++c )
			{
				result[ c ] = new DataColumnDescriptor( columns[ c ] );
			}

			return result;
		}
	}
}