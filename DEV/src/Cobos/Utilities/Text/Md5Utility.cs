// ----------------------------------------------------------------------------
// <copyright file="Md5Utility.cs" company="Cobos SDK">
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
