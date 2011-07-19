// ============================================================================
// Filename: NormalisedPath.cs
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
using System.IO;

#if INTERGRAPH_BRANDING
namespace Intergraph.AsiaPac.Utilities.File
#else
namespace Cobos.Utilities.File
#endif
{
	public class NormalisedPath : IComparable
	{
		/// <summary>
		/// Effectively a readonly value, but not enforced via the readonly keyword
		/// to allow efficient copying of already normalised paths.
		/// </summary>
		string _value;

		/// <summary>
		/// Create a normalised path
		/// </summary>
		/// <param name="path"></param>
		public NormalisedPath( string path )
		{
			_value = NormalisePath( path );
		}

		/// <summary>
		/// Create an absolute normalised path from a relative path.
		/// </summary>
		/// <param name="relativePath">The relative file path</param>
		/// <param name="relativeTo">This must be a folder.  The class cannot easily differentiate
		/// between a folder path and a file with no extension.  We cannot always test whether
		/// it's a file or folder because there is no guarantee that the file or folder will exist</param>
		public NormalisedPath( string relativePath, string relativeTo )
		{
			_value = NormalisePath( relativeTo + @"\" + relativePath );
		}

		/// <summary>
		/// Private constructor to provide a more efficient method of copying
		/// some or all of a normalised path, bypassing the normal parsing
		/// required of an arbitrary path.
		/// </summary>
		private NormalisedPath()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Check whether the path contains anything....
		/// </summary>
		public bool IsNullPath
		{
			get
			{
				return string.IsNullOrEmpty( _value );
			}
		}

		/// <summary>
		/// Provide a more efficient method of getting a normalised directory 
		/// from an already normalised path.
		/// </summary>
		/// <returns></returns>
		public NormalisedPath GetDirectoryName()
		{
			NormalisedPath path = new NormalisedPath();
			path._value = Path.GetDirectoryName( _value );
			return path;
		}

		/// <summary>
		/// Get the filename of the path
		/// </summary>
		/// <returns></returns>
		public string GetFileName()
		{
			return Path.GetFileName( _value );
		}

		/// <summary>
		/// Get the filename without the extension
		/// </summary>
		/// <returns></returns>
		public string GetFileNameWithoutExtension()
		{
			return Path.GetFileNameWithoutExtension( _value );
		}

		/// <summary>
		/// Get the extension of the file
		/// </summary>
		/// <returns></returns>
		public string GetExtension()
		{
			return Path.GetExtension( _value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo( object obj )
		{
			NormalisedPath other = obj as NormalisedPath;

			if ( other != null )
			{
				return Value.CompareTo( other.Value );
			}
			else
			{
				throw new ArgumentException( "Object is not a NormalisedPath" );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals( System.Object obj )
		{
			// If parameter is null return false.
			if ( obj == null )
			{
				return false;
			}

			// If parameter cannot be cast to NormalisedPath return false.
			NormalisedPath p = obj as NormalisedPath;

			if ( (System.Object)p == null )
			{
				return false;
			}

			// Return true if the fields match:
			return Value == p.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator == ( NormalisedPath a, NormalisedPath b )
		{
			// If both are null, or both are same instance, return true.
			if ( System.Object.ReferenceEquals( a, b ) )
			{
				return true;
			}

			// If one is null, but not both, return false.
			if ( ((object)a == null) || ((object)b == null) )
			{
				return false;
			}

			// Return true if the fields match:
			return a.Value == b.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator != ( NormalisedPath a, NormalisedPath b )
		{
			return !(a == b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		/// <summary>
		/// Normalise a path to lower case, single Windows style delimiters with no
		/// dotted directory paths.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string NormalisePath( string path )
		{
			// Normally we identify a file using file information such as volume serial, and file index values.
			// This is OK as long as the file exists, which will not always be the case.

			string pathSeperator = Path.DirectorySeparatorChar.ToString();

			// enable quick matching by forcing everything to lower case
			// force Windows style path delimiters and remove quoted paths
			path = path.ToLower().Replace( '/', Path.DirectorySeparatorChar ).Replace( "\"", "" ).Replace( "'", "" );

			// now remove any \. current and \.. relative directory paths
			string[] directories = path.Split( new char[] { Path.DirectorySeparatorChar } );
			string[] fullpath = new string[ directories.Length ];

			// crude test for unc path specifiers.  Remember this for later, the processing strips the
			// leading '\\' characters out.
			bool isUnc = false;

			if ( path.Length >= 2 )
			{
				isUnc = path[ 0 ] == Path.DirectorySeparatorChar && path[ 1 ] == Path.DirectorySeparatorChar;
			}

			int d = 0, f = 0;

			for ( ; d < directories.Length; ++d )
			{
				string directory = directories[ d ].Trim();

				if ( string.IsNullOrEmpty( directory ) || directory == "." )
				{
					// don't copy this one, either empty (double delimiter) or current directory
				}
				else if ( directory == ".." )
				{
					// to resolve the path, dont copy this element or the previous one
					--f;
				}
				else
				{
					fullpath[ f++ ] = directory;
				}
			}

			if ( isUnc )
			{
				// re-apply the leading unc path specifier
				return @"\\" + string.Join( pathSeperator, fullpath, 0, f );
			}
			else
			{
				return string.Join( pathSeperator, fullpath, 0, f );
			}
		}

	}
}
