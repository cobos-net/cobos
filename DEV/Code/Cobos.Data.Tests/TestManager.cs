using System;
using System.Collections.Generic;
using System.Text;
using Cobos.Data.Adapters;

namespace Cobos.Data.Tests
{
	public static class TestManager
	{
		public readonly static string ConnectionString = "Data Source=VEA795DB2.WORLD;User Id=eadev;Password=eadev";

		public readonly static IDatabaseAdapter DatabaseAdapter = new OracleDatabaseAdapter( ConnectionString );

		public readonly static string TestFiles = @"\Projects\Cobos.Core\DEV\Code\Cobos.Data.Tests\TestData\";
	}
}
