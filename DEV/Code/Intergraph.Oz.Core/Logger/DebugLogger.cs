using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.Oz.Core.Logger
{
	/// <summary>
	/// Debug logger for use by unit tests
	/// </summary>
	public class DebugLogger : ILogger
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void Log( Exception e )
		{
			DateTime timestamp = DateTime.Now;

			Log( String.Format( "{0} ({1})", e.Message, e.Source ), MessageCategory.Error, timestamp );

			Exception inner = e;

			while ( (inner = inner.InnerException) != null )
			{
				Log( String.Format( "{0} ({1})", inner.Message, inner.Source ), MessageCategory.Error, timestamp );
			}

			Log( e.StackTrace, MessageCategory.Error, timestamp );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void Log( IntergraphException e )
		{
			Log( e.Message, e.Category, e.Timestamp );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="category"></param>
		public void Log( string entry, MessageCategory category )
		{
			Log( entry, category, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Information( string format, params object[] args )
		{
			Log( String.Format( format, args ), MessageCategory.Information, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Warning( string format, params object[] args )
		{
			Log( String.Format( format, args ), MessageCategory.Warning, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Error( string format, params object[] args )
		{
			Log( String.Format( format, args ), MessageCategory.Error, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Debug( string format, params object[] args )
		{
			Log( String.Format( format, args ), MessageCategory.Debug, DateTime.Now );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="category"></param>
		/// <param name="timestamp"></param>
		public void Log( string entry, MessageCategory category, DateTime timestamp )
		{
			string categoryString = MessageCategoryFormat.ToString( category );

			string timestampString = timestamp.ToString( "s" );

			System.Diagnostics.Debug.WriteLine( "{0} {1}: {2}", timestampString, categoryString, entry );
		}
	}
}
