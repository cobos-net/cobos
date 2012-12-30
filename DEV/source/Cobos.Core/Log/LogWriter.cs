// ============================================================================
// Filename: LogWriter.cs
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
			Off,
			Exception,
			Error,
			Warning,
			Information,
			Debug,
			Trace
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
		/// Intialise the logger from the app.config or web.config settings.
		/// </summary>
		public void Initialise()
		{
			Initialise( ParseLogLevel( ConfigurationManager.AppSettings[ "LogLevel" ] ), ConfigurationManager.AppSettings[ "LogFolder" ], ConfigurationManager.AppSettings[ "LogName" ] );
		}

		/// <summary>
		/// Initialise the log using your application's hard coded values.
		/// </summary>
		/// <param name="logLevel"></param>
		/// <param name="logFolder"></param>
		/// <param name="logName"></param>
		public void Initialise( string logLevel, string logFolder, string logName )
		{
			Initialise( ParseLogLevel( logLevel ), logFolder, logName );
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

		/// <summary>
		/// Parse the log level from a string value (usually in the configuration file).
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public static LogLevelEnum ParseLogLevel( string level )
		{
			LogLevelEnum logLevel = LogLevelEnum.Error;

			if ( string.IsNullOrEmpty( level ) )
			{
				return logLevel;
			}

			switch ( level.ToLower() )
			{
            case "off":
                logLevel = LogLevelEnum.Off;
                break;

            case "trace":
				logLevel = LogLevelEnum.Trace;
				break;

			case "debug":
				logLevel = LogLevelEnum.Debug;
				break;

			case "information":
				logLevel = LogLevelEnum.Information;
				break;

			case "warning":
				logLevel = LogLevelEnum.Warning;
				break;

			case "error":
				logLevel = LogLevelEnum.Error;
				break;

			case "exception":
				logLevel = LogLevelEnum.Exception;
				break;

			default:
				logLevel = LogLevelEnum.Error;
				break;
			}

			return logLevel;
		}

		/// <summary>
		/// Write the log file header text.
		/// </summary>
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

		/// <summary>
		/// Log the message.
		/// </summary>
		/// <param name="category"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
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

		/// <summary>
		/// Clear the log file and start writing again.
		/// </summary>
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

		/// <summary>
		/// Log the exception.
		/// </summary>
		/// <param name="e"></param>
		public void Exception( Exception e )
		{
			const string format = "{0}\n" + "{1}\n" + "{2}\n";

			Log( "Exception", format, new object[]{ e.Message, e.Source, e.StackTrace } );

			Exception inner = e;

			while ( (inner = inner.InnerException) != null )
			{
				Exception( inner );
			}
		}

		/// <summary>
		/// Log a trace message.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Trace( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Trace )
			{
				Log( "Trace", format, args );
			}
		}

		/// <summary>
		/// Log a debug message.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Debug( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Debug )
			{
				Log( "Debug", format, args );
			}
		}

		/// <summary>
		/// Log an information message.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Information( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Information )
			{
				Log( "Information", format, args );
			}
		}

		/// <summary>
		/// Log a warning message.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Warning( string format, params object[] args )
		{
			if ( LogLevel >= LogLevelEnum.Warning )
			{
				Log( "Warning", format, args );
			}
		}

        /// <summary>
        /// Log an error message.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Error( string format, params object[] args )
        {
            if ( LogLevel >= LogLevelEnum.Error )
            {
                Log( "Error", format, args );
            }
        }

        #region IDisposable

		bool disposed = false;

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
			if ( this.disposed )
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

			this.disposed = true;
		}

		#endregion
	}
}
