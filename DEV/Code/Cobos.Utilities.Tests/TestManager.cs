using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Utilities.Tests
{
	public static class TestManager
	{
		/// <summary>
		/// Obviously this is a bit of a hacky solution to locating the test files.
		/// Given time, this should be implemented as configuration.
		/// </summary>

		public const string TestFilesLocation = @"C:\Projects\Cobos.Core\DEV\Code\Cobos.Utilities.Tests\TestFiles";

		public const string UncSharedFolder = @"\\ap-sgisourcectrl\cad_795\include";

		public static string ResolvePath( string relative )
		{
			return System.IO.Path.Combine( TestFilesLocation, relative );
		}
	}
}
