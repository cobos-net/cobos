// ----------------------------------------------------------------------------
// <copyright file="DataQueryNode.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Cobos.Data.Filter;

    /// <summary>
    /// Represents a hierarchical relationship between parent and child queries.
    /// </summary>
    public class DataQueryNode
    {
        /// <summary>
        /// Represents the query method.
        /// </summary>
        private readonly Action<Filter, SortBy> query;

        /// <summary>
        /// Represents the dependent child queries.
        /// </summary>
        private readonly List<ChildRelationship> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataQueryNode"/> class.
        /// </summary>
        /// <param name="query">The query logic.</param>
        public DataQueryNode(Action<Filter, SortBy> query)
        {
            this.query = query ?? throw new ArgumentNullException();
            this.children = new List<ChildRelationship>();
        }

        /// <summary>
        /// Add a child relationship.
        /// </summary>
        /// <param name="node">The node with the relationship.</param>
        /// <param name="parentKey">The key for the parent.</param>
        /// <param name="childKey">The key for the child.</param>
        public void AddChild(DataQueryNode node, string parentKey, string childKey)
        {
            this.children.Add(new ChildRelationship(node, parentKey, childKey));
        }

        /// <summary>
        /// Execute the query on this node and all children.
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
            /// Initializes a new instance of the <see cref="ChildRelationship"/> class.
            /// </summary>
            /// <param name="node">The node with the relationship.</param>
            /// <param name="parentKey">The key for the parent.</param>
            /// <param name="childKey">The key for the child.</param>
            public ChildRelationship(DataQueryNode node, string parentKey, string childKey)
            {
                this.Node = node;
                this.ParentKey = parentKey;
                this.ChildKey = childKey;
            }

            /// <summary>
            /// Gets the node.
            /// </summary>
            public DataQueryNode Node { get; }

            /// <summary>
            /// Gets the key for the parent.
            /// </summary>
            public string ParentKey { get; }

            /// <summary>
            /// Gets the key for the child.
            /// </summary>
            public string ChildKey { get; }
        }
    }
}
