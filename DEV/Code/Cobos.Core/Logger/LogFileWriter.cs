#define LOG_FILE_XML_FORMAT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Cobos.Utilities.Xml;
using Cobos.Utilities.File;

namespace Cobos.Core.Logger
{
	public class LogFileWriter : IDisposable, ILogger
	{
		#region Logger Members

		/// <summary>
		/// 
		/// </summary>
		private string _logPath = null;

		public string LogPath
		{
			get
			{
				return _logPath;
			}
		}

		public bool LogToConsole
		{
			get;
			set;
		}

		/// <summary>
		/// Default to information level for broad logging information
		/// </summary>
		MessageCategory _logLevel = MessageCategory.Information;
		
		public MessageCategory LogLevel
		{
			get
			{
				return _logLevel;
			}
			set
			{
				_logLevel = value;

				Log( "Log level set to " + MessageCategoryFormat.ToString( _logLevel ), MessageCategory.Information, DateTime.Now );
			}
		}
								
		/// <summary>
		/// 
		/// </summary>
		private FileStream _fstream = null;

		/// <summary>
		/// 
		/// </summary>
		private TextWriter _writer = null;

		/// <summary>
		/// 
		/// </summary>
		private object _readWriteLock = new object();

		#endregion

		#region Initialisation, Cleanup

		/// <summary>
		/// 
		/// </summary>
		/// <param name="logPath"></param>
		public LogFileWriter( string logFileFolder, string applicationName )
		{
			LogToConsole = false;

			if ( logFileFolder == null )
			{
				logFileFolder = @"C:\temp";
			}

			if ( applicationName != null )
			{
				_logPath = logFileFolder + @"\" + applicationName + "-";
			}
			else
			{
				_logPath = logFileFolder + @"\";
			}

#if LOG_FILE_XML_FORMAT
			_logPath += DateTime.Now.ToString( "yyyyMMdd-HHmmss" ) + ".xlog";
#else
			_logPath += DateTime.Now.ToString( "yyyyMMdd-HHmmss" ) + ".log";
#endif

			try
			{
				_fstream = new FileStream( _logPath, FileMode.Create, FileAccess.ReadWrite );

				// create a thread-safe wrapper around the log writer
				
				_writer = new StreamWriter( _fstream );

				//WriteHeader( _writer );
			}
			catch ( System.Exception )
			{
				_writer = null;
				_fstream = null;
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void WriteHeader( TextWriter writer )
		{
#if LOG_FILE_XML_FORMAT
			writer.WriteLine( "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" );
			writer.WriteLine( "<LogFile xmlns=\"http://www.cobos.co.uk/core/logger\">" );
#else
			writer.WriteLine( "*********************************************" );
			writer.WriteLine( String.Format( "** Log started {0}", DateTime.Now.ToString( "s" ) ) );
			writer.WriteLine( "*********************************************" );
#endif
			writer.Flush();
		}

		/// <summary>
		/// 
		/// </summary>
		private void WriteFooter( TextWriter writer )
		{
#if LOG_FILE_XML_FORMAT
			writer.WriteLine( "</LogFile>" );
#else
			writer.WriteLine( "*********************************************" );
			writer.WriteLine( String.Format( "** Log finished {0}", DateTime.Now.ToString( "s" ) ) );
			writer.WriteLine( "*********************************************" );
#endif
			writer.Flush();
		}

		#endregion

		#region IDisposable implementation

		private bool _disposed = false;

		~LogFileWriter()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			Dispose( true );
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				return;
			}

			if ( disposing )
			{
				if ( _writer != null )
				{
					//WriteFooter( _writer );
					_writer.Dispose();
					_writer = null;
				}

				if ( _fstream != null )
				{
					_fstream.Dispose();
					_fstream = null;
				}

				_logPath = null;

				GC.SuppressFinalize( this );
			}

			// free unmanaged resources...

			_disposed = true;
		}

		#endregion

		#region Log Writing

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void Log( Exception e )
		{
			if ( MessageCategory.Error > _logLevel )
			{
				return;
			}

			StringBuilder message = new StringBuilder( 1024 );

			message.AppendLine( e.Message + " (" + e.Source + ")" );

			Exception inner = e;

			while ( (inner = inner.InnerException) != null )
			{
				message.AppendLine( inner.Message + " (" + inner.Source + ")" );
			}

			message.AppendLine( e.StackTrace );
			
			Log( message.ToString(), MessageCategory.Error, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void Log( CobosException e )
		{
			Log( e.Message, e.Category, e.Timestamp );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="category"></param>
		public void Log( string entry, MessageCategory category )
		{
			Log( entry, category, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Information( string format, params object[] args )
		{
			if ( MessageCategory.Information > _logLevel )
			{
				return;
			}

			Log( String.Format( format, args ), MessageCategory.Information, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Warning( string format, params object[] args )
		{
			if ( MessageCategory.Warning > _logLevel )
			{
				return;
			}

			Log( String.Format( format, args ), MessageCategory.Warning, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Error( string format, params object[] args )
		{
			if ( MessageCategory.Error > _logLevel )
			{
				return;
			}

			Log( String.Format( format, args ), MessageCategory.Error, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Debug( string format, params object[] args )
		{
			if ( MessageCategory.Debug > _logLevel )
			{
				return;
			}

			Log( String.Format( format, args ), MessageCategory.Debug, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="category"></param>
		/// <param name="timestamp"></param>
		public void Log( string entry, MessageCategory category, DateTime timestamp )
		{
			if ( category > _logLevel )
			{
				return;
			}

			string categoryString = MessageCategoryFormat.ToString( category );

			string timestampString = timestamp.ToString( "s" );

			if ( _writer != null )
			{
				lock ( this )
				{
#if LOG_FILE_XML_FORMAT
					_writer.WriteLine( "<" + categoryString + " timestamp=\""+ timestampString + "\">" + entry + " </" + categoryString + ">" );
#else
					_writer.WriteLine( "{0} {1}: {2}", timestampString, categoryString, entry );
#endif
					_writer.Flush();
				}

			}

			if ( LogToConsole )
			{
				System.Console.WriteLine( "{0} {1}: {2}", timestampString, categoryString, entry );
			}
		}

		#endregion

		#region Metadata

		public void AddMetadata( string name, string value )
		{
			if ( _writer != null )
			{
				lock ( this )
				{
#if LOG_FILE_XML_FORMAT
					_writer.WriteLine( "<metadata>" );
					_writer.WriteLine( "<name>{0}</name>", name );
					_writer.WriteLine( "<value>{0}</value>", value );
					_writer.WriteLine( "</metadata>" );
#else
					_writer.WriteLine( "{0}:{1}", name, value );
#endif
					_writer.Flush();
				}
			}

			if ( LogToConsole )
			{
				System.Console.WriteLine( "{0}:{1}", name, value );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void AddMetadata( string name, string format, params object[] args )
		{
			AddMetadata( name, String.Format( format, args ) );
		}

		#endregion

		#region Log transformations

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		public void WriteToFile( string path )
		{
			const int CHUNK_SIZE = 128;

			lock ( this )
			{
				_writer.Flush();
				_fstream.Seek( 0, SeekOrigin.Begin );

				TextReader reader = new StreamReader( _fstream );

				using ( StreamWriter writer = new StreamWriter( path ) )
				{
					WriteHeader( writer );

					char[] buffer = new char[ CHUNK_SIZE ];
					int bytesRead;

					while ( (bytesRead = reader.Read( buffer, 0, CHUNK_SIZE )) != 0 )
					{
						writer.Write( buffer, 0, bytesRead );
					}

					WriteFooter( writer );

				}
				// dont Close or Dispose the reader, this will close the underlying FileStream object
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		public void WriteToHtml( string path )
		{
			string tmpname = Path.GetTempFileName();

			try
			{
				WriteToFile( tmpname );

				XsltHelper.Transform( "Cobos.Core.Logger.LogFile.xslt", tmpname, path );
			}
			catch ( System.Exception e )
			{
				System.Diagnostics.Debug.Print( e.Message );
				throw;
			}
			finally
			{
				FileUtility.DeleteFile( tmpname );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="output"></param>
		public void WriteToHtml( TextWriter output )
		{
			string tmpname = Path.GetTempFileName();

			try
			{
				WriteToFile( tmpname );

				using ( XmlReader xmlReader = new XmlTextReader( tmpname ) )
				{
					XsltHelper.Transform( "Cobos.Core.Logger.LogFile.xslt", xmlReader, output );
				}
			}
			catch ( System.Exception )
			{
				throw;
			}
			finally
			{
				FileUtility.DeleteFile( tmpname );
			}
		}
		
		#endregion

	}

}
