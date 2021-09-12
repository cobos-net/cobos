// ----------------------------------------------------------------------------
// <copyright file="IDataTableTransform.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Transforms
{
    using System.Data;

    /// <summary>
    /// Specification for a DataTable transform.
    /// </summary>
    public interface IDataTableTransform
    {
        /// <summary>
        /// Transform a DataTable and return the result as a new DataTable.
        /// </summary>
        /// <param name="table">The input DataTable.</param>
        /// <returns>The transformed result.</returns>
        DataTable Transform(DataTable table);
    }
}
