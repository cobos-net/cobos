// ----------------------------------------------------------------------------
// <copyright file="GuidHelper.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Text
{
    using System;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Helper class for returning a normalized GUID.
    /// </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// Returns a normalized GUID in upper case.
        /// </summary>
        /// <returns>The GUID value.</returns>
        public static string GUID()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        /// <summary>
        /// Returns a normalized GUID in quotes if required.
        /// </summary>
        /// <param name="quote">Indicates whether the returned GUID should be enclosed in quotation marks.</param>
        /// <returns>The GUID value.</returns>
        public static string GUID(bool quote)
        {
            return quote ? GuidHelper.GUID().Quote() : GuidHelper.GUID();
        }
    }
}
