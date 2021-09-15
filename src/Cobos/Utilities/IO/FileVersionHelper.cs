// ----------------------------------------------------------------------------
// <copyright file="FileVersionHelper.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
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
        /// Gets the major version number.
        /// </summary>
        public ushort Major { get; } = 0;

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        public ushort Minor { get; } = 0;

        /// <summary>
        /// Gets the build number.
        /// </summary>
        public ushort Build { get; } = 0;

        /// <summary>
        /// Gets the revision number.
        /// </summary>
        public ushort Revision { get; } = 0;

        /// <summary>
        /// Gets the full version number.
        /// </summary>
        public ulong VersionNumber { get; } = 0;

        /// <summary>
        /// Gets the string representation of the version number.
        /// </summary>
        public string StringValue { get; } = null;

        /// <summary>
        /// Gets a value indicating whether this version number is null.
        /// </summary>
        public bool IsNull { get; } = false;

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

            if (!(obj is FileVersionHelper version))
            {
                return false;
            }

            return this.VersionNumber == version.VersionNumber;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="FileVersionHelper"/>.</returns>
        public override int GetHashCode()
        {
            return this.VersionNumber.GetHashCode();
        }
    }
}
