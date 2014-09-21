// ----------------------------------------------------------------------------
// <copyright file="FileVersionHelper.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
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

namespace Cobos.Utilities.IO
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class for extracting file version information.
    /// </summary>
    public class FileVersionHelper : IComparable, IComparable<FileVersionHelper>
    {
        /// <summary>
        /// The major version number.
        /// </summary>
        public readonly ushort Major = 0;
        
        /// <summary>
        /// The minor version number.
        /// </summary>
        public readonly ushort Minor = 0;
        
        /// <summary>
        /// The build number.
        /// </summary>
        public readonly ushort Build = 0; 
        
        /// <summary>
        /// The revision number.
        /// </summary>
        public readonly ushort Revision = 0;

        /// <summary>
        /// The full version number.
        /// </summary>
        public readonly ulong VersionNumber = 0;

        /// <summary>
        /// The string representation of the version number.
        /// </summary>
        public readonly string StringValue = null;

        /// <summary>
        /// Indicates whether this version number is null.
        /// </summary>
        public readonly bool IsNull = false;

        /// <summary>
        /// Regular expression for parsing the version information.
        /// </summary>
        private const string RegexVersion = @"\s*(\d+)\s*[.|,]\s*(\d+)\s*[.|,]\s*(\d+)(?:\s*[.|,]\s*(\d+))?";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileVersionHelper"/> class.
        /// </summary>
        /// <param name="path">The path to the image containing the version information.</param>
        public FileVersionHelper(string path)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);

            if (info == null || info.FileVersion == null)
            {
                this.IsNull = true;
                return;
            }

            string[] tokens = Regex.Split(info.FileVersion, RegexVersion);

            if (tokens == null || tokens.Length == 1)
            {
                this.IsNull = true;
                return;
            }

            this.Major = Convert.ToUInt16(tokens[1]);
            this.Minor = Convert.ToUInt16(tokens[2]);
            this.Build = Convert.ToUInt16(tokens[3]);

            this.VersionNumber = ((ulong)this.Major << 48) | ((ulong)this.Minor << 32) | (uint)(this.Build << 16) | this.Revision;

            if (tokens.Length == 4)
            {
                this.StringValue = string.Format("{0}.{1}.{2}", tokens[1], tokens[2], tokens[3]);
            }
            else
            {
                this.StringValue = string.Format("{0}.{1}.{2}.{3}", tokens[1], tokens[2], tokens[3], tokens[5]);
                this.Revision = Convert.ToByte(tokens[5]);
            }
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="lhs">The first object to compare.</param>
        /// <param name="rhs">The second object to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public static bool operator ==(FileVersionHelper lhs, FileVersionHelper rhs)
        {
            return lhs.VersionNumber == rhs.VersionNumber;
        }

        /// <summary>
        /// Determines whether the specified object instances are not considered equal.
        /// </summary>
        /// <param name="lhs">The first object to compare.</param>
        /// <param name="rhs">The second object to compare.</param>
        /// <returns>true if the objects are considered not equal; otherwise, false.</returns>
        public static bool operator !=(FileVersionHelper lhs, FileVersionHelper rhs)
        {
            return lhs.VersionNumber != rhs.VersionNumber;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object obj)
        {
            if (obj != null && !(obj is FileVersionHelper))
            {
                throw new ArgumentException("Object must be of type FileVersionHelper.");
            }

            return this.CompareTo(obj as FileVersionHelper);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(FileVersionHelper obj)
        {
            if (obj == null)
            {
                return 1;
            }

            return this.VersionNumber.CompareTo(obj.VersionNumber);
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

            FileVersionHelper version = obj as FileVersionHelper;

            if ((object)version == null)
            {
                return false;
            }

            return this.VersionNumber == version.VersionNumber;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="FileVersionHelper"/></returns>
        public override int GetHashCode()
        {
            return this.VersionNumber.GetHashCode();
        }
    }
}
