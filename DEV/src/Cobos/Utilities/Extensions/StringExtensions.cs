// ----------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using Cobos.Utilities.Text;

    /// <summary>
    /// Extension methods for the <see cref="String"/> class.
    /// </summary>
    /// <remarks>
    /// Many of these methods are helpers for SQL string handling.
    /// </remarks>
    public static class StringExtensions
    {
        /// <summary>
        /// Tokenize a string and return the result as a single <c>CamelCase</c> value. 
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The <c>CamelCase</c> result.</returns>
        public static string CamelCase(this string self)
        {
            StringBuilder result = new StringBuilder(self.Length);

            string[] tokens = self.Split(' ', '\n', '/', '\\', '<', '>', '&', '?', '#', '!', '%', '\"', '\'');

            foreach (string t in tokens)
            {
                if (string.IsNullOrEmpty(t))
                {
                    continue;
                }

                result.AppendFormat("{0}{1}", char.ToUpper(t[0]), t.Substring(1));
            }

            return result.ToString();
        }

        /// <summary>
        /// Escape all XML entities.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>An escaped XML string.</returns>
        public static string EscapeXml(this string self)
        {
            if (self == string.Empty)
            {
                return self;
            }

            return SecurityElement.Escape(self);
        }

        /// <summary>
        /// Un-escape all XML entities.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>An unescaped XML string.</returns>
        public static string UnescapeXml(this string self)
        {
            return System.Net.WebUtility.HtmlDecode(self);
        }

        /// <summary>
        /// Covert the string to bytes.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The byte representation.</returns>
        public static byte[] ConvertToByteArray(this string self)
        {
            return new System.Text.UTF8Encoding().GetBytes(self);
        }

        /// <summary>
        /// Convert the bytes to a string.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The string representation.</returns>
        public static string ConvertToString(this byte[] self)
        {
            return new System.Text.UTF8Encoding().GetString(self);
        }

        /// <summary>
        /// Tests whether the string is enclosed in quotes: ''
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>true if the string is enclosed in quotes; otherwise false.</returns>
        public static bool IsQuoted(this string self)
        {
            self = self.Trim();

            return self.StartsWith("'") && self.EndsWith("'");
        }

        /// <summary>
        /// Enclose a string in quotes.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>A string enclosed in quotes.</returns>
        /// <remarks>
        /// NOT SQL OR DATABASE SAFE. Use SQLQuote (and family).
        /// </remarks>
        public static string Quote(this string self)
        {
            self = self.Trim();

            // is it already quoted ?
            if (self.StartsWith("'") && self.EndsWith("'"))
            {
                return self;
            }

            return "'" + self + "'";
        }

        /// <summary>
        /// Create SQL safe string value enclosed in quotes.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="maxLength">The maximum column length of the value.</param>
        /// <returns>The SQL string value enclosed in quotes.</returns>
        public static string SQLQuote(this string self, int maxLength)
        {
            if (string.IsNullOrEmpty(self))
            {
                return "''";
            }

            self = self.Trim();

            // return SQL friendly quoted text 
            StringBuilder newValue = new StringBuilder(self);

            // if the string is already quoted, remove quotes so we can check for special characters
            if (self.StartsWith("'") && self.EndsWith("'") && self.Length > 1)  
            {
                // disallow match on only single quote
                newValue.Remove(self.Length - 1, 1);
                newValue.Remove(0, 1);
                self = newValue.ToString();
            }

            // search the string for these characters:
            // single quote, backslash, double quote or question mark
            char[] lookFor = new char[] { '\'', '\\', '"', '?' };

            // must be delimited with ...
            // another single quote, backslash before real backslash, backslash before double quoute, backslash before questionmark
            char[] appendTo = new char[] { '\'', '\\', '\\', '\\' };

            // TODO: perhaps could use regex here.  Not sure if that will be quicker though... 
            int k = 0;
            for (int i = 0; i < self.Length; ++i, ++k)
            {
                for (int j = 0; j < lookFor.Length; ++j)
                {
                    if (self[i] == lookFor[j])
                    {
                        newValue.Insert(k++, appendTo[j]);
                    }
                }
            }

            if (maxLength != 0)
            {
                if (newValue.Length > maxLength)
                {
                    newValue.Length = maxLength;
                }
            }

            newValue.Insert(0, '\'');
            newValue.Append('\'');
            return newValue.ToString();
        }

        /// <summary>
        /// A simpler version of SQLQuote that doesn't test for embedded quotes.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The SQL string value enclosed in quotes.</returns>
        public static string SQLQuoteID(this string self)
        {
            if (self.IsQuoted())
            {
                return self;
            }

            // if there's guaranteed no funny chars, like an ID field - do it the easy way
            return "'" + self + "'";
        }

        /// <summary>
        /// Create SQL safe string value enclosed in quotes or NULL if no value.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The quoted string if self is not null or empty; otherwise NULL.</returns>
        public static string SQLQuoteOrNULL(this string self)
        {
            if (string.IsNullOrEmpty(self))
            {
                return "NULL";
            }
            else
            {
                return SQLQuote(self, 0);
            }
        }

        /// <summary>
        /// Create SQL safe string value enclosed in quotes or NULL if no value.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="maxLength">The maximum column length of the value.</param>
        /// <returns>The quoted string if self is not null or empty; otherwise NULL.</returns>
        public static string SQLQuoteOrNULL(this string self, int maxLength)
        {
            if (string.IsNullOrEmpty(self))
            {
                return "NULL";
            }
            else
            {
                return SQLQuote(self, maxLength);
            }
        }

        /// <summary>
        /// Create SQL safe string value enclosed in quotes.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The SQL string value enclosed in quotes.</returns>
        public static string SQLQuote(this string self)
        {
            return SQLQuote(self, 0);
        }

        /// <summary>
        /// Convert a list to quoted strings.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>An array of quoted strings.</returns>
        public static string[] GetQuotedValues(this List<string> self)
        {
            List<string> quotedids = self.ConvertAll(s => s.SQLQuote());

            return quotedids.ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether this string is a phone number.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>true if the object is a phone number; otherwise false.</returns>
        public static bool IsPhoneNumber(this string self)
        {
            return RegExHelper.PhoneExpression.IsMatch(self);
        }

        /// <summary>
        /// Gets a value indicating whether this string is a time value.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>true if the object is a time value; otherwise false.</returns>
        public static bool IsTime(this string self)
        {
            return RegExHelper.TimeExpression.IsMatch(self);
        }

        /// <summary>
        /// Gets a value indicating whether this string is a numeric value.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>true if the object is a numeric value; otherwise false.</returns>
        public static bool IsNumeric(this string self)
        {
            return RegExHelper.NumericExpression.IsMatch(self);
        }

        /// <summary>
        /// Validate a string field for null or empty values.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="name">The name of the field.</param>
        /// <exception cref="ArgumentException">Throws an exception if the value is null or empty.</exception>
        public static void ValidateEmptyField(this string self, string name)
        {
            if (string.IsNullOrEmpty(self))
            {
                throw new ArgumentException(name + " cannot be an empty field");
            }
        }

        /// <summary>
        /// Validate a collection of string field for null or empty values.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="names">The names of the fields in the corresponding collection.</param>
        /// <exception cref="ArgumentException">Throws an exception if the value is null or empty.</exception>
        public static void ValidateEmptyFields(this string[] self, string[] names)
        {
            if (self.Length != names.Length)
            {
                string exceptionString = "Arrays must have identical length in function ValidateEmptyFields.";
                throw new ArgumentException(exceptionString);
            }

            for (int i = 0; i < self.Length; ++i)
            {
                if (string.IsNullOrEmpty(self[i]))
                {
                    throw new ArgumentException(names[i] + " cannot be an empty field");
                }
            }
        }

        /// <summary>
        /// Minify the string, removing \r\n\t and excess whitespace
        /// Particularly useful when reading CDATA from Xml.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>A copy of the string with all extraneous whitespace removed.</returns>
        public static string RemoveWhitespace(this string self)
        {
            char[] arr = self.ToCharArray();

            for (int i = 0; i < arr.Length; i++)
            {
                switch (arr[i])
                {
                case '\t':
                case '\r':
                case '\n':
                    arr[i] = ' ';
                    break;
                }
            }

            self = new string(arr);

            self = Regex.Replace(self, @"\s+", " ");

            return self.Trim();
        }
    }
}
