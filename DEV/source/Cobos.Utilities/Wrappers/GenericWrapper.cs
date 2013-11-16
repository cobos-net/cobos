// ----------------------------------------------------------------------------
// <copyright file="GenericWrapper.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Wrappers
{
    using System;

    /// <summary>
    /// Can be used to implement generic classes that are designed to be used with
    /// unrelated reference types that cannot satisfy a 'where' constraint with
    /// multiple definitions.
    /// </summary>
    /// <typeparam name="T">The type contained in the wrapper.</typeparam>
    public class GenericWrapper<T> : IDisposable
    {
        /// <summary>
        /// The inner object reference.
        /// </summary>
        private T objectRef = default(T);

        /// <summary>
        /// Indicates whether this instance is disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericWrapper{T}"/> class.
        /// </summary>
        /// <param name="obj">The object reference contained within the wrapper.</param>
        public GenericWrapper(T obj)
        {
            this.objectRef = obj;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GenericWrapper{T}"/> class.
        /// </summary>
        ~GenericWrapper()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Cast the object to a related type.
        /// </summary>
        /// <typeparam name="C">The type to cast to.</typeparam>
        /// <returns>A reference to the object; otherwise null if the types are unrelated.</returns>
        public C Cast<C>() where C : class
        {
            return this.objectRef as C;
        }

        /// <summary>
        /// Disposes of the instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes of the instance.
        /// </summary>
        /// <param name="disposing">true if the object is disposing; otherwise false if the object is finalizing.</param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                IDisposable dispose = this.objectRef as IDisposable;

                if (dispose != null)
                {
                    dispose.Dispose();
                }

                this.objectRef = default(T);

                GC.SuppressFinalize(this);
            }

            this.disposed = true;
        }
    }
}
