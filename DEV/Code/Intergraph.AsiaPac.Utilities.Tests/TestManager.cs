﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Utilities.Tests
{
	public static class TestManager
	{
		/// <summary>
		/// Obviously this is a bit of a hacky solution to locating the test files.
		/// Given time, this should be implemented as configuration.
		/// </summary>

		public const string TestFilesLocation = @"D:\Projects\Intergraph.AsiaPac.Core\DEV\Code\Intergraph.AsiaPac.Utilities.Tests\TestFiles";

		public const string UncSharedFolder = @"\\ap-sgisourcectrl\cad_795\include";
	}
}