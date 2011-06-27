using System;
using System.Collections.Generic;
using System.Text;

namespace Intergraph.AsiaPac.Data.Tests
{
	public static class TestManager
	{
		public readonly static string ConnectionString = "Data Source=VEA795DB2.WORLD;User Id=eadev;Password=eadev";

		public readonly static DatabaseAdapter DatabaseAdapter = new DatabaseAdapter( ConnectionString );

		public readonly static string TestFiles = @"\Projects\Intergraph.AsiaPac.Core\DEV\Code\Intergraph.AsiaPac.Data.Tests\TestData\";
	}
}
