using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

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

				Logger.Instance.Debug( 0, "Compiler Options: {0}", parameters.CompilerOptions );
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
