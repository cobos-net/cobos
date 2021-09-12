// ----------------------------------------------------------------------------
// <copyright file="ProgressReport.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Progress
{
    /// <summary>
    /// Report the current status of a long running operation.
    /// </summary>
    public class ProgressReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressReport"/> class
        /// to show indeterminate progress.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        public ProgressReport(string prompt)
        {
            this.IsIndeterminate = true;
            this.Prompt = prompt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressReport"/> class
        /// to show range based progress.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="value">The current progress value.</param>
        /// <param name="maximum">The maximum progress value.</param>
        public ProgressReport(string prompt, int value, int maximum)
        {
            this.IsIndeterminate = false;
            this.Prompt = prompt;
            this.Value = value;
            this.Maximum = maximum;
        }

        /// <summary>
        /// Gets the prompt to display.
        /// </summary>
        public string Prompt { get; }

        /// <summary>
        /// Gets a value indicating whether the current progress is indeterminate.
        /// </summary>
        public bool IsIndeterminate { get; }

        /// <summary>
        /// Gets the current progress value.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets the maximum progress value.
        /// </summary>
        public int Maximum { get; }
    }
}
