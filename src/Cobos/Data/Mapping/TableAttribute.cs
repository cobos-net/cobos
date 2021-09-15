// ----------------------------------------------------------------------------
// <copyright file="TableAttribute.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Mapping
{
    using System;

    /// <summary>
    /// Specifies the source database table for a class or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the database table.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
