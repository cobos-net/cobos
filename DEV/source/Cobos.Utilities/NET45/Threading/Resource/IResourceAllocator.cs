// ----------------------------------------------------------------------------
// <copyright file="IResourceAllocator.cs" company="Cobos SDK">
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
