// ----------------------------------------------------------------------------
// <copyright file="AssemblyGenerator.cs" company="Cobos SDK">
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
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Cobos.Utilities.IO;
    using Microsoft.CSharp;

    /// <summary>
    /// Generates and assembly from source files.
    /// </summary>
    public static class AssemblyGenerator
    {
        /// <summary>
        /// The compiler version.
        /// </summary>
#if NET35
        private const string CompilerVersion = "v3.5";
#elif NET45
        private const string CompilerVersion = "v4.5";
#endif
        
        /// <summary>
        /// Build the source files into an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="sourceFiles">The source files to build.</param>
        /// <param name="references">The assembly references.</param>
        /// <param name="searchPaths">The paths to look in to resolve references.</param>
        /// <returns>An assembly containing the compiled byte code.</returns>
        public static AssemblyRef Build(string assemblyName, string[] sourceFiles, string[] references, string[] searchPaths)
        {
            string assemblyPath = AssemblyRef.ResolvePath(assemblyName);

            // check for modifications
            if (File.Exists(assemblyPath))
            {
                var lastWrite = File.GetLastWriteTime(assemblyPath);

                if (sourceFiles.All(s => File.GetLastWriteTime(s) < lastWrite))
                {
                    return new AssemblyRef(assemblyPath);
                }
            }

            var compiler = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", CompilerVersion } });

            var parameters = GetParameters(assemblyPath, references, searchPaths);

            var results = compiler.CompileAssemblyFromSource(parameters, GetSourceCode(sourceFiles));

            CheckResults(results);

            return new AssemblyRef(assemblyPath);
        }

        /// <summary>
        /// Get the source code from the source files.
        /// </summary>
        /// <param name="sourceFiles">The source files to load.</param>
        /// <returns>The loaded source code.</returns>
        public static string[] GetSourceCode(string[] sourceFiles)
        {
            var result = new string[sourceFiles.Length];

            for (int s = 0; s < sourceFiles.Length; ++s)
            {
                if (File.Exists(sourceFiles[s]) == false)
                {
                    throw new ArgumentException("The source file doesn't exist: " + sourceFiles[s]);
                }

                result[s] = File.ReadAllText(sourceFiles[s]);
            }

            return result;
        }

        /// <summary>
        /// Get all of the parameters for the compiler.
        /// </summary>
        /// <param name="assemblyPath">The path of the output assembly.</param>
        /// <param name="references">The assembly references.</param>
        /// <param name="searchPaths">The paths to look in to resolve references.</param>
        /// <returns>An object representing the compiler parameters.</returns>
        private static CompilerParameters GetParameters(string assemblyPath, string[] references, string[] searchPaths)
        {
            // initialise the compiler parameters
            CompilerParameters parameters = new CompilerParameters(references, assemblyPath, true);
            parameters.GenerateExecutable = false;

            // resolve paths to all referenced assemblies
            List<string> paths = AssemblyReferenceResolver.Resolve(references, searchPaths);

            if (paths != null)
            {
                StringBuilder buffer = new StringBuilder(1024);

                foreach (string path in paths)
                {
                    // /lib: parameters are cumulative
                    buffer.AppendFormat("/lib:\"{0}\" ", path);
                }

                parameters.CompilerOptions = buffer.ToString();
            }

            return parameters;
        }

        /// <summary>
        /// Check the results after compilation.
        /// </summary>
        /// <param name="results">The compiler results.</param>
        private static void CheckResults(CompilerResults results)
        {
            // check for errors, if any then abort the build
            if (results.Errors.HasErrors)
            {
                string errorMessage = string.Empty;
                results.Errors.Cast<CompilerError>().ToList().ForEach(error => errorMessage += error.ErrorText + "\r\n");
                throw new InvalidOperationException("Failed to build the assembly:\r\n" + errorMessage);
            }

            // check for warnings, just report these to the console
            if (results.Errors.HasWarnings)
            {
                string errorMessage = string.Empty;
                results.Errors.Cast<CompilerError>().ToList().ForEach(error => errorMessage += error.ErrorText + "\r\n");
                ////TBD - log warning messages
            }
        }
    }
}
