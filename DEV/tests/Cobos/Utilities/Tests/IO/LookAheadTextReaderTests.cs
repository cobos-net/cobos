﻿// ----------------------------------------------------------------------------
// <copyright file="LookAheadTextReaderTests.cs" company="Cobos SDK">
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
    /// Tests for the LookAheadTextReader class.
    /// </summary>
    [TestClass]
    public class LookAheadTextReaderTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a sample text file with known input.
        /// 2. Read the file using the LookAheadTextReader.
        /// 3. Confirm the file is read back as expected.
        /// </summary>
        [TestMethod]
        public void Can_read_file_successfully()
        {
            string fileName = Path.Combine(TestManager.TestFilesLocation, "LookAheadTextReaderTests.txt");

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // write some standard lines
                writer.WriteLine("Line 1");
                writer.WriteLine("Line 2");

                // write some whitespace
                writer.WriteLine(string.Empty);
                writer.WriteLine("     ");
                writer.WriteLine("\t\t\t");
                writer.WriteLine(" \t \t \t ");
                writer.WriteLine("\t \t \t");
                
                // write a good line
                writer.WriteLine("Line 3");
            }

            using (LookAheadTextReader reader = new LookAheadTextReader(fileName))
            {
                Assert.IsFalse(reader.EOF);
                Assert.AreEqual(0, reader.LineNumber);
                Assert.AreEqual("Line 1", reader.PeekLine());
                Assert.IsTrue(reader.PeekLineIsNotWhitespace());
                Assert.AreEqual("Line 1", reader.NextLine());

                Assert.IsFalse(reader.EOF);
                Assert.AreEqual(1, reader.LineNumber);
                Assert.AreEqual("Line 2", reader.PeekLine());
                Assert.IsTrue(reader.PeekLineIsNotWhitespace());
                Assert.AreEqual("Line 2", reader.NextLine());

                Assert.IsFalse(reader.EOF);
                Assert.AreEqual(2, reader.LineNumber);
                Assert.IsFalse(reader.PeekLineIsNotWhitespace());
                Assert.IsNotNull(reader.NextLine());

                Assert.IsFalse(reader.EOF);
                Assert.AreEqual(3, reader.LineNumber);
                Assert.IsFalse(reader.PeekLineIsNotWhitespace());
                Assert.IsNotNull(reader.NextLine());

                Assert.IsFalse(reader.EOF);
                Assert.AreEqual(4, reader.LineNumber);
                Assert.IsFalse(reader.PeekLineIsNotWhitespace());
                Assert.IsNotNull(reader.NextLine());

                Assert.IsFalse(reader.EOF);
                Assert.AreEqual(5, reader.LineNumber);
                Assert.IsFalse(reader.PeekLineIsNotWhitespace());
                Assert.IsNotNull(reader.NextLine());
                
                Assert.IsFalse(reader.EOF);
                Assert.AreEqual(6, reader.LineNumber);
                Assert.IsFalse(reader.PeekLineIsNotWhitespace());
                Assert.IsNotNull(reader.NextLine());

                Assert.IsFalse(reader.EOF);
                Assert.AreEqual(7, reader.LineNumber);
                Assert.AreEqual("Line 3", reader.PeekLine());
                Assert.IsTrue(reader.PeekLineIsNotWhitespace());
                Assert.AreEqual("Line 3", reader.NextLine());

                Assert.IsTrue(reader.EOF);
                Assert.AreEqual(8, reader.LineNumber);
                Assert.IsNull(reader.PeekLine());
                Assert.IsNull(reader.NextLine());

                // Test reading lines past the end is harmless and just returns null.
                for (int i = 0; i < 3; ++i)
                {
                    Assert.IsNull(reader.PeekLine());
                    Assert.IsNull(reader.NextLine());
                    Assert.IsFalse(reader.PeekLineIsNotWhitespace());
                }
            }

            File.Delete(fileName);
        }
    }
}