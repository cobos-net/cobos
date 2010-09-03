using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using EnvDTE;
using EnvDTE80;
using EnvDTE90;
using Intergraph.Oz.Utilities;
using Intergraph.Oz.Utilities.File;
using Intergraph.Oz.Utilities.Extensions;
using Intergraph.Oz.Utilities.Logger;
using Intergraph.Oz.Utilities.Text;
using Intergraph.Oz.Build.Configuration;

using Diagnostics = System.Diagnostics;

namespace Intergraph.Oz.Build.Vs
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class VisualStudio : IDisposable
	{
		#region Abstract Interface

		/// <summary>
		/// 
		/// </summary>
		public abstract void Dispose();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="solution"></param>
		/// <returns></returns>
		public abstract SolutionBuildConfiguration[] GetBuildConfigurations( string solution );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="solution"></param>
		/// <param name="configurations"></param>
		public abstract void BuildSolutionConfigurations( string solution, SolutionBuildConfiguration[] configurations, bool buildDebug, bool buildRelease );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="solution"></param>
		/// <param name="buildDebug"></param>
		/// <param name="buildRelease"></param>
		public abstract void BuildSolution( string solution, bool buildDebug, bool buildRelease );

		#endregion

		#region Factory Creation

		/// <summary>
		/// Static factory method
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public static VisualStudio Create( Configuration.BuildEnvironmentEnum environment )
		{
			try
			{
				switch ( environment )
				{
				case BuildEnvironmentEnum.Item70:
					return new VisualStudioImpl< VisualStudio70 >();

				case BuildEnvironmentEnum.Item71:
					return new VisualStudioImpl< VisualStudio71 >();

				case BuildEnvironmentEnum.Item80:
					return new VisualStudioImpl< VisualStudio80 >();

				case BuildEnvironmentEnum.Item90:
					return new VisualStudioImpl< VisualStudio90 >();

				default:
					throw new IntergraphError( "Unsupported version of Microsoft Visual Studio" );
				}

			}
			catch ( System.Exception e )
			{
				Application.Logger.Warning( "Exception thrown during Visual Studio instantiation.  Check that you have the correct version of Visual Studio installed" );
				throw e;
			}
		}

		#endregion

		#region Solution Preparation

		/// <summary>
		/// The automation will stall if it detects that a solution is under source control.
		/// The UI blocks while a Yes/No dialog is displayed.
		/// Also have to make sure various files are read-write enabled.
		/// </summary>
		/// <param name="solution"></param>
		public static void PrepareSolutionForBuild( string solution )
		{
			if ( !File.Exists( solution ) )
			{
				return;
			}

			// make read-write in case some conversion needs to take place
			FileUtility.MakeReadWrite( solution );

			string folder = Path.GetDirectoryName( solution );

			// first delete all related source control files
			string error = null;

			if ( !FileUtility.DeleteAllFiles( folder, "*.scc", true, ref error ) )
			{
				Application.Logger.Error( error );
			}

			if ( !FileUtility.DeleteAllFiles( folder, "*.vssscc", true, ref error ) )
			{
				Application.Logger.Error( error );
			}

			if ( !FileUtility.DeleteAllFiles( folder, "*.vspscc", true, ref error ) )
			{
				Application.Logger.Error( error );
			}

			// also, sometimes the *.ncb files are checked into SourceSafe... make sure they're read-write
			if ( !FileUtility.MakeAllFilesReadWrite( folder, "*.ncb", true, ref error ) )
			{
				Application.Logger.Error( error );
			}

			// finally, since the binaries are checked in, make sure that they are read-write
			if ( !FileUtility.MakeAllFilesReadWrite( folder, "*.exe", true, ref error ) )
			{
				Application.Logger.Error( error );
			}
			if ( !FileUtility.MakeAllFilesReadWrite( folder, "*.lib", true, ref error ) )
			{
				Application.Logger.Error( error );
			}
			if ( !FileUtility.MakeAllFilesReadWrite( folder, "*.dll", true, ref error ) )
			{
				Application.Logger.Error( error );
			}
			if ( !FileUtility.MakeAllFilesReadWrite( folder, "*.ocx", true, ref error ) )
			{
				Application.Logger.Error( error );
			}
			if ( !FileUtility.MakeAllFilesReadWrite( folder, "*.tlb", true, ref error ) )
			{
				Application.Logger.Error( error );
			}

			// remove the embedded source control from the solution file
			RemoveSolutionSourceControl( solution );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="solution"></param>
		private static void RemoveSolutionSourceControl( string solution )
		{
			// remove the source control from the solution file
			// this is within the following block:
			//
			//GlobalSection(SourceCodeControl) = preSolution
			//   SccNumberOfProjects = 2
			//   SccLocalPath0 = .
			//   CanCheckoutShared = false
			//   SolutionUniqueID = {E365626A-AA82-4759-8219-86B2E08A6346}
			//   SccProjectUniqueName1 = SelectTabs.vcproj
			//   SccLocalPath1 = .
			//   CanCheckoutShared = false
			//EndGlobalSection

			// while we're doing that, we'll pick up any referenced projects:
			string[] projects = null;

			StreamReader reader = null;
			StreamWriter writer = null;

			try
			{
				string tmpname = solution + ".tmp";

				reader = new StreamReader( solution );
				writer = new StreamWriter( tmpname );

				bool inSourceControlSection = false, removedSourceControlSection = false;

				while ( !reader.EndOfStream )
				{
					string line = reader.ReadLine();

					if ( !removedSourceControlSection )
					{
						if ( !inSourceControlSection )
						{
							// look for the beginning...
							if ( inSourceControlSection = Regex.IsMatch( line, @".*GlobalSection\(SourceCodeControl\) = preSolution.*" ) )
							{
								continue;
							}
						}
						else
						{
							// look for the end...
							removedSourceControlSection = Regex.IsMatch( line, ".*EndGlobalSection.*" );
							continue;
						}
					}

					// look for the projects:
					//
					// Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "VSSReportCheckout", "VSSReportCheckout\VSSReportCheckout.csproj", "{000CF047-1FCF-4051-83F8-7C19860F3E3F}"
					
					string[] tokens = Regex.Split( line, @".*Project\(""" + RegExHelper.GuidPattern + @"""\) = "".*"", ""(.*)"", .*" );

					if ( tokens.Length == 3 )
					{
						projects = projects.Append( tokens[ 1 ] );
					}
					
					writer.WriteLine( line );
				}

				reader.Close();
				reader = null;
				writer.Close();
				writer = null;

				if ( removedSourceControlSection )
				{
					File.Delete( solution );
					File.Move( tmpname, solution );

					Application.Logger.Information( "Removed source control information from {0}", solution );
				}
				else
				{
					File.Delete( tmpname );
				}

				// now remove any embedded source control from all related projects...
				if ( projects != null )
				{
					string folder = Path.GetDirectoryName( solution );

					foreach ( string p in projects )
					{
						RemoveProjectSourceControl( folder + @"\" + p );
					}
				}
			}
			catch ( System.Exception e )
			{
				Application.Logger.Log( e );
			}
			finally
			{
				if ( reader != null )
				{
					reader.Close();
					reader = null;
				}
				if ( writer != null )
				{
					writer.Close();
					writer = null;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		private static void RemoveProjectSourceControl( string project )
		{
			if ( !File.Exists( project ) )
			{
				return; // probably not checked out in analysis mode
			}

			// Make read-write in case some automatic conversion needs to take place...
			FileUtility.MakeReadWrite( project );

			// remove source control attributes from the project Xml:
			//
			// <VisualStudioProject
			//			ProjectType="Visual C++"
			//			Version="7.10"
			//			Name="SelectTabs"
			//	-->	SccProjectName="&quot;$/ESTA/7.9/Development/Fire/SelectTabs&quot;, NIGKAAAA"
			//	-->	SccAuxPath=""
			//	-->	SccLocalPath="."
			//	-->	SccProvider="MSSCCI:Microsoft Visual SourceSafe"
			//			Keyword="MFCProj">
			//
			// OR:
			//
			// <VisualStudioProject>
			//		<CSHARP
			//			ProjectType = "Local"
			//			ProductVersion = "7.10.3077"
			//			SchemaVersion = "2.0"
			//			ProjectGuid = "{BE272C26-C63D-4187-9933-42B60EED8C3E}"
			//	-->	SccProjectName = "SAK"
			//	-->	SccLocalPath = "SAK"
			//	-->	SccAuxPath = "SAK"
			//	-->	SccProvider = "SAK"
			//		>
			// OR similar to CSHARP for VisualBasic...

			try
			{
				XmlDocument xmlDoc = new XmlDocument();

				xmlDoc.Load( project );

				XmlAttributeCollection atts = null;

				XmlNode node = xmlDoc.DocumentElement.Attributes.GetNamedItem( "ProjectType" );

				if ( node != null && node.Value == "Visual C++" )
				{
					atts = xmlDoc.DocumentElement.Attributes;
				}
				else
				{
					XmlNode projectNode = xmlDoc.DocumentElement.FirstChild;

					if ( projectNode.Name != "CSHARP" && projectNode.Name != "VisualBasic" && projectNode.Name != "VISUALJSHARP" )
					{
						Application.Logger.Error( "Unknown VS project format {0} when trying to remove source control", projectNode.Name );
						return;
					}

					atts = projectNode.Attributes;
				}

				bool removedSomething = false;
				
				removedSomething |= atts.RemoveNamedItem( "SccProjectName" ) != null;
				removedSomething |= atts.RemoveNamedItem( "SccAuxPath" ) != null;
				removedSomething |= atts.RemoveNamedItem( "SccLocalPath" ) != null;
				removedSomething |= atts.RemoveNamedItem( "SccProvider" ) != null;

				if ( removedSomething )
				{
					xmlDoc.Save( project );
					Application.Logger.Information( "Removed source control from {0}", project );
				}
			}
			catch ( System.Exception e )
			{
				Application.Logger.Log( e );
			}
		}

		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DTEType"></typeparam>
	/// <typeparam name="SolutionType"></typeparam>
	/// <typeparam name="ProjectType"></typeparam>
	class VisualStudioImpl<DTEType> : VisualStudio where DTEType : VisualStudioDTE, new()
	{
		/// <summary>
		/// 
		/// </summary>
		private DTEType _visualStudio = new DTEType();

		public VisualStudioImpl()
		{
			BuildEvents build = _visualStudio.Events.BuildEvents;

			build.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler( BuildEvents_OnBuildBegin );
			build.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler( BuildEvents_OnBuildDone );

			//OutputWindowEvents outputEvents = _visualStudio.Events.get_OutputWindowEvents( "Build" );

			//if ( outputEvents != null )
			//{
			//   outputEvents.PaneUpdated += new _dispOutputWindowEvents_PaneUpdatedEventHandler( Output_PaneUpdated );
			//}

			//EnvDTE.Window window = (EnvDTE.Window)_visualStudio.Windows.Item( Constants.vsWindowKindOutput );

			//if ( window != null )
			//{
			//   OutputWindow outputWindow = window.Object as OutputWindow;

			//   if ( outputWindow != null )
			//   {
			//      Window filter = outputWindow.ActivePane as Window;
			//   }
			//}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Dispose()
		{
			if ( _visualStudio != null )
			{
				_visualStudio.Quit();
				_visualStudio = default(DTEType);
			}
		}

		#region Build Event Handlers

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="action"></param>
		protected void BuildEvents_OnBuildBegin( vsBuildScope scope, vsBuildAction action )
		{
			//Application.Logger.Information( "Starting: {0}", FormatBuildEvent( scope, action ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		private void BuildEvents_OnBuildDone( vsBuildScope scope, vsBuildAction action )
		{
			//Application.Logger.Information( "Finished: {0}", FormatBuildEvent( scope, action ) );

			//if ( action != vsBuildAction.vsBuildActionBuild && action != vsBuildAction.vsBuildActionRebuildAll )
			//{
			//   return;
			//}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		protected string FormatBuildEvent( vsBuildScope scope, vsBuildAction action )
		{
			string strScope = "[Unknown Scope]";

			switch ( scope )
			{
			case vsBuildScope.vsBuildScopeBatch:
				strScope = "Batch";
				break;
			case vsBuildScope.vsBuildScopeProject:
				strScope = "Project";
				break;
			case vsBuildScope.vsBuildScopeSolution:
				strScope = "Solution";
				break;
			}

			string strAction = "[Unknown Action]";

			switch ( action )
			{
			case vsBuildAction.vsBuildActionBuild:
				strAction = "Build";
				break;
			case vsBuildAction.vsBuildActionClean:
				strAction = "Clean";
				break;
			case vsBuildAction.vsBuildActionDeploy:
				strAction = "Deploy";
				break;
			case vsBuildAction.vsBuildActionRebuildAll:
				strAction = "Rebuild All";
				break;
			}

			return string.Format( "{0} {1}", strAction, strScope );
		}

		/// <summary>
		/// The output window is constantly updated, sometimes with partial
		/// line updated and sometimes without the current line being changed.
		/// This helper class ensures that we 
		/// </summary>
		private class OutputWindowBuffer : IDisposable 
		{
			/// <summary>
			/// The output window is updated by many different threads
			/// </summary>
			private object _mutex = new object();

			/// <summary>
			/// 
			/// </summary>
			private int _currentLineNum = 0;

			/// <summary>
			/// 
			/// </summary>
			private string _currentLineBuffer = "";

			public void OnUpdate( OutputWindowPane pane )
			{
				lock ( _mutex )
				{
					TextSelection sel = pane.TextDocument.Selection;
					sel.SelectLine();

					if ( sel.IsEmpty )
					{
						return;
					}

					int currentLineNum = sel.CurrentLine;

					// build up the string
					if ( currentLineNum == _currentLineNum )
					{
						_currentLineBuffer = sel.Text;
						return;
					}

					// changed line, log the buffer and update
					LogCurrentLine();

					_currentLineBuffer = sel.Text;
					_currentLineNum = currentLineNum;
				}
			}

			void LogCurrentLine()
			{
				_currentLineBuffer = _currentLineBuffer.Trim();
				_currentLineBuffer = _currentLineBuffer.TrimEnd( '\r', '\n' );

				if ( _currentLineBuffer == "" )
				{
					return;
				}

				if ( Regex.IsMatch( _currentLineBuffer, _reBuildError ) )
				{
					Application.Logger.Error( _currentLineBuffer );
				}
				else if ( Regex.IsMatch( _currentLineBuffer, _reBuildSummary ) )
				{
					string[] result = Regex.Split( _currentLineBuffer, _reBuildSummary );

					int numErrors = Convert.ToInt32( result[ 1 ] );
					int numWarnings = Convert.ToInt32( result[ 2 ] );

					if ( numErrors > 0 )
					{
						Application.Logger.Error( _currentLineBuffer );
					}
					else if ( numWarnings > 0 )
					{
						Application.Logger.Warning( _currentLineBuffer );
					}
					else
					{
						Application.Logger.Information( _currentLineBuffer );
					}
				}
				else if ( _currentLineBuffer.Contains( "error" ) )
				{
					Application.Logger.Debug( _currentLineBuffer );
				}
				else
				{
					System.Console.WriteLine( _currentLineBuffer );
				}
			}

			bool _disposed = false;

			/// <summary>
			/// 
			/// </summary>
			public void Dispose()
			{
				lock ( _mutex )
				{
					if ( _disposed )
					{
						return;
					}
					_disposed = true;
				}

				LogCurrentLine();
				_currentLineBuffer = "";
			}

		}
		
		/// <summary>
		/// 
		/// </summary>
		OutputWindowBuffer _outputWindowBuffer = null;
		
		/// <summary>
		/// Regular expression to match a build error:
		///	
		/// </summary>
		private static readonly string _reBuildError = @".* error (BK|C|CS|LNK|RC|RW|D|PRJ|MIDL)\d{3,4}\s?:";

		/// <summary>
		/// Match the build summary"
		///	rtoc - 2 error(s), 8 warning(s)
		///	Compile complete -- 0 errors, 2 warnings
		/// </summary>
		private static readonly string _reBuildSummary = @".* -{1,2} (\d+) error\(?s\)?, (\d+) warning\(?s\)?";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pane"></param>
		private void Output_PaneUpdated( OutputWindowPane pane )
		{
			if ( _outputWindowBuffer != null )
			{
				_outputWindowBuffer.OnUpdate( pane );
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="solution"></param>
		/// <returns></returns>
		public override SolutionBuildConfiguration[] GetBuildConfigurations( string solution )
		{
			try
			{
				Application.Logger.Information( "Analysing {0}", solution );

				// if we don't do this then the automation may stall while Yes/No UI message dialogs are displayed 
				PrepareSolutionForBuild( solution );

				_visualStudio.OpenSolution( solution );

				return GetBuildConfigurations( _visualStudio.SolutionBuild.SolutionConfigurations );
			}
			catch ( System.Exception e )
			{
				Application.Logger.Log( e );
				return null;
			}
			finally
			{
				_visualStudio.CloseSolution();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="solutionConfigurations"></param>
		/// <returns></returns>
		private SolutionBuildConfiguration[] GetBuildConfigurations( SolutionConfigurations solutionConfigurations )
		{
			SolutionBuildConfiguration[] foundConfig = null;

			// the DTE objects are VB style collections with 1 based indices
			for ( int c = 1; c <= solutionConfigurations.Count; ++c )
			{
				SolutionBuildConfiguration newConfig = new SolutionBuildConfiguration();
				newConfig.Value = solutionConfigurations.Item( c ).Name;

				foundConfig = foundConfig.Append( newConfig );

				Application.Logger.Information( "Found {0} build configuration", newConfig.Value );
			}

			return foundConfig;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="solution"></param>
		/// <param name="configurations"></param>
		public override void BuildSolutionConfigurations( string solution, SolutionBuildConfiguration[] buildConfigurations, bool buildDebug, bool buildRelease )
		{
			int numAttemptsRemaining = 5;

			while ( numAttemptsRemaining > 0 )
			{
				try
				{
					TryBuildSolutionConfigurations( solution, buildConfigurations, buildDebug, buildRelease );
					break;
				}
				catch ( System.Exception e )
				{
					// when the automation server throws an exception, it generally doesn't
					// recover, so restart the process and try again....
					Application.Logger.Log( e );
					Application.Logger.Information( "Restarting Visual Studio..." );
					_visualStudio.Restart();
					--numAttemptsRemaining;
				}
			}

			if ( numAttemptsRemaining == 0 )
			{
				Application.Logger.Error( "Failed to build {0} after 5 attempts", solution );
			}
		}

		public void TryBuildSolutionConfigurations( string solution, SolutionBuildConfiguration[] buildConfigurations, bool buildDebug, bool buildRelease )
		{
			try
			{
				Application.Logger.Information( "Starting build for {0}...", solution );

				// if we don't do this then the automation may stall while Yes/No UI message dialogs are displayed 
				PrepareSolutionForBuild( solution );

				_visualStudio.OpenSolution( solution );

				_outputWindowBuffer = new OutputWindowBuffer();

				OutputWindowEvents outputEvents = _visualStudio.Events.get_OutputWindowEvents( "Build" );

				if ( outputEvents != null )
				{
					outputEvents.PaneUpdated += new _dispOutputWindowEvents_PaneUpdatedEventHandler( Output_PaneUpdated );
				}

				foreach ( SolutionBuildConfiguration buildConfig in buildConfigurations )
				{
					if ( !buildConfig.includeInBuild )
					{
						if ( !buildConfig.includeInBuild )
						{
							Application.Logger.Information( "{0} configuration not built for {1} as directed by build manifest", buildConfig.Value, solution );
							continue;
						}
					}

					string buildConfigValue = buildConfig.Value.ToUpper();

					if ( !buildDebug && buildConfigValue.Contains( "DEBUG" ) )
					{
						Application.Logger.Information( "{0} configuration not built for {1} as directed by build manifest (buildDebug is false)", buildConfig.Value, solution );
						continue;
					}

					if ( !buildRelease && buildConfigValue.Contains( "RELEASE" ) )
					{
						Application.Logger.Information( "{0} configuration not built for {1} as directed by build manifest (buildRelease is false)", buildConfig.Value, solution );
						continue;
					}

					Application.Logger.Information( "Starting {0} configuration for {1}...", buildConfig.Value, solution );
	
					// find the configuration by name
					SolutionConfiguration solutionConfig = null;
					SolutionConfigurations solutionConfigurations = _visualStudio.SolutionBuild.SolutionConfigurations;

					// the DTE objects are VB style collections with 1 based indices
					for ( int c = 1; c <= solutionConfigurations.Count; ++c )
					{
						if ( solutionConfigurations.Item( c ).Name == buildConfig.Value )
						{
							solutionConfig = solutionConfigurations.Item( c );
						}
					}

					if ( solutionConfig == null )
					{
						Application.Logger.Error( "Could not find {0} configuration in {1}", buildConfig.Value, solution );
						continue;
					}
					
					solutionConfig.Activate();

					_visualStudio.SolutionBuild.Clean( true );

					_visualStudio.SolutionBuild.Build( true );

					_visualStudio.CloseSolution();
				}
			}
			catch ( System.Exception e )
			{
				throw e;
			}
			finally
			{
				if ( _outputWindowBuffer != null )
				{
					_outputWindowBuffer.Dispose(); // flush to log file
					_outputWindowBuffer = null;
				}
			}
		}

		
		public override void BuildSolution( string solution, bool buildDebug, bool buildRelease )
		{
			int numAttemptsRemaining = 5;

			while ( numAttemptsRemaining > 0 )
			{
				try
				{
					TryBuildSolution( solution, buildDebug, buildRelease );
					break;
				}
				catch ( System.Exception e )
				{
					// when the automation server throws an exception, it generally doesn't
					// recover, so restart the process and try again....
					Application.Logger.Log( e );
					Application.Logger.Information( "Restarting Visual Studio..." );
					_visualStudio.Restart();
					--numAttemptsRemaining;
				}
			}

			if ( numAttemptsRemaining == 0 )
			{
				Application.Logger.Error( "Failed to build {0} after 5 attempts", solution );
			}
		}

		public void TryBuildSolution( string solution, bool buildDebug, bool buildRelease )
		{
			try
			{
				Application.Logger.Information( "Starting build for {0}...", solution );

				// if we don't do this then the automation may stall while Yes/No UI message dialogs are displayed 
				PrepareSolutionForBuild( solution );

				_visualStudio.OpenSolution( solution );

				_outputWindowBuffer = new OutputWindowBuffer();

				OutputWindowEvents outputEvents = _visualStudio.Events.get_OutputWindowEvents( "Build" );

				if ( outputEvents != null )
				{
					outputEvents.PaneUpdated += new _dispOutputWindowEvents_PaneUpdatedEventHandler( Output_PaneUpdated );
				}

				SolutionConfigurations solutionConfigurations = _visualStudio.SolutionBuild.SolutionConfigurations;

				// the DTE objects are VB style collections with 1 based indices
				for ( int c = 1; c <= solutionConfigurations.Count; ++c )
				{
					SolutionConfiguration solutionConfig = solutionConfigurations.Item( c );

					string configName = solutionConfig.Name.ToUpper();

					if ( !buildDebug && configName.Contains( "DEBUG" ) )
					{
						Application.Logger.Information( "{0} configuration not built for {1} as directed by build manifest (buildDebug is false)", configName, solution );
						continue;
					}

					if ( !buildRelease && configName.Contains( "RELEASE" ) )
					{
						Application.Logger.Information( "{0} configuration not built for {1} as directed by build manifest (buildRelease is false)", configName, solution );
						continue;
					}

					solutionConfig.Activate();

					_visualStudio.SolutionBuild.Clean( true );

					_visualStudio.SolutionBuild.Build( true );

					_visualStudio.CloseSolution();
				}
			}
			catch ( System.Exception e )
			{
				throw e;
			}
			finally
			{
				if ( _outputWindowBuffer != null )
				{
					_outputWindowBuffer.Dispose(); // flush to log file
					_outputWindowBuffer = null;
				}
			}
		}
	}

	/// <summary>
	/// temporary workarounds while I figure out how to specify unrelated interfaces in a generic where constraint e.g.
	/// 	public class VisualStudioImpl<DTEType, SolutionType, ProjectType> : VisualStudio where DTEType : DTE, DTE2 etc...
	/// </summary>
	abstract class VisualStudioDTE
	{
		public abstract void CloseSolution();

		public abstract void OpenSolution( string file );

		public abstract void Quit();

		public abstract void Restart();

		public abstract Events Events
		{
			get;
		}
		
		public abstract SolutionBuild SolutionBuild
		{
			get;
		}

		public abstract Windows Windows
		{
			get;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DTEType"></typeparam>
	abstract class VisualStudioDTEImpl<DTEType> : VisualStudioDTE
	{
		protected DTEType _visualStudio = default( DTEType );

		readonly string _progID;

		int _processID;

		public VisualStudioDTEImpl( string progID )
		{
			_progID = progID;

			Start();
		}

		public override void Restart()
		{
			// The Visual Studio automation is a little bit unreliable, in particular it appears
			// to stop responding after building 50+ solutions in succession...
			// The only way I can think to resolve this is to simply terminate the instance of
			// Visual Studio that we have started and re-create a new instance.

			if ( _visualStudio != null )
			{
				Diagnostics.Process process = Diagnostics.Process.GetProcessById( _processID );

				if ( process != null )
				{
					process.Kill();
				}

				_visualStudio = default( DTEType );
			}

			Start();
		}

		private void Start()
		{
			// We may need to terminate this process unexpectedly (see Restart).  Make sure we know
			// which process we create (there may be other devenv process running), so we can kill 
			// it if we need to.
			HashSet<int> originalProcesses = new HashSet<int>();

			Diagnostics.Process[] processes = Diagnostics.Process.GetProcessesByName( "devenv" );

			foreach ( Diagnostics.Process process in processes )
			{
				originalProcesses.Add( process.Id );
			}

			_visualStudio = (DTEType)Microsoft.VisualBasic.Interaction.CreateObject( _progID, "" );

			processes = Diagnostics.Process.GetProcessesByName( "devenv" );

			foreach ( Diagnostics.Process process in processes )
			{
				if ( !originalProcesses.Contains( process.Id ) )
				{
					_processID = process.Id;
					break;
				}
			}
		}
	}
	

	/// <summary>
	/// Visual Studio .NET
	/// </summary>
	class VisualStudio70 : VisualStudioDTEImpl< EnvDTE.DTE >	
	{
		public VisualStudio70()
			: base( "VisualStudio.DTE.7.0" )
		{
			//_visualStudio.SuppressUI = true;
		}

		public override void CloseSolution()
		{
			_visualStudio.Solution.Close( false );
		}

		public override void OpenSolution( string file )
		{
			_visualStudio.Solution.Open( file );
		}

		public override void Quit()
		{
			_visualStudio.Quit();
			_visualStudio = null;
		}

		public override Events Events
		{
			get { return _visualStudio.Events; }
		}

		public override SolutionBuild SolutionBuild
		{
			get { return _visualStudio.Solution.SolutionBuild; }
		}

		public override Windows Windows
		{
			get { return _visualStudio.Windows; }
		}
	}

	/// <summary>
	/// Visual Studio .NET 2003
	/// </summary>
	class VisualStudio71 : VisualStudioDTEImpl<EnvDTE.DTE>
	{
		public VisualStudio71()
			: base( "VisualStudio.DTE.7.1" )
		{
			//_visualStudio.SuppressUI = true;
		}

		public override void CloseSolution()
		{
			_visualStudio.Solution.Close( false );
		}

		public override void OpenSolution( string file )
		{
			_visualStudio.Solution.Open( file );
		}

		public override void Quit()
		{
			_visualStudio.Quit();
			_visualStudio = null;
		}

		public override Events Events
		{
			get { return _visualStudio.Events; }
		}

		public override SolutionBuild SolutionBuild
		{
			get { return _visualStudio.Solution.SolutionBuild; }
		}

		public override Windows Windows
		{
			get { return _visualStudio.Windows; }
		}
	}

	/// <summary>
	/// Visual Studio .NET 2005
	/// </summary>
	class VisualStudio80 : VisualStudioDTEImpl<EnvDTE80.DTE2>
	{
		public VisualStudio80()
			: base( "VisualStudio.DTE.8.0" )
		{
			//_visualStudio.SuppressUI = true;
		}

		public override void CloseSolution()
		{
			_visualStudio.Solution.Close( false );
		}

		public override void OpenSolution( string file )
		{
			_visualStudio.Solution.Open( file );
		}

		public override void Quit()
		{
			_visualStudio.Quit();
			_visualStudio = null;
		}

		public override Events Events
		{
			get { return _visualStudio.Events; }
		}

		public override SolutionBuild SolutionBuild
		{
			get { return _visualStudio.Solution.SolutionBuild; }
		}

		public override Windows Windows
		{
			get { return _visualStudio.Windows; }
		}
	}

	/// <summary>
	/// Visual Studio .NET 2008
	/// </summary>
	class VisualStudio90 : VisualStudioDTEImpl<EnvDTE80.DTE2>
	{
		public VisualStudio90()
			: base( "VisualStudio.DTE.9.0" )
		{
			//_visualStudio.SuppressUI = true;
		}

		public override void CloseSolution()
		{
			_visualStudio.Solution.Close( false );
		}

		public override void OpenSolution( string file )
		{
			_visualStudio.Solution.Open( file );
		}

		public override void Quit()
		{
			_visualStudio.Quit();
			_visualStudio = null;
		}

		public override Events Events
		{
			get { return _visualStudio.Events; }
		}

		public override SolutionBuild SolutionBuild
		{
			get { return _visualStudio.Solution.SolutionBuild; }
		}

		public override Windows Windows
		{
			get { return _visualStudio.Windows; }
		}
	}

}
