// ============================================================================
// Filename: XmlHelper.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web;

namespace Cobos.Utilities.Xml
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
