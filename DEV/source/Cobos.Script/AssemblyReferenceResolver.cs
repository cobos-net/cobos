using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;

namespace Cobos.Script
{
	internal static class AssemblyReferenceResolver
	{
		/// <summary>
		/// Resolve paths to assemblies not found in C:\WINDOWS\Microsoft.NET\Framework\vXXX.
		/// The compiler doesn't use the CLR to load assemblies, if they are not in the same
		/// folder as the executable or the csc then you need to supply /lib: parameters to
		/// the compiler for it to be able to resolve the assembly references.
		/// Since the CLR is not used, assemblies are not loaded from the GAC.
		/// </summary>
		/// <param name="references"></param>
		/// <returns></returns>
		public static List<string> Resolve( string[] references )
		{
			// Strategy:
			// ---------
			// 1. Use a cache to store paths and retrieve paths we've previously found.  Recursively searching using Directory.Find is quite slow.
			// 2. Add any user configured assembly paths.
			//
			// For each reference:
			//
			// 3. Look in the cache for previoulsy identified framework assemblies.  This will be found by the compiler.
			// 4. Look in the current working directory.  If found don't add it, it will be picked up.
			// 5. Look in all previously found folders.  If found don't add it, the path is already referenced.
			// 5. Look in the cache for previously found folders. If found, add it.  Add the folder to the cache.
			// 6. Look in the framework folders.  If found don't add it, it will be picked up.
			// 7. Search the GAC.  If found, add it, the compiler doesn't use the CLR, so it won't be loaded.  Add the folder to cache.
			// 8. Search the %PATH%.  If found, add it.  Add the folder to the cache.

			CacheFile<string> cache = GetCache();

			List<string> paths = new List<string>();

			// Add any user specified assembly paths
			string[] configPaths = ValidateFoldersList( ConfigurationManager.AppSettings[ "AssemblyPaths" ].Split( ';' ) );

			if ( configPaths != null && configPaths.Length > 0 )
			{
				paths.AddRange( configPaths );
			}

			// store all paths in the %PATH% as a last resort
			string[] environmentPaths = Environment.GetEnvironmentVariable( "PATH" ).Split( ';' );

			// attempt to resolve all references
			foreach ( string reference in references )
			{
				string[] folders;
				string path;

				if ( cache.Contains( "FrameworkAssemblies", reference ) )
				{
					continue;  // confirmed framework assembly
				}

				if ( IsInCurrentWorkingFolder( reference ) )
				{
					continue; // will be found in working directory
				}

				if ( IsInFolderList( reference, paths, out path ) )
				{
					continue; // already found the folder containing this reference.
				}

				if ( IsInFolderCache( reference, cache, out path ) )
				{
					paths.Add( path ); // add the path from the cache.
					continue;
				}

				if ( IsInFrameworkFolders( reference ) )
				{
					cache.Add( "FrameworkAssemblies", reference );
					continue;  // will be found by the compiler.
				}

				if ( IsInFolder( @"C:\WINDOWS\assembly", reference, SearchOption.AllDirectories, out folders ) )
				{
					paths.AddRange( folders );
					cache.Add( "AssemblyReferenceFolders", folders );
					continue;
				}

				if ( IsInFolderList( reference, environmentPaths, out path ) )
				{
					paths.Add( path );
					cache.Add( "AssemblyReferenceFolders", path );
					continue;
				}
			}

			cache.Save();

			return paths;
		}

		public static bool IsInCurrentWorkingFolder( string reference )
		{
			// check whether the assembly is in the current working folder
			string[] found = Directory.GetFiles( Environment.CurrentDirectory, reference, SearchOption.TopDirectoryOnly );

			return found != null && found.Length > 0;
		}

		/// <summary>
		/// Check whether the file is in a list of paths we've already found.
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static bool IsInFolderList( string reference, IEnumerable<string> paths, out string foundPath )
		{
			foundPath = null;

			foreach ( string path in paths )
			{
				string[] found = Directory.GetFiles( path, reference, SearchOption.TopDirectoryOnly );

				if ( found != null && found.Length > 0 )
				{
					foundPath = Path.GetDirectoryName( found[ 0 ] );
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether the file is in a cached folder.
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="cache"></param>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static bool IsInFolderCache( string reference, CacheFile<string> cache, out string path )
		{
			path = null;

			string[] cachedFolders = cache[ "AssemblyReferenceFolders" ];

			foreach ( string cachedFolder in cachedFolders )
			{
				string[] found;

				if ( IsInFolder( cachedFolder, reference, SearchOption.TopDirectoryOnly, out found ) )
				{
					path = cachedFolder;
					return true;
				}
			}

			return false;
		}

		public static bool IsInFrameworkFolders( string reference )
		{
			// check whether the assembly is in the framework folders
            string[] folders = Directory.GetDirectories( @"C:\WINDOWS\Microsoft.NET\Framework", "V*", SearchOption.TopDirectoryOnly );

            foreach ( string folder in folders )
            {
                string[] found = Directory.GetFiles( folder, reference, SearchOption.TopDirectoryOnly );

                if ( found != null && found.Length > 0 )
                {
                    return true;
                }
            }

            return false;
		}

		/// <summary>
		/// Open or create the application cache file.
		/// </summary>
		/// <returns></returns>
		private static CacheFile<string> GetCache()
		{
			string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + ".cache";

			CacheFile<string> cache = new CacheFile<string>( path );

			cache.Open();

			return cache;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static bool IsInFolder( string searchPath, string searchPattern, SearchOption searchOption, out string[] found )
		{
			found = Directory.GetFiles( searchPath, searchPattern, searchOption );

			if ( found == null || found.Length == 0 )
			{
				return false;
			}

			found = ValidateFoldersList( found );

			return found != null && found.Length != 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static string[] ValidateFoldersList( string[] paths )
		{
			if ( paths == null || paths.Length == 0 )
			{
				return null;
			}

			List<string> valid = new List<string>( paths.Length );

			foreach ( string path in paths )
			{
				if ( string.IsNullOrEmpty( path ) )
				{
					continue;
				}

				if ( File.Exists( path ) )
				{
					valid.Add( Path.GetDirectoryName( path ) );
				}
				else if ( Directory.Exists( path ) )
				{
					valid.Add( path );
				}
			}

			return valid.ToArray();
		}
	}
}
