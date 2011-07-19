using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Core
{
	public enum MessageCategory
	{
		Error,
		Warning,
		Information,
		Debug
	}

	public class MessageCategoryFormat
	{
		private static string[] _format = { "error", "warning", "information", "debug" };

		public static string ToString( MessageCategory category ) 
		{
			return _format[ (int)category ];
		}

		public static MessageCategory FromString( string category )
		{
			switch ( category.ToLower() )
			{
			case "error":
				return MessageCategory.Error;

			case "warning":
				return MessageCategory.Warning;

			case "information":
				return MessageCategory.Information;

			case "debug":
				return MessageCategory.Debug;

			// should be tolerant of errors and return the default level
			default:
				return MessageCategory.Information;
			}
		}
	}
}
