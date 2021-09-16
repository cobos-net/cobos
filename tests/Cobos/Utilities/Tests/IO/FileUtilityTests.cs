// ----------------------------------------------------------------------------
// <copyright file="FileUtilityTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.IO
{
    using System.IO;
    using Cobos.Utilities.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="FileUtility"/> class.
    /// </summary>
    [TestClass]
    public class FileUtilityTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test invalid folder throws exception.
        /// </summary>
        [TestMethod]
        public void Folder_is_folder_but_doesnt_exist()
        {
            Assert.ThrowsException<DirectoryNotFoundException>(() => FileUtility.IsFolder(@"C:\totally\madeup\folder\location"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test invalid file throws an exception.
        /// </summary>
        [TestMethod]
        public void File_is_file_but_doesnt_exist()
        {
            Assert.ThrowsException<DirectoryNotFoundException>(() => FileUtility.IsFile(@"C:\totally\madeup\file\location.txt"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that a valid file succeeds.
        /// </summary>
        [TestMethod]
        public void File_is_a_file()
        {
            Assert.IsTrue(FileUtility.IsFile(TestManager.TestFilesLocation + @"\TestFile.txt"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that a valid folder exists.
        /// </summary>
        [TestMethod]
        public void Folder_is_a_folder()
        {
            Assert.IsTrue(FileUtility.IsFolder(TestManager.TestFilesLocation + @"\TestDirectory"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that a folder is not mistaken as a file.
        /// </summary>
        [TestMethod]
        public void File_is_really_a_folder()
        {
            Assert.IsFalse(FileUtility.IsFile(TestManager.TestFilesLocation + @"\TestDirectory"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that a file is not mistaken as a folder.
        /// </summary>
        [TestMethod]
        public void Folder_is_really_a_file()
        {
            Assert.IsFalse(FileUtility.IsFolder(TestManager.TestFilesLocation + @"\TestFile.txt"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that two identical files are considered the same.
        /// </summary>
        [TestMethod]
        public void File_contents_are_the_same()
        {
            Assert.IsTrue(FileUtility.Md5Compare(TestManager.TestFilesLocation + @"\TestFile.txt", TestManager.TestFilesLocation + @"\TestFile2.txt"));
            Assert.IsTrue(FileUtility.BinaryCompare(TestManager.TestFilesLocation + @"\TestFile.txt", TestManager.TestFilesLocation + @"\TestFile2.txt"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that two different files are not considered the same.
        /// </summary>
        [TestMethod]
        public void File_contents_are_not_the_same()
        {
            Assert.IsFalse(FileUtility.Md5Compare(TestManager.TestFilesLocation + @"\TestFile3.txt", TestManager.TestFilesLocation + @"\TestFile4.txt"));
            Assert.IsFalse(FileUtility.BinaryCompare(TestManager.TestFilesLocation + @"\TestFile3.txt", TestManager.TestFilesLocation + @"\TestFile4.txt"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that absolute and relative paths are properly identified.
        /// </summary>
        [TestMethod]
        public void Can_differentiate_between_relative_and_absolute_paths()
        {
            Assert.IsTrue(FileUtility.IsAbsolutePath(@"C:\temp\madeup.txt"));
            Assert.IsTrue(FileUtility.IsAbsolutePath(TestManager.TestFilesLocation + @"\TestFile.txt"));
            Assert.IsTrue(FileUtility.IsAbsolutePath(@"\\madeup_server\madeup share"));
            Assert.IsTrue(FileUtility.IsAbsolutePath(@"\\madeup_server\madeup share\madeup file.txt"));
            Assert.IsFalse(FileUtility.IsAbsolutePath(@"\madeup\folder\"));
            Assert.IsFalse(FileUtility.IsAbsolutePath(@"\madeup\folder\test.txt"));
            Assert.IsFalse(FileUtility.IsAbsolutePath(@"\temp\test.txt"));
        }
    }
}
