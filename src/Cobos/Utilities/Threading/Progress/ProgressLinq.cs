// ----------------------------------------------------------------------------
// <copyright file="ProgressLinq.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Progress
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Extension methods for progress controllers.
    /// </summary>
    public static class ProgressLinq
    {
        /// <summary>
        /// Perform a cancellable progress action on a list.
        /// </summary>
        /// <typeparam name="T">The type of the object to process.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="action">The action to perform on each object in the list.</param>
        /// <param name="progress">The progress report.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="prompt">The status prompt for the progress action.</param>
        public static void ForEachProgress<T>(this IList<T> self, Action<T> action, IProgress<ProgressReport> progress, CancellationToken cancel, string prompt)
        {
            int count = self.Count;

            for (int i = 0; i < count; ++i)
            {
                var current = string.Format("{0} ({1} of {2})", prompt, i + 1, count);

                progress.Report(new ProgressReport(current, i, count));

                action(self[i]);

                cancel.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Perform a non-cancellable progress action on a list.
        /// </summary>
        /// <typeparam name="T">The type of the object to process.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="action">The action to perform on each object in the list.</param>
        /// <param name="progress">The progress report.</param>
        /// <param name="prompt">The status prompt for the progress action.</param>
        public static void ForEachProgress<T>(this IList<T> self, Action<T> action, IProgress<ProgressReport> progress, string prompt)
        {
            int count = self.Count;

            for (int i = 0; i < count; ++i)
            {
                var current = string.Format("{0} ({1} of {2})", prompt, i, count);

                progress.Report(new ProgressReport(current, i, count));

                action(self[i]);
            }
        }
    }
}
