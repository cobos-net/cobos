using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Intergraph.AsiaPac.Utilities.Extensions
{
	public static class ObjectExtensions
	{
		/// <summary>
		/// extend the object class to support casting to an anonymous type
		/// </summary>
		/// <typeparam name="T">The anonymous type to cast to</typeparam>
		/// <param name="obj">The 'this' object reference</param>
		/// <param name="example">An instance of an anonymous type</param>
		/// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
		public static T CastByExample<T>( this object obj, T example )
		{
			try
			{
				return (T)obj;
			}
			catch ( InvalidCastException )
			{
				return default( T );
			}
		}

		/// <summary>
		/// Serialize the object (usually a struct with StructLayout attributes)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static byte[] ConvertToByteArray( this object obj )
		{
			int size = Marshal.SizeOf( obj );

			IntPtr buffer = Marshal.AllocHGlobal( size );
			Marshal.StructureToPtr( obj, buffer, false );

			byte[] data = new byte[ size ];
			Marshal.Copy( buffer, data, 0, size );
			Marshal.FreeHGlobal( buffer );
			
			return data;
		}

		public static T ConvertTo<T>( this byte[] data )
		{
			Type type = typeof( T );

			int size = Marshal.SizeOf( type );

			if ( size > data.Length )
			{
				return default(T);
			}

			IntPtr buffer = Marshal.AllocHGlobal( size );
			Marshal.Copy( data, 0, buffer, size );

			T obj = (T)Marshal.PtrToStructure( buffer, type );
			Marshal.FreeHGlobal( buffer );

			return obj;
		} 
	}
}
