// ----------------------------------------------------------------------------
// <copyright file="Singleton.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Wrappers
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides a singleton implementation, lazy loaded and thread-safe.
    /// </summary>
    /// <remarks>
    /// A private or protected constructor must be implemented in the T class.
    /// </remarks>
    /// <typeparam name="T">The type of singleton instance to create.</typeparam>
    public sealed class Singleton<T>
        where T : class
    {
        /// <summary>
        /// The singleton instance. Lazy loaded and thread-safe.
        /// </summary>
        private static readonly Lazy<T> TheInstance = new Lazy<T>(CreateInstance);

        /// <summary>
        /// Prevents a default instance of the <see cref="Singleton{T}" /> class from being created.
        /// </summary>
        private Singleton()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the repository.
        /// </summary>
        public static T Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return TheInstance.Value; }
        }

        /// <summary>
        /// Delegate method to create the instance for the Lazy instantiation.
        /// </summary>
        /// <returns>An instance of T.</returns>
        private static T CreateInstance()
        {
            ConstructorInfo constructor;
            try
            {
                // Binding flags exclude public constructors.
                constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
            }
            catch (Exception exception)
            {
                throw new SingletonException(exception);
            }

            if (constructor == null || constructor.IsAssembly)
            {
                // Also exclude internal constructors.
                throw new SingletonException(string.Format("A private or protected constructor is missing for '{0}'.", typeof(T).Name));
            }

            return (T)constructor.Invoke(null);
        }
    }
}
