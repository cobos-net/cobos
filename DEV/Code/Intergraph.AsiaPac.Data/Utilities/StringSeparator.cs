using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Data.Utilities
{
	public static class StringSeparator
	{
		public static string GetTokenAt( string source, char separator, int index )
		{
			if ( string.IsNullOrEmpty( source ) )
			{
				return null;
			}

			if ( index < 0 )
			{
				return null;
			}

			string[] tokens = source.Split( separator );

			if ( index >= tokens.Length )
			{
				return null;
			}

			return tokens[ index ];
		}
	}
}
