// ============================================================================
// Filename: FileVersionHelper.cs
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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Cobos.Utilities.File
{
	public class FileVersionHelper
	{
		private const string _regExpVersion = @"\s*(\d+)\s*[.|,]\s*(\d+)\s*[.|,]\s*(\d+)(?:\s*[.|,]\s*(\d+))?";
	
		public readonly UInt16 _major = 0, _minor = 0, _build = 0, _revision = 0;

		public readonly UInt64 _int64Value = 0;

		public readonly string _stringValue = null;

		public readonly bool _isNull = false;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		public FileVersionHelper( string path )
		{
			FileVersionInfo info = FileVersionInfo.GetVersionInfo( path );

			if ( info == null || info.FileVersion == null )
			{
				_isNull = true;
				return;
			}

			string[] tokens = Regex.Split( info.FileVersion, _regExpVersion );

			if ( tokens == null || tokens.Length == 1 )
			{
				_isNull = true;
				return;
			}

			_major = Convert.ToUInt16( tokens[ 1 ] );
			_minor = Convert.ToUInt16( tokens[ 2 ] );
			_build = Convert.ToUInt16( tokens[ 3 ] );

			_int64Value = ((UInt64)_major << 48) | ((UInt64)_minor << 32) | (UInt32)(_build << 16) | _revision;

			if ( tokens.Length == 4 )
			{
				_stringValue = String.Format( "{0}.{1}.{2}", tokens[ 1 ], tokens[ 2 ], tokens[ 3 ] );
			}
			else
			{
				_stringValue = String.Format( "{0}.{1}.{2}.{3}", tokens[ 1 ], tokens[ 2 ], tokens[ 3 ], tokens[ 5 ] );
				_revision = Convert.ToByte( tokens[ 5 ] );
			}
		}

		public static bool operator ==( FileVersionHelper lhs, FileVersionHelper rhs )
		{
			return lhs._int64Value == rhs._int64Value;
		}

		public static bool operator !=( FileVersionHelper lhs, FileVersionHelper rhs )
		{
			return lhs._int64Value != rhs._int64Value;
		}

		public static bool operator >( FileVersionHelper lhs, FileVersionHelper rhs )
		{
			return lhs._int64Value > rhs._int64Value;
		}

		public static bool operator <( FileVersionHelper lhs, FileVersionHelper rhs )
		{
			return lhs._int64Value < rhs._int64Value;
		}

		public static bool operator >=( FileVersionHelper lhs, FileVersionHelper rhs )
		{
			return lhs._int64Value >= rhs._int64Value;
		}

		public static bool operator <=( FileVersionHelper lhs, FileVersionHelper rhs )
		{
			return lhs._int64Value <= rhs._int64Value;
		}

		// Compiler Warning (level 3) CS0661
		// 'class' defines operator == or operator != but does not override Object.GetHashCode()
		// The compiler detected the user-defined equality or inequality operator, but no override 
		// for the GetHashCode function. A user-defined equality or inequality operator implies that 
		// you also want to override the GetHashCode function.
		public override int GetHashCode()
		{
			return 0;
		}

		// Compiler Warning (level 3) CS0660
		// 'class' defines operator == or operator != but does not override Object.Equals(object o)
		// The compiler detected the user-defined equality or inequality operator, but no override for 
		// the Equals function. A user-defined equality or inequality operator implies that you also 
		// want to override the Equals function.
		public override bool Equals( object o )
		{
			return true;
		}
		
	}
}
