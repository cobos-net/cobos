// ----------------------------------------------------------------------------
// <copyright file="SqlSelectTemplate.cs" company="Cobos SDK">
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
    using System.Text;

    /// <summary>
    /// Construct a SQL SELECT statement.
    /// </summary>
    /// <remarks>
    /// Simple select constructor, allows basic definition of
    /// a select statement with the option of adding additional
    /// clauses when constructing the string representation.
    /// </remarks>
    public class SqlSelectTemplate
    {
        /// <summary>
        /// The 'select' clause of the statement.
        /// </summary>
        private readonly string select;
        
        /// <summary>
        /// The 'from' clause of the statement.
        /// </summary>
        private readonly string from;

        /// <summary>
        /// The 'inner join' clause of the statement.
        /// </summary>
        private readonly string[] innerJoin;

        /// <summary>
        /// The 'outer join' clause of the statement.
        /// </summary>
        private readonly string[] outerJoin;

        /// <summary>
        /// The 'where' clause of the statement.
        /// </summary>
        private readonly string[] where;
        
        /// <summary>
        /// The 'group by' clause of the statement.
        /// </summary>
        private readonly string groupBy;
        
        /// <summary>
        /// The 'order by' clause of the statement.
        /// </summary>
        private readonly string orderBy;
        
        /// <summary>
        /// A buffered copy of the statement that can be returned quickly.
        /// </summary>
        private readonly string buffered;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelectTemplate"/> class.
        /// </summary>
        public SqlSelectTemplate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelectTemplate"/> class.
        /// </summary>
        /// <param name="select">The 'select' clause of the statement.</param>
        /// <param name="from">The 'from' clause of the statement</param>
        /// <param name="innerJoin">The 'inner join' clause of the statement</param>
        /// <param name="outerJoin">The 'outer join' clause of the statement</param>
        /// <param name="where">The 'where' clause of the statement</param>
        /// <param name="groupBy">The 'group by' clause of the statement</param>
        /// <param name="orderBy">The 'order by' clause of the statement</param>
        public SqlSelectTemplate(string select, string from, string[] innerJoin, string[] outerJoin, string[] where, string groupBy, string orderBy)
            : this(select, from, innerJoin, outerJoin, where, groupBy, orderBy, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelectTemplate"/> class.
        /// </summary>
        /// <param name="select">The 'select' clause of the statement.</param>
        /// <param name="from">The 'from' clause of the statement</param>
        /// <param name="innerJoin">The 'inner join' clause of the statement</param>
        /// <param name="outerJoin">The 'outer join' clause of the statement</param>
        /// <param name="where">The 'where' clause of the statement</param>
        /// <param name="groupBy">The 'group by' clause of the statement</param>
        /// <param name="orderBy">The 'order by' clause of the statement</param>
        /// <param name="buffered">Indicates whether the statement should be buffered.</param>
        public SqlSelectTemplate(string select, string from, string[] innerJoin, string[] outerJoin, string[] where, string groupBy, string orderBy, bool buffered)
        {
            this.select = select;
            this.from = from;
            this.innerJoin = innerJoin;
            this.outerJoin = outerJoin;
            this.where = where;
            this.groupBy = groupBy;
            this.orderBy = orderBy;

            if (buffered)
            {
                this.buffered = this.ToStringInternal();
            }
        }

        /// <summary>
        /// Get the string representation of the statement.
        /// </summary>
        /// <returns>A string representation of the statement.</returns>
        public override string ToString()
        {
            if (this.buffered != null)
            {
                return this.buffered;
            }
            else
            {
                return this.ToStringInternal();
            }
        }

        /// <summary>
        /// Augment the select statement with additional clauses.
        /// </summary>
        /// <param name="select">The 'select' clause of the statement.</param>
        /// <param name="from">The 'from' clause of the statement</param>
        /// <param name="innerJoin">The 'inner join' clause of the statement</param>
        /// <param name="outerJoin">The 'outer join' clause of the statement</param>
        /// <param name="where">The 'where' clause of the statement</param>
        /// <param name="groupBy">The 'group by' clause of the statement</param>
        /// <param name="orderBy">The 'order by' clause of the statement</param>
        /// <returns>The augmented select statement.</returns>
        public string ToString(string select, string from, string[] innerJoin, string[] outerJoin, string[] where, string groupBy, string orderBy)
        {
            StringBuilder buffer = new StringBuilder(512);

            // detect the most obvious error case
            if (string.IsNullOrEmpty(this.select) && string.IsNullOrEmpty(select))
            {
                throw new InvalidOperationException("SqlSelect.ToString: The select clause is empty");
            }

            bool existsSelect = !string.IsNullOrEmpty(this.select);

            if (existsSelect)
            {
                buffer.Append("SELECT " + this.select);
            }

            if (!string.IsNullOrEmpty(select))
            {
                if (!existsSelect)
                {
                    buffer.Append("SELECT " + select);
                }
                else
                {
                    buffer.Append(", " + select);
                }
            }

            bool existsFrom = !string.IsNullOrEmpty(this.from);

            if (existsFrom)
            {
                buffer.Append(" FROM " + this.from);
            }

            if (!string.IsNullOrEmpty(from))
            {
                if (!existsFrom)
                {
                    buffer.Append(" FROM " + from);
                }
                else
                {
                    buffer.Append(", " + from);
                }
            }

            if (this.innerJoin != null && this.innerJoin.Length > 0)
            {
                for (int i = 0; i < this.innerJoin.Length; ++i)
                {
                    buffer.Append(" INNER JOIN " + this.innerJoin[i]);
                }
            }

            if (innerJoin != null && innerJoin.Length > 0)
            {
                for (int i = 0; i < innerJoin.Length; ++i)
                {
                    buffer.Append(" INNER JOIN " + innerJoin[i]);
                }
            }

            if (this.outerJoin != null && this.outerJoin.Length > 0)
            {
                for (int i = 0; i < this.outerJoin.Length; ++i)
                {
                    buffer.Append(" OUTER JOIN " + this.outerJoin[i]);
                }
            }

            if (outerJoin != null && outerJoin.Length > 0)
            {
                for (int i = 0; i < outerJoin.Length; ++i)
                {
                    buffer.Append(" OUTER JOIN " + outerJoin[i]);
                }
            }

            bool existsWhere = this.where != null && this.where.Length > 0;

            if (existsWhere)
            {
                buffer.Append(" WHERE (" + this.where[0] + ")");

                for (int i = 1; i < this.where.Length; ++i)
                {
                    buffer.Append(" AND (" + this.where[i] + ")");
                }
            }

            if (where != null && where.Length > 0)
            {
                int i = 0;
                if (!existsWhere)
                {
                    buffer.Append(" WHERE (" + where[0] + ")");
                    i = 1;
                }

                for (; i < where.Length; ++i)
                {
                    buffer.Append(" AND (" + where[i] + ")");
                }
            }

            bool existsGroupBy = !string.IsNullOrEmpty(this.groupBy);

            if (existsGroupBy)
            {
                buffer.Append(" GROUP BY " + this.groupBy);
            }

            if (!string.IsNullOrEmpty(groupBy))
            {
                if (existsGroupBy)
                {
                    buffer.Append(", " + groupBy);
                }
                else
                {
                    buffer.Append(" GROUP BY " + groupBy);
                }
            }

            bool existsOrderBy = !string.IsNullOrEmpty(this.orderBy);

            if (existsOrderBy)
            {
                buffer.Append(" ORDER BY " + this.orderBy);
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                if (existsOrderBy)
                {
                    buffer.Append(", " + orderBy);
                }
                else
                {
                    buffer.Append(" ORDER BY " + orderBy);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Augment the statement with the common filtering clauses.
        /// </summary>
        /// <param name="where">The 'where' clause of the statement</param>
        /// <param name="orderBy">The 'order by' clause of the statement</param>
        /// <returns>A string representing the augmented statement.</returns>
        public string ToString(string[] where, string orderBy)
        {
            if ((where == null || where.Length == 0) && orderBy == null)
            {
                return this.ToString(); // return the buffered result if available
            }
            else
            {
                return this.ToString(null, null, null, null, where, null, orderBy);
            }
        }

        /// <summary>
        /// Augment the statement with the common filtering clauses.
        /// </summary>
        /// <param name="where">The 'where' clause of the statement</param>
        /// <param name="groupBy">The 'group by' clause of the statement</param>
        /// <param name="orderBy">The 'order by' clause of the statement</param>
        /// <returns>A string representing the augmented statement.</returns>
        public string ToString(string[] where, string groupBy, string orderBy)
        {
            if ((where == null || where.Length == 0) && groupBy == null && orderBy == null)
            {
                return this.ToString(); // return the buffered result if available
            }
            else
            {
                return this.ToString(null, null, null, null, where, groupBy, orderBy);
            }
        }

        /// <summary>
        /// Augment the statement with the a where clauses.
        /// </summary>
        /// <param name="where">The 'where' clause of the statement</param>
        /// <returns>A string representing the augmented statement.</returns>
        public string ToString(string[] where)
        {
            if (where != null && where.Length != 0)
            {
                return this.ToString(null, null, null, null, where, null, null);
            }
            else
            {
                return this.ToString(); // return the buffered result if available
            }
        }

        /// <summary>
        /// Internal method to construct the select statement.
        /// </summary>
        /// <returns>A string representation of the statement.</returns>
        private string ToStringInternal()
        {
            StringBuilder buffer = new StringBuilder(512);

            // detect the most obvious error case
            if (string.IsNullOrEmpty(this.select))
            {
                throw new InvalidOperationException("SqlSelect.ToString: The select clause is empty");
            }

            buffer.Append("SELECT " + this.select);

            if (!string.IsNullOrEmpty(this.from))
            {
                buffer.Append(" FROM " + this.from);
            }

            if (this.innerJoin != null && this.innerJoin.Length > 0)
            {
                for (int i = 0; i < this.innerJoin.Length; ++i)
                {
                    buffer.Append(" INNER JOIN " + this.innerJoin[i]);
                }
            }

            if (this.outerJoin != null && this.outerJoin.Length > 0)
            {
                for (int i = 0; i < this.outerJoin.Length; ++i)
                {
                    buffer.Append(" OUTER JOIN " + this.outerJoin[i]);
                }
            }

            if (this.where != null && this.where.Length > 0)
            {
                buffer.Append(" WHERE (" + this.where[0] + ")");

                for (int i = 1; i < this.where.Length; ++i)
                {
                    buffer.Append(" AND (" + this.where[i] + ")");
                }
            }

            if (!string.IsNullOrEmpty(this.groupBy))
            {
                buffer.Append(" GROUP BY " + this.groupBy);
            }

            if (!string.IsNullOrEmpty(this.orderBy))
            {
                buffer.Append(" ORDER BY " + this.orderBy);
            }

            return buffer.ToString();
        }
    }
}
