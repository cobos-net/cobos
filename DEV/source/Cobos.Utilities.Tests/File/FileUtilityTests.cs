// ============================================================================
// Filename: FileUtilityTests.cs
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
using System.IO;
using Cobos.Utilities.File;
using NUnit.Framework;

namespace Cobos.Utilities.Tests.File
{
	[TestFixture]
	public class FileUtilityTests
	{
		[TestCase]
		public void Folder_is_folder_but_doesnt_exist()
		{
			Assert.Throws<DirectoryNotFoundException>( delegate { FileUtility.IsFolder( @"C:\totally\madeup\folder\location" ); } );
		}

		[TestCase]
		public void File_is_file_but_doesnt_exist()
		{
			Assert.Throws<DirectoryNotFoundException>( delegate { FileUtility.IsFile( @"C:\totally\madeup\file\location.txt" ); } );
		}

		[TestCase]
		public void File_is_a_file()
		{
			Assert.True( FileUtility.IsFile( TestManager.TestFilesLocation + @"\TestFile.txt" ) );
		}

		[TestCase]
		public void Folder_is_a_folder()
		{
			Assert.True( FileUtility.IsFolder( TestManager.TestFilesLocation + @"\TestDirectory" ) );
		}

		[TestCase]
		public void File_is_really_a_folder()
		{
			Assert.False( FileUtility.IsFile( TestManager.TestFilesLocation + @"\TestDirectory" ) );
		}

		[TestCase]
		public void Folder_is_really_a_file()
		{
			Assert.False( FileUtility.IsFolder( TestManager.TestFilesLocation + @"\TestFile.txt" ) );
		}

		[TestCase]
		public void File_contents_are_the_same()
		{
			Assert.True( FileUtility.Md5Compare( TestManager.TestFilesLocation + @"\TestFile.txt", TestManager.TestFilesLocation + @"\TestFile2.txt" ) );
			Assert.True( FileUtility.BinaryCompare( TestManager.TestFilesLocation + @"\TestFile.txt", TestManager.TestFilesLocation + @"\TestFile2.txt" ) );
		}
		
		[TestCase]
		public void File_contents_are_not_the_same()
		{
			Assert.False( FileUtility.Md5Compare( TestManager.TestFilesLocation + @"\TestFile3.txt", TestManager.TestFilesLocation + @"\TestFile4.txt" ) );
			Assert.False( FileUtility.BinaryCompare( TestManager.TestFilesLocation + @"\TestFile3.txt", TestManager.TestFilesLocation + @"\TestFile4.txt" ) );
		}

		[TestCase]
		public void Can_differentiate_between_relative_and_absolute_paths()
		{
			Assert.True( FileUtility.IsAbsolutePath( @"C:\temp\madeup.txt" ) );
			Assert.True( FileUtility.IsAbsolutePath( TestManager.TestFilesLocation + @"\TestFile.txt" ) );
			Assert.True( FileUtility.IsAbsolutePath( @"\\madeup_server\madeup share" ) );
			Assert.True( FileUtility.IsAbsolutePath( @"\\madeup_server\madeup share\madeup file.txt" ) );
			Assert.False( FileUtility.IsAbsolutePath( @"\madeup\folder\" ) );
			Assert.False( FileUtility.IsAbsolutePath( @"\madeup\folder\test.txt" ) );
			Assert.False( FileUtility.IsAbsolutePath( @"\temp\test.txt" ) );
		}

	}
}
