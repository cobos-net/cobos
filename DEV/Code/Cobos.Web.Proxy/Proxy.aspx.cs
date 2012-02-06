using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobos.Web.Proxy
{
	public partial class Proxy : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			Cobos.Web.Utilities.Handlers.ProxyHandler.Process( this, "Cobos.Web.Proxy" );
		}
	}
}