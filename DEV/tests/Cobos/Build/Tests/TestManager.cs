// ----------------------------------------------------------------------------
// <copyright file="TestManager.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Build.Tests
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Manages test data.
    /// </summary>
    public static class TestManager
    {
        /// <summary>
        /// Gets the data sources for testing.
        /// </summary>
        public static IEnumerable<(string ProviderName, string ConnectionString)> DataSource
        {
            get
            {
                foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
                {
                    yield return (connection.ProviderName, connection.ConnectionString);
                }
            }
        }

        /// <summary>
        /// Gets the location of the test files.
        /// </summary>
        public static string TestFiles
        {
            get
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                return Path.GetDirectoryName(sf.GetFileName()) + @"\TestData\";
            }
        }
    }
}
