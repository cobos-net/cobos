// ----------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Extension methods for the <see cref="Object"/> class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// extend the object class to support casting to an anonymous type
        /// </summary>
        /// <typeparam name="T">The anonymous type to cast to</typeparam>
        /// <param name="self">The 'this' object reference</param>
        /// <param name="example">An instance of an anonymous type</param>
        /// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
        public static T CastByExample<T>(this object self, T example)
        {
            try
            {
                return (T)self;
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Serialize the object (usually a struct with StructLayout attributes)
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The bytes representing the object.</returns>
        public static byte[] ConvertToByteArray(this object self)
        {
            int size = Marshal.SizeOf(self);

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(self, buffer, false);

            byte[] data = new byte[size];
            Marshal.Copy(buffer, data, 0, size);
            Marshal.FreeHGlobal(buffer);

            return data;
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
                return default(T);
            }

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(self, 0, buffer, size);

            T obj = (T)Marshal.PtrToStructure(buffer, type);
            Marshal.FreeHGlobal(buffer);

            return obj;
        }
    }
}
