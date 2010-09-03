using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intergraph.Oz.Utilities.Extensions;

namespace Intergraph.Oz.Utilities.Text
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
