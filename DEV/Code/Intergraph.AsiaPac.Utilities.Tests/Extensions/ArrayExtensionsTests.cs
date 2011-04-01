using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Intergraph.AsiaPac.Utilities.Extensions;

namespace Intergraph.AsiaPac.Utilities.Tests.Extensions
{
	public class ArrayExtensionsTests
	{
		[Fact]
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

			Assert.Equal<int[]>( new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, result );
		}

		[Fact]
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

		[Fact]
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

			Assert.Equal<int[]>( new int[] { 0, 1, 2, 7, 8 }, result );

		}
	}
}
