// ----------------------------------------------------------------------------
// <copyright file="AssemblyRef.cs" company="Cobos SDK">
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

namespace Cobos.AssemblyGen
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Assembly loading methods.
    /// </summary>
    public class AssemblyRef
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyRef"/> class
        /// from an existing assembly.
        /// </summary>
        /// <param name="assembly">The assembly reference.</param>
        public AssemblyRef(Assembly assembly)
        {
            this.Instance = assembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyRef"/> class
        /// from the assembly path.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly.</param>
        public AssemblyRef(string assemblyPath)
        {
            this.Instance = Load(assemblyPath);
        }

        /// <summary>
        /// Gets the assembly instance.
        /// </summary>
        public Assembly Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Resolve the path to the assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>The resolved path.</returns>
        public static string ResolvePath(string assemblyName)
        {
            // resolve the source and compiled assembly path
            if (assemblyName.ToLower().EndsWith(".dll") == false)
            {
                assemblyName += ".dll";
            }

            if (Path.IsPathRooted(assemblyName) == false)
            {
                assemblyName = Path.Combine(Environment.CurrentDirectory, assemblyName);
            }

            return assemblyName;
        }

        /// <summary>
        /// Load the assembly.
        /// </summary>
        /// <param name="path">A full or relative assembly path.</param>
        /// <returns>The loaded assembly if found.</returns>
        public static Assembly Load(string path)
        {
            return AssemblyExtensions.LoadAssembly(path);
        }

        /// <summary>
        /// Invoke the named method on a type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="method">The name of the method.</param>
        /// <param name="arguments">The method arguments.</param>
        public void Invoke(string typeName, string method, KeyValuePair<string, object>[] arguments)
        {
            Type objectType = this.Instance.GetType(typeName);

            if (objectType == null)
            {
                throw new ArgumentException(string.Format("Failed to find the type {0} in the assembly: {1}", typeName, this.Instance.GetName().Name));
            }

            MethodInfo methodInfo = objectType.GetMethodCaseInsensitive(method);

            if (methodInfo == null)
            {
                throw new ArgumentException("Failed to find the method " + method);
            }

            object instance = Activator.CreateInstance(objectType);

            if (instance == null)
            {
                throw new InvalidOperationException("Failed to instantiate the type.");
            }

            object[] methodArgs = methodInfo.GetMethodArguments(arguments);

            try
            {
                methodInfo.Invoke(instance, methodArgs);
            }
            finally
            {
                if (instance is IDisposable)
                {
                    ((IDisposable)instance).Dispose();
                }
            }
        }
    }
}
