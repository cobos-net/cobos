// ----------------------------------------------------------------------------
// <copyright file="IResourceAllocator.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Threading.Resource
{
    /// <summary>
    /// User defined resource allocator to enable the resource pool
    /// to dynamically add resources on demand.
    /// </summary>
    /// <typeparam name="T">The type of the resource to create.</typeparam>
    public interface IResourceAllocator<T>
    {
        /// <summary>
        /// Create a resource.
        /// </summary>
        /// <returns>The newly created resource.</returns>
        T Create();
    }

    /// <summary>
    /// Convenience default allocator for objects with default or parameter-less constructors.
    /// </summary>
    /// <typeparam name="T">The type of resource to be created.</typeparam>
    public class ResourceDefaultAllocator<T> : IResourceAllocator<T>
        where T : new()
    {
        /// <summary>
        /// Create a resource.
        /// </summary>
        /// <returns>The newly created resource.</returns>
        public T Create()
        {
            return new T();
        }
    }
}
