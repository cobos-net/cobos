// ----------------------------------------------------------------------------
// <copyright file="BinaryComparisonOp.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    /// <summary>
    /// Partial class implementation of <see cref="BinaryComparisonOp"/>.
    /// </summary>
    public abstract partial class BinaryComparisonOp
    {
        /// <summary>
        /// Returns the expression that is on the other side of the operation.
        /// </summary>
        /// <param name="expression">The expression on one side.</param>
        /// <returns>The other expression.</returns>
        public Expression OtherSide(Expression expression)
        {
            var isLhs = object.ReferenceEquals(this.Left, expression);
            var isRhs = object.ReferenceEquals(this.Right, expression);

            if ((isLhs || isRhs) == false)
            {
                throw new System.InvalidOperationException($"The expression {expression.Value} does not belong to this {this.GetType().Name} operation.");
            }

            return isLhs ? this.Right : this.Left;
        }
    }
}
