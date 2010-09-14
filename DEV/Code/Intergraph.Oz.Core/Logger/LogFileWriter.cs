#define LOG_FILE_XML_FORMAT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Intergraph.Oz.Utilities.Xml;
using Intergraph.Oz.Utilities.File;

namespace Intergraph.Oz.Core.Logger
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
				logFileFolder += String.Format( @"\{0}-", applicationName );
			}
			else
			{
				logFileFolder += @"\";
			}

#if LOG_FILE_XML_FORMAT
			logFileFolder = String.Format( @"{0}{1}.xml", logFileFolder, DateTime.Now.ToString( "yyyyMMdd-HHmmss" ) );
#else
			logPath = String.Format( @"{0}{1}.log", logPath, DateTime.Now.ToString( "yyyyMMdd-HHmmss" ) );
#endif
			_logPath = logFileFolder;

			try
			{
				_fstream = new FileStream( _logPath, FileMode.Create, FileAccess.ReadWrite );

				// create a thread-safe wrapper around the log writer
				_writer = TextWriter.Synchronized( new StreamWriter( _fstream ) );

				WriteHeader();
			}
			catch ( System.Exception )
			{
				_writer = null;
				_fstream = null;
				throw;
			}
		}

		private bool _disposed = false;

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			lock ( this )
			{
				if ( _disposed )
				{
					return;
				}

				if ( _writer != null )
				{
					WriteFooter();
					_writer.Dispose();
					_writer = null;
				}

				if ( _fstream != null )
				{
					_fstream.Dispose();
					_fstream = null;
				}

				_logPath = null;

				_disposed = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void WriteHeader()
		{
#if LOG_FILE_XML_FORMAT
			_writer.WriteLine( "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" );
			_writer.WriteLine( "<LogFile xmlns=\"http://www.intergraph.com/oz/utilities/logger\">" );
#else
			_writer.WriteLine( "*********************************************" );
			_writer.WriteLine( String.Format( "** Log started {0}", DateTime.Now.ToString( "s" ) ) );
			_writer.WriteLine( "*********************************************" );
#endif
			_writer.Flush();
		}

		/// <summary>
		/// 
		/// </summary>
		private void WriteFooter()
		{
#if LOG_FILE_XML_FORMAT
			_writer.WriteLine( "</LogFile>" );
#else
			_writer.WriteLine( "*********************************************" );
			_writer.WriteLine( String.Format( "** Log finished {0}", DateTime.Now.ToString( "s" ) ) );
			_writer.WriteLine( "*********************************************" );
#endif
			_writer.Flush();
		}

		#endregion

		#region Log Writing

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void Log( Exception e )
		{
			DateTime timestamp = DateTime.Now;

			Log( String.Format( "{0} ({1})", e.Message, e.Source ), MessageCategory.Error, timestamp );

			Exception inner = e;

			while ( (inner = inner.InnerException) != null )
			{
				Log( String.Format( "{0} ({1})", inner.Message, inner.Source ), MessageCategory.Error, timestamp );
			}

			Log( e.StackTrace, MessageCategory.Error, timestamp );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void Log( IntergraphException e )
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
			Log( String.Format( format, args ), MessageCategory.Information, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Warning( string format, params object[] args )
		{
			Log( String.Format( format, args ), MessageCategory.Warning, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Error( string format, params object[] args )
		{
			Log( String.Format( format, args ), MessageCategory.Error, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Debug( string format, params object[] args )
		{
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
			string categoryString = MessageCategoryFormat.ToString( category );

			string timestampString = timestamp.ToString( "s" );

			if ( _writer != null )
			{
				lock ( _readWriteLock )
				{
#if LOG_FILE_XML_FORMAT
					_writer.WriteLine( "\t<{0} timestamp=\"{1}\">{2}</{0}>", categoryString, timestampString, entry );
#else
					_writer.WriteLine( "{0}\t{1}\t{2}", timestampString, categoryString, entry );
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
				lock ( _readWriteLock )
				{
#if LOG_FILE_XML_FORMAT
					_writer.WriteLine( "\t<Metadata>" );
					_writer.WriteLine( "\t\t<Name>{0}</Name>", name );
					_writer.WriteLine( "\t\t<Value>{0}</Value>", value );
					_writer.WriteLine( "\t</Metadata>" );
#else
					_writer.WriteLine( "{0}:\t\t{1}", name, value );
#endif
					_writer.Flush();
				}
			}

			if ( LogToConsole )
			{
				System.Console.WriteLine( "{0}:\t\t{1}", name, value );
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
			// bit more complicated than I would like because the log file is in Xml format
			// but isn't closed with the </LogFile> tag until the log file is actually closed,
			// so we need to read the file as is into a temporary store and then append the closing
			// Xml tag before we can use the Xslt to transform to Html.

			lock ( _readWriteLock )
			{
				_writer.Flush();
				_fstream.Seek( 0, SeekOrigin.Begin );

				TextReader reader = TextReader.Synchronized( new StreamReader( _fstream ) );

				using ( StreamWriter writer = new StreamWriter( path ) )
				{
					char[] buffer = new char[ 1024 ];
					int bytesRead;

					while ( (bytesRead = reader.Read( buffer, 0, 1024 )) != 0 )
					{
						writer.Write( buffer, 0, bytesRead );
					}

#if LOG_FILE_XML_FORMAT
					writer.Write( "</LogFile>" );
#endif
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

				XsltHelper.Transform( "Intergraph.Oz.Core.Logger.LogFile.xslt", tmpname, path );
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
					XsltHelper.Transform( "Intergraph.Oz.Core.Logger.LogFile.xslt", xmlReader, output );
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
