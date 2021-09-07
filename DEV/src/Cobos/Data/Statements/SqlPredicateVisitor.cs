// ----------------------------------------------------------------------------
// <copyright file="SqlPredicateVisitor.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
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
    using Cobos.Data.Statements;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Class specification and implementation of <see cref="SqlPredicateVisitor"/>.
    /// </summary>
    /// <typeparam name="T">The entity type to map to.</typeparam>
    public class SqlPredicateVisitor<T> : IFilterPredicateVisitor
    {
        /// <summary>
        /// The internal buffer.
        /// </summary>
        private StringBuilder buffer;

        /// <summary>
        /// The property map to map properties to columns.
        /// </summary>
        private PropertyMap map;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlPredicateVisitor{T}"/> class.
        /// </summary>
        public SqlPredicateVisitor()
        {
            this.buffer = new StringBuilder(4096);
            this.map = PropertyMapRegistry.Instance[typeof(T)];
        }

        /// <summary>
        /// Process a string representation of a filter to a SQL clause.
        /// </summary>
        /// <param name="filter">The string representation of the filter.</param>
        /// <returns>The SQL clause.</returns>
        public static string FilterToSql(string filter)
        {
            if (string.IsNullOrEmpty(filter) == true)
            {
                return null;
            }

            return FilterToSql(Filter.Deserialize(filter));
        }

        /// <summary>
        /// Process a filter to a SQL clause.
        /// </summary>
        /// <param name="filter">The string representation of the filter.</param>
        /// <returns>The SQL clause.</returns>
        public static string FilterToSql(Filter filter)
        {
            var visitor = new SqlPredicateVisitor<T>();
            filter.Predicate.Accept(visitor);
            return visitor.ToString();
        }

        /// <summary>
        /// Gets the string representation of the predicate.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.buffer.ToString();
        }
        
        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(And predicate)
        {
            this.buffer.Append('(');

            for (int i = 0; i < predicate.Predicate.Count; ++i)
            {
                if (i > 0)
                {
                    this.buffer.Append(" AND ");
                }

                predicate.Predicate[i].Accept(this);
            }
            
            this.buffer.Append(')');
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(Or predicate)
        {
            this.buffer.Append("(");

            for (int i = 0; i < predicate.Predicate.Count; ++i)
            {
                if (i > 0)
                {
                    this.buffer.Append(" OR ");
                }

                predicate.Predicate[i].Accept(this);
            }

            this.buffer.Append(")");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(Not predicate)
        {
            this.buffer.Append("NOT (");
            predicate.Predicate.Accept(this);
            this.buffer.Append(")");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsBetween predicate)
        {
            var property = this.map[predicate.ValueReference];
            var lower = this.Value(property, predicate.LowerBoundary.Literal);
            var upper = this.Value(property, predicate.UpperBoundary.Literal);

            this.buffer.Append("(");
            this.buffer.Append(property.ToString() + " > " + lower);
            this.buffer.Append(" AND ");
            this.buffer.Append(property.ToString() + " < " + upper);
            this.buffer.Append(")");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsEqualTo predicate)
        {
            this.Comparison(predicate, " = ");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsGreaterThan predicate)
        {
            this.Comparison(predicate, " > ");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsGreaterThanOrEqualTo predicate)
        {
            this.Comparison(predicate, " >= ");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsLessThan predicate)
        {
            this.Comparison(predicate, " < ");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsLessThanOrEqualTo predicate)
        {
            this.Comparison(predicate, " <= ");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsLike predicate)
        {
            var property = this.map[predicate.ValueReference];

            this.buffer.Append(property.ToString() + " LIKE " + this.Value(property, predicate.Literal));
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsNotEqualTo predicate)
        {
            this.Comparison(predicate, " != ");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsNull predicate)
        {
            this.buffer.Append(this.map[predicate.ValueReference].ToString() + " IS NULL"); 
        }

        /// <summary>
        /// Format a comparison as a string.
        /// </summary>
        /// <param name="comparison">The binary comparison.</param>
        /// <param name="op">The operator.</param>
        private void Comparison(BinaryComparisonOp comparison, string op)
        {
            var property = this.map[comparison.ValueReference];

            this.buffer.Append(property.ToString() + op + this.Value(property, comparison.Literal));
        }

        /// <summary>
        /// Format a property value as a string.
        /// </summary>
        /// <param name="property">The property descriptor.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The value formatted as a string.</returns>
        private string Value(PropertyDescriptor property, string value)
        {
            if (value == null)
            {
                return "NULL";
            }

            var type = property.Property.PropertyType;

            if (type == typeof(string))
            {
                return value.SQLQuote();
            }

            return value;
        }
    }
}
