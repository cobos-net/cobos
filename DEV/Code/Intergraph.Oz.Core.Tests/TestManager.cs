using System;
using System.Text;
using System.Reflection;

namespace Intergraph.Oz.Core.Tests
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
					_testFilesFolder = @"D:\Projects\Intergraph.Oz.Core\DEV\Code\Intergraph.Oz.Core.Tests\TestFiles";
				}
				return _testFilesFolder;
			}
		}

		static string _testFilesFolder;
	}
}
