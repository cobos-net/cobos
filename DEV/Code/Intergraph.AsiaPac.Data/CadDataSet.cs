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
	using AsyncDBTask = AsyncTask<CadDataSet, DatabaseConnection.QueryDatabaseAsync>;

	public class CadDataSet
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
		public readonly DataSet Results;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataSetName"></param>
		public CadDataSet( string dataSetName )
		{
			Results = new DataSet( dataSetName );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataSet"></param>
		public CadDataSet( DataSet dataSet )
		{
			Results = dataSet;
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
				DataTable parentTable = Results.Tables[ relation.ParentTable ];
				DataTable childTable = Results.Tables[ relation.ChildTable ];

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

				Results.Relations.Add( dataRelation );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void ClearRelationships()
		{
			Results.Relations.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public XmlDocument ToXml()
		{
			Stopwatch timer = new Stopwatch();
			timer.Start();

			XmlDocument doc = new XmlDocument();

			//MemoryStream stream = new MemoryStream( 10 * (1024 * 1024) );
			//Results.WriteXml( stream );

			//stream.Seek(0, SeekOrigin.Begin);
			//StreamReader reader = new StreamReader(stream);

			//doc.LoadXml( reader.ReadToEnd() );

			doc.LoadXml( Results.GetXml() );

			timer.Stop();
			Debug.Print( "Dataset to Xml: {0}", timer.ElapsedMilliseconds );

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

			//Add the layer name
			//xsltArgs.AddParam( "layername", "", m_layerName );
			//xsltArgs.AddParam( "layerdescription", "", m_description );

			Stopwatch timer = new Stopwatch();
			timer.Start();

			//Do the transform
			xslt.Transform( doc.CreateNavigator(), xsltArgs, results );

			timer.Stop();
			Debug.Print( "Xslt Transform: {0}", timer.ElapsedMilliseconds );
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

			//Add the layer name
			//xsltArgs.AddParam( "layername", "", m_layerName );
			//xsltArgs.AddParam( "layerdescription", "", m_description );

			//Create a memory stream to write the converted data in to
			using ( MemoryStream stream = new MemoryStream() )
			{
				//Do the transform
				xslt.Transform( doc.CreateNavigator(), xsltArgs, stream );
				//Move back to the start of the memory stream
				stream.Seek( 0, SeekOrigin.Begin );

				/*FileStream fstream = new FileStream( @"C:\temp\tesst.xml", FileMode.Create );
				stream.WriteTo( fstream );
				fstream.Close();
				stream.Seek( 0, SeekOrigin.Begin );*/

				//Deserialize the XML file to the object      
				result = XmlHelper<TObject>.Deserialize( stream );
			}

			return result;
		}
	}
}
