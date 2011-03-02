﻿using System;
using System.Collections.Generic;
using System.IO;
using Intergraph.AsiaPac.Utilities.File;
using Xunit;

namespace Intergraph.AsiaPac.Utilities.Tests.File
{
	public class FileUtilityTests
	{
		[Fact]
		public void Folder_is_folder_but_doesnt_exist()
		{
			Assert.Throws<DirectoryNotFoundException>( delegate { FileUtility.IsFolder( @"C:\totally\madeup\folder\location" ); } );
		}

		[Fact]
		public void File_is_file_but_doesnt_exist()
		{
			Assert.Throws<DirectoryNotFoundException>( delegate { FileUtility.IsFile( @"C:\totally\madeup\file\location.txt" ); } );
		}

		[Fact]
		public void File_is_a_file()
		{
			Assert.True( FileUtility.IsFile( TestManager.TestFilesLocation + @"\TestFile.txt" ) );
		}

		[Fact]
		public void Folder_is_a_folder()
		{
			Assert.True( FileUtility.IsFolder( TestManager.TestFilesLocation + @"\TestDirectory" ) );
		}

		[Fact]
		public void File_is_really_a_folder()
		{
			Assert.False( FileUtility.IsFile( TestManager.TestFilesLocation + @"\TestDirectory" ) );
		}

		[Fact]
		public void Folder_is_really_a_file()
		{
			Assert.False( FileUtility.IsFolder( TestManager.TestFilesLocation + @"\TestFile.txt" ) );
		}

		[Fact]
		public void File_contents_are_the_same()
		{
			Assert.True( FileUtility.Md5Compare( TestManager.TestFilesLocation + @"\TestFile.txt", TestManager.TestFilesLocation + @"\TestFile2.txt" ) );
			Assert.True( FileUtility.BinaryCompare( TestManager.TestFilesLocation + @"\TestFile.txt", TestManager.TestFilesLocation + @"\TestFile2.txt" ) );
		}
		
		[Fact]
		public void File_contents_are_not_the_same()
		{
			Assert.False( FileUtility.Md5Compare( TestManager.TestFilesLocation + @"\TestFile3.txt", TestManager.TestFilesLocation + @"\TestFile4.txt" ) );
			Assert.False( FileUtility.BinaryCompare( TestManager.TestFilesLocation + @"\TestFile3.txt", TestManager.TestFilesLocation + @"\TestFile4.txt" ) );
		}

		[Fact]
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