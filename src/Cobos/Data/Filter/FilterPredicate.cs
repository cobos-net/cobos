// ----------------------------------------------------------------------------
// <copyright file="FilterPredicate.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Partial class implementation of <see cref="FilterPredicate"/>.
    /// </summary>
    public abstract partial class FilterPredicate : IFilterPredicate
    {
        /// <summary>
        /// Accept the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(IFilterPredicateVisitor visitor);
    }
}
