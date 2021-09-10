// ----------------------------------------------------------------------------
// <copyright file="FileHandleTests.cs" company="Cobos SDK">
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

    /// <summary>
    /// Unit Tests for the <see cref="FileHandle"/> class.
    /// </summary>
    [TestClass]
    public class FileHandleTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that we can create a handle for an existing file.
        /// </summary>
        [TestMethod]
        public void File_handle_for_existing_file_succeeds()
        {
            new FileHandle(TestManager.TestFilesLocation + @"\TestFile.txt");
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that we cannot create a handle for a file that doesn't exist.
        /// </summary>
        [TestMethod]
        public void File_handle_for_non_existing_file_fails()
        {
            Assert.ThrowsException<DirectoryNotFoundException>(() => new FileHandle(@"C:\totally\madeup\file\location.txt"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that we can create a handle for an existing folder.
        /// </summary>
        [TestMethod]
        public void File_handle_for_existing_folder_succeeds()
        {
            new FileHandle(TestManager.TestFilesLocation + @"\TestDirectory");
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that we cannot create a handle for a folder that doesn't exist.
        /// </summary>
        [TestMethod]
        public void File_handle_for_non_existing_folder_fails()
        {
            Assert.ThrowsException<DirectoryNotFoundException>(() => new FileHandle(@"C:\totally\madeup\folder\location"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that file handles correctly identify the same file even when the path is expressed differently.
        /// </summary>
        [TestMethod]
        public void File_handle_comparison_for_same_file_but_different_path_succeeds()
        {
            string path1 = TestManager.TestFilesLocation + @"\TestFile.txt";
            string path2 = TestManager.TestFilesLocation + @"\..\TestData\TestFile.txt";

            // jumble the paths up a bit
            path1 = path1.ToLower();
            path1 = path1.Replace('\\', '/');
            path2 = path2.ToUpper();

            FileHandle handle1 = new FileHandle(path1);
            FileHandle handle2 = new FileHandle(path2);

            Assert.IsTrue(handle1.CompareTo(handle2) == 0);
            Assert.IsTrue(handle2.CompareTo(handle1) == 0);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that file handles for different files are not considered the same.
        /// </summary>
        [TestMethod]
        public void File_handle_comparison_for_different_files_fails()
        {
            string path1 = TestManager.TestFilesLocation + @"\TestFile.txt";
            string path2 = TestManager.TestFilesLocation + @"\TestFile2.txt";

            FileHandle handle1 = new FileHandle(path1);
            FileHandle handle2 = new FileHandle(path2);

            Assert.IsFalse(handle1.CompareTo(handle2) == 0);
            Assert.IsFalse(handle2.CompareTo(handle1) == 0);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that file handles correctly identify the same folder even when the path is expressed differently.
        /// </summary>
        [TestMethod]
        public void File_handle_comparison_for_same_folder_but_different_path_succeeds()
        {
            string path1 = TestManager.TestFilesLocation + @"\TestDirectory";
            string path2 = TestManager.TestFilesLocation + @"\..\TestData\TestDirectory";

            // jumble the paths up a bit
            path1 = path1.ToLower();
            path1 = path1.Replace('\\', '/');
            path2 = path2.ToUpper();

            FileHandle handle1 = new FileHandle(path1);
            FileHandle handle2 = new FileHandle(path2);

            Assert.IsTrue(handle1.CompareTo(handle2) == 0);
            Assert.IsTrue(handle2.CompareTo(handle1) == 0);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that file handles for different folders are not considered the same.
        /// </summary>
        [TestMethod]
        public void File_handle_comparison_for_different_folder_fails()
        {
            string path1 = TestManager.TestFilesLocation + @"\TestDirectory";
            string path2 = TestManager.TestFilesLocation + @"\TestDirectory2";

            FileHandle handle1 = new FileHandle(path1);
            FileHandle handle2 = new FileHandle(path2);

            Assert.IsFalse(handle1.CompareTo(handle2) == 0);
            Assert.IsFalse(handle2.CompareTo(handle1) == 0);
        }
    }
}
