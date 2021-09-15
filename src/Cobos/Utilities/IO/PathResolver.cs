// ----------------------------------------------------------------------------
// <copyright file="PathResolver.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.IO
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    /// <summary>
    /// Used to turn a relative path into a fully qualified path.
    /// Uses a single path as a root folder and takes a list
    /// of other relative folders (with respect to the root)
    /// and attempts to resolve a relative file path into a
    /// fully qualified path.
    /// </summary>
    public class PathResolver
    {
        /// <summary>
        /// Maintain a list of folders to search in when resolving paths.
        /// </summary>
        private readonly List<NormalisedPath> lookInFolders;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathResolver"/> class.
        /// </summary>
        /// <param name="rootFolderPath">The root folder for all relative folder paths.</param>
        /// <param name="lookInFolders">A list of folders that may be absolute paths or relative to the root.</param>
        /// <remarks>
        /// Construct the object and resolve all relative folders before processing.
        /// </remarks>
        public PathResolver(string rootFolderPath, string[] lookInFolders)
        {
            NormalisedPath rootFolder = ResolveFolderPath(rootFolderPath);

            if (!FileUtility.ExistsAsFolder(rootFolder.Value))
            {
                throw new Exception(string.Format("Cannot use a non-existent folder as the root: {0}", rootFolder.Value));
            }

            this.lookInFolders = new List<NormalisedPath>(lookInFolders == null ? 1 : lookInFolders.Length + 1)
            {
                rootFolder,
            };

            if (lookInFolders != null)
            {
                foreach (string folder in lookInFolders)
                {
                    this.AddLookInFolder(folder);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathResolver"/> class by copying another instance.
        /// </summary>
        /// <param name="other">The other instance to copy.</param>
        public PathResolver(PathResolver other)
        {
            this.lookInFolders = new List<NormalisedPath>(other.lookInFolders.Count);
            this.lookInFolders.AddRange(other.LookInFolders);
        }

        /// <summary>
        /// Gets a readonly collection of look in folders.
        /// </summary>
        public ReadOnlyCollection<NormalisedPath> LookInFolders
        {
            get
            {
                return this.lookInFolders.AsReadOnly();
            }
        }

        /// <summary>
        /// Utility method to return a valid normalized folder path.
        /// </summary>
        /// <param name="path">Path to a folder (or file) to resolve.  This must be an absolute path.</param>
        /// <returns>A normalized path.  Check the result to determine whether the folder actually exists.</returns>
        public static NormalisedPath ResolveFolderPath(string path)
        {
            // do some basic checking to get the root folder
            if (!FileUtility.FileOrFolderExists(path))
            {
                return new NormalisedPath(path); // doesn't exist, so we can't do any checking
            }

            if (FileUtility.IsFile(path))
            {
                // change path to be the containing folder
                path = Path.GetDirectoryName(path);
            }

            return new NormalisedPath(path);
        }

        /// <summary>
        /// Utility method to resolve a relative folder path.  The method will only succeed if
        /// the path actually exists.  If folder specifies an absolute path then it is not modified.
        /// </summary>
        /// <param name="path">The relative folder path.</param>
        /// <param name="relativeTo">The path that the folder is relative to.</param>
        /// <returns>The full path of the folder; otherwise null if the folder doesn't exist.</returns>
        public static NormalisedPath ResolveFolderPath(string path, string relativeTo)
        {
            // first off, does the path actually need resolving?
            if (FileUtility.IsAbsolutePath(path))
            {
                return ResolveFolderPath(path);
            }

            if (FileUtility.ExistsAsFile(relativeTo))
            {
                // change path to be the containing folder
                relativeTo = Path.GetDirectoryName(relativeTo);
            }

            return new NormalisedPath(path, relativeTo);
        }

        /// <summary>
        /// Utility method to resolve a relative file path.  The method will only succeed if
        /// the path actually exists.  If path specifies an absolute path then it is not modified.
        /// </summary>
        /// <param name="path">The relative file path.</param>
        /// <param name="relativeTo">The path that the file is relative to.</param>
        /// <returns>The full path of the file; otherwise null if the file doesn't exist.</returns>
        public static NormalisedPath ResolveFilePath(string path, string relativeTo)
        {
            // first off, does the path actually need resolving?
            if (FileUtility.IsAbsolutePath(path))
            {
                return new NormalisedPath(path);
            }

            if (FileUtility.ExistsAsFile(relativeTo))
            {
                // change path to be the containing folder
                relativeTo = Path.GetDirectoryName(relativeTo);
            }

            return new NormalisedPath(path, relativeTo);
        }

        /// <summary>
        /// Add a new look in folder to the internal list.
        /// </summary>
        /// <param name="folder">The path of the folder to add.</param>
        public void AddLookInFolder(string folder)
        {
            // NOTE: you cannot create a PathResolver without at least 1 lookin folder,
            // so this code is safe.
            NormalisedPath lookin = ResolveFolderPath(folder, this.lookInFolders[0].Value);

            if (lookin != null && FileUtility.ExistsAsFolder(lookin.Value))
            {
                this.lookInFolders.Add(lookin);
            }
        }

        /// <summary>
        /// Add a group of look in folders to the internal list.
        /// </summary>
        /// <param name="folders">The paths of the folders to add.</param>
        public void AddLookInFolders(string[] folders)
        {
            foreach (string folder in folders)
            {
                this.AddLookInFolder(folder);
            }
        }

        /// <summary>
        /// Add all of the look in folders from another <see cref="PathResolver"/> instance.
        /// </summary>
        /// <param name="other">The other instance to copy from.</param>
        public void AddLookInFoldersFrom(PathResolver other)
        {
            if (other != null)
            {
                this.lookInFolders.AddRange(other.LookInFolders);
            }
        }

        /// <summary>
        /// Resolve the relative path using the look in folders.
        /// </summary>
        /// <param name="relativePath">Can be an absolute path, in which case no resolving takes place.</param>
        /// <returns>Null if the file cannot be found.</returns>
        public NormalisedPath FindFilePath(string relativePath)
        {
            // first off, does the path actually need resolving?
            if (FileUtility.IsAbsolutePath(relativePath))
            {
                if (!FileUtility.ExistsAsFile(relativePath))
                {
                    return null;
                }

                return new NormalisedPath(relativePath);
            }

            // so it's a relative path, try resolving with the look in folders
            foreach (NormalisedPath lookIn in this.lookInFolders)
            {
                NormalisedPath fullPath = ResolveFilePath(relativePath, lookIn.Value);

                if (FileUtility.ExistsAsFile(fullPath.Value))
                {
                    return fullPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Resolve the relative path using the look in folders.  Is forgiving if the path is
        /// a file, it will return the containing folder.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The full path of the relative path.</returns>
        public NormalisedPath FindFolderPath(string relativePath)
        {
            // first off, does the path actually need resolving?
            if (FileUtility.IsAbsolutePath(relativePath))
            {
                if (!FileUtility.FileOrFolderExists(relativePath))
                {
                    return null;
                }

                if (FileUtility.IsFile(relativePath))
                {
                    relativePath = Path.GetDirectoryName(relativePath);
                }

                return new NormalisedPath(relativePath);
            }

            // so it's a relative path, try resolving with the look in folders
            foreach (NormalisedPath lookIn in this.lookInFolders)
            {
                NormalisedPath fullPath = ResolveFilePath(relativePath, lookIn.Value);

                if (FileUtility.FileOrFolderExists(fullPath.Value))
                {
                    if (FileUtility.IsFile(fullPath.Value))
                    {
                        return fullPath.GetDirectoryName();
                    }
                    else
                    {
                        return fullPath;
                    }
                }
            }

            return null;
        }
    }
}
