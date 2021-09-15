// ----------------------------------------------------------------------------
// <copyright file="FileUtility.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.IO
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Cobos.Utilities.Text;

    /// <summary>
    /// Provides various file utility methods.
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Match a drive folder.
        /// </summary>
        public static readonly Regex RegexDrivePath = new Regex("^[a-zA-Z]{1}:\\.*");

        /// <summary>
        /// Match a UNC folder.
        /// </summary>
        public static readonly Regex RegexUncPath = new Regex("^\\\\.*\\.*");

        /// <summary>
        /// Test whether a file or folder exists.
        /// </summary>
        /// <param name="path">The path to the file or folder.</param>
        /// <returns>true if the file or folder exists; otherwise false.</returns>
        public static bool FileOrFolderExists(string path)
        {
            try
            {
                return System.IO.File.GetAttributes(path) != 0;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            catch (IOException)
            {
                return true; // in use by another process
            }
        }

        /// <summary>
        /// Test whether the path exists and is a folder.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>true if the folder exists; otherwise false.</returns>
        public static bool ExistsAsFolder(string path)
        {
            if (!FileOrFolderExists(path))
            {
                return false;
            }

            return IsFolder(path);
        }

        /// <summary>
        /// Test whether the path exists and is a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>true if the file exists; otherwise false.</returns>
        public static bool ExistsAsFile(string path)
        {
            if (!FileOrFolderExists(path))
            {
                return false;
            }

            return IsFile(path);
        }

        /// <summary>
        /// Is the path a file and not a folder?.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>true if the path is a file; otherwise false.</returns>
        public static bool IsFile(string path)
        {
            FileAttributes attr = System.IO.File.GetAttributes(path);

            return (attr & FileAttributes.Directory) != FileAttributes.Directory;
        }

        /// <summary>
        /// Is the path a folder and not a file?.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>true if the path is a folder; otherwise false.</returns>
        public static bool IsFolder(string path)
        {
            FileAttributes attr = System.IO.File.GetAttributes(path);

            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>
        /// Convenience method for testing a folder has files.  Not recursive.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>true if the folder has files; otherwise false.</returns>
        public static bool FolderHasFiles(string path)
        {
            return Directory.GetFiles(path).Length > 0;
        }

        /// <summary>
        /// Convenience method for testing a folder for sub-folders.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>true if the folder has sub-folders; otherwise false.</returns>
        public static bool FolderHasFolders(string path)
        {
            return Directory.GetDirectories(path).Length > 0;
        }

        /// <summary>
        /// The normal windows definition of an empty folder is one that
        /// has no files, directories don't count.  This method tells you
        /// that the folder is completely empty of files and folders.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>true if the folder is empty; otherwise false.</returns>
        public static bool FolderIsEmpty(string path)
        {
            return !(FolderHasFiles(path) || FolderHasFiles(path));
        }

        /// <summary>
        /// Tries to determine whether a path is an absolute path or not.
        /// Extends Path.IsRooted, which cannot detect the difference
        /// between a relative path e.g. "\relative\subfolder" and the
        /// root folder of a directory or a UNC path.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>true if the path is absolute; otherwise false.</returns>
        public static bool IsAbsolutePath(string path)
        {
            return Uri.TryCreate(path, UriKind.Absolute, out Uri uri) && uri.IsAbsoluteUri;
        }

        /// <summary>
        /// Delete a file.  If the file is read-only, this attribute is removed
        /// otherwise the standard File.Delete method will fail.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public static void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);

                if ((fi.Attributes & FileAttributes.ReadOnly) > 0)
                {
                    fi.Attributes -= FileAttributes.ReadOnly;
                }

                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// Delete all files in a folder and sub-folders.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <param name="pattern">The search pattern to select the files to delete.</param>
        /// <param name="recursive">Indicates whether the search should descend through the sub-folders.</param>
        /// <param name="error">Receives the error message if the operation fails.</param>
        /// <returns>true if the operation succeeded; otherwise false.</returns>
        public static bool DeleteAllFiles(string path, string pattern, bool recursive, out string error)
        {
            if (!TryFindAllFiles(path, pattern, recursive, out string[] found, out error))
            {
                return false;
            }

            foreach (string f in found)
            {
                DeleteFile(f);
            }

            return true;
        }

        /// <summary>
        /// Delete all sub-folders from the specified folder.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>true if the operation succeeded; otherwise false.</returns>
        public static bool DeleteAllFolders(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            foreach (string d in Directory.GetDirectories(path))
            {
                Directory.Delete(d, true);
            }

            return true;
        }

        /// <summary>
        /// Clean the folder of all files and folders.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>true if the operation succeeded; otherwise false.</returns>
        public static bool CleanFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            _ = DeleteAllFiles(path, "*", true, out _);

            DeleteAllFolders(path);

            return true;
        }

        /// <summary>
        /// Remove any read only attributes from the file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public static void MakeReadWrite(string path)
        {
            if (System.IO.File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);

                if ((fi.Attributes & FileAttributes.ReadOnly) > 0)
                {
                    fi.Attributes -= FileAttributes.ReadOnly;
                }
            }
        }

        /// <summary>
        /// Remove any read only attributes any files in a folder hierarchy.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="recursive">Indicates whether the search should descend through sub-folders.</param>
        /// <param name="error">Receives an error message if the operation fails.</param>
        /// <returns>true if the operation succeeded; otherwise false.</returns>
        public static bool MakeAllFilesReadWrite(string path, string pattern, bool recursive, out string error)
        {
            if (!TryFindAllFiles(path, pattern, recursive, out string[] found, out error))
            {
                return false;
            }

            foreach (string f in found)
            {
                FileInfo fi = new FileInfo(f);

                if ((fi.Attributes & FileAttributes.ReadOnly) > 0)
                {
                    fi.Attributes -= FileAttributes.ReadOnly;
                }
            }

            return true;
        }

        /// <summary>
        /// Get an MD5 hash of the contents of the specified file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The MD5 hash result.</returns>
        public static string Md5Hash(string path)
        {
            byte[] fileData;

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fileData = new byte[stream.Length];
                stream.Read(fileData, 0, fileData.Length);
            }

            return Md5Utility.GetHash(fileData);
        }

        /// <summary>
        /// Compare the MD5 hash values of the specified files.
        /// </summary>
        /// <param name="a">The first file to compare.</param>
        /// <param name="b">The second file to compare.</param>
        /// <returns>true if the file hash values are the same; otherwise false.</returns>
        public static bool Md5Compare(string a, string b)
        {
            return Md5Hash(a) == Md5Hash(b);
        }

        /// <summary>
        /// Perform a binary comparison of the two files.
        /// </summary>
        /// <param name="a">The first file to compare.</param>
        /// <param name="b">The second file to compare.</param>
        /// <returns>true if the file bytes are the same; otherwise false.</returns>
        public static bool BinaryCompare(string a, string b)
        {
            FileInfo fileA = new FileInfo(a);
            FileInfo fileB = new FileInfo(b);

            if (fileA.Length != fileB.Length)
            {
                return false;
            }

            using (FileStream streamA = new FileStream(a, FileMode.Open, FileAccess.Read))
            {
                using (FileStream streamB = new FileStream(b, FileMode.Open, FileAccess.Read))
                {
                    const int BufferSize = 4096;

                    byte[] bytesA = new byte[BufferSize];
                    byte[] bytesB = new byte[BufferSize];

                    int bytesReadA, bytesReadB, totalBytesRead = 0;

                    // we know the files are the same length, so we can assume they will stop reading at the same point
                    while ((bytesReadA = streamA.Read(bytesA, 0, BufferSize)) > 0
                        && (bytesReadB = streamB.Read(bytesB, 0, BufferSize)) > 0)
                    {
                        if (bytesReadA != bytesReadB)
                        {
                            // strictly speaking this case should never happen
                            return false;
                        }

                        for (int i = 0; i < bytesReadA; ++i)
                        {
                            if (bytesA[i] != bytesB[i])
                            {
                                return false;
                            }
                        }

                        totalBytesRead += bytesReadA;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Exception safe wrapper for Directory.GetFiles method.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="recursive">Indicates whether the search should descend through sub-folders.</param>
        /// <param name="results">Receives the results of the search.</param>
        /// <param name="error">Receives an error message if the operation fails.</param>
        /// <returns>true if the operation succeeded; otherwise false.</returns>
        public static bool TryFindAllFiles(string path, string pattern, bool recursive, out string[] results, out string error)
        {
            try
            {
                results = Directory.GetFiles(path, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                error = null;
                return true;
            }
            catch (System.Exception e)
            {
                results = null;
                error = e.Message;
                return false;
            }
        }
    }
}
