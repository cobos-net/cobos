// ============================================================================
// Filename: Program.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Tools v0.1
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
using System.Configuration;
using System.IO;
using System.Reflection;
using Cobos.Data;
using Cobos.Data.Adapters;
using Cobos.Utilities.Xml;
using NDesk.Options;

namespace Cobos.DatabaseToXsd
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

			try
			{
				string schema = null;
				string output = null;
				string dump = null;
				string connectString = null;
				bool help = false;

				var p = new OptionSet() 
				{
					{ "output:",         v => output = v },
					{ "dump:",				v => dump = v },
					{ "schema:",			v => schema = v },
					{ "connection:",	v => connectString = v },
					{ "h|?|help",			v => help = v != null } 
				};
				
				List<string> tables = p.Parse( args );

				if ( help )
				{
					WriteHelp();
					return;
				}

				if ( schema == null )
				{
					schema = ConfigurationManager.AppSettings[ "Schema" ];
				}

				///////////////////////////////////////////////////////////////////////////
				// 1. Connect to Oracle 

				if ( connectString == null )
				{
					connectString = ConfigurationManager.ConnectionStrings[ "Default" ].ConnectionString;
				}

				Console.WriteLine( "Connecting to Oracle: " + connectString );

				///////////////////////////////////////////////////////////////////////////
				// 2. Create the output file and get the table schema

				if ( output == null )
				{
					output = ConfigurationManager.AppSettings[ "OutputFolder" ] + @"\" + schema + ".xsd";
				}

				IDatabaseAdapter database = new Cobos.Data.Oracle.OracleDatabaseAdapter( connectString );
				
				if ( dump != null )
				{
					using ( FileStream fstream = new FileStream( dump, FileMode.Create ) )
					{
						database.GetTableSchema( schema, tables.ToArray(), fstream );
					}
				}

				using ( FileStream fstream = new FileStream( output, FileMode.Create ) )
				{
					Console.WriteLine( "Getting table schema(s) for " + string.Join( ",", tables ) );
					Console.WriteLine( "Reading tables from " + schema );

					database.GetTableSchema( schema, tables.ToArray(), fstream );
				}

				///////////////////////////////////////////////////////////////////////////
				// 3. Success

				Console.WriteLine( "Successfully written result to " + output );
			}
			catch ( Exception e )
			{
				Console.WriteLine( e.Message );

				while ( (e = e.InnerException) != null )
				{
					Console.WriteLine( e.Message );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		static void WriteHeader()
		{
			const string header = "\n----------------------------------------------------\n" +
										 "DatabaseToXsd - Generate Xsd from Oracle DB table.\n" +
										 "Copyright (c) 2010 Nicholas Davis (nick@cobos.co.uk).\n" +
										 "----------------------------------------------------\n\n";

			Console.Write( header );
		}

		/// <summary>
		/// 
		/// </summary>
		static void WriteHelp()
		{
			const string help = "Usage: DatabaseToXsd <table>\n" +
										"\n" +
										"output\t\tOutput filename (defaults to <schema>.xsd using the save path in the config file)." +
										"schema\t\tThe database schema to read the tables from (defaults to settings in the config file)." +
										"dbconnection\t\tThe database connection to use (defaults to the database connection in the config file)." +
										"table...table(n)\t\tlist of table names to process\n" +
										"\n";

			Console.Write( help );
		}


	}
}
