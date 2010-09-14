using System;

namespace Intergraph.Oz.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class IntergraphException : System.Exception
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="category"></param>
		public IntergraphException( string message, MessageCategory category )
			: base( message )
		{
			Category = category;
			Timestamp = System.DateTime.Now;
		}

		/// <summary>
		/// 
		/// </summary>
		public readonly MessageCategory Category;
		
		/// <summary>
		/// 
		/// </summary>
		public readonly DateTime Timestamp;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public static void ReportToConsole( System.Exception e )
		{
			System.Console.WriteLine( e.Message );

			if ( e.InnerException != null )
			{
				ReportToConsole( e.InnerException );
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class IntergraphInformation : IntergraphException
	{
		public IntergraphInformation( string message )
			: base( message, MessageCategory.Information )
		{
		}

		public IntergraphInformation( string format, params object[] args )
			: base ( string.Format( format, args ), MessageCategory.Information )
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class IntergraphWarning : IntergraphException
	{
		public IntergraphWarning( string message )
			: base( message, MessageCategory.Warning )
		{
		}
		
		public IntergraphWarning( string format, params object[] args )
			: base ( string.Format( format, args ), MessageCategory.Warning )
		{
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class IntergraphError : IntergraphException
	{
		public IntergraphError( string message )
			: base( message, MessageCategory.Error )
		{
		}

		public IntergraphError( string format, params object[] args )
			: base ( string.Format( format, args ), MessageCategory.Error )
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class IntergraphDebug : IntergraphException
	{
		public IntergraphDebug( string message )
			: base( message, MessageCategory.Debug )
		{
		}

		public IntergraphDebug( string format, params object[] args )
			: base ( string.Format( format, args ), MessageCategory.Debug )
		{
		}
	}

}
