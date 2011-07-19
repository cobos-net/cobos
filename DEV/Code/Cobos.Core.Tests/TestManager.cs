using System;
using System.Text;
using System.Reflection;

namespace Cobos.Core.Tests
{
	public static class TestManager
	{
		public static string TestFilesFolder
		{
			get
			{
				if ( _testFilesFolder == null )
				{
					// should somehow automate this
					_testFilesFolder = @"C:\Projects\Cobos.Core\DEV\Code\Cobos.Core.Tests\TestFiles";
				}
				return _testFilesFolder;
			}
		}

		static string _testFilesFolder;
	}
}
