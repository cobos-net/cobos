using System;
using System.IO;
using Cobos.Utilities.File;
using NUnit.Framework;

namespace Cobos.Utilities.Tests.File
{
	[TestFixture]
	public class FileHandleTests
	{
		[TestCase]
		public void File_handle_for_existing_file_succeeds()
		{
			Assert.DoesNotThrow( delegate { new FileHandle( TestManager.TestFilesLocation + @"\TestFile.txt" ); } );
		}
		
		[TestCase]
		public void File_handle_for_non_existing_file_fails()
		{
			Assert.Throws<DirectoryNotFoundException>( delegate { new FileHandle( @"C:\totally\madeup\file\location.txt" ); } );
		}

		[TestCase]
		public void File_handle_for_existing_folder_succeeds()
		{
			Assert.DoesNotThrow( delegate { new FileHandle( TestManager.TestFilesLocation + @"\TestDirectory" ); } );
		}

		[TestCase]
		public void File_handle_for_non_existing_folder_fails()
		{
			Assert.Throws<DirectoryNotFoundException>( delegate { new FileHandle( @"C:\totally\madeup\folder\location" ); } );
		}

		[TestCase]
		public void File_handle_comparison_for_same_file_but_different_path_succeeds()
		{
			string path1 = TestManager.TestFilesLocation + @"\TestFile.txt";
			string path2 = TestManager.TestFilesLocation + @"\..\TestFiles\TestFile.txt";
			
			// jumble the paths up a bit
			path1 = path1.ToLower();
			path1 = path1.Replace( '\\', '/' );
			path2 = path2.ToUpper();

			FileHandle handle1 = new FileHandle( path1 );
			FileHandle handle2 = new FileHandle( path2 );

			Assert.True( handle1.CompareTo( handle2 ) == 0 );
			Assert.True( handle2.CompareTo( handle1 ) == 0 );
		}

		[TestCase]
		public void File_handle_comparison_for_different_files_fails()
		{
			string path1 = TestManager.TestFilesLocation + @"\TestFile.txt";
			string path2 = TestManager.TestFilesLocation + @"\TestFile2.txt";

			FileHandle handle1 = new FileHandle( path1 );
			FileHandle handle2 = new FileHandle( path2 );

			Assert.False( handle1.CompareTo( handle2 ) == 0 );
			Assert.False( handle2.CompareTo( handle1 ) == 0 );
		}

		[TestCase]
		public void File_handle_comparison_for_same_folder_but_different_path_succeeds()
		{
			string path1 = TestManager.TestFilesLocation + @"\TestDirectory";
			string path2 = TestManager.TestFilesLocation + @"\..\TestFiles\TestDirectory";

			// jumble the paths up a bit
			path1 = path1.ToLower();
			path1 = path1.Replace( '\\', '/' );
			path2 = path2.ToUpper();

			FileHandle handle1 = new FileHandle( path1 );
			FileHandle handle2 = new FileHandle( path2 );

			Assert.True( handle1.CompareTo( handle2 ) == 0 );
			Assert.True( handle2.CompareTo( handle1 ) == 0 );
		}

		[TestCase]
		public void File_handle_comparison_for_different_folder_fails()
		{
			string path1 = TestManager.TestFilesLocation + @"\TestDirectory";
			string path2 = TestManager.TestFilesLocation + @"\TestDirectory2";

			FileHandle handle1 = new FileHandle( path1 );
			FileHandle handle2 = new FileHandle( path2 );

			Assert.False( handle1.CompareTo( handle2 ) == 0 );
			Assert.False( handle2.CompareTo( handle1 ) == 0 );
		}
	}
}
