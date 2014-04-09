// ----------------------------------------------------------------------------
// <copyright file="DatabaseAdapterFactory.cs" company="Cobos SDK">
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

namespace Cobos.Data
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Cobos.Data.Adapters;
    using Cobos.Utilities.Extensions;
#if NET35
    using Cobos.Utilities.Wrappers;
#endif

    /// <summary>
    /// Registry of Database Adapter types.
    /// </summary>
    public sealed class DatabaseAdapterFactory
    {
        /// <summary>
        /// The singleton instance of the class.
        /// </summary>
        private static Lazy<DatabaseAdapterFactory> instance = new Lazy<DatabaseAdapterFactory>(() => new DatabaseAdapterFactory());

        /// <summary>
        /// The registry instance.
        /// </summary>
        private Dictionary<string, Type> registry = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Synchronize access to the registry.
        /// </summary>
        private object registryLock = new object();

        /// <summary>
        /// Prevents a default instance of the <see cref="DatabaseAdapterFactory"/> class from being created.
        /// </summary>
        private DatabaseAdapterFactory()
        {
            this.Register<MySqlDatabaseAdapter>("MySQL");
            this.Register<PostgreSqlDatabaseAdapter>("PostgreSQL");
            this.Register<SqlServerDatabaseAdapter>("SqlServer");

#if NET45
            var is64BitProcess = Environment.Is64BitProcess;
#else
            var is64BitProcess = IntPtr.Size == 8;
#endif

            string assemblyPath;

            if (is64BitProcess)
            {
                assemblyPath = "x64\\Cobos.Data.Adapters.x64.dll";
            }
            else
            {
                assemblyPath = "x86\\Cobos.Data.Adapters.x86.dll";
            }

            this.LoadFromAssembly(AssemblyExtensions.LoadAssemblyRelativeToThis(assemblyPath));
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static DatabaseAdapterFactory Instance
        {
            get
            {
                return instance.Value;
            }
        }

        /// <summary>
        /// Load all adapters from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        /// <remarks>
        /// The assembly is scanned for all <see cref="IDatabaseAdapterProvider"/> types
        /// to register the provided types from.
        /// </remarks>
        public void LoadFromAssembly(Assembly assembly)
        {
            var types = assembly.GetAllTypesWithAttribute<DatabaseAdapterProviderAttribute>();

            foreach (var type in types)
            {
                var provider = Activator.CreateInstance(type) as IDatabaseAdapterProvider;

                if (provider != null)
                {
                    provider.RegisterDatabaseAdapters(this);
                }
            }
        }

        /// <summary>
        /// Register a Database Adapter type.
        /// </summary>
        /// <typeparam name="T">The type of adapter.</typeparam>
        /// <param name="name">The name to register the type with.</param>
        public void Register<T>(string name)
            where T : IDatabaseAdapter
        {
            lock (this.registryLock)
            {
                if (this.registry.ContainsKey(name))
                {
                    throw new InvalidOperationException("An adapter with the name '" + name + "' is already registered.");
                }

                this.registry.Add(name, typeof(T));
            }
        }

        /// <summary>
        /// Creates an instance of the registered adapter.
        /// </summary>
        /// <param name="name">The adapter name.</param>
        /// <param name="connectionString">The connection string for new connections.</param>
        /// <returns>An object representing the adapter if found; otherwise null.</returns>
        public IDatabaseAdapter TryCreate(string name, string connectionString)
        {
            Type type = null;

            lock (this.registryLock)
            {
                if (this.registry.TryGetValue(name, out type) == false)
                {
                    return null;
                }
            }

            var adapter = (IDatabaseAdapter)Activator.CreateInstance(type);
            adapter.ConnectionString = connectionString;

            return adapter;
        }
    }
}
