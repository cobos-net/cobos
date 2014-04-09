// ----------------------------------------------------------------------------
// <copyright file="TestManager.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Tests
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Manages Test Resources.
    /// </summary>
    public static class TestManager
    {
        /// <summary>
        /// Path to an accessible UNC location.
        /// </summary>
        public const string UncSharedFolder = @"\\<not_set>";

        /// <summary>
        /// Gets the location of the test files.
        /// </summary>
        public static string TestFilesLocation
        {
            get
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                return Path.GetDirectoryName(sf.GetFileName()) + @"\TestFiles\";
            }
        }

        /// <summary>
        /// Resolve the path to a test file using the relative path and filename.
        /// </summary>
        /// <param name="relative">The relative path and filename to resolve.</param>
        /// <returns>A fully qualified path for the test resource.</returns>
        public static string ResolvePath(string relative)
        {
            return Path.Combine(TestFilesLocation, relative);
        }
    }
}
