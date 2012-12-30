// ============================================================================
// Filename: ScriptSource.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 21-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Cobos.Core.Log;

namespace Cobos.Script
{
	public class ScriptSource
	{
		/// <summary>
		/// 
		/// </summary>
		public readonly string SourcePath;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourcePath"></param>
		public ScriptSource( string sourcePath )
		{
			if ( !Path.IsPathRooted( sourcePath ) )
			{
				sourcePath = Path.Combine( Environment.CurrentDirectory, sourcePath );
			}

			SourcePath = sourcePath;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ScriptAssembly Compile()
		{
			if ( !File.Exists( SourcePath ) )
			{
				throw new ScriptException( "The script path does not exist: {0}", SourcePath );
			}

			Logger.Instance.Information( "Compiling script {0}", SourcePath );

			// resolve the source and compiled assembly path
			string scriptSource = File.ReadAllText( SourcePath );
			string assemblyPath = Path.Combine( Environment.CurrentDirectory, Path.GetFileNameWithoutExtension( SourcePath ) + ".dll" );

			// check for modifications
			if ( File.Exists( assemblyPath ) )
			{
				if ( File.GetLastWriteTime( SourcePath ) < File.GetLastWriteTime( assemblyPath ) )
				{
					Logger.Instance.Information( "No modifications detected, skipping compilation" );
					return new ScriptAssembly( assemblyPath );
				}
			}

			Logger.Instance.Information( "Starting compilation..." );

			CSharpCodeProvider compiler = new CSharpCodeProvider( new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } } );

			CompilerParameters parameters = GetParameters( assemblyPath );
	
			CompilerResults results = compiler.CompileAssemblyFromSource( parameters, scriptSource );

			CheckResults( results );

			Logger.Instance.Information( "Compilation complete" );

			return new ScriptAssembly( assemblyPath );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <returns></returns>
		private CompilerParameters GetParameters( string assemblyPath )
		{
			// find all referenced assemblies in app.config
			string[] references = ((string)ConfigurationManager.AppSettings[ "AssemblyReferences" ]).Split( ' ' );

			// initialise the compiler parameters
			
			CompilerParameters parameters = new CompilerParameters( references, assemblyPath, true );
			parameters.GenerateExecutable = false;

			Logger.Instance.Information( "Resolving all assembly references... (this may take some time)" );

			// resolve paths to all referenced assemblies

			List<string> paths = AssemblyReferenceResolver.Resolve( references );

			if ( paths != null )
			{
				StringBuilder buffer = new StringBuilder( 1024 );

				foreach ( string path in paths )
				{
					// /lib: parameters are cumulative
					buffer.AppendFormat( "/lib:\"{0}\" ", path );
				}

				parameters.CompilerOptions = buffer.ToString();

				Logger.Instance.Trace( "Compiler Options: {0}", parameters.CompilerOptions );
			}

			return parameters;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="results"></param>
		private void CheckResults( CompilerResults results )
		{
			// check for errors, if any then abort the script runtime
			if ( results.Errors.HasErrors )
			{
				string errorMessage = "";
				results.Errors.Cast<CompilerError>().ToList().ForEach( error => errorMessage += error.ErrorText + "\n" );

				throw new ScriptException( "Failed to compile the script {0}:\n{1}", SourcePath, errorMessage );
			}

			// check for warnings, just report these to the console
			if ( results.Errors.HasWarnings )
			{
				results.Errors.Cast<CompilerError>().ToList().ForEach( error => Logger.Instance.Warning( error.ErrorText ) );
			}
		}

	}
}
