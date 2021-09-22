// ----------------------------------------------------------------------------
// <copyright file="SqlPredicateVisitor.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Statements
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Cobos.Data.Filter;
    using Cobos.Data.Mapping;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Class specification and implementation of <see cref="SqlPredicateVisitor{T}"/>.
    /// </summary>
    /// <typeparam name="T">The entity type to map to.</typeparam>
    public class SqlPredicateVisitor<T> : IFilterPredicateVisitor, IFilterExpressionVisitor
    {
        /// <summary>
        /// The internal buffer.
        /// </summary>
        private readonly StringBuilder buffer;

        /// <summary>
        /// The property map to map properties to columns.
        /// </summary>
        private readonly PropertyMap map;

        /// <summary>
        /// Track context for evaluating expressions.
        /// </summary>
        private readonly Stack<ComparisonOps> expressionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlPredicateVisitor{T}"/> class.
        /// </summary>
        public SqlPredicateVisitor()
        {
            this.buffer = new StringBuilder(4096);
            this.map = PropertyMapRegistry.Instance[typeof(T)];
            this.expressionContext = new Stack<ComparisonOps>();
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
        /// Visit the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void Visit(PropertyName expression)
        {
            var property = this.map[expression.Value];

            if (property == null)
            {
                throw new System.InvalidOperationException($"Invalid property name '{expression.Value}' for {this.map.MappingType.Name}.");
            }

            this.buffer.Append(property.ToString());
        }

        /// <summary>
        /// Visit the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void Visit(Literal expression)
        {
            var context = this.expressionContext.Peek();

            if (string.IsNullOrEmpty(expression?.Value?.Trim()))
            {
                this.buffer.Append("NULL");
                return;
            }

            if (context is BinaryComparisonOp comparison)
            {
                this.Evaluate(expression, comparison);
            }
            else if (context is PropertyIsBetween between)
            {
                this.Evaluate(expression, between);
            }
            else if (context is PropertyIsLike like)
            {
                this.Evaluate(expression, like);
            }
            else
            {
                Debug.Assert(false, $"Unsupported literal context: {context?.GetType().Name ?? "null reference"}");
            }
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(And predicate)
        {
            this.buffer.Append('(');
            predicate.Condition1.Accept(this);
            this.buffer.Append(" AND ");
            predicate.Condition2.Accept(this);
            this.buffer.Append(')');
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(Or predicate)
        {
            this.buffer.Append('(');
            predicate.Condition1.Accept(this);
            this.buffer.Append(" OR ");
            predicate.Condition2.Accept(this);
            this.buffer.Append(')');
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(Not predicate)
        {
            this.buffer.Append($"NOT (");
            predicate.Condition.Accept(this);
            this.buffer.Append(")");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsBetween predicate)
        {
            if (predicate.Expression is PropertyName == false)
            {
                throw new System.NotSupportedException($"Cannot evaluate a {nameof(PropertyIsBetween)} comparison where the expression is not a {nameof(PropertyName)}");
            }

            this.expressionContext.Push(predicate);

            this.buffer.Append("(");
            predicate.Expression.Accept(this);
            this.buffer.Append(" >= ");
            predicate.LowerBoundary.Expression.Accept(this);
            this.buffer.Append(" AND ");
            predicate.Expression.Accept(this);
            this.buffer.Append(" <= ");
            predicate.UpperBoundary.Expression.Accept(this);
            this.buffer.Append(")");

            this.expressionContext.Pop();
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsEqualTo predicate)
        {
            this.Comparison(predicate, "=");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsGreaterThan predicate)
        {
            this.Comparison(predicate, ">");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsGreaterThanOrEqualTo predicate)
        {
            this.Comparison(predicate, ">=");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsLessThan predicate)
        {
            this.Comparison(predicate, "<");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsLessThanOrEqualTo predicate)
        {
            this.Comparison(predicate, "<=");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsLike predicate)
        {
            this.expressionContext.Push(predicate);

            predicate.PropertyName.Accept(this);
            this.buffer.Append($" LIKE ");
            predicate.Literal.Accept(this);

            this.expressionContext.Pop();
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsNotEqualTo predicate)
        {
            this.Comparison(predicate, "!=");
        }

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Visit(PropertyIsNull predicate)
        {
            this.buffer.Append(this.map[predicate.PropertyName.Value].ToString() + " IS NULL");
        }

        /// <summary>
        /// Format a comparison as a string.
        /// </summary>
        /// <param name="comparison">The binary comparison.</param>
        /// <param name="op">The operator.</param>
        private void Comparison(BinaryComparisonOp comparison, string op)
        {
            this.expressionContext.Push(comparison);

            comparison.Left.Accept(this);
            this.buffer.Append($" {op} ");
            comparison.Right.Accept(this);

            this.expressionContext.Pop();
        }

        private void Evaluate(Literal literal, BinaryComparisonOp context)
        {
            var isLhs = object.ReferenceEquals(context.Left, literal);
            var isRhs = object.ReferenceEquals(context.Right, literal);

            Debug.Assert(isLhs || isRhs, "Invalid expression context");

            var other = isLhs ? context.Right : context.Left;

            if (other is PropertyName propertyName)
            {
                this.Evaluate(literal, propertyName);
            }
            else
            {
                this.buffer.Append(literal.Value);
            }
        }

        private void Evaluate(Literal literal, PropertyIsLike context)
        {
            this.Evaluate(literal, context.PropertyName);
        }

        private void Evaluate(Literal literal, PropertyIsBetween context)
        {
            if (object.ReferenceEquals(literal, context.Expression))
            {
                Debug.Assert(false, $"Cannot evaluate a {nameof(PropertyIsBetween)} comparison where the expression is not a {nameof(PropertyName)}");
                this.buffer.Append(literal.Value);
                return;
            }

            if (context.Expression is PropertyName propertyName)
            {
                this.Evaluate(literal, propertyName);
            }
            else
            {
                throw new System.NotSupportedException($"Cannot evaluate a {nameof(PropertyIsBetween)} comparison where the expression is not a {nameof(PropertyName)}");
            }
        }

        private void Evaluate(Literal literal, PropertyName propertyName)
        {
            var property = this.map[propertyName.Value];

            if (property == null)
            {
                throw new System.InvalidOperationException($"Invalid property name '{literal.Value}' for {this.map.MappingType.Name}.");
            }

            if (property.Property.PropertyType == typeof(string))
            {
                this.buffer.Append(literal.Value.SQLQuote());
            }
            else
            {
                this.buffer.Append(literal.Value);
            }
        }
    }
}
