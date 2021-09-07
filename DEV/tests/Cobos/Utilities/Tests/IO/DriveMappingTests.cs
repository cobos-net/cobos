// ----------------------------------------------------------------------------
// <copyright file="DriveMappingTests.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
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

namespace Cobos.Utilities.Tests.IO
{
    using System;
    using System.IO;
    using Cobos.Utilities.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SysFile = System.IO.File;

    /// <summary>
    /// Unit Tests for the <see cref="DriveMapping"/> class.
    /// </summary>
    [TestClass]
    public class DriveMappingTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that we can map and un-map a local folder.
        /// </summary>
        [TestMethod]
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
            Assert.IsTrue(DriveMapping.RemoveLocalFolder(drive, null));

            Assert.IsTrue(DriveMapping.MapLocalFolder(drive, TestManager.TestFilesLocation));

            Assert.IsTrue(DriveMapping.DriveExists(drive));

            // confirm that if we access the test file by either path, we are actually looking at the same file
            FileHandle h1 = new FileHandle(testFilePath);
            FileHandle h2 = new FileHandle(drive + @":\drivemappingtest.txt");

            Assert.AreEqual(h1, h2);

            Assert.IsTrue(DriveMapping.RemoveLocalFolder(drive, null));

            Assert.IsFalse(DriveMapping.DriveExists(drive));

            // cleanup the the test file
            FileUtility.DeleteFile(testFilePath);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that we can map and un-map a network folder.
        /// </summary>
        [TestMethod]
        public void Can_map_and_unmap_a_network_folder()
        {
            if (TestManager.UncSharedFolder == @"\\<not_set>")
            {
                Assert.Inconclusive("Requires a UNC share to be specified.");
            }

            // find a free drive...
            char drive = 'A';

            while (DriveMapping.DriveExists(drive))
            {
                ++drive;
            }

            // test that removing a non-existent drive fails gracefully
            Assert.IsTrue(DriveMapping.RemoveNetworkDrive(drive));

            Assert.IsTrue(DriveMapping.MapNetworkDrive(drive, TestManager.UncSharedFolder));

            Assert.IsTrue(DriveMapping.DriveExists(drive));

            // confirm that if we access the test file by either path, we are actually looking at the same file
            FileHandle h1 = new FileHandle(TestManager.UncSharedFolder + @"\test_file.h");
            FileHandle h2 = new FileHandle(drive + @":\test_file.h");

            Assert.AreEqual(h1, h2);

            Assert.IsTrue(DriveMapping.RemoveLocalFolder(drive, null));

            Assert.IsFalse(DriveMapping.DriveExists(drive));
        }
    }
}
