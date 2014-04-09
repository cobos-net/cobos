// ----------------------------------------------------------------------------
// <copyright file="ScriptingFramework.cs" company="Cobos SDK">
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

namespace Cobos.Script
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Singleton framework class.
    /// </summary>
    public partial class ScriptingFramework
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static ScriptingFramework instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="ScriptingFramework"/> class from being created.
        /// </summary>
        private ScriptingFramework()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the framework.
        /// </summary>
        public static ScriptingFramework Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScriptingFramework();
                }

                return instance;
            }
        }
    }

    /// <summary>
    /// Implements IDisposable.
    /// </summary>
    public partial class ScriptingFramework : IDisposable
    {
        /// <summary>
        /// Indicates whether the instance is disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Finalizes an instance of the <see cref="ScriptingFramework"/> class.
        /// </summary>
        ~ScriptingFramework()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose of the instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose of the current instance.
        /// </summary>
        /// <param name="disposing">Indicates whether the instance is disposing or finalizing.</param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            this.disposed = true;
        }
    }
}
