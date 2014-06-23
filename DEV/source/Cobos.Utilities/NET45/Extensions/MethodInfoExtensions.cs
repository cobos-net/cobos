// ----------------------------------------------------------------------------
// <copyright file="MethodInfoExtensions.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods for the <see cref="MethodInfo"/> class.
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Get the arguments for a method.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="arguments">An array of name, value pairs of arguments</param>
        /// <returns>The arguments.</returns>
        public static object[] GetMethodArguments(this MethodInfo self, KeyValuePair<string, object>[] arguments)
        {
            // Check the parameters match the supplied arguments
            if (arguments == null || arguments.Length == 0)
            {
                return null;
            }

            ParameterInfo[] paramInfos = self.GetParameters();

            if (paramInfos == null || paramInfos.Length == 0)
            {
                return null;
            }

            // Parse all of the arguments into the correct types
            object[] parameters = new object[paramInfos.Length];

            for (int i = 0; i < paramInfos.Length; ++i)
            {
                ParameterInfo paramInfo = paramInfos[i];
                Type paramType = paramInfo.ParameterType;

                var value = from a in arguments
                            where string.Compare(a.Key, paramInfo.Name, true) == 0
                            select a.Value;

                if (value != null)
                {
                    parameters[i] = Convert.ChangeType(value, paramType);
                }
                else
                {
                    parameters[i] = paramType.GetDefaultValue();
                }
            }

            return parameters;
        }
    }
}
