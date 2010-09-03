using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Intergraph.Oz.Utilities.Logger;
using System.IO;

namespace Intergraph.Oz.Utilities.Tests.Logger
{
	public class LogFileTest
	{
		[Fact]
		public void Log_file_can_write_to_Html()
		{
			LogFileWriter logFile = new LogFileWriter( @"c:\temp", "test" );
			logFile.AddMetadata( "Title", "Test Log File" );

			logFile.Error( "Test error" );

			logFile.WriteToHtml( @"c:\temp\test.htm" );

			logFile.Warning( "Test warning" );

			string path = logFile.LogPath;

			logFile.Dispose();
			logFile = null;

			System.IO.File.Delete( path );
		}
	}
}
