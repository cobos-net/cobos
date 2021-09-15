// ----------------------------------------------------------------------------
// <copyright file="SingletonException.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Wrappers
{
    using System;

    /// <summary>
    /// Exception for creating singletons.
    /// </summary>
    public class SingletonException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonException"/> class.
        /// </summary>
        /// <param name="message">The reason why the exception was raised.</param>
        public SingletonException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonException"/> class.
        /// </summary>
        /// <param name="inner">The inner exception.</param>
        public SingletonException(Exception inner)
            : base(inner.Message, inner)
        {
        }
    }
}