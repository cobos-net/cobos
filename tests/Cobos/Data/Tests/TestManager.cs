﻿// ----------------------------------------------------------------------------
// <copyright file="TestManager.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using Cobos.Utilities.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Manages test data.
    /// </summary>
    public static class TestManager
    {
        /// <summary>
        /// Gets the data sources for testing.
        /// </summary>
        public static IEnumerable<IDatabaseAdapter> DataSource
        {
            get
            {
                foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
                {
                    yield return DatabaseAdapterFactory.Instance.TryCreate(connection.ProviderName, connection.ConnectionString);
                }
            }
        }

        /// <summary>
        /// Gets the location of the project files.
        /// </summary>
        public static string ProjectDirectory
        {
            get
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                return Path.GetDirectoryName(sf.GetFileName()) + @"\";
            }
        }

        /// <summary>
        /// Gets the location of the test files.
        /// </summary>
        public static string TestFiles => ProjectDirectory + @"TestData\";

        /// <summary>
        /// Serialize an entity to file.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to serialize.</param>
        public static void Serialize<T>(T entity)
        {
            string name;

            if (typeof(T).IsGenericType == true)
            {
#if NET35
                name = typeof(T).GetGenericArguments()[0].Name;
#else
                name = typeof(T).GenericTypeArguments[0].Name;
#endif
            }
            else
            {
                name = typeof(T).Name;
            }

            var filePath = TestFiles + name + ".xml";

            if (File.Exists(filePath) == true)
            {
                File.Delete(filePath);
            }

            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(DataContractHelper.Serialize<T>(entity, new Type[0]));
            }

            Assert.IsTrue(File.Exists(filePath));
        }
    }
}
