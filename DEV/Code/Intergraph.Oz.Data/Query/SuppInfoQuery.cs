using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.Oz.Data.Query
{
	public class SuppInfoQuery : DatabaseQuery
	{
		public SuppInfoQuery( string tablename, string suppinfotable, int bitmask )
			: base( GetSuppInfoSelect( suppinfotable, bitmask ), tablename )
		{
		}

		public static string GetSuppInfoSelect( string tablename, int bitmask )
		{
			return string.Format( "select '{0}' as supp_info_table, aeven.num_1, {0}.* from {0} inner join aeven on {0}.eid = aeven.eid where aeven.open_and_curent = 'T' and aeven.curent in ('T', 'O') and BITAND( aeven.supp_info, {1} ) = {1}", tablename, bitmask );

			// old style not including num_1 from aeven
			//return string.Format( "select '{0}' as supp_info_table, {0}.* from {0} where {0}.eid in (select eid from aeven where open_and_curent = 'T' and curent in ('T', 'O') and BITAND( supp_info, {1} ) = {1} )", tablename, bitmask );
		}
	}
}
