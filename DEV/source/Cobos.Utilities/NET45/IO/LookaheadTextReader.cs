// ----------------------------------------------------------------------------
// <copyright file="LookAheadTextReader.cs" company="Cobos SDK">
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
        #region Fields

        /// <summary>
        /// Access the underlying TextReader object
        /// </summary>
        public readonly TextReader Reader;

        /// <summary>
        /// Cache the read line, so we can use check the contents before using it.
        /// </summary>
        private string line;

        /// <summary>
        /// Set the current line number.
        /// </summary>
        private int lineNumber;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the LookAheadTextReader class.
        /// </summary>
        /// <param name="path">The path to the text file to read.</param>
        public LookAheadTextReader(string path)
        {
            this.Reader = File.OpenText(path);
            this.line = this.Reader.ReadLine();
        }

        #endregion

        #region Finalizers
        #endregion

        #region Events
        #endregion

        #region Enums
        #endregion

        #region Interfaces
        #endregion

        #region Properties

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

        #endregion

        #region Indexers
        #endregion

        #region Methods

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

        #endregion

        #region Structs
        #endregion

        #region Classes
        #endregion
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
        /// Finalizes an instance of the LookAheadTextReader class.
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
