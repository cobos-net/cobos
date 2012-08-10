using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Cobos.Utilities.Extensions;

namespace Cobos.Utilities.Tests.Extensions
{
	[TestFixture]
	public class ArrayExtensionsTests
	{
		[TestCase]
		public void Can_concatenate_multiple_arrays()
		{
			int[][] multi = new int[][]
			{
				new int[]{ 0, 1, 2 },
				new int[]{ 3, 4, 5, 6 },
				new int[]{ 7, 8 },
				new int[]{ 9 }
			};

			int[] result = ArrayExtensions.ConcatenateAll<int>( multi );

			Assert.AreEqual( new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, result );
		}

		[TestCase]
		public void Can_concatenate_multiple_null_arrays()
		{
			int[][] multi = new int[][]
			{
				null,
				null,
				null,
				null
			};

			int[] result = ArrayExtensions.ConcatenateAll<int>( multi );

			Assert.Null( result );
		}

		[TestCase]
		public void Can_concatenate_multiple_arrays_with_some_nulls()
		{
			int[][] multi = new int[][]
			{
				new int[]{ 0, 1, 2 },
				null,
				new int[]{ 7, 8 },
				null
			};

			int[] result = ArrayExtensions.ConcatenateAll<int>( multi );

			Assert.AreEqual( new int[] { 0, 1, 2, 7, 8 }, result );

		}
	}
}
