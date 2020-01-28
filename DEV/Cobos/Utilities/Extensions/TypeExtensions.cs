// ----------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Cobos SDK">
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
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The default value for the type.</returns>
        public static object GetDefaultValue(this Type self)
        {
            return self.IsValueType ? Activator.CreateInstance(self) : null;
        }

        /// <summary>
        /// Get the named method from the type.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="methodName">The method to find.</param>
        /// <returns>The named method if found; otherwise null.</returns>
        public static MethodInfo GetMethodCaseInsensitive(this Type self, string methodName)
        {
            return self.GetMethods().FirstOrDefault(m => string.Compare(m.Name, methodName, true) == 0);
        }
    }
}
