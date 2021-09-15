// ----------------------------------------------------------------------------
// <copyright file="Md5Utility.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Text
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// MD5 wrapper class.
    /// </summary>
    public static class Md5Utility
    {
        /// <summary>
        /// Get an MD5 hash of an input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>An MD5 hash.</returns>
        public static string GetHash(string input)
        {
            return GetHash(Encoding.Default.GetBytes(input));
        }

        /// <summary>
        /// Get an MD5 hash of an input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>An MD5 hash.</returns>
        public static string GetHash(byte[] input)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(input);

            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                buffer.Append(data[i].ToString("x2"));
            }

            return buffer.ToString();
        }
    }
}
