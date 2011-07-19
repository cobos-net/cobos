using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Intergraph.AsiaPac.WpfApplication.UI
{
	public class CommandLineArgs
	{
		Dictionary<string, string> _parsed = new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase ); 

		public CommandLineArgs()
		{
			string[] args = Environment.GetCommandLineArgs();

			// first argument is the executable
			Executable = args[ 0 ];

			// in form /switch:value
			Regex regex = new Regex( @"\/(\w*)\:(.*)" );

			for ( int a = 1; a < args.Length; ++a )
			{
				Match match = regex.Match( args[ a ] );

				if ( match.Success )
				{
					_parsed[ match.Groups[ 1 ].Value ] = match.Groups[ 2 ].Value;
				}
			}
		}

		public string Executable
		{
			get;
			private set;
		}

		public string this[ string key ]
		{
			get
			{
				string value;

				if ( _parsed.TryGetValue( key, out value ) )
				{
					return value;
				}

				return null;
			}
		}
	}
}
