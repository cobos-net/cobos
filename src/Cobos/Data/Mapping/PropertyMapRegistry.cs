// ----------------------------------------------------------------------------
// <copyright file="PropertyMapRegistry.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Mapping
{
    using System;
    using System.Collections.Generic;
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
        private static readonly Lazy<PropertyMapRegistry> InstanceValue = new Lazy<PropertyMapRegistry>(() => new PropertyMapRegistry());

        /// <summary>
        /// The registry instance.
        /// </summary>
        private readonly Dictionary<Type, PropertyMap> registry = new Dictionary<Type, PropertyMap>();

        /// <summary>
        /// Synchronize access to the registry.
        /// </summary>
        private readonly object registryLock = new object();

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
                return PropertyMapRegistry.InstanceValue.Value;
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
