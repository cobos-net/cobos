// ----------------------------------------------------------------------------
// <copyright file="StringSeparator.cs" company="Cobos SDK">
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
    using System;

    /// <summary>
    /// Extract a token from a delimited text string
    /// </summary>
    public static class StringSeparator
    {
        /// <summary>
        /// Extract a token from a delimited text string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="separator">The separator token, e.g. | , - etc.</param>
        /// <param name="index">The index of the token to extract</param>
        /// <returns>The token if a valid token exists at the specified index, otherwise null.</returns>
        public static string GetTokenAt(string source, char separator, int index)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            if (index < 0)
            {
                return null;
            }

            string[] tokens = source.Split(separator);

            if (index >= tokens.Length)
            {
                return null;
            }

            return tokens[index];
        }
    }
}
