using System;

namespace Cobos.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class CobosException : System.Exception
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="category"></param>
		public CobosException( string message, MessageCategory category )
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
	public class CobosInformation : CobosException
	{
		public CobosInformation( string message )
			: base( message, MessageCategory.Information )
		{
		}

		public CobosInformation( string format, params object[] args )
			: base ( string.Format( format, args ), MessageCategory.Information )
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class CobosWarning : CobosException
	{
		public CobosWarning( string message )
			: base( message, MessageCategory.Warning )
		{
		}
		
		public CobosWarning( string format, params object[] args )
			: base ( string.Format( format, args ), MessageCategory.Warning )
		{
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class CobosError : CobosException
	{
		public CobosError( string message )
			: base( message, MessageCategory.Error )
		{
		}

		public CobosError( string format, params object[] args )
			: base ( string.Format( format, args ), MessageCategory.Error )
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class CobosDebug : CobosException
	{
		public CobosDebug( string message )
			: base( message, MessageCategory.Debug )
		{
		}

		public CobosDebug( string format, params object[] args )
			: base ( string.Format( format, args ), MessageCategory.Debug )
		{
		}
	}

}
