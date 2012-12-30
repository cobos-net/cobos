// ============================================================================
// Filename: Program.cs
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
using System.Collections.Generic;
using System.Text;
using Cobos.Script;
using Cobos.Core.Log;
using NDesk.Options;

namespace Cobos.ScriptEngine
{
	class Program
	{
		static void Main( string[] args )
		{
			WriteHeader();

			if ( args.Length < 1 )
			{
				WriteHelp();
				return;
			}

            Logger.Instance.Initialise();

			string script = null, @class = null, method = null;
			bool help = false;

			var p = new OptionSet() 
				{
					{ "script:",	v => script = v },
					{ "class:",		v => @class = v },
					{ "method:",	v => @method = v },
					{ "h|?|help",	v => help = v != null } 
				};

			List<string> scriptArgs = p.Parse( args );

			if ( help )
			{
				WriteHelp();
				return;
			}

            Logger.Instance.LogToConsole = true;

			if ( string.IsNullOrEmpty( script ) )
			{
				Logger.Instance.Error( "The script source path cannot be null or an empty string." );
				return;
			}

			try
			{
				ScriptAssembly assembly = new ScriptSource( script ).Compile();

				Logger.Instance.Clear();

				using ( ScriptingFramework.Instance )
				{
					ScriptingFramework.Instance.Initialise();
					
					assembly.Invoke( @class, method, scriptArgs.ToArray() );
				}
			}
			catch ( Exception e )
			{
				Logger.Instance.Exception( e );
			}

            Logger.Instance.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		static void WriteHeader()
		{
			const string header = "\n-----------------------------------------------------\n" +
									 "Cobos.ScriptEngine - CSharp scripting engine.\n" +
									 "Copyright (c) 2012 Nicholas Davis.\n" +
									 "-----------------------------------------------------\n\n";

			Console.Write( header );
		}

		/// <summary>
		/// 
		/// </summary>
		static void WriteHelp()
		{
			const string help = "Usage: Cobos.ScriptEngine /script:<path> [/class:<name> /method:<name> <name=value...>]\n" +
										"\n" +
										"\t/script:<path>\t\tPath to the CSharp script to run.\n" +
										"\t/class:<name>\t\tOptional. Name of the class to invoke.\n" +
										"\t/method:<name>\t\tOptional. Name of the method to invoke.\n" +
										"\tname=value...\t\tOptional. Variable list of name/value pair arguments to pass to the method.\n" +
										"\n" +
										"Remarks:\n\n" +
										"If the class or method name are omitted then this instructs the scripting engine to invoke the first method on the first script class object that it encounters.  Used for scripts containing only one script class with only one method." +
										"\n\n" +
										"If you omit some script arguments, the the method will be called using default values for those missing arguments." +
										"\n\n";


			Console.Write( help );
		}
	}
}
