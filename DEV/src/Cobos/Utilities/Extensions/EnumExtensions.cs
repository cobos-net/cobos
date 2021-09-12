// ----------------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
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
        /// <exception cref="System.ArgumentException">If the string is null or empty.</exception>
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
