// <copyright file="IValueConverter.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>

namespace Cobos.Data.Mapping
{
    /// <summary>
    /// Modifies data between the model and data layers.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target data object.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target data object property.</returns>
        object Convert(object value, System.Type targetType, object parameter);

        /// <summary>
        /// Modifies the target data before passing it to the source data.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the source object.</returns>
        object ConvertBack(object value, System.Type targetType, object parameter);
    }
}
