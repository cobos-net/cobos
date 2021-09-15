// ----------------------------------------------------------------------------
// <copyright file="SortBy.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Filter
{
    using Cobos.Utilities.Xml;

    /// <summary>
    /// Partial class implementation of <see cref="SortBy"/>.
    /// </summary>
    public partial class SortBy
    {
        /// <summary>
        /// Deserialize a sort by from a string representation.
        /// </summary>
        /// <param name="sortBy">The string representation of the sort by.</param>
        /// <returns>The deserialized entity.</returns>
        public static SortBy Deserialize(string sortBy)
        {
            return DataContractHelper.Deserialize<SortBy>(sortBy, null);
        }

        /// <summary>
        /// Accept the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(ISortVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// Partial class implementation of <see cref="SortProperty"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Common implementation for related types.")]
    public partial class SortProperty
    {
        /// <summary>
        /// Accept the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(ISortVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
