using System;
using System.Text;
using System.Reflection;

namespace Intergraph.AsiaPac.Core.Tests
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
					_testFilesFolder = @"D:\Projects\Intergraph.AsiaPac.Core\DEV\Code\Intergraph.AsiaPac.Core.Tests\TestFiles";
				}
				return _testFilesFolder;
			}
		}

		static string _testFilesFolder;
	}
}
