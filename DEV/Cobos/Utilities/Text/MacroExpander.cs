// ----------------------------------------------------------------------------
// <copyright file="MacroExpander.cs" company="Cobos SDK">
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
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Expand inline macro definitions in string values
    /// </summary>
    public class MacroExpander
    {
        /// <summary>
        /// Optional format specified for a macro token.  Useful for 
        /// cases such as Visual Studio where a macro is normally in
        /// the form $(_TOKEN_).
        /// </summary>
        private readonly string format;

        /// <summary>
        /// Internal lookup of macros
        /// </summary>
        private Dictionary<string, string> tokens = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Cached regular expression pattern based on the this.tokens keys.
        /// </summary>
        private Regex regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroExpander"/> class.
        /// </summary>
        public MacroExpander()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroExpander"/> class.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <remarks>
        /// The macro format must include the _TOKEN_ keyword
        /// to indicate where the actual token should be inserted.
        /// E.g. $(_TOKEN_)
        /// </remarks>
        public MacroExpander(string format)
        {
            if (format != null && !format.Contains("_TOKEN_"))
            {
                throw new Exception("Invalid token format supplied to MacroExpander.  Requires _TOKEN_ indicator.");
            }

            this.format = format;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroExpander"/> class by copying from an existing object.
        /// </summary>
        /// <param name="other">The object to copy.</param>
        public MacroExpander(MacroExpander other)
        {
            foreach (string key in other.tokens.Keys)
            {
                this.tokens.Add(key, other.tokens[key]);
            }

            this.format = other.format;
        }

        /// <summary>
        /// Add the token/value pair.  If the token is already inserted,
        /// the previous value is replaced.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="value">The value.</param>
        public void Add(string token, string value)
        {
            if (value == null)
            {
                if (this.tokens.ContainsKey(token))
                {
                    this.tokens.Remove(token);
                }

                return;
            }

            if (this.format != null)
            {
                token = this.format.Replace("_TOKEN_", token);
            }

            string found;

            if (!this.tokens.TryGetValue(token, out found))
            {
                this.tokens.Add(token, value);
            }
            else
            {
                this.tokens[token] = value;
            }

            // cleared the cached regex pattern
            this.regex = null;
        }

        /// <summary>
        /// Expand the raw string with the macro values.
        /// </summary>
        /// <param name="raw">The raw string.</param>
        /// <returns>The expanded version of raw.</returns>
        public string Expand(string raw)
        {
            if (raw == null)
            {
                return null;
            }

            return this.GetRegExPattern().Replace(
                                            raw, 
                                            match => 
                                            {
                                                string found;

                                                if (this.tokens.TryGetValue(match.Value, out found))
                                                {
                                                    return found;
                                                }

                                                return match.Value;
                                            });
        }

        /// <summary>
        /// Gets the regex pattern for the macro.
        /// </summary>
        /// <returns>The regex pattern.</returns>
        private Regex GetRegExPattern()
        {
            if (this.regex != null)
            {
                return this.regex;
            }

            // copy the token keys to an array 
            string[] regexTokens = new string[this.tokens.Keys.Count];
            this.tokens.Keys.CopyTo(regexTokens, 0);

            // replace any characters in the token format that contain special regular expression characters
            Regex regexEscape = new Regex(@"\[|\\|\^|\$|\.|\||\?|\*|\+|\(|\)|\{|\}");

            for (int i = 0; i < regexTokens.Length; ++i)
            {
                regexTokens[i] = regexEscape.Replace(
                                                regexTokens[i], 
                                                match =>
                                                {
                                                    return @"\" + match.Value;
                                                });
            }

            // now do the actual matching
            this.regex = new Regex(string.Join("|", regexTokens), RegexOptions.IgnoreCase);

            return this.regex;
        }
    }
}
