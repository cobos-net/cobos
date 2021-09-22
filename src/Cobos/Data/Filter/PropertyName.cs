// ----------------------------------------------------------------------------
// <copyright file="PropertyName.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Property Name expression.
    /// </summary>
    public partial class PropertyName
    {
        /// <inheritdoc />
        public override void Accept(IFilterExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
