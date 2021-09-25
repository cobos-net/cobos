// <copyright file="StringUpperCase.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>

namespace Cobos.Data.Tests.Mapping
{
    using System;
    using Cobos.Data.Mapping;

    /// <summary>
    /// Convert a string to upper case.
    /// </summary>
    public class StringUpperCase : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter) => (value as string)?.ToUpper() ?? null;

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter) => value;
    }
}
