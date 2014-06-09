// ----------------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Cobos SDK">
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
    /// <summary>
    /// Extension methods for enumerated types.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Convert a enumeration to a string value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The string representation of the enumerated value.</returns>
        public static string ToEnumString<T>(this T self)
        {
            return System.Enum.GetName(typeof(T), self);
        }

        /// <summary>
        /// Convert a null-able enumeration to a string value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The string representation of the enumerated value if the null-able type has a value; otherwise null.</returns>
        public static string ToEnumString<T>(this T? self)
            where T : struct
        {
            if (self.HasValue == false)
            {
                return null;
            }

            return System.Enum.GetName(typeof(T), self.Value);
        }

        /// <summary>
        /// Converts a string to an enumerated value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The enumerated value.</returns>
        /// <exception cref="ArgumentException">If the string is null or empty.</exception>
        public static T ToEnum<T>(this string self)
        {
            if (string.IsNullOrEmpty(self))
            {
                throw new System.ArgumentException("Cannot convert a null or empty string to an enumeration");
            }

            return (T)System.Enum.Parse(typeof(T), self, true);
        }
    }
}
