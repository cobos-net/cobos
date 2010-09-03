using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.SourceSafe.Interop;

namespace Intergraph.Oz.Build.Vss
{
	/// <summary>
	/// 
	/// </summary>
	public class FileHelper
	{
		/// <summary>
		/// 
		/// </summary>
		public interface IFileSelector
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="vssItem"></param>
			void Select( IVSSItem vssItem );

			/// <summary>
			/// 
			/// </summary>
			bool Recursive { get; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vssItem"></param>
		/// <param name="match"></param>
		/// <param name="found"></param>
		/// <param name="recursive"></param>
		public static void FindAllFiles( IVSSItem vssItem, IFileSelector selector )
		{
			if ( vssItem.Type == (int)VSSItemType.VSSITEM_FILE )
			{
				selector.Select( vssItem );
				return;
			}

			foreach ( IVSSItem vssChild in vssItem.get_Items( false ) )
			{
				if ( vssChild.Type == (int)VSSItemType.VSSITEM_FILE )
				{
					selector.Select( vssChild );
				}
				else if ( selector.Recursive )
				{
					FindAllFiles( vssChild, selector );
				}
			}
		}
	}

	/// <summary>
	/// Match a filename against a search string where:
	/// The search string matches any portion of the filename OR
	/// The search string 
	/// </summary>
	public class FilenameMatcher
	{
		/// <summary>
		/// 
		/// </summary>
		public enum MatchType
		{
			FileMatchAny,
			FileMatchBegin,
			FileMatchEnd
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="match"></param>
		/// <param name="matchType"></param>
		public FilenameMatcher( string match, MatchType matchType )
		{
			_match = match.ToUpper();
			_matchType = matchType;
		}

		/// <summary>
		/// Find out whether the item filename matches the search string.
		/// </summary>
		/// <param name="vssItem"></param>
		/// <returns></returns>
		public bool matches( IVSSItem vssItem )
		{
			string upperName = vssItem.Name.ToUpper();

			bool matches = false;

			switch ( _matchType )
			{
			case MatchType.FileMatchAny:

				matches = upperName.Contains( _match );
				break;

			case MatchType.FileMatchBegin:

				if ( upperName.Length >= _match.Length )
				{
					matches = upperName.Substring( 0, _match.Length ) == _match;
				}
				break;

			case MatchType.FileMatchEnd:

				if ( upperName.Length >= _match.Length )
				{
					matches = upperName.Substring( upperName.Length - _match.Length ) == _match;
				}
				break;
			}

			return matches;
		}

		/// <summary>
		/// 
		/// </summary>
		private string _match;

		/// <summary>
		/// 
		/// </summary>
		private MatchType _matchType;
	}

	/// <summary>
	/// Match a filename based on it's exension
	/// </summary>
	public class FileExtensionMatcher : FilenameMatcher
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="extension"></param>
		public FileExtensionMatcher( string extension )
			: base( extension, MatchType.FileMatchEnd )
		{
		}
	}
}
