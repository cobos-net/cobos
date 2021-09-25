// <copyright file="ByteToBoolean.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>

namespace Cobos.Data.Tests.Mapping
{
    using System;
    using Cobos.Data.Mapping;

    /// <summary>
    /// Convert an integer value to a boolean.
    /// </summary>
    public class ByteToBoolean : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter) => (byte)value != 0;

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter) => (byte)((bool)value ? 1 : 0);
    }
}
