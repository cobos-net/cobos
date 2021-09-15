// ----------------------------------------------------------------------------
// <copyright file="DatabaseAdapterFactory.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Cobos.Data.Adapters;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Registry of Database Adapter types.
    /// </summary>
    public sealed class DatabaseAdapterFactory
    {
        /// <summary>
        /// The singleton instance of the class.
        /// </summary>
        private static readonly Lazy<DatabaseAdapterFactory> InstanceValue = new Lazy<DatabaseAdapterFactory>(() => new DatabaseAdapterFactory());

        /// <summary>
        /// The registry instance.
        /// </summary>
        private readonly Dictionary<string, Type> registry = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Synchronize access to the registry.
        /// </summary>
        private readonly object registryLock = new object();

        /// <summary>
        /// Prevents a default instance of the <see cref="DatabaseAdapterFactory"/> class from being created.
        /// </summary>
        private DatabaseAdapterFactory()
        {
            this.Register<MySqlDatabaseAdapter>("MySQL");
            this.Register<OracleDatabaseAdapter>("Oracle");
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

            try
            {
                this.LoadFromAssembly(AssemblyExtensions.LoadAssemblyRelativeToThis(assemblyPath));
            }
            catch (System.IO.FileNotFoundException)
            {
                return;
            }
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static DatabaseAdapterFactory Instance
        {
            get
            {
                return InstanceValue.Value;
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
            var types = assembly.GetAllTypesWithCustomAttribute<DatabaseAdapterProviderAttribute>();

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is IDatabaseAdapterProvider provider)
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
