using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.VisualStudio.SourceSafe.Interop;
using Intergraph.Oz.Build.Vs;
using Intergraph.Oz.Build.Configuration;
using Intergraph.Oz.Utilities.Extensions;
using Intergraph.Oz.Utilities.Logger;
using Intergraph.Oz.Utilities.Xml;

namespace Intergraph.Oz.Build
{
	public static class Application
	{
		/// <summary>
		/// 
		/// </summary>
		private static BuildManifest _manifest = null;

		/// <summary>
		/// 
		/// </summary>
		private static VSSDatabaseClass _sourceSafe = null;

		/// <summary>
		/// 
		/// </summary>
		private static VisualStudio _visualStudio = null;

		public static VisualStudio VisualStudio
		{
			get
			{
				return _visualStudio;
			}
		}

		private enum Mode
		{
			AnalysisMode,
			BuildMode
		}

		public static LogFileWriter Logger
		{
			get { return _logFile; }
		}

		static LogFileWriter _logFile;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="manifest"></param>
		private static void Initialise( string manifest, Mode mode )
		{
			Cleanup();

			// load the skeleton manifest, which we will populate
			XmlHelper<BuildManifest>.Deserialize( out _manifest, manifest );
			
			_logFile = new LogFileWriter( _manifest.project.configPath + @"\buildlog.xml", "autobuild" );

			Logger.AddMetadata( "Filename", _manifest.project.configPath + @"\buildlog.xml" );
			Logger.AddMetadata( "Description", "AutoBuild log file" );
			Logger.AddMetadata( "Created", DateTime.Now.ToString( "s" ) );

			if ( mode == Mode.AnalysisMode )
			{
				Logger.AddMetadata( "Analysis on", System.Environment.MachineName );
				Logger.AddMetadata( "Analysis by", WindowsIdentity.GetCurrent().Name.ToString() );
			}
			else
			{
				Logger.AddMetadata( "Built on", System.Environment.MachineName );
				Logger.AddMetadata( "Built by", WindowsIdentity.GetCurrent().Name.ToString() );

				if ( _manifest.fileVersion != null )
				{
					Logger.AddMetadata( "File Version", _manifest.fileVersion.Increment() );
				}

				if ( _manifest.productVersion != null )
				{
					Logger.AddMetadata( "Product Version", _manifest.productVersion.Increment() );
				}
			}

			OpenSourceSafe();

			OpenVisualStudio();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static void Cleanup()
		{
			if ( _manifest != null )
			{
				string manifestPath = _manifest.project.configPath + @"\manifest.xml";

				Logger.Information( "Saving configuration to {0}", manifestPath );

				try
				{
					XmlHelper<BuildManifest>.Serialize( _manifest, manifestPath );
				}
				catch ( Exception e )
				{
					Logger.Log( e );
				}
				_manifest = null;
			}

			string logPath = Logger.LogPath;

			Logger.Dispose();

			if ( logPath != null )
			{
				string folder = System.IO.Path.GetDirectoryName( logPath );

				XsltHelper.Transform( "IPS.AutoBuild.LogFile.LogFile.xslt", logPath, folder + @"\buildlog.htm" );
			}

			CloseSourceSafe();

			CloseVisualStudio();
		}

		/// <summary>
		/// 
		/// </summary>
		public static void DoAnalysis( string analysisManifest )
		{
			try
			{
				Initialise( analysisManifest, Mode.AnalysisMode );
				
				AnalyseVersion();

				AnalyseSolutions();
			}
			catch ( System.Exception e )
			{
				Logger.Log( e );
			}
			finally
			{
				Cleanup();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void DoBuild( string buildManifest )
		{
			try
			{
				Initialise( buildManifest, Mode.BuildMode );

				PreBuildCopy();

				GetLatest();

				UpdateVersions();

				BuildSolutions();

				PostBuildCopy();

				CreateInstallation();

				RunInstallation();
			}
			catch ( System.Exception e )
			{
				Logger.Log( e );
			}
			finally
			{
				Cleanup();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static void OpenSourceSafe()
		{
			CloseSourceSafe();

			Logger.Information( "Opening Source Safe...." );

			VssConfiguration vss = _manifest.project.vssConfiguration;

			_sourceSafe = new VSSDatabaseClass();

			_sourceSafe.Open( vss.database, vss.userName, vss.password );

			IVSSItem vssProject = _sourceSafe.get_VSSItem( "$/", false );
			vssProject.LocalSpec = _manifest.project.workingFolder;
		}

		/// <summary>
		/// 
		/// </summary>
		private static void CloseSourceSafe()
		{
			if ( _sourceSafe != null )
			{
				_sourceSafe.Close();
				_sourceSafe = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private static void OpenVisualStudio()
		{
			CloseVisualStudio();

			if ( !_manifest.solutionFiles.performBuildStep )
			{
				Logger.Information( "Not required to start Visual Studio instance" );
				return; // this allows us to run on machines without VS installed.
			}

			// takes a while...
			Logger.Information( "Starting Visual Studio...." );

			_visualStudio = VisualStudio.Create( _manifest.solutionFiles.environment );
		}

		/// <summary>
		/// 
		/// </summary>
		private static void CloseVisualStudio()
		{
			if ( _visualStudio != null )
			{
				_visualStudio.Dispose();
				_visualStudio = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private static void AnalyseVersion()
		{
			if ( _manifest.fileVersion == null )
			{
				_manifest.fileVersion = new VersionNumber( 1, 0, 1, 0 );
			}

			if ( _manifest.productVersion == null )
			{
				_manifest.productVersion = new VersionNumber( 1, 0, 1, 0 );
			}

			if ( _manifest.rcFiles != null && !_manifest.rcFiles.performBuildStep )
			{
				Logger.Information( "Not analysing version numbers as directed by build manifest" );
				return;
			}

			// overwrite any existing manifest, would be nice to have a merge function one day
			_manifest.rcFiles = new RCFiles();

			foreach ( VssFolder folder in _manifest.getLatest.folder )
			{
				IVSSItem vssFolder = _sourceSafe.get_VSSItem( folder.Value, false );
				
				Vss.FileHelper.FindAllFiles( vssFolder, new Vss.RCFileSelector() );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rcFile"></param>
		public static void AddRCFile( RCFile rcFile )
		{
			if ( _manifest.rcFiles.rcFile == null )
			{
				_manifest.rcFiles.rcFile = new RCFile[] { rcFile };
			}
			else
			{
				_manifest.rcFiles.rcFile = _manifest.rcFiles.rcFile.Concat( rcFile );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="folders"></param>
		private static void AnalyseSolutions()
		{
			if ( _manifest.solutionFiles != null && !_manifest.solutionFiles.performBuildStep )
			{
				Logger.Information( "Not analysing Visual Studio solutions as directed by build manifest" );
				return;
			}

			// overwrite any existing manifest, would be nice to have a merge function one day
			_manifest.solutionFiles = new SolutionFiles();

			foreach ( VssFolder folder in _manifest.getLatest.folder )
			{
				IVSSItem vssFolder = _sourceSafe.get_VSSItem( folder.Value, false );

				Vss.FileHelper.FindAllFiles( vssFolder, new Vss.SolutionFileSelector() );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="slnFile"></param>
		public static void AddSolutionFile( SolutionFile slnFile )
		{
			if ( _manifest.solutionFiles.solutionFile == null )
			{
				_manifest.solutionFiles.solutionFile = new SolutionFile[] { slnFile };
			}
			else
			{
				_manifest.solutionFiles.solutionFile = _manifest.solutionFiles.solutionFile.Concat( slnFile );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private static void GetLatest()
		{
			if ( !_manifest.getLatest.performBuildStep )
			{
				Logger.Information( "Not getting latest source as directed by build manifest" );
				return;
			}

			foreach ( VssFolder folder in _manifest.getLatest.folder )
			{
				IVSSItem vssFolder = _sourceSafe.get_VSSItem( folder.Value, false );

				if ( vssFolder == null )
				{
					Logger.Error( "Could not get latest for {0}", vssFolder.Spec );
					continue;
				}

				try
				{
					Logger.Information( "Getting latest for {0}", vssFolder.Spec );

					if ( vssFolder.Type == (int)VSSItemType.VSSITEM_FILE )
					{
						string working = null;
						vssFolder.Get( ref working, 0 );
					}
					else
					{
						string working = null;
						vssFolder.Get( ref working, (int)VSSFlags.VSSFLAG_RECURSYES );
					}
				}
				catch ( System.Exception e )
				{
					Logger.Log( e );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private static void UpdateVersions()
		{
			if ( !_manifest.rcFiles.performBuildStep )
			{
				Logger.Information( "Not updating version numbers as directed by build manifest" );
				return;
			}

			foreach ( RCFile rcFile in _manifest.rcFiles.rcFile )
			{
				try
				{
					rcFile.Update( _sourceSafe, _manifest.fileVersion, _manifest.productVersion, _manifest.rcFiles.checkInUpdates );
				}
				catch ( System.Exception e )
				{
					Logger.Log( e );
				}
			}
		}

		/// <summary>
		///  
		/// </summary>
		private static void BuildSolutions()
		{
			if ( !_manifest.solutionFiles.performBuildStep )
			{
				Logger.Information( "Not building VS solutions as directed by build manifest" );
				return;
			}

			if ( _manifest.solutionFiles.solutionFile.Length == 0 )
			{
				Logger.Information( "No VS solutions specified in build manifest" );
				return;
			}

			// make sure that the manifest solution files are sorted in build order...
			System.Array.Sort( _manifest.solutionFiles.solutionFile, new SolutionFile.Comparer() );

			long buildPass = _manifest.solutionFiles.solutionFile[ 0 ].buildOnPass;
			Logger.Information( "Building VS solutions, currently pass {0}", buildPass );

			foreach ( SolutionFile slnFile in _manifest.solutionFiles.solutionFile )
			{
				if ( slnFile.buildOnPass != buildPass )
				{
					buildPass = slnFile.buildOnPass;
					Logger.Information( "Building VS solutions, currently pass {0}", buildPass );
				}

				try
				{
					slnFile.Build( _sourceSafe, _manifest.solutionFiles.buildDebug, _manifest.solutionFiles.buildRelease );
				}
				catch ( System.Exception e )
				{
					Logger.Log( e );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private static void PostBuildCopy()
		{
			if ( _manifest.postBuildCopy != null )
			{
				DoBuildCopy( _manifest.postBuildCopy );
			}
			else
			{
				Logger.Information( "No post-build copy step specified in manifest" );
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		private static void PreBuildCopy()
		{
			if ( _manifest.preBuildCopy != null )
			{
				DoBuildCopy( _manifest.preBuildCopy );
			}
			else
			{
				Logger.Information( "No pre-build copy step specified in manifest" );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buildCopy"></param>
		private static void DoBuildCopy( BuildCopy buildCopy )
		{


		}

		/// <summary>
		/// 
		/// </summary>
		private static void CreateInstallation()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		private static void RunInstallation()
		{
		}

	}
}
