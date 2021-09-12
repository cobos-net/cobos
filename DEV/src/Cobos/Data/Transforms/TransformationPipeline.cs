// ----------------------------------------------------------------------------
// <copyright file="TransformationPipeline.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Transforms
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;

    /// <summary>
    /// A transformation pipeline for a DataTable.
    /// </summary>
    public class TransformationPipeline
    {
        /// <summary>
        /// The transforms for the pipeline.
        /// </summary>
        private readonly List<IDataTableTransform> transforms;

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
