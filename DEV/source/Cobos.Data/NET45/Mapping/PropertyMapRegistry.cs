// ----------------------------------------------------------------------------
// <copyright file="PropertyMapRegistry.cs" company="Cobos SDK">
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

namespace Cobos.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if NET35
    using Cobos.Utilities.Wrappers;
#endif

    /// <summary>
    /// Class specification and implementation of <see cref="PropertyMapRegistry"/>.
    /// </summary>
    public sealed class PropertyMapRegistry
    {
        /// <summary>
        /// The singleton instance of the class.
        /// </summary>
        private static Lazy<PropertyMapRegistry> instance = new Lazy<PropertyMapRegistry>(() => new PropertyMapRegistry());

        /// <summary>
        /// The registry instance.
        /// </summary>
        private Dictionary<Type, PropertyMap> registry = new Dictionary<Type, PropertyMap>();

        /// <summary>
        /// Synchronize access to the registry.
        /// </summary>
        private object registryLock = new object();

        /// <summary>
        /// Prevents a default instance of the <see cref="PropertyMapRegistry"/> class from being created.
        /// </summary>
        private PropertyMapRegistry()
        {
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static PropertyMapRegistry Instance
        {
            get
            {
                return PropertyMapRegistry.instance.Value;
            }
        }

        /// <summary>
        /// Gets the property map for the registered type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The registered property map.</returns>
        public PropertyMap this[Type type]
        {
            get
            {
                lock (this.registryLock)
                {
                    this.RegisterTypeInternal(type);

                    return this.registry[type];
                }
            }
        }

        /// <summary>
        /// Register the type.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <exception cref="InvalidOperationException">Throws if the type is already registered.</exception>
        public void RegisterType(Type type)
        {
            lock (this.registryLock)
            {
                this.RegisterTypeInternal(type);
            }
        }

        /// <summary>
        /// Register the type.
        /// </summary>
        /// <param name="type">The type to register.</param>
        private void RegisterTypeInternal(Type type)
        {
            if (this.registry.ContainsKey(type) == true)
            {
                return;
            }

            this.registry[type] = PropertyMap.FromType(type);

            foreach (var nested in type.GetNestedTypes())
            {
                this.RegisterTypeInternal(nested);
            }
        }
    }
}
