// ============================================================================
// Filename: CobosDataSet.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

// 05-Feb-11 N.Davis
// -----------------
// Rebranded from "Cobos" to "Intergraph.AsiaPac" in preparation for use in the Generic CAD Interoperability project

using System;
using System.Data;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using System.Diagnostics;

#if INTERGRAPH_BRANDING
using Intergraph.AsiaPac.Utilities.Xml;

namespace Intergraph.AsiaPac.Data
#else
using Cobos.Utilities.Xml;

namespace Cobos.Data
#endif
{
	public class CobosDataSet : DataSet
	{
		/// <summary>
		/// 
		/// </summary>
		public class Relationship
		{
			public readonly string Name;
			public readonly string ParentTable;
			public readonly string ParentColumn;
			public readonly string ChildTable;
			public readonly string ChildColumn;

			public Relationship( string name, string parentTable, string parentColumn, string childTable, string childColumn )
			{
				Name = name;
				ParentTable = parentTable;
				ParentColumn = parentColumn;
				ChildTable = childTable;
				ChildColumn = childColumn;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataSetName"></param>
		public CobosDataSet( string dataSetName )
			: base( dataSetName )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataSet"></param>
		public CobosDataSet()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="relationships"></param>
		public void CreateRelationships( Relationship[] relationships )
		{
			// create any relationships that may be required
			foreach ( Relationship relation in relationships )
			{
				DataTable parentTable = Tables[ relation.ParentTable ];
				DataTable childTable = Tables[ relation.ChildTable ];

				if ( parentTable == null || childTable == null )
				{
					continue;
				}

				DataColumn parentColumn = parentTable.Columns[ relation.ParentColumn ];
				DataColumn childColumn = childTable.Columns[ relation.ChildColumn ];

				if ( parentColumn == null || childColumn == null )
				{
					continue;
				}

				DataRelation dataRelation = new DataRelation( relation.Name, parentColumn, childColumn, false );
				dataRelation.Nested = true;

				Relations.Add( dataRelation );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void ClearRelationships()
		{
			Relations.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public XmlDocument ToXml()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( GetXml() );
			return doc;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="results"></param>
		public void ToXml( Stream results )
		{
			XmlDocument doc = ToXml();

			doc.Save( results );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xslt"></param>
		/// <param name="xsltArgs"></param>
		/// <param name="results"></param>
		public void ToXml( XslCompiledTransform xslt, XsltArgumentList xsltArgs, Stream results )
		{
			XmlDocument doc = ToXml();

			if ( xsltArgs == null )
			{
				xsltArgs = new XsltArgumentList();
			}

			//xsltArgs.AddParam( "dummy", "", dummy );

			// do the transform
			xslt.Transform( doc.CreateNavigator(), xsltArgs, results );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="xslt"></param>
		/// <param name="xsltArgs"></param>
		/// <returns></returns>
		public TObject ToObject<TObject>( XslCompiledTransform xslt, XsltArgumentList xsltArgs )
		{
			TObject result = default( TObject );

			XmlDocument doc = ToXml();

			if ( xsltArgs == null )
			{
				xsltArgs = new XsltArgumentList();
			}

			//xsltArgs.AddParam( "dummy", "", dummy );

			using ( MemoryStream stream = new MemoryStream() )
			{
				xslt.Transform( doc.CreateNavigator(), xsltArgs, stream );
				stream.Seek( 0, SeekOrigin.Begin );

				/*FileStream fstream = new FileStream( @"C:\temp\test.xml", FileMode.Create );
				stream.WriteTo( fstream );
				fstream.Close();
				stream.Seek( 0, SeekOrigin.Begin );*/

				result = XmlHelper<TObject>.Deserialize( stream );
			}

			return result;
		}
	}
}
