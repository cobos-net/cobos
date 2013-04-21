// ----------------------------------------------------------------------------
// <copyright file="DriveMappingTests.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

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
            string testFilePath = TestManager.TestFilesLocation + @"\drivemappingtest.txt";

            // create dummy file to test that the mapping succeeded
            FileStream fs = SysFile.Create(testFilePath);
            using (TextWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine("this is a test file");
            }
            fs.Close();

            // find a free drive...
            char drive = 'A';

            while (DriveMapping.DriveExists(drive))
            {
                ++drive;
            }

            // test that removing a non-existent drive fails gracefully
            Assert.True(DriveMapping.RemoveLocalFolder(drive, null));

            Assert.True(DriveMapping.MapLocalFolder(drive, TestManager.TestFilesLocation));

            Assert.True(DriveMapping.DriveExists(drive));

            // confirm that if we access the test file by either path, we are actually looking at the same file
            FileHandle h1 = new FileHandle(testFilePath);
            FileHandle h2 = new FileHandle(drive + @":\drivemappingtest.txt");

            Assert.AreEqual(h1, h2);

            Assert.True(DriveMapping.RemoveLocalFolder(drive, null));

            Assert.False(DriveMapping.DriveExists(drive));

            // cleanup the the test file
            FileUtility.DeleteFile(testFilePath);
        }

        [TestCase]
        public void Can_map_and_unmap_a_network_folder()
        {
            // find a free drive...
            char drive = 'A';

            while (DriveMapping.DriveExists(drive))
            {
                ++drive;
            }

            // test that removing a non-existent drive fails gracefully
            Assert.True(DriveMapping.RemoveNetworkDrive(drive));

            Assert.True(DriveMapping.MapNetworkDrive(drive, TestManager.UncSharedFolder));

            Assert.True(DriveMapping.DriveExists(drive));

            // confirm that if we access the test file by either path, we are actually looking at the same file
            FileHandle h1 = new FileHandle(TestManager.UncSharedFolder + @"\init_cad.h");
            FileHandle h2 = new FileHandle(drive + @":\init_cad.h");

            Assert.AreEqual(h1, h2);

            Assert.True(DriveMapping.RemoveLocalFolder(drive, null));

            Assert.False(DriveMapping.DriveExists(drive));
        }

    }
}
