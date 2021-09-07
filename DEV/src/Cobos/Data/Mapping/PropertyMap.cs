// ----------------------------------------------------------------------------
// <copyright file="PropertyMap.cs" company="Cobos SDK">
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
                    path = path.Substring(index + 1);
                    
                    return this.children[propertyName][path];
                }

                if (this.properties.TryGetValue(path, out var property) == true)
                {
                    return property;
                }

                return null;
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
