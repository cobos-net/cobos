// ----------------------------------------------------------------------------
// <copyright file="IFilterPredicateVisitor.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    using System;

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
