// ----------------------------------------------------------------------------
// <copyright file="PathResolverTests.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Tests.File
{
    using System;
    using Cobos.Utilities.File;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="PathResolver"/> class.
    /// </summary>
    [TestFixture]
    public class PathResolverTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test with valid folders.
        /// </summary>
        [TestCase]
        public void Construction_with_valid_folders_works()
        {
            Assert.DoesNotThrow(delegate { PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, null); });
            Assert.DoesNotThrow(delegate { PathResolver resolver = new PathResolver(TestManager.TestFilesLocation + @"/testfile.txt", null); });

            // fully qualified look in folder
            Assert.DoesNotThrow(() =>
                {
                    PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { TestManager.TestFilesLocation + "/TestDirectory" });
                    Assert.AreEqual(resolver.LookInFolders.Count, 2);
                });

            // Relative look in folder
            Assert.DoesNotThrow(() =>
                {
                    PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { "TestDirectory" });
                    Assert.AreEqual(resolver.LookInFolders.Count, 2);
                });
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test invalid root folder fails.
        /// </summary>
        [TestCase]
        public void Construction_with_invalid_root_fails()
        {
            Assert.Throws<Exception>(() => 
            { 
                PathResolver resolver = new PathResolver(@"C:\totally\madeup\folder\location", null); 
            });
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test invalid look-in folders are ignored.
        /// </summary>
        [TestCase]
        public void Invalid_look_in_folders_succeeds()
        {
            PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"C:\totally\madeup\folder\location" });
            Assert.AreEqual(resolver.LookInFolders.Count, 1);

            resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"\madeup\folder\location" });
            Assert.AreEqual(resolver.LookInFolders.Count, 1);

            // even though we supply a file, it should resolve the lookin folder correctly
            resolver = new PathResolver(TestManager.TestFilesLocation + @"\testfile.txt", new string[] { TestManager.TestFilesLocation + "/TestDirectory/TestRelativeFile.txt" });
            Assert.AreEqual(resolver.LookInFolders.Count, 2);

            Assert.AreEqual(resolver.LookInFolders[0], new NormalisedPath(TestManager.TestFilesLocation));
            Assert.AreEqual(resolver.LookInFolders[1], new NormalisedPath(TestManager.TestFilesLocation + "/TestDirectory/"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test both absolute and relative paths for look-in folders.
        /// </summary>
        [TestCase]
        public void Relative_and_absolute_lookin_folders_succeeds()
        {
            PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"\TestDirectory" });
            Assert.AreEqual(resolver.LookInFolders.Count, 2);

            resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"\TestDirectory", @"TestDirectory2/TestDirectory3/" });
            Assert.AreEqual(resolver.LookInFolders.Count, 3);

            resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"\TestDirectory", TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3/" });
            Assert.AreEqual(resolver.LookInFolders.Count, 3);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that valid files can be found.
        /// </summary>
        [TestCase]
        public void Can_resolve_valid_files()
        {
            PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"\TestDirectory", TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3/" });
            Assert.AreEqual(resolver.LookInFolders.Count, 3);

            NormalisedPath found = resolver.FindFilePath("testFILE.txt");
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath("testFile.txt", TestManager.TestFilesLocation));

            found = resolver.FindFilePath("TestRelativeFile.txt");
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath("TESTRELATIVEFILE.txt", TestManager.TestFilesLocation + @"/TestDirectory/"));

            found = resolver.FindFilePath("TestDeepNestedFile.txt");
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3\TESTDEEPNESTEDFILE.txt"));

            // TestDirectory2 is not included in the lookin folders
            found = resolver.FindFilePath("Another test file.txt");
            Assert.Null(found);

            // but should still be able find it relative to other lookin folders
            found = resolver.FindFilePath(@"./TestDirectory2\Another test file.txt");  // root folder
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2/ANOTHER TEST FILE.TXT"));

            found = resolver.FindFilePath(@"../TestDirectory2\Another test file.txt"); // testdirectory
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2/ANOTHER TEST FILE.TXT"));

            found = resolver.FindFilePath(@"..\ANOTHER TEST FILE.TXT"); // testdirectory3
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2/ANOTHER TEST FILE.TXT"));

            // finds fully qualified paths without attempting to use the lookin folders
            found = resolver.FindFilePath(TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3\TESTDEEPNESTEDFILE.txt");
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3\TESTDEEPNESTEDFILE.txt"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that invalid files are not found.
        /// </summary>
        [TestCase]
        public void Doesnt_resolve_invalid_files()
        {
            PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"\TestDirectory", TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3/" });
            Assert.AreEqual(resolver.LookInFolders.Count, 3);

            NormalisedPath found = resolver.FindFilePath("totally made up file.txt");
            Assert.Null(found);

            found = resolver.FindFilePath(@"C:\totally\madeup\folder\location\file.txt");
            Assert.Null(found);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that valid folders can be found.
        /// </summary>
        [TestCase]
        public void Can_resolve_valid_folders()
        {
            PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"\TestDirectory" });
            Assert.AreEqual(resolver.LookInFolders.Count, 2);

            NormalisedPath found = resolver.FindFolderPath("TestDirectory2");
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2"));

            // TestDirectory3 is not included in the lookin folders
            found = resolver.FindFolderPath("TestDirectory3");
            Assert.Null(found);

            // but should still be able find it relative to other lookin folders
            found = resolver.FindFolderPath(@"./TestDirectory2\TestDirectory3");  // root folder
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3"));

            found = resolver.FindFolderPath(@"../TestDirectory2\TestDirectory3"); // testdirectory
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3"));

            // resolves folder paths even when given a file
            found = resolver.FindFolderPath(@"..\TestDirectory2/ANOTHER TEST FILE.TXT"); // testdirectory
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TESTDIRECTORY2/"));

            // finds fully qualified paths without attempting to use the lookin folders
            found = resolver.FindFolderPath(TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3");
            Assert.NotNull(found);
            Assert.AreEqual(found, new NormalisedPath(TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3"));
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Test that invalid folders are not found.
        /// </summary>
        [TestCase]
        public void Doesnt_resolve_invalid_folders()
        {
            PathResolver resolver = new PathResolver(TestManager.TestFilesLocation, new string[] { @"\TestDirectory", TestManager.TestFilesLocation + @"/TestDirectory2/TestDirectory3/" });
            Assert.AreEqual(resolver.LookInFolders.Count, 3);

            NormalisedPath found = resolver.FindFilePath("totally made up folder");
            Assert.Null(found);

            found = resolver.FindFilePath(@"C:\totally\madeup\folder\location");
            Assert.Null(found);

            found = resolver.FindFilePath(@"C:\totally\madeup\folder\location\file.txt");
            Assert.Null(found);
        }
    }
}
