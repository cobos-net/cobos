using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Cobos.Core.Logger;
using Cobos.Utilities.File;
using System.IO;
using System.Threading;

namespace Cobos.Core.Tests.Logger
{
	public class LogFileTest
	{
		[Fact]
		public void Log_file_can_write_to_Html()
		{
			string path;

			using ( LogFileWriter logFile = new LogFileWriter( @"c:\temp", "test" ) )
			{
				logFile.AddMetadata( "Title", "Test Log File" );

				logFile.Error( "Test error" );

				logFile.WriteToHtml( @"c:\temp\test.htm" );

				logFile.Warning( "Test warning" );

				path = logFile.LogPath;
			}

			File.Delete( path );
			File.Delete( @"c:\temp\test.htm" );
		}

		[Fact]
		public void Log_file_can_handle_multiple_reads_and_writes()
		{
			FileUtility.CleanDirectory( TestManager.TestFilesFolder + @"\LogFiles" );

			LogFileWriter logFile = new LogFileWriter( TestManager.TestFilesFolder + @"\LogFiles", "test" );
			logFile.AddMetadata( "Title", "Test Log File" );

			const int NUM_WRITER_THREADS = 10;
			const int NUM_READER_THREADS = 3;

			Thread[] writers = new Thread[ NUM_WRITER_THREADS ];

			for ( int i = 0; i < NUM_WRITER_THREADS; ++i )
			{
				WriterParams param = new WriterParams();
				param.Category = (MessageCategory)(i % 4);
				param.LogFile = logFile;
				param.Id = i;

				writers[ i ] = new Thread( WriterStart );
				writers[ i ].Start( param );
			}

			Thread[] readers = new Thread[ NUM_READER_THREADS ];

			for ( int i = 0; i < NUM_READER_THREADS; ++i )
			{
				ReaderParams param = new ReaderParams();
				param.LogFile = logFile;
				param.Id = i;

				readers[ i ] = new Thread( ReaderStart );
				readers[ i ].Start( param );
			}

			// wait for the threads to finish
			for ( int i = 0; i < NUM_WRITER_THREADS; ++i )
			{
				writers[ i ].Join();
			}

			for ( int i = 0; i < NUM_READER_THREADS; ++i )
			{
				readers[ i ].Join();
			}

			logFile.Dispose();
		}

		struct WriterParams
		{
			public MessageCategory Category;
			public LogFileWriter LogFile;
			public int Id;
		}

		void WriterStart( object param )
		{
			WriterParams writerParams = (WriterParams)param;

			int write = 0;
			DateTime end = DateTime.Now + new TimeSpan( 0, 0, 10 );

			while ( DateTime.Now < end )
			{
				++write;
				writerParams.LogFile.Log( "Log entry " + write + " from writer " + writerParams.Id, writerParams.Category );
			}
		}

		struct ReaderParams
		{
			public LogFileWriter LogFile;
			public int Id;
		}

		void ReaderStart( object param )
		{
			return;
			ReaderParams readerParams = (ReaderParams)param;

			int read = 0;
			DateTime end = DateTime.Now + new TimeSpan( 0, 0, 10 );

			while ( DateTime.Now < end )
			{
				++read;
				readerParams.LogFile.WriteToHtml( TestManager.TestFilesFolder + @"\LogFiles\" + readerParams.Id + "_" + read + ".htm" );
			}
		}

	}
}
