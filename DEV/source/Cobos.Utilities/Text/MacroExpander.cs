// ----------------------------------------------------------------------------
// <copyright file="MacroExpander.cs" company="Cobos SDK">
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cobos.Utilities.Text
{
    /// <summary>
    /// Expand inline macro definitions in string values
    /// </summary>
    public class MacroExpander
    {
        /// <summary>
        /// Internal lookup of macros
        /// </summary>
        Dictionary<string, string> _tokens = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Optional format specifier for a macro token.  Useful for 
        /// cases such as Visual Studio where a macro is normally in
        /// the form $(_TOKEN_).
        /// </summary>
        readonly string _format;

        /// <summary>
        /// Cached regular expression pattern based on the _tokens keys.
        /// </summary>
        Regex _regex;

        /// <summary>
        /// Default constructor.  Nuff said.
        /// </summary>
        public MacroExpander()
        {
        }

        /// <summary>
        /// The macro format must include the _TOKEN_ keyword
        /// to indicate where the actual token should be inserted.
        /// E.g. $(_TOKEN_)
        /// </summary>
        /// <param name="format"></param>
        public MacroExpander(string format)
        {
            if (format != null && !format.Contains("_TOKEN_"))
            {
                throw new Exception("Invalid token format supplied to MacroExpander.  Requires _TOKEN_ indicator.");
            }

            _format = format;
        }

        /// <summary>
        /// Copy constructor for merging expanders.
        /// </summary>
        /// <param name="other"></param>
        public MacroExpander(MacroExpander other)
        {
            foreach (string key in other._tokens.Keys)
            {
                _tokens.Add(key, other._tokens[key]);
            }

            _format = other._format;
        }

        /// <summary>
        /// Add the token/value pair.  If the token is already inserted,
        /// the previous value is replaced.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        public void Add(string token, string value)
        {
            if (value == null)
            {
                if (_tokens.ContainsKey(token))
                {
                    _tokens.Remove(token);
                }
                return;
            }

            if (_format != null)
            {
                token = _format.Replace("_TOKEN_", token);
            }

            string found;

            if (!_tokens.TryGetValue(token, out found))
            {
                _tokens.Add(token, value);
            }
            else // replace
            {
                _tokens[token] = value;
            }

            // cleared the cached regex pattern
            _regex = null;
        }

        /// <summary>
        /// Expand the raw string with the macro values.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public string Expand(string raw)
        {
            if (raw == null)
            {
                return null;
            }

            return GetRegExPattern().Replace(raw, delegate(Match match)
                                                                {
                                                                    string found;

                                                                    if (_tokens.TryGetValue(match.Value, out found))
                                                                    {
                                                                        return found;
                                                                    }

                                                                    return match.Value;
                                                                });
        }

        Regex GetRegExPattern()
        {
            if (_regex != null)
            {
                return _regex;
            }

            // copy the token keys to an array 
            string[] regexTokens = new string[_tokens.Keys.Count];
            _tokens.Keys.CopyTo(regexTokens, 0);

            // replace any characters in the token format that contain special regular expression characters
            Regex regexEscape = new Regex(@"\[|\\|\^|\$|\.|\||\?|\*|\+|\(|\)|\{|\}");

            for (int i = 0; i < regexTokens.Length; ++i)
            {
                regexTokens[i] = regexEscape.Replace(regexTokens[i], delegate(Match match)
                                                                                                {
                                                                                                    return @"\" + match.Value;
                                                                                                });
            }

            // now do the actual matching
            _regex = new Regex(string.Join("|", regexTokens), RegexOptions.IgnoreCase);

            return _regex;
        }
    }
}
