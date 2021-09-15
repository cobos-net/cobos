// ----------------------------------------------------------------------------
// <copyright file="SqlSortVisitor.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Statements
{
    using System.Collections.Generic;
    using Cobos.Data.Filter;
    using Cobos.Data.Mapping;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Class specification and implementation of <see cref="SqlSortVisitor{T}"/>.
    /// </summary>
    /// <typeparam name="T">The entity type to map to.</typeparam>
    public class SqlSortVisitor<T> : ISortVisitor
    {
        /// <summary>
        /// The internal buffer.
        /// </summary>
        private readonly List<string> columns;

        /// <summary>
        /// The property map to map properties to columns.
        /// </summary>
        private readonly PropertyMap map;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSortVisitor{T}"/> class.
        /// </summary>
        public SqlSortVisitor()
        {
            this.columns = new List<string>();
            this.map = PropertyMapRegistry.Instance[typeof(T)];
        }

        /// <summary>
        /// Process a string representation of a sort to a SQL clause.
        /// </summary>
        /// <param name="sort">The string representation of the sort.</param>
        /// <returns>The SQL clause.</returns>
        public static string SortToSql(string sort)
        {
            if (string.IsNullOrEmpty(sort) == true)
            {
                return null;
            }

            return SortToSql(SortBy.Deserialize(sort));
        }

        /// <summary>
        /// Process a sort to a SQL clause.
        /// </summary>
        /// <param name="sort">The string representation of the sort.</param>
        /// <returns>The SQL clause.</returns>
        public static string SortToSql(SortBy sort)
        {
            var visitor = new SqlSortVisitor<T>();
            sort.Accept(visitor);
            return visitor.ToString();
        }

        /// <summary>
        /// Gets the string representation of the predicate.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
#if NET35
            return string.Join(",", this.columns.ToArray());
#else
            return string.Join(",", this.columns);
#endif
        }

        /// <summary>
        /// Visit the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Visit(SortBy element)
        {
            foreach (var sort in element)
            {
                sort.Accept(this);
            }
        }

        /// <summary>
        /// Visit the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Visit(SortProperty element)
        {
            this.columns.Add(this.map[element.ValueReference].ToString() + " " + element.SortOrder.ToEnumString());
        }
    }
}
