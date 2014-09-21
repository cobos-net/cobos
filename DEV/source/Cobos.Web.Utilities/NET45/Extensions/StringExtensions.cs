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

namespace Cobos.Web.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Extensions for the string class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// deserialize from JSON
        /// </summary>
        /// <param name="self">The 'this' object reference</param>
        /// <returns>Deserialized object</returns>
        public static object FromJson(this string self)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.DeserializeObject(self);
        }

        /// <summary>
        /// Deserialize from JSON as a specified type
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="self">The 'this' object reference</param>
        /// <param name="example">An instance of an anonymous type</param>
        /// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
        public static T FromJsonAs<T>(this string self, T example)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            object json = serializer.DeserializeObject(self);

            if (json is Dictionary<string, object>)
            {
                return ((Dictionary<string, object>)json).CastByExample(example);
            }
            else
            {
                // this may or may not work depending on the object type
                return json.CastByExample(example);
            }
        }
    }
}
