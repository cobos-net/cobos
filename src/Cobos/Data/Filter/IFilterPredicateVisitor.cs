// ----------------------------------------------------------------------------
// <copyright file="IFilterPredicateVisitor.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Interface specification of <see cref="IFilterPredicateVisitor"/>.
    /// </summary>
    public interface IFilterPredicateVisitor
    {
        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(And predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(Or predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(Not predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsBetween predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsEqualTo predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsGreaterThan predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsGreaterThanOrEqualTo predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsLessThan predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsLessThanOrEqualTo predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsLike predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsNotEqualTo predicate);

        /// <summary>
        /// Visit the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void Visit(PropertyIsNull predicate);
    }
}
