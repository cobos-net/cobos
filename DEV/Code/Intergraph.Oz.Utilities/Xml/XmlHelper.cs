using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web;

namespace Intergraph.Oz.Utilities.Xml
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class XmlHelper<T>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static void Deserialize( out T entity, string filename )
		{
			entity = default(T);

			XmlSerializer s = new XmlSerializer( typeof( T ) );
			
			using ( TextReader r = new StreamReader( filename ) )
			{
				entity = (T)s.Deserialize( r );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static void Serialize( T entity, string filename )
		{
			XmlSerializer s = new XmlSerializer( typeof( T ) );
			
			using ( TextWriter w = new StreamWriter( filename ) )
			{
				s.Serialize( w, entity );
			}
		}

		public static void Serialize( T entity, Stream stream )
		{
			XmlSerializer serializer = new XmlSerializer( typeof( T ) );
			serializer.Serialize( stream, entity );
		}

		public static T Deserialize( Stream stream )
		{
			XmlSerializer serializer = new XmlSerializer( typeof( T ) );
			return (T)serializer.Deserialize( stream );
		}

		public static void Serialize( Stream output, T response, string prefix, string ns )
		{
			XmlSerializerNamespaces nsSerializer = null;

			// Now we must write the response to the stream
			if ( prefix != null && ns != null )
			{
				nsSerializer = new XmlSerializerNamespaces();
				nsSerializer.Add( prefix, ns );
			}

			// Create the serializer and write to the stream
			XmlSerializer serializer = new XmlSerializer( typeof( T ) );

			if ( nsSerializer != null )
			{
				serializer.Serialize( output, response, nsSerializer );
			}
			else
			{
				serializer.Serialize( output, response );
			}
		}
	}
}
