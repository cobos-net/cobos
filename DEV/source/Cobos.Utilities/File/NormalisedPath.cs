// ----------------------------------------------------------------------------
// <copyright file="NormalisedPath.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.File
{
    using System;
    using System.IO;

    /// <summary>
    /// A normalized representation of a file or folder path.
    /// </summary>
    public class NormalisedPath : IComparable, IComparable<NormalisedPath>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalisedPath"/> class.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        public NormalisedPath(string path)
        {
            this.Value = NormalisePath(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalisedPath"/> class 
        /// by creating an absolute normalized path from a relative path.
        /// </summary>
        /// <param name="relativePath">The relative file path</param>
        /// <param name="relativeTo">This must be a folder.  The class cannot easily differentiate
        /// between a folder path and a file with no extension.  We cannot always test whether
        /// it's a file or folder because there is no guarantee that the file or folder will exist</param>
        public NormalisedPath(string relativePath, string relativeTo)
        {
            this.Value = NormalisePath(relativeTo + @"\" + relativePath);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="NormalisedPath"/> class from being created.
        /// </summary>
        /// <remarks>
        /// To provide a more efficient method of copying some or all of a normalized path, 
        /// bypassing the normal parsing required of an arbitrary path.
        /// </remarks>
        private NormalisedPath()
        {
        }

        /// <summary>
        /// Gets the current value of the path.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the path contains anything.
        /// </summary>
        public bool IsNullPath
        {
            get
            {
                return string.IsNullOrEmpty(this.Value);
            }
        }

        /// <summary>
        /// Normalize a path to lower case, single Windows style delimiters with no
        /// dotted directory paths.
        /// </summary>
        /// <param name="path">The input path.</param>
        /// <returns>The normalized path representation.</returns>
        public static string NormalisePath(string path)
        {
            // Normally we identify a file using file information such as volume serial, and file index values.
            // This is OK as long as the file exists, which will not always be the case.
            string pathSeperator = Path.DirectorySeparatorChar.ToString();

            // enable quick matching by forcing everything to lower case
            // force Windows style path delimiters and remove quoted paths
            path = path.ToLower().Replace('/', Path.DirectorySeparatorChar).Replace("\"", string.Empty).Replace("'", string.Empty);

            // now remove any \. current and \.. relative directory paths
            string[] directories = path.Split(new char[] { Path.DirectorySeparatorChar });
            string[] fullpath = new string[directories.Length];

            // crude test for unc path specifiers.  Remember this for later, the processing strips the
            // leading '\\' characters out.
            bool isUnc = false;

            if (path.Length >= 2)
            {
                isUnc = path[0] == Path.DirectorySeparatorChar && path[1] == Path.DirectorySeparatorChar;
            }

            int d = 0, f = 0;

            for (; d < directories.Length; ++d)
            {
                string directory = directories[d].Trim();

                if (string.IsNullOrEmpty(directory) || directory == ".")
                {
                    // don't copy this one, either empty (double delimiter) or current directory
                }
                else if (directory == "..")
                {
                    // to resolve the path, dont copy this element or the previous one
                    --f;
                }
                else
                {
                    fullpath[f++] = directory;
                }
            }

            if (isUnc)
            {
                // re-apply the leading unc path specifier
                return @"\\" + string.Join(pathSeperator, fullpath, 0, f);
            }
            else
            {
                return string.Join(pathSeperator, fullpath, 0, f);
            }
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public static bool operator ==(NormalisedPath a, NormalisedPath b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Value == b.Value;
        }

        /// <summary>
        /// Determines whether the specified object instances are not considered equal.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>true if the objects are considered not equal; otherwise, false.</returns>
        public static bool operator !=(NormalisedPath a, NormalisedPath b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Gets a normalized directory representation of the path.
        /// </summary>
        /// <returns>A normalized representation of the directory.</returns>
        public NormalisedPath GetDirectoryName()
        {
            NormalisedPath path = new NormalisedPath();
            path.Value = Path.GetDirectoryName(this.Value);
            return path;
        }

        /// <summary>
        /// Get the filename of the path.
        /// </summary>
        /// <returns>The filename part of the path.</returns>
        public string GetFileName()
        {
            return Path.GetFileName(this.Value);
        }

        /// <summary>
        /// Get the filename without the extension.
        /// </summary>
        /// <returns>The filename without the extension.</returns>
        public string GetFileNameWithoutExtension()
        {
            return Path.GetFileNameWithoutExtension(this.Value);
        }

        /// <summary>
        /// Get the extension of the file.
        /// </summary>
        /// <returns>The extension of the file.</returns>
        public string GetExtension()
        {
            return Path.GetExtension(this.Value);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object obj)
        {
            if (obj != null && !(obj is NormalisedPath))
            {
                throw new ArgumentException("Object must be of type NormalisedPath.");
            }

            return this.CompareTo(obj as NormalisedPath);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(NormalisedPath obj)
        {
            if (obj == null)
            {
                return 1;
            }

            return this.Value.CompareTo(obj.Value);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            NormalisedPath p = obj as NormalisedPath;

            if ((object)p == null)
            {
                return false;
            }

            return this.Value == p.Value;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="NormalisedPath"/></returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}
