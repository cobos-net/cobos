// ============================================================================
// Filename: HttpXmlHelper.cs
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
using System.Text;
using System.Web;
using Cobos.Utilities.Xml;

namespace Cobos.Web.Utilities.Xml
{
	public static class HttpXmlHelper<T>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="context"></param>
		public static void Serialize( T entity, HttpContext context )
		{
			//Setup the response
			context.Response.ContentType = "text/xml";
			context.Response.Cache.SetNoStore();
			context.Response.Cache.SetNoServerCaching();
			context.Response.ContentEncoding = Encoding.UTF8;
			context.Response.Cache.SetCacheability( HttpCacheability.NoCache );

			XmlHelper<T>.Serialize( entity, context.Response.OutputStream );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static T Deserialize( HttpContext context )
		{
			return XmlHelper<T>.Deserialize( context.Request.InputStream );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="response"></param>
		/// <param name="prefix"></param>
		/// <param name="ns"></param>
		public static void Serialize( HttpContext context, T response, string prefix, string ns )
		{
			// Setup the response
			context.Response.ContentType = "text/xml";
			context.Response.Cache.SetNoStore();

			XmlHelper<T>.Serialize( context.Response.OutputStream, response, prefix, ns );
		}
	}
}
