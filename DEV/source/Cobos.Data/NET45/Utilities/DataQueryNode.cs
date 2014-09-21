// ----------------------------------------------------------------------------
// <copyright file="DataTableTreeNode.cs" company="Cobos SDK">
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

namespace Cobos.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cobos.Data.Filter;
using System.Data;

    /// <summary>
    /// Represents a hierarchical relationship between parent and child queries.
    /// </summary>
    public class DataQueryNode
    {
        /// <summary>
        /// Represents the query method.
        /// </summary>
        private Action<Filter, SortBy> query;

        /// <summary>
        /// Represents the dependent child queries.
        /// </summary>
        private List<ChildRelationship> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataQueryNode"/> class.
        /// </summary>
        public DataQueryNode(Action<Filter, SortBy> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException();
            }

            this.query = query;
            this.children = new List<ChildRelationship>();
        }

        /// <summary>
        /// Add a child relationship.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentKey"></param>
        /// <param name="childKey"></param>
        public void AddChild(DataQueryNode node, string parentKey, string childKey)
        {
            this.children.Add(new ChildRelationship(node, parentKey, childKey));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="sort">The sort expression.</param>
        public void Query(Filter filter, SortBy sort)
        {
            this.query(filter, sort);

            int count = this.children.Count;

            if (count == 0)
            {
                return;
            }

            var queries = (from c in this.children 
                           select (Action<Filter, SortBy>)c.Node.Query).ToArray(); 
            
            var results = new IAsyncResult[count];

            for (int c = 0; c < count; ++c)
            {
                results[c] = queries[c].BeginInvoke(null, null, null, null);
            }

            for (int c = 0; c < count; ++c)
            {
                queries[c].EndInvoke(results[c]);
            }
        }

        /// <summary>
        /// Represents a child relationship with another node.
        /// </summary>
        private class ChildRelationship
        {
            /// <summary>
            /// 
            /// </summary>
            public readonly DataQueryNode Node;

            /// <summary>
            /// 
            /// </summary>
            public readonly string ParentKey;

            /// <summary>
            /// 
            /// </summary>
            public readonly string ChildKey;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="parentKey"></param>
            /// <param name="childKey"></param>
            public ChildRelationship(DataQueryNode node, string parentKey, string childKey)
            {
                this.Node = node;
                this.ParentKey = parentKey;
                this.ChildKey = childKey;
            }
        }
    }
}
