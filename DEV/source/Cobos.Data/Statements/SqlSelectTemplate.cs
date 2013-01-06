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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Data.Statements
{
    /// <summary>
    /// Simple select constructor, allows basic definition of
    /// a select statement with the option of adding additional
    /// clauses when constructing the string representation.
    /// </summary>
    public class SqlSelectTemplate
    {
        /// <summary>
        /// Immutable internal storage for statement clauses
        /// </summary>
        readonly string _select;
        readonly string _from;
        readonly string[] _innerJoin;
        readonly string[] _where;
        readonly string _groupBy;
        readonly string _orderBy;
        readonly string _buffered;

        public SqlSelectTemplate()
        {
        }

        public SqlSelectTemplate(string select, string from, string[] innerJoin,
                                string[] where, string groupBy, string orderBy)
            : this(select, from, innerJoin, where, groupBy, orderBy, false)
        {
        }

        public SqlSelectTemplate(string select, string from, string[] innerJoin,
                                string[] where, string groupBy, string orderBy, bool buffered)
        {
            _select = select;
            _from = from;
            _innerJoin = innerJoin;
            _where = where;
            _groupBy = groupBy;
            _orderBy = orderBy;

            if (buffered)
            {
                _buffered = ToStringInternal();
            }
        }

        /// <summary>
        /// Get the string representation of the query.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_buffered != null)
            {
                return _buffered;
            }
            else
            {
                return ToStringInternal();
            }
        }

        /// <summary>
        /// Internal method to construct the select statement.
        /// </summary>
        /// <returns></returns>
        string ToStringInternal()
        {
            StringBuilder buffer = new StringBuilder(512);

            // detect the most obvious error case
            if (string.IsNullOrEmpty(_select))
            {
                throw new InvalidOperationException("SqlSelect.ToString: The select clause is empty");
            }

            buffer.Append("SELECT " + _select);

            if (!string.IsNullOrEmpty(_from))
            {
                buffer.Append(" FROM " + _from);
            }

            if (_innerJoin != null && _innerJoin.Length > 0)
            {
                for (int i = 0; i < _innerJoin.Length; ++i)
                {
                    buffer.Append(" INNER JOIN " + _innerJoin[i]);
                }
            }

            if (_where != null && _where.Length > 0)
            {
                buffer.Append(" WHERE (" + _where[0] + ")");

                for (int i = 1; i < _where.Length; ++i)
                {
                    buffer.Append(" AND (" + _where[i] + ")");
                }
            }

            if (!string.IsNullOrEmpty(_groupBy))
            {
                buffer.Append(" GROUP BY " + _groupBy);
            }

            if (!string.IsNullOrEmpty(_orderBy))
            {
                buffer.Append(" ORDER BY " + _orderBy);
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Fully augment the string before returning
        /// </summary>
        /// <param name="select"></param>
        /// <param name="from"></param>
        /// <param name="innerJoin"></param>
        /// <param name="where"></param>
        /// <param name="groupBy"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public string ToString(string select, string from, string[] innerJoin,
                                        string[] where, string groupBy, string orderBy)
        {
            StringBuilder buffer = new StringBuilder(512);

            // detect the most obvious error case
            if (string.IsNullOrEmpty(_select) && string.IsNullOrEmpty(select))
            {
                throw new InvalidOperationException("SqlSelect.ToString: The select clause is empty");
            }

            bool existsSelect = !string.IsNullOrEmpty(_select);

            if (existsSelect)
            {
                buffer.Append("SELECT " + _select);
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

            bool existsFrom = !string.IsNullOrEmpty(_from);

            if (existsFrom)
            {
                buffer.Append(" FROM " + _from);
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

            if (_innerJoin != null && _innerJoin.Length > 0)
            {
                for (int i = 0; i < _innerJoin.Length; ++i)
                {
                    buffer.Append(" INNER JOIN " + _innerJoin[i]);
                }
            }

            if (innerJoin != null && innerJoin.Length > 0)
            {
                for (int i = 0; i < innerJoin.Length; ++i)
                {
                    buffer.Append(" INNER JOIN " + innerJoin[i]);
                }
            }

            bool existsWhere = _where != null && _where.Length > 0;

            if (existsWhere)
            {
                buffer.Append(" WHERE (" + _where[0] + ")");

                for (int i = 1; i < _where.Length; ++i)
                {
                    buffer.Append(" AND (" + _where[i] + ")");
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

            bool existsGroupBy = !string.IsNullOrEmpty(_groupBy);

            if (existsGroupBy)
            {
                buffer.Append(" GROUP BY " + _groupBy);
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

            bool existsOrderBy = !string.IsNullOrEmpty(_orderBy);

            if (existsOrderBy)
            {
                buffer.Append(" ORDER BY " + _orderBy);
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
        /// Augment the statement with the most common clauses that are usually needed
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public string ToString(string[] where, string orderBy)
        {
            if ((where == null || where.Length == 0) && orderBy == null)
            {
                return ToString(); // return the buffered result if available
            }
            else
            {
                return ToString(null, null, null, where, null, orderBy);
            }
        }

        /// <summary>
        /// Augment the statement with the most common clauses that are usually needed
        /// </summary>
        /// <param name="where"></param>
        /// <param name="groupBy"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public string ToString(string[] where, string groupBy, string orderBy)
        {
            if ((where == null || where.Length == 0) && groupBy == null && orderBy == null)
            {
                return ToString(); // return the buffered result if available
            }
            else
            {
                return ToString(null, null, null, where, groupBy, orderBy);
            }
        }

        /// <summary>
        /// Simple augmentation of just a where clause
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public string ToString(string[] where)
        {
            if (where != null && where.Length != 0)
            {
                return ToString(null, null, null, where, null, null);
            }
            else
            {
                return ToString(); // return the buffered result if available
            }
        }

    }
}
