using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.Oz.Core
{
	public enum MessageCategory
	{
		Information,
		Warning,
		Error,
		Debug
	}

	public class MessageCategoryFormat
	{
		private static string[] _format = { "Information", "Warning", "Error", "Debug" };

		public static string ToString( MessageCategory category ) 
		{
			return _format[ (int)category ];
		}
	}
}
