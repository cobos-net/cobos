// ----------------------------------------------------------------------------
// <copyright file="ProgressReport.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Threading.Progress
{
    /// <summary>
    /// Report the current status of a long running operation.
    /// </summary>
    public class ProgressReport
    {
        /// <summary>
        /// The prompt to display.
        /// </summary>
        public readonly string Prompt;

        /// <summary>
        /// Indicates that the current progress is indeterminate.
        /// </summary>
        public readonly bool IsIndeterminate;

        /// <summary>
        /// The current progress value.
        /// </summary>
        public readonly int Value;

        /// <summary>
        /// The maximum progress value.
        /// </summary>
        public readonly int Maximum;

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
    }
}
