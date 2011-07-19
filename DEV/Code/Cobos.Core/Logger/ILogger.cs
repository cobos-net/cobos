using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Core.Logger
{
	public interface ILogger
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		void Log( Exception e );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		void Log( CobosException e );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="category"></param>
		void Log( string entry, MessageCategory category );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void Information( string format, params object[] args );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void Warning( string format, params object[] args );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void Error( string format, params object[] args );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void Debug( string format, params object[] args );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="category"></param>
		/// <param name="timestamp"></param>
		void Log( string entry, MessageCategory category, DateTime timestamp );

	}
}
