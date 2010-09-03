using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.SourceSafe.Interop;
using Intergraph.Oz.Utilities.Logger;

namespace Intergraph.Oz.Build.Configuration
{
	/// <summary>
	/// Partial declaration of Generated class to add useful functionality
	/// </summary>
	public partial class SolutionFile : BuildEntity 
	{
		//public SolutionFile()
		//{
		//   // parameterless constructor for Xml serialization
		//}

		public SolutionFile( string nameValue, string specValue )
		{
			nameField = nameValue;
			specField = specValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vssItem"></param>
		public static SolutionFile Analyse( IVSSItem vssItem )
		{
			if ( Application.VisualStudio == null )
			{
				Application.Logger.Error( "Failed to analyse {0}, Visual Studio is not initialised", vssItem.Spec );
				return null;
			}

			try
			{
				// only need to get the solution file, don't need any project files etc.
				string working = null;
				vssItem.Get( ref working, 0 );

				SolutionFile slnFile = new SolutionFile( vssItem.Name, vssItem.Spec );

				slnFile.configurations = Application.VisualStudio.GetBuildConfigurations( working );

				return slnFile;
			}
			catch ( System.Exception e )
			{
				Application.Logger.Log( e );
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vssItem"></param>
		public void Build( IVSSDatabase vssDB, bool buildDebug, bool buildRelease )
		{
			if ( !this.includeInBuild )
			{
				Application.Logger.Information( "{0} not build as directed by build manifest", this.spec );
				return;
			}

			if ( this.configurations == null || this.configurations.Length == 0 )
			{
				Application.Logger.Warning( "No build configurations supplied for {0}", this.spec );
				return;
			}

			IVSSItem vssItem = null;

			try
			{
				vssItem = vssDB.get_VSSItem( this.spec, false );

				Application.VisualStudio.BuildSolutionConfigurations( vssItem.LocalSpec, this.configurations, buildDebug, buildRelease );
			}
			catch ( System.Exception e )
			{
				Application.Logger.Log( e );
			}
			finally
			{
				vssItem = null;
			}
		}

		public class Comparer : IComparer
		{
			public int Compare( object lhs, object rhs )
			{
				long comparison = ((SolutionFile)lhs).buildOnPassField - ((SolutionFile)rhs).buildOnPassField;

				return (int)comparison;
			}
		}
	}
}
