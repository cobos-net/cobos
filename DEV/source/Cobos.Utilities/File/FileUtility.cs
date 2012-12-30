// ============================================================================
// Filename: FileUtility.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Cobos.Utilities.Text;

namespace Cobos.Utilities.File
{
	public static class FileUtility
	{
		/// <summary>
		/// Test whether a file or folder exists
		/// </summary>
		/// <returns></returns>
		public static bool FileOrFolderExists( string path )
		{
			try
			{
				return System.IO.File.GetAttributes( path ) != 0;
			}
			catch ( FileNotFoundException )
			{
				return false;
			}
			catch ( DirectoryNotFoundException )
			{
				return false;
			}
			catch ( IOException )
			{
				return true; // in use by another process
			}
			// let other exceptions pass through, something is really wrong with the path
		}

		/// <summary>
		/// Test whether the path exists and is a folder
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool ExistsAsFolder( string path )
		{
			if ( !FileOrFolderExists( path ) )
			{
				return false;
			}

			return IsFolder( path );
		}

		/// <summary>
		/// Test whether the path exists and is a file
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool ExistsAsFile( string path )
		{
			if ( !FileOrFolderExists( path ) )
			{
				return false;
			}

			return IsFile( path );
		}

		/// <summary>
		/// Is the path a file and not a folder?
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool IsFile( string path )
		{
			FileAttributes attr = System.IO.File.GetAttributes( path );

			return (attr & FileAttributes.Directory) != FileAttributes.Directory;
		}

		/// <summary>
		/// Is the path a folder and not a file?
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool IsFolder( string path )
		{
			FileAttributes attr = System.IO.File.GetAttributes( path );

			return (attr & FileAttributes.Directory) == FileAttributes.Directory;
		}

		/// <summary>
		/// Convenience method for testing a folder has files.  Not recursive.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool FolderHasFiles( string path )
		{
			return Directory.GetFiles( path ).Length > 0;
		}

		/// <summary>
		/// Convenience method for testing a folder for sub-folders
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool FolderHasFolders( string path )
		{
			return Directory.GetDirectories( path ).Length > 0;
		}

		/// <summary>
		/// The normal windows definition of an empty folder is one that
		/// has no files, directories don't count.  This method tells you
		/// that the folder is completely empty of files and folders.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool FolderIsEmpty( string path )
		{
			return !(FolderHasFiles( path ) || FolderHasFiles( path ));
		}


		/// <summary>
		/// Tries to determine whether a path is an absolute path or not.
		/// Extends Path.IsRooted, which cannot detect the difference
		/// between a relative path e.g. "\relative\subfolder" and the
		/// root folder of a directory or a unc path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool IsAbsolutePath( string path )
		{
			Uri uri;

			return Uri.TryCreate( path, UriKind.Absolute, out uri ) && uri.IsAbsoluteUri;

			//return _regexDrivePath.Match( path ) != Match.Empty || _regexUncPath.Match( path ) != Match.Empty;
		}

		/// <summary>
		/// Match a drive folder e.g. 
		/// </summary>
		static Regex _regexDrivePath = new Regex( "^[a-zA-Z]{1}:\\.*" );
		
		/// <summary>
		/// 
		/// </summary>
		static Regex _regexUncPath = new Regex( "^\\\\.*\\.*" );

		/// <summary>
		/// Delete a file.  If the file is read-only, this attribute is removed
		/// otherwise the standard File.Delete method will fail.
		/// </summary>
		/// <param name="path"></param>
		public static void DeleteFile( string path )
		{
			if ( System.IO.File.Exists( path ) )
			{
				FileInfo fi = new FileInfo( path );

				if ( (fi.Attributes & FileAttributes.ReadOnly) > 0 )
				{
					fi.Attributes -= FileAttributes.ReadOnly;
				}

				System.IO.File.Delete( path );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pattern"></param>
		/// <param name="recursive"></param>
		public static bool DeleteAllFiles( string path, string pattern, bool recursive, ref string error )
		{
			string[] found = null;

			if ( !FindAllFiles( path, pattern, recursive, ref found, ref error ) )
			{
				return false;
			}

			foreach ( string f in found )
			{
				DeleteFile( f );
			}

			return true;
		}

		/// <summary>
		/// Delete all folders from the specified directory
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool DeleteAllFolders( string path )
		{
			if ( !Directory.Exists( path ) )
			{
				return false;
			}

			foreach ( string d in Directory.GetDirectories( path ) )
			{
				Directory.Delete( d, true );
			}

			return true;
		}
		
		/// <summary>
		/// Clean the directory of all files and folders
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool CleanDirectory( string path )
		{
			if ( !Directory.Exists( path ) )
			{
				return false;
			}

			string error = null;

			DeleteAllFiles( path, "*", true, ref error );

			DeleteAllFolders( path );

			return true;
		}
		
		/// <summary>
		/// Remove any read only attributes from the file
		/// </summary>
		/// <param name="path"></param>
		public static void MakeReadWrite( string path )
		{
			if ( System.IO.File.Exists( path ) )
			{
				FileInfo fi = new FileInfo( path );

				if ( (fi.Attributes & FileAttributes.ReadOnly) > 0 )
				{
					fi.Attributes -= FileAttributes.ReadOnly;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pattern"></param>
		/// <param name="recursive"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public static bool MakeAllFilesReadWrite( string path, string pattern, bool recursive, ref string error )
		{
			string[] found = null;

			if ( !FindAllFiles( path, pattern, recursive, ref found, ref error ) )
			{
				return false;
			}

			foreach ( string f in found )
			{
				FileInfo fi = new FileInfo( f );

				if ( (fi.Attributes & FileAttributes.ReadOnly) > 0 )
				{
					fi.Attributes -= FileAttributes.ReadOnly;
				}
			}

			return true;
		}

		/// <summary>
		/// Get an Md5 hash of the contents of the specified file
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string Md5Hash( string path )
		{
			byte[] fileData;

			using ( FileStream stream = new FileStream( path, FileMode.Open, FileAccess.Read ) )
			{
				fileData = new byte[ stream.Length ];
				stream.Read( fileData, 0, fileData.Length );
			}

			return Md5Utility.GetHash( fileData );
		}

		/// <summary>
		/// Compare the Md5 hash values of the specified files
		/// </summary>
		/// <param name="compare"></param>
		/// <param name="compareTo"></param>
		/// <returns></returns>
		public static bool Md5Compare( string compare, string compareTo )
		{
			return Md5Hash( compare ) == Md5Hash( compareTo );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="compare"></param>
		/// <param name="compareTo"></param>
		/// <returns></returns>
		public static bool BinaryCompare( string compare, string compareTo )
		{
			FileInfo cmpInfo = new FileInfo( compare );
			FileInfo cmpToInfo = new FileInfo( compareTo );

			if ( cmpInfo.Length != cmpToInfo.Length )
			{
				return false;
			}

			using ( FileStream cmpStream = new FileStream( compare, FileMode.Open, FileAccess.Read ) )
			{
				using ( FileStream cmpToStream = new FileStream( compareTo, FileMode.Open, FileAccess.Read ) )
				{
					const int bufferSize = 4096;

					byte[] cmpBytes = new byte[ bufferSize ];
					byte[] cmpToBytes = new byte[ bufferSize ];

					int cmpBytesRead, cmpToBytesRead, totalBytesRead = 0;

					// we know the files are the same length, so we can assume they will stop reading at the same point

					while ( (cmpBytesRead = cmpStream.Read( cmpBytes, 0, bufferSize )) > 0
						&& (cmpToBytesRead = cmpToStream.Read( cmpToBytes, 0, bufferSize )) > 0 )
					{
						if ( cmpBytesRead != cmpToBytesRead )
						{
							// strictly speaking this case should never happen
							return false;
						}

						for ( int i = 0; i < cmpBytesRead; ++i )
						{
							if ( cmpBytes[ i ] != cmpToBytes[ i ] )
							{
								return false;
							}
						}

						totalBytesRead += cmpBytesRead;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Only really useful for help in handling error conditions
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pattern"></param>
		/// <param name="recursive"></param>
		/// <param name="results"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public static bool FindAllFiles( string path, string pattern, bool recursive, ref string[] results, ref string error )
		{
			try
			{
				results = Directory.GetFiles( path, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );
				return true;
			}
			catch ( System.Exception e )
			{
				error = e.Message;
				return false;
			}
		}
	}
}
