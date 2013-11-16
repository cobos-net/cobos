// -----------------------------------------------------------------------
// <copyright file="Lazy.cs" company="Intergraph Corporation">
//
//        Copyright (c) 2011-2013 Intergraph Corporation
//                    All Rights Reserved
//
//        THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE
//                OF INTERGRAPH CORPORATION
//
//      The copyright notice above does not evidence any actual
//          or intended publication of such source code
//
// </copyright>
// -----------------------------------------------------------------------

namespace Cobos.Utilities.Wrappers
{
    using System;

    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <remarks>
    /// From .NET 4.0 onwards use System.Lazy{T}.
    /// </remarks>
    [System.Diagnostics.DebuggerStepThrough]
    public sealed class Lazy<T>
    {
        /// <summary>
        /// Provide thread-safe initialization by double locking.
        /// </summary>
        private readonly object padlock = new object();

        /// <summary>
        /// Delegate to call to create the value.
        /// </summary>
        private readonly Func<T> createValue;

        /// <summary>
        /// Test whether the value has been created.
        /// </summary>
        private bool isValueCreated;

        /// <summary>
        /// The value held by the lazy initializer.
        /// </summary>
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="createValue">The delegate that produces the value when it is needed.</param>
        public Lazy(Func<T> createValue)
        {
            if (createValue == null)
            {
                throw new ArgumentNullException("You must provide a createValue delegate.");
            }

            this.createValue = createValue;
        }

        /// <summary>
        /// Gets the lazily initialized value of the current Lazy{T} instance.
        /// </summary>
        public T Value
        {
            get
            {
                if (!this.isValueCreated)
                {
                    lock (this.padlock)
                    {
                        if (!this.isValueCreated)
                        {
                            this.value = this.createValue();
                            this.isValueCreated = true;
                        }
                    }
                }

                return this.value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a value has been created for this Lazy{T} instance.
        /// </summary>
        public bool IsValueCreated
        {
            get
            {
                lock (this.padlock)
                {
                    return this.isValueCreated;
                }
            }
        }

        /// <summary>
        /// Creates and returns a string representation of the Lazy{T}.Value.
        /// </summary>
        /// <returns>The string representation of the Lazy{T}.Value property.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
