using System;
using System.IO;
using Cobos.Core.Logger;

namespace Cobos.Core.UI
{
	public class CobosApplication
	{
		#region Singleton Instance

		private static CobosApplication _current = null;

		private CobosApplication()
		{
		}

		public static CobosApplication Current
		{
			get
			{
				if ( _current ==  null )
				{
					_current = new CobosApplication();
				}
				else if ( _current._disposed )
				{
					throw new ObjectDisposedException( "Cobos.Core.UI.PhoneViewApplication", "The application has already been disposed." );
				}
				return _current;
			}
		}

		#endregion

		#region IDisposable

		~CobosApplication()
		{
			Dispose( false );
		}

		private bool _disposed = false;

		public void Dispose()
		{
			Dispose( true );
		}

		protected void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				return;
			}

			if ( disposing )
			{
				_current = null;

				GC.SuppressFinalize( this );
			}

			// free non managed resources

			_disposed = true;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// 
		/// </summary>
		public ICurrentCursor Cursor
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public IMessageHandler Message
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public IProgressBar ProgressBar
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string StartupPath
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public ICurrentUser User
		{
			get;
			private set;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Convenience method to initialise the application components.
		/// Will throw an exception if any of the required application components
		/// are null or invalid paths.
		/// </summary>
		/// <param name="cursor"></param>
		/// <param name="messages"></param>
		/// <param name="progress"></param>
		/// <param name="user"></param>
		/// <param name="startupPath"></param>
		public void Initialise( ICurrentCursor cursor, IMessageHandler message, IProgressBar progress, ICurrentUser user, string startupPath )
		{
			#region Initialise UI components

			if ( cursor == null )
			{
				throw new Exception( "Invalid cursor handle specified" );
			}
			Cursor = cursor;

			if ( message == null )
			{
				throw new Exception( "Invalid cursor handle specified" );
			}
			Message = message;

			if ( progress == null )
			{
				throw new Exception( "Invalid progress bar handle specified" );
			}
			ProgressBar = progress;

			if ( user == null )
			{
				throw new Exception( "Invalid user handle specified" );
			}
			User = user;

#endregion

			#region Set the working folder

			if ( string.IsNullOrEmpty( startupPath ) )
			{
				throw new Exception( "Invalid startup path specified" );
			}

			if ( !Directory.Exists( startupPath ) )
			{
				throw new Exception( string.Format( "The startup path {0} does not exist", startupPath ) );
			}
			StartupPath = startupPath;

			#endregion

		}

		/// <summary>
		/// Resolves a configuration path using the startup path if the file can't be found
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string ResolveFilePath( string path, bool failIfNotExist )
		{
			if ( File.Exists( path ) )
			{
				return path;
			}

			if ( Path.IsPathRooted( path ) )
			{
				// try resolving full path to the settings folder instead
				path = Path.GetFileName( path );
			}
			
			path = StartupPath + @"\" + path;

			if ( File.Exists( path ) )
			{
				return path;
			}

			if ( failIfNotExist )
			{
				return null;
			}

			return path;
		}

		#endregion

	}
}
