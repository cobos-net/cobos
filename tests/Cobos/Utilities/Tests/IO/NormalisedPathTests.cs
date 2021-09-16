// ----------------------------------------------------------------------------
// <copyright file="NormalisedPathTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.IO
{
    using Cobos.Utilities.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the <see cref="NormalisedPath"/> class.
    /// </summary>
    [TestClass]
    public class NormalisedPathTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that different expressions of the same actual path are recognized.
        /// </summary>
        [TestMethod]
        public void Same_normalised_file_paths_are_the_same()
        {
            string path1 = @"C:\file\  location  \\   \\  \\without\any\relative\paths\\\\\\\\\file.txt";
            string path2 = @"C:\file\.\location\  \\\\    .\without\\\\.   \.\any\relative\.\.\   .\.   \   .  \.\paths\.\file.txt";

            // jumble the paths up a bit
            path1 = path1.Replace('\\', '/');
            path2 = path2.ToUpper();

            NormalisedPath npath1 = new NormalisedPath(path1);
            NormalisedPath npath2 = new NormalisedPath(path2);

            Assert.IsTrue(npath1 == npath2);

            string path3 = @"C:\file\location\..\location\without\\ \ \\any\..\..\  without   \\\\\\\\\any\relative  \  paths\    ..\..   \   ..    \any\relative\paths\file.txt";
            string path4 = path3;

            NormalisedPath npath3 = new NormalisedPath(path3);

            Assert.IsTrue(npath1 == npath3);

            // jumble it up a bit
            path4 = path4.Replace('\\', '/');
            path4 = path4.ToUpper();

            NormalisedPath npath4 = new NormalisedPath(path4);

            Assert.IsTrue(npath1 == npath4);

            // compare them all...
            Assert.IsTrue(npath1 == npath2 && npath1 == npath3 && npath1 == npath4 && npath2 == npath3 && npath2 == npath4 && npath3 == npath4);

            // compare relative paths
            string folderpath = @"C:\file\  location  \\   \\  \\without\any\relative\paths";

            NormalisedPath rpath1 = new NormalisedPath("file.txt", folderpath);

            Assert.IsTrue(npath1 == rpath1);

            string path5 = @"..\..\..\any\relative\paths\file.txt";

            // jumble it up
            path5 = path5.Replace('\\', '/');
            path5 = path5.ToUpper();

            NormalisedPath rpath2 = new NormalisedPath(path5, folderpath);

            // compare them all
            Assert.IsTrue(rpath1 == rpath2 && npath1 == rpath2);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that all combinations of emptiness are properly recognized.
        /// </summary>
        [TestMethod]
        public void Can_handle_empty_paths()
        {
            var empty = new NormalisedPath(string.Empty);
            Assert.IsTrue(empty.IsNullPath);

            empty = new NormalisedPath("            ");
            Assert.IsTrue(empty.IsNullPath);

            empty = new NormalisedPath("\"\"");
            Assert.IsTrue(empty.IsNullPath);

            empty = new NormalisedPath("       \"      \"          ");
            Assert.IsTrue(empty.IsNullPath);

            empty = new NormalisedPath("       \"      \"          ");
            Assert.IsTrue(empty.IsNullPath);

            empty = new NormalisedPath("  \\  '   \"    \\  \"    \" \"  ' /   '  ");
            Assert.IsTrue(empty.IsNullPath);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Construct a convoluted path with multiple spaces, separators and .. directory paths.
        /// 2. Test that we can get the normalized path.
        /// 3. Test that we can get the filename, extension etc.
        /// </summary>
        [TestMethod]
        public void Can_get_path_components()
        {
            string path = @"C:\file\LOCATION\..\location\without\\ \ \\ANY\..\..\  without   \\\\\\\\\any\RELATIVE  \  pAtHs\    ..\..   \   ..    \any\relative\paths\file.txt";
            NormalisedPath npath = new NormalisedPath(path);

            Assert.AreEqual(@"c:\file\location\without\any\relative\paths", npath.GetDirectoryName().Value);
            Assert.AreEqual(@"file.txt", npath.GetFileName());
            Assert.AreEqual(@"file", npath.GetFileNameWithoutExtension());
            Assert.AreEqual(@".txt", npath.GetExtension());
        }
    }
}
