using System;
using System.Data;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using System.Diagnostics;
using Intergraph.AsiaPac.Utilities.Xml;

namespace Intergraph.AsiaPac.Data
{
	using AsyncDBTask = AsyncTask<CadDataSet, DatabaseAdapter.QueryDatabaseAsync>;

	public class CadDataSet : DataSet
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
		public CadDataSet( string dataSetName )
			: base( dataSetName )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataSet"></param>
		public CadDataSet()
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
#if DEBUG
			Stopwatch timer = new Stopwatch();
			timer.Start();
#endif

			XmlDocument doc = new XmlDocument();
			doc.LoadXml( GetXml() );

#if DEBUG
			timer.Stop();
			Debug.Print( "Dataset to Xml: {0}", timer.ElapsedMilliseconds );
#endif

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

			// add the layer name
			//xsltArgs.AddParam( "layername", "", m_layerName );
			//xsltArgs.AddParam( "layerdescription", "", m_description );

#if DEBUG
			Stopwatch timer = new Stopwatch();
			timer.Start();
#endif
			//Do the transform
			xslt.Transform( doc.CreateNavigator(), xsltArgs, results );

#if DEBUG
			timer.Stop();
			Debug.Print( "Xslt Transform: {0}", timer.ElapsedMilliseconds );
#endif
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

			//xsltArgs.AddParam( "layername", "", m_layerName );
			//xsltArgs.AddParam( "layerdescription", "", m_description );

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
