// ----------------------------------------------------------------------------
// <copyright file="GenericWrapper.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
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
        private T objectRef = default;

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
        /// <typeparam name="TCast">The type to cast to.</typeparam>
        /// <returns>A reference to the object; otherwise null if the types are unrelated.</returns>
        public TCast Cast<TCast>()
            where TCast : class
        {
            return this.objectRef as TCast;
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
                if (this.objectRef is IDisposable dispose)
                {
                    dispose.Dispose();
                }

                this.objectRef = default;

                GC.SuppressFinalize(this);
            }

            this.disposed = true;
        }
    }
}
