// ----------------------------------------------------------------------------
// <copyright file="LookaheadTextReader.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.IO
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Utility class to allow us to "peek" the next line.
    /// Works around the limitation that the text reader is not seek-able.
    /// </summary>
    public partial class LookAheadTextReader : IDisposable
    {
        /// <summary>
        /// Cache the read line, so we can use check the contents before using it.
        /// </summary>
        private string line;

        /// <summary>
        /// Set the current line number.
        /// </summary>
        private int lineNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookAheadTextReader"/> class.
        /// </summary>
        /// <param name="path">The path to the text file to read.</param>
        public LookAheadTextReader(string path)
        {
            this.Reader = File.OpenText(path);
            this.line = this.Reader.ReadLine();
        }

        /// <summary>
        /// Gets the underlying TextReader object.
        /// </summary>
        public TextReader Reader { get; }

        /// <summary>
        /// Gets a value indicating whether this stream is EOF.
        /// </summary>
        public bool EOF
        {
            get
            {
                return this.line == null;
            }
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return this.lineNumber;
            }
        }

        /// <summary>
        /// Gets the next line from the stream and returns the previously cached line.
        /// </summary>
        /// <returns>The previous text line.</returns>
        public string NextLine()
        {
            string last = this.line;
            this.line = this.Reader.ReadLine();
            ++this.lineNumber;
            return last;
        }

        /// <summary>
        /// Look at the currently cached line.
        /// </summary>
        /// <returns>The cached line.</returns>
        public string PeekLine()
        {
            return this.line;
        }

        /// <summary>
        /// Peek the current line to test whether it has data.
        /// </summary>
        /// <returns>True if there is data, false if the string is null empty or full of whitespace characters.</returns>
        public bool PeekLineIsNotWhitespace()
        {
            if (string.IsNullOrEmpty(this.line))
            {
                return false;
            }

            return Regex.IsMatch(this.line, @"\S");
        }
    }

    /// <summary>
    /// Implements the IDisposable interface.
    /// </summary>
    public partial class LookAheadTextReader : IDisposable
    {
        /// <summary>
        /// Is this object disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Finalizes an instance of the <see cref="LookAheadTextReader"/> class.
        /// </summary>
        ~LookAheadTextReader()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose this object.
        /// </summary>
        /// <param name="disposing">Is this called from Dispose or Finalize.</param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // free managed resources
                this.Reader.Dispose();

                GC.SuppressFinalize(this);
            }

            // free unmanaged resources
            this.disposed = true;
        }
    }
}
