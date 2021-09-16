// ----------------------------------------------------------------------------
// <copyright file="DriveMappingTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.IO
{
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
