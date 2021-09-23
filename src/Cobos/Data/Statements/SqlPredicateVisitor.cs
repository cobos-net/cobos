// ----------------------------------------------------------------------------
// <copyright file="SqlPredicateVisitor.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Statements
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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
        private readonly Stack<ComparisonOps> operationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlPredicateVisitor{T}"/> class.
        /// </summary>
        public SqlPredicateVisitor()
        {
            this.buffer = new StringBuilder(4096);
            this.map = PropertyMapRegistry.Instance[typeof(T)];
            this.operationContext = new Stack<ComparisonOps>();
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
        public override string ToString() => this.buffer.ToString();

        /// <summary>
        /// Visit the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void Visit(PropertyName expression) => this.Evaluate(expression, this.operationContext.Peek());

        /// <summary>
        /// Visit the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void Visit(Literal expression)
        {
            if (string.IsNullOrEmpty(expression?.Value?.Trim()))
            {
                this.buffer.Append("NULL");
                return;
            }

            this.Evaluate(expression, this.operationContext.Peek());
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

            this.operationContext.Push(predicate);

            this.buffer.Append("(");
            predicate.Expression.Accept(this);
            this.buffer.Append(" >= ");
            predicate.LowerBoundary.Expression.Accept(this);
            this.buffer.Append(" AND ");
            predicate.Expression.Accept(this);
            this.buffer.Append(" <= ");
            predicate.UpperBoundary.Expression.Accept(this);
            this.buffer.Append(")");

            this.operationContext.Pop();
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
        public void Visit(PropertyIsInList predicate)
        {
            var property = this.map[predicate.PropertyName.Value];

            if (property == null)
            {
                throw new System.InvalidOperationException($"Invalid property name '{predicate.PropertyName.Value}' for {this.map.MappingType.Name}.");
            }

            if (predicate.Values == null)
            {
                throw new System.ArgumentNullException($"No list values passed for '{predicate.PropertyName.Value}'");
            }

            this.operationContext.Push(predicate);

            predicate.PropertyName.Accept(this);

            var propertyIsString = property.Property.PropertyType == typeof(string);
            var matchCase = predicate.Options?.MatchCase ?? true;
            var upper = propertyIsString == true && matchCase == false;
            var sqlValues = predicate.Values.Select(value => $"{(upper ? "UPPER(" : string.Empty)}{(propertyIsString ? value.SQLQuote() : value)}{(upper ? ")" : string.Empty)}");

            this.buffer.Append(" IN (");
            this.buffer.Append(string.Join(", ", sqlValues));
            this.buffer.Append(")");

            this.operationContext.Pop();
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
            this.operationContext.Push(predicate);

            predicate.PropertyName.Accept(this);
            this.buffer.Append($" LIKE ");
            predicate.Literal.Accept(this);

            this.operationContext.Pop();
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
            this.operationContext.Push(comparison);

            comparison.Left.Accept(this);
            this.buffer.Append($" {op} ");
            comparison.Right.Accept(this);

            this.operationContext.Pop();
        }

        private void Evaluate(PropertyName name, ComparisonOps operation)
        {
            if (operation is BinaryComparisonOp comparison)
            {
                this.Evaluate(name, comparison);
            }
            else if (operation is PropertyIsBetween)
            {
                this.Evaluate(name, true);
            }
            else if (operation is PropertyIsInList list)
            {
                this.Evaluate(name, list);
            }
            else if (operation is PropertyIsLike like)
            {
                this.Evaluate(name, like);
            }
            else
            {
                Debug.Assert(false, $"Unsupported literal context: {operation?.GetType().Name ?? "null reference"}");
            }
        }

        private void Evaluate(PropertyName name, BinaryComparisonOp operation)
        {
            this.Evaluate(name, operation.Options?.MatchCase ?? true);
        }

        private void Evaluate(PropertyName name, PropertyIsInList operation)
        {
            this.Evaluate(name, operation.Options?.MatchCase ?? true);
        }

        private void Evaluate(PropertyName name, PropertyIsLike operation)
        {
            this.Evaluate(name, operation.Options?.MatchCase ?? true);
        }

        private void Evaluate(PropertyName name, bool matchCase)
        {
            var property = this.map[name.Value];

            if (property == null)
            {
                throw new System.InvalidOperationException($"Invalid property name '{name.Value}' for {this.map.MappingType.Name}.");
            }

            var propertyIsString = property.Property.PropertyType == typeof(string);

            if (propertyIsString == true && matchCase == false)
            {
                this.buffer.Append("UPPER(");
            }

            this.buffer.Append(property.ToString());

            if (propertyIsString == true && matchCase == false)
            {
                this.buffer.Append(")");
            }
        }

        private void Evaluate(Literal literal, ComparisonOps operation)
        {
            if (operation is BinaryComparisonOp comparison)
            {
                this.Evaluate(literal, comparison);
            }
            else if (operation is PropertyIsBetween between)
            {
                this.Evaluate(literal, between);
            }
            else if (operation is PropertyIsLike like)
            {
                this.Evaluate(literal, like);
            }
            else
            {
                Debug.Assert(false, $"Unsupported literal context: {operation?.GetType().Name ?? "null reference"}");
            }
        }

        private void Evaluate(Literal literal, BinaryComparisonOp operation)
        {
            var other = operation.OtherSide(literal);

            if (other is PropertyName propertyName)
            {
                this.Evaluate(literal, propertyName, operation.Options?.MatchCase ?? true);
            }
            else
            {
                this.buffer.Append(literal.Value);
            }
        }

        private void Evaluate(Literal literal, PropertyIsLike operation)
        {
            this.Evaluate(literal, operation.PropertyName, operation.Options?.MatchCase ?? true);
        }

        private void Evaluate(Literal literal, PropertyIsBetween operation)
        {
            if (object.ReferenceEquals(literal, operation.Expression))
            {
                Debug.Assert(false, $"Cannot evaluate a {nameof(PropertyIsBetween)} comparison where the expression is not a {nameof(PropertyName)}");
                this.buffer.Append(literal.Value);
                return;
            }

            if (operation.Expression is PropertyName propertyName)
            {
                this.Evaluate(literal, propertyName, true);
            }
            else
            {
                throw new System.NotSupportedException($"Cannot evaluate a {nameof(PropertyIsBetween)} comparison where the expression is not a {nameof(PropertyName)}");
            }
        }

        private void Evaluate(Literal literal, PropertyName propertyName, bool matchCase)
        {
            var property = this.map[propertyName.Value];

            if (property == null)
            {
                throw new System.InvalidOperationException($"Invalid property name '{literal.Value}' for {this.map.MappingType.Name}.");
            }

            var propertyIsString = property.Property.PropertyType == typeof(string);

            if (propertyIsString == true && matchCase == false)
            {
                this.buffer.Append("UPPER(");
            }

            this.buffer.Append(propertyIsString ? literal.Value.SQLQuote() : literal.Value);

            if (propertyIsString == true && matchCase == false)
            {
                this.buffer.Append(")");
            }
        }
    }
}
