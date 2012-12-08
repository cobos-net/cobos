﻿using System;
using System.IO;
using Cobos.Utilities.File;
using Cobos.Core.Log;

namespace Cobos.Core.UI
{
	public class CobosApplication : IDisposable
	{
		#region Singleton Instance

		private static CobosApplication _instance = null;

		protected CobosApplication()
		{
			Instance = this;
		}

		public static CobosApplication Instance
		{
			get
			{
				if ( _instance ==  null )
				{
					throw new InvalidOperationException( "Cobos.Core.UI.CobosApplication: No application object has been set." );
				}
				else if ( _instance._disposed )
				{
					throw new ObjectDisposedException( "Cobos.Core.UI.CobosApplication", "The application has already been disposed." );
				}

				return _instance;
			}
			protected set
			{
				if ( value == null )
				{
					_instance = value;
				}
				else if ( _instance != null )
				{
					throw new InvalidOperationException( "Cobos.Core.UI.CobosApplication: The application object has already been set." );
				}
				else
				{
					_instance = value;
				}
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
			
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				return;
			}

			if ( disposing )
			{
				// free managed resources

				if ( Log != null )
				{
					Log.Dispose();
					Log = null;
				}

				IDisposable toDispose;

				if ( (toDispose = Cursor as IDisposable) != null )
				{
					toDispose.Dispose();
				}

				Cursor = null;

				if ( (toDispose = Message as IDisposable) != null )
				{
					toDispose.Dispose();
				}

				Message = null;

				if ( (toDispose = ProgressBar as IDisposable) != null )
				{
					toDispose.Dispose();
				}

				ProgressBar = null;

				if ( (toDispose = User as IDisposable) != null )
				{
					toDispose.Dispose();
				}

				User = null;

				Instance = null;
			}

			// free non managed resources

			_disposed = true;
		}

		#endregion

		#region Public properties

		public LogWriter Log
		{
			get;
			private set;
		}

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
		public virtual void Initialise( ICurrentCursor cursor, IMessageHandler message, IProgressBar progress, ICurrentUser user, string startupPath )
		{
			#region Initialise the logger 

			Log = new LogWriter();
			Log.Initialise();

			#endregion

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
		/// Report an exception to the user and add to the log.
		/// </summary>
		/// <param name="e"></param>
		public void Report( Exception e, string category )
		{
			Log.Exception( e );

			Message.ShowError( e, category );
		}

		/// <summary>
		/// Resolves a configuration path using the startup path if the file can't be found
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string ResolveFilePath( string path )
		{
			PathResolver resolver = new PathResolver( StartupPath, null );

			NormalisedPath found = resolver.FindFilePath( path );

			if ( found == null )
			{
				return null;
			}

			return found.Value;
		}

		#endregion

	}
}