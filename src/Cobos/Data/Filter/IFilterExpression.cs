// ----------------------------------------------------------------------------
// <copyright file="IFilterExpression.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Interface specification specification of <see cref="IFilterExpression"/>.
    /// </summary>
    public interface IFilterExpression
    {
        /// <summary>
        /// Accept the visitor.
        /// </summary>
        /// <param name="visitor">The predicate visitor.</param>
        void Accept(IFilterExpressionVisitor visitor);
    }
}
