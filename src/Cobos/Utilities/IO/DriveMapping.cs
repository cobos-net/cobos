// ----------------------------------------------------------------------------
// <copyright file="DriveMapping.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.IO
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Utility methods for mapping network drives in Windows.
    /// </summary>
    public static partial class DriveMapping
    {
        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint ResourceTypeDisk = 1;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint ConnectUpdateProfile = 0x1;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
#pragma warning disable IDE0051 // Remove unused private members
        private const uint ConnectInteractive = 0x8;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint ConnectPrompt = 0x10;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint ConnectRedirect = 0x80;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint ConnectCommandLine = 0x800;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint ConnectCommandSaveCredentials = 0x1000;
#pragma warning restore IDE0051 // Remove unused private members

        /// <summary>
        /// No error code taken from WINERR.h.
        /// </summary>
        private const uint ErrorSuccess = 0x0;

        /// <summary>
        /// Map a network resource to a drive letter.
        /// </summary>
        /// <param name="driveLetter">The drive letter to map the resource to.</param>
        /// <param name="path">The path of the network resource.</param>
        /// <returns>true if the operation succeeded; otherwise false.</returns>
        public static bool MapNetworkDrive(char driveLetter, string path)
        {
            RemoveNetworkDrive(driveLetter);

            NETRESOURCE networkResource = new NETRESOURCE
            {
                ResourceType = ResourceTypeDisk,
                LocalName = driveLetter + ":",
                RemoteName = path,
                Provider = null,
            };

            uint result = WNetAddConnection2(ref networkResource, null, null, 0);

            return result == ErrorSuccess;
        }

        /// <summary>
        /// Remove a drive mapping, if it exists.
        /// </summary>
        /// <param name="driveLetter">The drive letter to remove.</param>
        /// <returns>true if the operation succeeded; otherwise false.</returns>
        public static bool RemoveNetworkDrive(char driveLetter)
        {
            if (!DriveExists(driveLetter))
            {
                return true;
            }

            uint result = WNetCancelConnection2(driveLetter + ":", ConnectUpdateProfile, false);

            return result == ErrorSuccess;
        }

        [DllImport("mpr.dll")]
        private static extern uint WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, uint dwFlags);

        [DllImport("mpr.dll")]
        private static extern uint WNetAddConnection3(IntPtr hWndOwner, ref NETRESOURCE lpNetResource, string lpPassword, string lpUserName, uint dwFlags);

        [DllImport("mpr.dll")]
        private static extern uint WNetCancelConnection2(string lpName, uint dwFlags, bool bForce);

        /// <summary>
        /// The NETRESOURCE structure contains information about a network resource.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            /// <summary>
            /// Specifies the scope of the enumeration.
            /// </summary>
            public uint Scope;

            /// <summary>
            /// Specifies a bitmask that gives the resource type.
            /// </summary>
            public uint ResourceType;

            /// <summary>
            /// Specifies how the network object should be displayed in a network browsing user interface.
            /// </summary>
            public uint DisplayType;

            /// <summary>
            /// Specifies a bitmask that gives the resource usage.
            /// </summary>
            public uint Usage;

            /// <summary>
            /// The local name of a network resource if the dwScope member is RESOURCE_CONNECTED or RESOURCE_REMEMBERED. This member is NULL if the connection does not have a local name.
            /// </summary>
            public string LocalName;

            /// <summary>
            /// The remote network name if the entry is a network resource.
            /// </summary>
            public string RemoteName;

            /// <summary>
            /// Provider-supplied comment.
            /// </summary>
            public string Comment;

            /// <summary>
            /// The name of the provider owning this resource. This member can be NULL if the provider name is unknown.
            /// </summary>
            public string Provider;
        }
    }

    /// <summary>
    /// Utility methods for mapping local drives in Windows.
    /// </summary>
    /// <remarks>
    /// DOS folder mapping functions taken from:
    /// http://bytes.com/topic/c-sharp/answers/517053-virtual-drive.
    /// </remarks>
    public static partial class DriveMapping
    {
        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
#pragma warning disable IDE0051 // Remove unused private members
        private const uint RawTargetPath = 0x00000001;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint RemoveDefinition = 0x00000002;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint ExactMatchOnRemove = 0x00000004;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint NoBroadcastSystem = 0x00000008;
#pragma warning restore IDE0051 // Remove unused private members

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const string MappedFolderIndicator = @"\??\";

        /// <summary>
        /// Map the folder to a drive letter.
        /// </summary>
        /// <param name="driveLetter">Drive letter in the format "C:" without a back slash.</param>
        /// <param name="folderName">Folder to map without a back slash.</param>
        /// <returns>true if the method succeeded; otherwise false.</returns>
        public static bool MapLocalFolder(char driveLetter, string folderName)
        {
            // Is this drive already mapped? If so, we don't remap it!
            StringBuilder volumeMap = new StringBuilder(1024);
            QueryDosDevice(driveLetter + ":", volumeMap, 1024U);

            if (volumeMap.ToString().StartsWith(MappedFolderIndicator))
            {
                if (!RemoveLocalFolder(driveLetter, null))
                {
                    return false;
                }
            }

            // Map the folder to the drive
            if (DefineDosDevice(0, driveLetter + ":", folderName))
            {
                return true;
            }
            else
            {
                // Display a status message to the user.
                string statusMessage = new Win32Exception(Marshal.GetLastWin32Error()).ToString();
                System.Diagnostics.Debug.WriteLine(statusMessage.Substring(statusMessage.IndexOf(":") + 1));

                return false;
            }
        }

        /// <summary>
        /// Un-map a drive letter. Always un-map the drive, without checking the folder name.
        /// </summary>
        /// <param name="driveLetter">Drive letter to be released, the the format "C:".</param>
        /// <param name="folderName">Folder name that the drive is mapped to.</param>
        /// <returns>true if the method succeeded; otherwise false.</returns>
        public static bool RemoveLocalFolder(char driveLetter, string folderName)
        {
            if (DefineDosDevice(RemoveDefinition, driveLetter + ":", folderName))
            {
                return true;
            }
            else
            {
                // Display the status of the "last" unmap we run.
                string statusMessage = new Win32Exception(Marshal.GetLastWin32Error()).ToString();
                System.Diagnostics.Debug.WriteLine(statusMessage.Substring(statusMessage.IndexOf(":") + 1));

                return false;
            }
        }

        /// <summary>
        /// Returns the folder that a drive is mapped to. If not mapped, return an empty string.
        /// </summary>
        /// <param name="driveLetter">The drive letter.</param>
        /// <returns>The folder if the drive is mapped; otherwise empty.</returns>
        public static string DriveIsMappedToFolder(char driveLetter)
        {
            StringBuilder volumeMap = new StringBuilder(512);
            string mappedVolumeName = string.Empty;

            // If it's not a mapped drive, just remove it from the list
            uint mapped = QueryDosDevice(driveLetter + ":", volumeMap, 512U);

            if (mapped != 0)
            {
                if (volumeMap.ToString().StartsWith(MappedFolderIndicator))
                {
                    // It's a mapped drive, so return the mapped folder name
                    mappedVolumeName = volumeMap.ToString().Substring(4);
                }
            }

            return mappedVolumeName;
        }

        /// <summary>
        /// Check if a drive exists.
        /// </summary>
        /// <param name="driveLetter">The drive letter to test.</param>
        /// <returns>true if the drive exists; otherwise false.</returns>
        public static bool DriveExists(char driveLetter)
        {
            driveLetter = char.ToUpper(driveLetter);

            string[] allDrives = Directory.GetLogicalDrives();

            foreach (string d in allDrives)
            {
                if (char.ToUpper(d[0]) == driveLetter)
                {
                    return true;
                }
            }

            return false;
        }

        [DllImport("kernel32.dll")]
        private static extern bool DefineDosDevice(uint dwFlags, string lpDeviceName, string lpTargetPath);

        [DllImport("Kernel32.dll")]
        private static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, uint ucchMax);
    }
}
