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
        public Type ConverterType { get; set; }

        /// <summary>
        /// Gets or sets the converter target type.
        /// </summary>
        public Type SourceType { get; set; }

        /// <summary>
        /// Gets or sets the converter target type.
        /// </summary>
        public Type TargetType { get; set; }

        /// <summary>
        /// Gets or sets the converter parameter.
        /// </summary>
        public string Parameter { get; set; }
    }
}
