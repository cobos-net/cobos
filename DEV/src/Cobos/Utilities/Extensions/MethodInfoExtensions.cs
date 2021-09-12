// ----------------------------------------------------------------------------
// <copyright file="MethodInfoExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
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
        /// <param name="arguments">An array of name, value pairs of arguments.</param>
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
