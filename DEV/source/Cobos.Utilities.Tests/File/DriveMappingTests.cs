using System;
using Cobos.Utilities.File;
using NUnit.Framework;
using System.IO;

using SysFile = System.IO.File;

namespace Cobos.Utilities.Tests.File
{
	[TestFixture]
	public class DriveMappingTests
	{
		[TestCase]
		public void Can_map_and_unmap_a_local_folder()
		{
			const string testFilePath = TestManager.TestFilesLocation + @"\drivemappingtest.txt";

			// create dummy file to test that the mapping succeeded
			FileStream fs = SysFile.Create( testFilePath );
			using ( TextWriter writer = new StreamWriter( fs ) )
			{
				writer.WriteLine( "this is a test file" );
			}
			fs.Close();		
	
			// find a free drive...
			char drive = 'A';

			while ( DriveMapping.DriveExists( drive ) )
			{
				++drive;
			}

			// test that removing a non-existent drive fails gracefully
			Assert.True( DriveMapping.RemoveLocalFolder( drive, null ) );

			Assert.True( DriveMapping.MapLocalFolder( drive, TestManager.TestFilesLocation ) );

			Assert.True( DriveMapping.DriveExists( drive ) );

			// confirm that if we access the test file by either path, we are actually looking at the same file
			FileHandle h1 = new FileHandle( testFilePath );
			FileHandle h2 = new FileHandle( drive + @":\drivemappingtest.txt" );

			Assert.AreEqual( h1, h2 );

			Assert.True( DriveMapping.RemoveLocalFolder( drive, null ) );

			Assert.False( DriveMapping.DriveExists( drive ) );
			
			// cleanup the the test file
			FileUtility.DeleteFile( testFilePath );
		}

		[TestCase]
		public void Can_map_and_unmap_a_network_folder()
		{
			// find a free drive...
			char drive = 'A';

			while ( DriveMapping.DriveExists( drive ) )
			{
				++drive;
			}

			// test that removing a non-existent drive fails gracefully
			Assert.True( DriveMapping.RemoveNetworkDrive( drive ) );

			Assert.True( DriveMapping.MapNetworkDrive( drive, TestManager.UncSharedFolder ) );

			Assert.True( DriveMapping.DriveExists( drive ) );

			// confirm that if we access the test file by either path, we are actually looking at the same file
			FileHandle h1 = new FileHandle( TestManager.UncSharedFolder + @"\init_cad.h" );
			FileHandle h2 = new FileHandle( drive + @":\init_cad.h" );

			Assert.AreEqual( h1, h2 );

			Assert.True( DriveMapping.RemoveLocalFolder( drive, null ) );

			Assert.False( DriveMapping.DriveExists( drive ) );
		}

	}
}
