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
        /// <param name="propertyName">The value reference name.</param>
        /// <param name="values">The list of literal values.</param>
        /// <returns>The constructed Binary Logic operation.</returns>
        public static FilterPredicate Compose<TLogic, TComparison, TLiteral>(string propertyName, global::System.Collections.Generic.IEnumerable<TLiteral> values)
            where TLogic : BinaryLogicOp, new()
            where TComparison : BinaryComparisonOp, new()
        {
            if (values?.Any() != true)
            {
                return null;
            }

            var comparisons = values.Select(v =>
            new TComparison()
            {
                Left = new PropertyName { Value = propertyName },
                Right = new Literal { Value = v.ToString() },
            }).ToList();

            if (comparisons.Count == 1)
            {
                return comparisons[0];
            }

            var logic = new TLogic();
            var current = logic;
            current.Condition1 = comparisons[0];

            for (int i = 1; i < comparisons.Count; ++i)
            {
                if (i == comparisons.Count - 1)
                {
                    current.Condition2 = comparisons[i];
                }
                else
                {
                    current.Condition2 = new TLogic();
                    current = (TLogic)current.Condition2;
                    current.Condition1 = comparisons[i];
                }
            }

            return logic;
        }
    }
}
