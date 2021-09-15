// ----------------------------------------------------------------------------
// <copyright file="IProgressControllerCancellable.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Progress
{
    using System.Threading;

    /// <summary>
    /// Interface for cancellable long running operation.
    /// </summary>
    public interface IProgressControllerCancellable : IProgressController
    {
        /// <summary>
        /// Gets the cancel token for the long running operation.
        /// </summary>
        CancellationToken Cancel
        {
            get;
        }
    }
}
