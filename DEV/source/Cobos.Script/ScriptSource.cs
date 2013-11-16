// ----------------------------------------------------------------------------
// <copyright file="ScriptSource.cs" company="Cobos SDK">
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

namespace Cobos.Script
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
    using Microsoft.CSharp;

    /// <summary>
    /// Represents a script source file.
    /// </summary>
    public class ScriptSource
    {
        /// <summary>
        /// The script source file path.
        /// </summary>
        public readonly string SourcePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptSource"/> class.
        /// </summary>
        /// <param name="sourcePath">The script source file path.</param>
        public ScriptSource(string sourcePath)
        {
            if (!Path.IsPathRooted(sourcePath))
            {
                sourcePath = Path.Combine(Environment.CurrentDirectory, sourcePath);
            }

            this.SourcePath = sourcePath;
        }

        /// <summary>
        /// Compile the script into an assembly.
        /// </summary>
        /// <returns>An assembly containing the compiled byte code.</returns>
        public ScriptAssembly Compile()
        {
            if (!File.Exists(this.SourcePath))
            {
                throw new ScriptException("The script path does not exist: {0}", this.SourcePath);
            }

            ScriptTrace.Instance.TraceInformation("Compiling script {0}", this.SourcePath);

            // resolve the source and compiled assembly path
            string scriptSource = File.ReadAllText(this.SourcePath);
            string assemblyPath = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(this.SourcePath) + ".dll");

            // check for modifications
            if (File.Exists(assemblyPath))
            {
                if (File.GetLastWriteTime(this.SourcePath) < File.GetLastWriteTime(assemblyPath))
                {
                    ScriptTrace.Instance.TraceInformation("No modifications detected, skipping compilation");
                    return new ScriptAssembly(assemblyPath);
                }
            }

            ScriptTrace.Instance.TraceInformation("Starting compilation...");

            CSharpCodeProvider compiler = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });

            CompilerParameters parameters = this.GetParameters(assemblyPath);

            CompilerResults results = compiler.CompileAssemblyFromSource(parameters, scriptSource);

            this.CheckResults(results);

            ScriptTrace.Instance.TraceInformation("Compilation complete");

            return new ScriptAssembly(assemblyPath);
        }

        /// <summary>
        /// Get all of the parameters for the compiler.
        /// </summary>
        /// <param name="assemblyPath">The path of the output assembly.</param>
        /// <returns>An object representing the compiler parameters.</returns>
        private CompilerParameters GetParameters(string assemblyPath)
        {
            // find all referenced assemblies in app.config
            string[] references = ((string)ConfigurationManager.AppSettings["AssemblyReferences"]).Split(' ');

            // initialise the compiler parameters
            CompilerParameters parameters = new CompilerParameters(references, assemblyPath, true);
            parameters.GenerateExecutable = false;

            ScriptTrace.Instance.TraceInformation("Resolving all assembly references... (this may take some time)");

            // resolve paths to all referenced assemblies
            List<string> paths = AssemblyReferenceResolver.Resolve(references);

            if (paths != null)
            {
                StringBuilder buffer = new StringBuilder(1024);

                foreach (string path in paths)
                {
                    // /lib: parameters are cumulative
                    buffer.AppendFormat("/lib:\"{0}\" ", path);
                }

                parameters.CompilerOptions = buffer.ToString();

                ScriptTrace.Instance.TraceEvent(TraceEventType.Verbose, 0, "Compiler Options: {0}", parameters.CompilerOptions);
            }

            return parameters;
        }

        /// <summary>
        /// Check the results after compilation.
        /// </summary>
        /// <param name="results">The compiler results.</param>
        private void CheckResults(CompilerResults results)
        {
            // check for errors, if any then abort the script runtime
            if (results.Errors.HasErrors)
            {
                string errorMessage = string.Empty;
                results.Errors.Cast<CompilerError>().ToList().ForEach(error => errorMessage += error.ErrorText + "\n");

                throw new ScriptException("Failed to compile the script {0}:\n{1}", this.SourcePath, errorMessage);
            }

            // check for warnings, just report these to the console
            if (results.Errors.HasWarnings)
            {
                results.Errors.Cast<CompilerError>().ToList().ForEach(error => ScriptTrace.Instance.TraceEvent(TraceEventType.Warning, 0, error.ErrorText));
            }
        }
    }
}
