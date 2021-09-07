// ----------------------------------------------------------------------------
// <copyright file="PropertyDescriptor.cs" company="Cobos SDK">
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
    /// Class specification and implementation of <see cref="PropertyDescriptor"/>.
    /// </summary>
    public class PropertyDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDescriptor"/> class.
        /// </summary>
        /// <param name="property">The property for the descriptor.</param>
        /// <param name="table">The table name.</param>
        /// <param name="column">The column name.</param>
        public PropertyDescriptor(PropertyInfo property, string table, string column)
        {
            this.Property = property;
            this.Table = table;
            this.Column = column;
        }

        /// <summary>
        /// Gets the property info for this descriptor.
        /// </summary>
        public PropertyInfo Property
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Table
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string Column
        {
            get;
            private set;
        }

        /// <summary>
        /// Create descriptor from a property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="tableName">The table name of the descriptor.  May be overridden on the property.</param>
        /// <returns>A property descriptor representing the property.</returns>
        public static PropertyDescriptor FromProperty(PropertyInfo property, string tableName)
        {
#if NET35
            var table = property.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
#else
            var table = property.GetCustomAttribute<TableAttribute>();
#endif
            if (table != null)
            {
                tableName = table.Name;
            }

            var columnName = property.Name;

#if NET35
            var column = property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;
#else
            var column = property.GetCustomAttribute<ColumnAttribute>();
#endif

            if (column != null)
            {
                columnName = column.Name;
            }

            return new PropertyDescriptor(property, tableName, columnName);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.Table + "." + this.Column;
        }
    }
}
