// ----------------------------------------------------------------------------
// <copyright file="PropertyIsGreaterThan.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsGreaterThan"/>.
    /// </summary>
    public partial class PropertyIsGreaterThan
    {
        /// <summary>
        /// Accept the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Accept(IFilterPredicateVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
