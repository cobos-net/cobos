// ----------------------------------------------------------------------------
// <copyright file="IFilterPredicate.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Interface specification specification of <see cref="IFilterPredicate"/>.
    /// </summary>
    public interface IFilterPredicate
    {
        /// <summary>
        /// Accept the visitor.
        /// </summary>
        /// <param name="visitor">The predicate visitor.</param>
        void Accept(IFilterPredicateVisitor visitor);
    }
}
