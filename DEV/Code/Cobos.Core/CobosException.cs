using System;
using Cobos.Core.Log;

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
		public CobosException( string message, LogWriter.LogLevelEnum category )
			: base( message )
		{
			Category = category;
			Timestamp = System.DateTime.Now;
		}

		/// <summary>
		/// 
		/// </summary>
		public readonly LogWriter.LogLevelEnum Category;
		
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
			: base( message, LogWriter.LogLevelEnum.Information )
		{
		}

		public CobosInformation( string format, params object[] args )
			: base ( string.Format( format, args ), LogWriter.LogLevelEnum.Information )
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class CobosWarning : CobosException
	{
		public CobosWarning( string message )
			: base( message, LogWriter.LogLevelEnum.Warning )
		{
		}
		
		public CobosWarning( string format, params object[] args )
			: base ( string.Format( format, args ), LogWriter.LogLevelEnum.Warning )
		{
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class CobosError : CobosException
	{
		public CobosError( string message )
			: base( message, LogWriter.LogLevelEnum.Error )
		{
		}

		public CobosError( string format, params object[] args )
			: base ( string.Format( format, args ), LogWriter.LogLevelEnum.Error )
		{
		}
	}

	/// <summary>
	///  
	/// </summary>
	public class CobosDebug : CobosException
	{
		public CobosDebug( string message )
			: base( message, LogWriter.LogLevelEnum.Debug )
		{
		}

		public CobosDebug( string format, params object[] args )
			: base ( string.Format( format, args ), LogWriter.LogLevelEnum.Debug )
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class CobosUserOperationCancelled : CobosException
	{
		public CobosUserOperationCancelled()
			: base( "Operation cancelled by user", LogWriter.LogLevelEnum.Information )
		{
		}

		public CobosUserOperationCancelled( string operation )
			: base( operation + " cancelled by user", LogWriter.LogLevelEnum.Information )
		{
		}

		public CobosUserOperationCancelled( string format, params object[] args )
			: base( string.Format( format, args ), LogWriter.LogLevelEnum.Information )
		{
		}
	}
}
