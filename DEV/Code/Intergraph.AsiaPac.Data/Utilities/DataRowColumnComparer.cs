using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intergraph.AsiaPac.Data.Utilities
{
	public class DataRowColumnComparer : IComparer<DataRow>
	{
		/// <summary>
		/// Specifiy the sort order
		/// </summary>
		public enum SortOrderEnum
		{
			Ascending,
			Descending
		}

		readonly SortOrderEnum SortOrder;

		/// <summary>
		/// Column order for sort comparison
		/// </summary>
		readonly DataColumnDescriptor[] Columns;

		/// <summary>
		/// Ignore case for string comparisons
		/// </summary>
		readonly bool IgnoreCase;

		public DataRowColumnComparer( DataColumnDescriptor[] sortColumns, SortOrderEnum sortOrder, bool ignoreCase )
		{
			Columns = sortColumns;
			SortOrder = sortOrder;
			IgnoreCase = ignoreCase;
		}

		public DataRowColumnComparer( DataColumn[] sortColumns, SortOrderEnum sortOrder, bool ignoreCase )
			: this( DataColumnDescriptor.ConstructFrom( sortColumns ), sortOrder, ignoreCase )
		{
		}


		/// <summary>
		/// Simple constructor for creating a case-sensitive ascending sort
		/// </summary>
		/// <param name="sortColumns"></param>
		public DataRowColumnComparer( DataColumn[] sortColumns )
			: this( sortColumns, SortOrderEnum.Ascending, false )
		{
		}

		/// <summary>
		/// Compare the two rows for sort order based on the class' sort columns.
		/// this assumces that both rows either belong to the
		/// same table or that they have the same column format.
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public int Compare( DataRow lhs, DataRow rhs )
		{
			for ( int c = 0; c < Columns.Length; ++c )
			{
				DataColumnDescriptor column = Columns[ c ];
				int ordinal = column.Ordinal;

				int result;

				// check for nulls
				bool lhsIsNull = lhs.IsNull( ordinal );
				bool rhsIsNull = rhs.IsNull( ordinal );

				if ( lhsIsNull && rhsIsNull )
				{
					continue;
				}
				else if ( lhsIsNull )
				{
					result = -1;
				}
				else if ( rhsIsNull )
				{
					result = 1;
				}
				else if ( column.DataType == typeof( string ) )
				{
					// special case for strings, use the case sensitive matching if required.
					result = string.Compare( (string)lhs[ ordinal ], (string)rhs[ ordinal ], IgnoreCase );
				}
				else // any other primitive type (including DateTime)
				{
					// both objects *should* be of the same type based on the assumptions listed 
					// in the method summary.
					IComparable comparable = lhs[ ordinal ] as IComparable;

					if ( comparable != null )
					{
						result = comparable.CompareTo( rhs[ ordinal ] );
					}
					else
					{
						// all primitive types implement IComparable, so if we get here,
						// all that we can do is do a ToString comparison.
						result = string.Compare( lhs[ ordinal ].ToString(), rhs[ ordinal ].ToString(), IgnoreCase );
					}
				}

				if ( result != 0 )
				{
					return SortOrder == SortOrderEnum.Ascending ? result : -result;
				}

			}

			return 0; // rows are equal based on column comparison
		}

	}
}
