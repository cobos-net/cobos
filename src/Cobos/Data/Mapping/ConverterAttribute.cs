// <copyright file="ConverterAttribute.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>

namespace Cobos.Data.Mapping
{
    using System;

    /// <summary>
    /// Indicates that a property has a value converter.
    /// </summary>
    public class ConverterAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type that implements <see cref="IValueConverter"/>.
        /// </summary>
        public Type Converter { get; set; }

        /// <summary>
        /// Gets or sets the converter type.
        /// </summary>
        public Type ConverterTargetType { get; set; }

        /// <summary>
        /// Gets or sets the converter parameter.
        /// </summary>
        public string ConverterParameter { get; set; }
    }
}
