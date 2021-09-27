// ----------------------------------------------------------------------------
// <copyright file="PropertyDescriptor.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Mapping
{
    using System;
    using System.Reflection;
    using Cobos.Utilities.Extensions;

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
        /// <param name="converter">The converter information.</param>
        public PropertyDescriptor(PropertyInfo property, string table, string column, ConverterAttribute converter)
        {
            this.Property = property;
            this.Table = table;
            this.Column = column;
            this.Converter = converter;
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Table { get; }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string Column { get; }

        /// <summary>
        /// Gets a value indicating whether the property type is a string.
        /// </summary>
        public bool IsStringType => this.DatabaseType == typeof(string);

        /// <summary>
        /// Gets the Data Object property type.
        /// </summary>
        public Type PropertyType => this.Converter?.TargetType ?? this.Property.PropertyType;

        /// <summary>
        /// Gets the underlying database type.
        /// </summary>
        public Type DatabaseType => this.Converter?.SourceType ?? this.Property.PropertyType;

        /// <summary>
        /// Gets the property info for this descriptor.
        /// </summary>
        private PropertyInfo Property { get; }

        /// <summary>
        /// Gets the value converter.
        /// </summary>
        private ConverterAttribute Converter { get; }

        /// <summary>
        /// Create descriptor from a property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="tableName">The table name of the descriptor.  May be overridden on the property.</param>
        /// <returns>A property descriptor representing the property.</returns>
        public static PropertyDescriptor FromProperty(PropertyInfo property, string tableName)
        {
            var table = property.GetCustomAttribute<TableAttribute>();

            if (table != null)
            {
                tableName = table.Name;
            }

            var columnName = property.Name;

            var column = property.GetCustomAttribute<ColumnAttribute>();

            if (column != null)
            {
                columnName = column.Name;
            }

            return new PropertyDescriptor(property, tableName, columnName, property.GetCustomAttribute<ConverterAttribute>());
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.Table + "." + this.Column;
        }

        /// <summary>
        /// Convert the property value to SQL.
        /// </summary>
        /// <param name="value">The property value.</param>
        /// <returns>The converted value.</returns>
        public string ToSqlValue(object value)
        {
            if (this.Converter != null)
            {
                value = ((IValueConverter)Activator.CreateInstance(this.Converter.ConverterType)).ConvertBack(value, this.Converter.TargetType, this.Converter.Parameter);
            }

            if (value == null)
            {
                return "NULL";
            }

            if (this.IsStringType)
            {
                return value.ToString().SQLQuote();
            }

            return value.ToString();
        }
    }
}
