// ----------------------------------------------------------------------------
// <copyright file="DriveMapping.cs" company="Cobos SDK">
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
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Utility methods for mapping network drives in Windows.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Using Win32 naming for consistency.")]
    public static partial class DriveMapping
    {
        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint RESOURCETYPE_DISK = 1;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint CONNECT_UPDATE_PROFILE = 0x1;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint CONNECT_INTERACTIVE = 0x8;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint CONNECT_PROMPT = 0x10;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint CONNECT_REDIRECT = 0x80;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint CONNECT_COMMANDLINE = 0x800;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint CONNECT_CMD_SAVECRED = 0x1000;

        /// <summary>
        /// No error code taken from WINERR.h.
        /// </summary>
        private const uint ERROR_SUCCESS = 0x0;

        /// <summary>
        /// Map a network resource to a drive letter.
        /// </summary>
        /// <param name="driveLetter">The drive letter to map the resource to.</param>
        /// <param name="path">The path of the network resource.</param>
        /// <returns>true if the operation succeeded; otherwise false.</returns>
        public static bool MapNetworkDrive(char driveLetter, string path)
        {
            RemoveNetworkDrive(driveLetter);

            NETRESOURCE networkResource = new NETRESOURCE();
            networkResource.dwType = RESOURCETYPE_DISK;
            networkResource.lpLocalName = driveLetter + ":";
            networkResource.lpRemoteName = path;
            networkResource.lpProvider = null;

            uint result = WNetAddConnection2(ref networkResource, null, null, 0);

            return result == ERROR_SUCCESS;
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

            uint result = WNetCancelConnection2(driveLetter + ":", CONNECT_UPDATE_PROFILE, false);

            return result == ERROR_SUCCESS;
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
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Using Win32 naming for consistency.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Using Win32 naming for consistency.")]
        public struct NETRESOURCE
        {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }
    }

    /// <summary>
    /// Utility methods for mapping local drives in Windows.
    /// </summary>
    /// <remarks>
    /// DOS folder mapping functions taken from:
    /// http://bytes.com/topic/c-sharp/answers/517053-virtual-drive
    /// </remarks>
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Using Win32 naming for consistency.")]
    public static partial class DriveMapping
    {
        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint DDD_RAW_TARGET_PATH = 0x00000001;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint DDD_REMOVE_DEFINITION = 0x00000002;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint DDD_EXACT_MATCH_ON_REMOVE = 0x00000004;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const uint DDD_NO_BROADCAST_SYSTEM = 0x00000008;

        /// <summary>
        /// See the Win32 API documentation.
        /// </summary>
        private const string MAPPED_FOLDER_INDICATOR = @"\??\";

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
            QueryDosDevice(driveLetter + ":", volumeMap, (uint)1024);

            if (volumeMap.ToString().StartsWith(MAPPED_FOLDER_INDICATOR))
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
        /// <param name="driveLetter">Drive letter to be released, the the format "C:"</param>
        /// <param name="folderName">Folder name that the drive is mapped to.</param>
        /// <returns>true if the method succeeded; otherwise false.</returns>
        public static bool RemoveLocalFolder(char driveLetter, string folderName)
        {
            if (DefineDosDevice(DDD_REMOVE_DEFINITION, driveLetter + ":", folderName))
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
            uint mapped = QueryDosDevice(driveLetter + ":", volumeMap, (uint)512);

            if (mapped != 0)
            {
                if (volumeMap.ToString().StartsWith(MAPPED_FOLDER_INDICATOR))
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
