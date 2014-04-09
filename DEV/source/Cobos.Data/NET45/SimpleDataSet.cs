// ----------------------------------------------------------------------------
// <copyright file="SimpleDataSet.cs" company="Cobos SDK">
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

namespace Cobos.Data
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Xsl;
    using Cobos.Utilities.Xml;

    /// <summary>
    /// A simple extension to the <see cref="DataSet"/> class.
    /// </summary>
    public class SimpleDataSet : DataSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDataSet"/> class.
        /// </summary>
        public SimpleDataSet()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDataSet"/> class.
        /// </summary>
        /// <param name="dataSetName">The name of the data set.</param>
        public SimpleDataSet(string dataSetName)
            : base(dataSetName)
        {
        }

        /// <summary>
        /// Create the specified relationships in the data set.
        /// </summary>
        /// <param name="relationships">The relationships to create.</param>
        public void CreateRelationships(Relationship[] relationships)
        {
            // create any relationships that may be required
            foreach (Relationship relation in relationships)
            {
                DataTable parentTable = Tables[relation.ParentTable];
                DataTable childTable = Tables[relation.ChildTable];

                if (parentTable == null || childTable == null)
                {
                    continue;
                }

                DataColumn parentColumn = parentTable.Columns[relation.ParentColumn];
                DataColumn childColumn = childTable.Columns[relation.ChildColumn];

                if (parentColumn == null || childColumn == null)
                {
                    continue;
                }

                DataRelation dataRelation = new DataRelation(relation.Name, parentColumn, childColumn, false);
                dataRelation.Nested = true;

                Relations.Add(dataRelation);
            }
        }

        /// <summary>
        /// Clear all data set relationships.
        /// </summary>
        public void ClearRelationships()
        {
            Relations.Clear();
        }

        /// <summary>
        /// Serialize the data set to XML.
        /// </summary>
        /// <returns>An XML document representing the contents of the dataset.</returns>
        public XmlDocument ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.GetXml());
            return doc;
        }

        /// <summary>
        /// Serialize the data set to XML.
        /// </summary>
        /// <param name="result">The stream to write the result to.</param>
        public void ToXml(Stream result)
        {
            XmlDocument doc = this.ToXml();
            doc.Save(result);
        }

        /// <summary>
        /// Serialize the data set to XML.
        /// </summary>
        /// <param name="xslt">The transform to use.  May be null.</param>
        /// <param name="xsltArgs">The arguments for the transform.</param>
        /// <param name="result">The stream to write the result to.</param>
        public void ToXml(XslCompiledTransform xslt, XsltArgumentList xsltArgs, Stream result)
        {
            XmlDocument doc = this.ToXml();

            if (xsltArgs == null)
            {
                xsltArgs = new XsltArgumentList();
            }

            ////xsltArgs.AddParam( "example", "", example );

            // do the transform
            xslt.Transform(doc.CreateNavigator(), xsltArgs, result);
        }

        /// <summary>
        /// Serialize the data set to an object.
        /// </summary>
        /// <typeparam name="TObject">The type of object to serialize to.</typeparam>
        /// <param name="xslt">The transform to use.  May be null.</param>
        /// <param name="xsltArgs">The arguments for the transform.</param>
        /// <returns>The serialized object.</returns>
        public TObject ToObject<TObject>(XslCompiledTransform xslt, XsltArgumentList xsltArgs)
        {
            TObject result = default(TObject);

            XmlDocument doc = this.ToXml();

            if (xsltArgs == null)
            {
                xsltArgs = new XsltArgumentList();
            }

            ////xsltArgs.AddParam( "example", "", example );

            using (MemoryStream stream = new MemoryStream())
            {
                xslt.Transform(doc.CreateNavigator(), xsltArgs, stream);
                stream.Seek(0, SeekOrigin.Begin);

                ////FileStream fstream = new FileStream( @"C:\temp\test.xml", FileMode.Create );
                ////stream.WriteTo( fstream );
                ////fstream.Close();
                ////stream.Seek( 0, SeekOrigin.Begin );

                result = XmlHelper<TObject>.Deserialize(stream);
            }

            return result;
        }

        /// <summary>
        /// Represents a relationship in the data set.
        /// </summary>
        public class Relationship
        {
            /// <summary>
            /// The name of the relationship.
            /// </summary>
            public readonly string Name;
            
            /// <summary>
            /// The name of the parent table.
            /// </summary>
            public readonly string ParentTable;
            
            /// <summary>
            /// The name of the column in the parent table.
            /// </summary>
            public readonly string ParentColumn;
            
            /// <summary>
            /// The name of the child table.
            /// </summary>
            public readonly string ChildTable;
            
            /// <summary>
            /// The name of the column in the child table.
            /// </summary>
            public readonly string ChildColumn;

            /// <summary>
            /// Initializes a new instance of the <see cref="Relationship"/> class.
            /// </summary>
            /// <param name="name">The name of the relationship.</param>
            /// <param name="parentTable">The name of the parent table.</param>
            /// <param name="parentColumn">The name of the column in the parent table.</param>
            /// <param name="childTable">The name of the child table.</param>
            /// <param name="childColumn">The name of the column in the child table.</param>
            public Relationship(string name, string parentTable, string parentColumn, string childTable, string childColumn)
            {
                this.Name = name;
                this.ParentTable = parentTable;
                this.ParentColumn = parentColumn;
                this.ChildTable = childTable;
                this.ChildColumn = childColumn;
            }
        }
    }
}
