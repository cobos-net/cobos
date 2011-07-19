using System;
using System.Collections.Generic;
using System.Text;

namespace Cobos.Data.Tests
{
	public static class TestManager
	{
		public readonly static string ConnectionString = "Data Source=VEA795DB2.WORLD;User Id=eadev;Password=eadev";

		public readonly static DatabaseAdapter DatabaseAdapter = new DatabaseAdapter( ConnectionString );

		public readonly static string TestFiles = @"\Projects\Cobos.Core\DEV\Code\Cobos.Data.Tests\TestData\";
	}
}
