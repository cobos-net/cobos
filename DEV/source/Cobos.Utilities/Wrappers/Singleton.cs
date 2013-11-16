// ----------------------------------------------------------------------------
// <copyright file="Singleton.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Wrappers
{
    using System;
    using System.Reflection;
#if NET_35
    using Cobos.Utilities.Wrappers;
#endif

    /// <summary>
    /// Provides a singleton implementation, lazy loaded and thread-safe.
    /// </summary>
    /// <remarks>
    /// A private or protected constructor must be implemented in the T class
    /// </remarks>
    /// <typeparam name="T">The type of singleton instance to create.</typeparam>
    public sealed class Singleton<T> where T : class
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
            ConstructorInfo constructor = null;

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
