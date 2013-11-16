// ----------------------------------------------------------------------------
// <copyright file="Win32File.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Win32
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;

    using COM_FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    /// <summary>
    /// Win32 interoperability file functions.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
    public static class Win32File
    {
        /// <summary>
        /// Enumerated File Access permissions.
        /// </summary>
        [Flags]
        public enum FileAccessEnum : uint
        {
            /// <summary>
            /// Read permission.
            /// </summary>
            GenericRead = 0x80000000,
            
            /// <summary>
            /// Write permission.
            /// </summary>
            GenericWrite = 0x40000000,

            /// <summary>
            /// Execute permission.
            /// </summary>
            GenericExecute = 0x20000000,
            
            /// <summary>
            /// All permissions.
            /// </summary>
            GenericAll = 0x10000000
        }

        /// <summary>
        /// Enumerates file sharing options.
        /// </summary>
        [Flags]
        public enum FileShareEnum : uint
        {
            /// <summary>
            /// Prevents other processes from opening a file or device if they request delete, read, or write access.
            /// </summary>
            None = 0x00000000,

            /// <summary>
            /// Enables subsequent open operations on an object to request read access. 
            /// Otherwise, other processes cannot open the object if they request read access. 
            /// If this flag is not specified, but the object has been opened for read access, the function fails.
            /// </summary>
            Read = 0x00000001,

            /// <summary>
            /// Enables subsequent open operations on an object to request write access. 
            /// Otherwise, other processes cannot open the object if they request write access. 
            /// If this flag is not specified, but the object has been opened for write access, the function fails.
            /// </summary>
            Write = 0x00000002,

            /// <summary>
            /// Enables subsequent open operations on an object to request delete access. 
            /// Otherwise, other processes cannot open the object if they request delete access.
            /// If this flag is not specified, but the object has been opened for delete access, the function fails.
            /// </summary>
            Delete = 0x00000004
        }

        /// <summary>
        /// Enumerates an action to take on a file or device that exists or does not exist.
        /// </summary>
        public enum CreationDispositionEnum : uint
        {
            /// <summary>
            /// Creates a new file. The function fails if a specified file exists.
            /// </summary>
            New = 1,

            /// <summary>
            /// Creates a new file, always. 
            /// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes, 
            /// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
            /// </summary>
            CreateAlways = 2,

            /// <summary>
            /// Opens a file. The function fails if the file does not exist. 
            /// </summary>
            OpenExisting = 3,

            /// <summary>
            /// Opens a file, always. 
            /// If a file does not exist, the function creates a file as if <c>dwCreationDisposition</c> is CREATE_NEW.
            /// </summary>
            OpenAlways = 4,

            /// <summary>
            /// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
            /// The calling process must open the file with the GENERIC_WRITE access right. 
            /// </summary>
            TruncateExisting = 5
        }

        /// <summary>
        /// Enumerates file attributes.
        /// </summary>
        [Flags]
        public enum FileAttributesEnum : uint
        {
            /// <summary>
            /// The file is read only. Applications can read the file, but cannot write to or delete it.
            /// </summary>
            Readonly = 0x00000001,

            /// <summary>
            /// The file is hidden. Do not include it in an ordinary directory listing.
            /// </summary>
            Hidden = 0x00000002,

            /// <summary>
            /// The file is part of or used exclusively by an operating system.
            /// </summary>
            System = 0x00000004,

            /// <summary>
            /// The handle that identifies a directory.
            /// </summary>
            Directory = 0x00000010,

            /// <summary>
            /// The file should be archived. Applications use this attribute to mark files for backup or removal.
            /// </summary>
            Archive = 0x00000020,

            /// <summary>
            /// This value is reserved for system use.
            /// </summary>
            Device = 0x00000040,

            /// <summary>
            /// The file does not have other attributes set. This attribute is valid only if used alone.
            /// </summary>
            Normal = 0x00000080,

            /// <summary>
            /// The file is being used for temporary storage.
            /// </summary>
            Temporary = 0x00000100,

            /// <summary>
            /// A file that is a sparse file.
            /// </summary>
            SparseFile = 0x00000200,

            /// <summary>
            /// A file or directory that has an associated reparse point, or a file that is a symbolic link.
            /// </summary>
            ReparsePoint = 0x00000400,

            /// <summary>
            /// A file or directory that is compressed. 
            /// </summary>
            Compressed = 0x00000800,

            /// <summary>
            /// The data of a file is not available immediately. 
            /// </summary>
            Offline = 0x00001000,

            /// <summary>
            /// The file or directory is not to be indexed by the content indexing service.
            /// </summary>
            NotContentIndexed = 0x00002000,

            /// <summary>
            /// A file or directory that is encrypted. For a file, all data streams in the file are encrypted. For a directory, encryption is the default for newly created files and subdirectories.
            /// </summary>
            Encrypted = 0x00004000,

            /// <summary>
            /// Write operations will not go through any intermediate cache, they will go directly to disk.
            /// </summary>
            Write_Through = 0x80000000,

            /// <summary>
            /// The file or device is being opened or created for asynchronous I/O.
            /// </summary>
            Overlapped = 0x40000000,

            /// <summary>
            /// The file or device is being opened with no system caching for data reads and writes.
            /// </summary>
            NoBuffering = 0x20000000,

            /// <summary>
            /// Access is intended to be random. The system can use this as a hint to optimize file caching.
            /// </summary>
            RandomAccess = 0x10000000,

            /// <summary>
            /// Access is intended to be sequential from beginning to end. The system can use this as a hint to optimize file caching.
            /// </summary>
            SequentialScan = 0x08000000,

            /// <summary>
            /// The file is to be deleted immediately after all of its handles are closed, which includes the specified handle and any other open or duplicated handles.
            /// </summary>
            DeleteOnClose = 0x04000000,

            /// <summary>
            /// The file is being opened or created for a backup or restore operation. 
            /// </summary>
            BackupSemantics = 0x02000000,

            /// <summary>
            /// Access will occur according to POSIX rules. This includes allowing multiple files with names, differing only in case, for file systems that support that naming.
            /// </summary>
            PosixSemantics = 0x01000000,

            /// <summary>
            /// Normal reparse point processing will not occur; CreateFile will attempt to open the reparse point. When a file is opened, a file handle is returned, whether or not the filter that controls the reparse point is operational.
            /// </summary>
            OpenReparsePoint = 0x00200000,

            /// <summary>
            /// The file data is requested, but it should continue to be located in remote storage. It should not be transported back to local storage. 
            /// </summary>
            OpenNoRecall = 0x00100000,

            /// <summary>
            /// If you attempt to create multiple instances of a pipe with this flag, creation of the first instance succeeds, but creation of the next instance fails with ERROR_ACCESS_DENIED.
            /// </summary>
            FirstPipeInstance = 0x00080000
        }

        /// <summary>
        /// Retrieves file information for the specified file.
        /// </summary>
        /// <param name="hFile">A handle to the file that contains the information to be retrieved.</param>
        /// <param name="lpFileInformation">A pointer to a BY_HANDLE_FILE_INFORMATION structure that receives the file information.</param>
        /// <returns>If the function succeeds, the return value is nonzero and file information data is contained in the buffer pointed to by the <c>lpFileInformation</c> parameter.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        /// <summary>
        /// The CreateFile function creates or opens a file, file stream, directory, physical disk, volume, console buffer, tape drive,
        /// communications resource, <c>mailslot</c>, or named pipe. The function returns a handle that can be used to access an object.
        /// </summary>
        /// <param name="lpFileName">The name of the file or device to be created or opened.</param>
        /// <param name="dwDesiredAccess"> access to the object, which can be read, write, or both</param>
        /// <param name="dwShareMode">The sharing mode of an object, which can be read, write, both, or none</param>
        /// <param name="lpSecurityAttributes">A pointer to a SECURITY_ATTRIBUTES structure that determines whether or not the returned handle can 
        /// be inherited by child processes. Can be null</param>
        /// <param name="dwCreationDisposition">An action to take on files that exist and do not exist</param>
        /// <param name="dwFlagsAndAttributes">The file attributes and flags. </param>
        /// <param name="hTemplateFile">A handle to a template file with the GENERIC_READ access right. The template file supplies file attributes 
        /// and extended attributes for the file that is being created. This parameter can be null</param>
        /// <returns>If the function succeeds, the return value is an open handle to a specified file. If a specified file exists before the function 
        /// all and <c>dwCreationDisposition</c> is CREATE_ALWAYS or OPEN_ALWAYS, a call to GetLastError returns ERROR_ALREADY_EXISTS, even when the function 
        /// succeeds. If a file does not exist before the call, GetLastError returns 0 (zero).
        /// If the function fails, the return value is INVALID_HANDLE_VALUE. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
                                                    string lpFileName,
                                                    FileAccessEnum dwDesiredAccess,
                                                    FileShareEnum dwShareMode,
                                                    IntPtr lpSecurityAttributes,
                                                    CreationDispositionEnum dwCreationDisposition,
                                                    FileAttributesEnum dwFlagsAndAttributes,
                                                    IntPtr hTemplateFile);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(SafeFileHandle hObject);

        /// <summary>
        /// Contains information that the GetFileInformationByHandle function retrieves.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Using Win32 naming for consistency.")]
        public struct BY_HANDLE_FILE_INFORMATION
        {
            /// <summary>
            /// The file attributes. For possible values and their descriptions, see File Attribute Constants.
            /// </summary>
            public uint dwFileAttributes;
            
            /// <summary>
            /// A FILETIME structure that specifies when a file or directory is created.
            /// </summary>
            public COM_FILETIME ftCreationTime;
            
            /// <summary>
            /// A FILETIME structure. For a file, the structure specifies the last time that a file is read from or written to. 
            /// </summary>
            public COM_FILETIME ftLastAccessTime;
            
            /// <summary>
            /// A FILETIME structure. For a file, the structure specifies the last time that a file is written to.
            /// </summary>
            public COM_FILETIME ftLastWriteTime;
            
            /// <summary>
            /// The serial number of the volume that contains a file.
            /// </summary>
            public uint dwVolumeSerialNumber;
            
            /// <summary>
            /// The high-order part of the file size.
            /// </summary>
            public uint nFileSizeHigh;
            
            /// <summary>
            /// The low-order part of the file size.
            /// </summary>
            public uint nFileSizeLow;
            
            /// <summary>
            /// The number of links to this file. For the FAT file system this member is always 1. For the NTFS file system, it can be more than 1.
            /// </summary>
            public uint nNumberOfLinks;
            
            /// <summary>
            /// The high-order part of a unique identifier that is associated with a file. 
            /// </summary>
            public uint nFileIndexHigh;
            
            /// <summary>
            /// The low-order part of a unique identifier that is associated with a file.
            /// </summary>
            public uint nFileIndexLow;
        }
    }
}
