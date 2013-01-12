
namespace Cobos.Utilities.IO
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Utility class to allow us to "peek" the next line.  
    /// Works around the limitation that the text reader is not seek-able.
    /// </summary>
    public partial class LookaheadTextReader : IDisposable
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
        /// 
        /// </summary>
        /// <param name="path"></param>
        public LookaheadTextReader(string path)
        {
            this.Reader = File.OpenText(path);
            this.line = Reader.ReadLine();
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
        /// Test for the end of the stream
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
        /// <returns></returns>
        public string NextLine()
        {
            string last = this.line;
            this.line = Reader.ReadLine();
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
        /// 
        /// </summary>
        /// <returns>True if there is data, false if the string is null empty or full of whitespace characters.</returns>
        public bool PeekLineHasData()
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
    public partial class LookaheadTextReader : IDisposable
    {
        /// <summary>
        /// Is this object disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Finalizes an instance of the LookaheadTextReader class.
        /// </summary>
        ~LookaheadTextReader()
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
                Reader.Dispose();

                GC.SuppressFinalize(this);
            }

            // free unmanaged resources
            this.disposed = true;
        }
    }
}
