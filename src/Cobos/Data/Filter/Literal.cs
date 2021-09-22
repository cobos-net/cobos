// ----------------------------------------------------------------------------
// <copyright file="Literal.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Partial class implementation of <see cref="Literal"/>.
    /// </summary>
    public partial class Literal
    {
        /// <inheritdoc />
        public override void Accept(IFilterExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
