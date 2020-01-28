// ----------------------------------------------------------------------------
// <copyright file="IDataRowAggregator.cs" company="Cobos SDK">
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

namespace Cobos.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Client provided custom row aggregator
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
