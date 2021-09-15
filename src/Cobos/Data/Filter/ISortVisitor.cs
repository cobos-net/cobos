// ----------------------------------------------------------------------------
// <copyright file="ISortVisitor.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Interface specification of <see cref="ISortVisitor"/>.
    /// </summary>
    public interface ISortVisitor
    {
        /// <summary>
        /// Visit the element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Visit(SortBy element);

        /// <summary>
        /// Visit the element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Visit(SortProperty element);
    }
}
