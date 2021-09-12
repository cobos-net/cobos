// ----------------------------------------------------------------------------
// <copyright file="BinaryLogicOp.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    using System.Linq;

    /// <summary>
    /// Partial class implementation of <see cref="BinaryLogicOp"/>.
    /// </summary>
    public abstract partial class BinaryLogicOp
    {
        /// <summary>
        /// Create a Binary Logic operation from a list of literal values.
        /// </summary>
        /// <typeparam name="TLogic">The binary Logic type.</typeparam>
        /// <typeparam name="TComparison">The binary Comparison type.</typeparam>
        /// <typeparam name="TLiteral">The literal Type.</typeparam>
        /// <param name="valueReference">The value reference name.</param>
        /// <param name="values">The list of literal values.</param>
        /// <returns>The constructed Binary Logic operation.</returns>
        public static TLogic FromList<TLogic, TComparison, TLiteral>(string valueReference, global::System.Collections.Generic.List<TLiteral> values)
            where TLogic : BinaryLogicOp, new()
            where TComparison : BinaryComparisonOp, new()
        {
            if (values == null || values.Count == 0)
            {
                return null;
            }

            var comparisons = values.Select(v => new TComparison() { ValueReference = valueReference, Literal = v.ToString() }).ToList();

            var logical = new TLogic
            {
                Predicate = new BinaryLogicOp.PredicateType(),
            };
            logical.Predicate.AddRange(comparisons);

            return logical;
        }
    }
}
