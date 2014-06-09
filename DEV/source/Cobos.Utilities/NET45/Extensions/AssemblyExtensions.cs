// ----------------------------------------------------------------------------
// <copyright file="AssemblyExtensions.cs" company="Cobos SDK">
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
        /// Loads the assembly at the given path.
        /// </summary>
        /// <param name="path">The path of the assembly.</param>
        /// <returns>The loaded assembly.</returns>
        /// <exception cref="ArgumentException">Throws if the path is invalid.</exception>
        public static Assembly LoadAssembly(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The assembly path does not exist: " + path);
            }

            var perm = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            perm.Assert();

            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(path));

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
