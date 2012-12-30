// ============================================================================
// Filename: CobosException.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 21-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

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
