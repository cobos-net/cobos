using System;
using System.Data;

namespace Intergraph.AsiaPac.Data.Utilities
{
	public struct DataColumnDescriptor
	{
		public readonly int Ordinal;

		public readonly Type DataType;

		public DataColumnDescriptor( int ordinal, Type type )
		{
			Ordinal = ordinal;
			DataType = type;
		}

		public DataColumnDescriptor( DataColumn column )
		{
			Ordinal = column.Ordinal;
			DataType = column.DataType;
		}

		/// <summary>
		/// Helper method to convert an array of DataColumn objects.
		/// </summary>
		public static DataColumnDescriptor[] ConstructFrom( DataColumn[] columns )
		{
			if ( columns == null )
			{
				return null;
			}

			DataColumnDescriptor[] result = new DataColumnDescriptor[ columns.Length ];

			for ( int c = 0; c < columns.Length; ++c )
			{
				result[ c ] = new DataColumnDescriptor( columns[ c ] );
			}

			return result;
		}
	}
}
