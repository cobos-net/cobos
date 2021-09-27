// ----------------------------------------------------------------------------
// <copyright file="PropertyMap.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Interface specification and implementation of the <see cref="PropertyMap"/> class.
    /// </summary>
    public class PropertyMap
    {
        /// <summary>
        /// Represents the properties that are value types.
        /// </summary>
        private readonly Dictionary<string, PropertyDescriptor> properties;

        /// <summary>
        /// Represents properties that are reference types.
        /// </summary>
        private readonly Dictionary<string, PropertyMap> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap"/> class.
        /// </summary>
        /// <param name="type">The type to map.</param>
        public PropertyMap(Type type)
        {
            this.children = new Dictionary<string, PropertyMap>(StringComparer.CurrentCultureIgnoreCase);
            this.properties = new Dictionary<string, PropertyDescriptor>(StringComparer.CurrentCultureIgnoreCase);
            this.MappingType = type;
        }

        /// <summary>
        /// Gets the mapping type for the property map.
        /// </summary>
        public Type MappingType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the property descriptor for the named property.
        /// </summary>
        /// <param name="path">The navigation path to the property.</param>
        /// <returns>The property descriptor for the named property.</returns>
        public PropertyDescriptor this[string path]
        {
            get
            {
                var index = path.IndexOf('.');

                if (index != -1)
                {
                    var propertyName = path.Substring(0, index);
                    var childPath = path.Substring(index + 1);

                    if (this.children.TryGetValue(propertyName, out var child))
                    {
                        return child[childPath];
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("path", $"Invalid property name ({path})");
                    }
                }

                if (this.properties.TryGetValue(path, out var property) == true)
                {
                    return property;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("path", $"Invalid property name ({path})");
                }
            }

            set
            {
                this.properties[path] = value;
            }
        }

        /// <summary>
        /// Create a property map from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The property map for the type.</returns>
        /// <exception cref="ArgumentException">Throws if the type does not contain a TableAttribute declaration.</exception>
        public static PropertyMap FromType(Type type)
        {
            var tableName = type.Name;

#if NET35
            var table = type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
#else
            var table = type.GetCustomAttribute<TableAttribute>();
#endif

            if (table != null)
            {
                tableName = table.Name;
            }

            var map = new PropertyMap(type);

            foreach (var property in type.GetProperties())
            {
                var propertyType = property.PropertyType;

                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    if (propertyType.GetInterface("IEnumerable") != null)
                    {
                        continue;
                    }

                    if (type == property.PropertyType)
                    {
                        map.children[property.Name] = map;
                    }
                    else
                    {
                        map.children[property.Name] = PropertyMapRegistry.Instance[property.PropertyType];
                    }
                }
                else
                {
                    var descriptor = PropertyDescriptor.FromProperty(property, tableName);

                    if (descriptor != null)
                    {
                        map[property.Name] = descriptor;
                    }
                }
            }

            return map;
        }
    }
}
