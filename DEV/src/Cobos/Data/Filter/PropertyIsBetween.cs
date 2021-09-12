// ----------------------------------------------------------------------------
// <copyright file="PropertyIsBetween.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsBetween"/>.
    /// </summary>
    public partial class PropertyIsBetween
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
