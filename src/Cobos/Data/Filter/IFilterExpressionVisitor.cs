// ----------------------------------------------------------------------------
// <copyright file="IFilterExpressionVisitor.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Vistor for a <see cref="IFilterExpression"/>.
    /// </summary>
    public interface IFilterExpressionVisitor
    {
        /// <summary>
        /// Visit the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        void Visit(PropertyName expression);

        /// <summary>
        /// Visit the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        void Visit(Literal expression);
    }
}
