using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.SourceSafe.Interop;
using Intergraph.Oz.Build.Configuration;

namespace Intergraph.Oz.Build.Vss
{
	public class SolutionFileSelector : FileExtensionMatcher, FileHelper.IFileSelector
	{
		public SolutionFileSelector()
			: base( ".sln" )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Recursive
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vssItem"></param>
		public void Select( IVSSItem vssItem )
		{
			if ( matches( vssItem ) )
			{
				SolutionFile slnFile = SolutionFile.Analyse( vssItem );

				if ( slnFile != null )
				{
					Application.AddSolutionFile( slnFile );
				}
			}
		}
	}
}
