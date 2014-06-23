// ----------------------------------------------------------------------------
// <copyright file="AssemblyReferenceResolver.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.AssemblyGen
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using Cobos.Utilities.Cache;

    /// <summary>
    /// Helper class for resolving assembly references.
    /// </summary>
    public static class AssemblyReferenceResolver
    {
        /// <summary>
        /// Resolve paths to assemblies not found in C:\WINDOWS\Microsoft.NET\Framework\vXXX.
        /// The compiler doesn't use the CLR to load assemblies, if they are not in the same
        /// folder as the executable or the CSC then you need to supply /lib: parameters to
        /// the compiler for it to be able to resolve the assembly references.
        /// Since the CLR is not used, assemblies are not loaded from the GAC.
        /// </summary>
        /// <param name="assemblies">The assembly references to resolve.</param>
        /// <param name="searchPaths">The additional assembly paths to search.</param>
        /// <returns>A list of fully resolved assembly paths.</returns>
        /// <remarks>
        /// Strategy:
        /// ---------
        /// 1. Use a cache to store paths and retrieve paths we've previously found.  Recursively searching using Directory.Find is quite slow.
        /// 2. Add any user configured assembly paths.
        /// <para>
        /// For each reference:
        /// </para>
        /// 3. Look in the cache for previously identified framework assemblies.  This will be found by the compiler.
        /// 4. Look in the current working directory.  If found don't add it, it will be picked up.
        /// 5. Look in all previously found folders.  If found don't add it, the path is already referenced.
        /// 5. Look in the cache for previously found folders. If found, add it.  Add the folder to the cache.
        /// 6. Look in the framework folders.  If found don't add it, it will be picked up.
        /// 7. Search the GAC.  If found, add it, the compiler doesn't use the CLR, so it won't be loaded.  Add the folder to cache.
        /// 8. Search the %PATH%.  If found, add it.  Add the folder to the cache.
        /// </remarks>
        public static List<string> Resolve(string[] assemblies, string[] searchPaths)
        {
            CacheFile<string> cache = GetCache();

            List<string> paths = new List<string>();

            // Add any user specified assembly paths
            searchPaths = ValidatePaths(searchPaths);

            if (searchPaths.Length > 0)
            {
                paths.AddRange(searchPaths);
            }

            // store all paths in the %PATH% as a last resort
            string[] environmentPaths = Environment.GetEnvironmentVariable("PATH").Split(';');

            // attempt to resolve all references
            foreach (string assembly in assemblies)
            {
                string[] folders;
                string path;

                if (cache.Contains("FrameworkAssemblies", assembly))
                {
                    continue;  // confirmed framework assembly
                }

                if (IsInCurrentWorkingFolder(assembly))
                {
                    continue; // will be found in working directory
                }

                if (IsInFolderList(assembly, paths, out path))
                {
                    continue; // already found the folder containing this reference.
                }

                if (IsInFolderCache(assembly, cache, out path))
                {
                    paths.Add(path); // add the path from the cache.
                    continue;
                }

                if (IsInFrameworkFolders(assembly))
                {
                    cache.Add("FrameworkAssemblies", assembly);
                    continue;  // will be found by the compiler.
                }

                if (IsInFolder(@"C:\WINDOWS\assembly", assembly, SearchOption.AllDirectories, out folders))
                {
                    paths.AddRange(folders);
                    cache.Add("AssemblyReferenceFolders", folders);
                    continue;
                }

                if (IsInFolderList(assembly, environmentPaths, out path))
                {
                    paths.Add(path);
                    cache.Add("AssemblyReferenceFolders", path);
                    continue;
                }
            }

            cache.Save();

            return paths;
        }

        /// <summary>
        /// Check whether the resource is in the current working folder.
        /// </summary>
        /// <param name="reference">The resource to find.</param>
        /// <returns>true if the resource was found; otherwise false.</returns>
        public static bool IsInCurrentWorkingFolder(string reference)
        {
            string[] found = Directory.GetFiles(Environment.CurrentDirectory, reference, SearchOption.TopDirectoryOnly);

            return found != null && found.Length > 0;
        }

        /// <summary>
        /// Check whether the resource is in a list of paths we've already found.
        /// </summary>
        /// <param name="reference">The resource to find.</param>
        /// <param name="paths">The paths we've already found.</param>
        /// <param name="foundPath">Receives the path of the resource if found.</param>
        /// <returns>true if the resource was found; otherwise false.</returns>
        public static bool IsInFolderList(string reference, IEnumerable<string> paths, out string foundPath)
        {
            foundPath = null;

            foreach (string path in paths)
            {
                string[] found = Directory.GetFiles(path, reference, SearchOption.TopDirectoryOnly);

                if (found != null && found.Length > 0)
                {
                    foundPath = Path.GetDirectoryName(found[0]);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether the resource is in a cached folder.
        /// </summary>
        /// <param name="reference">The reference to find.</param>
        /// <param name="cache">The file cache.</param>
        /// <param name="path">Receives the path if found.</param>
        /// <returns>true if the resource was found in the cache; otherwise false.</returns>
        public static bool IsInFolderCache(string reference, CacheFile<string> cache, out string path)
        {
            path = null;

            string[] cachedFolders = cache["AssemblyReferenceFolders"];

            foreach (string cachedFolder in cachedFolders)
            {
                string[] found;

                if (IsInFolder(cachedFolder, reference, SearchOption.TopDirectoryOnly, out found))
                {
                    path = cachedFolder;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether a particular resource exists in the framework folders.
        /// </summary>
        /// <param name="reference">The reference to find.</param>
        /// <returns>true if the resource is found in the framework folders; otherwise false.</returns>
        public static bool IsInFrameworkFolders(string reference)
        {
            // check whether the assembly is in the framework folders
            string[] folders = Directory.GetDirectories(@"C:\WINDOWS\Microsoft.NET\Framework", "V*", SearchOption.TopDirectoryOnly);

            foreach (string folder in folders)
            {
                string[] found = Directory.GetFiles(folder, reference, SearchOption.TopDirectoryOnly);
                
                if (found != null && found.Length > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether a particular resource exists in a specific folder.
        /// </summary>
        /// <param name="searchPath">The path to search in.</param>
        /// <param name="searchPattern">The pattern to search for.</param>
        /// <param name="searchOption">The search options.</param>
        /// <param name="found">Receives a list of the found items.</param>
        /// <returns>true if the search was successful; otherwise false.</returns>
        public static bool IsInFolder(string searchPath, string searchPattern, SearchOption searchOption, out string[] found)
        {
            found = Directory.GetFiles(searchPath, searchPattern, searchOption);

            if (found == null || found.Length == 0)
            {
                return false;
            }

            found = ValidatePaths(found);

            return found != null && found.Length != 0;
        }

        /// <summary>
        /// Validates the collection of paths to confirm that they exist.
        /// </summary>
        /// <param name="paths">The collection of paths to validate.</param>
        /// <returns>A collection of validated paths.</returns>
        public static string[] ValidatePaths(string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                return new string[0];
            }

            List<string> valid = new List<string>(paths.Length);

            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (File.Exists(path))
                {
                    valid.Add(Path.GetDirectoryName(path));
                }
                else if (Directory.Exists(path))
                {
                    valid.Add(path);
                }
            }

            return valid.ToArray();
        }

        /// <summary>
        /// Open or create the application cache file.
        /// </summary>
        /// <returns>The cache file.</returns>
        private static CacheFile<string> GetCache()
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + ".cache";

            CacheFile<string> cache = new CacheFile<string>(path);

            cache.Open();

            return cache;
        }
    }
}
