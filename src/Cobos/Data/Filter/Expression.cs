// ----------------------------------------------------------------------------
// <copyright file="Expression.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Partial class implementation of <see cref="Expression"/>.
    /// </summary>
    public abstract partial class Expression : IFilterExpression
    {
        /// <inheritdoc />
        public abstract void Accept(IFilterExpressionVisitor visitor);
    }
}
