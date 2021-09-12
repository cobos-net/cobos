// ----------------------------------------------------------------------------
// <copyright file="ColumnAttribute.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Mapping
{
    using System;

    /// <summary>
    /// Specifies the source database column for a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the database column.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column is a primary key.
        /// </summary>
        public bool IsPrimaryKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column is unique.
        /// </summary>
        public bool IsUnique
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column is a foreign key.
        /// </summary>
        public bool IsForeignKey
        {
            get;
            set;
        }
    }
}
