using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cobos.Script.Tests
{
	public static class TestManager
	{
		const string TestDirectory = @"C:\Projects\Cobos\DEV\source\Cobos.Script.Tests\TestFiles";

		public static string ResolvePath( string relative )
		{
			return Path.Combine( TestDirectory, relative );
		}
	}
}
