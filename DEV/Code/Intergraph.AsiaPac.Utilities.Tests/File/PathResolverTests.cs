using System;
using Xunit;
using Intergraph.AsiaPac.Utilities.File;

namespace Intergraph.AsiaPac.Utilities.Tests.File
{
	class PathResolverTests
	{
		[Fact]
		public void Construction_with_valid_folders_works()
		{
			Assert.DoesNotThrow( delegate { PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, null ); } );
			Assert.DoesNotThrow( delegate { PathResolver resolver = new PathResolver( TestManager.TestFilesLocation + @"/testfile.txt", null ); } );

			// fully qualified look in folder
			Assert.DoesNotThrow(
				delegate
				{
					PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { TestManager.TestFilesLocation + "/TestDirectory" } );
					Assert.Equal( resolver.LookInFolders.Count, 2 );
				} );
			
			// Relative look in folder
			Assert.DoesNotThrow(
				delegate
				{
					PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { "TestDirectory" } );
					Assert.Equal( resolver.LookInFolders.Count, 2 );
				} );
		}

		[Fact]
		public void Construction_with_invalid_root_fails()
		{
			Assert.Throws<Exception>( 
				delegate 
				{ 
					PathResolver resolver = new PathResolver( @"C:\totally\madeup\folder\location", null ); 
				} );
		}

		[Fact]
		public void Invalid_look_in_folders_succeeds()
		{
			PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"C:\totally\madeup\folder\location" } );
			Assert.Equal( resolver.LookInFolders.Count, 1 );

			resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"\madeup\folder\location" } );
			Assert.Equal( resolver.LookInFolders.Count, 1 );

			// even though we supply a file, it should resolve the lookin folder correctly
			resolver = new PathResolver( TestManager.TestFilesLocation + @"\testfile.txt", new string[] { TestManager.TestFilesLocation + "/TestDirectory/TestRelativeFile.txt" } );
			Assert.Equal( resolver.LookInFolders.Count, 2 );

			Assert.Equal( resolver.LookInFolders[ 0 ], new NormalisedPath( TestManager.TestFilesLocation ) );
			Assert.Equal( resolver.LookInFolders[ 1 ], new NormalisedPath( TestManager.TestFilesLocation + "/TestDirectory/" ) );
		}

		[Fact]
		public void Relative_and_absolute_lookin_folders_succeeds()
		{
			PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"\TestDirectory" } );
			Assert.Equal( resolver.LookInFolders.Count, 2 );

			resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"\TestDirectory", @"TestDirectory2/TestDirectory3/" } );
			Assert.Equal( resolver.LookInFolders.Count, 3 );

			resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"\TestDirectory", TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3/" } );
			Assert.Equal( resolver.LookInFolders.Count, 3 );
		}

		[Fact]
		public void Can_resolve_valid_files()
		{
			PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"\TestDirectory", TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3/" } );
			Assert.Equal( resolver.LookInFolders.Count, 3 );

			NormalisedPath found = resolver.FindFilePath( "testFILE.txt" );
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( "testFile.txt", TestManager.TestFilesLocation ) );

			found = resolver.FindFilePath( "TestRelativeFile.txt" );
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( "TESTRELATIVEFILE.txt", TestManager.TestFilesLocation + @"/TestDirectory/" ) );

			found = resolver.FindFilePath( "TestDeepNestedFile.txt" );
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3\TESTDEEPNESTEDFILE.txt" ) );

			// TestDirectory2 is not included in the lookin folders
			found = resolver.FindFilePath( "Another test file.txt" );
			Assert.Null( found );

			// but should still be able find it relative to other lookin folders
			found = resolver.FindFilePath( @"./TestDirectory2\Another test file.txt" );  // root folder
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2/ANOTHER TEST FILE.TXT" ) );

			found = resolver.FindFilePath( @"../TestDirectory2\Another test file.txt" ); // testdirectory
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2/ANOTHER TEST FILE.TXT" ) );

			found = resolver.FindFilePath( @"..\ANOTHER TEST FILE.TXT" ); // testdirectory3
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2/ANOTHER TEST FILE.TXT" ) );

			// finds fully qualified paths without attempting to use the lookin folders
			found = resolver.FindFilePath( TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3\TESTDEEPNESTEDFILE.txt" );
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3\TESTDEEPNESTEDFILE.txt" ) );
		}

		[Fact]
		public void Doesnt_resolve_invalid_files()
		{
			PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"\TestDirectory", TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3/" } );
			Assert.Equal( resolver.LookInFolders.Count, 3 );

			NormalisedPath found = resolver.FindFilePath( "totally made up file.txt" );
			Assert.Null( found );

			found = resolver.FindFilePath( @"C:\totally\madeup\folder\location\file.txt" );
			Assert.Null( found );
		}

		[Fact]
		public void Can_resolve_valid_folders()
		{
			PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"\TestDirectory" } );
			Assert.Equal( resolver.LookInFolders.Count, 2 );

			NormalisedPath found = resolver.FindFolderPath( "TestDirectory2" );
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2" ) );

			// TestDirectory3 is not included in the lookin folders
			found = resolver.FindFolderPath( "TestDirectory3" );
			Assert.Null( found );

			// but should still be able find it relative to other lookin folders
			found = resolver.FindFolderPath( @"./TestDirectory2\TestDirectory3" );  // root folder
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3" ) );

			found = resolver.FindFolderPath( @"../TestDirectory2\TestDirectory3" ); // testdirectory
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3" ) );

			// resolves folder paths even when given a file
			found = resolver.FindFolderPath( @"..\TestDirectory2/ANOTHER TEST FILE.TXT" ); // testdirectory
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TESTDIRECTORY2/" ) );

			// finds fully qualified paths without attempting to use the lookin folders
			found = resolver.FindFolderPath( TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3" );
			Assert.NotNull( found );
			Assert.Equal( found, new NormalisedPath( TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3" ) );
		}

		[Fact]
		public void Doesnt_resolve_invalid_folders()
		{
			PathResolver resolver = new PathResolver( TestManager.TestFilesLocation, new string[] { @"\TestDirectory", TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3/" } );
			Assert.Equal( resolver.LookInFolders.Count, 3 );

			NormalisedPath found = resolver.FindFilePath( "totally made up folder" );
			Assert.Null( found );

			found = resolver.FindFilePath( @"C:\totally\madeup\folder\location" );
			Assert.Null( found );

			found = resolver.FindFilePath( @"C:\totally\madeup\folder\location\file.txt" );
			Assert.Null( found );
		}

	}
}
