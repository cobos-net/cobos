using System;
using System.Collections.Generic;
using System.Text;
using Cobos.Script;
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
