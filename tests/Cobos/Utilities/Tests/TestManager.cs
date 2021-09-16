// ----------------------------------------------------------------------------
// <copyright file="TestManager.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
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
                return Path.GetDirectoryName(sf.GetFileName()) + @"\TestData\";
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
