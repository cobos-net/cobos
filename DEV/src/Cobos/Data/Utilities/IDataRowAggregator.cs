// ----------------------------------------------------------------------------
// <copyright file="IDataRowAggregator.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Utilities
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Client provided custom row aggregator.
    /// </summary>
    public interface IDataRowAggregator : Cobos.Data.Transforms.IDataTableTransform
    {
        /// <summary>
        /// Perform custom aggregation on the row group.
        /// The row collection must contain at least two rows.
        /// </summary>
        /// <param name="rows">The rows to aggregate.</param>
        /// <param name="result">The resultant aggregated row.</param>
        void Aggregate(List<DataRow> rows, DataRow result);

        /// <summary>
        /// Rows are aggregated based on key values.  Clients must
        /// provide their own custom key generation behavior.
        /// </summary>
        /// <param name="row">The row to generate the key for.</param>
        /// <returns>The key associated with this row.</returns>
        string GetKey(DataRow row);
    }
}
