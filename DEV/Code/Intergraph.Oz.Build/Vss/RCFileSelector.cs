using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.SourceSafe.Interop;
using Intergraph.Oz.Build.Configuration;

namespace Intergraph.Oz.Build.Vss
{
	public class RCFileSelector : FileExtensionMatcher, FileHelper.IFileSelector
	{
		/// <summary>
		/// 
		/// </summary>
		public RCFileSelector()
			: base( ".rc" )
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
				RCFile rcFile = RCFile.Analyse( vssItem );

				if ( rcFile != null )
				{
					Application.AddRCFile( rcFile );
				}
			}
		}
	}
}
