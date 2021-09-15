// ----------------------------------------------------------------------------
// <copyright file="IProgressController.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Progress
{
    /// <summary>
    /// Notify the progress of a long running operation.
    /// </summary>
    public interface IProgressController
    {
        /// <summary>
        /// Report the current progress.
        /// </summary>
        /// <param name="progress">The current progress report.</param>
        void ReportProgress(ProgressReport progress);
    }
}
