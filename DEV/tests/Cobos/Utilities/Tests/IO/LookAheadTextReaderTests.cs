// ----------------------------------------------------------------------------
// <copyright file="LookAheadTextReaderTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.IO
{
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
