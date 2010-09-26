using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Data.Query
{
	public class CommentQuery : DatabaseQuery
	{
		public CommentQuery()
			: base( _sql, "EventComment" )
		{
		}

		//const string _sql = "select a.num_1, e.eid, e.comm, e.cterm, e.cpers, e.cdts, e.lin_grp, e.lin_ord from evcom e, aeven a where e.eid = a.eid and a.open_and_curent = 'T' and a.curent in ('T','O') and (e.comm_scope is null or e.comm_scope = a.ag_id)";

		const string _sql = "select a.num_1, e.eid, e.comm, e.cterm, e.cpers, e.cdts, e.lin_grp, e.lin_ord from evcom e, aeven a where e.eid = a.eid and a.open_and_curent = 'T' and a.curent in ('T','O') and (e.comm_scope is null or e.comm_scope = a.ag_id) order by e.cdts, e.cterm, e.lin_grp, e.lin_ord";

		//"select e.eid, e.comm, e.cterm, e.cpers, e.cdts, e.lin_grp, e.lin_ord " +
		//"from " + 
		//"evcom e, " + 
		//"(select eid, ag_id from aeven where open_and_curent = 'T' and curent in ('T','O')) a " + 
		//"where e.eid = a.eid " + 
		//"and (e.comm_scope is null or e.comm_scope = a.ag_id) " + 
		//"order by e.cdts, e.cterm, e.lin_grp, e.lin_ord";
	}
}
