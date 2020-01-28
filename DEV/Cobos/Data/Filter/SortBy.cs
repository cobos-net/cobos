// ----------------------------------------------------------------------------
// <copyright file="SortBy.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
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
