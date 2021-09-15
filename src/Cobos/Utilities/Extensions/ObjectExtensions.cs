// ----------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Extensions
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Extension methods for the <see cref="object"/> class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize the object (usually a struct with StructLayout attributes).
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The bytes representing the object.</returns>
        public static byte[] ConvertToByteArray(this object self)
        {
            int size = Marshal.SizeOf(self);
            IntPtr buffer = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(self, buffer, false);
                byte[] data = new byte[size];
                Marshal.Copy(buffer, data, 0, size);
                return data;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Convert the bytes to an object type.
        /// </summary>
        /// <typeparam name="T">The type of object to convert to.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The object result.</returns>
        public static T ConvertTo<T>(this byte[] self)
        {
            Type type = typeof(T);

            int size = Marshal.SizeOf(type);

            if (size > self.Length)
            {
                return default;
            }

            IntPtr buffer = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.Copy(self, 0, buffer, size);

                return (T)Marshal.PtrToStructure(buffer, type);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
