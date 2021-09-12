// ----------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
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
