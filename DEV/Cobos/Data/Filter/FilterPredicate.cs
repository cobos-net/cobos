// ----------------------------------------------------------------------------
// <copyright file="FilterPredicate.cs" company="Cobos SDK">
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
    using System.Collections.Generic;
    using System.Linq;

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

    /// <summary>
    /// Partial class implementation of <see cref="ComparisonOps"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Common implementation for related types.")]
    public abstract partial class ComparisonOps
    {
    }

    /// <summary>
    /// Partial class implementation of <see cref="BinaryComparisonOp"/>.
    /// </summary>
    public abstract partial class BinaryComparisonOp
    {
    }

    /// <summary>
    /// Partial class implementation of <see cref="LogicOps"/>.
    /// </summary>
    public abstract partial class LogicOps
    {
    }

    /// <summary>
    /// Partial class implementation of <see cref="BinaryLogicOp"/>.
    /// </summary>
    public abstract partial class BinaryLogicOp
    {
        /// <summary>
        /// Create a Binary Logic operation from a list of literal values.
        /// </summary>
        /// <typeparam name="L">The binary Logic type.</typeparam>
        /// <typeparam name="C">The binary Comparison type.</typeparam>
        /// <typeparam name="T">The literal Type.</typeparam>
        /// <param name="valueReference">The value reference name.</param>
        /// <param name="values">The list of literal values.</param>
        /// <returns>The constructed Binary Logic operation.</returns>
        public static L FromList<L, C, T>(string valueReference, global::System.Collections.Generic.List<T> values)
            where L : BinaryLogicOp, new()
            where C : BinaryComparisonOp, new()
        {
            if (values == null || values.Count == 0)
            {
                return null;
            }

            var comparisons = values.Select(v => new C() { ValueReference = valueReference, Literal = v.ToString() }).ToList();

            var logical = new L();
            logical.Predicate = new BinaryLogicOp.PredicateType();
#if NET35
            logical.Predicate.AddRange(comparisons.Cast<FilterPredicate>());
#else
            logical.Predicate.AddRange(comparisons);
#endif

            return logical;
        }
    }

    /// <summary>
    /// Partial class implementation of <see cref="UnaryLogicOp"/>.
    /// </summary>
    public abstract partial class UnaryLogicOp
    {
    }

    /// <summary>
    /// Partial class implementation of <see cref="And"/>.
    /// </summary>
    public partial class And
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

    /// <summary>
    /// Partial class implementation of <see cref="Or"/>.
    /// </summary>
    public partial class Or
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

    /// <summary>
    /// Partial class implementation of <see cref="Not"/>.
    /// </summary>
    public partial class Not : IFilterPredicate
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

    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsEqualTo"/>.
    /// </summary>
    public partial class PropertyIsEqualTo
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

    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsGreaterThanOrEqualTo"/>.
    /// </summary>
    public partial class PropertyIsGreaterThanOrEqualTo
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

    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsLessThan"/>.
    /// </summary>
    public partial class PropertyIsLessThan
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

    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsLessThanOrEqualTo"/>.
    /// </summary>
    public partial class PropertyIsLessThanOrEqualTo
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

    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsLike"/>.
    /// </summary>
    public partial class PropertyIsLike
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

    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsNotEqualTo"/>.
    /// </summary>
    public partial class PropertyIsNotEqualTo
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

    /// <summary>
    /// Partial class implementation of <see cref="PropertyIsNull"/>.
    /// </summary>
    public partial class PropertyIsNull
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
