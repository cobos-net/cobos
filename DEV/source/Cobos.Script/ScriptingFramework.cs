using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Cobos.Core.Log;

namespace Cobos.Script
{
	public class ScriptingFramework : IDisposable
	{
		private ScriptingFramework()
		{
		}

		#region Singleton

		public static ScriptingFramework Instance
		{
			get
			{
				if ( TheInstance == null )
				{
					TheInstance = new ScriptingFramework();
				}

				return TheInstance;
			}
		}

		private static ScriptingFramework TheInstance;

		#endregion

		#region Initialisation and cleanup

		public void Initialise()
		{
			Logger.Instance.Information( "Initialising Scripting" );
		}

		public void Terminate()
		{
		}

		#endregion

		#region IDisposable

		bool disposed = false;

		~ScriptingFramework()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			Dispose( true );
		}

		private void Dispose( bool disposing )
		{
			if ( this.disposed )
			{
				return;
			}

			if ( disposing )
			{
				Terminate();

				GC.SuppressFinalize( this );
			}

			this.disposed = true;
		}

		#endregion
	}
}
