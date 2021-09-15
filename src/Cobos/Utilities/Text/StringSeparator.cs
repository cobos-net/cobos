// ----------------------------------------------------------------------------
// <copyright file="StringSeparator.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Text
{
    /// <summary>
    /// Extract a token from a delimited text string.
    /// </summary>
    public static class StringSeparator
    {
        /// <summary>
        /// Extract a token from a delimited text string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="separator">The separator token, e.g. | , - etc.</param>
        /// <param name="index">The index of the token to extract.</param>
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
