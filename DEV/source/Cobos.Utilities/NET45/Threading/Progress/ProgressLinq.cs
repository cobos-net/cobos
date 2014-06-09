// ----------------------------------------------------------------------------
// <copyright file="ProgressLinq.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Threading.Progress
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

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
