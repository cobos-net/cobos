// ----------------------------------------------------------------------------
// <copyright file="AssemblyExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;
    using Cobos.Utilities.IO;

    /// <summary>
    /// Extension methods for the <see cref="Assembly"/> class.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets all Types in the assembly marked with the custom attribute.
        /// </summary>
        /// <typeparam name="T">The Attribute type to look for.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>A list of all types marked with the attribute.</returns>
        public static List<Type> GetAllTypesWithCustomAttribute<T>(this Assembly self)
            where T : Attribute
        {
            var result = new List<Type>();

            foreach (Type type in self.GetTypes())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                object[] attributes = type.GetCustomAttributes(typeof(T), true);

                if (attributes.Length > 0)
                {
                    result.Add(type);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the named type.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="typeName">The name of the type to find.</param>
        /// <returns>The type if found; otherwise null.</returns>
        public static Type GetType(this Assembly self, string typeName)
        {
            foreach (Type type in self.GetTypes())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (string.Compare(type.Name, typeName, true) == 0
                    || string.Compare(type.FullName, typeName, true) == 0)
                {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Loads the assembly at the given path.
        /// </summary>
        /// <param name="path">The path of the assembly.</param>
        /// <returns>The loaded assembly.</returns>
        /// <exception cref="ArgumentException">Throws if the path is invalid.</exception>
        public static Assembly LoadAssembly(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Environment.CurrentDirectory, path);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The assembly path does not exist: " + path);
            }

            var perm = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            perm.Assert();

            var assemblyName = AssemblyName.GetAssemblyName(path);

            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName() == assemblyName);

            if (assembly == null)
            {
                assembly = Assembly.Load(assemblyName);
            }

            if (assembly == null)
            {
                throw new ArgumentException("Could not load the assembly: " + path);
            }

            return assembly;
        }

        /// <summary>
        /// Loads an assembly from a relative path to the calling assembly.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The loaded assembly.</returns>
        /// <exception cref="ArgumentException">Throws if the path is invalid.</exception>
        public static Assembly LoadAssemblyRelativeToThis(string relativePath)
        {
            var relativeTo = Assembly.GetCallingAssembly();

            var resolver = new PathResolver(relativeTo.Location, null);

            var path = resolver.FindFilePath(relativePath);

            if (path == null)
            {
                throw new FileNotFoundException("Cannot find the relative assembly: " + relativePath);
            }

            return LoadAssembly(path.ToString());
        }
    }
}
