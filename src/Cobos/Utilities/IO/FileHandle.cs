﻿// ----------------------------------------------------------------------------
// <copyright file="FileHandle.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.IO
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Cobos.Utilities.Win32;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// The file handle is used to uniquely identify a file on disk based on the
    /// underlying file information from the OS.  This allows us to compare file
    /// paths and determine whether two paths refer to the same file.  This is
    /// cannot be done reliably using string paths.
    /// </summary>
    public class FileHandle : IComparable
    {
        /// <summary>
        /// The serial number of the volume that contains a file.
        /// </summary>
        private readonly uint volumeSerialNumber;

        /// <summary>
        /// The high-order part of a unique identifier that is associated with a file.
        /// </summary>
        private readonly uint fileIndexHigh;

        /// <summary>
        /// The low-order part of a unique identifier that is associated with a file.
        /// </summary>
        private readonly uint fileIndexLow;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandle"/> class.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public FileHandle(string path)
        {
            this.FilePath = new NormalisedPath(path);

            SafeFileHandle handleValue = null;

            RuntimeHelpers.PrepareConstrainedRegions();

            try
            {
                handleValue = Win32File.CreateFile(
                                            this.FilePath.Value,
                                            Win32File.FileAccessEnum.GenericRead,
                                            Win32File.FileShareEnum.Read,
                                            IntPtr.Zero,
                                            Win32File.CreationDispositionEnum.OpenExisting,
                                            Win32File.FileAttributesEnum.Normal | Win32File.FileAttributesEnum.Directory | Win32File.FileAttributesEnum.BackupSemantics,
                                            IntPtr.Zero);

                // If the handle is invalid,
                // get the last Win32 error
                // and throw a Win32Exception.
                if (handleValue.IsInvalid)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                if (Win32File.GetFileInformationByHandle(handleValue, out Win32File.BY_HANDLE_FILE_INFORMATION info))
                {
                    this.volumeSerialNumber = info.dwVolumeSerialNumber;
                    this.fileIndexHigh = info.nFileIndexHigh;
                    this.fileIndexLow = info.nFileIndexLow;
                }
                else
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
            finally
            {
                if (handleValue != null && !handleValue.IsInvalid)
                {
                    Win32File.CloseHandle(handleValue);
                }
            }
        }

        /// <summary>
        /// Gets the path to the file on disk.
        /// </summary>
        public NormalisedPath FilePath { get; }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public static bool operator ==(FileHandle a, FileHandle b)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((a is null) || (b is null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.volumeSerialNumber == b.volumeSerialNumber && a.fileIndexHigh == b.fileIndexHigh && a.fileIndexLow == b.fileIndexLow;
        }

        /// <summary>
        /// Determines whether the specified object instances are considered not equal.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>true if the objects are considered not equal; otherwise, false.</returns>
        public static bool operator !=(FileHandle a, FileHandle b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object obj)
        {
            FileHandle other = obj as FileHandle;

            if (other != null)
            {
                if (this.volumeSerialNumber != other.volumeSerialNumber)
                {
                    return (int)(this.volumeSerialNumber - other.volumeSerialNumber);
                }

                if (this.fileIndexHigh != other.fileIndexHigh)
                {
                    return (int)(this.fileIndexHigh - other.fileIndexHigh);
                }

                if (this.fileIndexLow != other.fileIndexLow)
                {
                    return (int)(this.fileIndexLow - other.fileIndexLow);
                }

                return 0; // the same
            }
            else
            {
                throw new ArgumentException("Object is not a FileHandle");
            }
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if ((object)obj == (object)null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            FileHandle h = obj as FileHandle;

            if ((object)h == (object)null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.volumeSerialNumber == h.volumeSerialNumber && this.fileIndexHigh == h.fileIndexHigh && this.fileIndexLow == h.fileIndexLow;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current System.Object.</returns>
        public override int GetHashCode()
        {
            return this.volumeSerialNumber.GetHashCode() ^ this.fileIndexHigh.GetHashCode() ^ this.fileIndexLow.GetHashCode();
        }
    }
}
