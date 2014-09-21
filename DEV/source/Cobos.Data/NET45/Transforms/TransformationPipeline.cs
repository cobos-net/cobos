// ----------------------------------------------------------------------------
// <copyright file="TransformationPipeline.cs" company="Cobos SDK">
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

namespace Cobos.Data.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;

    /// <summary>
    /// A transformation pipeline for a DataTable
    /// </summary>
    public class TransformationPipeline
    {
        /// <summary>
        /// The transforms for the pipeline.
        /// </summary>
        private List<IDataTableTransform> transforms;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationPipeline"/> class.
        /// </summary>
        /// <param name="transforms">The transforms for the pipeline.</param>
        public TransformationPipeline(IEnumerable<IDataTableTransform> transforms)
        {
            this.transforms = new List<IDataTableTransform>(transforms);
        }

        /// <summary>
        /// Gets the transforms in the pipeline.
        /// </summary>
        public ReadOnlyCollection<IDataTableTransform> Transforms
        {
            get
            {
                return this.transforms.AsReadOnly();
            }
        }

        /// <summary>
        /// Execute the pipeline for the input table.
        /// </summary>
        /// <param name="dataTable">The input table.</param>
        /// <returns>The pipeline result.</returns>
        public DataTable Execute(DataTable dataTable)
        {
            foreach (var transform in this.transforms)
            {
                dataTable = transform.Transform(dataTable);
            }

            return dataTable;
        }
    }
}
