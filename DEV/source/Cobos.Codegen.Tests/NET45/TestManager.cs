// ----------------------------------------------------------------------------
// <copyright file="TestManager.cs" company="Cobos SDK">
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

namespace Cobos.Codegen.Tests
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using Cobos.Data;
    using Cobos.Utilities.Xml;
    using NUnit.Framework;

    /// <summary>
    /// Manages test data.
    /// </summary>
    public static class TestManager
    {
        /// <summary>
        /// Gets the data sources for testing.
        /// </summary>
        public static IEnumerable DataSource
        {
            get
            {
                yield return DatabaseAdapterFactory.Instance.TryCreate("MySQL", "Server=localhost;Database=Northwind;Uid=root;Pwd=mysql;");
            }
        }

        /// <summary>
        /// Serialize an entity to file.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to serialize.</param>
        public static void Serialize<T>(T entity)
        {
            const string Folder = @"C:\temp\";

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

            var filePath = Folder + name + ".xml";

            if (File.Exists(filePath) == true)
            {
                File.Delete(filePath);
            }

            Assert.DoesNotThrow(() =>
            {
                using (var writer = new StreamWriter(filePath))
                {
                    writer.Write(DataContractHelper.Serialize<T>(entity, new Type[0]));
                }
            });

            Assert.True(File.Exists(filePath));
        }
    }
}
