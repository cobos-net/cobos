using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;
using Intergraph.Oz.Utilities.Win32;

namespace Intergraph.Oz.Utilities.File
{
	/// <summary>
	/// The file handle is used to uniquely identify a file on disk based on the
	/// underlying file information from the OS.  This allows us to compare file
	/// paths and determine whether two paths refer to the same file.  This is 
	/// cannot be done reliably using string paths.
	/// </summary>
	public class FileHandle : IComparable 
	{
		#region Properties

		public readonly NormalisedPath FilePath;

		readonly uint VolumeSerialNumber;
		readonly uint FileIndexHigh;
		readonly uint FileIndexLow;

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		public FileHandle( string path )
		{
			FilePath = new NormalisedPath( path );
			
			SafeFileHandle handleValue = null;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				handleValue = Win32File.CreateFile( FilePath.Value,
																Win32File.EFileAccess.GenericRead,
																Win32File.EFileShare.Read,
																IntPtr.Zero,
																Win32File.ECreationDisposition.OpenExisting,
																Win32File.EFileAttributes.Normal | Win32File.EFileAttributes.Directory | Win32File.EFileAttributes.BackupSemantics,
																IntPtr.Zero );

				// If the handle is invalid,
				// get the last Win32 error 
				// and throw a Win32Exception.
				if ( handleValue.IsInvalid )
				{
					Marshal.ThrowExceptionForHR( Marshal.GetHRForLastWin32Error() );
				}

				Win32File.BY_HANDLE_FILE_INFORMATION info;
				
				if ( Win32File.GetFileInformationByHandle( handleValue, out info ) )
				{
					VolumeSerialNumber = info.VolumeSerialNumber;
					FileIndexHigh = info.FileIndexHigh;
					FileIndexLow = info.FileIndexLow;
				}
				else
				{
					Marshal.ThrowExceptionForHR( Marshal.GetHRForLastWin32Error() );
				}
			}
			finally
			{
				if ( handleValue != null && !handleValue.IsInvalid )
				{
					Win32File.CloseHandle( handleValue );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo( object obj )
		{
			FileHandle other = obj as FileHandle;
			
			if ( other != null )
			{
				if ( VolumeSerialNumber != other.VolumeSerialNumber )
				{
					return (int)(VolumeSerialNumber - other.VolumeSerialNumber); 
				}
				if ( FileIndexHigh != other.FileIndexHigh )
				{
					return (int)(FileIndexHigh - other.FileIndexHigh);
				}
				if ( FileIndexLow != other.FileIndexLow )
				{
					return (int)(FileIndexLow - other.FileIndexLow);
				}
				return 0; // the same
			}
			else
			{
				throw new ArgumentException( "Object is not a FileHandle" );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals( System.Object obj )
		{
			// If parameter is null return false.
			if ( obj == null )
			{
				return false;
			}

			// If parameter cannot be cast to Point return false.
			FileHandle h = obj as FileHandle;
			if ( (System.Object)h == null )
			{
				return false;
			}

			// Return true if the fields match:
			return VolumeSerialNumber == h.VolumeSerialNumber && FileIndexHigh == h.FileIndexHigh && FileIndexLow == h.FileIndexLow;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==( FileHandle a, FileHandle b )
		{
			// If both are null, or both are same instance, return true.
			if ( System.Object.ReferenceEquals( a, b ) )
			{
				return true;
			}

			// If one is null, but not both, return false.
			if ( ((object)a == null) || ((object)b == null) )
			{
				return false;
			}

			// Return true if the fields match:
			return a.VolumeSerialNumber == b.VolumeSerialNumber && a.FileIndexHigh == b.FileIndexHigh && a.FileIndexLow == b.FileIndexLow;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=( FileHandle a, FileHandle b )
		{
			return !(a == b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return VolumeSerialNumber.GetHashCode() ^ FileIndexHigh.GetHashCode() ^ FileIndexLow.GetHashCode();
		}

	}
}
