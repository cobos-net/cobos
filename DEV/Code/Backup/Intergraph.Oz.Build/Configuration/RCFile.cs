using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.SourceSafe.Interop;
using System.Text.RegularExpressions;
using Intergraph.Oz.Utilities.Extensions;
using Intergraph.Oz.Utilities.File;

namespace Intergraph.Oz.Build.Configuration
{
	/// <summary>
	/// Partial declaration of Generated class to add useful functionality
	/// </summary>
	public partial class RCFile : BuildEntity 
	{
		public RCFile()
		{
			// parameterless constructor for Xml serialization
		}

		public RCFile( string nameValue, string specValue )
		{
			nameField = nameValue;
			specField = specValue;
		}

		/// <summary>
		/// Regular expressions for file version
		/// File version is defined in two places, possible definitions are:
		/// FILEVERSION 2,0,0,4
		///            VALUE "FileVersion", "2, 0, 0, 4"
		/// </summary>
		//private const string _regExpFileVersion1 = @"\s+FILEVERSION\s+(\d+),(\d+),(\d+),(\d+)";
		//private const string _regExpFileVersion2 = @"\s+VALUE\s+""FileVersion"",\s+""(\d+),\s*(\d+),\s*(\d+),\s*(\d+)""";

		/// <summary>
		/// Regular expressions for file version
		/// File version is defined in two places, possible definitions are:
		/// PRODUCTVERSION 7,9,5,0
		///             VALUE "ProductVersion", "7, 9, 5, 0"
		/// </summary>
		//private const string _regExpProductVersion1 = @"\s+PRODUCTVERSION\s+(\d+),(\d+),(\d+),(\d+)";
		//private const string _regExpProductVersion2 = @"\s+VALUE\s+""ProductVersion"",\s+""(\d+),\s*(\d+),\s*(\d+),\s*(\d+)""";

		/// <summary>
		/// One size fits all regular expression pattern
		/// </summary>
		private const string _regExpVersion = @"(\s*)(\d+)(\s*[.|,]\s*)(\d+)(\s*[.|,]\s*)(\d+)(?:(\s*[.|,]\s*)(\d+))?";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rcItem"></param>
		/// <returns></returns>
		public static RCFile Analyse( IVSSItem rcItem )
		{
			StreamReader reader = null;

			try
			{
				bool foundFileVersion = false, foundProductVersion = false, foundLegalCopyright = false;

				string filename = rcItem.LocalSpec;
				string rcItemName = rcItem.Name;

				// make sure we have the latest
				string working = null;
				rcItem.Get( ref working, 0 );

				reader = new StreamReader( working );

				uint lineNum = 1;

				while ( !reader.EndOfStream )
				{
					string line = reader.ReadLine();
					string lineUpper = line.ToUpper();

					if ( lineUpper.Contains( "FILEVERSION" ) )
					{
						if ( Regex.IsMatch( line, _regExpVersion ) )
						{
							Application.Logger.Information( "Found version information in rc file {0} at line {1}: {2}", rcItemName, lineNum, line );
						}
						else
						{
							Application.Logger.Error( "Invalid file version format in rc file {0} at line {1}: {2}", rcItem.Name, lineNum, line );
						}

						foundFileVersion = true;
					}
					else if ( lineUpper.Contains( "PRODUCTVERSION" ) )
					{
						if ( Regex.IsMatch( line, _regExpVersion ) )
						{
							Application.Logger.Information( "Found version information in rc file {0} at line {1}: {2}", rcItemName, lineNum, line );
						}
						else
						{
							Application.Logger.Error( "Invalid product version format in rc file {0} at line {1}: {2}", rcItemName, lineNum, line );
						}

						foundProductVersion = true;
					}
					else if ( line.Contains( "LegalCopyright" ) )
					{
						Application.Logger.Information( "Found legal copyright information in rc file {0} at line {1}: {2}", rcItemName, lineNum, line );

						foundLegalCopyright = true;
					}

					++lineNum;
				}

				// post analysis
				if ( !foundFileVersion )
				{
					Application.Logger.Warning( "Could not find file version information in {0}", rcItemName );
				}

				if ( !foundProductVersion )
				{
					Application.Logger.Warning( "Could not find product version information in {0}", rcItemName );
				}

				if ( !foundLegalCopyright )
				{
					Application.Logger.Warning( "Could not find copyright information in {0}", rcItemName );
				}

				// add the item to the build manifest
				return new RCFile( rcItem.Name, rcItem.Spec );
			}
			catch ( System.Exception e )
			{
				Application.Logger.Log( e );
				return null;
			}
			finally
			{
				if ( reader != null )
				{
					reader.Close();
					reader = null;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rcItem"></param>
		/// <param name="fileVersion"></param>
		/// <param name="productVersion"></param>
		public void Update( IVSSDatabase vssDB, VersionNumber fileVersion, VersionNumber productVersion, bool checkInChanges )
		{
			if ( !this.includeInBuild )
			{
				Application.Logger.Information( "{0} not updated as directed by build manifest", this.spec );
				return;
			}

			IVSSItem rcItem = null;
			StreamReader reader = null;
			StreamWriter writer = null;

			// parse the file and update the version
			try
			{
				rcItem = vssDB.get_VSSItem( spec, false );

				// can only work on items that aren't checked out
				if ( checkInChanges && rcItem.IsCheckedOut != (int)VSSFileStatus.VSSFILE_NOTCHECKEDOUT )
				{
					IVSSCheckout vssCheckout = rcItem.Checkouts[ rcItem.Checkouts.Count ];
					Application.Logger.Error( "Can't update {0}.  The file is checked out to {1}", rcItem.Name, vssCheckout.Username );
					rcItem = null;
					return;
				}

				// cache configuration options locally
				bool updateFileVersion = fileVersion.includeInBuild;
				bool updateProductVersion = productVersion.includeInBuild;

				// check out the item
				if ( checkInChanges )
				{
					rcItem.Checkout( "Autobuild updating version for build", "", 0 );
				}
				else
				{
					string working = null;
					rcItem.Get( ref working, 0 );

					FileUtility.MakeReadWrite( working );
				}

				string filename = rcItem.LocalSpec;
				string tmpname = rcItem.LocalSpec + ".tmp";
				string rcItemName = rcItem.Name;

				reader = new StreamReader( filename );
				writer = new StreamWriter( tmpname );

				uint lineNum = 1;

				while ( !reader.EndOfStream )
				{
					string line = reader.ReadLine();
					string lineUpper = line.ToUpper();

					if ( updateFileVersion && lineUpper.Contains( "FILEVERSION" ) )
					{
						line = UpdateVersion( fileVersion, line, lineNum );
					}
					else if ( updateProductVersion && lineUpper.Contains( "PRODUCTVERSION" ) )
					{
						line = UpdateVersion( productVersion, line, lineNum );
					}
					else if ( line.Contains( "LegalCopyright" ) )
					{
						line = UpdateLegalCopyright( line, lineNum );
					}

					writer.WriteLine( line );

					++lineNum;
				}

				reader.Close();
				reader = null;
				writer.Close();
				writer = null;

				File.Delete( filename );
				File.Move( tmpname, filename );

				if ( checkInChanges )
				{
					// check the modified file back in
					rcItem.Checkin( "Autobuild updated version for build", "", 0 );
				}
			}
			catch ( System.Exception e )
			{
				Application.Logger.Log( e );

				if ( Vss.ItemHelper.IsCheckedOutByMe( rcItem ) )
				{
					rcItem.UndoCheckout( "", 0 );
				}
			}
			finally
			{
				rcItem = null;

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
		/// <param name="line"></param>
		/// <param name="regexp"></param>
		/// <param name="version"></param>
		/// <param name="rcItemName"></param>
		/// <param name="lineNum"></param>
		/// <returns></returns>
		private string UpdateVersion( VersionNumber version, string line, uint lineNum )
		{
			string[] tokens = Regex.Split( line, _regExpVersion );

			if ( tokens == null || tokens.Length == 1 )
			{
				Application.Logger.Error( "Invalid File Version format in rc file {0} at line {1}: {2}", this.name, lineNum, line );
			}
			else
			{
				Application.Logger.Information( "Found version information in rc file {0} at line {1}: {2}", this.name, lineNum, line );

				tokens[ 2 ] = version.major;
				tokens[ 4 ] = version.minor;
				tokens[ 6 ] = version.build;

				if ( tokens.Length == 10 )
				{
					tokens[ 8 ] = version.revision;
				}
				else //if ( tokens.Length == 8 )
				{
					tokens = tokens.Splice( 7, 0, tokens[ 5 ], (string)version.revision );
				}

				line = string.Join( "", tokens );

				Application.Logger.Information( "Updated version information in rc file {0} at line {1}: {2}", this.name, lineNum, line );
			}

			return line;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private string UpdateLegalCopyright( string line, uint lineNum )
		{
			Application.Logger.Information( "Found copyright information in rc file {0} at line {1}: {2}", this.name, lineNum, line );

			return line;
		}
	}
}
