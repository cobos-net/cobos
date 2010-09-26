using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intergraph.AsiaPac.Utilities.Extensions;

namespace Intergraph.AsiaPac.Utilities.Text
{
	public static class GuidHelper
	{
		public static string GUID()
		{
			return Guid.NewGuid().ToString().ToUpper();
		}

		public static string GUID( bool quote )
		{
			return (quote) ? (GuidHelper.GUID().Quote()) : GuidHelper.GUID();
		}
	}
}
