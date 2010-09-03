using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.Oz.Utilities.Text
{
	public static class RegExHelper
	{
		public static readonly string GuidPattern = @"\{[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}\}";
	}
}
