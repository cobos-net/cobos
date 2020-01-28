// ----------------------------------------------------------------------------
// <copyright file="SqlSortVisitor.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Statements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cobos.Data.Filter;
    using Cobos.Data.Mapping;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Class specification and implementation of <see cref="SqlSortByVisitor"/>.
    /// </summary>
    /// <typeparam name="T">The entity type to map to.</typeparam>
    public class SqlSortVisitor<T> : ISortVisitor
    {
        /// <summary>
        /// The internal buffer.
        /// </summary>
        private List<string> columns;

        /// <summary>
        /// The property map to map properties to columns.
        /// </summary>
        private PropertyMap map;

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
