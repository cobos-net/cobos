using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Reflection;

namespace Cobos.Core.Log
{
	public class LogWriter
	{
		public enum LogLevelEnum
		{
			Fatal,
			Error,
			Warning,
			Information,
			Debug,
			Trace,
			Off
		}

		public LogLevelEnum LogLevel
		{
			get;
			set;
		}


		public bool LogToConsole
		{
			get;
			set;
		}

		/// <summary>
		/// Thread-safe writer created via TextWriter.Synchronized.
		/// </summary>
		private TextWriter Writer;

		/// <summary>
		/// Path to the log file.
		/// </summary>
		private string LogPath;

		/// <summary>
		/// The application name for the log file.
		/// </summary>
		private string LogName;

		/// <summary>
		/// Intialise the logger from the app.config settings.
		/// </summary>
		public void Initialise()
		{
			string level = ConfigurationManager.AppSettings[ "LogLevel" ];

			switch ( level.ToLower() )
			{
			case "debug":
				LogLevel = LogLevelEnum.Debug;
				break;

			case "information":
				LogLevel = LogLevelEnum.Information;
				break;

			case "warning":
				LogLevel = LogLevelEnum.Warning;
				break;

			case "error":
			default:
				LogLevel = LogLevelEnum.Error;
				break;
			}

			Initialise( LogLevel, ConfigurationManager.AppSettings[ "LogFolder" ], ConfigurationManager.AppSettings[ "LogName" ] );
		}

		/// <summary>
		/// Initialise the log using your application's hard coded values.
		/// </summary>
		/// <param name="logLevel"></param>
		/// <param name="logFolder"></param>
		/// <param name="applicationName"></param>
		public void Initialise( LogLevelEnum logLevel, string logFolder, string logName )
		{
			LogLevel = logLevel;

			LogName = logName;

			if ( string.IsNullOrEmpty( LogName ) )
			{
				LogName = Assembly.GetEntryAssembly().GetName().Name;
			}

			if ( !string.IsNullOrEmpty( logFolder ) )
			{
				if ( !Directory.Exists( logFolder ) )
				{
					DirectoryInfo info = Directory.CreateDirectory( logFolder );

					if ( info == null || !info.Exists )
					{
						throw new ArgumentException( "Cannot create LogFolder at " + logFolder );
					}
				}

				string logFileName = DateTime.Now.ToString( "yyyyMMdd" ) + "_" + LogName + ".log";

				LogPath = Path.Combine( logFolder, logFileName );

				Writer = TextWriter.Synchronized( new StreamWriter( LogPath, File.Exists( LogPath ) ) );

				WriteHeader();
			}
			else
			{
				LogToConsole = true;
			}
		}

		private void WriteHeader()
		{
			string timestamp = DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss:fff" );

			if ( Writer != null )
			{
				Writer.WriteLine( "<LogMetadata>" );
				Writer.WriteLine( "    <Application>" + LogName + "</Application>" );
				Writer.WriteLine( "    <Started>" + timestamp + "</Started>" );
				Writer.WriteLine( "</LogMetadata>" );
			}

			if ( LogToConsole )
			{
				Console.WriteLine( "===============================================================================" );
				Console.WriteLine( "Application: " + LogName );
				Console.WriteLine( "Log Started: " + timestamp );
				Console.WriteLine( "===============================================================================" );
			}
		}

		private void Log( string category, string format, params object[] args )
		{
			string timestamp = DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss:fff" );
			string message = string.Format( format, args );

			if ( Writer != null )
			{
				Writer.WriteLine( "<" + category + @" timestamp=""" + timestamp + @""">" + message + "</" + category + ">" );
				Writer.Flush();
			}

			if ( LogToConsole )
			{
				Console.WriteLine( timestamp + " - " + category + ": " + message );
			}
		}

		public void Clear()
		{
			if ( Writer != null )
			{
				Writer.Flush();
				Writer.Dispose();
				Writer = null;
			}

			if ( !string.IsNullOrEmpty( LogPath ) )
			{
				Writer = TextWriter.Synchronized( new StreamWriter( LogPath, false ) );

				WriteHeader();
			}

			if ( LogToConsole )
			{
				Console.Clear();
			}
		}

		public void Exception( Exception e )
		{
			const string format = "{0}\n" + "{1}\n" + "{2}\n";

			Error( format, e.Message, e.Source, e.StackTrace );

			Exception inner = e;

			while ( (inner = inner.InnerException) != null )
			{
				Exception( inner );
			}
		}

		public void Trace( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Trace )
			{
				Log( "Trace", format, args );
			}
		}

		public void Debug( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Debug )
			{
				Log( "Debug", format, args );
			}
		}

		public void Information( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Information )
			{
				Log( "Information", format, args );
			}
		}

		public void Warning( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Warning )
			{
				Log( "Warning", format, args );
			}
		}

		public void Error( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Error )
			{
				Log( "Error", format, args );
			}
		}

		#region IDisposable

		bool _disposed = false;

		~LogWriter()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			Dispose( true );
		}

		private void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				return;
			}

			if ( disposing )
			{
				if ( Writer != null )
				{
					Writer.Flush();
					Writer.Dispose();
					Writer = null;
				}

				GC.SuppressFinalize( this );
			}

			_disposed = true;
		}

		#endregion
	}
}
