using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Intergraph.Oz.Utilities;
using Intergraph.Oz.Utilities.Extensions;
using Intergraph.Oz.Utilities.Xml;

namespace Intergraph.Oz.Build.Configuration
{
	public class FileCopyGroupImpl : FileCopyGroup
	{
		/// <summary>
		/// 
		/// </summary>
		private SortedDictionary<string, FileCopyTargetFolder> _folderLookup = new SortedDictionary<string, FileCopyTargetFolder>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		public FileCopyGroupImpl( string root )
		{
			targetFolder = root;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		public FileCopyTargetFolder GetTargetFolder( string folderPath )
		{
			try
			{
				//Profiler.Start( "FileCopyGroup.GetTargetFolder" );

				if ( folderPath[ 0 ] == '/' || folderPath[ 0 ] == '\\' )
				{
					folderPath = folderPath.Substring( 1 );
				}

				FileCopyTargetFolder found;

				if ( _folderLookup.TryGetValue( folderPath, out found ) )
				{
					return found;
				}

				// doesn't exist, create and insert into the hierarchy
				string[] parts = folderPath.Split( '\\', '/' );

				FileCopyTargetFolder targetFolder = new FileCopyTargetFolder();
				targetFolder.name = parts[ parts.Length - 1 ];

				_folderLookup.Add( folderPath, targetFolder );

				if ( parts.Length == 1 )
				{
					this.folder = this.folder.Concat( targetFolder );
				}
				else
				{
					string parentPath = string.Join( @"\", parts, 0, parts.Length - 1 );

					FileCopyTargetFolder parent = GetTargetFolder( parentPath );

					if ( parent == null )
					{
						throw new IntergraphError( "FileCopyTargetFolder.GetTargetFolder: Couldn't get the target folder {0}", parentPath );
					}

					parent.Items = parent.Items.Concat( targetFolder );
				}

				return targetFolder;
			}
			catch ( System.Exception e )
			{
				throw e;
			}
			finally
			{
				//Profiler.Stop( "FileCopyGroup.GetTargetFolder" );
			}
		}

		public static void Analyse()
		{
			string root = @"D:\cd6\ESTA\7.9\Delivery";
			string pattern = "*.*";
			bool recursive = true;

			FileCopyGroupImpl copyGroup = new FileCopyGroupImpl( root );

			string[] targets = Directory.GetFiles( root, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );

			string[] sources = { @"D:\cd6\Shared", @"D:\cd6\ESTA\7.9\Development" };

			int numTargets = targets.Length;
			int numTargetsSoFar = 0;

			foreach ( string t in targets )
			{
				System.Console.WriteLine( "Processing {0} of {1}", ++numTargetsSoFar, numTargets );

				// get the relative path
				string tpath = Path.GetDirectoryName( t );
				tpath = tpath.Substring( root.Length );

				string tname = Path.GetFileName( t );

				foreach ( string s in sources )
				{
					System.Console.WriteLine( "Looking for {0} in {1}", tname, s );

					string[] found = Directory.GetFiles( s, tname, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );

					System.Console.WriteLine( "Found {0} possibilities:", found.Length );

					foreach ( string fs in found )
					{
						System.Console.WriteLine( "Processing {0}", fs );

						FileCopyTargetFolder targetFolder = copyGroup.GetTargetFolder( tpath );

						// get the relative path
						string spath = Path.GetDirectoryName( fs );
						spath = spath.Substring( s.Length );

						FileCopySourceFolder sourceFolder = new FileCopySourceFolder();
						sourceFolder.Value = spath;
						sourceFolder.wildcard = Path.GetFileName( fs );
						sourceFolder.buildTree = false;
						sourceFolder.searchOption = FileSearchOption.TopDirectoryOnly;

						targetFolder.Items = targetFolder.Items.Concat( sourceFolder );
					}
				}
			}

			//System.IO.TextWriter writer = new System.IO.StreamWriter( @"d:\temp\filecopy.xml" );
			//writer.WriteLine( 

			XmlHelper<FileCopyGroup>.Serialize( copyGroup, @"d:\temp\filecopy.xml" );
		}


	}
}
