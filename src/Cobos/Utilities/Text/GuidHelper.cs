// ----------------------------------------------------------------------------
// <copyright file="GuidHelper.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Text
{
    using System;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Helper class for returning a normalized GUID.
    /// </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// Returns a normalized GUID in upper case.
        /// </summary>
        /// <returns>The GUID value.</returns>
        public static string GUID()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        /// <summary>
        /// Returns a normalized GUID in quotes if required.
        /// </summary>
        /// <param name="quote">Indicates whether the returned GUID should be enclosed in quotation marks.</param>
        /// <returns>The GUID value.</returns>
        public static string GUID(bool quote)
        {
            return quote ? GuidHelper.GUID().Quote() : GuidHelper.GUID();
        }
    }
}
