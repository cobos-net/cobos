// ----------------------------------------------------------------------------
// <copyright file="IDictionaryExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="System.Collections.IDictionary"/> interface.
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Extend the dictionary class to support casting to an anonymous type.
        /// </summary>
        /// <typeparam name="T">The anonymous type to cast to.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="example">An instance of an anonymous type.</param>
        /// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
        public static T CastByExample<T>(this IDictionary<string, object> self, T example)
        {
            // get the sole constructor
            var ctor = example.GetType().GetConstructors().Single();

            // conveniently named constructor parameters make this all possible...
            var args = from p in ctor.GetParameters()
                       let val = self.GetValueOrDefault(p.Name)
                       select val != null && p.ParameterType.IsAssignableFrom(val.GetType()) ? (object)val : null;

            return (T)ctor.Invoke(args.ToArray());
        }

        /// <summary>
        /// Helper method to extract values from a dictionary to support the CastByExample method.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="key">The key of the value.</param>
        /// <returns>The value extracted.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
        {
            self.TryGetValue(key, out TValue result);
            return result;
        }
    }
}
