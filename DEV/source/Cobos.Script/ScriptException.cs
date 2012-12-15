using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Script
{
	public class ScriptException : Exception
	{
		public ScriptException( string message )
			: base( message )
		{
		}

		public ScriptException( string format, params object[] args )
			: base ( string.Format( format, args ) )
		{
		}

		public ScriptException( string message, Exception inner )
			: base( message, inner )
		{
		}

		public ScriptException( string format, Exception inner, params object[] args )
			: base( string.Format( format, args ), inner )
		{
		}

	}
}
